using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Helpers.PageResult;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using Microsoft.AspNetCore.Authorization;
using MyCampusV2.Helpers.ActionFilters;
using OfficeOpenXml;
using MyCampusV2.Helpers.ExcelHelper;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MyCampusV2.Common;
using MyCampusV2.Helpers.Constants;

namespace MyCampusV2.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmergencyLogoutController : BaseController
    {
        private readonly IEmergencyLogoutService _emergencyLogoutService;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;
        private readonly IBatchUploadService _batchUploadService;
        private readonly BatchUploadSettings _batchSettings;
        private readonly ExcelHelper _excelHelper;

        private ResultModel result = new ResultModel();

        public EmergencyLogoutController(IEmergencyLogoutService emergencyLogoutService, IMapper mapper, IAuditTrailService audit, IBatchUploadService batchUploadService, IOptions<BatchUploadSettings> batchSettings, IOptions<ExcelHelper> excelHelper)
        {
            this._emergencyLogoutService = emergencyLogoutService;
            _mapper = mapper;
            _audit = audit;
            _batchUploadService = batchUploadService;
            _batchSettings = batchSettings.Value;
            _excelHelper = excelHelper.Value;
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("getEmergencyLogoutById/{id}")]
        public async Task<emergencyLogoutVM> GetEmergencyLogoutById(int id)
        {
            return _mapper.Map<emergencyLogoutVM>(await _emergencyLogoutService.GetEmergencyLogoutById(id));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("all")]
        public async Task<emergencyLogoutPagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            return _mapper.Map<emergencyLogoutPagedResultVM>(await _emergencyLogoutService.GetAll(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("studentList")]
        public async Task<studentListPagedResultVM> GetStudentList([FromQuery] StudentListFilter filter)
        {
            return _mapper.Map<studentListPagedResultVM>(await _emergencyLogoutService.GetStudentList(filter.campusId, filter.educLevelId, filter.yearSecId, filter.studSecId));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] emergencyLogoutVM entity)
        {
            try
            {
                entity.addedBy = GetUserId();
                entity.updatedBy = GetUserId();
                entity.userId = GetUserId();
                return await _emergencyLogoutService.AddEmergencyLogout(_mapper.Map<emergencyLogoutEntity>(entity));
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
        public async Task<ResultModel> Update([FromBody] emergencyLogoutVM entity)
        {
            try
            {
                entity.addedBy = GetUserId();
                entity.updatedBy = GetUserId();
                entity.userId = GetUserId();
                return await _emergencyLogoutService.UpdateEmergencyLogout(_mapper.Map<emergencyLogoutEntity>(entity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("createbybatch/{studentRemarks}/{studentEffectiveDate}")]
        public async Task<ResultModel> CreateByBatch([FromBody] string[] studentList, string studentRemarks, string studentEffectiveDate)
        {
            try
            {
                return await _emergencyLogoutService.CreateByBatch(studentList, studentRemarks, studentEffectiveDate, GetUserId());
            }
            catch(Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("permanentDeleteEmergencyLogout/{id}")]
        public async Task<ResultModel> PermanentDelete(int id)
        {
            try
            {
                return await _emergencyLogoutService.DeleteEmergencyLogoutPermanent(id, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("temporaryDeleteEmergencyLogout/{id}")]
        public async Task<ResultModel> TemporaryDelete(int id)
        {
            try
            {
                return await _emergencyLogoutService.DeleteEmergencyLogoutTemporary(id, GetUserId());
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
        public async Task<ResultModel> RetrieveData(int id)
        {
            try
            {
                return await _emergencyLogoutService.RetrieveEmergencyLogout(id, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportEmergencyLogoutStudentsExcelFile")]
        public async Task<IActionResult> ExportEmergencyLogoutStudentsExcelFile([FromQuery] emergencyLogoutStudentsFilter filter)
        {
            try
            {
                var result = _mapper.Map<emergencyLogoutPagedResultVM>(await _emergencyLogoutService.ExportEmergencyLogoutStudentsExcelFile(
                        filter.campusId,
                        filter.educLevelId,
                        filter.yearSecId,
                        filter.studSecId));

                byte[] file = null;

                if (result.emergencyLogouts.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.EmergencyLogoutStudentsColHeader;
                    string wsTitle = ExcelVar.EmergencyLogoutStudentsTitle;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[1, i].Value = ColHeader[i - 1].Replace("*", string.Empty);

                        if (ColHeader[i - 1].Contains("*"))
                        {
                            worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.IndianRed);
                        }
                    }

                    using (var range = worksheet.Cells[1, 1, 1, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 2;
                    foreach (emergencyLogoutVM row in result.emergencyLogouts)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.studentName;
                        worksheet.Cells[rowNumber, 2].Value = filter.studentRemarks;
                        worksheet.Cells[rowNumber, 3].Value = filter.studentEffectivityDate;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";
                        }

                        rowNumber++;
                    }

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Column(i).AutoFit();
                    }

                    package.Workbook.Properties.Title = wsTitle;
                    package.Workbook.Properties.Author = ExcelVar.Author;
                    package.Workbook.Properties.Company = ExcelVar.Company;

                    file = package.GetAsByteArray();
                }
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
        [Route("batchupload/{id:int}")]
        public async Task<IActionResult> BatchUploadProcess(int id)
        {
            string message = "Batch file has been successfully process.";
            var entity = await _batchUploadService.GetByIdSync(id);
            var response = new BatchUploadResponse();
            try
            {
                await ValidateBatchProcess(entity);
                int lastProcess = entity.ProcessCount;

                IEnumerable<emergencyLogoutBatchUploadVM> records = _batchUploadService.GetEmergencyLogoutRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                // Do the update process here.
                response = await _emergencyLogoutService.BatchUpload(process, GetUserId(), entity.ID, lastProcess);

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
        [CustomAuthorize]
        [HttpPost]
        [Route("batchupload")]
        public async Task<IActionResult> BatchUpload(IFormFile file)
        {
            string message = "Batch file has been successfully uploaded.";
            var response = new BatchUploadResponse();
            var entity = new batchUploadEntity();
            try
            {
                entity.Form_ID = (int)Form.Fetcher_Emergency_Logout;
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
    }
}
