using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Services;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using MyCampusV2.Common;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices;
using MyCampusV2.Common.ViewModels.UserViewModel;

namespace MyCampusV2.Services.Services
{
    public class UserRoleService : BaseService, IUserRoleService
    {
        public UserRoleService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user, IOptions<AppSettings> app) : base(unitOfWork, audit, user)
        {
        }
    }
}
