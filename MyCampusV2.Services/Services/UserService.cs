using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyCampusV2.Common;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices;
using MyCampusV2.Common.ViewModels.UserViewModel;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Common.Helpers;

namespace MyCampusV2.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly AppSettings _appSettings;
        private DirectorySearcher dirSearch = null;

        public UserService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user, IOptions<AppSettings> app) : base(unitOfWork, audit, user) {
            _appSettings = app.Value;
            dirSearch = null;
        }

        private IQueryable<userEntity> GetData(IQueryable<userEntity> query)
        {
            //return !_user.MasterAccess ? query.Where(o => o.PersonEntity.CampusEntity.Campus_ID == _user.Campus) : query.Where(o => o.UserRoleEntity.Role_ID != 1);
            return !_user.MasterAccess ? query.Where(o => o.PersonEntity.CampusEntity.Campus_ID == _user.Campus) : query.Where(o => o.Role_ID != 1);
        }

        public personEntity VerifyEmail(string email)
        {
            return _unitOfWork.UserRepository.VerifyEmail(email);
        }

        public userEntity VerifyUserAccountRole(long id)
        {
            return _unitOfWork.UserRepository.VerifyUserAccountRole(id);
        }

        public async Task<userEntity> AuthenticateAdmin(string username, string password)
        {
            try
            {
                return await _unitOfWork.UserRepository.AuthenticateAdmin(username, password);
            }
            catch (Exception ex)
            {
                return null;
            } 
		}

        public userEntity Authenticate(string username, string password)
        {
            var user = _unitOfWork.UserRepository.authenticate(username, password).Result;

            if (user == null)
                return null;

            return user;
        }

        public async Task<ICollection<userEntity>> GetUserList()
        {
            //return await GetData(_unitOfWork.UserRepository.GetUserList()).ToListAsync();
            return await _unitOfWork.UserRepository.GetUserList().ToListAsync();
        }

        public async Task<ICollection<formEntity>> GetUserAccess2(string username)
        {
            return await _unitOfWork.UserRepository.GetUserAccess2(username);
        }
        public async Task<ICollection<userEntity>> GetAll()
        {
            return await _unitOfWork.UserRepository.GetAllAsyn();
        }

        public async Task<userEntity> GetById(int id)
        {
            try
            {
                return await _unitOfWork.UserRepository.GetUserList().Where(x => x.User_ID == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<userEntity> AuthenticateAccount(string username)
        {
            var user = await _unitOfWork.UserRepository.AuthenticateAccount(username);

            if (user == null)
                return null;

            return user;        
		}


        public async Task AddUserRole(userRoleEntity userrole)
        {
            try
            {
                await _unitOfWork.UserRoleRepository.AddAsyn(userrole);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateRoleUser(int user_ID, int role_ID)
        {
			await _unitOfWork.UserRepository.UpdateUserRole(user_ID, role_ID);
            //var UserAccount = await _unitOfWork.UserRepository.GetUserList().Where(x => x.User_ID == userrole.User_ID).FirstOrDefaultAsync();
            //await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Batch_Upload, string.Format("Updated: User: {0}, Role: {1}", UserAccount.User_Name, UserAccount.RoleEntity.Role_Name));
        }

        public async Task<userEntity> GetUserId(string username)
        {
            return await _unitOfWork.UserRepository.FindAsync(q => q.User_Name == username);        
		}

        public async Task AddUserWithDefaultRole(userEntity user)
        {
            try
            {
                await _unitOfWork.UserRepository.AddUserWithDefaultRole(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AddUser(userEntity user)
        {
            try
            {
                await _unitOfWork.UserRepository.AddAsyn(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateUser(userEntity user, int id)
        {
            try
            {
                user.User_ID = id;
                await _unitOfWork.UserRepository.UpdateAsyn(user, user.User_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteUser(int id)
        {
            try
            {
                userEntity user = await _unitOfWork.UserRepository.GetAsync(id);
                user.IsActive = false;
                await _unitOfWork.UserRepository.UpdateAsyn(user, user.User_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public personEntity GetPersonCampusId(long id)
        {
            return _unitOfWork.UserRepository.GetPersonCampusId(id);
        }


        public LoginModel ValidateAdAccount(string username, string password, string domain, string url)
        {
            LoginModel _loginModel = new LoginModel();

            _loginModel.Username = username;
            _loginModel.Password = password;
            _loginModel.HasRole = false;
            _loginModel.ValidLogin = false;

            try
            {
                SearchResult sr = null;

                if (username.Trim().IndexOf("@") > 0)
                    sr = SearchByEmail(GetDirectorySearcher(username, password, domain, url), username.Trim());
                else
                    sr = SearchByUsername(GetDirectorySearcher(username, password, domain, url), username.Trim());

                if (sr != null)
                {
                    _loginModel.ValidLogin = true;

                    _loginModel.mail = sr.GetDirectoryEntry().Properties["mail"].Value.ToString();
                }
                else
                {
                    _loginModel.ValidLogin = false;
                }

                return _loginModel;
            }
            catch(Exception ex)
            {
                ErrorLogging errorLogging = new ErrorLogging();
                errorLogging.WriteError(ex);

                return _loginModel;
            }
        }

        private SearchResult SearchByUsername(DirectorySearcher ds, string username)
        {
            ds.Filter = "(&((&(objectCategory=Person)(objectClass=User)))(samaccountname=" + username + "))";

            ds.Asynchronous = true;
            ds.SearchScope = SearchScope.Subtree;
            ds.ServerTimeLimit = TimeSpan.FromSeconds(120);

            SearchResult userObject = ds.FindOne();

            if (userObject != null)
                return userObject;
            else
                return null;
        }

        private SearchResult SearchByEmail(DirectorySearcher ds, string email)
        {
            ds.Filter = "(&((&(objectCategory=Person)(objectClass=User)))(mail=" + email + "))";

            ds.Asynchronous = true;
            ds.SearchScope = SearchScope.Subtree;
            ds.ServerTimeLimit = TimeSpan.FromSeconds(120);

            SearchResult userObject = ds.FindOne();

            if (userObject != null)
                return userObject;
            else
                return null;
        }

        private DirectorySearcher GetDirectorySearcher(string username, string password, string domain, string url)
        {
            if (dirSearch == null)
            {
                try
                {
                    dirSearch = null;
                    dirSearch = new DirectorySearcher(new DirectoryEntry("LDAP://" + domain, username, password));
                }
                catch (DirectoryServicesCOMException e)
                {
                    ErrorLogging errorLogging = new ErrorLogging();
                    errorLogging.Write_Message(e.Message.ToString());

                    e.Message.ToString();
                }

                if (dirSearch == null)
                {
                    try
                    {
                        dirSearch = null;
                        dirSearch = new DirectorySearcher(new DirectoryEntry("LDAP://" + url, username, password));
                    }
                    catch (DirectoryServicesCOMException e)
                    {
                        ErrorLogging errorLogging = new ErrorLogging();
                        errorLogging.Write_Message(e.Message.ToString());

                        e.Message.ToString();
                    }
                }

                return dirSearch;
            }
            else
            {
                return dirSearch;
            }
        }

        public async Task<userPagedResult> GetAllUsers(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.UserRepository.GetAllUsers(pageNo, pageSize, keyword);
        }
    }
}
