using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IPositionRepository : IBaseRepository<positionEntity>
    {
        Task<IList<positionEntity>> GetPositionsUsingDepartmentId(int id);
        Task<positionEntity> GetPositionById(int id);
        Task<positionPagedResult> GetAllPosition(int pageNo, int pageSize, string keyword);
        Task<positionPagedResult> ExportAllPositions(string keyword);

        Task<Boolean> UpdatePositionWithBoolReturn(positionEntity position, int user);
        Task<Boolean> AddPositionWithBoolReturn(positionEntity position, int user);
    }
}
