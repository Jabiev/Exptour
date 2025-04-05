using Exptour.Application.DTOs.User;
using Exptour.Domain.Events;

namespace Exptour.Infrastructure.ElasticSearch.Services.Interfaces;

public interface IUserSearchService
{
    Task IndexUserAsync(UserResponse user);
    Task<List<UserResponse>> GetAllUsersAsync(bool isPaginated = false, int pageNumber = 1, int pageSize = 10);
    Task<List<UserResponse>> SearchUsersAsync(string searchText, bool isPaginated = false, int pageNumber = 1, int pageSize = 10);
    Task UpsertUserAsync(UserEvent userEvent);
    Task EnsureIndexExists();
}
