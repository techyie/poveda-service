using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface ICollegeRepository : IBaseRepository<collegeEntity>
    {
        Task<IList<collegeEntity>> GetCollegesUsingEducationalLevelId(int id);
        Task<collegeEntity> GetCollegeById(int id);
        Task<collegePagedResult> GetAllCollege(int pageNo, int pageSize, string keyword);
        Task<collegePagedResult> ExportAllColleges(string keyword);

        Task<Boolean> UpdateCollegeWithBoolReturn(collegeEntity college, int user);
        Task<Boolean> AddCollegeWithBoolReturn(collegeEntity college, int user);
    }
}
