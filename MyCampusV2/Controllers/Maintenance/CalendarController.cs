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
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : BaseController
    {
        private readonly ICalendarService _calendarService;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;
        private readonly IBatchUploadService _batchUploadService;
        private readonly BatchUploadSettings _batchSettings;
        private readonly ExcelHelper _excelHelper;
        private ResultModel result = new ResultModel();

        public CalendarController(ICalendarService calendarService, IMapper mapper, IAuditTrailService audit, IBatchUploadService batchUploadService, IOptions<BatchUploadSettings> batchSettings, IOptions<ExcelHelper> excelHelper)
        {
            this._calendarService = calendarService;
            _mapper = mapper;
            _audit = audit;
            _batchUploadService = batchUploadService;
            _batchSettings = batchSettings.Value;
            _excelHelper = excelHelper.Value;
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("all")]
        public async Task<calendarPagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            return _mapper.Map<calendarPagedResultVM>(await _calendarService.GetAll(param.PageNo, param.PageSize, param.Keyword));
        }

        [HttpGet]
        [Route("getYearLevels")]
        public async Task<IList<calendarRecipientsVM>> GetStudents()
        {
            var data = _mapper.Map<IList<calendarRecipientsVM>>(await _calendarService.GetYearLevelList());
            calendarRecipientsVM newRecipient = new calendarRecipientsVM();
            newRecipient.key = "All";
            newRecipient.value = "All";
            data.Insert(0, newRecipient);
            return data;
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] calendarVM calendar)
        {
            try
            {
                return await _calendarService.AddCalendar(_mapper.Map<calendarEntity>(calendar), GetUserId());
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
        public async Task<ResultModel> Update([FromBody] calendarVM calendar)
        {
            try
            {
                return await _calendarService.UpdateCalendar(_mapper.Map<calendarEntity>(calendar), GetUserId());
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
        public async Task<ResultModel> Delete([FromQuery] string calendarCode)
        {
            try
            {
                return await _calendarService.DeleteCalendarPermanent(calendarCode, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }
        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("get")]
        public async Task<calendarVM> GetCalendar([FromQuery] string calendarCode)
        {
            return _mapper.Map<calendarVM>(await _calendarService.GetCalendarByCalendarCode(calendarCode));
        }

        [HttpGet]
        [Route("export")]
        public async Task<IActionResult> Export([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<calendarPagedResultVM>(await _calendarService.Export(param.Keyword == null ? string.Empty : param.Keyword));
                byte[] file = null;

                if (result.calendar.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.CalendarColHeader;
                        string wsTitle = ExcelVar.CalendarTitle;
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
                        foreach (calendarVM row in result.calendar)
                        {
                            worksheet.Cells[rowNumber, 1].Value = row.title;
                            worksheet.Cells[rowNumber, 2].Value = row.body;
                            worksheet.Cells[rowNumber, 3].Value = row.postDate;
                            worksheet.Cells[rowNumber, 4].Value = row.recipientsDisplay.Replace(',', ';');

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
    }
}
