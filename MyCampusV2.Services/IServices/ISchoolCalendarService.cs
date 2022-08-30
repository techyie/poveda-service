using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{
    public interface ISchoolCalendarService
    {
        Task<schoolCalendarResult> GetCalendarDates(string schoolyear);
        Task<List<schoolCalendarDatesVM>> GetCalendarList(string schoolyear);
        Task<BatchUploadResponse> BatchUpload(ICollection<schoolCalendarBatchUploadVM> calendarlist, int user, int uploadID, int row);
        Task<schoolYearPagedResult> GetAll(int pageNo, int pageSize, string keyword);
        Task<schoolYearPagedResult> GetFiltered();
        Task<ResultModel> AddSchoolYear(schoolYearEntity schoolYear);
        Task<ResultModel> UpdateSchoolYear(schoolYearEntity schoolYear);
        Task<ResultModel> DeleteSchoolYear(int id, int user);
        Task<schoolYearEntity> GetById(int id);
    }
}
