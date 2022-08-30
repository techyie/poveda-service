using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IEducationLevelRepository : IBaseRepository<educationalLevelEntity>
    {
        Task<IList<educationalLevelEntity>> GetEducationalLevelsUsingCampusId(int id);
        Task<IList<educationalLevelEntity>> GetEducationalLevelsCollegeOnlyUsingCampusId(int id);
        Task<educationalLevelEntity> GetEducationalLevelById(int id);
        Task<educlevelPagedResult> GetAllEduclevel(int pageNo, int pageSize, string keyword);
        Task<Boolean> UpdateWithBoolReturn(educationalLevelEntity educLevel, int user);
        Task<Boolean> AddWithBoolReturn(educationalLevelEntity educLevel, int user);
    }
}
