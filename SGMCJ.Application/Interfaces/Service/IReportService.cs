//using SGMCJ.Application.Dto.Appointments;
//using SGMCJ.Domain.Base;

//namespace SGMCJ.Application.Interfaces.Service
//{
//    public interface IReportService
//    {
//        //generacion de reportes rf2.2.6/3.1.12
//        Task<OperationResult<byte[]>> GenerateAppointmentReportAsync(ReportFilterDto filter, string format);
//        Task<OperationResult<byte[]>> GenerateDoctorPerformanceReportAsync(int doctorId, DateTime startDate, DateTime endDate, string format);
//        Task<OperationResult<byte[]>> GeneratePatientHistoryReportAsync(int patientId, string format);
//        Task<OperationResult<AppointmentStatisticsDto>> GetAppointmentStatisticsAsync(DateTime startDate, DateTime endDate);
//        Task<OperationResult<List<AppointmentSummaryDto>>> GetAppointmentsByStatusAsync(int statusId);
//        Task<OperationResult<List<AppointmentSummaryDto>>> GetAppointmentsByDoctorAsync(int doctorId, DateTime startDate, DateTime endDate);
//    }
//}

using SGMCJ.Application.Dto.Appointments;
using SGMCJ.Domain.Base;

namespace SGMCJ.Application.Interfaces.Service
{
    public interface IReportService
    {
        Task<OperationResult<byte[]>> GenerateAppointmentsReportAsync(ReportFilterDto filter);
        Task<OperationResult<byte[]>> GenerateExcelAppointmentsReportAsync(ReportFilterDto filter);
        Task<OperationResult<AppointmentStatisticsDto>> GetAppointmentStatisticsAsync(ReportFilterDto filter);
    }
}