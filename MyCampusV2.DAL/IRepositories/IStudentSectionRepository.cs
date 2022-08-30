using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IStudentSectionRepository : IBaseRepository<studentSectionEntity>
    {
        Task<IList<studentSectionEntity>> GetStudentSectionsUsingYearSectionId(int id);
        Task<studentSectionEntity> GetStudentSectionById(int id);
        Task<studentSecPagedResult> GetAllStudentSection(int pageNo, int pageSize, string keyword);
        Task<Boolean> UpdateWithBoolReturn(studentSectionEntity section, int user);
        Task<Boolean> AddWithBoolReturn(studentSectionEntity section, int user);
    }
}
