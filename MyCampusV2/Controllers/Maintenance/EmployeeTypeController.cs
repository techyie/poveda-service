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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeTypeController : BaseController
    {
        private readonly IEmployeeTypeService _employeeTypeService;
        private readonly IMapper _mapper;
        private ResultModel result = new ResultModel();

        public EmployeeTypeController(IEmployeeTypeService employeeTypeService, IMapper mapper)
        {
            this._employeeTypeService = employeeTypeService;
            this._mapper = mapper;
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getEmployeeTypesUsingCampusId/{id}")]
        public async Task<IList<employeeTypeVM>> GetEmployeeTypesUsingCampusId(int id)
        {
            return _mapper.Map<IList<employeeTypeVM>>(await _employeeTypeService.GetEmployeeTypesUsingCampusId(id));
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getEmployeeTypeById/{id}")]
        public async Task<employeeTypeVM> GetEmployeeTypeById(int id)
        {
            try
            {
                return _mapper.Map<employeeTypeVM>(await _employeeTypeService.GetEmployeeTypeById(id));
            } catch (Exception err)
            {
                return null;
            } 
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] employeeTypeVM employeeType)
        {
            try
            {
                employeeType.addedBy = GetUserId();
                employeeType.updatedBy = GetUserId();
                return await _employeeTypeService.AddEmployeeType(_mapper.Map<empTypeEntity>(employeeType));
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
        public async Task<ResultModel> Update([FromBody] employeeTypeVM employeeType)
        {
            try
            {
                employeeType.updatedBy = GetUserId();
                return await _employeeTypeService.UpdateEmployeeType(_mapper.Map<empTypeEntity>(employeeType));
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
                employeeTypeVM entity = new employeeTypeVM();
                entity.employeeTypeId = id;
                entity.updatedBy = GetUserId();
                return await _employeeTypeService.DeleteEmployeeTypePermanent(_mapper.Map<empTypeEntity>(entity));
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
                employeeTypeVM entity = new employeeTypeVM();
                entity.employeeTypeId = id;
                entity.updatedBy = GetUserId();
                return await _employeeTypeService.DeleteEmployeeTypeTemporary(_mapper.Map<empTypeEntity>(entity));
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
                employeeTypeVM entity = new employeeTypeVM();
                entity.employeeTypeId = id;
                entity.updatedBy = GetUserId();
                return await _employeeTypeService.RetrieveEmployeeType(_mapper.Map<empTypeEntity>(entity));
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
        public async Task<employeeTypePagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            //return await _employeeTypeService.GetAllEmployeeType(param.PageNo, param.PageSize, param.Keyword);
            return _mapper.Map<employeeTypePagedResultVM>(await _employeeTypeService.GetAllEmployeeType(param.PageNo, param.PageSize, param.Keyword));
        }

        [HttpGet]
        [Route("getEmployeeTypes")]
        public async Task<IList<employeeTypeVM>> GetEmployeeTypes()
        {
            return _mapper.Map<IList<employeeTypeVM>>(await _employeeTypeService.GetEmployeeTypes());
        }
    }
}