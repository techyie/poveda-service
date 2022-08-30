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
    public class CalendarRecipientsService : BaseService, ICalendarRecipientsService
    {
        private string _calendarRecipientsBatch = AppDomain.CurrentDomain.BaseDirectory + @"Calendar Recipients\";
        private ResultModel result = new ResultModel();

        public CalendarRecipientsService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user)
            : base(unitOfWork, audit, user)
        {

        }
    }
}
