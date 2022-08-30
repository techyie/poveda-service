using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Common.Helpers;

namespace MyCampusV2.Services
{
    public class RoleService : BaseService, IRoleService
    {
        private string _formName = "User Role";

        public RoleService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<ICollection<roleEntity>> GetAll()
        {
            return await _unitOfWork.RoleRepository.GetAllRole().Where(q => q.Role_ID != 1).ToListAsync(); // && q.IsActive == true
        }

        public async Task<ICollection<roleEntity>> GetAllIsActive()
        {
            return await _unitOfWork.RoleRepository.GetAllRoleIsActive().Where(q => q.Role_ID != 1 && q.IsActive == true).ToListAsync();
        }

        public async Task<ICollection<roleEntity>> GetAllIsActiveNoGuard()
        {
            return await _unitOfWork.RoleRepository.GetAllRoleIsActive().Where(q => q.Role_ID != 1 && q.Role_ID != 3 && q.IsActive == true).ToListAsync();
        }

        public async Task<ICollection<formEntity>> GetModules()
        {
            return await Task.Run(async () => BuildTreeAndReturnRootNodes(await _unitOfWork.RoleRepository.GetModules()));
        }

        public async Task<ICollection<rolePermissionEntity>> GetRoleModules(int id)
        {
            return await _unitOfWork.RoleRepository.GetRoleModules(id);
        }

        static ICollection<formEntity> BuildTreeAndReturnRootNodes(ICollection<formEntity> items)
        {
            var lookup = items.ToLookup(i => i.Form_ID);
            foreach (var item in items)
            {
                if (item.Parent_ID != null)
                {
                    var parent = lookup[item.Parent_ID.Value].FirstOrDefault();
                    parent.children.Add(item);
                }
            }
            return items.Where(i => i.Parent_ID == null).ToList();
        }

        public async Task<roleEntity> GetById(int id)
        {
            return await _unitOfWork.RoleRepository.GetByID(id);
        }

        public async Task<ICollection<roleEntity>> DuplicateRecordChecker(string name, string description)
        {
            return await _unitOfWork.RoleRepository.DuplicateRecordChecker(name, description);
        }

