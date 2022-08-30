using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IDepedReportRepository : IBaseRepository<schoolCalendarEntity>
    {
        Task<DepedReportHeaderVM> GetHeaders(string schoolYear, string month, int sectionId);
        Task<List<ScheduleVM>> GetSchedule(string schoolYear, string month, int sectionId);
        Task<List<RecordsVM>> GetAttendance(List<ScheduleVM> schedule, string schoolYear, string month, int sectionId);
    }
}
