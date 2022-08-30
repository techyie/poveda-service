using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface ISchoolCalendarRepository : IBaseRepository<schoolCalendarEntity>
    {
        Task<schoolCalendarResult> GetCalendarDates(string schoolyear);
        Task<List<schoolCalendarDatesVM>> GetCalendarList(string schoolyear);

        Task<Boolean> AddWithBoolReturn(schoolCalendarEntity calendar, int user);
        Task<Boolean> UpdateWithBoolReturn(schoolCalendarEntity calendar, int user);
        Task<List<schoolCalendarEntity>> GetBySchoolYear(string calendar);
    }
}
