using System;
using MyCampusV2.Common.ViewModels;
using System.Collections.Generic;
using System.Linq;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.DAL.Context;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyCampusV2.DAL.Repositories
{
    public class RoleRepository : BaseRepository<roleEntity>, IRoleRepository
    {
        private readonly MyCampusCardContext context;

        public RoleRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<roleEntity> GetByID(int id)
        {
            return await _context.RoleEntity.Where(c => c.Role_ID == id).FirstOrDefaultAsync();
        }

        public async Task<ICollection<roleEntity>> DuplicateRecordChecker(string name, string description)
        {
            return await _context.RoleEntity.Where(c => c.Role_Name == name).ToListAsync();
        }

        public IQueryable<roleEntity> GetAllRole()
        {
            return _context.RoleEntity;
        }

        public IQueryable<roleEntity> GetAllRoleIsActive()
        {
            return _context.RoleEntity.Where(role => role.IsActive == true).OrderBy(x => x.Role_Name);
        }

        public async Task<ICollection<formEntity>> GetModules() 
        {
            return await _context.FormEntity.Where(x => x.IsTitle == false && x.Form_ID != 1 && x.Form_Code != "CMP101").ToListAsync();
        }

        public async Task<ICollection<rolePermissionEntity>> GetRoleModules(int role)
        {
            return await _context.RolePermissionEntity
                .Include(x => x.FormEntity)
                .Where(r => r.Role_ID == role && r.Form_ID != 1 && r.FormEntity.Administrator == true)
                .ToListAsync();

            //return await _context.UserAccessEntity.Include(x => x.FormEntity).Where(r => r.Permission_ID == role && r.Form_ID != 1 && r.FormEntity.Administrator == true).ToListAsync();
        }

        public async Task UpdateRoleModules(int role, string[] modules)
        {
            try
            {
                _context.RolePermissionEntity.RemoveRange(_context.RolePermissionEntity.Where(x => x.Role_ID == role && x.Form_ID != 1));
                for (int i = 0; i < modules.Length; i++)
                {
                    rolePermissionEntity rolePermission = new rolePermissionEntity
                    {
                        Can_Access = true,
                        Can_Delete = true,
                        Can_Insert = true,
                        Can_Update = true,
                        Form_ID = Convert.ToInt32(modules[i]),
                        Role_ID = role
                    };
                    await _context.RolePermissionEntity.AddAsync(rolePermission);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task UpdateRoleAccess(int role, string[] access)
        {
            try
            {
                for (int i = 0; i < access.Length; i++)
                {
                    var entity = await _context.RolePermissionEntity.Where(x => x.Role_ID == role && x.Form_ID == i).FirstOrDefaultAsync();
                    string accessstr = access[i];
                    if (entity != null)
                    {
                        if (access[i] != null)
                        {
                            if (accessstr == "true")
                            {
                                entity.Can_Access = true;
                                entity.Can_Delete = true;
                                entity.Can_Insert = true;
                                entity.Can_Update = true;
                            }
                            else if (accessstr == "false")
                            {
                                entity.Can_Access = true;
                                entity.Can_Delete = false;
                                entity.Can_Insert = false;
                                entity.Can_Update = false;
                            }
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        public async Task<userAccessEntity> AuthorizedRoles(int role, int form)
        {
            //return await _context.UserAccessEntity.Where(x => x.Form_ID == form && x.Role_ID == role).SingleOrDefaultAsync();
            return await _context.UserAccessEntity
                .Include(a => a.RolePermissionEntity)
                .Include(b => b.RolePermissionEntity.RoleEntity)
                .Where(x => x.Form_ID == form && x.RolePermissionEntity.RoleEntity.Role_ID == role).SingleOrDefaultAsync();

            //return await _context.UserAccessEntity.Where(x => x.Form_ID == form && x.Permission_ID == role).SingleOrDefaultAsync();
        }

        public async Task<rolePermissionEntity> AuthorizedRole(int role, int form)
        {
            return await _context.RolePermissionEntity
                .Include(a => a.RoleEntity)
                .Where(x => x.Form_ID == form && x.RoleEntity.Role_ID == role).SingleOrDefaultAsync();
        }

        public async Task<bool> AuthorizedAccess(int role, string form)
        {
            //return await _context.UserAccessEntity
            //      .Include(b => b.FormEntity)
            //      .Where(a => a.Role_ID == role
            //     && a.FormEntity.Form_Name.Trim() == form
            //     && a.Can_Access
            //  ).AnyAsync();

            //return await _context.UserAccessEntity
            //    .Include(b => b.FormEntity)
            //    .Include(a => a.RolePermissionEntity)
            //    .Include(b => b.RolePermissionEntity.RoleEntity)
            //    .Where(a => a.RolePermissionEntity.RoleEntity.Role_ID == role
            //         && a.FormEntity.Form_Name.Trim() == form
            //         && a.Can_Access).AnyAsync();

            return await _context.RolePermissionEntity
                .Include(a => a.FormEntity)
                .Include(b => b.RoleEntity)
                .Where(a => a.RoleEntity.Role_ID == role
                     && a.FormEntity.Form_Name.Replace(" ", "").Trim() == form
                     && a.Can_Access).AnyAsync();

            //.Include(b => b.FormEntity)
            //      .Where(a => a.Permission_ID == role
            //     && a.FormEntity.Form_Name.Trim() == form
            //     && a.Can_Access
        }

        public async Task<bool> AuthorizedInsert(int role, string form)
        {
            //return await _context.UserAccessEntity
            //    .Include(b => b.FormEntity)
            //    .Where(a => a.Role_ID == role
            //   && a.FormEntity.Form_Name.Replace(" ", "").Trim() == form
            //   && a.Can_Insert
            //).AnyAsync();

            //return await _context.UserAccessEntity
            //    .Include(b => b.FormEntity)
            //    .Include(a => a.RolePermissionEntity)
            //    .Include(b => b.RolePermissionEntity.RoleEntity)
            //    .Where(a => a.RolePermissionEntity.RoleEntity.Role_ID == role
            //          && a.FormEntity.Form_Name.Replace(" ", "").Trim() == form
            //          && a.Can_Insert).AnyAsync();

            return await _context.RolePermissionEntity
                .Include(a => a.FormEntity)
                .Include(b => b.RoleEntity)
                .Where(a => a.RoleEntity.Role_ID == role
                      && a.FormEntity.Form_Name.Replace(" ", "").Trim() == form
                      && a.Can_Insert == true).AnyAsync();

            //    .Where(a => a.Permission_ID == role
            //   && a.FormEntity.Form_Name.Replace(" ", "").Trim() == form
            //   && a.Can_Insert
            //).AnyAsync();
        }

        public async Task<bool> AuthorizedUpdate(int role, string form)
        {
            // return await _context.UserAccessEntity
            //    .Include(b => b.FormEntity)
            //    .Where(a => a.Role_ID == role
            //   && a.FormEntity.Form_Name.Replace(" ", "").Trim() == form
            //   && a.Can_Update
            //).AnyAsync();

            //return await _context.UserAccessEntity
            //    .Include(b => b.FormEntity)
            //    .Include(a => a.RolePermissionEntity)
            //    .Include(b => b.RolePermissionEntity.RoleEntity)
            //    .Where(a => a.RolePermissionEntity.RoleEntity.Role_ID == role
            //          && a.FormEntity.Form_Name.Replace(" ", "").Trim() == form
            //          && a.Can_Update).AnyAsync();

            return await _context.RolePermissionEntity
                .Include(a => a.FormEntity)
                .Include(b => b.RoleEntity)
                .Where(a => a.RoleEntity.Role_ID == role
                      && a.FormEntity.Form_Name.Replace(" ", "").Trim() == form
                      && a.Can_Update == true).AnyAsync();

            //.Include(b => b.FormEntity)
            //   .Where(a => a.Permission_ID == role
            //  && a.FormEntity.Form_Name.Replace(" ", "").Trim() == form
            //  && a.Can_Update
        }

        public async Task<bool> AuthorizedDelete(int role, string form)
        {
            // return await _context.UserAccessEntity
            //    .Include(b => b.FormEntity)
            //    .Where(a => a.Role_ID == role
            //   && a.FormEntity.Form_Name.Replace(" ", "").Trim() == form
            //   && a.Can_Delete
            //).AnyAsync();

            //return await _context.UserAccessEntity
            //    .Include(a => a.RolePermissionEntity)
            //    .Include(b => b.RolePermissionEntity.RoleEntity)
            //    .Where(a => a.RolePermissionEntity.RoleEntity.Role_ID == role
            //         && a.FormEntity.Form_Name.Replace(" ", "").Trim() == form
            //         && a.Can_Delete).AnyAsync();

            return await _context.RolePermissionEntity
                .Include(b => b.RoleEntity)
                .Where(a => a.RoleEntity.Role_ID == role
                     && a.FormEntity.Form_Name.Replace(" ", "").Trim() == form
                     && a.Can_Delete).AnyAsync();
            
            //.Include(b => b.FormEntity)
            //   .Where(a => a.Permission_ID == role
            //  && a.FormEntity.Form_Name.Replace(" ", "").Trim() == form
            //  && a.Can_Delete
           //).AnyAsync();
        }

        public async Task<rolePagedResult> GetAllRoles(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new rolePagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = _context.RoleEntity.Where(r => (r.Role_ID != 1)).Count();
                else
                    result.RowCount = _context.RoleEntity.Where(r => (r.Role_ID != 1) && r.Role_Name.Contains(keyword)).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.roles = await _context.RoleEntity
                        .Where(r => (r.Role_ID != 1))
                        .OrderBy(r => r.Role_Name)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.roles = await _context.RoleEntity.Where(r => (r.Role_ID != 1) && r.Role_Name.Contains(keyword))
                        .OrderBy(r => r.Role_Name)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ICollection<userRoleEntity>> GetCountRoleIfActive(int id)
        {
            return await _context.UserRoleEntity.Where(role => role.Role_ID == id).ToListAsync();
        }
    }
}
