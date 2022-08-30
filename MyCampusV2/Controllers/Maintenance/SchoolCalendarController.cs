using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Helpers.ActionFilters;
using MyCampusV2.Helpers.Constants;
using MyCampusV2.Helpers.ExcelHelper;
using MyCampusV2.Helpers.PageResult;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.IServices;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MyCampusV2.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolCalendarController : BaseController
    {
        private readonly ISchoolCalendarService _schoolCalendarService;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;
        private readonly IBatchUploadService _batchUploadService;
        private readonly BatchUploadSettings _batchSettings;
        private readonly ExcelHelper _excelHelper;
        private ResultModel result = new ResultModel();

        public SchoolCalendarController(ISchoolCalendarService schoolCalendarService, IMapper mapper, IAuditTrailService audit, IBatchUploadService batchUploadService, IOptions<BatchUploadSettings> batchSettings, IOptions<ExcelHelper> excelHelper)
        {
            this._schoolCalendarService = schoolCalendarService;
            _mapper = mapper;
            _audit = audit;
            _batchUploadService = batchUploadService;
            _batchSettings = batchSettings.Value;
            _excelHelper = excelHelper.Value;
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("schoolcalendar")]
        public async Task<schoolCalendarResult> GetCalendarDates(string schoolyear)
        {
            return await _schoolCalendarService.GetCalendarDates(schoolyear);
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("download")]
        public async Task<IActionResult> DownloadTemplate([FromQuery] string schoolYear, string month)
        {
            try
            {
                byte[] file = null;
                file = await Task.Run(() => _excelHelper.ExportCalendarTemplate(ExcelVar.SchoolCalendarTemplateTitle, schoolYear, month));
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("export")]
        public async Task<IActionResult> ExportSchoolCalendar([FromQuery] string schoolYear)
        {
            try
            {
                var result = await _schoolCalendarService.GetCalendarList(schoolYear);
                byte[] file = null;
                if (result.Count != 0)
                {
                    file = await Task.Run(() => _excelHelper.ExportCalendarList(ExcelVar.SchoolCalendarTitle, schoolYear, result));
                }
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("batchupload/{id:int}")]
        public async Task<IActionResult> BatchUploadProcess(int id)
        {
            string message = "Batch file has been successfully process.";
            var entity = await _batchUploadService.GetByIdSync(id);
            var response = new BatchUploadResponse();
            try
            {
                await ValidateBatchProcess(entity);
                int lastProcess = entity.ProcessCount;

                IEnumerable<schoolCalendarBatchUploadVM> records = _batchUploadService.GetCalendarRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                response = await _schoolCalendarService.BatchUpload(process, GetUserId(), entity.ID, lastProcess);

                entity.ProcessCount = entity.ProcessCount + process.Count();
                entity.Status = entity.ProcessCount == entity.TotalCount ? Constants.BATCH_UPLOAD_STATUS_SUCCESS : entity.Status;
                if (entity.Status != Constants.BATCH_UPLOAD_STATUS_SUCCESS)
                    message = string.Format("Processing {0} to {1}", lastProcess, entity.ProcessCount);

                await _batchUploadService.UpdateBatchUplaod(entity, GetUserId());

                response.ProcessCount = entity.ProcessCount;
                response.TotalCount = entity.TotalCount;
                response.Status = entity.Status;
                return Ok(new { message = message, response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpPost("batchupload")]
        public async Task<IActionResult> BatchUpload(IFormFile file)
        {
            string message = "School Calendar has been successfully uploaded.";
            var response = new BatchUploadResponse();
            var entity = new batchUploadEntity();
            try
            {
                entity.Form_ID = (int)Form.Campus_School_Calendar;
                entity.Date_Time_Added = DateTime.Now;
                await ValidateBatchFile(file, entity);
                await _batchUploadService.Upload(file, entity, GetUserId());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message, response, key = entity.ID });
        }

        public async Task ValidateBatchProcess(batchUploadEntity upload)
        {
            if (upload.Status == Constants.BATCH_UPLOAD_STATUS_CANCELLED)
                throw new Exception("Upload has been cancelled.");
        }

        public async Task ValidateBatchFile(IFormFile file, batchUploadEntity upload)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
            var fileType = Path.GetExtension(originalFileName);

            if (fileType != ".xlsx\"")
                throw new Exception("Invalid file! Please download the valid template.");

            try
            {
                upload.TotalCount = _batchUploadService.GetRecordsCount(file);
            }
            catch (Exception)
            {
                throw new Exception("File contains invalid data or missing fields.");
            }
            if (upload.TotalCount == 0)
                throw new Exception("File is empty!");
        }

        [Authorize]
        [HttpGet]
        [Route("all")]
        public async Task<schoolYearPagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            return _mapper.Map<schoolYearPagedResultVM>(await _schoolCalendarService.GetAll(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        [HttpGet]
        [Route("filtered")]
        public async Task<schoolYearPagedResultVM> GetFiltered()
        {
            return _mapper.Map<schoolYearPagedResultVM>(await _schoolCalendarService.GetFiltered());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] schoolYearVM schoolYear)
        {
            try
            {
                schoolYear.addedBy = GetUserId();
                schoolYear.updatedBy = GetUserId();
                return await _schoolCalendarService.AddSchoolYear(_mapper.Map<schoolYearEntity>(schoolYear));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpPut]
        [Route("update")]
        public async Task<ResultModel> Update([FromBody] schoolYearVM schoolYear)
        {
            try
            {
                schoolYear.updatedBy = GetUserId();
                return await _schoolCalendarService.UpdateSchoolYear(_mapper.Map<schoolYearEntity>(schoolYear));
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
        public async Task<ResultModel> Delete(int id)
        {
            try
            {
                return await _schoolCalendarService.DeleteSchoolYear(id, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("schoolYear/{id}")]
        public async Task<schoolYearVM> GetById(int id)
        {
            return _mapper.Map<schoolYearVM>(await _schoolCalendarService.GetById(id));
        }


    }
}
