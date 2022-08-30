using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;

namespace MyCampusV2.Services.Services
{
    public class TerminalService : BaseService, ITerminalService
    {
        public TerminalService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        private IQueryable<terminalEntity> GetData(IQueryable<terminalEntity> query)
        {
            return !_user.MasterAccess ? query.Where(o => o.AreaEntity.Campus_ID == _user.Campus) : query;
        }

        public async Task<ICollection<terminalEntity>> GetAll()
        {
            //return await GetData(_unitOfWork.TerminalRepository.GetAllTerminal()).ToListAsync();
            return await _unitOfWork.TerminalRepository.GetAllTerminal().ToListAsync();
        }

        public async Task<ICollection<terminalEntity>> GetAllActive()
        {
            return await _unitOfWork.TerminalRepository.FindAllAsync(q => q.IsActive == true);
        }

        public async Task<ICollection<terminalEntity>> GetByArea(int id)
        {
            //return await GetData(_unitOfWork.TerminalRepository.GetAllTerminal()).Where(x=>x.Area_ID == id).ToListAsync();
            return await _unitOfWork.TerminalRepository.GetAllTerminal().Where(x => x.Area_ID == id).ToListAsync();
        }

        public async Task<ICollection<terminalEntity>> GetByArea(int id, int isnotification)
        {
            //return await GetData(_unitOfWork.TerminalRepository.GetAllTerminal()).Where(x => x.Area_ID == id && x.Terminal_Category_ID == 1).ToListAsync();
            //return await _unitOfWork.TerminalRepository.GetAllTerminal().Where(x => x.Area_ID == id && x.Terminal_Category_ID == 1).ToListAsync();
            return null;
        }

        public async Task<ICollection<terminalEntity>> GetTerminalByArea(int id)
        {
            //return await GetData(_unitOfWork.TerminalRepository.GetAllTerminal()).Where(x => x.Area_ID == id && x.Terminal_Category_ID == 2).ToListAsync();
            //return await _unitOfWork.TerminalRepository.GetAllTerminal().Where(x => x.Area_ID == id && x.Terminal_Category_ID == 2).ToListAsync();
            return null;
        }

        //public async Task<ICollection<tbl_terminal_category>> GetAllCategory()
        //{
        //    return await _unitOfWork.TerminalRepository.GetAllTerminalCategory();
        //}

        public async Task<List<terminalEntity>> GetTerminalByCampusId(int id)
        {
            return await _unitOfWork.TerminalRepository.GetTerminalByCampusId(id);
        }

        public async Task<terminalEntity> GetByTerminalCode(string code)
        {
            //return await GetData(_unitOfWork.TerminalRepository.GetAllTerminal()).Where(x => x.Terminal_Code == code && x.IsActive == true).FirstOrDefaultAsync();
            return await _unitOfWork.TerminalRepository.GetAllTerminal().Where(x => x.Terminal_Code == code && x.IsActive == true).FirstOrDefaultAsync();
        }

        public async Task<terminalEntity> GetByTerminalIP(string ip)
        {
            //return await GetData(_unitOfWork.TerminalRepository.GetAllTerminal()).Where(x => x.Terminal_IP == ip && x.IsActive == true).FirstOrDefaultAsync();
            return await _unitOfWork.TerminalRepository.GetAllTerminal().Where(x => x.Terminal_IP == ip && x.IsActive == true).FirstOrDefaultAsync();
        }

        public async Task<terminalEntity> GetById(int? id)
        {
            return await _unitOfWork.TerminalRepository.GetById(id);
        }

