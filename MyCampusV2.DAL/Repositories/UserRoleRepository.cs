using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;

namespace MyCampusV2.DAL.Repositories
{
    public class UserRoleRepository : BaseRepository<userRoleEntity>, IUserRoleRepository
    {
        private readonly MyCampusCardContext context;

        public UserRoleRepository(MyCampusCardContext Context) : base(Context)
        {
            this.context = Context;
        }
    }
}
