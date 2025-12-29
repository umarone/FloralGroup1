using FloralGroup.Application.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace FloralGroup.WebApi.HealthChecks
{
    public class FileHealthCheck : IHealthCheck
    {
        private readonly ApplicationFileStorageService _fileStorageService;

        public FileHealthCheck(ApplicationFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var result = _fileStorageService.CheckFileSystemHealth();
            return Task.FromResult(result);
        }
    }
}