        public async Task AddTerminal(terminalEntity ter, int user)
        {
            try
            {
                ter.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                ter.Added_By = user;
                ter.Last_Updated = DateTime.UtcNow.ToLocalTime();
                ter.Updated_By = user;

                await _unitOfWork.TerminalRepository.AddAsyn(ter);
                await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Device_Terminal, string.Format("Added: Terminal Name: {0}, Terminal Code: {1}, Terminal IP {2}", ter.Terminal_Name, ter.Terminal_Code, ter.Terminal_IP));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateTerminal(terminalEntity terminal, int user)
        {
            try
            {
                terminalEntity oldTerminal = await GetTerminal(terminal.Terminal_ID);

                terminal.Date_Time_Added = oldTerminal.Date_Time_Added;
                terminal.Added_By = oldTerminal.Added_By;
                terminal.Last_Updated = DateTime.UtcNow.ToLocalTime();
                terminal.Updated_By = user;

                await _unitOfWork.TerminalRepository.UpdateAsyn(terminal, terminal.Terminal_ID);
                await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Device_Terminal, string.Format("Updated: Terminal Name: {0} to {2}, Terminal Code: {1} to {3}, Terminal IP: {4} to {5}", oldTerminal.Terminal_Name, oldTerminal.Terminal_Code, terminal.Terminal_Name, terminal.Terminal_Code, oldTerminal.Terminal_IP, terminal.Terminal_IP));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResultModel> DeleteTerminal(int id, int user)
        {
            try
            {
                terminalEntity terminal = await _unitOfWork.TerminalRepository.GetAsync(id);

                if (terminal.IsActive)
                {
                    terminal.IsActive = false;
                    terminal.Terminal_Status = "Inactive";
                    terminal.Last_Updated = DateTime.UtcNow.ToLocalTime();
                    terminal.Updated_By = user;
                }
                else
                {
                    terminal.IsActive = true;
                    terminal.Terminal_Status = "Active";
                    terminal.Last_Updated = DateTime.UtcNow.ToLocalTime();
                    terminal.Updated_By = user;
                }

                await _unitOfWork.TerminalRepository.UpdateAsyn(terminal, terminal.Terminal_ID);
                await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Device_Terminal, string.Format("Updated Terminal {1} status to {0}", terminal.IsActive ? "Active" : "Inactive", terminal.Terminal_Name));

                if (terminal.IsActive)
                {
                    return CreateResult("200", "Terminal has been successfully activated!", true);
                } 
                else
                {
                    return CreateResult("200", "Terminal has been successfully deactivated!", true);
                }
                
            } catch(Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<terminalEntity> GetTerminal(long id)
        {
            return await _unitOfWork.TerminalRepository.FindAsync(x => x.Terminal_ID == id);
        }

        //public async Task<ICollection<tbl_terminal_whitelist>> GetTerminalWhitelist(int id)
        //{
        //    return await _unitOfWork.TerminalWhiteListRepository.GetAllTerminalWhitelist().Where(q => q.Terminal_ID == id);
        //}

        public async Task AddWhitelistItem(int terminalid, int carddetailsid, int user)
        {
            await _unitOfWork.TerminalRepository.AddWhitelistItem(terminalid, carddetailsid, user);
        }

        public async Task RemoveWhitelistItem(int terminalid, int carddetailsid, int user)
        {
            await _unitOfWork.TerminalRepository.RemoveWhitelistItem(terminalid, carddetailsid, user);
        }

        public async Task<ResultModel> SyncLatestCards(int terminalid, int user)
        {
            return await _unitOfWork.TerminalRepository.SyncLatestCards(terminalid, user);
        }

        public async Task<ResultModel> AddTerminal(terminalEntity entity)
        {
            try
            {
                var exist = await _unitOfWork.TerminalRepository.FindAsync(q => 
                    (q.Terminal_Name == entity.Terminal_Name) || 
                    (q.Terminal_Code == entity.Terminal_Code) ||
                    (q.Terminal_IP == entity.Terminal_IP) &&
                    (q.IsActive == true) && 
                    (q.Area_ID == entity.Area_ID));

                if (exist != null)
                    return CreateResult("409", TERMINAL_EXIST, false);

                return await _unitOfWork.TerminalRepository.AddTerminal(entity,
                    fillEventLogging(entity.Added_By, (int)Form.Device_Terminal, "Add Terminal", "INSERT", false, "Failed: " + entity.Terminal_Name, DateTime.UtcNow.ToLocalTime()));


                //var data = await _unitOfWork.TerminalRepository.AddAsyncWithBase(entity);

                //if (data == null)
                //{
                //    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Added_By, (int)Form.Campus_Office, "Add Terminal", "INSERT", false, "Failed: " + entity.Terminal_Name, DateTime.UtcNow.ToLocalTime()));
                //    return CreateResult("409", "Terminal", false);
                //}

                //await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Added_By, (int)Form.Campus_Office, "Add Terminal", "INSERT", true, "Success: " + entity.Terminal_Name, DateTime.UtcNow.ToLocalTime()));

                //return CreateResult("200", "Terminal" + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateTerminal(terminalEntity entity)
        {
            try
            {
                var exist = await _unitOfWork.TerminalRepository.FindAsync(q =>
                    (q.Terminal_Name == entity.Terminal_Name) &&
                    (q.Terminal_IP == entity.Terminal_IP) &&
                    (q.IsActive == true) &&
                    (q.Terminal_ID != entity.Terminal_ID));

                if (exist != null)
                    return CreateResult("409", TERMINAL_EXIST, false);

                return await _unitOfWork.TerminalRepository.UpdateTerminal(entity,
                        fillEventLogging(entity.Updated_By, (int)Form.Device_Terminal, "Update Terminal", "UPDATE", false, "Failed: " + entity.Terminal_Name, DateTime.UtcNow.ToLocalTime()));

                //var data = await _unitOfWork.TerminalRepository.UpdateAsyncWithBase(entity, entity.Terminal_ID);

                //if (data == null)
                //{
                //    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Added_By, (int)Form.Campus_Office, "Update Terminal", "UPDATE", false, "Failed: " + entity.Terminal_Name, DateTime.UtcNow.ToLocalTime()));
                //    return CreateResult("409", "Terminal", false);
                //}

                //await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Updated_By, (int)Form.Campus_Office, "Update Terminal", "UPDATE", true, "Success: Terminal ID: " + entity.Terminal_ID + " Terminal Name: " + entity.Terminal_Name, DateTime.UtcNow.ToLocalTime()));

                //return CreateResult("200", "Terminal" + Constants.SuccessMessageUpdate, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<terminalPagedResult> GetAllTerminals(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.TerminalRepository.GetAllTerminals(pageNo, pageSize, keyword);
        }
    }
}
