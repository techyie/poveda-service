using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Helpers.ActionFilters;
using MyCampusV2.Helpers.PageResult;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeSubTypeController : BaseController
    {
        private readonly IEmployeeSubTypeService _employeeSubTypeService;
        private readonly IMapper _mapper;
        private ResultModel result = new ResultModel();

        public EmployeeSubTypeController(IEmployeeSubTypeService employeeSubTypeService, IMapper mapper)
        {
            this._employeeSubTypeService = employeeSubTypeService;
            this._mapper = mapper;
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getEmployeeSubTypeByEmployeeTypeId/{id}")]
        public async Task<IList<employeeSubTypeVM>> getEmployeeSubTypeByEmployeeTypeId(int id)
        {
            return _mapper.Map<IList<employeeSubTypeVM>>(await _employeeSubTypeService.GetEmployeeSubTypesUsingEmployeeTypeId(id));
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getEmployeeSubTypeById/{id}")]
        public async Task<employeeSubTypeVM> GetEmployeeTypeById(int id)
        {
            return _mapper.Map<employeeSubTypeVM>(await _employeeSubTypeService.GetEmployeeSubTypeById(id));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] employeeSubTypeVM employeeSubType)
        {
            try
            {
                employeeSubType.addedBy = GetUserId();
                employeeSubType.updatedBy = GetUserId();
                return await _employeeSubTypeService.AddEmployeeSubType(_mapper.Map<employeeSubTypeEntity>(employeeSubType));
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
        public async Task<ResultModel> Update([FromBody] employeeSubTypeVM employeeSubType)
        {
            try
            {
                employeeSubType.updatedBy = GetUserId();
                return await _employeeSubTypeService.UpdateEmployeeSubType(_mapper.Map<employeeSubTypeEntity>(employeeSubType));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("permanentDelete/{id}")]
        public async Task<ResultModel> PermanentDelete(int? id)
        {
            try
            {
                employeeSubTypeVM entity = new employeeSubTypeVM();
                entity.employeeSubTypeId = id;
                entity.updatedBy = GetUserId();
                return await _employeeSubTypeService.DeleteEmployeeSubTypePermanent(_mapper.Map<employeeSubTypeEntity>(entity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("temporaryDelete/{id}")]
        public async Task<ResultModel> TemporaryDelete(int? id)
        {
            try
            {
                employeeSubTypeVM entity = new employeeSubTypeVM();
                entity.employeeSubTypeId = id;
                entity.updatedBy = GetUserId();
                return await _employeeSubTypeService.DeleteEmployeeSubTypeTemporary(_mapper.Map<employeeSubTypeEntity>(entity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("retrieveData/{id}")]
        public async Task<ResultModel> RetrieveData(int? id)
        {
            try
            {
                employeeSubTypeVM entity = new employeeSubTypeVM();
                entity.employeeSubTypeId = id;
                entity.updatedBy = GetUserId();
                return await _employeeSubTypeService.RetrieveEmployeeSubType(_mapper.Map<employeeSubTypeEntity>(entity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("all")]
        public async Task<employeeSubTypePagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            return _mapper.Map<employeeSubTypePagedResultVM>(await _employeeSubTypeService.GetAllEmployeeSubType(param.PageNo, param.PageSize, param.Keyword));
        }

        [HttpGet]
        [Route("getEmployeeSubTypes")]
        public async Task<IList<employeeSubTypeVM>> GetEmployeeSubTypes()
        {
            return _mapper.Map<IList<employeeSubTypeVM>>(await _employeeSubTypeService.GetEmployeeSubTypes());
        }
    }
}