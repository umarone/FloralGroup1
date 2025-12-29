using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FloralGroup.Domain.DBEntities;
namespace FloralGroup.Infrastructure.EntityMappings
{
    public class FileStorageMappingConfiguration : IEntityTypeConfiguration<StoredObjects>
    {
        public void Configure(EntityTypeBuilder<StoredObjects> builder)
        {
            builder.ToTable("StoredObjects"); 
            builder.HasKey(x => x.Id); 
            builder.Property(x => x.Key) .IsRequired() .HasMaxLength(500); 
            builder.Property(x => x.OriginalName) .IsRequired() .HasMaxLength(255); 
            builder.Property(x => x.SizeBytes) .IsRequired(); 
            builder.Property(x => x.ContentType) .IsRequired() .HasMaxLength(100); 
            builder.Property(x => x.Checksum) .IsRequired() .HasMaxLength(128); 
            builder.Property(x => x.Tags) .HasMaxLength(500); 
            builder.Property(x => x.CreatedAtUtc) .IsRequired();
            builder.Property(x => x.DeletedAtUtc) .IsRequired(false); 
            builder.Property(x => x.Version) .IsRequired(); 
            builder.Property(x => x.CreatedByUserId) .IsRequired();
            builder.HasIndex(x => x.Key) .IsUnique(); 
            builder.HasIndex(x => x.CreatedAtUtc);
        }
    }
}
