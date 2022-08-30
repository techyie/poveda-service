using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Common.ViewModels;

namespace MyCampusV2.DAL.Repositories
{
    public class UserRepository : BaseRepository<userEntity>, IUserRepository
    {
        private readonly MyCampusCardContext context;

        public UserRepository(MyCampusCardContext Context) : base(Context)
        {
            this.context = Context;
        }

        public IQueryable<userEntity> GetUserList()
        {
            try
            {
                //var result = _context.userEntity.
                //    Include(x => x.PersonEntity).
                //    ThenInclude(pos => pos.PositionEntity).
                //    ThenInclude(dep => dep.DepartmentEntity).
                //    ThenInclude(d => d.CampusEntity).
                //    Include(us => us.PersonEntity.userEntity.userRoleEntity).
                //    ThenInclude(ro => ro.roleEntity).
                //    Include(us => us.PersonEntity.CampusEntity);

                //return _context.UserEntity.
                //    Include(p => p.PersonEntity).
                //    Include(ur => ur.UserRoleEntity).
                //    ThenInclude(r => r.RoleEntity);

                var result = _context.UserEntity
                    .Include(p => p.PersonEntity)
                    .Include(r => r.RoleEntity);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ICollection<formEntity>> GetUserAccess2(string username)
        {
            var access = context.FormEntity.ToListAsync();

            return await access;
        }

        public async Task AddUserWithDefaultRole(userEntity user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    user.Added_By = 1;
                    user.Date_Time_Added = DateTime.Now;
                    user.Updated_By = 1;
                    user.Last_Updated = DateTime.Now;

                    await _context.UserEntity.AddAsync(user);

                    //userrole.User_ID = user.User_ID;

                    //await _context.UserRoleEntity.AddAsync(userrole);

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task UpdateUserRole(int user_id, int role_id)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // update tbl_user: Role_ID
                    var result = _context.UserEntity.SingleOrDefault(x => x.User_ID == user_id);

                    if (result != null)
                    {
                        result.Role_ID = role_id;
                    }

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();

                    // delete and insert permissions from tbl_role_permission to tbl_user_access
                    _context.UserAccessEntity.RemoveRange(_context.UserAccessEntity.Where(x => x.User_ID == user_id && x.Form_ID != 1));
                    var modules = await _context.RolePermissionEntity.Where(x => x.Role_ID == role_id && x.Form_ID != 1).ToListAsync();
                    foreach (rolePermissionEntity module in modules)
                    {
                        using (IDbContextTransaction dbcxtransactionrole = _context.Database.BeginTransaction())
                        {
                            userAccessEntity userAccess = new userAccessEntity();

                            userAccess.User_ID = user_id;
                            userAccess.Form_ID = module.Form_ID;
                            userAccess.Permission_ID = module.Permission_ID;
                            userAccess.Can_Access = module.Can_Access;
                            userAccess.Can_Delete = module.Can_Delete;
                            userAccess.Can_Insert = module.Can_Insert;
                            userAccess.Can_Update = module.Can_Update;

                            await _context.UserAccessEntity.AddAsync(userAccess);

                            await _context.SaveChangesAsync();

                            dbcxtransactionrole.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    throw ex;
                }
            }
        }
        public async Task<userEntity> authenticate(string username, string password)
        {
            //var access = context.UserEntity.Include(a=>a.PersonEntity).Include(b=>b.UserRoleEntity).Where(user => user.User_Name == username && user.User_Password == password && user.IsActive == true).FirstOrDefaultAsync();
            var access = context.UserEntity.Include(a => a.PersonEntity).Where(user => user.User_Name == username && user.User_Password == password && user.IsActive == true).FirstOrDefaultAsync();
            return await access;
        }

        public async Task<userEntity> AuthenticateAdmin(string username, string password)
        {
            try
            {
                //return await context.UserEntity
                //    .Include(a => a.PersonEntity)
                //    .Include(b => b.UserRoleEntity)
                //    .Where(user => user.User_Name == username && user.User_Password == password && user.IsActive == true)
                //    .FirstOrDefaultAsync();

                return await context.UserEntity
                    .Include(a => a.PersonEntity)
                    .Where(user => user.User_Name == username && user.User_Password == password && user.IsActive == true)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<userEntity> AuthenticateAccount(string username)
        {
            if(username.Trim().IndexOf("@") > 0)
                //return await context.UserEntity.Include(a => a.PersonEntity).Include(b => b.UserRoleEntity).Where(user => user.User_Name == username && user.IsActive == true).FirstOrDefaultAsync();
                return await context.UserEntity.Include(a => a.PersonEntity).Where(user => user.User_Name == username && user.IsActive == true).FirstOrDefaultAsync();
            else
                //return await context.userEntity.Include(a => a.PersonEntity).Include(b => b.userRoleEntity).Where(user => user.User_Name.Substring(0, user.User_Name.IndexOf("@")) == username && user.IsActive == true).FirstOrDefaultAsync();
                //return await context.UserEntity.Include(a => a.PersonEntity).Include(b => b.UserRoleEntity).Where(user => user.User_Name == username && user.IsActive == true).FirstOrDefaultAsync();
                return await context.UserEntity.Include(a => a.PersonEntity).Where(user => user.User_Name == username && user.IsActive == true).FirstOrDefaultAsync();
        }
        public async Task<userEntity> GetById(int id)
        {
            //return await _context.UserEntity.Include(x => x.PersonEntity).Include(x => x.UserRoleEntity).Where(user => user.User_ID == id).FirstOrDefaultAsync();
            return await _context.UserEntity.Include(x => x.PersonEntity).Where(user => user.User_ID == id).FirstOrDefaultAsync();
        }

        public personEntity VerifyEmail(string email)
        {
            if (email.Trim().IndexOf("@") > 0)
                return _context.PersonEntity.Where(q => q.Email_Address == email).FirstOrDefault();
            else
                return _context.PersonEntity.Where(q => q.Email_Address.Substring(0, q.Email_Address.IndexOf("@")) == email).FirstOrDefault();
        }
        public userEntity VerifyUserAccountRole(long id)
        {
            return _context.UserEntity.Where(q => q.Person_ID == id).FirstOrDefault();
        }

        public personEntity GetPersonCampusId(long id)
        {
            return _context.PersonEntity.Include(q => q.PositionEntity).ThenInclude(w => w.DepartmentEntity).ThenInclude(c => c.CampusEntity).Where(v => v.Person_ID == id).FirstOrDefault();
        }

        //public async Task AddNewUser(userEntity user)
        //{
        //    using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            personInfo.Added_By = user;
        //            //personInfo.Date_Time_Added = DateTime.UtcNow;
        //            personInfo.Date_Time_Added = DateTime.Now;

        //            employeeInfo.Added_By = user;
        //            employeeInfo.Date_Time_Added = DateTime.Now;

        //            personInfo.Middle_Name = (personInfo.Middle_Name == null ? string.Empty : personInfo.Middle_Name);
        //            personInfo.Prefix = (personInfo.Prefix == null ? string.Empty : personInfo.Prefix);
        //            personInfo.Suffix = (personInfo.Suffix == null ? string.Empty : personInfo.Suffix);
        //            personInfo.Email_Address = (personInfo.Email_Address == null ? string.Empty : personInfo.Email_Address);

        //            await _context.PersonEntity.AddAsync(personInfo);

        //            employeeInfo.Person_ID = personInfo.Person_ID;
        //            await _context.PersonEntity_employee.AddAsync(employeeInfo);

        //            emergencyInfo.Connected_Person_ID = personInfo.Person_ID;
        //            await _context.tbl_emergency_contact.AddAsync(emergencyInfo);

        //            govInfo.Person_ID = personInfo.Person_ID;
        //            await _context.tbl_gov_ids.AddAsync(govInfo);

        //            await _context.SaveChangesAsync();

        //            dbcxtransaction.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            dbcxtransaction.Rollback();
        //            throw ex;
        //        }
        //    }
        //}

        public async Task<userPagedResult> GetAllUsers(int pageNo, int pageSize, string keyword) 
        {
            try
            {
                var result = new userPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                {
                    result.RowCount = _context.UserEntity
                        .Include(x => x.PersonEntity)
                            .ThenInclude(pos => pos.PositionEntity)
                            .ThenInclude(dep => dep.DepartmentEntity)
                            .ThenInclude(d => d.CampusEntity)
                        .Include(us => us.PersonEntity.UserEntity.RoleEntity)
                        .Include(us => us.PersonEntity.CampusEntity).Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G")).Count();

                    //result.RowCount = _context.UserEntity
                    //    .Include(x => x.PersonEntity)
                    //    .Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G")).Count();
                }
                else if (keyword.Contains(","))
                {
                    string[] fullname = keyword.Split(",");
                    result.RowCount = _context.UserEntity
                        .Include(x => x.PersonEntity)
                            .ThenInclude(pos => pos.PositionEntity)
                            .ThenInclude(dep => dep.DepartmentEntity)
                            .ThenInclude(d => d.CampusEntity)
                        .Include(us => us.PersonEntity.UserEntity.RoleEntity)
                        .Include(us => us.PersonEntity.CampusEntity).Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G") && (u.PersonEntity.ID_Number.Contains(keyword)
                            || u.User_Name.Contains(keyword)
                            || u.RoleEntity.Role_Name.Contains(keyword)
                            || (u.PersonEntity.First_Name.Contains(fullname[1].Trim()) && u.PersonEntity.Last_Name.Contains(fullname[0].Trim()))
                            || u.PersonEntity.CampusEntity.Campus_Name.Contains(keyword)
                            || u.PersonEntity.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                            || u.PersonEntity.PositionEntity.Position_Name.Contains(keyword))).Count();

                    //result.RowCount = _context.UserEntity
                    //        .Include(x => x.PersonEntity)
                    //        .Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G") && (u.PersonEntity.ID_Number.Contains(keyword)
                    //            || u.User_Name.Contains(keyword)
                    //            || u.UserRoleEntity.RoleEntity.Role_Name.Contains(keyword)
                    //            || (u.PersonEntity.First_Name.Contains(fullname[1].Trim()) && u.PersonEntity.Last_Name.Contains(fullname[0].Trim())))).Count();

                    //result.RowCount = _context.UserEntity
                    //        .Include(x => x.PersonEntity)
                    //        .Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G") && (u.PersonEntity.ID_Number.Contains(keyword)
                    //            || u.User_Name.Contains(keyword)
                    //            || u.RoleEntity.Role_Name.Contains(keyword)
                    //            || (u.PersonEntity.First_Name.Contains(fullname[1].Trim()) && u.PersonEntity.Last_Name.Contains(fullname[0].Trim())))).Count();
                }
                else
                {
                    result.RowCount = _context.UserEntity
                        .Include(x => x.PersonEntity)
                            .ThenInclude(pos => pos.PositionEntity)
                            .ThenInclude(dep => dep.DepartmentEntity)
                            .ThenInclude(d => d.CampusEntity)
                        .Include(us => us.PersonEntity.UserEntity.RoleEntity)
                        .Include(us => us.PersonEntity.CampusEntity).Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G") && (u.PersonEntity.ID_Number.Contains(keyword)
                            || u.User_Name.Contains(keyword)
                            || u.RoleEntity.Role_Name.Contains(keyword)
                            || u.PersonEntity.First_Name.Contains(keyword)
                            || u.PersonEntity.Last_Name.Contains(keyword)
                            || u.PersonEntity.CampusEntity.Campus_Name.Contains(keyword)
                            || u.PersonEntity.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                            || u.PersonEntity.PositionEntity.Position_Name.Contains(keyword))).Count();

                    //result.RowCount = _context.UserEntity
                    //        .Include(x => x.PersonEntity)
                    //        .Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G") && (u.PersonEntity.ID_Number.Contains(keyword)
                    //            || u.User_Name.Contains(keyword)
                    //            || u.UserRoleEntity.RoleEntity.Role_Name.Contains(keyword)
                    //            || u.PersonEntity.First_Name.Contains(keyword)
                    //            || u.PersonEntity.Last_Name.Contains(keyword))).Count();

                    //result.RowCount = _context.UserEntity
                    //            .Include(x => x.PersonEntity)
                    //            .Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G") && (u.PersonEntity.ID_Number.Contains(keyword)
                    //                || u.User_Name.Contains(keyword)
                    //                || u.RoleEntity.Role_Name.Contains(keyword)
                    //                || u.PersonEntity.First_Name.Contains(keyword)
                    //                || u.PersonEntity.Last_Name.Contains(keyword))).Count();
                }

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                {
                    result.users = await _context.UserEntity
                        .Include(x => x.PersonEntity)
                            .ThenInclude(pos => pos.PositionEntity)
                            .ThenInclude(dep => dep.DepartmentEntity)
                            .ThenInclude(d => d.CampusEntity)
                        .Include(us => us.PersonEntity.UserEntity.RoleEntity)
                        .Include(us => us.PersonEntity.CampusEntity).Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G"))
                        .OrderBy(u => u.PersonEntity.ID_Number)
                        .Skip(skip).Take(pageSize).ToListAsync();

                    //result.users = await _context.UserEntity
                    //    .Include(x => x.PersonEntity)
                    //    .Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G"))
                    //    .OrderBy(u => u.PersonEntity.ID_Number)
                    //    .Skip(skip).Take(pageSize).ToListAsync();
                }
                else if (keyword.Contains(","))
                {
                    string[] fullname = keyword.Split(",");
                    result.users = await _context.UserEntity
                        .Include(x => x.PersonEntity)
                            .ThenInclude(pos => pos.PositionEntity)
                            .ThenInclude(dep => dep.DepartmentEntity)
                            .ThenInclude(d => d.CampusEntity)
                        .Include(us => us.PersonEntity.UserEntity.RoleEntity)
                        .Include(us => us.PersonEntity.CampusEntity).Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G") && (u.PersonEntity.ID_Number.Contains(keyword)
                            || u.User_Name.Contains(keyword)
                            || u.RoleEntity.Role_Name.Contains(keyword)
                            || (u.PersonEntity.First_Name.Contains(fullname[1].Trim()) && u.PersonEntity.Last_Name.Contains(fullname[0].Trim()))
                            || u.PersonEntity.CampusEntity.Campus_Name.Contains(keyword)
                            || u.PersonEntity.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                            || u.PersonEntity.PositionEntity.Position_Name.Contains(keyword)))
                            .OrderBy(u => u.PersonEntity.ID_Number)
                            .Skip(skip).Take(pageSize).ToListAsync();

                    //result.users = await _context.UserEntity
                    //    .Include(x => x.PersonEntity)
                    //    .Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G") && (u.PersonEntity.ID_Number.Contains(keyword)
                    //        || u.User_Name.Contains(keyword)
                    //        || u.UserRoleEntity.RoleEntity.Role_Name.Contains(keyword)
                    //        || (u.PersonEntity.First_Name.Contains(fullname[1].Trim()) && u.PersonEntity.Last_Name.Contains(fullname[0].Trim()))))
                    //    .OrderBy(u => u.PersonEntity.ID_Number)
                    //    .Skip(skip).Take(pageSize).ToListAsync();

                    //result.users = await _context.UserEntity
                    //        .Include(x => x.PersonEntity)
                    //        .Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G") && (u.PersonEntity.ID_Number.Contains(keyword)
                    //            || u.User_Name.Contains(keyword)
                    //            || u.RoleEntity.Role_Name.Contains(keyword)
                    //            || (u.PersonEntity.First_Name.Contains(fullname[1].Trim()) && u.PersonEntity.Last_Name.Contains(fullname[0].Trim()))))
                    //        .OrderBy(u => u.PersonEntity.ID_Number)
                    //        .Skip(skip).Take(pageSize).ToListAsync();
                }
                else
                {
                    result.users = await _context.UserEntity
                        .Include(x => x.PersonEntity)
                            .ThenInclude(pos => pos.PositionEntity)
                            .ThenInclude(dep => dep.DepartmentEntity)
                            .ThenInclude(d => d.CampusEntity)
                        .Include(us => us.PersonEntity.UserEntity.RoleEntity)
                        .Include(us => us.PersonEntity.CampusEntity).Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G") && (u.PersonEntity.ID_Number.Contains(keyword)
                            || u.User_Name.Contains(keyword)
                            || u.RoleEntity.Role_Name.Contains(keyword)
                            || u.PersonEntity.First_Name.Contains(keyword)
                            || u.PersonEntity.Last_Name.Contains(keyword)
                            || u.PersonEntity.CampusEntity.Campus_Name.Contains(keyword)
                            || u.PersonEntity.PositionEntity.DepartmentEntity.Department_Name.Contains(keyword)
                            || u.PersonEntity.PositionEntity.Position_Name.Contains(keyword)))
                            .OrderBy(u => u.PersonEntity.ID_Number)
                            .Skip(skip).Take(pageSize).ToListAsync();

