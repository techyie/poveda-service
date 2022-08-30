using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{
    public interface ITerminalWhiteListService 
    {
        Task<terminalWhitelistEntity> GetTerminalWhiteList(long id);
        Task AddTerminalWhitelist(terminalWhitelistEntity whitelist, int user);
        Task UpdateTerminalWhitelist(terminalWhitelistEntity whitelist, int user);
        Task<terminalWhitelistPagedResult> GetTerminalWhitelistV2(int pageNo, int pageSize, string keyword, int terminalid);
        Task DeleteTerminalWhitelist(long id, int user);
        Task DeleteAllTerminalWhitelist(long Card_ID);
        Task<ICollection<terminalWhitelistVM>> GetWhitelist(long id, string keyword);
        Task<terminalWhitelistPagedResult> GetAllTerminalWhitelist(long id, int pageNo, int pageSize, string keyword);
        Task<ResultModel> RemoveWhitelistItem(int terminalId, int personId, string cardSerial, int user);
        Task<ResultModel> AddWhitelistItem(int terminalId, int personId, string cardSerial, int user);
    }
}
