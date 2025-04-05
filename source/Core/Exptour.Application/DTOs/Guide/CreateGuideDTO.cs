using Exptour.Domain;
using System.Text.Json.Serialization;

namespace Exptour.Application.DTOs.Guide;

public record CreateGuideDTO(string FullName,
    string Email,
    string Password,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] Gender Gender,
    List<string> LanguageIds);
