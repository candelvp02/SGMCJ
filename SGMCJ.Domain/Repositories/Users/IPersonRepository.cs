using SGMCJ.Domain.Entities.Users;

namespace SGMCJ.Domain.Repositories.Users
{
    public interface IPersonRepository
    {
        Task<Person?> GetByIdAsync(int personId);
        Task<IEnumerable<Person>> GetAllAsync();
        Task<Person> AddAsync(Person person);
        Task UpdateAsync(Person person);
        Task DeleteAsync(int personId);

        Task<Person?> GetByIdentificationNumberAsync(string identificationNumber);
        Task<bool> ExistsAsync(int personId);
        Task<bool> ExistsByIdentificationNumberAsync(string identificationNumber);
    }
}