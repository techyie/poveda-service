using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Services.Services
{
    public class EventLoggingService : BaseService, IEventLoggingService
    {
        private string _areaBatch = AppDomain.CurrentDomain.BaseDirectory + @"EventLogging\";
        private ResultModel result = new ResultModel();

        public EventLoggingService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user)
            : base(unitOfWork, audit, user)
        {

        }

        public async Task<ResultModel> AddEventLogging(eventLoggingEntity eventLogging)
        {
            var data = await _unitOfWork.EventLoggingRepository.AddAsyncWithBase(eventLogging);

            if (data == null)
            {
                return null;
            }

            result = new ResultModel();
            result.resultCode = "200";
            result.resultMessage = Convert.ToString(data.EventLog_ID);

            return result;
        }

        public async Task<eventLoggingPagedResult> GetAllEventLogging(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.EventLoggingRepository.GetAllEventLogging(pageNo, pageSize, keyword);
        }
    }
}