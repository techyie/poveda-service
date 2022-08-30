using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{
    public interface IDataSyncService 
    {
        Task DataSyncAdd(datasyncEntity dataSync, int user);
        Task DataSyncDelete(datasyncEntity dataSync, int user);
        Task DataSyncUpdate(datasyncEntity dataSync, int user);

        Task AddDataSync(datasyncEntity dataSync, int user);
        Task UpdateDataSync(datasyncEntity dataSync, int user);
        Task DeleteDataSync(long id, int user);

        Task<datasyncEntity> GetDataSync(long id);
    }
}
