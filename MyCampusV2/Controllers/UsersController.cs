using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Common.ViewModels.UserViewModel;
using MyCampusV2.Helpers.PageResult;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Helpers.ActionFilters;

namespace MyCampusV2.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper, IRoleService role)
        {
            _userService = userService;
            _roleService = role;
            _mapper = mapper;
        }

        //[Authorize]
        [HttpGet("auth/user")]
        public IActionResult Get()
        {
            try
            {
                var user = Task.Run(() => _userService.GetById(GetUserId())).Result;

                var Data = new UserModel { First_Name = user.PersonEntity.First_Name, Last_Name = user.PersonEntity.Last_Name, SA = user.Role_ID == 1 ? true : false };

                return Ok(Data);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //[Authorize]
        [HttpGet("allowed/{form}")]
        public async Task<roleAuthorised> GetAuthorization(int form)
        {
            try
            {
                return _mapper.Map<roleAuthorised>(await _roleService.AuthorizedRole(GetRoleId(), form));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] userRoleVM userdetails)
        {
            //Task UpdateRoleUser(tbl_user_role userrole, int id)
            string message = "User Role " + SuccessMessageUpdate;
            try
            {
                //userRoleEntity userrole = _mapper.Map<userRoleEntity>(userdetails);

                await _userService.UpdateRoleUser(userdetails.user_ID, userdetails.role_ID);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message });
        }

        [HttpGet]
        [Route("userlist")]
        public async Task<userPagedResultVM> GetAllUsers([FromQuery] PaginationParams param)
        {
            return _mapper.Map<userPagedResultVM>(await _userService.GetAllUsers(param.PageNo, param.PageSize, param.Keyword));
        }

        [HttpGet]
        public async Task<ICollection<userRoleVM>> GetAll()
        {
            return _mapper.Map<ICollection<userRoleVM>>(await _userService.GetUserList());
        }


        [HttpGet("{id:int}")]
        public async Task<userRoleVM> GetById(int id)
        {
            //var x = _mapper.Map<userVM>(await _userService.GetById(id));
            return _mapper.Map<userRoleVM>(await _userService.GetById(id));
        }
    }
}