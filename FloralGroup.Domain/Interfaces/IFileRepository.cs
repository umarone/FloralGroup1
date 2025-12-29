using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FloralGroup.Domain.DBEntities;

namespace FloralGroup.Domain.Interfaces
{
    public interface IFileRepository
    {
        Task<StoredObjects> GetByIdAsync(Guid id);
        Task<StoredObjects> GetByKeyAsync(string key);
        Task AddAsync(StoredObjects storedObject);
        Task UpdateAsync(StoredObjects storedObject);
        Task SoftDeleteAsync(Guid id);
        Task HardDeleteAsync(Guid id);
        Task<IEnumerable<StoredObjects>> GetAllAsync(bool includeDeleted = false);
    }
}
