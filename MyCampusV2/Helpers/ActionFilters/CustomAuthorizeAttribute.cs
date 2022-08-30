using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using MyCampusV2.IServices;
using MyCampusV2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyCampusV2.Helpers.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public CustomAuthorizeAttribute()
        {
         
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var roleService = (IRoleService)context.HttpContext.RequestServices.GetService(typeof(IRoleService));
            var user = context.HttpContext.User;

            int role = 0;

            if (user.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                role = Convert.ToInt32(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value);
            }

            string method = context.HttpContext.Request.Method.ToUpper();
            string form = controllerActionDescriptor?.ControllerName;

            if (controllerActionDescriptor?.ControllerName == "YearSection")
            {
                form = "YearLevel";
            }
            else if (controllerActionDescriptor?.ControllerName == "StudentSection")
            {
                form = "Section";
            }
            else if (controllerActionDescriptor?.ControllerName == "GroupDetails")
            {
                form = "Group";
            }
            else if (controllerActionDescriptor?.ControllerName == "Report")
            {
                form = "Reports";
            }
            else if (controllerActionDescriptor?.ControllerName == "Person")
            {
                if (controllerActionDescriptor.ActionName.Contains("Student"))
                {
                    form = "Student";
                }
                else if (controllerActionDescriptor.ActionName.Contains("Employee"))
                {
                    form = "Employee";
                }
                else if (controllerActionDescriptor.ActionName.Contains("Visitor"))
                {
                    form = "Visitor";
                }
                else if (controllerActionDescriptor.ActionName.Contains("Fetcher"))
                {
                    form = "Fetcher";
                }
                else if (controllerActionDescriptor.ActionName.Contains("OtherAccess"))
                {
                    form = "OtherAccess";
                }
            }
            else if (controllerActionDescriptor?.ControllerName == "Notification")
            {
                if (controllerActionDescriptor.ActionName.Contains("General"))
                {
                    form = "General";
                }
                else if (controllerActionDescriptor.ActionName.Contains("Personal"))
                {
                    form = "Personal";
                }
            }
            else if (controllerActionDescriptor?.ControllerName == "Users" && method == "PUT")
            {
                form = "User List";
            }
            else if (controllerActionDescriptor?.ControllerName == "PAPAccount")
            {
                form = "Mobile App Account";
            }
            else
            {
                form = controllerActionDescriptor?.ControllerName;
            }

            bool isAuthorized = await roleService.IsAuthorized(role, form.Replace(" ", ""), method);
            if (!isAuthorized)
            {
                //context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
                context.Result = new JsonResult("No Permission to Access") { StatusCode = (int)System.Net.HttpStatusCode.Forbidden };
                return;
            }

            return;
        }
    }
}
