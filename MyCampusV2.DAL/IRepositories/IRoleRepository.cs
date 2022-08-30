using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IRoleRepository : IBaseRepository<roleEntity>
    {
        Task<roleEntity> GetByID(int id);
        Task<ICollection<roleEntity>> DuplicateRecordChecker(string name, string description);
        IQueryable<roleEntity> GetAllRole();
        IQueryable<roleEntity> GetAllRoleIsActive();
        Task<ICollection<formEntity>> GetModules();
        Task<ICollection<rolePermissionEntity>> GetRoleModules(int role);
        Task UpdateRoleModules(int role, string[] modules);
        Task UpdateRoleAccess(int role, string[] access);
        Task<userAccessEntity> AuthorizedRoles(int role, int form);
        Task<rolePermissionEntity> AuthorizedRole(int role, int form);
        Task<bool> AuthorizedAccess(int role, string form);
        Task<bool> AuthorizedInsert(int role, string form);
        Task<bool> AuthorizedUpdate(int role, string form);
        Task<bool> AuthorizedDelete(int role, string form);

        Task<rolePagedResult> GetAllRoles(int pageNo, int pageSize, string keyword);
        Task<ICollection<userRoleEntity>> GetCountRoleIfActive(int id);
    }
}
