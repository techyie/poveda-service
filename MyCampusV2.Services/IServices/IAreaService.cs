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
    public interface IAreaService
    {
        Task<IList<areaEntity>> GetAreasUsingCampusId(int id);
        Task<IList<areaEntity>> GetAreasUsingCampusName(string campus);
        Task<areaEntity> GetAreaById(int id);
        Task<areaPagedResult> GetAllArea(int pageNo, int pageSize, string keyword);
        Task<ResultModel> AddArea(areaEntity area);
        Task<ResultModel> UpdateArea(areaEntity area);
        Task<ResultModel> DeleteAreaPermanent(int id, int user);
        Task<ResultModel> DeleteAreaTemporary(int id, int user);
        Task<ResultModel> RetrieveArea(areaEntity area);
        Task<areaPagedResult> ExportAllAreas(string keyword);
        Task<BatchUploadResponse> BatchUpload(ICollection<areaBatchUploadVM> areas, int user, int uploadID, int row);
    }

}
