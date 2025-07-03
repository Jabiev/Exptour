using Exptour.Application.DTOs.Files;
using Exptour.Infrastructure.Storage.Services;
using Microsoft.AspNetCore.Mvc;

namespace Exptour.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileUploadController : ControllerBase
{
    private readonly CloudinaryService _cloudinaryService;

    public FileUploadController(CloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] UploadFileDto request)
    {
        var url = await _cloudinaryService.UploadFileAsync(request.File);
        if (url is null)
            return BadRequest("Upload failed");

        return Ok(new { url });
    }
}
