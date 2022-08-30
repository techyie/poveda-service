using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace MyCampusV2.Common
{
    public class CurrentWebUserService : ICurrentUser
    {
        private readonly IHttpContextAccessor _context;

        public CurrentWebUserService(IHttpContextAccessor httpContextAccessor)
        {
            _context = httpContextAccessor;
        }

        public int Campus {
            get {
                int campus = 0;
                if (_context.HttpContext.User.HasClaim(c => c.Type == ClaimTypes.PrimarySid)) {
                    campus = Convert.ToInt32(_context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid).Value);
                }
                return campus;
            }
        }

        public bool MasterAccess
        {
            get
            {
                int role = 0;

                if (_context.HttpContext.User.HasClaim(c => c.Type == ClaimTypes.Role)) {
                    role = Convert.ToInt32(_context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value);
                }

                if(role == 1) {
                    return true;
                }

                return false;
            }
        }
    }
}
