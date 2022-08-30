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
    public interface ITerminalRepository : IBaseRepository<terminalEntity>
    {
        Task<ICollection<terminalEntity>> GetAllTerminalOnly(long campusID);
        Task<ICollection<terminalEntity>> GetAllTerminalPerso();
        Task<terminalEntity> GetById(int? id);
        IQueryable<terminalEntity> GetAllTerminal();
        Task AddWhitelistItem(int terminalid, int carddetailsid, int user);
        Task RemoveWhitelistItem(int terminalid, int carddetailsid, int user);
        Task<ResultModel> SyncLatestCards(int terminalid, int user);

        Task<ResultModel> AddTerminal(terminalEntity entity, eventLoggingEntity eventEntity);
        Task<ResultModel> UpdateTerminal(terminalEntity entity, eventLoggingEntity eventEntity);
        Task<terminalPagedResult> GetAllTerminals(int pageNo, int pageSize, string keyword);
        Task<List<terminalEntity>> GetTerminalByCampusId(int campusId);
    }
}
