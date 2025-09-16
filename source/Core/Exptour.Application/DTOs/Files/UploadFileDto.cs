using Microsoft.AspNetCore.Http;

namespace Exptour.Application.DTOs.Files;

public record UploadFileDto(IFormFile File);
