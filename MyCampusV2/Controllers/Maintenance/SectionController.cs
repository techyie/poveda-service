using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Helpers.ActionFilters;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCampusV2.Helpers.Constants;
using MyCampusV2.Helpers.PageResult;
using System.Text;
using Microsoft.Extensions.Options;
using MyCampusV2.Common.Common;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using OfficeOpenXml;
using MyCampusV2.Helpers.ExcelHelper;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml.Style;
using System.Drawing;

namespace MyCampusV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : BaseController
    {
        private readonly ISectionService _section;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;
        private readonly IBatchUploadService _batchUploadService;
        private readonly BatchUploadSettings _batchSettings;
        private readonly ExcelHelper _excelHelper;

        public SectionController(ISectionService section, IMapper mapper, IAuditTrailService audit, IBatchUploadService batchUploadService, IOptions<BatchUploadSettings> batchSettings, IOptions<ExcelHelper> excelHelper)
        {
            _section = section;
            _mapper = mapper;
            _audit = audit;
            _batchUploadService = batchUploadService;
            _batchSettings = batchSettings.Value;
            _excelHelper = excelHelper.Value;
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        public async Task<ICollection<sectionVM>> GetAll()
        {
            return _mapper.Map<ICollection<sectionVM>>(await _section.GetAll());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("sections")]
        public async Task<sectionPagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            return _mapper.Map<sectionPagedResultVM>(await _section.GetAllSectionList(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("exportsectionlist")]
        public async Task<IActionResult> exportlist([FromQuery] PaginationParams param)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("CampusName,EducationalLevelName,YearLevelName,SectionName,SectionDescription");

                var result = _mapper.Map<sectionPagedResultVM>(await _section.ExportSections(param.Keyword));

                foreach (sectionVM row in result.sections)
                {
                    sb.AppendLine(row.Campus_Name + "," + row.Educ_Level_Name + "," + row.Year_Level_Name + "," + row.Section_Name + "," + row.Section_Desc);
                }

                return File(new System.Text.UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("exportsectionexcelfile")]
        public async Task<IActionResult> exportexcelfile([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<sectionPagedResultVM>(await _section.ExportSections(param.Keyword));
                byte[] file = null;

                if (result.sections.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.SectionColHeader;
                        string wsTitle = ExcelVar.SectionTitle;
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
                        foreach (sectionVM row in result.sections)
                        {
                            worksheet.Cells[rowNumber, 1].Value = row.Campus_Name;
                            worksheet.Cells[rowNumber, 2].Value = row.Educ_Level_Name;
                            worksheet.Cells[rowNumber, 3].Value = row.Year_Level_Name;
                            worksheet.Cells[rowNumber, 4].Value = row.Section_Name;
                            worksheet.Cells[rowNumber, 5].Value = row.Section_Desc;

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
        [HttpGet("downloadtemplate")]
        public async Task<IActionResult> DownloadTemplate([FromQuery] PaginationParams param)
        {
            try
            {
                byte[] file = null;
                file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.SectionColHeader, ExcelVar.SectionSampleData, ExcelVar.SectionTitle));
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("{id:int}")]
        public async Task<sectionVM> GetByID(int id)
        {
            return _mapper.Map<sectionVM>(await _section.GetById(id));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("yearlevel/{id:int}")]
        public async Task<ICollection<sectionVM>> GetByYearLevel(int id)
        {
            return _mapper.Map<ICollection<sectionVM>>(await _section.GetByYearLevel(id));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("yearlevelwithactivestatus/{id:int}")]
        public async Task<ICollection<sectionVM>> GetByYearLevelWithActiveStatus(int id)
        {
            return _mapper.Map<ICollection<sectionVM>>(await _section.GetByYearLevelWithActiveStatus(id));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]sectionVM section)
        {
            string message = "Section" + SuccessMessageAdd;
            try
            {
                if (section.Section_Name.Trim() == null || section.Section_Name.Trim()== string.Empty)
                {
                    return BadRequest(new { message = "Student Section Name is required" });
                }
                if (section.Section_Desc.Trim() == null || section.Section_Desc.Trim() == string.Empty)
                {
                    return BadRequest(new { message = "Student Section Description is required" });
                }

                var current = await _section.GetSection(section.Section_Name, (int)section.Year_Level_ID, (int)section.Educ_Level_ID, (int)section.Campus_ID, 0);
                if (current != null)
                    return BadRequest(new { message = "Section already exists" });

                sectionEntity sections = _mapper.Map<sectionEntity>(section);

                int userId = GetUserId();
                DateTime now = DateTime.Now;

                sections.Added_By = userId;
                sections.Date_Time_Added = now;
                sections.Updated_By = userId;
                sections.Last_Updated = now;

                await _section.AddSection(sections, userId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message });
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]sectionVM section)
        {
            string message = "Section" + SuccessMessageUpdate;
            try
            {
                if (section.Section_Name.Trim() == null || section.Section_Name.Trim() == string.Empty)
                {
                    return BadRequest(new { message = "Student Section Name is required" });
                }
                if (section.Section_Desc.Trim() == null || section.Section_Desc.Trim() == string.Empty)
                {
                    return BadRequest(new { message = "Student Section Description is required" });
                }

                var educlevel = await _section.CheckboxChecker((int)section.Campus_ID, (int)section.Educ_Level_ID);
                if (educlevel.Count == 0)
                {
                    return BadRequest(new { message = "Please select Educational Level" });
                }

                var yearlevel = await _section.CheckboxCheckerOne((int)section.Educ_Level_ID, (int)section.Year_Level_ID);
                if (yearlevel.Count == 0)
                {
                    return BadRequest(new { message = "Please select Year Level" });
                }

                var exist = await _section.GetSection(section.Section_Name, (int)section.Year_Level_ID, (int)section.Educ_Level_ID, (int)section.Campus_ID, 0);
                if (exist != null)
                {
                    if (exist.Section_ID != section.Section_ID)
                        return BadRequest(new { message = "Section already exists" });
                }

                sectionEntity sections = _mapper.Map<sectionEntity>(section);
                sections.Updated_By = GetUserId();
                sections.Last_Updated = DateTime.Now;

                await _section.UpdateSection(sections, GetUserId());

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message });
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var current = await _section.GetSection(string.Empty, 0, 0 , 0, id);
            if (current != null)
                return BadRequest(new { message = "Section already exists" });

            var result = await _section.GetCountSectionIfActive(id);
            if (result.Count > 0)
            {
                return BadRequest(new { message = "Section is in use by an active Student." });
            }

            await _section.DeleteSection(id, GetUserId());
           
            return Ok(new { message = "Section status" + SuccessMessageDelete });
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

                IEnumerable<sectionBatchUploadVM> records = _batchUploadService.GetSectionRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                // Do the update process here.
                response = await _section.BatchUpload(process, GetUserId(), entity.ID, lastProcess);

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
            // Validate if valid file.
            if (upload.Status == Constants.BATCH_UPLOAD_STATUS_CANCELLED)
                throw new Exception("Upload has been cancelled.");
        }

        public async Task ValidateBatchFile(IFormFile file, batchUploadEntity upload)
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
            catch (Exception)
            {

                throw new Exception("File contains invalid data or missing fields.");
            }

            if (upload.TotalCount == 0)
                throw new Exception("File is empty!");
        }
    }
}
