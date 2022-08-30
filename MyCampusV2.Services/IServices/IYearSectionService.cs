using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{
    public interface IYearSectionService
    {
        Task<ICollection<yearSectionEntity>> GetYearSections();
        Task<IList<yearSectionEntity>> GetYearSectionsUsingEducationalLevelId(int id);
        Task<yearSectionEntity> GetYearSectionById(int id);
        Task<ResultModel> AddYearSection(yearSectionEntity yearSection);
        Task<ResultModel> UpdateYearSection(yearSectionEntity yearSection);
        Task<ResultModel> DeleteYearSectionPermanent(int id, int user);
        Task<ResultModel> DeleteYearSectionTemporary(int id, int user);
        Task<ResultModel> RetrieveYearSection(yearSectionEntity yearSection);
        Task<yearSectionPagedResult> GetAllYearSection(int pageNo, int pageSize, string keyword);
        Task<yearSectionPagedResult> ExportAllYearSections(string keyword);
        Task<BatchUploadResponse> BatchUpload(ICollection<yearSectionBatchUploadVM> yearsections, int user, int uploadID, int row);
    }
}
