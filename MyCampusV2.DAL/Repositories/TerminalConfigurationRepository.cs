using Microsoft.EntityFrameworkCore;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class TerminalConfigurationRepository : BaseRepository<terminalConfigurationEntity>, ITerminalConfigurationRepository
    {
        private readonly MyCampusCardContext context;

        public TerminalConfigurationRepository(MyCampusCardContext Context) : base(Context)
        {
            this.context = Context;
        }

        public async Task<terminalConfigurationEntity> GetTerminalConfigurationDetails(int id)
        {
            return await this.context.TerminalConfigurationEntity
                .Include(a => a.TerminalEntity)
                .ThenInclude(b => b.AreaEntity)
                .ThenInclude(c => c.CampusEntity)
                .Where(q => q.Terminal_ID == id).FirstOrDefaultAsync();
        }
    }
}
