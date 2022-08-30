using MyCampusV2.Common.ViewModels;
using MyCampusV2.Common.ViewModels.OfficeViewModel;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{
    public interface IOfficeService
    {
        Task<IList<officeEntity>> GetOfficesUsingCampusId(int id);
        Task<officeEntity> GetOfficeById(int id);
        Task<ResultModel> AddOffice(officeEntity office);
        Task<ResultModel> UpdateOffice(officeEntity office);
        Task<ResultModel> DeleteOfficePermanent(int id, int user);
        Task<ResultModel> DeleteOfficeTemporary(int id, int user);
        Task<ResultModel> RetrieveOffice(officeEntity office);
        Task<officePagedResult> GetAllOffice(int pageNo, int pageSize, string keyword);
        Task<officePagedResult> ExportOffice(string keyword);
        Task<BatchUploadResponse> BatchUpload(ICollection<officeBatchUploadVM> offices, int user, int uploadID, int row);
    }
}
