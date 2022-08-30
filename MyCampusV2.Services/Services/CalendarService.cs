using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.Services
{
    public class CalendarService : BaseService, ICalendarService
    {
        private string _calendarBatch = AppDomain.CurrentDomain.BaseDirectory + @"Calendar\";
        private ResultModel result = new ResultModel();

        public CalendarService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user)
            : base(unitOfWork, audit, user)
        {

        }

        public async Task<ResultModel> AddCalendar(calendarEntity calendarEntity, int userId)
        {
            try
            {
                return await _unitOfWork.CalendarRepository.AddCalendar(calendarEntity, userId);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateCalendar(calendarEntity calendarEntity, int userId)
        {
            try
            {
                return await _unitOfWork.CalendarRepository.UpdateCalendar(calendarEntity, userId);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteCalendarPermanent(string calendarCode, int userId)
        {
            try
            {
                return await _unitOfWork.CalendarRepository.DeleteCalendarPermanent(calendarCode, userId);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }
        
        public async Task<calendarPagedResult> GetAll(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.CalendarRepository.GetAll(pageNo, pageSize, keyword);
        }

        public async Task<calendarEntity> GetCalendarByCalendarCode(string code)
        {
            return await _unitOfWork.CalendarRepository.GetCalendarByCalendarCode(code);
        }

        public async Task<IList<yearSectionEntity>> GetYearLevelList()
        {
            return await _unitOfWork.AnnouncementsRepository.GetYearLevelList();
        }

        public async Task<calendarPagedResult> Export(string keyword)
        {
            return await _unitOfWork.CalendarRepository.Export(keyword);
        }
    }
}
