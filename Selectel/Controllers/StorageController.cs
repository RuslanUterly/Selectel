using Microsoft.AspNetCore.Mvc;
using Selectel.Services;
using System.IO;

namespace Selectel.Controllers;

[ApiController]
[Route("api/storage")]
public class StorageController : ControllerBase
{
    private readonly SelectelStorageService _storageService;

    public StorageController(SelectelStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, string path = "")
    {
        var keyName = $"{path.TrimEnd('/')}{Ulid.NewUlid()}{Path.GetExtension(file.FileName)}";
        var result = await _storageService.UploadFile(file, keyName, path);
        return Ok(new { KeyName = result });
    }

    [HttpPost("upload-big")]
    public async Task<IActionResult> UploadBigFile(IFormFile file, string path = "")
    {
        var keyName = $"{path.TrimEnd('/')}{Ulid.NewUlid()}{Path.GetExtension(file.FileName)}";
        var result = await _storageService.UploadBigFile(
            file,
            keyName,
            path,
            progress =>
            {
                Console.WriteLine($"Current progress: {progress}%");
            }
        );
        return Ok(new { KeyName = result });
    }

    [HttpGet("download/{keyName}")]
    public async Task<IActionResult> DownloadFile(string keyName, string path = "")
    {
        var stream = await _storageService.DownloadFileAsync($"{path.TrimEnd('/')}{keyName}");
        return File(stream, "application/octet-stream", keyName);
    }

    [HttpDelete("{keyName}")]
    public async Task<IActionResult> DeleteFile(string keyName, string path= "")
    {
        await _storageService.DeleteFileAsync($"{path.TrimEnd('/')}{keyName}");
        return NoContent();
    }
}
