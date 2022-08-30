using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IEventLoggingRepository : IBaseRepository<eventLoggingEntity>
    {
        Task<eventLoggingPagedResult> GetAllEventLogging(int pageNo, int pageSize, string keyword);
    }
}