using AutoMapper;
using Exptour.Application.Abstract.Repositories.Countries;
using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.City;
using Exptour.Application.DTOs.Country;
using Exptour.Application.Validators.Country;
using Exptour.Common.Helpers;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Shared;
using Exptour.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;

namespace Exptour.Persistence.Concrete.Services;

public class CountryService : BaseService, ICountryService
{
    private readonly IMapper _mapper;
    private readonly ICountryReadRepository _countryReadRepository;
    private readonly ICountryWriteRepository _countryWriteRepository;

    public CountryService(IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        ICountryWriteRepository countryWriteRepository,
        ICountryReadRepository countryReadRepository,
        IMapper mapper) : base(httpContextAccessor, configuration)
    {
        _countryWriteRepository = countryWriteRepository;
        _countryReadRepository = countryReadRepository;
        _mapper = mapper;
    }

    public async Task<APIResponse<CountryResponse>> CreateAsync(CountryDTO countryDTO)
    {
        var response = new APIResponse<CountryResponse>();

        CountryDTOValidator validations = new();
        var result = await validations.ValidateAsync(countryDTO);

        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("InvalidRequest").state;
            return response;
        }

        if (await _countryReadRepository.Table.AnyAsync(c => c.Name.ToLower() == countryDTO.Name.ToLower() && c.IsDeleted == false))
        {
            var msgCountryExists = GetMessageByLocalization("AlreadyExists");
            response.Message = msgCountryExists.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgCountryExists.state;
            return response;
        }

        var country = _mapper.Map<Country>(countryDTO);
        await _countryWriteRepository.AddAsync(country);
        await _countryWriteRepository.SaveChangesAsync();

        response.Payload = _mapper.Map<CountryResponse>(country);
        return response;
    }

    public async Task<APIResponse<CountryResponse>> DeleteAsync(string id)
    {
        var response = new APIResponse<CountryResponse>();

        if (!id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var country = await _countryReadRepository.FirstOrDefaultAsync(c => c.Id == Guid.Parse(id) && c.IsDeleted == false);
        if (country is null)
        {
            var msgNotFound = GetMessageByLocalization("NotFound");
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        //if there are cities in this country, delete them first | LOOK AT THIS
        if (await _countryReadRepository.Table.AnyAsync(c => c.Id == Guid.Parse(id) && c.Cities.Count > 0))
        {
            var msgCountryHasCities = GetMessageByLocalization("EntityHasAssociatedEntities");
            response.Message = msgCountryHasCities.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgCountryHasCities.state;
            return response;
        }

        country.IsDeleted = true;
        _countryWriteRepository.Update(country);
        await _countryWriteRepository.SaveChangesAsync();

        response.Payload = _mapper.Map<CountryResponse>(country);
        return response;
    }

    public async Task<APIResponse<Pagination<CountryResponse>>> GetAllAsync(int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<CountryResponse>>();

        if (pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var countries = isPaginated
            ? _countryReadRepository.Table
                .Where(c => c.IsDeleted == false)
                .Skip((pageNumber - 1) * take)
                .Take(take)
            : _countryReadRepository.Table
                .Where(c => c.IsDeleted == false);

        var countriesCount = await countries.CountAsync();

        response.Payload = new Pagination<CountryResponse>()
        {
            Items = await countries.Select(x => _mapper.Map<CountryResponse>(x)).ToListAsync(),
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : countriesCount,
            TotalCount = countriesCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)countriesCount / take) : 1
        };
        return response;
    }

    public async Task<APIResponse<Pagination<CountryWithCitiesResponse>>> GetAllWithCitiesAsync(int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<CountryWithCitiesResponse>>();

        if (pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var countriesQuery = _countryReadRepository.GetAll(
            c => !c.IsDeleted,
            include: c => c.Include(c => c.Cities),
            orderBy: c => c.Name
        );

        var countriesCount = await countriesQuery.CountAsync();

        if (isPaginated)
            countriesQuery = countriesQuery
                .Skip((pageNumber - 1) * take)
                .Take(take);

        var countries = await countriesQuery
            .Select(c => new CountryWithCitiesResponse(c.Id, c.Name, c.Cities
                .Select(city => new CountryCityResponse(city.Id, city.Name))
                .ToList()))
            .ToListAsync();

        response.Payload = new Pagination<CountryWithCitiesResponse>()
        {
            Items = countries,
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : countriesCount,
            TotalCount = countriesCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)countriesCount / take) : 1
        };
        return response;
    }

    public async Task<APIResponse<CountryResponse>> GetByIdAsync(string id)
    {
        var response = new APIResponse<CountryResponse>();

        if (!id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var country = await _countryReadRepository.FirstOrDefaultAsync(c => c.Id == Guid.Parse(id) && c.IsDeleted == false);
        if (country is null)
        {
            var msgNotFound = GetMessageByLocalization("NotFound");
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        response.Payload = _mapper.Map<CountryResponse>(country);
        response.ResponseCode = HttpStatusCode.OK;
        return response;
    }

    public async Task<APIResponse<Pagination<CountryResponse>>> GetByNameAsync(string clue, int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<CountryResponse>>();

        if (pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var countries = isPaginated
            ? _countryReadRepository.Table
                .Where(c => c.Name.ToLower().Contains(clue.ToLower()) && c.IsDeleted == false)
                .Skip((pageNumber - 1) * take)
                .Take(take)
            : _countryReadRepository.Table
                .Where(c => c.Name.ToLower().Contains(clue.ToLower()) && c.IsDeleted == false);

        if (countries is null || !countries.Any())
        {
            var msgNotFound = GetMessageByLocalization("NotFound");
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        var countriesCount = await countries.CountAsync();

        response.Payload = new Pagination<CountryResponse>()
        {
            Items = await countries.Select(x => _mapper.Map<CountryResponse>(x)).ToListAsync(),
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : countriesCount,
            TotalCount = countriesCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)countriesCount / take) : 1
        };
        return response;
    }

    public async Task<APIResponse<CountryResponse>> UpdateAsync(string id, CountryDTO countryDTO)
    {
        var response = new APIResponse<CountryResponse>();

        if (string.IsNullOrEmpty(id) || !id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        CountryDTOValidator validations = new();
        var result = await validations.ValidateAsync(countryDTO);

        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("InvalidRequest").state;
            return response;
        }

        var country = await _countryReadRepository.FirstOrDefaultAsync(c => c.Id == Guid.Parse(id) && c.IsDeleted == false);
        if (country is null)
        {
            var msgNotFound = GetMessageByLocalization("NotFound");
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        if (await _countryReadRepository.Table.AnyAsync(c => c.Name.ToLower() == countryDTO.Name.ToLower() && c.Id != Guid.Parse(id) && c.IsDeleted == false))
        {
            var msgCountryExists = GetMessageByLocalization("AlreadyExists");
            response.Message = msgCountryExists.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgCountryExists.state;
            return response;
        }

        country.Name = countryDTO.Name;
        _countryWriteRepository.Update(country);
        await _countryWriteRepository.SaveChangesAsync();

        response.Payload = _mapper.Map<CountryResponse>(country);
        response.ResponseCode = HttpStatusCode.OK;
        return response;
    }
}
