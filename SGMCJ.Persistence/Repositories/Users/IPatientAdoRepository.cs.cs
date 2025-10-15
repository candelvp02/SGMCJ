//using SGMCJ.Domain.Entities.Medical;

//namespace SGMCJ.Domain.Repositories.Medical
//{
//    public interface IPacienteAdoRepository
//    {
//        Task<List<Paciente>> ListarActivosAsync();
//        Task<List<Paciente>> BuscarPorNombreAsync(string nombre);
//        Task<int> ObtenerTotalCitasAsync(int pacienteId);
//    }
//}

using SGMCJ.Application.Dto.Users;

namespace SGMCJ.Domain.Repositories.Users
{
    public interface IPatientAdoRepository
    {
        Task<List<PatientDto>> ListActiveAsync();
        Task<List<PatientDto>> SearchByNameAsync(string name);
        Task<int> GetTotalAppointmentsAsync(int patientId);
    }
}