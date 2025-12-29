using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FloralGroup.Domain.DBEntities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Microsoft.Extensions.Logging;

namespace FloralGroup.Infrastructure.Services
{
    public class FileService
    {
        private readonly string _basePath;
        private readonly ILogger<FileService> _logger;
        private readonly long _maxFileSize;
        private readonly string[] _allowedContentTypes;
        public FileService(IConfiguration configuration, ILogger<FileService> logger)
        {
            _logger = logger;

            _basePath = configuration["windowsPath:FileStoragePathWindows"] ?? "uploads";
            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);

            // Upload constraints
            _maxFileSize = configuration.GetValue<long>("FileUpload:MaxFileSizeBytes", 104857600); // default 100 MB
            _allowedContentTypes = configuration.GetSection("FileUpload:AllowedContentTypes").Get<string[]>() ?? new string[0];
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string originalFileName, string contentType)
        {
            if (fileStream.Length > _maxFileSize)
                throw new FileUploadException($"File exceeds maximum allowed size of {_maxFileSize} bytes.");

            if (_allowedContentTypes.Length > 0 && !_allowedContentTypes.Contains(contentType))
                throw new FileUploadException($"File type '{contentType}' is not allowed.");

            var key = Guid.NewGuid().ToString();
            var filePath = Path.Combine(_basePath, key);

            _logger.LogInformation("Saving file {FileName} with key {Key}", originalFileName, key);

            try
            {
                await using var file = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true);
                await fileStream.CopyToAsync(file);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file {FileName}", originalFileName);
                throw;
            }

            return key;
        }
        public Stream GetFileStream(string key)
        {
            var filePath = Path.Combine(_basePath, key);
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("File not found: {Key}", key);
                throw new FileNotFoundException($"File {key} not found.");
            }

            _logger.LogInformation("Retrieving file {Key}", key);
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true);
        }

        public void DeleteFile(string key)
        {
            var filePath = Path.Combine(_basePath, key);
            if (File.Exists(filePath))
            {
                _logger.LogInformation("Deleting file {Key}", key);
                File.Delete(filePath);
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent file {Key}", key);
            }
        }

        public string ComputeSHA256(Stream stream)
        {
            using var sha = SHA256.Create();
            stream.Position = 0;
            var hash = sha.ComputeHash(stream);
            stream.Position = 0;
            return Convert.ToHexString(hash);
        }
        public HealthCheckResult CheckFileSystemHealth()
        {
            try
            {
                var testFile = Path.Combine(_basePath, "healthcheck.tmp");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
                return HealthCheckResult.Healthy("Filesystem is readable and writable.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Filesystem health check failed.");
                return HealthCheckResult.Unhealthy("Filesystem is not writable.");
            }
        }
    }
    public class FileUploadException : Exception
    {
        public FileUploadException(string message) : base(message) { }
    }
}
