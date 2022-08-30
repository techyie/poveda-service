using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
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

namespace MyCampusV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationalLevelController : BaseController
    {
        private readonly IEducationLevelService _educlevelService;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;
        private readonly IBatchUploadService _batchUploadService;
        private readonly BatchUploadSettings _batchSettings;
        private readonly ExcelHelper _excelHelper;

        private ResultModel result = new ResultModel();

        public EducationalLevelController(IEducationLevelService educlevelService, IMapper mapper, IAuditTrailService audit, IBatchUploadService batchUploadService, IOptions<BatchUploadSettings> batchSettings, IOptions<ExcelHelper> excelHelper)
        {
            _educlevelService = educlevelService;
            _mapper = mapper;
            _audit = audit;
            _batchUploadService = batchUploadService;
            _batchSettings = batchSettings.Value;
            _excelHelper = excelHelper.Value;
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getEducationalLevelsUsingCampusId/{id}")]
        public async Task<IList<educlevelVM>> GetEducationalLevelsUsingCampusId(int id)
        {
            return _mapper.Map<IList<educlevelVM>>(await _educlevelService.GetEducationalLevelsUsingCampusId(id));
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getEducationalLevelsCollegeOnlyUsingCampusId/{id}")]
        public async Task<IList<educlevelVM>> GetEducationalLevelsCollegeOnlyUsingCampusId(int id)
        {
            return _mapper.Map<IList<educlevelVM>>(await _educlevelService.GetEducationalLevelsCollegeOnlyUsingCampusId(id));
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getEducLevelById/{id}")]
        public async Task<educlevelVM> GetEducationalLevelById(int id)
        {
            return _mapper.Map<educlevelVM>(await _educlevelService.GetEducationalLevelById(id));
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("all")]
        public async Task<educlevelPagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            return _mapper.Map<educlevelPagedResultVM>(await _educlevelService.GetAllEduclevel(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] educlevelVM educLevel)
        {
            try
            {
                educLevel.addedBy = GetUserId();
                educLevel.updatedBy = GetUserId();
                return await _educlevelService.AddEducationalLevel(_mapper.Map<educationalLevelEntity>(educLevel));
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
        public async Task<ResultModel> Update([FromBody] educlevelVM educLevel)
        {
            try
            {
                educLevel.updatedBy = GetUserId();
                return await _educlevelService.UpdateEducationalLevel(_mapper.Map<educationalLevelEntity>(educLevel));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpDelete]
        [Route("permanentDelete/{id}")]
        public async Task<ResultModel> PermanentDelete(int id)
        {
            try
            {
                return await _educlevelService.DeleteEducationalLevelPermanent(id, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpDelete]
        [Route("temporaryDelete/{id}")]
        public async Task<ResultModel> TemporaryDelete(int id)
        {
            try
            {
                return await _educlevelService.DeleteEducationalLevelTemporary(id, GetUserId());
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
                educationalLevelEntity newEntity = new educationalLevelEntity();
                newEntity.Updated_By = GetUserId();
                newEntity.Level_ID = id;
                return await _educlevelService.RetrieveEducationalLevel(_mapper.Map<educationalLevelEntity>(newEntity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet("downloadtemplate")]
        public async Task<IActionResult> DownloadTemplate([FromQuery] PaginationParams param)
        {
            try
            {
                byte[] file = null;
                file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.EducLevelColHeader, ExcelVar.EducLevelSampleData, ExcelVar.EducLevelTemplateTitle));
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet("export")]
        public async Task<IActionResult> ExportEducationalLevel([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<educlevelPagedResultVM>(await _educlevelService.ExportEducationalLevel(param.Keyword));
                byte[] file = null;

                if (result.educlevels.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.EducLevelColHeader;
                        string wsTitle = ExcelVar.EducLevelTitle;
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
                        foreach (educlevelVM row in result.educlevels)
                        {
                            worksheet.Cells[rowNumber, 1].Value = row.campusName;
                            worksheet.Cells[rowNumber, 2].Value = row.educLevelName;
                            worksheet.Cells[rowNumber, 3].Value = row.hasCourse == true ? "Yes" : "No";

                            using (var range = worksheet.Cells[rowNumber, 1, rowNumber, 4])
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

                IEnumerable<educLevelBatchUploadVM> records = _batchUploadService.GetEducLevelRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                // Do the update process here.
                response = await _educlevelService.BatchUpload(process, GetUserId(), entity.ID, lastProcess);

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
        [HttpPost("batchupload")]
        public async Task<IActionResult> BatchUpload(IFormFile file)
        {
            string message = "Batch file has been successfully uploaded.";
            var response = new BatchUploadResponse();
            var entity = new batchUploadEntity();
            try
            {
                entity.Form_ID = (int)Form.Campus_EducationalLevel;
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