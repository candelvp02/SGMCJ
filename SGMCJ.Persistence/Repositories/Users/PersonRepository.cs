using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Users;
using SGMCJ.Domain.Repositories.Users;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Users
{
    public class PersonRepository : BaseRepository<Person>, IPersonRepository
    {
        public PersonRepository(HealtSyncContext context) : base(context) { }

        public async Task<Person?> GetByIdentificationNumberAsync(string identificationNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.IdentificationNumber == identificationNumber);
        }

        public async Task<bool> ExistsByIdentificationNumberAsync(string identificationNumber)
        {
            return await _dbSet.AnyAsync(p => p.IdentificationNumber == identificationNumber);
        }

        Task IPersonRepository.DeleteAsync(int personId)
        {
            return DeleteAsync(personId);
        }

        public Task<bool> ExistsAsync(int personId)
        {
            throw new NotImplementedException();
        }
    }
}