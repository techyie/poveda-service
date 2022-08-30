using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCampusV2.Common.ViewModels;

namespace MyCampusV2.IServices
{
    public interface IRoleService
    {
        Task<ICollection<roleEntity>> GetAll();
        Task<ICollection<roleEntity>> GetAllIsActive();
        Task<ICollection<roleEntity>> GetAllIsActiveNoGuard();
        Task<roleEntity> GetById(int id);
        Task<ICollection<roleEntity>> DuplicateRecordChecker(string name, string description);
        Task<ICollection<formEntity>> GetModules();
        Task<ICollection<rolePermissionEntity>> GetRoleModules(int id);

        Task<ResultModel> AddRole(roleEntity role);
        Task<ResultModel> UpdateRole(roleEntity role);
        Task<ResultModel> DeleteRole(int id, int user);
        Task<ResultModel> RetrieveRole(roleEntity role);

        Task UpdateRoleModules(int role, string[] modules, int user);
        Task UpdateRoleAccess(int role, string[] access, int user);
        Task<userAccessEntity> AuthorizedRoles(int role, int form);
        Task<rolePermissionEntity> AuthorizedRole(int role, int form);
        Task<bool> IsAuthorized(int role, string form, string method);

        Task<rolePagedResult> GetAllRoles(int pageNo, int pageSize, string keyword);
        Task<ICollection<userRoleEntity>> GetCountRoleIfActive(int id);
    }
}
