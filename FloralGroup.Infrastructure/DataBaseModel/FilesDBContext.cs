using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FloralGroup.Domain.DBEntities;
namespace FloralGroup.Infrastructure.DataBaseModel
{
    public class FilesDBContext : DbContext
    {
        public FilesDBContext(DbContextOptions<FilesDBContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FilesDBContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<StoredObjects> FileTable => Set<StoredObjects>();
    }
}