        //public async Task AddRole(roleEntity roles, int user)
        //{
        //    try
        //    {
        //        roles.Added_By = user;
        //        roles.Updated_By = user;
        //        roles.Date_Time_Added = DateTime.Now;
        //        roles.Last_Updated = DateTime.Now;
        //        await _unitOfWork.RoleRepository.AddAsyn(roles);
        //        await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.User_Role, string.Format("Added: Role Name: {0}", roles.Role_Name));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<ResultModel> AddRole(roleEntity role)
        {
            try
            {
                role.IsActive = true;
                role.ToDisplay = true;

                var exist = await _unitOfWork.RoleRepository.FindAsync(q => q.Role_Name == role.Role_Name);

                if (exist != null)
                    return CreateResult("409", ROLE_EXIST, false);

                var data = await _unitOfWork.RoleRepository.AddAsyncWithBase(role);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(role.Added_By, (int)Form.User_Role, "Add " + _formName, "INSERT", false, "Failed: " + role.Role_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(role.Added_By, (int)Form.User_Role, "Add " + _formName, "INSERT", true, "Success: " + role.Role_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateRole(roleEntity role)
        {
            try
            {
                var data = await _unitOfWork.RoleRepository.UpdateAsyncWithBase(role, role.Role_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(role.Added_By, (int)Form.User_Role, "Update " + _formName, "UPDATE", false, "Failed: " + role.Role_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(role.Updated_By, (int)Form.User_Role, "Update " + _formName, "UPDATE", true, "Success: User Role Name: " + role.Role_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageUpdate, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }
        public async Task<ResultModel> DeleteRole(int id, int user)
        {
            try
            {
                roleEntity role = await GetRole(id);
                role.Updated_By = user;

                var data = await _unitOfWork.RoleRepository.DeleteAsyncTemporary(role, role.Role_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(role.Updated_By, (int)Form.User_Role, "Deactivate " + _formName, "DEACTIVATE", false, "Failed: " + role.Role_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(role.Updated_By, (int)Form.User_Role, "Deactivate " + _formName, "DEACTIVATE", true, "Success: " + role.Role_Name, DateTime.UtcNow.ToLocalTime()));
                
                return CreateResult("200", _formName + Constants.SuccessMessageTemporaryDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }
        public async Task<ResultModel> RetrieveRole(roleEntity role)
        {
            try
            {
                var newEntity = await _unitOfWork.RoleRepository.GetAsync(role.Role_ID);
                
                newEntity.Updated_By = role.Updated_By;

                var data = await _unitOfWork.RoleRepository.RetrieveAsync(newEntity, newEntity.Role_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.User_Role, "Activate User Role", "ACTIVATE USER ROLE", false, "Failed: " + newEntity.Role_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", _formName, false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.User_Role, "Activate User Role", "ACTIVATE USER ROLE", true, "Success: " + newEntity.Role_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", _formName + Constants.SuccessMessageRetrieve, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        //public async Task UpdateRole(roleEntity roles, int user)
        //{
        //    try
        //    {
        //        //roleEntity oldRole = await GetRole(roles.ID);
        //        //roles.Added_By = oldRole.Added_By;
        //        roles.Updated_By = user;
        //        //roles.Date_Time_Added = oldRole.Date_Time_Added;
        //        roles.Last_Updated = DateTime.Now;
        //        await _unitOfWork.RoleRepository.UpdateAsyn(roles, roles.Role_ID);
        //        await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.User_Role, string.Format("Updated: Role Name: {0}", roles.Role_Name));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public async Task DeleteRole(long id, int user)
        //{
        //    try
        //    {
        //        roleEntity roles = await _unitOfWork.RoleRepository.GetAsync(id);
        //        roles.Added_By = user;
        //        roles.Updated_By = user;
        //        roles.Date_Time_Added = DateTime.Now;
        //        roles.Last_Updated = DateTime.Now;
        //         if(roles.IsActive){
        //            roles.IsActive = false;
        //        }else{
        //            roles.IsActive = true;
        //        }
        //        await _unitOfWork.RoleRepository.UpdateAsyn(roles, roles.Role_ID);
        //        await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.User_Role, string.Format("Updated Role {1} status to {0}", roles.IsActive ? "Active" : "Inactive", roles.Role_Name));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<ICollection<userRoleEntity>> GetCountRoleIfActive(int id)
        {
            return await _unitOfWork.RoleRepository.GetCountRoleIfActive(id);
        }

        public async Task<roleEntity> GetRole(long id)
        {
            return await _unitOfWork.RoleRepository.FindAsync(x => x.Role_ID == id);
        }

        public async Task UpdateRoleModules(int role, string[] modules, int user)
        {
            roleEntity roles = await _unitOfWork.RoleRepository.GetAsync((int)role);
            await _unitOfWork.RoleRepository.UpdateRoleModules(role, modules);

            //await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.User_Role, string.Format("Updated Permission of Role {0}", roles.Role_Name));
        }

        public async Task UpdateRoleAccess(int role, string[] access, int user)
        {
            //tbl_roles roles = await _unitOfWork.RoleRepository.GetAsync((long)role);
            await _unitOfWork.RoleRepository.UpdateRoleAccess(role, access);
        }

        public async Task<userAccessEntity> AuthorizedRoles(int role, int form)
        {
            return await _unitOfWork.RoleRepository.AuthorizedRoles(role, form);
        }

        public async Task<rolePermissionEntity> AuthorizedRole(int role, int form)
        {
            return await _unitOfWork.RoleRepository.AuthorizedRole(role, form);
        }

        public async Task<bool> IsAuthorized(int role, string form, string method)
        {
            bool access = false;
            try{
                if (method == "GET")
                    access = await _unitOfWork.RoleRepository.AuthorizedAccess(role, form);
                if (method == "POST")
                    access = await _unitOfWork.RoleRepository.AuthorizedInsert(role, form);
                if (method == "PUT")
                    access = await _unitOfWork.RoleRepository.AuthorizedUpdate(role, form);
                if (method == "DELETE")
                    access = await _unitOfWork.RoleRepository.AuthorizedDelete(role, form);

            }
            catch (Exception ex){
                throw ex;
            }

            return access;
        }

        public async Task<rolePagedResult> GetAllRoles(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.RoleRepository.GetAllRoles(pageNo, pageSize, keyword);
        }
    }
}
