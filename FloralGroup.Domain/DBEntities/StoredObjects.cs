using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloralGroup.Domain.DBEntities
{
    public class StoredObjects
    {
        public Guid Id { get; private set; }
        public string Key { get; private set; } = null!;
        public string OriginalName { get; private set; } = null!;
        public long SizeBytes { get; private set; }
        public string ContentType { get; private set; } = null!;
        public string Checksum { get; private set; } = null!;
        public string? Tags { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? DeletedAtUtc { get; private set; }
        public int Version { get; private set; }
        public Guid CreatedByUserId { get; private set; }
        
        private StoredObjects() { } // For Infrastructure Layer
        public StoredObjects(
        string key,
        string originalName,
        long sizeBytes,
        string contentType,
        string checksum,
        string? tags,
        Guid createdByUserId)
        {
            Id = Guid.NewGuid();
            Key = key;
            OriginalName = originalName;
            SizeBytes = sizeBytes;
            ContentType = contentType;
            Checksum = checksum;
            Tags = tags;
            CreatedAtUtc = DateTime.UtcNow;
            Version = 1;
            CreatedByUserId = createdByUserId;
        }
        public void DeleteUTC()
        {
            DeletedAtUtc = DateTime.UtcNow;
        }

        public void VersionIncreament()
        {
            Version++;
        }

    }
}
