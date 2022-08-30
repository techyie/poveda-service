using System;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IAreaRepository : IBaseRepository<areaEntity>
    {
        Task<IList<areaEntity>> GetAreasUsingCampusId(int id);
        Task<IList<areaEntity>> GetAreasUsingCampusName(string campus);
        Task<areaEntity> GetAreaById(int id);
        Task<areaPagedResult> GetAllArea(int pageNo, int pageSize, string keyword);
        Task<areaPagedResult> ExportAllAreas(string keyword);

        Task<Boolean> UpdateAreaWithBoolReturn(areaEntity area, int user);
        Task<Boolean> AddAreaWithBoolReturn(areaEntity area, int user);
    }

}
