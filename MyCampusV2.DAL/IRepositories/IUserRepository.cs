using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Common.ViewModels;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IUserRepository : IBaseRepository<userEntity>
    { 
        IQueryable<userEntity> GetUserList();
        Task<ICollection<formEntity>> GetUserAccess2(string username);
        Task<userEntity> authenticate(string username, string password);
        Task<userEntity> AuthenticateAdmin(string username, string password);
        Task<userEntity> GetById(int id);
        personEntity VerifyEmail(string email);
        userEntity VerifyUserAccountRole(long id);
        personEntity GetPersonCampusId(long id);
        Task<userEntity> AuthenticateAccount(string username);
        Task UpdateUserRole(int user_ID, int role_ID);
        Task AddUserWithDefaultRole(userEntity user);

        Task<userPagedResult> GetAllUsers(int pageNo, int pageSize, string keyword);
    }
}
