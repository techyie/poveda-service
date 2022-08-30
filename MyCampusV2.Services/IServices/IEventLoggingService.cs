using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.IServices
{
    public interface IEventLoggingService
    {
        Task<eventLoggingPagedResult> GetAllEventLogging(int pageNo, int pageSize, string keyword);
    }
}