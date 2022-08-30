using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{
    public interface ITerminalConfigurationService
    {
        Task<ResultModel> UpdateTerminalConfiguration(terminalConfigurationEntity entity);
        Task<terminalConfigurationEntity> GetTerminalConfigurationDetails(int id);
    }
}
