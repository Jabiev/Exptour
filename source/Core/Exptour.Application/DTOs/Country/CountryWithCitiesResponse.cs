namespace Exptour.Application.DTOs.Country;

public record CountryWithCitiesResponse(Guid Id, string Name, List<CountryCityResponse> Cities);

public record CountryCityResponse(Guid Id, string Name);
