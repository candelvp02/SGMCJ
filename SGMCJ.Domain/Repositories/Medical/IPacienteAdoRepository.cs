using SGMCJ.Domain.Entities.Medical;

namespace SGMCJ.Domain.Repositories.Medical
{
    public interface IPacienteAdoRepository
    {
        Task<List<Paciente>> ListarActivosAsync();
        Task<List<Paciente>> BuscarPorNombreAsync(string nombre);
        Task<int> ObtenerTotalCitasAsync(int pacienteId);
    }
}