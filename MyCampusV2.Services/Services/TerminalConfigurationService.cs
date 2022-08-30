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
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.Services
{
    public class TerminalConfigurationService: BaseService, ITerminalConfigurationService
    {
        public TerminalConfigurationService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }
        
        public async Task<terminalConfigurationEntity> GetTerminalConfigurationDetails(int id)
        {
            return await _unitOfWork.TerminalConfigurationRepository.GetTerminalConfigurationDetails(id);
        }

        public async Task<ResultModel> UpdateTerminalConfiguration (terminalConfigurationEntity entity)
        {
            try
            {
                var data = await _unitOfWork.TerminalConfigurationRepository.UpdateAsyncWithBase(entity, entity.Config_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Added_By, (int)Form.Device_Terminal, "Update Terminal Configuration", "UPDATE", false, "Failed: " + entity.Config_ID, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Terminal Configuration", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Updated_By, (int)Form.Device_Terminal, "Update Terminal Configuration", "UPDATE", true, "Success: Terminal ID: " + entity.Config_ID, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Terminal Configuration" + Constants.SuccessMessageUpdate, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

    }
}