                    //result.users = await _context.UserEntity
                    //    .Include(x => x.PersonEntity)
                    //    .Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G") && (u.PersonEntity.ID_Number.Contains(keyword)
                    //        || u.User_Name.Contains(keyword)
                    //        || u.UserRoleEntity.RoleEntity.Role_Name.Contains(keyword)
                    //        || u.PersonEntity.First_Name.Contains(keyword)
                    //        || u.PersonEntity.Last_Name.Contains(keyword)))
                    //    .OrderBy(u => u.PersonEntity.ID_Number)
                    //    .Skip(skip).Take(pageSize).ToListAsync();

                    //result.users = await _context.UserEntity
                    //        .Include(x => x.PersonEntity)
                    //        .Where(u => (u.PersonEntity.Person_Type != "A" && u.PersonEntity.Person_Type != "G") && (u.PersonEntity.ID_Number.Contains(keyword)
                    //            || u.User_Name.Contains(keyword)
                    //            || u.RoleEntity.Role_Name.Contains(keyword)
                    //            || u.PersonEntity.First_Name.Contains(keyword)
                    //            || u.PersonEntity.Last_Name.Contains(keyword)))
                    //        .OrderBy(u => u.PersonEntity.ID_Number)
                    //        .Skip(skip).Take(pageSize).ToListAsync();
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
