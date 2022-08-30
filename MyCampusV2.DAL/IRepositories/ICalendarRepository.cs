using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface ICalendarRepository : IBaseRepository<calendarEntity>
    {
        Task<ResultModel> AddCalendar(calendarEntity calendarEntity, int userId);
        Task<ResultModel> UpdateCalendar(calendarEntity calendarEntity, int userId);
        Task<calendarEntity> GetCalendarByCalendarCode(string code);
        Task<calendarPagedResult> GetAll(int pageNo, int pageSize, string keyword);
        Task<ResultModel> DeleteCalendarPermanent(string calendarCode, int userId);
        Task<calendarPagedResult> Export(string keyword);
    }
}
