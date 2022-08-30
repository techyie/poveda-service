using MyCampusV2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;

namespace MyCampusV2.Services.IServices
{
    public interface ITerminalService
    {
        Task<ICollection<terminalEntity>> GetAll();
        Task<ICollection<terminalEntity>> GetAllActive();
        Task<ICollection<terminalEntity>> GetByArea(int id);
        Task<ICollection<terminalEntity>> GetByArea(int id, int isnotification);
        Task<terminalEntity> GetById(int? id);
        Task<terminalEntity> GetByTerminalCode(string code);
        Task<terminalEntity> GetByTerminalIP(string ip);
        Task AddTerminal(terminalEntity position, int user);
        Task<ResultModel> AddTerminal(terminalEntity entity);
        Task<ResultModel> UpdateTerminal(terminalEntity entity);
        Task UpdateTerminal(terminalEntity position, int user);
        Task<ResultModel> DeleteTerminal(int id, int user);
		Task<ICollection<terminalEntity>> GetTerminalByArea(int id);
        Task AddWhitelistItem(int terminalid, int carddetailsid, int user);
        Task RemoveWhitelistItem(int terminalid, int carddetailsid, int user);
        Task<ResultModel> SyncLatestCards(int terminalid, int user);

        Task<terminalPagedResult> GetAllTerminals(int pageNo, int pageSize, string keyword);
        Task<List<terminalEntity>> GetTerminalByCampusId(int campusId);
    }
}
