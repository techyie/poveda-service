using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.Common.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Common.Helpers;
using MySql.Data.MySqlClient;
using System.Data;

namespace MyCampusV2.DAL.Repositories
{
    public class FetcherScheduleRepository : BaseRepository<fetcherScheduleEntity>, IFetcherScheduleRepository
    {
        private readonly MyCampusCardContext context;

        public FetcherScheduleRepository(MyCampusCardContext Context) : base(Context)
        {
            context = Context;
        }

        public async Task<fetcherScheduleEntity> GetFetcherScheduleByFetcherScheduleId(int fetcherSchedId)
        {
            try
            {
                return await this.context.FetcherScheduleEntity
                    .Where(b => b.Fetcher_Sched_ID == fetcherSchedId && b.IsActive == true).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
