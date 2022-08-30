using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
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

namespace MyCampusV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notification;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;
        private readonly IBatchUploadService _batchUploadService;
        private readonly BatchUploadSettings _batchSettings;
        private readonly ExcelHelper _excelHelper;

        public NotificationController(INotificationService notification, IMapper mapper, IAuditTrailService audit, IBatchUploadService batchUploadService, IOptions<BatchUploadSettings> batchSettings, IOptions<ExcelHelper> excelHelper)
        {
            _notification = notification;
            _mapper = mapper;
            _audit = audit;
            _batchUploadService = batchUploadService;
            _batchSettings = batchSettings.Value;
            _excelHelper = excelHelper.Value;
        }

        [HttpGet]
        [Route("getByGuid/{guid}")]
        public async Task<notificationVM> GetByGuid(string guid)
        {
            return _mapper.Map<notificationVM>(await _notification.GetByGuid(guid));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("getByID/{id}")]
        public async Task<notificationVM> GetByID(int id)
        {
            return _mapper.Map<notificationVM>(await _notification.GetById(id));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("addGeneralNotification")]
        public async Task<ResultModel> AddGeneralNotification([FromBody] notificationVM notification)
        {
            try
            {
                notification.addedBy = GetUserId();
                notification.updatedBy = GetUserId();
                return await _notification.AddGeneralNotification(_mapper.Map<notificationEntity>(notification));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("updateGeneralNotification")]
        public async Task<ResultModel> UpdateGeneralNotification([FromBody] notificationVM notification)
        {
            try
            {
                notification.updatedBy = GetUserId();
                return await _notification.UpdateGeneralNotification(_mapper.Map<notificationEntity>(notification));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }
            
        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("deleteGeneralNotification/{id}")]
        public async Task<ResultModel> DeleteGeneralNotification(string id)
        {
            try
            {
                notificationVM notification = new notificationVM();
                return await _notification.DeleteGeneralNotification(Convert.ToInt32(id), GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("addPersonalNotification")]
        public async Task<ResultModel> AddPersonalNotification([FromBody] notificationVM notification)
        {
            try
            {
                notification.addedBy = GetUserId();
                notification.updatedBy = GetUserId();
                return await _notification.AddPersonalNotification(_mapper.Map<notificationEntity>(notification));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("updatePersonalNotification")]
        public async Task<ResultModel> UpdatePersonalNotification([FromBody] notificationVM notification)
        {
            try
            {
                notification.updatedBy = GetUserId();
                return await _notification.UpdatePersonalNotification(_mapper.Map<notificationEntity>(notification));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("deletePersonalNotification/{guid}")]
        public async Task<ResultModel> DeletePersonalNotification(string guid)
        {
            try
            {
                notificationVM notification = new notificationVM();
                notification.updatedBy = GetUserId();
                notification.guid = guid;
                return await _notification.DeletePersonalNotification(_mapper.Map<notificationEntity>(notification));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("generalNotificationsPagination")]
        public async Task<notificationPagedResultVM> GeneralNotificationsPagination([FromQuery] PaginationParams param)
        {
            return await _notification.GetGeneralNotificationsPagination(param.PageNo, param.PageSize, param.Keyword);
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("personalNotificationsPagination")]
        public async Task<notificationPagedResultVM> PersonalNotificationsPagination([FromQuery] PaginationParams param)
        {
            return await _notification.GetPersonalNotificationsPagination(param.PageNo, param.PageSize, param.Keyword);
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("personnotification")]
        public async Task<ICollection<personNotificationVM>> GetPersonNotification()
        {
            try
            {
                return _mapper.Map<ICollection<personNotificationVM>>(await _notification.GetAllActivePerson());
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("downloadtemplatepersonal")]
        public async Task<IActionResult> DownloadTemplatePersonal()
        {
            try
            {
                byte[] file = null;
                file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.PersonalColHeader, ExcelVar.PersonalSampleData, ExcelVar.PersonalTitle));
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("batchupload/personalnotifications/{id:int}")]
        public async Task<IActionResult> BatchPersonalUploadProcess(int id)
        {
            string message = "Batch file has been successfully process.";
            var entity = await _batchUploadService.GetByIdSync(id);
            var response = new BatchUploadResponse();
            try
            {
                await ValidateBatchPersonalProcess(entity);
                int lastProcess = entity.ProcessCount;

                IEnumerable<personalNotificationBatchUploadVM> records = _batchUploadService.GetPersonalNotifRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                // Do the update process here.
                response = await _notification.BatchPersonalNotificationUpload(process, GetUserId(), entity.ID, lastProcess);

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

        public async Task ValidateBatchPersonalProcess(batchUploadEntity upload)
        {
            // Validate if valid file.
            if (upload.Status == Constants.BATCH_UPLOAD_STATUS_CANCELLED)
                throw new Exception("Upload has been cancelled.");

        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("batchupload/personalnotifications")]
        public async Task<IActionResult> BatchPersonalUpload(IFormFile file)
        {
            string message = "Batch file has been successfully uploaded.";
            var response = new BatchUploadResponse();
            var entity = new batchUploadEntity();
            try
            {
                entity.Form_ID = (int)Form.Notification;
                entity.Date_Time_Added = DateTime.Now;
                await ValidateBatchPersonalFile(file, entity);
                await _batchUploadService.Upload(file, entity, GetUserId());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message, response, key = entity.ID });
        }

        public async Task ValidateBatchPersonalFile(IFormFile file, batchUploadEntity upload)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
            var fileType = Path.GetExtension(originalFileName);

            // Validate if valid file.
            if (fileType != ".xlsx\"")
                throw new Exception("Invalid file! Please download the valid template.");

            try
            {
                upload.TotalCount = _batchUploadService.GetRecordsCount(file);
            }
            catch (Exception ex)
            {

                throw new Exception("File contains invalid data or missing fields.");
            }
            if (upload.TotalCount == 0)
                throw new Exception("File is empty!");
        }
    }
}