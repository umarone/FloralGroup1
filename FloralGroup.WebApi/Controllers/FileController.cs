using System.Security.Claims;
using FloralGroup.Application.Services;
using FloralGroup.Domain.DBEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FloralGroup.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "user,admin")]
    public class FileController : ControllerBase
    {
        private readonly ApplicationFileStorageService _fileStorageService;
        private readonly HealthCheckService _healthCheckService;

        public FileController(
            ApplicationFileStorageService fileStorageService,
            HealthCheckService healthCheckService)
        {
            _fileStorageService = fileStorageService;
            _healthCheckService = healthCheckService;
        }
        [HttpPost]
        [RequestSizeLimit(104_857_600)] // 100 MB, optional
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string? tags)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            using var stream = file.OpenReadStream();
            var storedObject = await _fileStorageService.UploadFileAsync(
                stream,
                file.FileName,
                file.ContentType,
                tags,
                userId
            );

            return CreatedAtAction(nameof(DownloadFile), new { id = storedObject.Id }, storedObject);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
           var files = await _fileStorageService.GetAllFilesAsync(); // You need to implement this
            return Ok(files);
        }

        [HttpGet("{id:guid}/download")]
        public async Task<IActionResult> DownloadFile([FromRoute] Guid id)
        {
            var file = await _fileStorageService.GetFileByIdAsync(id); // You need to implement this
            if (file == null)
                return NotFound();

            var stream = _fileStorageService.DownloadFile(file.Key);
            return File(stream, file.ContentType, file.OriginalName);
        }

        [HttpGet("{id:guid}/preview")]
        public async Task<IActionResult> PreviewFile([FromRoute] Guid id)
        {
            var file = await _fileStorageService.GetFileByIdAsync(id); // You need to implement this
            if (file == null)
                return NotFound();

            var stream = _fileStorageService.DownloadFile(file.Key);
            // Inline display for preview (browser can render PDFs, images, etc.)
            return File(stream, file.ContentType, file.OriginalName, enableRangeProcessing: true);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> SoftDeleteFile([FromRoute] Guid id)
        {
            await _fileStorageService.SoftDeleteAsync(id);
            return NoContent();
        }

        [HttpDelete("{id:guid}/hard")]
        public async Task<IActionResult> HardDeleteFile([FromRoute] Guid id)
        {
            await _fileStorageService.HardDeleteAsync(id);
            return NoContent();
        }

        [HttpGet("/health/live")]
        public IActionResult Live() => Ok("Alive");

        [HttpGet("/health/ready")]
        public async Task<IActionResult> Ready()
        {
            var report = await _healthCheckService.CheckHealthAsync();
            var statusCode = report.Status == HealthStatus.Healthy ? 200 : 503;
            return StatusCode(statusCode, report);
        }
    }
}
