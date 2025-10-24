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