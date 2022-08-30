using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface ITerminalConfigurationRepository : IBaseRepository<terminalConfigurationEntity>
    {
        Task<terminalConfigurationEntity> GetTerminalConfigurationDetails(int id);
    }

}
