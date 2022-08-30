﻿using System;
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
    public class StudentSectionController : BaseController
    {
        private readonly IStudentSectionService _studentSectionService;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;
        private readonly IBatchUploadService _batchUploadService;
        private readonly BatchUploadSettings _batchSettings;
        private readonly ExcelHelper _excelHelper;
        private ResultModel result = new ResultModel();

        public StudentSectionController(IStudentSectionService studentSectionService, IMapper mapper, IAuditTrailService audit, IBatchUploadService batchUploadService, IOptions<BatchUploadSettings> batchSettings, IOptions<ExcelHelper> excelHelper)
        {
            this._studentSectionService = studentSectionService;
            _mapper = mapper;
            _audit = audit;
            _batchUploadService = batchUploadService;
            _batchSettings = batchSettings.Value;
            _excelHelper = excelHelper.Value;
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getStudentSectionsUsingYearSectionId/{id}")]
        public async Task<IList<studentSectionVM>> GetStudentSectionsUsingYearSectionId(int id)
        {
            return _mapper.Map<IList<studentSectionVM>>(await _studentSectionService.GetStudentSectionsUsingYearSectionId(id));
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getStudentSectionById/{id}")]
        public async Task<studentSectionVM> GetStudentSectionById(int id)
        {
            return _mapper.Map<studentSectionVM>(await _studentSectionService.GetStudentSectionById(id));
        }

        //[Authorize]
        [HttpGet]
        [Route("all")]
        public async Task<studentSecPagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            return _mapper.Map<studentSecPagedResultVM>(await _studentSectionService.GetAllStudentSection(param.PageNo, param.PageSize, param.Keyword));
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] studentSectionVM section)
        {
            try
            {
                section.addedBy = GetUserId();
                section.updatedBy = GetUserId();
                return await _studentSectionService.AddStudentSection(_mapper.Map<studentSectionEntity>(section));
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
        public async Task<ResultModel> Update([FromBody] studentSectionVM section)
        {
            try
            {
                section.updatedBy = GetUserId();
                return await _studentSectionService.UpdateStudentSection(_mapper.Map<studentSectionEntity>(section));
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
                return await _studentSectionService.DeleteStudentSectionPermanent(id, GetUserId());
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
                return await _studentSectionService.DeleteStudentSectionTemporary(id, GetUserId());
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
                studentSectionEntity newEntity = new studentSectionEntity();
                newEntity.StudSec_ID = id;
                newEntity.Updated_By = GetUserId();
                return await _studentSectionService.RetrieveStudentSection(_mapper.Map<studentSectionEntity>(newEntity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("download")]
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
        [HttpGet("export")]
        public async Task<IActionResult> ExportSection([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<studentSecPagedResultVM>(await _studentSectionService.ExportSection(param.Keyword));
                byte[] file = null;

                if (result.studentSections.Count != 0)
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
                        foreach (studentSectionVM row in result.studentSections)
                        {
                            worksheet.Cells[rowNumber, 1].Value = row.campusName;
                            worksheet.Cells[rowNumber, 2].Value = row.educLevelName;
                            worksheet.Cells[rowNumber, 3].Value = row.yearSecName;
                            worksheet.Cells[rowNumber, 4].Value = row.description;
                            worksheet.Cells[rowNumber, 5].Value = row.startTime;
                            worksheet.Cells[rowNumber, 6].Value = row.endTime;
                            worksheet.Cells[rowNumber, 7].Value = row.halfDay;
                            worksheet.Cells[rowNumber, 8].Value = row.gracePeriod;

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

                IEnumerable<studentSectionBatchUploadVM> records = _batchUploadService.GetSectionRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                // Do the update process here.
                response = await _studentSectionService.BatchUpload(process, GetUserId(), entity.ID, lastProcess);

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
                entity.Form_ID = (int)Form.Campus_Section;
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
        [Route("schedule/{id}")]
        public async Task<sectionSchedulePagedResultVM> GetSchedule(int id, [FromQuery] PaginationParams param)
        {
            return _mapper.Map<sectionSchedulePagedResultVM>(await _studentSectionService.GetAllSchedule(id, param.PageNo, param.PageSize, param.Keyword));
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpPost]
        [Route("create/schedule")]
        public async Task<ResultModel> CreateSchedule([FromBody] sectionScheduleVM schedule)
        {
            try
            {
                schedule.addedBy = GetUserId();
                schedule.updatedBy = GetUserId();
                return await _studentSectionService.AddSectionSchedule(_mapper.Map<sectionScheduleEntity>(schedule));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpPut]
        [Route("update/schedule")]
        public async Task<ResultModel> UpdateSchedule([FromBody] sectionScheduleVM schedule)
        {
            try
            {
                schedule.updatedBy = GetUserId();
                return await _studentSectionService.UpdateSectionSchedule(_mapper.Map<sectionScheduleEntity>(schedule));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpDelete]
        [Route("delete/schedule/{id}")]
        public async Task<ResultModel> DeleteSchedule(int id)
        {
            try
            {
                return await _studentSectionService.DeleteSectionSchedule(id, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getScheduleById/{id}")]
        public async Task<sectionScheduleVM> GetScheduleById(int id)
        {
            return _mapper.Map<sectionScheduleVM>(await _studentSectionService.GetSectionScheduleById(id));
        }


    }
}
