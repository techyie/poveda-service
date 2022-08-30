using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Helpers.Constants;
using MyCampusV2.Helpers.ActionFilters;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Helpers.PageResult;
using System.Text;
using System.IO;
using MyCampusV2.Services.IServices;
using MyCampusV2.Common;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using CsvHelper;
using OfficeOpenXml;
using ExcelDataReader;
using System.Runtime.Serialization.Formatters.Binary;
using MyCampusV2.Helpers.ExcelHelper;
using System.Drawing;
using OfficeOpenXml.Style;
using MyCampusV2.Models.V2.entity;

namespace MyCampusV2.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CampusController : BaseController
    {
        private readonly ICampusService _campusService;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;
        private readonly IBatchUploadService _batchUploadService;
        private readonly BatchUploadSettings _batchSettings;
        private readonly ExcelHelper _excelHelper;

        public CampusController(ICampusService campusService, IMapper mapper, IAuditTrailService audit, IBatchUploadService batchUploadService, IOptions<BatchUploadSettings> batchSettings, IOptions<ExcelHelper> excelHelper)
        {
            _campusService = campusService;
            _mapper = mapper;
            _audit = audit;
            _batchUploadService = batchUploadService;
            _batchSettings = batchSettings.Value;
            _excelHelper = excelHelper.Value;
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("all")]
        public async Task<campusPagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            return _mapper.Map<campusPagedResultVM>(await _campusService.GetAllCampuses(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] campusVM campus)
        {
            return await _campusService.AddCampus(_mapper.Map<campusEntity>(campus), GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("update")]
        public async Task<ResultModel> Update([FromBody] campusVM campus)
        {
            return await _campusService.UpdateCampus(_mapper.Map<campusEntity>(campus), GetUserId());
        }


        [Authorize]
        //[CustomAuthorize]
        [HttpDelete]
        [Route("permanentDelete/{id}")]
        public async Task<ResultModel> PermanentDelete(int id)
        {
            try
            {
                return await _campusService.DeleteCampusPermanent(id, GetUserId());
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
                return await _campusService.DeleteCampusTemporary(id, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("region")]
        public async Task<ICollection<regionEntity>> GetRegion()
        {
            var result = _mapper.Map<ICollection<regionEntity>>(await _campusService.GetRegion());
            return result.ToList();
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("division")]
        public async Task<ICollection<divisionEntity>> GetDivision([FromQuery] int id)
        {
            var result = await _campusService.GetDivisionByRegion(id);
            return result.ToList();
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("campus")]
        public async Task<campusVM> GetCampusByID([FromQuery] int id)
        {
            return _mapper.Map<campusVM>(await _campusService.GetCampusByID(id));
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("allcampus")]
        public async Task<ICollection<campusVM>> GetCampuses()
        {
            return _mapper.Map<ICollection<campusVM>>(await _campusService.GetCampuses());
        }
        
        [Authorize]
        [CustomAuthorize]
        [HttpGet("downloadtemplate")]
        public async Task<IActionResult> DownloadTemplate([FromQuery] PaginationParams param)
        {
            try
            {
                byte[] file = null;
                file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.CampusColHeader, ExcelVar.CampusSampleData, ExcelVar.CampusTemplateTitle));
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
        public async Task<IActionResult> ExportCampus([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<campusPagedResultVM>(await _campusService.ExportCampus(param.Keyword));
                byte[] file = null;

                if (result.campuses.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.CampusColHeader;
                        string wsTitle = ExcelVar.CampusTitle;
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
                        foreach (campusVM row in result.campuses)
                        {
                            worksheet.Cells[rowNumber, 1].Value = row.campusCode;
                            worksheet.Cells[rowNumber, 2].Value = row.campusName;
                            worksheet.Cells[rowNumber, 3].Value = row.campusStatus;
                            worksheet.Cells[rowNumber, 4].Value = row.campusAddress;
                            worksheet.Cells[rowNumber, 5].Value = row.campusContactNo;
                            worksheet.Cells[rowNumber, 6].Value = row.regionName;
                            worksheet.Cells[rowNumber, 7].Value = row.divisionName;

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

                IEnumerable<campusBatchUploadVM> records = _batchUploadService.GetCampusRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                // Do the update process here.
                response = await _campusService.BatchUpload(process, GetUserId(), entity.ID, lastProcess);

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
                entity.Form_ID = (int)Form.Campus_Campus;
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