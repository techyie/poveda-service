using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;

namespace MyCampusV2.IServices
{
    public interface IEducationLevelService
    {
        Task<IList<educationalLevelEntity>> GetEducationalLevelsUsingCampusId(int id);
        Task<IList<educationalLevelEntity>> GetEducationalLevelsCollegeOnlyUsingCampusId(int id);
        Task<educationalLevelEntity> GetEducationalLevelById(int id);
        Task<ResultModel> AddEducationalLevel(educationalLevelEntity educLevel);
        Task<ResultModel> UpdateEducationalLevel(educationalLevelEntity educLevel);
        Task<ResultModel> DeleteEducationalLevelPermanent(int id, int user);
        Task<ResultModel> DeleteEducationalLevelTemporary(int id, int user);
        Task<ResultModel> RetrieveEducationalLevel(educationalLevelEntity educationalLevel);
        Task<BatchUploadResponse> BatchUpload(ICollection<educLevelBatchUploadVM> educLevels, int user, int uploadID, int row);
        Task<educlevelPagedResult> GetAllEduclevel(int pageNo, int pageSize, string keyword);
        Task<educlevelPagedResult> ExportEducationalLevel(string keyword);
    }
}
