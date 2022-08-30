using MyCampusV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;

namespace MyCampusV2.IServices
{
    public interface ICampusService
    {
        Task<ResultModel> AddCampus(campusEntity campus, int user);
        Task<ResultModel> UpdateCampus(campusEntity campus, int user);
        Task<campusPagedResult> GetAllCampuses(int pageNo, int pageSize, string keyword);
        Task<ICollection<regionEntity>> GetRegion();
        Task<ICollection<divisionEntity>> GetDivisionByRegion(int id);
        Task<campusEntity> GetCampusByID(int id);
        Task<ICollection<campusEntity>> GetCampuses();
        Task<campusPagedResult> ExportCampus(string keyword);
        Task<ResultModel> DeleteCampusTemporary(int id, int user);
        Task<ResultModel> DeleteCampusPermanent(int id, int user);
        Task<BatchUploadResponse> BatchUpload(ICollection<campusBatchUploadVM> campus, int user, int uploadID, int row);
    }
}
