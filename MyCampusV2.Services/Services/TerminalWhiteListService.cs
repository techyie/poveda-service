using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.Services
{
    public class TerminalWhiteListService :BaseService, ITerminalWhiteListService
    {
        public TerminalWhiteListService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user)
        {
        }

        public async Task<ICollection<terminalWhitelistVM>> GetWhitelist(long id, string keyword)
        {
            return await _unitOfWork.TerminalWhiteListRepository.GetWhitelist(id, keyword).ToListAsync();
        }

        public async Task AddTerminalWhitelist(terminalWhitelistEntity whitelist, int user)
        {
            try
            {
                whitelist.Added_By = user;
                await _unitOfWork.TerminalWhiteListRepository.AddAsyn(whitelist);
               // await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Campus_Section, string.Format("Added: Section Name: {0}, Section Description: {1}", section.Section_Name, section.Section_Desc));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResultModel> RemoveWhitelistItem(int terminalId, int personId, string cardSerial, int user)
        {
            try
            {
                var terminalEntity = await this._unitOfWork.TerminalRepository.FindAsync(q => q.Terminal_ID == terminalId);

                var terminalWhitelistEntity = await this._unitOfWork.TerminalWhiteListRepository.FindAsync(q => q.Terminal_ID == terminalId && q.Person_ID == personId);

                var cardEntity = await this._unitOfWork.CardDetailsRepository.FindAsync(q => q.Card_Serial == cardSerial);

                datasyncEntity newDataSync = new datasyncEntity();
                newDataSync.Card_Serial = cardSerial;
                newDataSync.User_ID = user;
                newDataSync.Last_Updated = DateTime.UtcNow.ToLocalTime();
                newDataSync.Terminal_ID = terminalId;
                newDataSync.ToDisplay = true;
                newDataSync.IsActive = true;
                newDataSync.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                newDataSync.Updated_By = user;
                newDataSync.Added_By = user;
                newDataSync.DS_Action = "D";
                newDataSync.Expiry_Date = cardEntity.Expiry_Date;

                eventLoggingEntity eventLogging = new eventLoggingEntity();
                eventLogging.User_ID = user;
                eventLogging.Form_ID = (int)Form.Card_Assign;
                eventLogging.Source = "Remove Whitelist Item";
                eventLogging.Category = "DELETE";
                eventLogging.Log_Level = true;
                eventLogging.Message = "Successfully Deleted Card Serial: " + cardSerial + " from Terminal : " + Encryption.Decrypt(terminalEntity.Terminal_Name);
                eventLogging.Log_Date_Time = DateTime.UtcNow.ToLocalTime();

                return await _unitOfWork.TerminalWhiteListRepository.RemoveFromTerminalWhitelist(terminalWhitelistEntity, newDataSync, eventLogging);
            } catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> AddWhitelistItem(int terminalId, int personId, string cardSerial, int user)
        {
            try
            {
                var terminalEntity = await this._unitOfWork.TerminalRepository.FindAsync(q => q.Terminal_ID == terminalId);

                var cardEntity = await this._unitOfWork.CardDetailsRepository.FindAsync(q => q.Card_Serial == cardSerial);

                terminalWhitelistEntity newTerminalWhitelistEntity = new terminalWhitelistEntity();
                newTerminalWhitelistEntity.ToDisplay = true;
                newTerminalWhitelistEntity.IsActive = true;
                newTerminalWhitelistEntity.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                newTerminalWhitelistEntity.Updated_By = user;
                newTerminalWhitelistEntity.Added_By = user;
                newTerminalWhitelistEntity.Last_Updated = DateTime.UtcNow.ToLocalTime();
                newTerminalWhitelistEntity.Terminal_ID = terminalId;
                newTerminalWhitelistEntity.Person_ID = personId;

                datasyncEntity newDataSync = new datasyncEntity();
                newDataSync.Card_Serial = cardSerial;
                newDataSync.User_ID = user;
                newDataSync.Last_Updated = DateTime.UtcNow.ToLocalTime();
                newDataSync.Terminal_ID = terminalId;
                newDataSync.ToDisplay = true;
                newDataSync.IsActive = true;
                newDataSync.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                newDataSync.Updated_By = user;
                newDataSync.Added_By = user;
                newDataSync.DS_Action = "A";
                newDataSync.Expiry_Date = cardEntity.Expiry_Date;

                eventLoggingEntity eventLogging = new eventLoggingEntity();
                eventLogging.User_ID = user;
                eventLogging.Form_ID = (int)Form.Card_Assign;
                eventLogging.Source = "Add Whitelist Item";
                eventLogging.Category = "INSERT";
                eventLogging.Log_Level = true;
                eventLogging.Message = "Successfully Added Card Serial: " + cardSerial + " from Terminal : " + Encryption.Decrypt(terminalEntity.Terminal_Name);
                eventLogging.Log_Date_Time = DateTime.UtcNow.ToLocalTime();

                return await _unitOfWork.TerminalWhiteListRepository.AddToTerminalWhitelist(newTerminalWhitelistEntity, newDataSync, eventLogging);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task DeleteTerminalWhitelist(long id, int user)
        {
         
            try
            {
                terminalWhitelistEntity whiteList = await GetTerminalWhiteList(id);
                if (whiteList.IsActive)
                {
                    whiteList.IsActive = false;
                }
                else
                {
                    whiteList.IsActive = true;
                }
                await _unitOfWork.TerminalWhiteListRepository.UpdateAsyn(whiteList, whiteList.Whitelist_ID);
                // await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Campus_Area, string.Format("Updated Section {1} status to {0}", section.IsActive ? "Active" : "Inactive", section.Section_Name));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteAllTerminalWhitelist(long Card_ID)
        {
            try
            {
                var termnalWhitelist = await _unitOfWork.TerminalWhiteListRepository.GetAllByCardID(Card_ID);
                foreach (var terminal in termnalWhitelist)
                {
                   await _unitOfWork.TerminalWhiteListRepository.DeleteAsyn(terminal);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<terminalWhitelistEntity> GetTerminalWhiteList(long id)
        {
            return await _unitOfWork.TerminalWhiteListRepository.GetAsync(id);
        }

        public async Task<terminalWhitelistPagedResult> GetTerminalWhitelistV2(int pageNo, int pageSize, string keyword, int terminalid)
        {
            var fetcher = await _unitOfWork.TerminalRepository.FindAsync(q => q.Terminal_ID == terminalid);

            return await _unitOfWork.TerminalWhiteListRepository.GetTerminalWhitelistV2(pageNo, pageSize, keyword, (fetcher.IsForFetcher == true ? 1 : 0), terminalid);
        }

        public async Task UpdateTerminalWhitelist(terminalWhitelistEntity whitelist, int user)
        {
            try
            {
                terminalWhitelistEntity oldWhitelist = await GetTerminalWhiteList(whitelist.Whitelist_ID);
                await _unitOfWork.TerminalWhiteListRepository.UpdateAsyn(whitelist, whitelist.Whitelist_ID);
               // await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Campus_Section, string.Format("Updated: Section Name: {0} to {2}, Section Description: {1} to {3} ", oldSection.Section_Name, oldSection.Section_Desc, section.Section_Name, section.Section_Desc));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<terminalWhitelistPagedResult> GetAllTerminalWhitelist(long id, int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.TerminalWhiteListRepository.GetAllTerminalWhitelist(id, pageNo, pageSize, keyword);
        }
    }
}
