using SGMCJ.Domain.Entities.Appointments;

namespace SGMCJ.Domain.Repositories.Appointments
{
    public interface IAppointmentRepository : IBaseRepository<Appointment>
    {
        //Task<List<Appointment>> GetByPacienteIdAsync(int pacienteId);
        //Task<List<Appointment>> GetByMedicoIdAsync(int medicoId);
        //Task<List<Appointment>> GetByFechaAsync(DateTime fecha);
        //Task<List<Appointment>> GetByEstadoAsync(string estado);
        //Task<bool> ExisteCitaEnHorarioAsync(int medicoId, DateTime fechaHora);
        //Task<List<Appointment>> GetCitasByPacienteAsync(int pacienteId);
        //Task<List<Appointment>> GetCitasProximasAsync(DateTime desde, DateTime hasta);

        //Task<Appointment?> GetIdByAsync(int appointmentId);
        //Task<IEnumerable<Appointment>> GetAllAsync();
        //Task<Appointment> AddAsync(Appointment appointment);
        //Task UpdateAsync(Appointment appointment);
        //Task DeleteAsync(int appointmentId);
        //Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
        //Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
        //Task<IEnumerable<Appointment>> GetByStatusIdAsync(int statusId);
        //Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        //Task<IEnumerable<Appointment>> GetUpcomingAppointmentAsync(int patientId);
        //Task<bool> ExistsAsync(int appointmentId);
        //Task<Appointment?> GetByIdWithDetailsAsync(int appointmentId);
        //Task<IEnumerable<Appointment>> GetAllWithDetailsAsync();

        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetByStatusIdAsync(int statusId);
        Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentAsync(int patientId);
        Task<bool> ExistsAsync(int appointmentId);
        Task<Appointment?> GetByIdWithDetailsAsync(int appointmentId);
        Task<IEnumerable<Appointment>> GetAllWithDetailsAsync();
        Task<bool> ExistsInTimeSlotAsync(int doctorId, DateTime appointmentDate);
        Task<IEnumerable<Appointment>> GetByPatientIdWithDetailsAsync(int patientId);
        Task DeleteAsync(Appointment appointment);
    }
}