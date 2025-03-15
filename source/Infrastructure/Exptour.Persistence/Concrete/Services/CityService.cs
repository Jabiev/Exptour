using AutoMapper;
using Exptour.Application.Abstract.Repositories.Cities;
using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.City;
using Exptour.Application.Validators.City;
using Exptour.Common.Helpers;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Shared;
using Exptour.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;

namespace Exptour.Persistence.Concrete.Services;

public class CityService : BaseService, ICityService
{
    private readonly IMapper _mapper;
    private readonly ICountryService _countryService;
    private readonly ICityReadRepository _cityReadRepository;
    private readonly ICityWriteRepository _cityWriteRepository;

    public CityService(IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IMapper mapper,
        ICountryService countryService,
        ICityReadRepository cityReadRepository,
        ICityWriteRepository cityWriteRepository) : base(httpContextAccessor, configuration)
    {
        _mapper = mapper;
        _countryService = countryService;
        _cityReadRepository = cityReadRepository;
        _cityWriteRepository = cityWriteRepository;
    }

    public async Task<APIResponse<CityResponse>> CreateAsync(CityDTO cityDto)
    {
        var response = new APIResponse<CityResponse>();

        CityDTOValidator validations = new();

        var result = await validations.ValidateAsync(cityDto);
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

        var country = await _countryService.GetByIdAsync(cityDto.CountryId);
        if (country.Payload is null)
        {
            var msgNoEntityAssociation = GetMessageByLocalization("NoEntityAssociation");
            response.Message = msgNoEntityAssociation.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNoEntityAssociation.state;
            return response;
        }

        if (await _cityReadRepository.Table.AnyAsync(c => c.Name.ToLower() == cityDto.Name.ToLower() && c.IsDeleted == false && c.CountryId == Guid.Parse(cityDto.CountryId)))
        {
            var msgCityExists = GetMessageByLocalization("AlreadyExists");
            response.Message = msgCityExists.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgCityExists.state;
            return response;
        }

        var city = _mapper.Map<City>(cityDto);
        await _cityWriteRepository.AddAsync(city);
        await _cityWriteRepository.SaveChangesAsync();

        response.Payload = new CityResponse(city.Id, city.Name, city.CountryId, country.Payload.Name);
        return response;
    }

    public async Task<APIResponse<CityResponse>> DeleteAsync(string id)
    {
        var response = new APIResponse<CityResponse>();

        if (!id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var city = await _cityReadRepository.FirstOrDefaultAsync(c => c.Id == Guid.Parse(id) && c.IsDeleted == false, include: c => c.Include(c => c.Country));
        if (city is null)
        {
            var msgNotFound = GetMessageByLocalization("NotFound");
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        city.IsDeleted = true;
        _cityWriteRepository.Update(city);
        await _cityWriteRepository.SaveChangesAsync();

        response.Payload = new CityResponse(city.Id, city.Name, city.CountryId, city.Country.Name);
        return response;
    }

    public async Task<APIResponse<Pagination<CityResponse>>> GetAllAsync(int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<CityResponse>>();

        if (pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var cities = isPaginated
            ? _cityReadRepository.GetAll(c => !c.IsDeleted, include: c => c.Include(c => c.Country))
                .Skip((pageNumber - 1) * take)
                .Take(take)
            : _cityReadRepository.GetAll(c => !c.IsDeleted, include: c => c.Include(c => c.Country))
                .Where(c => c.IsDeleted == false);

        var citiesCount = await cities.CountAsync();

        response.Payload = new Pagination<CityResponse>()
        {
            Items = await cities.Select(x => new CityResponse(x.Id, x.Name, x.CountryId, x.Country.Name)).ToListAsync(),
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : citiesCount,
            TotalCount = citiesCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)citiesCount / take) : 1
        };
        return response;
    }

    public async Task<APIResponse<Pagination<CityResponse>>> GetByCountryIdAsync(string countryId, int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<CityResponse>>();

        if (pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        if (!countryId.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var cities = isPaginated
            ? _cityReadRepository.GetAll(c => !c.IsDeleted && c.CountryId == Guid.Parse(countryId), include: c => c.Include(c => c.Country))
                .Skip((pageNumber - 1) * take)
                .Take(take)
            : _cityReadRepository.GetAll(c => !c.IsDeleted && c.CountryId == Guid.Parse(countryId), include: c => c.Include(c => c.Country));

        var citiesCount = await cities.CountAsync();

        response.Payload = new Pagination<CityResponse>()
        {
            Items = await cities.Select(x => new CityResponse(x.Id, x.Name, x.CountryId, x.Country.Name)).ToListAsync(),
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : citiesCount,
            TotalCount = citiesCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)citiesCount / take) : 1
        };
        return response;
    }

    public async Task<APIResponse<CityResponse>> GetByIdAsync(string id)
    {
        var response = new APIResponse<CityResponse>();

        if (!id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var city = await _cityReadRepository.FirstOrDefaultAsync(c => c.Id == Guid.Parse(id) && c.IsDeleted == false, include: c => c.Include(c => c.Country));
        if (city is null)
        {
            var msgNotFound = GetMessageByLocalization("NotFound");
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        response.Payload = new CityResponse(city.Id, city.Name, city.CountryId, city.Country.Name);
        return response;
    }

    public async Task<APIResponse<Pagination<CityResponse>>> GetByNameAsync(string clue, int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<CityResponse>>();

        if (pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var cities = isPaginated
            ? _cityReadRepository.GetAll(c => !c.IsDeleted && c.Name.ToLower().Contains(clue.ToLower()), include: c => c.Include(c => c.Country))
                .Skip((pageNumber - 1) * take)
                .Take(take)
            : _cityReadRepository.GetAll(c => !c.IsDeleted && c.Name.ToLower().Contains(clue.ToLower()), include: c => c.Include(c => c.Country));

        var citiesCount = await cities.CountAsync();

        response.Payload = new Pagination<CityResponse>()
        {
            Items = await cities.Select(x => new CityResponse(x.Id, x.Name, x.CountryId, x.Country.Name)).ToListAsync(),
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : citiesCount,
            TotalCount = citiesCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)citiesCount / take) : 1
        };
        return response;
    }

    public async Task<APIResponse<CityResponse>> UpdateAsync(string id, UpdateCityDTO updateCityDto)
    {
        var response = new APIResponse<CityResponse>();

        if (!id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        UpdateCityDTOValidator validations = new();

        var result = await validations.ValidateAsync(updateCityDto);
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

        var city = await _cityReadRepository.FirstOrDefaultAsync(c => c.Id == Guid.Parse(id) && c.IsDeleted == false, include: c => c.Include(c => c.Country));
        if (city is null)
        {
            var msgNotFound = GetMessageByLocalization("NotFound");
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        if (await _cityReadRepository.Table.AnyAsync(c => c.Name.ToLower() == updateCityDto.Name.ToLower() && c.IsDeleted == false && c.Id != Guid.Parse(id)))
        {
            var msgInvalidRequest = GetMessageByLocalization("AlreadyExists");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        if(!string.IsNullOrEmpty(updateCityDto.CountryId))
        {
            if (!updateCityDto.CountryId.IsValidGuid())
            {
                var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.Message = msgInvalidRequest.message;
                response.State = msgInvalidRequest.state;
                return response;
            }
            city.CountryId = Guid.Parse(updateCityDto.CountryId);
        }

        city.Name = updateCityDto.Name;
        _cityWriteRepository.Update(city);
        await _cityWriteRepository.SaveChangesAsync();

        response.Payload = new CityResponse(city.Id, city.Name, city.CountryId, city.Country.Name);
        return response;
    }
}
