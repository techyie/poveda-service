using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.IServices
{
    public interface ISectionService
    {
        Task<ICollection<sectionEntity>> GetAll();
        Task<sectionEntity> GetById(int id);
        Task<ICollection<sectionEntity>> GetByYearLevel(int id);
        Task<ICollection<sectionEntity>> DuplicateRecordChecker(string name, string desc, int yearid, int educid, int campusid);
        Task<ICollection<educationalLevelEntity>> CheckboxChecker(int value1, int value2); 
        Task<ICollection<yearLevelEntity>> CheckboxCheckerOne(int value1, int value2);
        Task<ICollection<sectionEntity>> GetByYearLevelWithActiveStatus(int id);
        Task AddSection(sectionEntity section, int user);
        Task UpdateSection(sectionEntity section, int user);
        Task DeleteSection(long id, int user);
        Task<BatchUploadResponse> BatchUpload(ICollection<studentSectionBatchUploadVM> sections, int user, int uploadID, int row);

        Task<studentSecPagedResult> GetAllSectionList(int pageNo, int pageSize, string keyword);

        Task<studentSecPagedResult> ExportSections(string keyword);

        Task<studentSectionEntity> GetSection(string name, int yearlevelid, int educid, int campusid, int id);
        Task<studentSectionEntity> GetSectionById(int id);
        //2019-07-17 - START
        Task<ICollection<personEntity>> GetCountSectionIfActive(int id);
        //2019-07-17 - END
    }
}
