//using SGMCJ.Domain.Configuration;
//using SGMCJ.Domain.Entities.Medical;
//using System.Threading.Tasks;

//namespace SGMCJ.Domain.Repositories.Medical
//{
//    public interface IMedicoAdoRepository
//    {
//        Task<List<Medico>> ListarActivosAsync();
//        Task<List<Medico>> BuscarPorNombreAsync(string nombre);
//        Task<int> ObtenerTotalCitasAtendidasAsync(int medicoId);
//        Task<List<Medico>> ListarPorEspecialidadAsync(Especialidad especialidad);
//    }
//}

using SGMCJ.Application.Dto.Users;

namespace SGMCJ.Domain.Repositories.Users
{
    public interface IDoctorAdoRepository
    {
        Task<List<DoctorDto>> ListActiveAsync();
        Task<List<DoctorDto>> SearchByNameAsync(string name);
        Task<int> GetTotalAppointmentsAsync(int doctorId);
        Task<List<DoctorDto>> ListBySpecialtyAsync(short specialtyId);
    }
}