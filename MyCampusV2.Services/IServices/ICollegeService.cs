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
    public interface ICollegeService
    {
        Task<IList<collegeEntity>> GetCollegesUsingEducationalLevelId(int id);
        Task<collegeEntity> GetCollegeById(int id);
        Task<ResultModel> AddCollege(collegeEntity college);
        Task<ResultModel> UpdateCollege(collegeEntity college);
        Task<ResultModel> DeleteCollegePermanent(int id, int user);
        Task<ResultModel> DeleteCollegeTemporary(int id, int user);
        Task<ResultModel> RetrieveCollege(collegeEntity college);
        Task<collegePagedResult> GetAllCollege(int pageNo, int pageSize, string keyword);
        Task<collegePagedResult> ExportAllColleges(string keyword);
        Task<BatchUploadResponse> BatchUpload(ICollection<collegeBatchUploadVM> colleges, int user, int uploadID, int row);
    }
}
