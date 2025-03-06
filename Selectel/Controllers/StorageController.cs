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
        var keyName = string.IsNullOrEmpty(path)
            ? $"{Ulid.NewUlid()}{Path.GetExtension(file.FileName)}"
            : $"{path.TrimEnd('/')}/{Ulid.NewUlid()}{Path.GetExtension(file.FileName)}";

        var result = await _storageService.UploadFile(file, keyName);
        return Ok(new { KeyName = result });
    }

    [HttpPost("upload-big")]
    public async Task<IActionResult> UploadBigFile(IFormFile file, string path = "")
    {
        var keyName = string.IsNullOrEmpty(path)
            ? $"{Ulid.NewUlid()}{Path.GetExtension(file.FileName)}"
            : $"{path.TrimEnd('/')}/{Ulid.NewUlid()}{Path.GetExtension(file.FileName)}";

        var result = await _storageService.UploadBigFile(
            file,
            keyName,
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
        var fullPath = string.IsNullOrEmpty(path)
            ? $"{keyName}"
            : $"{path.TrimEnd('/')}/{keyName}";

        var stream = await _storageService.DownloadFileAsync($"{fullPath}");
        return File(stream, "application/octet-stream", keyName);
    }

    [HttpDelete("{keyName}")]
    public async Task<IActionResult> DeleteFile(string keyName, string path= "")
    {
        var fullPath = string.IsNullOrEmpty(path)
            ? $"{keyName}"
            : $"{path.TrimEnd('/')}/{keyName}";

        await _storageService.DeleteFileAsync($"{fullPath}");
        return NoContent();
    }
}
