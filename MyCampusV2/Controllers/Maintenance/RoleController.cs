using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Helpers.ActionFilters;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Helpers.PageResult;
using Microsoft.AspNetCore.Authorization;

namespace MyCampusV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RoleController(IRoleService role, IMapper mapper)
        {
            _roleService = role;
            _mapper = mapper;
        }

        [CustomAuthorize]
        [HttpGet]
        [Route("all")]
        public async Task<rolePagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            return _mapper.Map<rolePagedResultVM>(await _roleService.GetAllRoles(param.PageNo, param.PageSize, param.Keyword));
        }

        [CustomAuthorize]
        [HttpGet]
        [Route("{id:int}")]
        public async Task<roleVM> GetByID(int id)
        {
            return _mapper.Map<roleVM>(await _roleService.GetById(id));
        }

        [CustomAuthorize]
        [HttpGet]
        [Route("modules")]
        public async Task<ICollection<formEntity>> GetModules()
        {
            var result = _mapper.Map<ICollection<formEntity>>(await _roleService.GetModules());
            return result;
        }

        [CustomAuthorize]
        [HttpGet]
        [Route("rolemodules/{id:int}")]
        public async Task<ICollection<roleModuleVM>> GetRoleModules(int id)
        {
            try
            {
                return _mapper.Map<ICollection<roleModuleVM>>(await _roleService.GetRoleModules(id));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [CustomAuthorize]
        [HttpPost]
        [Route("rolemodules/{id:int}")]
        public async Task<IActionResult> UpdateRoleModules(int id, [FromBody] string[] modules)
        {
            try
            {
                await _roleService.UpdateRoleModules(id, modules, GetUserId());
                return Ok(new { message = "Role Modules has been successfully updated." });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [CustomAuthorize]
        [HttpPut]
        [Route("rolemodules/{id:int}")]
        public async Task<IActionResult> UpdateRoleAccess(int id, [FromBody] string[] access)
        {
            await _roleService.UpdateRoleAccess(id, access, GetUserId());
            return Ok(new { message = "Role Modules has been successfully updated." });
        }

        [HttpGet]
        [Route("getallrolesisactive")]
        public async Task<ICollection<roleVM>> GetAllRolesIsActive()
        {
            var result = _mapper.Map<ICollection<roleVM>>(await _roleService.GetAllIsActive());
            return result;
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] roleVM role)
        {
            try
            {
                role.addedBy = GetUserId();
                role.updatedBy = GetUserId();
                return await _roleService.AddRole(_mapper.Map<roleEntity>(role));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("update")]
        public async Task<ResultModel> Update([FromBody] roleVM role)
        {
            try
            {
                role.updatedBy = GetUserId();
                return await _roleService.UpdateRole(_mapper.Map<roleEntity>(role));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ResultModel> TemporaryDelete(int id)
        {
            try
            {
                return await _roleService.DeleteRole(id, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("retrieve/{id}")]
        public async Task<ResultModel> RetrieveRole(int id)
        {
            try
            {
                roleEntity entity = new roleEntity();
                entity.Role_ID = id;
                entity.Updated_By = GetUserId();
                return await _roleService.RetrieveRole(_mapper.Map<roleEntity>(entity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        /*
        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        public async Task<ICollection<roleVM>> GetAll()
        {
            return _mapper.Map<ICollection<roleVM>>(await _role.GetAll());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("roles")]
        public async Task<rolePagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            return _mapper.Map<rolePagedResultVM>(await _role.GetAllRoles(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("{id:int}")]
        public async Task<roleVM> GetByID(int id)
        {
            return _mapper.Map<roleVM>(await _role.GetById(id));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]roleVM role)
        {
            string message = "Role" + SuccessMessageAdd;
            try
            {
                if (role.Role_Name.Trim() == null || role.Role_Name.Trim() == string.Empty)
                {
                    return BadRequest(new { message = "Role Name is required" });
                }
                if (role.Role_Desc.Trim() == null || role.Role_Desc.Trim() == string.Empty)
                {
                    return BadRequest(new { message = "Role Description is required" });
                }

                var result = await _role.DuplicateRecordChecker(role.Role_Name, role.Role_Desc);
                if (result.Count > 0)
                {
                    return BadRequest(new { message = "Role Name/Description already exists" });
                }

                roleEntity roles = _mapper.Map<roleEntity>(role);
                roles.Added_By = GetUserId();

                await _role.AddRole(roles, GetUserId());

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message });
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]roleVM role)
        {
            string message = "Role" + SuccessMessageUpdate;
            try
            {
                if (role.Role_Name.Trim() == null || role.Role_Name.Trim() == string.Empty)
                {
                    return BadRequest(new { message = "Role Name is required" });
                }
                if (role.Role_Desc.Trim() == null || role.Role_Desc.Trim() == string.Empty)
                {
                    return BadRequest(new { message = "Role Description is required" });
                }

                roleEntity roles = _mapper.Map<roleEntity>(role);
                roles.Updated_By = GetUserId();

                await _role.UpdateRole(roles, (int)role.Role_ID.Value);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message });
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _role.GetCountRoleIfActive(id);
            if (result.Count > 0)
                return BadRequest(new { message = "Role is in use by an active user." });

            await _role.DeleteRole(id, GetUserId());
            return Ok(new { message = "Role" + SuccessMessageDelete});
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("modules")]
        public async Task<ICollection<formEntity>> GetModules()
        {
            var result = _mapper.Map<ICollection<formEntity>>(await _role.GetModules());
            return result;
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("rolemodules/{id:int}")]
        public async Task<ICollection<roleModuleVM>> GetRoleModules(int id)
        {
            var result = _mapper.Map<ICollection<roleModuleVM>>(await _role.GetRoleModules(id));
            return result;
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost("rolemodules/{id:int}")]
        public async Task<IActionResult> UpdateRoleModules(int id,[FromBody] string[] modules)
        {
            await _role.UpdateRoleModules(id, modules, GetUserId());
            return Ok(new { message = "Role Modules has been successfully updated." });
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("getallroles")]
        public async Task<ICollection<rolesVM>> GetAllRoles()
        {
            var result = _mapper.Map<ICollection<rolesVM>>(await _role.GetAll());
            return result;
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("getallrolesisactivenoguard")]
        public async Task<ICollection<rolesVM>> GetAllRolesIsActiveNoGuard()
        {
            var result = _mapper.Map<ICollection<rolesVM>>(await _role.GetAllIsActiveNoGuard());
            return result;
        }
        */
    }
}