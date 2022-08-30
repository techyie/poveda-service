using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.IServices
{
    public interface IPositionService
    {
        Task<IList<positionEntity>> GetPositionsUsingDepartmentId(int id);
        Task<positionEntity> GetPositionById(int id);
        Task<ResultModel> AddPosition(positionEntity position);
        Task<ResultModel> UpdatePosition(positionEntity position);
        Task<ResultModel> DeletePositionPermanent(int id, int user);
        Task<ResultModel> DeletePositionTemporary(int id, int user);
        Task<ResultModel> RetrievePosition(positionEntity position);
        Task<positionPagedResult> GetAllPosition(int pageNo, int pageSize, string keyword);
        Task<positionPagedResult> ExportAllPositions(string keyword);
        Task<BatchUploadResponse> BatchUpload(ICollection<positionBatchUploadVM> positions, int user, int uploadID, int row);
    }

}
