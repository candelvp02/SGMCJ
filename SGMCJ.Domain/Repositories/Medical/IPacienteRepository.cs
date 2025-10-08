using SGMCJ.Domain.Entities.Medical;

namespace SGMCJ.Domain.Repositories.Medical
{
    public interface IPacienteRepository : IBaseRepository<Paciente>
    {
        Task<Paciente?> GetByIdentificacionAsync(string identificacion);
        Task<bool> ExistePacienteAsync(string identificacion);
    }
}