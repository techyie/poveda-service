using MyCampusV2.Common.ViewModels.UserViewModel;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyCampusV2.Common.ViewModels;

namespace MyCampusV2.IServices
{
    public interface IUserService
    {
        Task<ICollection<userEntity>> GetAll();
        Task<ICollection<userEntity>> GetUserList();
        Task<userEntity> GetById(int id);
        Task AddUser(userEntity user);
        Task UpdateUser(userEntity user, int id);
        Task DeleteUser(int id);
        userEntity Authenticate(string username, string password);
        Task<userEntity> AuthenticateAdmin(string username, string password);
        Task<ICollection<formEntity>> GetUserAccess2(string username);
        LoginModel ValidateAdAccount(string username, string password, string domain, string url);
        personEntity VerifyEmail(string email);
        userEntity VerifyUserAccountRole(long id);
        personEntity GetPersonCampusId(long id);
        Task<userEntity> AuthenticateAccount(string username);
        Task AddUserRole(userRoleEntity userrole);
        Task UpdateRoleUser(int user_id, int role_id);
        Task<userEntity> GetUserId(string username);
        Task AddUserWithDefaultRole(userEntity user);

        Task<userPagedResult> GetAllUsers(int pageNo, int pageSize, string keyword);
    }
}