using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyCampusV2.Common.ViewModels;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IFetcherScheduleRepository : IBaseRepository<fetcherScheduleEntity>
    {
        Task<fetcherScheduleEntity> GetFetcherScheduleByFetcherScheduleId(int fetcherSchedId);
    }
}
