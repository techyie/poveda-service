using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface ISectionRepository : IBaseRepository<sectionEntity>
    {
        Task<sectionEntity> GetByID(int id);
        IQueryable<sectionEntity> GetAllSection();
        Task<ICollection<sectionEntity>> GetByYearLevel(int id);
        Task<sectionEntity> FindData(string section, long yearlevelid);
        Task<studentSecPagedResult> GetAllSectionList(int pageNo, int pageSize, string keyword);
        Task<ICollection<sectionEntity>> DuplicateRecordChecker(string name, string desc, int yearid, int educid, int campusid);
        Task<ICollection<educationalLevelEntity>> CheckboxChecker(int value1, int value2);
        Task<ICollection<yearLevelEntity>> CheckboxCheckerOne(int value1, int value2);
        Task<ICollection<personEntity>> GetCountSectionIfActive(int id);
    }
}
