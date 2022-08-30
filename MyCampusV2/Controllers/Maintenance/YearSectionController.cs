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
    public class YearSectionController : BaseController
    {
        private readonly IYearSectionService _yearSectionService;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;
        private readonly IBatchUploadService _batchUploadService;
        private readonly BatchUploadSettings _batchSettings;
        private readonly ExcelHelper _excelHelper;
        private ResultModel result = new ResultModel();

        public YearSectionController(IYearSectionService yearSectionService, IMapper mapper, IAuditTrailService audit, IBatchUploadService batchUploadService, IOptions<BatchUploadSettings> batchSettings, IOptions<ExcelHelper> excelHelper)
        {
            this._yearSectionService = yearSectionService;
            _mapper = mapper;
            _audit = audit;
            _batchUploadService = batchUploadService;
            _batchSettings = batchSettings.Value;
            _excelHelper = excelHelper.Value;
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getYearSectionsUsingEducationalLevelId/{id}")]
        public async Task<IList<yearSectionVM>> GetYearSectionsUsingEducationalLevelId(int id)
        {
            return _mapper.Map<IList<yearSectionVM>>(await _yearSectionService.GetYearSectionsUsingEducationalLevelId(id));
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getYearSectionById/{id}")]
        public async Task<yearSectionVM> GetYearSectionById(int id)
        {
            return _mapper.Map<yearSectionVM>(await _yearSectionService.GetYearSectionById(id));
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("all")]
        public async Task<yearSectionPagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            return _mapper.Map<yearSectionPagedResultVM>(await _yearSectionService.GetAllYearSection(param.PageNo, param.PageSize, param.Keyword));
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("allyearsection")]
        public async Task<ICollection<yearSectionVM>> GetYearSections()
        {
            return _mapper.Map<ICollection<yearSectionVM>>(await _yearSectionService.GetYearSections());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] yearSectionVM yearSection)
        {
            try
            {
                yearSection.addedBy = GetUserId();
                yearSection.updatedBy = GetUserId();
                return await _yearSectionService.AddYearSection(_mapper.Map<yearSectionEntity>(yearSection));
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
        public async Task<ResultModel> Update([FromBody] yearSectionVM yearSection)
        {
            try
            {
                yearSection.updatedBy = GetUserId();
                return await _yearSectionService.UpdateYearSection(_mapper.Map<yearSectionEntity>(yearSection));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("permanentDeleteYearLevel/{id}")]
        public async Task<ResultModel> PermanentDelete(int id)
        {
            try
            {
                return await _yearSectionService.DeleteYearSectionPermanent(id, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("temporaryDeleteYearLevel/{id}")]
        public async Task<ResultModel> TemporaryDelete(int id)
        {
            try
            {
                return await _yearSectionService.DeleteYearSectionTemporary(id, GetUserId());
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
                yearSectionEntity entity = new yearSectionEntity();
                entity.YearSec_ID = id;
                entity.Updated_By = GetUserId();
                return await _yearSectionService.RetrieveYearSection(_mapper.Map<yearSectionEntity>(entity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [HttpGet]
        [Route("downloadtemplate")]
        public async Task<IActionResult> DownloadTemplateYearSection([FromQuery] PaginationParams param, string type)
        {
            try
            {
                byte[] file = null;
                file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.YearLevelColHeader, ExcelVar.YearLevelSampleData, ExcelVar.YearLevelTemplateTitle));

                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("export")]
        public async Task<IActionResult> ExportYearSectionExcelFile([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<yearSectionPagedResultVM>(await _yearSectionService.ExportAllYearSections(param.Keyword == null ? string.Empty : param.Keyword));
                byte[] file = null;

                if (result.yearSections.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.YearLevelColHeader;
                        string wsTitle = ExcelVar.YearLevelTitle;
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
                        foreach (yearSectionVM row in result.yearSections)
                        {
                            worksheet.Cells[rowNumber, 1].Value = row.campusName;
                            worksheet.Cells[rowNumber, 2].Value = row.educLevelName;
                            worksheet.Cells[rowNumber, 3].Value = row.yearSecName;

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
                }
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

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

                IEnumerable<yearSectionBatchUploadVM> records = _batchUploadService.GetYearSectionRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                // Do the update process here.
                response = await _yearSectionService.BatchUpload(process, GetUserId(), entity.ID, lastProcess);

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

        public async Task ValidateBatchProcess(batchUploadEntity upload)
        {
            if (upload.Status == Constants.BATCH_UPLOAD_STATUS_CANCELLED)
                throw new Exception("Upload has been cancelled.");
        }

        [HttpPost]
        [Route("batchupload")]
        public async Task<IActionResult> BatchUpload(IFormFile file)
        {
            string message = "Batch file has been successfully uploaded.";
            var response = new BatchUploadResponse();
            var entity = new batchUploadEntity();
            try
            {
                entity.Form_ID = (int)Form.Campus_Year;
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
