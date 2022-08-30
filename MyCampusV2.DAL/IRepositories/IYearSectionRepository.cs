using System;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IYearSectionRepository : IBaseRepository<yearSectionEntity>
    {
        IQueryable<yearSectionEntity> GetYearSections();
        Task<IList<yearSectionEntity>> GetYearSectionsUsingEducationalLevelId(int id);
        Task<yearSectionEntity> GetYearSectionById(int id);
        Task<yearSectionPagedResult> GetAllYearSection(int pageNo, int pageSize, string keyword);
        Task<yearSectionPagedResult> ExportAllYearSections(string keyword);

        Task<Boolean> UpdateYearSectionWithBoolReturn(yearSectionEntity yearsection, int user);
        Task<Boolean> AddYearSectionWithBoolReturn(yearSectionEntity yearsection, int user);
    }
}
