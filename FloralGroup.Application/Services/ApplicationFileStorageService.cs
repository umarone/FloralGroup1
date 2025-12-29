using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FloralGroup.Domain.DBEntities;
using FloralGroup.Domain.Interfaces;
using FloralGroup.Infrastructure.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace FloralGroup.Application.Services
{
    public class ApplicationFileStorageService
    {
        private readonly IFileRepository _repository;
        private readonly FileService _fileService;
        public ApplicationFileStorageService(IFileRepository repository, FileService fileService)
        {
            _repository = repository;
            _fileService = fileService;
        }
        public async Task<StoredObjects> UploadFileAsync(Stream fileStream, string originalName, string contentType, string? tags, Guid createdByUserId)
        {
            var key = await _fileService.SaveFileAsync(fileStream, originalName,contentType);
            var checksum = _fileService.ComputeSHA256(fileStream);

            var storedObject = new StoredObjects(key, originalName, fileStream.Length, contentType, checksum, tags, createdByUserId);
            await _repository.AddAsync(storedObject);

            return storedObject;
        }
        public async Task<StoredObjects?> GetFileByIdAsync(Guid id)
        {
            // Fetch file metadata from database
            var storedObject = await _repository.GetByIdAsync(id);

            if (storedObject == null)
                return null; 

            return storedObject;
        }
        public async Task<IEnumerable<StoredObjects?>> GetAllFilesAsync()
        {
            // Fetch file metadata from database
            var storedObjects = await _repository.GetAllAsync();

            if (storedObjects == null)
                return null;

            return storedObjects;
        }
        public Stream DownloadFile(string key)
        {
            return _fileService.GetFileStream(key);
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            await _repository.SoftDeleteAsync(id);
        }

        public async Task HardDeleteAsync(Guid id)
        {
            var obj = await _repository.GetByIdAsync(id);
            if (obj != null)
            {
                _fileService.DeleteFile(obj.Key);
                await _repository.HardDeleteAsync(id);
            }
        }
        public HealthCheckResult CheckFileSystemHealth()
        {
            return _fileService.CheckFileSystemHealth();
        }

    }
}
