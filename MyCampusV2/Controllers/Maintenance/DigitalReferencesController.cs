using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Helpers.ActionFilters;
using MyCampusV2.Helpers.ExcelHelper;
using MyCampusV2.Helpers.PageResult;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.IServices;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MyCampusV2.Controllers.Maintenance
{ 

    [Route("api/[controller]")]
    [ApiController]
    public class DigitalReferencesController : BaseController
    {
        private readonly IDigitalReferencesService _digitalReferencesService;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;
        private readonly IBatchUploadService _batchUploadService;
        private readonly BatchUploadSettings _batchSettings;
        private readonly ExcelHelper _excelHelper;
        private readonly FileSettings _fileSettings;
        private ResultModel result = new ResultModel();

        public DigitalReferencesController(IDigitalReferencesService digitalReferencesService, IMapper mapper, IAuditTrailService audit, IBatchUploadService batchUploadService, IOptions<BatchUploadSettings> batchSettings, IOptions<ExcelHelper> excelHelper, IOptions<FileSettings> fileSettings)
        {
            this._digitalReferencesService = digitalReferencesService;
            _mapper = mapper;
            _audit = audit;
            _batchUploadService = batchUploadService;
            _batchSettings = batchSettings.Value;
            _excelHelper = excelHelper.Value;
            _fileSettings = fileSettings.Value;
        }

        [Authorize]
        [HttpGet]
        [Route("all")]
        public async Task<digitalReferencesPagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            return _mapper.Map<digitalReferencesPagedResultVM>(await _digitalReferencesService.GetAll(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] digitalReferencesVM digitalReferencesVM)
        {
            try
            {
                return await _digitalReferencesService.AddDigitalReference(_mapper.Map<digitalReferencesEntity>(digitalReferencesVM), GetUserId());
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
        public async Task<ResultModel> Update([FromBody] digitalReferencesVM digitalReferencesVM)
        {
            try
            {
                return await _digitalReferencesService.UpdateDigitalReference(_mapper.Map<digitalReferencesEntity>(digitalReferencesVM), GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("delete")]
        public async Task<ResultModel> Delete([FromQuery] string digitalReferenceCode)
        {
            try
            {
                return await _digitalReferencesService.DeleteDigitalReferencePermanent(digitalReferenceCode, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("get")]
        public async Task<digitalReferencesVM> GetDigitalRefence([FromQuery] string digitalReferenceCode)
        {
            return _mapper.Map<digitalReferencesVM>(await _digitalReferencesService.GetDigitalReferenceByCode(digitalReferenceCode));
        }

        [HttpGet]
        [Route("export")]
        public async Task<IActionResult> Export([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<digitalReferencesPagedResultVM>(await _digitalReferencesService.Export(param.Keyword == null ? string.Empty : param.Keyword));
                byte[] file = null;

                if (result.digitalReferences.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.DigitalReferencesColHeader;
                        string wsTitle = ExcelVar.DigitalReferencesTitle;
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
                        foreach (digitalReferencesVM row in result.digitalReferences)
                        {
                            worksheet.Cells[rowNumber, 1].Value = row.title;
                            worksheet.Cells[rowNumber, 2].Value = row.fileType;
                            worksheet.Cells[rowNumber, 3].Value = Convert.ToDateTime(row.dateUploaded).ToString("M/dd/yyyy h:mm tt");

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

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                ValidateFile(file);
                var result = await _digitalReferencesService.Upload(file, _fileSettings.FilePath);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = string.Empty });
            }
        }

        public void ValidateFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
            var fileType = Path.GetExtension(originalFileName);

            if (fileType != ".xlsx\"" && fileType != ".pdf\"" && fileType != ".docx\"")
                throw new Exception("Invalid file! Please download the valid template.");

            if (file.Length == 0)
                throw new Exception("File is empty!");
        }
    }
}
