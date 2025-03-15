namespace Exptour.Application.DTOs.City;

public record CityResponse(Guid Id, string Name, Guid CountryId, string CountryName);
