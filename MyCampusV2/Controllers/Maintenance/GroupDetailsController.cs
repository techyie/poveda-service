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
    public class GroupDetailsController : BaseController
    {
        private readonly IGroupDetailsService _groupDetailsService;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;

        private ResultModel result = new ResultModel();

        public GroupDetailsController(IGroupDetailsService groupDetailsService, IMapper mapper, IAuditTrailService audit)
        {
            this._groupDetailsService = groupDetailsService;
            _mapper = mapper;
            _audit = audit;
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("all/{groupId}")]
        public async Task<fetcherGroupDetailsPagedResultVM> AddStudentToGroup([FromQuery] PaginationParams param, int groupId)
        {
            return _mapper.Map<fetcherGroupDetailsPagedResultVM>(await _groupDetailsService.GetAllStudentAssignedToGroup(param.PageNo, param.PageSize, param.Keyword, groupId));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] fetcherGroupDetailsVM entity)
        {
            try
            {
                entity.addedBy = GetUserId();
                entity.updatedBy = GetUserId();
                return await _groupDetailsService.AddStudentToGroup(_mapper.Map<fetcherGroupDetailsEntity>(entity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        /*
        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("deleteStudent")]
        public async Task<ResultModel> PermanentDelete([FromBody] fetcherGroupDetailsVM entity)
        {
            try
            {
                entity.addedBy = GetUserId();
                entity.updatedBy = GetUserId();
                return await _groupDetailsService.DeleteGroup(_mapper.Map<fetcherGroupDetailsEntity>(entity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }
        */

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("deleteStudent/{id}/{personid}")]
        public async Task<ResultModel> PermanentDelete(int id, int personId)
        {
            try
            {
                return await _groupDetailsService.DeleteGroup(id, personId, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        } 
    }
}
