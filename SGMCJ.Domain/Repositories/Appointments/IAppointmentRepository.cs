//using SGMCJ.Domain.Entities.Appointments;

//namespace SGMCJ.Domain.Repositories.Appointments
//{
//    public interface IAppointmentRepository
//    {
//        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
//        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
//        Task<IEnumerable<Appointment>> GetByStatusIdAsync(int statusId);
//        Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
//        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int patientId);
//        Task<bool> ExistsAsync(int appointmentId);
//        Task<Appointment?> GetByIdWithDetailsAsync(int appointmentId);
//        Task<IEnumerable<Appointment>> GetAllWithDetailsAsync();
//        Task<bool> ExistsInTimeSlotAsync(int doctorId, DateTime appointmentDate);
//        Task<Appointment> AddAsync(Appointment appointment);
//        Task GetByIdAsync(int id);
//        Task UpdateAsync(object appointment);
//        Task<IEnumerable<Appointment>> GetByPatientIdWithDetailsAsync(int patientId);
//    }
//}

using SGMCJ.Domain.Entities.Appointments;

namespace SGMCJ.Domain.Repositories.Appointments
{
    public interface IAppointmentRepository
    {
        // Operaciones de consulta
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetByStatusIdAsync(int statusId);
        Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int patientId);
        Task<bool> ExistsAsync(int appointmentId);
        Task<Appointment?> GetByIdAsync(int appointmentId);
        Task<Appointment?> GetByIdWithDetailsAsync(int appointmentId);
        Task<IEnumerable<Appointment>> GetAllWithDetailsAsync();
        Task<IEnumerable<Appointment>> GetByPatientIdWithDetailsAsync(int patientId);
        Task<bool> ExistsInTimeSlotAsync(int doctorId, DateTime appointmentDate);

        // Operaciones de escritura
        Task<Appointment> AddAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
        Task DeleteAsync(int id);
    }
}