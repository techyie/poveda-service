using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Helpers.ActionFilters;
using MyCampusV2.Helpers.PageResult;
using MyCampusV2.Models;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Models.V2.entity;

namespace MyCampusV2.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleService _scheduleService;
        private readonly IMapper _mapper;
        private ResultModel result = new ResultModel();

        public ScheduleController(IScheduleService scheduleService, IMapper mapper)
        {
            this._scheduleService = scheduleService;
            this._mapper = mapper;
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] scheduleVM scheduleData)
        {
            try
            {
                scheduleData.scheduleStatus = "Active";
                scheduleData.addedBy = GetUserId();
                scheduleData.updatedBy = GetUserId();
                return await _scheduleService.AddSchedule(_mapper.Map<scheduleEntity>(scheduleData));
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
        public async Task<ResultModel> Update([FromBody] scheduleVM scheduleData)
        {
            try
            {
                scheduleData.scheduleStatus = "Active";
                scheduleData.addedBy = GetUserId();
                scheduleData.updatedBy = GetUserId();
                return await _scheduleService.UpdateSchedule(_mapper.Map<scheduleEntity>(scheduleData));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("permanentDeleteSchedule/{id}")]
        public async Task<ResultModel> PermanentDelete(int id)
        {
            try
            {
                return await _scheduleService.DeleteSchedulePermanent(id, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("temporaryDeleteSchedule/{id}")]
        public async Task<ResultModel> TemporaryDelete(int id)
        {
            try
            {
                return await _scheduleService.DeleteScheduleTemporary(id, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("retrieveData")]
        public async Task<ResultModel> RetrieveSchedule([FromBody] scheduleVM scheduleData)
        {
            try
            {
                scheduleData.updatedBy = GetUserId();
                return await _scheduleService.RetrieveSchedule(_mapper.Map<scheduleEntity>(scheduleData));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("all")]
        public async Task<schedulePagedResultVM> GetAllSchedule([FromQuery] PaginationParams param)
        {
            return _mapper.Map<schedulePagedResultVM>(await _scheduleService.GetAllSchedule(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("allschedule")]
        public async Task<ICollection<scheduleVM>> GetSchedules()
        {
            return _mapper.Map<ICollection<scheduleVM>>(await _scheduleService.GetSchedules());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getScheduleById/{id}")]
        public async Task<scheduleVM> GetScheduleByID(int id)
        {
            return _mapper.Map<scheduleVM>(await _scheduleService.GetScheduleByID(id));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getScheduleByFetcherID/{id}")]
        public async Task<fetcherSchedulePagedResultVM> GetScheduleByFetcherID(string id, [FromQuery] PaginationParams param)
        {
            try
            {
                return _mapper.Map<fetcherSchedulePagedResultVM>(await _scheduleService.GetScheduleByFetcherID(id, param.PageNo, param.PageSize, param.Keyword));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("createFetcherSchedule")]
        public async Task<ResultModel> CreateFetcherSchedule([FromBody] fetcherScheduleVM entity)
        {
            try
            {
                entity.userId = GetUserId();
                entity.addedBy = GetUserId();
                entity.updatedBy = GetUserId();
                return await _scheduleService.AddFetcherSchedule(_mapper.Map<fetcherScheduleEntity>(entity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("deleteFetcherSchedule/{id}")]
        public async Task<ResultModel> DeleteFetcherSchedule(int id)
        {
            return await _scheduleService.DeleteFetcherSchedule(id, GetUserId());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getStudentByFetcherScheduleID/{id}")]
        public async Task<fetcherScheduleDetailsPagedResultVM> GetStudentByFetcherScheduleID(string id, [FromQuery] PaginationParams param)
        {
            try
            {
               return _mapper.Map<fetcherScheduleDetailsPagedResultVM>(await _scheduleService.GetStudentByFetcherScheduleID(id, param.PageNo, param.PageSize, param.Keyword));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getGroupByFetcherScheduleID/{id}")]
        public async Task<fetcherScheduleDetailsPagedResultVM> GetGroupByFetcherScheduleID(string id, [FromQuery] PaginationParams param)
        {
            try
            {
                //return _mapper.Map<fetcherScheduleDetailsPagedResultVM>(await _scheduleService.GetGroupByFetcherScheduleID(id, param.PageNo, param.PageSize, param.Keyword));
                return await _scheduleService.GetGroupByFetcherScheduleID(id, param.PageNo, param.PageSize, param.Keyword);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("createFetcherScheduleStudent")]
        public async Task<ResultModel> CreateFetcherScheduleStudent([FromBody] fetcherScheduleDetailsVM entity)
        {
            try
            {
                entity.userId = GetUserId();
                entity.addedBy = GetUserId();
                entity.updatedBy = GetUserId();
                return await _scheduleService.AddFetcherScheduleStudent(_mapper.Map<fetcherScheduleDetailsEntity>(entity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("deleteFetcherScheduleStudent/{id}")]
        public async Task<ResultModel> DeleteFetcherScheduleStudent(int id)
        {
            return await _scheduleService.DeleteFetcherScheduleStudent(id, GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("deleteFetcherScheduleGroup/{id}")]
        public async Task<ResultModel> DeleteFetcherScheduleGroup(int id)
        {
            return await _scheduleService.DeleteFetcherScheduleGroup(id, GetUserId());
        }
    }
}
