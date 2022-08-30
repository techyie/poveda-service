using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.DAL.Repositories
{
    public class DataSyncRepository : BaseRepository<datasyncEntity>, IDataSyncRepository
    {
        public DataSyncRepository(MyCampusCardContext context) : base(context)
        {
        }
    }
}
