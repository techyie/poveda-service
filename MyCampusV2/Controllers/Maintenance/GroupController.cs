using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Helpers.PageResult;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using MyCampusV2.Helpers.ActionFilters;

namespace MyCampusV2.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : BaseController
    {
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;

        private ResultModel result = new ResultModel();

        public GroupController(IGroupService groupService, IMapper mapper, IAuditTrailService audit)
        {
            this._groupService = groupService;
            _mapper = mapper;
            _audit = audit;
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("all")]
        public async Task<fetcherGroupPagedResultVM> GetAllGroups([FromQuery] PaginationParams param)
        {
            return _mapper.Map<fetcherGroupPagedResultVM>(await _groupService.GetAllGroups(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("allgroup")]
        public async Task<ICollection<fetcherGroupVM>> GetGroups()
        {
            return _mapper.Map<ICollection<fetcherGroupVM>>(await _groupService.GetGroups());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getGroupById/{id}")]
        public async Task<fetcherGroupVM> GetGroupByID(int id)
        {
            return _mapper.Map<fetcherGroupVM>(await _groupService.GetGroupByID(id));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] fetcherGroupVM entity)
        {
            try
            {
                entity.addedBy = GetUserId();
                entity.updatedBy = GetUserId();
                entity.userId = GetUserId();
                return await _groupService.AddGroup(_mapper.Map<fetcherGroupEntity>(entity));
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
        public async Task<ResultModel> Update([FromBody] fetcherGroupVM entity)
        {
            try
            {
                entity.addedBy = GetUserId();
                entity.updatedBy = GetUserId();
                return await _groupService.UpdateGroup(_mapper.Map<fetcherGroupEntity>(entity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("permanentDeleteGroup/{id}")]
        public async Task<ResultModel> PermanentDelete(int id)
        {
            try
            {
                return await _groupService.DeleteGroupPermanent(id, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("temporaryDeleteGroup/{id}")]
        public async Task<ResultModel> TemporaryDelete(int id)
        {
            try
            {
                return await _groupService.DeleteGroupTemporary(id, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }
    }
}
