using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface ITerminalWhiteListRepository : IBaseRepository<terminalWhitelistEntity>
    {
        Task<ICollection<terminalWhitelistEntity>> GetAllByCardID(long card_Details_Id);
        IQueryable<terminalWhitelistVM> GetWhitelist(long id, string keyword);
        Task<terminalWhitelistPagedResult> GetTerminalWhitelistV2(int pageNo, int pageSize, string keyword, int fetcheruse, int terminalid);
        Task<terminalWhitelistPagedResult> GetAllTerminalWhitelist(long id, int pageNo, int pageSize, string keyword);
        Task<ResultModel> RemoveFromTerminalWhitelist(terminalWhitelistEntity terminalWhitelistEntity, datasyncEntity datasyncEntity, eventLoggingEntity eventLogging);
        Task<ResultModel> AddToTerminalWhitelist(terminalWhitelistEntity terminalWhitelistEntity, datasyncEntity datasyncEntity, eventLoggingEntity eventLogging);
    }
}
