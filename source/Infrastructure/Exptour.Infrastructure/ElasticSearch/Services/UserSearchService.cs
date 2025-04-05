using Exptour.Application.DTOs.User;
using Exptour.Domain.Events;
using Exptour.Infrastructure.ElasticSearch.Services.Interfaces;
using Nest;

namespace Exptour.Infrastructure.ElasticSearch.Services;

public class UserSearchService : IUserSearchService
{
    private readonly IElasticClient _elasticClient;

    public UserSearchService(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task IndexUserAsync(UserResponse user)
    {
        await EnsureIndexExists();
        await _elasticClient.IndexDocumentAsync(user);
    }

    public async Task<List<UserResponse>> GetAllUsersAsync(bool isPaginated = false, int pageNumber = 1, int pageSize = 10)
    {
        var searchResponse = await _elasticClient.SearchAsync<UserResponse>(s => s
             .Index("users")
             .Query(q => q.MatchAll())
             .Size(isPaginated ? pageSize : 1000)
             .From(isPaginated ? (pageNumber - 1) * pageSize : 0));

        return searchResponse.Documents.ToList();
    }

    public async Task<List<UserResponse>> SearchUsersAsync(string searchText, bool isPaginated = false, int pageNumber = 1, int pageSize = 10)
    {
        var searchResponse = await _elasticClient.SearchAsync<UserResponse>(s => s
            .Query(q => q
                .Fuzzy(f => f
                    .Field(f => f.FullName)
                        .Value(searchText)
                        .Fuzziness(Fuzziness.Auto)
                        )
                )
            .Size(isPaginated ? pageSize : 1000)
            .From(isPaginated ? (pageNumber - 1) * pageSize : 0)
        );

        return searchResponse.Documents.ToList();
    }

    public async Task UpsertUserAsync(UserEvent userEvent)
    {
        await EnsureIndexExists();

        var user = new UserResponse(userEvent.Id, userEvent.Email, userEvent.FullName, userEvent.Roles);

        var searchResponse = await _elasticClient.SearchAsync<UserResponse>(s => s
            .Index("users")
            .Query(q =>
                q.Term(t =>
                    t.Field(f => f.Id)
                     .Value(userEvent.Id)))
            .Size(1)
        );

        if (searchResponse.Documents.Any())
        {
            var updateResponse = await _elasticClient.UpdateAsync<UserResponse>(userEvent.Id, u => u
                .Index("users")
                .Doc(user)
                .DocAsUpsert(true)
            );
        }
        else
            await IndexUserAsync(user);
    }

    public async Task EnsureIndexExists()
    {
        var existsResponse = await _elasticClient.Indices.ExistsAsync("users");
        if (!existsResponse.Exists)
        {
            var createResponse = await _elasticClient.Indices.CreateAsync("users", c => c
                .Map<UserResponse>(m => m
                    .AutoMap()
                )
            );
        }
    }
}
