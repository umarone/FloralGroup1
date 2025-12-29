using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FloralGroup.Domain.DBEntities;
using FloralGroup.Domain.Interfaces;
using FloralGroup.Infrastructure.DataBaseModel;
using Microsoft.EntityFrameworkCore;

namespace FloralGroup.Infrastructure.Repositories
{
        public class FileRepository : IFileRepository
        {
            private readonly FilesDBContext _context;

            public FileRepository(FilesDBContext context)
            {
                _context = context;
            }

            public async Task AddAsync(StoredObjects storedObject)
            {
                _context.FileTable.Add(storedObject);
                await _context.SaveChangesAsync();
            }

            public async Task<IEnumerable<StoredObjects>> GetAllAsync(bool includeDeleted = false)
            {
                return await _context.FileTable
                    .Where(x => includeDeleted || x.DeletedAtUtc == null)
                    .ToListAsync();
            }

            public async Task<StoredObjects> GetByIdAsync(Guid id)
            {
                return await _context.FileTable.FirstOrDefaultAsync(x => x.Id == id);
            }

            public async Task<StoredObjects> GetByKeyAsync(string key)
            {
                return await _context.FileTable.FirstOrDefaultAsync(x => x.Key == key);
            }

            public async Task SoftDeleteAsync(Guid id)
            {
                var obj = await GetByIdAsync(id);
                if (obj != null)
                {
                    obj.DeleteUTC();
                    await _context.SaveChangesAsync();
                }
            }

            public async Task HardDeleteAsync(Guid id)
            {
                var obj = await GetByIdAsync(id);
                if (obj != null)
                {
                    _context.FileTable.Remove(obj);
                    await _context.SaveChangesAsync();
                }
            }

            public async Task UpdateAsync(StoredObjects storedObject)
            {
                _context.FileTable.Update(storedObject);
                await _context.SaveChangesAsync();
            }   
    }
}
