using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{
    public interface IStudentSectionService
    {
        Task<IList<studentSectionEntity>> GetStudentSectionsUsingYearSectionId(int id);
        Task<studentSectionEntity> GetStudentSectionById(int id);
        Task<ResultModel> AddStudentSection(studentSectionEntity studentSection);
        Task<ResultModel> UpdateStudentSection(studentSectionEntity studentSection);
        Task<ResultModel> DeleteStudentSectionPermanent(int id, int user);
        Task<ResultModel> DeleteStudentSectionTemporary(int id, int user);
        Task<ResultModel> RetrieveStudentSection(studentSectionEntity studentSection);
        Task<studentSecPagedResult> GetAllStudentSection(int pageNo, int pageSize, string keyword);
        Task<studentSecPagedResult> ExportSection(string keyword);
        Task<BatchUploadResponse> BatchUpload(ICollection<studentSectionBatchUploadVM> sections, int user, int uploadID, int row);


        Task<sectionSchedulePagedResult> GetAllSchedule(int sectionId, int pageNo, int pageSize, string keyword);
        Task<ResultModel> AddSectionSchedule(sectionScheduleEntity sectionSchedule);
        Task<ResultModel> UpdateSectionSchedule(sectionScheduleEntity sectionSchedule);
        Task<ResultModel> DeleteSectionSchedule(int id, int user);
        Task<sectionScheduleEntity> GetSectionScheduleById(int id);
    }
}
