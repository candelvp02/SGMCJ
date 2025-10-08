using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Repositories;
using System.Threading.Tasks;

namespace SGMCJ.Domain.Interfaces.Repositories
{
    public interface IMedicoRepository : IBaseRepository<Medico>
    {
        Task<List<Medico>> GetByEspecialidadAsync(string especialidad);
        Task<bool> ExisteMedicoAsync(string cedula);
    }
}