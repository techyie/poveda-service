using Microsoft.EntityFrameworkCore;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace MyCampusV2.DAL.Repositories
{
    public class FormRepository : BaseRepository<formEntity>, IFormRepository
    {
        private readonly MyCampusCardContext context;

        public FormRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        /*public async Task<ICollection<FormEntity>> getForms()
        {
            var forms = context.FormEntity
                    .Include(form => form.FormEntitysub)
                    .ToListAsync();
            return await forms;
        }

        public async Task<ICollection<FormEntity>> getForms(string username)
        {
            var forms = context.FormEntity
                    .Include(form => form.FormEntitysub)
                    .ToListAsync();
            return await forms;
        }*/

        public IQueryable<formEntity> GetAllForms(){
            return _context.FormEntity.Where(x => x.IsTitle == false || x.Parent_ID != null);
        }

        public List<formEntity> getForms()
        {
            return  _context.FormEntity.ToList();
        }

        public bool CheckForm(int user)
        {
            var account = _context.UserEntity.Where(x => x.User_ID == user).FirstOrDefault();
            var userCount = _context.UserAccessEntity.Where(x => x.User_ID == user).Count();
            var roleCount = _context.RolePermissionEntity.Where(x => x.Role_ID == account.Role_ID).Count();

            var userCountEdit = _context.UserAccessEntity.Where(x => x.User_ID == user && x.Can_Insert == true).Count();
            var roleCountEdit = _context.RolePermissionEntity.Where(x => x.Role_ID == account.Role_ID && x.Can_Insert == true).Count();

            if (userCount == roleCount)
            {
                if(userCountEdit == roleCountEdit)
                    return true;
            }

            return false;
        }

        public List<formEntity> GetUserForm(int user)
        {
            return _context.FormEntity.FromSql<formEntity>("call spGet_User_Access(@roleid, @userid)", new MySqlParameter("@roleid", UserGetRoles(user)), new MySqlParameter("@userid", user)).ToList();
        }

        public string UserGetRoles(int userid)
        {
            var roles = context.UserEntity.Include(x => x.RoleEntity).Where(user => user.User_ID == userid).Select(role => role.RoleEntity.Role_ID).ToList();

            var rolesString = string.Join(',', roles);

            return rolesString;
        }
    }
}
