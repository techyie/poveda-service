using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Services.IServices;
using MyCampusV2.Models;
using System.IO;
using MyCampusV2.Helpers.PageResult;
using System.Text;
using OfficeOpenXml;
using MyCampusV2.Common.ViewModels.ReportViewModel;
using MyCampusV2.Models.Reports;
using MyCampusV2.Helpers.ExcelHelper;
using MyCampusV2.Helpers.ActionFilters;
using Microsoft.Extensions.Options;
using iTextSharp.text.pdf;
using iTextSharp.text;
using MyCampusV2.Helpers;

namespace MyCampusV2.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;
        private readonly ExcelHelper _excelHelper;
        private Logger _logger = new Logger(); 

        public ReportController(IReportService reportService, IMapper mapper, IOptions<ExcelHelper> excelHelper)
        {
            _reportService = reportService;
            _mapper = mapper;
            _excelHelper = excelHelper.Value;
        }

        [HttpGet("deped")]
        public async Task<IActionResult> DownloadDepedReport([FromQuery] string schoolyear, string month, int section)
        {
            try
            {
                var schedule = await _reportService.GetSchedule(schoolyear, month, section);
                var headers = await _reportService.GetHeaders(schoolyear, month, section);
                var attendance = await _reportService.GetAttendance(schedule, schoolyear, month, section);
                
                byte[] file = null;

                if (attendance.Count <= 0)
                    return BadRequest(new { message = "No data available." });
                
                file = await Task.Run(() => _excelHelper.ExportDepedReport(schedule, headers, attendance));
                return File(file, "application/zip", "export");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("getDetailsPerPersonByID/{idNumber}/{date}")]
        public async Task<detailsPerPersonPagedResultVM> GetDetailsPerPersonByID(string idNumber, string date)
        {
            return _mapper.Map<detailsPerPersonPagedResultVM>(await _reportService.GetDetailsPerPersonByID(idNumber, date));
        }

        #region Time and Attendance List

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("timeattendancealllist")]
        public async Task<timeAndAttendanceAllPagedResultVM> GetAllTimeAttendanceAll([FromQuery] timeAttendanceAllFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<timeAndAttendanceAllPagedResultVM>(
                await _reportService.GetAllTimeAttendanceAll(
                    filter.campusId,
                    filter.personId,
                    filter.reportType,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("timeattendancestudentlist")]
        public async Task<timeAndAttendanceStudentPagedResultVM> GetAllTimeAttendanceStudent([FromQuery] timeAttendanceStudentFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<timeAndAttendanceStudentPagedResultVM>(
                await _reportService.GetAllTimeAttendanceStudent(
                    filter.campusId,
                    filter.educLevelId,
                    filter.yearSecId,
                    filter.studSecId,
                    filter.personId,
                    filter.reportType,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("timeattendanceemployeelist")]
        public async Task<timeAndAttendanceEmployeePagedResultVM> GetAllTimeAttendanceEmployee([FromQuery] timeAttendanceEmployeesFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<timeAndAttendanceEmployeePagedResultVM>(
                await _reportService.GetAllTimeAttendanceEmployee(
                    filter.campusId,
                    filter.employeeTypeId,
                    filter.employeeSubTypeId,
                    filter.personId,
                    filter.reportType,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("timeattendanceotheraccesslist")]
        public async Task<timeAndAttendanceOtherAccessPagedResultVM> GetAllTimeAttendanceOtherAccess([FromQuery] timeAttendanceOtherAccessFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<timeAndAttendanceOtherAccessPagedResultVM>(
                await _reportService.GetAllTimeAttendanceOtherAccess(
                    filter.campusId,
                    filter.officeId,
                    filter.personId,
                    filter.reportType,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("timeattendancefetcherlist")]
        public async Task<timeAndAttendanceFetcherPagedResultVM> GetAllTimeAttendanceFetcher([FromQuery] timeAttendanceFetcherFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<timeAndAttendanceFetcherPagedResultVM>(
                await _reportService.GetAllTimeAttendanceFetcher(
                    filter.campusId,
                    filter.personId,
                    filter.reportType,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        #endregion

        #region Time and Attendance Excel

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exporttimeattendanceallexcel")]
        public async Task<IActionResult> ExportTimeAttendanceAllExcel([FromQuery] timeAttendanceAllFilter filter)
        {
            try
            {
                var result = _mapper.Map<timeAndAttendanceAllPagedResultVM>(
                    await _reportService.ExportTimeAttendanceAll(
                        filter.campusId,
                        filter.personId,
                        filter.reportType,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.TimeAndAttendanceColHeader;
                    string wsTitle = (filter.reportType == "ATTENDANCE" ? ExcelVar.TAATitle : filter.reportType == "ABSENT" ? "Absent Report" : "Present Report");
                    int rowHeader = 9;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    if (filter.reportType == "ATTENDANCE")
                    {
                        worksheet.Cells["A1:E1"].Merge = true;
                        worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        worksheet.Cells["A3:E3"].Merge = true;
                        //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                        worksheet.Cells[3, 1].Value = "";
                        worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A4:E4"].Merge = true;
                        worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                        worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A5:E5"].Merge = true;
                        worksheet.Cells[5, 1].Value = wsTitle;
                        worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A7:B7"].Merge = true;
                        worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt");

                        worksheet.Cells["C7:E7"].Merge = true;
                        worksheet.Cells[7, 3].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt");
                        worksheet.Cells[7, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    }
                    else
                    {
                        worksheet.Cells["A1:C1"].Merge = true;
                        worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        worksheet.Cells["A3:C3"].Merge = true;
                        //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                        worksheet.Cells[3, 1].Value = "";
                        worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A4:C4"].Merge = true;
                        worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                        worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A5:C5"].Merge = true;
                        worksheet.Cells[5, 1].Value = wsTitle;
                        worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells[7, 1].Value = "From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt");

                        worksheet.Cells["B7:C7"].Merge = true;
                        worksheet.Cells[7, 2].Value = "End: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt");
                        worksheet.Cells[7, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    }

                    for (int i = 1; i <= (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2)); i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2))])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 10;
                    foreach (TimeAndAttendanceAllVM row in result.dailylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Date;
                        if (filter.reportType == "ATTENDANCE")
                        {
                            worksheet.Cells[rowNumber, 4].Value = row.Log_In;
                            worksheet.Cells[rowNumber, 5].Value = row.Log_Out;
                        }

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2))])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }

                        rowNumber++;
                    }

                    for (int i = 1; i <= (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2)); i++)
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
        [Route("exporttimeattendancestudentexcel")]
        public async Task<IActionResult> ExportTimeAttendanceStudentExcel([FromQuery] timeAttendanceStudentFilter filter)
        {
            try
            {
                var result = _mapper.Map<timeAndAttendanceStudentPagedResultVM>(
                    await _reportService.ExportTimeAttendanceStudent(
                        filter.campusId,
                        filter.educLevelId,
                        filter.yearSecId,
                        filter.studSecId,
                        filter.personId,
                        filter.reportType,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.TimeAndAttendanceColHeader;
                    string wsTitle = (filter.reportType == "ATTENDANCE" ? ExcelVar.TAATitle : filter.reportType == "ABSENT" ? "Absent Report" : "Present Report");
                    int rowHeader = 9;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    if (filter.reportType == "ATTENDANCE")
                    {
                        worksheet.Cells["A1:E1"].Merge = true;
                        worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        worksheet.Cells["A3:E3"].Merge = true;
                        //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                        worksheet.Cells[3, 1].Value = "";
                        worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A4:E4"].Merge = true;
                        worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                        worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A5:E5"].Merge = true;
                        worksheet.Cells[5, 1].Value = wsTitle;
                        worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A7:B7"].Merge = true;
                        worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt");

                        worksheet.Cells["C7:E7"].Merge = true;
                        worksheet.Cells[7, 3].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt");
                        worksheet.Cells[7, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    }
                    else
                    {
                        worksheet.Cells["A1:C1"].Merge = true;
                        worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        worksheet.Cells["A3:C3"].Merge = true;
                        //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                        worksheet.Cells[3, 1].Value = "";
                        worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A4:C4"].Merge = true;
                        worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                        worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A5:C5"].Merge = true;
                        worksheet.Cells[5, 1].Value = wsTitle;
                        worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells[7, 1].Value = "From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt");

                        worksheet.Cells["B7:C7"].Merge = true;
                        worksheet.Cells[7, 2].Value = "End: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt");
                        worksheet.Cells[7, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    }

                    for (int i = 1; i <= (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2)); i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2))])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 10;
                    foreach (TimeAndAttendanceStudentVM row in result.dailylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Date;
                        if (filter.reportType == "ATTENDANCE")
                        {
                            worksheet.Cells[rowNumber, 4].Value = row.Log_In;
                            worksheet.Cells[rowNumber, 5].Value = row.Log_Out;
                        }

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2))])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }

                        rowNumber++;
                    }

                    for (int i = 1; i <= (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2)); i++)
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
        [Route("exporttimeattendanceemployeeexcel")]
        public async Task<IActionResult> ExportTimeAttendanceEmployeeExcel([FromQuery] timeAttendanceEmployeesFilter filter)
        {
            try
            {
                var result = _mapper.Map<timeAndAttendanceEmployeePagedResultVM>(
                    await _reportService.ExportTimeAttendanceEmployee(
                        filter.campusId,
                        filter.employeeTypeId,
                        filter.employeeSubTypeId,
                        filter.personId,
                        filter.reportType,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.TimeAndAttendanceColHeader;
                    string wsTitle = (filter.reportType == "ATTENDANCE" ? ExcelVar.TAATitle : filter.reportType == "ABSENT" ? "Absent Report" : "Present Report");
                    int rowHeader = 9;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    if (filter.reportType == "ATTENDANCE")
                    {
                        worksheet.Cells["A1:E1"].Merge = true;
                        worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        worksheet.Cells["A3:E3"].Merge = true;
                        //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                        worksheet.Cells[3, 1].Value = "";
                        worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A4:E4"].Merge = true;
                        worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                        worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A5:E5"].Merge = true;
                        worksheet.Cells[5, 1].Value = wsTitle;
                        worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A7:B7"].Merge = true;
                        worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt");

                        worksheet.Cells["C7:E7"].Merge = true;
                        worksheet.Cells[7, 3].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt");
                        worksheet.Cells[7, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    }
                    else
                    {
                        worksheet.Cells["A1:C1"].Merge = true;
                        worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        worksheet.Cells["A3:C3"].Merge = true;
                        //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                        worksheet.Cells[3, 1].Value = "";
                        worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A4:C4"].Merge = true;
                        worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                        worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A5:C5"].Merge = true;
                        worksheet.Cells[5, 1].Value = wsTitle;
                        worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells[7, 1].Value = "From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt");

                        worksheet.Cells["B7:C7"].Merge = true;
                        worksheet.Cells[7, 2].Value = "End: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt");
                        worksheet.Cells[7, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    }

                    for (int i = 1; i <= (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2)); i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2))])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 10;
                    foreach (TimeAndAttendanceEmployeeVM row in result.dailylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Date;
                        if (filter.reportType == "ATTENDANCE")
                        {
                            worksheet.Cells[rowNumber, 4].Value = row.Log_In;
                            worksheet.Cells[rowNumber, 5].Value = row.Log_Out;
                        }

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2))])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }

                        rowNumber++;
                    }

                    for (int i = 1; i <= (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2)); i++)
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
        [Route("exporttimeattendanceotheraccessexcel")]
        public async Task<IActionResult> ExportTimeAttendanceOtherAccessExcel([FromQuery] timeAttendanceOtherAccessFilter filter)
        {
            try
            {
                var result = _mapper.Map<timeAndAttendanceOtherAccessPagedResultVM>(
                    await _reportService.ExportTimeAttendanceOtherAccess(
                        filter.campusId,
                        filter.officeId,
                        filter.personId,
                        filter.reportType,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.TimeAndAttendanceColHeader;
                    string wsTitle = (filter.reportType == "ATTENDANCE" ? ExcelVar.TAATitle : filter.reportType == "ABSENT" ? "Absent Report" : "Present Report");
                    int rowHeader = 9;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    if (filter.reportType == "ATTENDANCE")
                    {
                        worksheet.Cells["A1:E1"].Merge = true;
                        worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        worksheet.Cells["A3:E3"].Merge = true;
                        //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                        worksheet.Cells[3, 1].Value = "";
                        worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A4:E4"].Merge = true;
                        worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                        worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A5:E5"].Merge = true;
                        worksheet.Cells[5, 1].Value = wsTitle;
                        worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A7:B7"].Merge = true;
                        worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt");

                        worksheet.Cells["C7:E7"].Merge = true;
                        worksheet.Cells[7, 3].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt");
                        worksheet.Cells[7, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    }
                    else
                    {
                        worksheet.Cells["A1:C1"].Merge = true;
                        worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        worksheet.Cells["A3:C3"].Merge = true;
                        //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                        worksheet.Cells[3, 1].Value = "";
                        worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A4:C4"].Merge = true;
                        worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                        worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A5:C5"].Merge = true;
                        worksheet.Cells[5, 1].Value = wsTitle;
                        worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells[7, 1].Value = "From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt");

                        worksheet.Cells["B7:C7"].Merge = true;
                        worksheet.Cells[7, 2].Value = "End: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt");
                        worksheet.Cells[7, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    }

                    for (int i = 1; i <= (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2)); i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2))])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 10;
                    foreach (TimeAndAttendanceOtherAccessVM row in result.dailylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Date;
                        if (filter.reportType == "ATTENDANCE")
                        {
                            worksheet.Cells[rowNumber, 4].Value = row.Log_In;
                            worksheet.Cells[rowNumber, 5].Value = row.Log_Out;
                        }

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2))])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }

                        rowNumber++;
                    }

                    for (int i = 1; i <= (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2)); i++)
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
        [Route("exporttimeattendancefetcherexcel")]
        public async Task<IActionResult> ExportTimeAttendanceFetcherExcel([FromQuery] timeAttendanceFetcherFilter filter)
        {
            try
            {
                var result = _mapper.Map<timeAndAttendanceFetcherPagedResultVM>(
                    await _reportService.ExportTimeAttendanceFetcher(
                        filter.campusId,
                        filter.personId,
                        filter.reportType,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.TimeAndAttendanceColHeader;
                    string wsTitle = (filter.reportType == "ATTENDANCE" ? ExcelVar.TAATitle : filter.reportType == "ABSENT" ? "Absent Report" : "Present Report");
                    int rowHeader = 9;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    if (filter.reportType == "ATTENDANCE")
                    {
                        worksheet.Cells["A1:E1"].Merge = true;
                        worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        worksheet.Cells["A3:E3"].Merge = true;
                        //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                        worksheet.Cells[3, 1].Value = "";
                        worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A4:E4"].Merge = true;
                        worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                        worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A5:E5"].Merge = true;
                        worksheet.Cells[5, 1].Value = wsTitle;
                        worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A7:B7"].Merge = true;
                        worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt");

                        worksheet.Cells["C7:E7"].Merge = true;
                        worksheet.Cells[7, 3].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt");
                        worksheet.Cells[7, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    }
                    else
                    {
                        worksheet.Cells["A1:C1"].Merge = true;
                        worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        worksheet.Cells["A3:C3"].Merge = true;
                        //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                        worksheet.Cells[3, 1].Value = "";
                        worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A4:C4"].Merge = true;
                        worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                        worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A5:C5"].Merge = true;
                        worksheet.Cells[5, 1].Value = wsTitle;
                        worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells[7, 1].Value = "From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt");

                        worksheet.Cells["B7:C7"].Merge = true;
                        worksheet.Cells[7, 2].Value = "End: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt");
                        worksheet.Cells[7, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    }

                    for (int i = 1; i <= (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2)); i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2))])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 10;
                    foreach (TimeAndAttendanceFetcherVM row in result.dailylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Date;
                        if (filter.reportType == "ATTENDANCE")
                        {
                            worksheet.Cells[rowNumber, 4].Value = row.Log_In;
                            worksheet.Cells[rowNumber, 5].Value = row.Log_Out;
                        }

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2))])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }

                        rowNumber++;
                    }

                    for (int i = 1; i <= (filter.reportType == "ATTENDANCE" ? ColHeader.Length : (ColHeader.Length - 2)); i++)
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

        #endregion

        #region Time and Attendance PDF

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exporttimeattendanceallpdf")]
        public async Task<IActionResult> ExportTimeAttendanceAllPDF([FromQuery] timeAttendanceAllFilter filter)
        {
            try
            {
                var result = _mapper.Map<timeAndAttendanceAllPagedResultVM>(
                    await _reportService.ExportTimeAttendanceAll(
                        filter.campusId,
                        filter.personId,
                        filter.reportType,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date Generated : " + DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Time and Attendance Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(filter.reportType == "ATTENDANCE" ? 5 : 3);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Date", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                if (filter.reportType == "ATTENDANCE")
                {
                    cell = new PdfPCell(new Phrase("Log In", FontTahoma6Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Log Out", FontTahoma6Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                }

                table.HeaderRows = 1;

                foreach (TimeAndAttendanceAllVM row in result.dailylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                    if (filter.reportType == "ATTENDANCE")
                    {
                        table.AddCell(new Phrase(row.Log_In, FontTahoma6Normal));
                        table.AddCell(new Phrase(row.Log_Out, FontTahoma6Normal));
                    }
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exporttimeattendancestudentpdf")]
        public async Task<IActionResult> ExportTimeAttendanceStudentPDF([FromQuery] timeAttendanceStudentFilter filter)
        {
            try
            {
                var result = _mapper.Map<timeAndAttendanceStudentPagedResultVM>(
                    await _reportService.ExportTimeAttendanceStudent(
                        filter.campusId,
                        filter.educLevelId,
                        filter.yearSecId,
                        filter.studSecId,
                        filter.personId,
                        filter.reportType,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date Generated : " + DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Time and Attendance Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(filter.reportType == "ATTENDANCE" ? 5 : 3);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Date", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                if (filter.reportType == "ATTENDANCE")
                {
                    cell = new PdfPCell(new Phrase("Log In", FontTahoma6Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Log Out", FontTahoma6Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                }

                table.HeaderRows = 1;

                foreach (TimeAndAttendanceStudentVM row in result.dailylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                    if (filter.reportType == "ATTENDANCE")
                    {
                        table.AddCell(new Phrase(row.Log_In, FontTahoma6Normal));
                        table.AddCell(new Phrase(row.Log_Out, FontTahoma6Normal));
                    }
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exporttimeattendanceemployeepdf")]
        public async Task<IActionResult> ExportTimeAttendanceEmployeePDF([FromQuery] timeAttendanceEmployeesFilter filter)
        {
            try
            {
                var result = _mapper.Map<timeAndAttendanceEmployeePagedResultVM>(
                    await _reportService.ExportTimeAttendanceEmployee(
                        filter.campusId,
                        filter.employeeTypeId,
                        filter.employeeSubTypeId,
                        filter.personId,
                        filter.reportType,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date Generated : " + DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Time and Attendance Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(filter.reportType == "ATTENDANCE" ? 5 : 3);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Date", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                if (filter.reportType == "ATTENDANCE")
                {
                    cell = new PdfPCell(new Phrase("Log In", FontTahoma6Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Log Out", FontTahoma6Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                }

                table.HeaderRows = 1;

                foreach (TimeAndAttendanceEmployeeVM row in result.dailylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                    if (filter.reportType == "ATTENDANCE")
                    {
                        table.AddCell(new Phrase(row.Log_In, FontTahoma6Normal));
                        table.AddCell(new Phrase(row.Log_Out, FontTahoma6Normal));
                    }
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exporttimeattendanceotheraccesspdf")]
        public async Task<IActionResult> ExportTimeAttendanceOtherAccessPDF([FromQuery] timeAttendanceOtherAccessFilter filter)
        {
            try
            {
                var result = _mapper.Map<timeAndAttendanceOtherAccessPagedResultVM>(
                    await _reportService.ExportTimeAttendanceOtherAccess(
                        filter.campusId,
                        filter.officeId,
                        filter.personId,
                        filter.reportType,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date Generated : " + DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Time and Attendance Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(filter.reportType == "ATTENDANCE" ? 5 : 3);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Date", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                if (filter.reportType == "ATTENDANCE")
                {
                    cell = new PdfPCell(new Phrase("Log In", FontTahoma6Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Log Out", FontTahoma6Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                }

                table.HeaderRows = 1;

                foreach (TimeAndAttendanceOtherAccessVM row in result.dailylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                    if (filter.reportType == "ATTENDANCE")
                    {
                        table.AddCell(new Phrase(row.Log_In, FontTahoma6Normal));
                        table.AddCell(new Phrase(row.Log_Out, FontTahoma6Normal));
                    }
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exporttimeattendancefetcherpdf")]
        public async Task<IActionResult> ExportTimeAttendanceFetcherPDF([FromQuery] timeAttendanceFetcherFilter filter)
        {
            try
            {
                var result = _mapper.Map<timeAndAttendanceFetcherPagedResultVM>(
                    await _reportService.ExportTimeAttendanceFetcher(
                        filter.campusId,
                        filter.personId,
                        filter.reportType,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date Generated : " + DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Time and Attendance Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy hh:mm tt"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy hh:mm tt"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(filter.reportType == "ATTENDANCE" ? 5 : 3);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Date", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                if (filter.reportType == "ATTENDANCE")
                {
                    cell = new PdfPCell(new Phrase("Log In", FontTahoma6Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Log Out", FontTahoma6Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                }

                table.HeaderRows = 1;

                foreach (TimeAndAttendanceFetcherVM row in result.dailylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                    if (filter.reportType == "ATTENDANCE")
                    {
                        table.AddCell(new Phrase(row.Log_In, FontTahoma6Normal));
                        table.AddCell(new Phrase(row.Log_Out, FontTahoma6Normal));
                    }
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Alarm List

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("alarmalllist")]
        public async Task<alarmAllPagedResultVM> GetAllAlarmAll([FromQuery] alarmAllFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<alarmAllPagedResultVM>(
                await _reportService.GetAllAlarmAll(
                    filter.campusId,
                    filter.personId,
                    filter.persontype,
                    filter.logMessage,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("alarmstudentlist")]
        public async Task<alarmStudentPagedResultVM> GetAllAlarmStudent([FromQuery] alarmStudentFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<alarmStudentPagedResultVM>(
                await _reportService.GetAllAlarmStudent(
                    filter.campusId,
                    filter.educLevelId,
                    filter.yearSecId,
                    filter.studSecId,
                    filter.personId,
                    filter.persontype,
                    filter.logMessage,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("alarmemployeelist")]
        public async Task<alarmEmployeePagedResultVM> GetAllAlarmEmployee([FromQuery] alarmEmployeesFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<alarmEmployeePagedResultVM>(
                await _reportService.GetAllAlarmEmployee(
                    filter.campusId,
                    filter.personId,
                    filter.persontype,
                    filter.logMessage,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("alarmotheraccesslist")]
        public async Task<alarmOtherAccessPagedResultVM> GetAllAlarmOtherAccess([FromQuery] alarmOtherAccessFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<alarmOtherAccessPagedResultVM>(
                await _reportService.GetAllAlarmOtherAccess(
                    filter.campusId,
                    filter.personId,
                    filter.persontype,
                    filter.logMessage,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("alarmfetcherlist")]
        public async Task<alarmFetcherPagedResultVM> GetAllAlarmFetcher([FromQuery] alarmFetcherFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<alarmFetcherPagedResultVM>(
                await _reportService.GetAllAlarmFetcher(
                    filter.campusId,
                    filter.personId,
                    filter.persontype,
                    filter.logMessage,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        #endregion

        #region Alarm Excel

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportalarmallexcel")]
        public async Task<IActionResult> ExportAlarmAllExcel([FromQuery] alarmAllFilter filter)
        {
            try
            {
                var result = _mapper.Map<alarmAllPagedResultVM>(
                    await _reportService.ExportAlarmAll(
                        filter.campusId,
                        filter.personId,
                        filter.persontype,
                        filter.logMessage,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.AlarmAllColHeader;
                    string wsTitle = ExcelVar.ATitle;
                    int rowHeader = 9;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:H1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:H3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:H4"].Merge = true;
                    worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A5:H5"].Merge = true;
                    worksheet.Cells[5, 1].Value = wsTitle;
                    worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A7:D7"].Merge = true;
                    worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["E7:H7"].Merge = true;
                    worksheet.Cells[7, 5].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[7, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 10;
                    foreach (AlarmAllVM row in result.dailylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Employee_Subtype;
                        worksheet.Cells[rowNumber, 4].Value = row.Educ_Level;
                        worksheet.Cells[rowNumber, 5].Value = row.Year_Level;
                        worksheet.Cells[rowNumber, 6].Value = row.Section;
                        worksheet.Cells[rowNumber, 7].Value = row.Log_Message;
                        worksheet.Cells[rowNumber, 8].Value = row.Date;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
        [Route("exportalarmstudentexcel")]
        public async Task<IActionResult> ExportAlarmStudentExcel([FromQuery] alarmStudentFilter filter)
        {
            try
            {
                var result = _mapper.Map<alarmStudentPagedResultVM>(
                    await _reportService.ExportAlarmStudent(
                        filter.campusId,
                        filter.educLevelId,
                        filter.yearSecId,
                        filter.studSecId,
                        filter.personId,
                        filter.persontype,
                        filter.logMessage,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.AlarmStudentColHeader;
                    string wsTitle = ExcelVar.ATitle;
                    int rowHeader = 9;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:G1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:G3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:G4"].Merge = true;
                    worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A5:G5"].Merge = true;
                    worksheet.Cells[5, 1].Value = wsTitle;
                    worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A7:C7"].Merge = true;
                    worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["D7:G7"].Merge = true;
                    worksheet.Cells[7, 4].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[7, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 10;
                    foreach (AlarmStudentVM row in result.dailylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Educ_Level;
                        worksheet.Cells[rowNumber, 4].Value = row.Year_Level;
                        worksheet.Cells[rowNumber, 5].Value = row.Section;
                        worksheet.Cells[rowNumber, 6].Value = row.Log_Message;
                        worksheet.Cells[rowNumber, 7].Value = row.Date;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
        [Route("exportalarmemployeeexcel")]
        public async Task<IActionResult> ExportAlarmEmployeeExcel([FromQuery] alarmEmployeesFilter filter)
        {
            try
            {
                var result = _mapper.Map<alarmEmployeePagedResultVM>(
                    await _reportService.ExportAlarmEmployee(
                        filter.campusId,
                        filter.personId,
                        filter.persontype,
                        filter.logMessage,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.AlarmEmployeeColHeader;
                    string wsTitle = ExcelVar.ATitle;
                    int rowHeader = 9;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:G1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:G3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:G4"].Merge = true;
                    worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A5:G5"].Merge = true;
                    worksheet.Cells[5, 1].Value = wsTitle;
                    worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A7:C7"].Merge = true;
                    worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["D7:G7"].Merge = true;
                    worksheet.Cells[7, 4].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[7, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 10;
                    foreach (AlarmEmployeeVM row in result.dailylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Employee_Subtype;
                        worksheet.Cells[rowNumber, 4].Value = row.Department_Name;
                        worksheet.Cells[rowNumber, 5].Value = row.Position_Name;
                        worksheet.Cells[rowNumber, 6].Value = row.Log_Message;
                        worksheet.Cells[rowNumber, 7].Value = row.Date;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
        [Route("exportalarmotheraccessexcel")]
        public async Task<IActionResult> ExportAlarmOtherAccessExcel([FromQuery] alarmOtherAccessFilter filter)
        {
            try
            {
                var result = _mapper.Map<alarmOtherAccessPagedResultVM>(
                    await _reportService.ExportAlarmOtherAccess(
                        filter.campusId,
                        filter.personId,
                        filter.persontype,
                        filter.logMessage,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.AlarmOtherAccessColHeader;
                    string wsTitle = ExcelVar.ATitle;
                    int rowHeader = 9;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:F1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:F3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:F4"].Merge = true;
                    worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A5:F5"].Merge = true;
                    worksheet.Cells[5, 1].Value = wsTitle;
                    worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A7:C7"].Merge = true;
                    worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["D7:F7"].Merge = true;
                    worksheet.Cells[7, 4].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[7, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 10;
                    foreach (AlarmOtherAccessVM row in result.dailylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Department_Name;
                        worksheet.Cells[rowNumber, 4].Value = row.Position_Name;
                        worksheet.Cells[rowNumber, 5].Value = row.Log_Message;
                        worksheet.Cells[rowNumber, 6].Value = row.Date;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
        [Route("exportalarmfetcherexcel")]
        public async Task<IActionResult> ExportAlarmFetcherExcel([FromQuery] alarmFetcherFilter filter)
        {
            try
            {
                var result = _mapper.Map<alarmFetcherPagedResultVM>(
                    await _reportService.ExportAlarmFetcher(
                        filter.campusId,
                        filter.personId,
                        filter.persontype,
                        filter.logMessage,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.AlarmFetcherColHeader;
                    string wsTitle = ExcelVar.ATitle;
                    int rowHeader = 9;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:E1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:E3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:E4"].Merge = true;
                    worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A5:E5"].Merge = true;
                    worksheet.Cells[5, 1].Value = wsTitle;
                    worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A7:B7"].Merge = true;
                    worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["C7:E7"].Merge = true;
                    worksheet.Cells[7, 3].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[7, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 10;
                    foreach (AlarmFetcherVM row in result.dailylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Fetcher_Relationship;
                        worksheet.Cells[rowNumber, 4].Value = row.Log_Message;
                        worksheet.Cells[rowNumber, 5].Value = row.Date;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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

        #endregion

        #region Alarm PDF

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportalarmallpdf")]
        public async Task<IActionResult> ExportAlarmAllPDF([FromQuery] alarmAllFilter filter)
        {
            try
            {
                var result = _mapper.Map<alarmAllPagedResultVM>(
                    await _reportService.ExportAlarmAll(
                        filter.campusId,
                        filter.personId,
                        filter.persontype,
                        filter.logMessage,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date Generated : " + DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Alarm Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(8);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Employee Sub-Type", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Department / Educational Level", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Position / Course / Year / Grade", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Student Section", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Message", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Date and Time", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (AlarmAllVM row in result.dailylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Employee_Subtype, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Educ_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Year_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Section, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Log_Message, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportalarmstudentpdf")]
        public async Task<IActionResult> ExportAlarmStudentPDF([FromQuery] alarmStudentFilter filter)
        {
            try
            {
                var result = _mapper.Map<alarmStudentPagedResultVM>(
                    await _reportService.ExportAlarmStudent(
                        filter.campusId,
                        filter.educLevelId,
                        filter.yearSecId,
                        filter.studSecId,
                        filter.personId,
                        filter.persontype,
                        filter.logMessage,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date Generated : " + DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Alarm Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(7);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Educational Level", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Course / Year / Grade", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Section", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Message", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Date and Time", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (AlarmStudentVM row in result.dailylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Educ_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Year_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Section, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Log_Message, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportalarmemployeepdf")]
        public async Task<IActionResult> ExportAlarmEmployeePDF([FromQuery] alarmEmployeesFilter filter)
        {
            try
            {
                var result = _mapper.Map<alarmEmployeePagedResultVM>(
                    await _reportService.ExportAlarmEmployee(
                        filter.campusId,
                        filter.personId,
                        filter.persontype,
                        filter.logMessage,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date Generated : " + DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Alarm Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(7);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Employee Sub-Type", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Department", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Position", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Message", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Date and Time", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (AlarmEmployeeVM row in result.dailylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Employee_Subtype, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Department_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Position_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Log_Message, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportalarmotheraccesspdf")]
        public async Task<IActionResult> ExportAlarmOtherAccessPDF([FromQuery] alarmOtherAccessFilter filter)
        {
            try
            {
                var result = _mapper.Map<alarmOtherAccessPagedResultVM>(
                    await _reportService.ExportAlarmOtherAccess(
                        filter.campusId,
                        filter.personId,
                        filter.persontype,
                        filter.logMessage,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date Generated : " + DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Alarm Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(6);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Department", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Position", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Message", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Date and Time", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (AlarmOtherAccessVM row in result.dailylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Department_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Position_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Log_Message, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportalarmfetcherpdf")]
        public async Task<IActionResult> ExportAlarmFetcherPDF([FromQuery] alarmFetcherFilter filter)
        {
            try
            {
                var result = _mapper.Map<alarmFetcherPagedResultVM>(
                    await _reportService.ExportAlarmFetcher(
                        filter.campusId,
                        filter.personId,
                        filter.persontype,
                        filter.logMessage,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date Generated : " + DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Alarm Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(5);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Fetcher Relationship", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Message", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Date and Time", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (AlarmFetcherVM row in result.dailylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Fetcher_Relationship, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Log_Message, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Visitor

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("visitorlist")]
        public async Task<visitorReportPagedResultVM> GetVisitor([FromQuery] int personId, DateTime from, DateTime to, int pageNo, int pageSize)
        {
            return _mapper.Map<visitorReportPagedResultVM>(await _reportService.GetAllVisitorReport(personId, from, to, pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportvisitorexcel")]
        public async Task<IActionResult> ExportVisitorExcel([FromQuery] int personId, DateTime from, DateTime to)
        {
            try
            {
                var result = _mapper.Map<visitorReportPagedResultVM>(await _reportService.ExportAllVisitorReport(personId, from, to));

                if (result.visitorinformations.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.VisitorReportColHeader;
                    string wsTitle = ExcelVar.VisitorReportTitle;
                    int rowHeader = 8;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:I1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:I3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:I4"].Merge = true;
                    worksheet.Cells[4, 1].Value = wsTitle;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A6:E6"].Merge = true;
                    worksheet.Cells[6, 1].Value = "Date From: " + from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["F6:I6"].Merge = true;
                    worksheet.Cells[6, 6].Value = "Date To: " + to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[6, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 9;
                    foreach (VisitorInformationVM row in result.visitorinformations)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Visitor_Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Remarks;
                        worksheet.Cells[rowNumber, 4].Value = row.Visited_Employee;
                        worksheet.Cells[rowNumber, 5].Value = row.Area_Name;
                        worksheet.Cells[rowNumber, 6].Value = row.Scheduled_Date;
                        worksheet.Cells[rowNumber, 7].Value = row.Date_Time_Registered;
                        worksheet.Cells[rowNumber, 8].Value = row.LogDateTime;
                        worksheet.Cells[rowNumber, 9].Value = row.Date_Time_Surrendered;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
        [Route("exportvisitorpdf")]
        public async Task<IActionResult> ExportVisitorPDF([FromQuery] int personId, DateTime from, DateTime to)
        {
            try
            {
                var result = _mapper.Map<visitorReportPagedResultVM>(await _reportService.ExportAllVisitorReport(personId, from, to));

                if (result.visitorinformations.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph("Visitor Report", FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(9);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Visitor Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Remarks", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Visited Employee", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Area", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Scheduled Date", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date and Time Registered", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Date and Time", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date and Time Surrendered", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (VisitorInformationVM row in result.visitorinformations)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Visitor_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Remarks, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Visited_Employee, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Area_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Scheduled_Date, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date_Time_Registered, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.LogDateTime, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date_Time_Surrendered, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Library Attendance List

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("libraryattendancealllist")]
        public async Task<libraryAttendanceAllPagedResultVM> GetAllLibraryAttendanceAll([FromQuery] libraryAttendanceAllFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<libraryAttendanceAllPagedResultVM>(
                await _reportService.GetAllLibraryAttendanceAll(
                    filter.campusId,
                    filter.personId,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("libraryattendancestudentlist")]
        public async Task<libraryAttendanceStudentPagedResultVM> GetAllLibraryAttendanceStudent([FromQuery] libraryAttendanceStudentFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<libraryAttendanceStudentPagedResultVM>(
                await _reportService.GetAllLibraryAttendanceStudent(
                    filter.campusId,
                    filter.educLevelId,
                    filter.yearSecId,
                    filter.studSecId,
                    filter.personId,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("libraryattendanceemployeelist")]
        public async Task<libraryAttendanceEmployeePagedResultVM> GetAllLibraryAttendanceEmployee([FromQuery] libraryAttendanceEmployeesFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<libraryAttendanceEmployeePagedResultVM>(
                await _reportService.GetAllLibraryAttendanceEmployee(
                    filter.campusId,
                    filter.departmentId,
                    filter.positionId,
                    filter.personId,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        #endregion

        #region Library Attendance Excel

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportlibraryattendanceallexcel")]
        public async Task<IActionResult> ExportLibraryAttendanceAllExcel([FromQuery] libraryAttendanceAllFilter filter)
        {
            try
            {
                var result = _mapper.Map<libraryAttendanceAllPagedResultVM>(
                    await _reportService.ExportLibraryAttendanceAll(
                        filter.campusId,
                        filter.personId,
                        filter.from,
                        filter.to));

                if (result.librarylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.LibraryAttendanceAllColHeader;
                    string wsTitle = ExcelVar.LATitle;
                    int rowHeader = 9;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:G1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:G3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:G4"].Merge = true;
                    worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A5:G5"].Merge = true;
                    worksheet.Cells[5, 1].Value = wsTitle;
                    worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A7:D7"].Merge = true;
                    worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["E7:G7"].Merge = true;
                    worksheet.Cells[7, 5].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[7, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 10;
                    foreach (LibraryAttendanceAllVM row in result.librarylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Educ_Level;
                        worksheet.Cells[rowNumber, 4].Value = row.Year_Level;
                        worksheet.Cells[rowNumber, 5].Value = row.Section;
                        worksheet.Cells[rowNumber, 6].Value = row.Date;
                        worksheet.Cells[rowNumber, 7].Value = row.Log_Message;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
        [Route("exportlibraryattendancestudentexcel")]
        public async Task<IActionResult> ExportLibraryAttendanceStudentExcel([FromQuery] libraryAttendanceStudentFilter filter)
        {
            try
            {
                var result = _mapper.Map<libraryAttendanceStudentPagedResultVM>(
                    await _reportService.ExportLibraryAttendanceStudent(
                        filter.campusId,
                        filter.educLevelId,
                        filter.yearSecId,
                        filter.studSecId,
                        filter.personId,
                        filter.from,
                        filter.to));

                if (result.librarylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.LibraryAttendanceStudentColHeader;
                    string wsTitle = ExcelVar.LATitle;
                    int rowHeader = 9;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:G1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:G3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:G4"].Merge = true;
                    worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A5:G5"].Merge = true;
                    worksheet.Cells[5, 1].Value = wsTitle;
                    worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A7:D7"].Merge = true;
                    worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["E7:G7"].Merge = true;
                    worksheet.Cells[7, 5].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[7, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 10;
                    foreach (LibraryAttendanceStudentVM row in result.librarylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Educ_Level;
                        worksheet.Cells[rowNumber, 4].Value = row.Year_Level;
                        worksheet.Cells[rowNumber, 5].Value = row.Section;
                        worksheet.Cells[rowNumber, 6].Value = row.Date;
                        worksheet.Cells[rowNumber, 7].Value = row.Log_Message;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
        [Route("exportlibraryattendanceemployeeexcel")]
        public async Task<IActionResult> ExportLibraryAttendanceEmployeeExcel([FromQuery] libraryAttendanceEmployeesFilter filter)
        {
            try
            {
                var result = _mapper.Map<libraryAttendanceEmployeePagedResultVM>(
                    await _reportService.ExportLibraryAttendanceEmployee(
                        filter.campusId,
                        filter.departmentId,
                        filter.positionId,
                        filter.personId,
                        filter.from,
                        filter.to));

                if (result.librarylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.LibraryAttendanceEmployeeColHeader;
                    string wsTitle = ExcelVar.LATitle;
                    int rowHeader = 9;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:F1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:F3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:F4"].Merge = true;
                    worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A5:F5"].Merge = true;
                    worksheet.Cells[5, 1].Value = wsTitle;
                    worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A7:C7"].Merge = true;
                    worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["D7:F7"].Merge = true;
                    worksheet.Cells[7, 4].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[7, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 10;
                    foreach (LibraryAttendanceEmployeeVM row in result.librarylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Department_Name;
                        worksheet.Cells[rowNumber, 4].Value = row.Position_Name;
                        worksheet.Cells[rowNumber, 5].Value = row.Date;
                        worksheet.Cells[rowNumber, 6].Value = row.Log_Message;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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

        #endregion

        #region Library Attendance PDF

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportlibraryattendanceallpdf")]
        public async Task<IActionResult> ExportLibraryAttendanceAllPDF([FromQuery] libraryAttendanceAllFilter filter)
        {
            try
            {
                var result = _mapper.Map<libraryAttendanceAllPagedResultVM>(
                    await _reportService.ExportLibraryAttendanceAll(
                        filter.campusId,
                        filter.personId,
                        filter.from,
                        filter.to));

                if (result.librarylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Library Attendance Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(7);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Department / Educational Level", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Position / Course / Year / Grade Level", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Section", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Date and Time", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Message", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (LibraryAttendanceAllVM row in result.librarylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Educ_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Year_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Section, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Log_Message, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportlibraryattendancestudentpdf")]
        public async Task<IActionResult> ExportLibraryAttendanceStudentPDF([FromQuery] libraryAttendanceStudentFilter filter)
        {
            try
            {
                var result = _mapper.Map<libraryAttendanceStudentPagedResultVM>(
                    await _reportService.ExportLibraryAttendanceStudent(
                        filter.campusId,
                        filter.educLevelId,
                        filter.yearSecId,
                        filter.studSecId,
                        filter.personId,
                        filter.from,
                        filter.to));

                if (result.librarylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Library Attendance Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(7);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Educational Level", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Course / Year / Grade Level", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Section", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Date and Time", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Message", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (LibraryAttendanceStudentVM row in result.librarylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Educ_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Year_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Section, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Log_Message, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportlibraryattendanceemployeepdf")]
        public async Task<IActionResult> ExportLibraryAttendanceEmployeePDF([FromQuery] libraryAttendanceEmployeesFilter filter)
        {
            try
            {
                var result = _mapper.Map<libraryAttendanceEmployeePagedResultVM>(
                    await _reportService.ExportLibraryAttendanceEmployee(
                        filter.campusId,
                        filter.departmentId,
                        filter.positionId,
                        filter.personId,
                        filter.from,
                        filter.to));

                if (result.librarylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Library Attendance Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(6);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Department", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Position", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Date and Time", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Log Message", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (LibraryAttendanceEmployeeVM row in result.librarylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Department_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Position_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Log_Message, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Library Usage List

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("libraryusagealllist")]
        public async Task<libraryUsageAllPagedResultVM> GetAllLibraryUsageAll([FromQuery] libraryUsageAllFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<libraryUsageAllPagedResultVM>(
                await _reportService.GetAllLibraryUsageAll(
                    filter.campusId,
                    filter.persontype,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("libraryusagestudentlist")]
        public async Task<libraryUsageStudentPagedResultVM> GetAllLibraryUsageStudent([FromQuery] libraryUsageStudentFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<libraryUsageStudentPagedResultVM>(
                await _reportService.GetAllLibraryUsageStudent(
                    filter.campusId,
                    filter.persontype,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("libraryusageemployeelist")]
        public async Task<libraryUsageEmployeePagedResultVM> GetAllLibraryUsageEmployee([FromQuery] libraryUsageEmployeesFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<libraryUsageEmployeePagedResultVM>(
                await _reportService.GetAllLibraryUsageEmployee(
                    filter.campusId,
                    filter.persontype,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        #endregion

        #region Library Usage Excel

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportlibraryusageallexcel")]
        public async Task<IActionResult> ExportLibraryUsageAllExcel([FromQuery] libraryUsageAllFilter filter)
        {
            try
            {
                var result = _mapper.Map<libraryUsageAllPagedResultVM>(
                    await _reportService.ExportLibraryUsageAll(
                        filter.campusId,
                        filter.persontype,
                        filter.from,
                        filter.to));

                if (result.librarylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                int totalUsageEmployee = 0;
                int totalUsageStudent = 0;
                int totalUsageAll = 0;
                foreach (LibraryUsageAllVM row in result.librarylogs)
                {
                    if (row.Person_Type.Equals("E"))
                        totalUsageEmployee += Convert.ToInt32(row.Usage_Count);
                    else
                        totalUsageStudent += Convert.ToInt32(row.Usage_Count);

                    totalUsageAll += Convert.ToInt32(row.Usage_Count);
                }

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.LibraryUsageAllColHeader;
                    string wsTitle = ExcelVar.LUTitle;
                    int rowHeader = 13;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:G1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:G3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:G4"].Merge = true;
                    worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A5:G5"].Merge = true;
                    worksheet.Cells[5, 1].Value = wsTitle;
                    worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A7:D7"].Merge = true;
                    worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["E7:G7"].Merge = true;
                    worksheet.Cells[7, 5].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[7, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A9:G9"].Merge = true;
                    worksheet.Cells[9, 1].Value = "USAGE COUNT - Employee : " + totalUsageEmployee.ToString();
                    worksheet.Cells[9, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    worksheet.Cells["A10:G10"].Merge = true;
                    worksheet.Cells[10, 1].Value = "USAGE COUNT - Student : " + totalUsageStudent.ToString();
                    worksheet.Cells[10, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    worksheet.Cells["A11:G11"].Merge = true;
                    worksheet.Cells[11, 1].Value = "OVERALL COUNT : " + totalUsageAll.ToString();
                    worksheet.Cells[11, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 14;
                    foreach (LibraryUsageAllVM row in result.librarylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Educ_Level;
                        worksheet.Cells[rowNumber, 4].Value = row.Year_Level;
                        worksheet.Cells[rowNumber, 5].Value = row.Section;
                        worksheet.Cells[rowNumber, 6].Value = row.Date;
                        worksheet.Cells[rowNumber, 7].Value = row.Usage_Count;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
        [Route("exportlibraryusagestudentexcel")]
        public async Task<IActionResult> ExportLibraryUsageStudentExcel([FromQuery] libraryUsageStudentFilter filter)
        {
            try
            {
                var result = _mapper.Map<libraryUsageStudentPagedResultVM>(
                    await _reportService.ExportLibraryUsageStudent(
                        filter.campusId,
                        filter.persontype,
                        filter.from,
                        filter.to));

                if (result.librarylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                int totalUsageEmployee = 0;
                int totalUsageStudent = 0;
                int totalUsageAll = 0;
                foreach (LibraryUsageStudentVM row in result.librarylogs)
                {
                    if (row.Person_Type.Equals("E"))
                        totalUsageEmployee += Convert.ToInt32(row.Usage_Count);
                    else
                        totalUsageStudent += Convert.ToInt32(row.Usage_Count);

                    totalUsageAll += Convert.ToInt32(row.Usage_Count);
                }

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.LibraryUsageStudentColHeader;
                    string wsTitle = ExcelVar.LUTitle;
                    int rowHeader = 13;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:G1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:G3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:G4"].Merge = true;
                    worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A5:G5"].Merge = true;
                    worksheet.Cells[5, 1].Value = wsTitle;
                    worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A7:D7"].Merge = true;
                    worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["E7:G7"].Merge = true;
                    worksheet.Cells[7, 5].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[7, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A9:G9"].Merge = true;
                    worksheet.Cells[9, 1].Value = "USAGE COUNT - Employee : " + totalUsageEmployee.ToString();
                    worksheet.Cells[9, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    worksheet.Cells["A10:G10"].Merge = true;
                    worksheet.Cells[10, 1].Value = "USAGE COUNT - Student : " + totalUsageStudent.ToString();
                    worksheet.Cells[10, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    worksheet.Cells["A11:G11"].Merge = true;
                    worksheet.Cells[11, 1].Value = "OVERALL COUNT : " + totalUsageAll.ToString();
                    worksheet.Cells[11, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 14;
                    foreach (LibraryUsageStudentVM row in result.librarylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Educ_Level;
                        worksheet.Cells[rowNumber, 4].Value = row.Year_Level;
                        worksheet.Cells[rowNumber, 5].Value = row.Section;
                        worksheet.Cells[rowNumber, 6].Value = row.Date;
                        worksheet.Cells[rowNumber, 7].Value = row.Usage_Count;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
        [Route("exportlibraryusageemployeeexcel")]
        public async Task<IActionResult> ExportLibraryUsageEmployeeExcel([FromQuery] libraryUsageEmployeesFilter filter)
        {
            try
            {
                var result = _mapper.Map<libraryUsageEmployeePagedResultVM>(
                    await _reportService.ExportLibraryUsageEmployee(
                        filter.campusId,
                        filter.persontype,
                        filter.from,
                        filter.to));

                if (result.librarylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                int totalUsageEmployee = 0;
                int totalUsageStudent = 0;
                int totalUsageAll = 0;
                foreach (LibraryUsageEmployeeVM row in result.librarylogs)
                {
                    if (row.Person_Type.Equals("E"))
                        totalUsageEmployee += Convert.ToInt32(row.Usage_Count);
                    else
                        totalUsageStudent += Convert.ToInt32(row.Usage_Count);

                    totalUsageAll += Convert.ToInt32(row.Usage_Count);
                }

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.LibraryUsageEmployeeColHeader;
                    string wsTitle = ExcelVar.LUTitle;
                    int rowHeader = 13;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:F1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:F3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:F4"].Merge = true;
                    worksheet.Cells[4, 1].Value = campusName == null ? "" : campusName.Campus_Name;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A5:F5"].Merge = true;
                    worksheet.Cells[5, 1].Value = wsTitle;
                    worksheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A7:C7"].Merge = true;
                    worksheet.Cells[7, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["D7:F7"].Merge = true;
                    worksheet.Cells[7, 4].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[7, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A9:F9"].Merge = true;
                    worksheet.Cells[9, 1].Value = "USAGE COUNT - Employee : " + totalUsageEmployee.ToString();
                    worksheet.Cells[9, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    worksheet.Cells["A10:F10"].Merge = true;
                    worksheet.Cells[10, 1].Value = "USAGE COUNT - Student : " + totalUsageStudent.ToString();
                    worksheet.Cells[10, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    worksheet.Cells["A11:F11"].Merge = true;
                    worksheet.Cells[11, 1].Value = "OVERALL COUNT : " + totalUsageAll.ToString();
                    worksheet.Cells[11, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 14;
                    foreach (LibraryUsageEmployeeVM row in result.librarylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Department_Name;
                        worksheet.Cells[rowNumber, 4].Value = row.Position_Name;
                        worksheet.Cells[rowNumber, 5].Value = row.Date;
                        worksheet.Cells[rowNumber, 6].Value = row.Usage_Count;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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

        #endregion

        #region Library Usage PDF

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportlibraryusageallpdf")]
        public async Task<IActionResult> ExportLibraryUsageAllPDF([FromQuery] libraryUsageAllFilter filter)
        {
            try
            {
                var result = _mapper.Map<libraryUsageAllPagedResultVM>(
                    await _reportService.ExportLibraryUsageAll(
                        filter.campusId,
                        filter.persontype,
                        filter.from,
                        filter.to));

                if (result.librarylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                int totalUsageEmployee = 0;
                int totalUsageStudent = 0;
                int totalUsageAll = 0;
                foreach (LibraryUsageAllVM row in result.librarylogs)
                {
                    if (row.Person_Type.Equals("E"))
                        totalUsageEmployee += Convert.ToInt32(row.Usage_Count);
                    else
                        totalUsageStudent += Convert.ToInt32(row.Usage_Count);

                    totalUsageAll += Convert.ToInt32(row.Usage_Count);
                }

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Library Usage Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table for usage count
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("USAGE COUNT - Employee : " + totalUsageEmployee, FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("USAGE COUNT - Student : " + totalUsageStudent, FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("OVERALL COUNT : " + totalUsageAll, FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 10;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(7);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Department / Educational Level", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Position / Course / Year / Grade Level", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Section", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Usage Count", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (LibraryUsageAllVM row in result.librarylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Educ_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Year_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Section, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Usage_Count, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportlibraryusagestudentpdf")]
        public async Task<IActionResult> ExportLibraryUsageStudentPDF([FromQuery] libraryUsageStudentFilter filter)
        {
            try
            {
                var result = _mapper.Map<libraryUsageStudentPagedResultVM>(
                    await _reportService.ExportLibraryUsageStudent(
                        filter.campusId,
                        filter.persontype,
                        filter.from,
                        filter.to));

                if (result.librarylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                int totalUsageEmployee = 0;
                int totalUsageStudent = 0;
                int totalUsageAll = 0;
                foreach (LibraryUsageStudentVM row in result.librarylogs)
                {
                    if (row.Person_Type.Equals("E"))
                        totalUsageEmployee += Convert.ToInt32(row.Usage_Count);
                    else
                        totalUsageStudent += Convert.ToInt32(row.Usage_Count);

                    totalUsageAll += Convert.ToInt32(row.Usage_Count);
                }

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Library Usage Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table for usage count
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("USAGE COUNT - Employee : " + totalUsageEmployee, FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("USAGE COUNT - Student : " + totalUsageStudent, FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("OVERALL COUNT : " + totalUsageAll, FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 10;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(7);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Educational Level", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Course / Year / Grade Level", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Section", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Usage Count", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (LibraryUsageStudentVM row in result.librarylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Educ_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Year_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Section, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Usage_Count, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportlibraryusageemployeepdf")]
        public async Task<IActionResult> ExportLibraryUsageEmployeePDF([FromQuery] libraryUsageEmployeesFilter filter)
        {
            try
            {
                var result = _mapper.Map<libraryUsageEmployeePagedResultVM>(
                    await _reportService.ExportLibraryUsageEmployee(
                        filter.campusId,
                        filter.persontype,
                        filter.from,
                        filter.to));

                if (result.librarylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                int totalUsageEmployee = 0;
                int totalUsageStudent = 0;
                int totalUsageAll = 0;
                foreach (LibraryUsageEmployeeVM row in result.librarylogs)
                {
                    if (row.Person_Type.Equals("E"))
                        totalUsageEmployee += Convert.ToInt32(row.Usage_Count);
                    else
                        totalUsageStudent += Convert.ToInt32(row.Usage_Count);

                    totalUsageAll += Convert.ToInt32(row.Usage_Count);
                }

                var campusName = await _reportService.GetReportCampusByID(filter.campusId);

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph(campusName == null ? "" : campusName.Campus_Name, FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("Library Usage Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table for usage count
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("USAGE COUNT - Employee : " + totalUsageEmployee, FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("USAGE COUNT - Student : " + totalUsageStudent, FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("OVERALL COUNT : " + totalUsageAll, FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 10;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(6);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Department", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Position", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Usage Count", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (LibraryUsageEmployeeVM row in result.librarylogs)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Department_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Position_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Usage_Count, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Card List

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("cardalllist")]
        public async Task<cardAllPagedResultVM> GetAllCardAll([FromQuery] cardAllFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<cardAllPagedResultVM>(
                await _reportService.GetAllCardAll(
                    filter.campusId,
                    filter.personId,
                    filter.statustype,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("cardstudentlist")]
        public async Task<cardStudentPagedResultVM> GetAllCardStudent([FromQuery] cardStudentFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<cardStudentPagedResultVM>(
                await _reportService.GetAllCardStudent(
                    filter.campusId,
                    filter.educLevelId,
                    filter.yearSecId,
                    filter.studSecId,
                    filter.personId,
                    filter.statustype,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("cardemployeelist")]
        public async Task<cardEmployeePagedResultVM> GetAllCardEmployee([FromQuery] cardEmployeesFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<cardEmployeePagedResultVM>(
                await _reportService.GetAllCardEmployee(
                    filter.campusId,
                    filter.departmentId,
                    filter.positionId,
                    filter.personId,
                    filter.statustype,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("cardotheraccesslist")]
        public async Task<cardOtherAccessPagedResultVM> GetAllCardOtherAccess([FromQuery] cardOtherAccessFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<cardOtherAccessPagedResultVM>(
                await _reportService.GetAllCardOtherAccess(
                    filter.campusId,
                    filter.departmentId,
                    filter.positionId,
                    filter.personId,
                    filter.statustype,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("cardfetcherlist")]
        public async Task<cardFetcherPagedResultVM> GetAllCardFetcher([FromQuery] cardFetcherFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<cardFetcherPagedResultVM>(
                await _reportService.GetAllCardFetcher(
                    filter.campusId,
                    filter.personId,
                    filter.statustype,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        #endregion

        #region Card Excel

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportcardallexcel")]
        public async Task<IActionResult> ExportCardAllExcel([FromQuery] cardAllFilter filter)
        {
            try
            {
                var result = _mapper.Map<cardAllPagedResultVM>(
                    await _reportService.ExportCardAll(
                        filter.campusId,
                        filter.personId,
                        filter.statustype,
                        filter.from,
                        filter.to));

                if (result.carddetails.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.CardAllColHeader;
                    string wsTitle = ExcelVar.CTitle;
                    int rowHeader = 8;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:F1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:F3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:F4"].Merge = true;
                    worksheet.Cells[4, 1].Value = wsTitle;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A6:C6"].Merge = true;
                    worksheet.Cells[6, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["D6:F6"].Merge = true;
                    worksheet.Cells[6, 4].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[6, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 9;
                    foreach (CardAllVM row in result.carddetails)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Person_Type;
                        worksheet.Cells[rowNumber, 4].Value = row.Educ_Level;
                        worksheet.Cells[rowNumber, 5].Value = row.Year_Level;
                        worksheet.Cells[rowNumber, 6].Value = row.Date_Updated;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
        [Route("exportcardstudentexcel")]
        public async Task<IActionResult> ExportCardStudentExcel([FromQuery] cardStudentFilter filter)
        {
            try
            {
                var result = _mapper.Map<cardStudentPagedResultVM>(
                    await _reportService.ExportCardStudent(
                        filter.campusId,
                        filter.educLevelId,
                        filter.yearSecId,
                        filter.studSecId,
                        filter.personId,
                        filter.statustype,
                        filter.from,
                        filter.to));

                if (result.carddetails.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.CardStudentColHeader;
                    string wsTitle = ExcelVar.CTitle;
                    int rowHeader = 8;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:E1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:E3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:E4"].Merge = true;
                    worksheet.Cells[4, 1].Value = wsTitle;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A6:B6"].Merge = true;
                    worksheet.Cells[6, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["C6:E6"].Merge = true;
                    worksheet.Cells[6, 3].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[6, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 9;
                    foreach (CardStudentVM row in result.carddetails)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Educ_Level;
                        worksheet.Cells[rowNumber, 4].Value = row.Year_Level;
                        worksheet.Cells[rowNumber, 5].Value = row.Date_Updated;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
        [Route("exportcardemployeeexcel")]
        public async Task<IActionResult> ExportCardEmployeeExcel([FromQuery] cardEmployeesFilter filter)
        {
            try
            {
                var result = _mapper.Map<cardEmployeePagedResultVM>(
                    await _reportService.ExportCardEmployee(
                        filter.campusId,
                        filter.departmentId,
                        filter.positionId,
                        filter.personId,
                        filter.statustype,
                        filter.from,
                        filter.to));

                if (result.carddetails.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.CardEmployeeColHeader;
                    string wsTitle = ExcelVar.CTitle;
                    int rowHeader = 8;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:E1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:E3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:E4"].Merge = true;
                    worksheet.Cells[4, 1].Value = wsTitle;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A6:B6"].Merge = true;
                    worksheet.Cells[6, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["C6:E6"].Merge = true;
                    worksheet.Cells[6, 3].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[6, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 9;
                    foreach (CardEmployeeVM row in result.carddetails)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Department_Name;
                        worksheet.Cells[rowNumber, 4].Value = row.Position_Name;
                        worksheet.Cells[rowNumber, 5].Value = row.Date_Updated;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
        [Route("exportcardotheraccessexcel")]
        public async Task<IActionResult> ExportCardOtherAccessExcel([FromQuery] cardOtherAccessFilter filter)
        {
            try
            {
                var result = _mapper.Map<cardOtherAccessPagedResultVM>(
                    await _reportService.ExportCardOtherAccess(
                        filter.campusId,
                        filter.departmentId,
                        filter.positionId,
                        filter.personId,
                        filter.statustype,
                        filter.from,
                        filter.to));

                if (result.carddetails.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.CardOtherAccessColHeader;
                    string wsTitle = ExcelVar.CTitle;
                    int rowHeader = 8;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:E1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:E3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:E4"].Merge = true;
                    worksheet.Cells[4, 1].Value = wsTitle;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A6:B6"].Merge = true;
                    worksheet.Cells[6, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    worksheet.Cells["C6:E6"].Merge = true;
                    worksheet.Cells[6, 3].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[6, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 9;
                    foreach (CardOtherAccessVM row in result.carddetails)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Department_Name;
                        worksheet.Cells[rowNumber, 4].Value = row.Position_Name;
                        worksheet.Cells[rowNumber, 5].Value = row.Date_Updated;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
        [Route("exportcardfetcherexcel")]
        public async Task<IActionResult> ExportCardFetcherExcel([FromQuery] cardFetcherFilter filter)
        {
            try
            {
                var result = _mapper.Map<cardFetcherPagedResultVM>(
                    await _reportService.ExportCardFetcher(
                        filter.campusId,
                        filter.personId,
                        filter.statustype,
                        filter.from,
                        filter.to));

                if (result.carddetails.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.CardFetcherColHeader;
                    string wsTitle = ExcelVar.CTitle;
                    int rowHeader = 8;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:C1"].Merge = true;
                    worksheet.Cells[1, 1].Value = "Date Generated : " + DateTime.Now.ToLongDateString();
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    worksheet.Cells["A3:C3"].Merge = true;
                    //worksheet.Cells[3, 1].Value = "My Campus Card Timekeeping";
                    worksheet.Cells[3, 1].Value = "";
                    worksheet.Cells[3, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A4:C4"].Merge = true;
                    worksheet.Cells[4, 1].Value = wsTitle;
                    worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A6:B6"].Merge = true;
                    worksheet.Cells[6, 1].Value = "Date From: " + filter.from.ToString("MMMM dd, yyyy");

                    //worksheet.Cells["B6:C6"].Merge = true;
                    worksheet.Cells[6, 3].Value = "Date To: " + filter.to.ToString("MMMM dd, yyyy");
                    worksheet.Cells[6, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 9;
                    foreach (CardFetcherVM row in result.carddetails)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Date_Updated;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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

        #endregion

        #region Card PDF

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportcardallpdf")]
        public async Task<IActionResult> ExportCardAllPDF([FromQuery] cardAllFilter filter)
        {
            try
            {
                var result = _mapper.Map<cardAllPagedResultVM>(
                    await _reportService.ExportCardAll(
                        filter.campusId,
                        filter.personId,
                        filter.statustype,
                        filter.from,
                        filter.to));

                if (result.carddetails.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph("Card Report", FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(6);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Person Type", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Department / Educational Level", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Position / Course / Year / Grade Level", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date and Time Updated", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (CardAllVM row in result.carddetails)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Person_Type, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Educ_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Year_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date_Updated, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportcardstudentpdf")]
        public async Task<IActionResult> ExportCardStudentPDF([FromQuery] cardStudentFilter filter)
        {
            try
            {
                var result = _mapper.Map<cardStudentPagedResultVM>(
                    await _reportService.ExportCardStudent(
                        filter.campusId,
                        filter.educLevelId,
                        filter.yearSecId,
                        filter.studSecId,
                        filter.personId,
                        filter.statustype,
                        filter.from,
                        filter.to));

                if (result.carddetails.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph("Card Report", FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(5);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Educational Level", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Course / Year / Grade Level", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date and Time Updated", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (CardStudentVM row in result.carddetails)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Educ_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Year_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date_Updated, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportcardemployeepdf")]
        public async Task<IActionResult> ExportCardEmployeePDF([FromQuery] cardEmployeesFilter filter)
        {
            try
            {
                var result = _mapper.Map<cardEmployeePagedResultVM>(
                    await _reportService.ExportCardEmployee(
                        filter.campusId,
                        filter.departmentId,
                        filter.positionId,
                        filter.personId,
                        filter.statustype,
                        filter.from,
                        filter.to));

                if (result.carddetails.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph("Card Report", FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(5);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Department", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Position", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date and Time Updated", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (CardEmployeeVM row in result.carddetails)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Department_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Position_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date_Updated, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportcardotheraccesspdf")]
        public async Task<IActionResult> ExportCardOtherAccessPDF([FromQuery] cardOtherAccessFilter filter)
        {
            try
            {
                var result = _mapper.Map<cardOtherAccessPagedResultVM>(
                    await _reportService.ExportCardOtherAccess(
                        filter.campusId,
                        filter.departmentId,
                        filter.positionId,
                        filter.personId,
                        filter.statustype,
                        filter.from,
                        filter.to));

                if (result.carddetails.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph("Card Report", FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(5);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Department", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Position", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date and Time Updated", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (CardOtherAccessVM row in result.carddetails)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Department_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Position_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date_Updated, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportcardfetcherpdf")]
        public async Task<IActionResult> ExportCardFetcherPDF([FromQuery] cardFetcherFilter filter)
        {
            try
            {
                var result = _mapper.Map<cardFetcherPagedResultVM>(
                    await _reportService.ExportCardFetcher(
                        filter.campusId,
                        filter.personId,
                        filter.statustype,
                        filter.from,
                        filter.to));

                if (result.carddetails.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //Paragraph p2 = new Paragraph("My Campus Card Timekeeping", FontTahoma11Bold);
                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p3 = new Paragraph("Card Report", FontTahoma11Bold);
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p3));

                Paragraph p4 = new Paragraph("", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("Date From: " + filter.from.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("Date To: " + filter.to.ToString("MMMM dd, yyyy"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(3);

                cell = new PdfPCell(new Phrase("ID Number", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Full Name", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date and Time Updated", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (CardFetcherVM row in result.carddetails)
                {
                    table.AddCell(new Phrase(row.ID_Number, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Date_Updated, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Audit Trail

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("audittraillist")]
        public async Task<auditTrailPagedResultVM> GetAuditTrail([FromQuery] AuditTrailPaginationParams audittrail)
        {
            try
            {
                return _mapper.Map<auditTrailPagedResultVM>(
                    await _reportService.GetAllAuditTrail(
                        audittrail.user_ID,
                        audittrail.status,
                        audittrail.from,
                        audittrail.to,
                        audittrail.PageNo,
                        audittrail.PageSize));
            }
            catch (Exception err)
            {
                _logger.Error(err.Message);
                throw err;
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpGet]
        [Route("exportaudittrailexcel")]
        public async Task<IActionResult> ExportAuditListExcel([FromQuery] AuditTrailPaginationParams filter)
        {
            try
            {
                var result = _mapper.Map<auditTrailPagedResultVM>(
                    await _reportService.ExportAuditTrail(
                        filter.user_ID,
                        filter.status,
                        filter.from,
                        filter.to));

                if (result.audittrails.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.AuditTrailColHeader;
                    string wsTitle = ExcelVar.AuditTrailTitle;
                    int rowHeader = 5;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells["A1:F1"].Merge = true;
                    worksheet.Cells[1, 1].Value = wsTitle;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A3:C3"].Merge = true;
                    worksheet.Cells[3, 1].Value = "From: " + filter.from.ToString("yyyy-MM-dd");

                    worksheet.Cells["D3:F3"].Merge = true;
                    worksheet.Cells[3, 4].Value = "To: " + filter.to.ToString("yyyy-MM-dd");
                    worksheet.Cells[3, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];

                        worksheet.Cells[rowHeader, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowHeader, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[rowHeader, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 6;
                    foreach (AuditTrailReportVM row in result.audittrails)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.User_Name;
                        worksheet.Cells[rowNumber, 2].Value = row.Source;
                        worksheet.Cells[rowNumber, 3].Value = row.Category;
                        worksheet.Cells[rowNumber, 4].Value = row.Log_Level;
                        worksheet.Cells[rowNumber, 5].Value = row.Message;
                        worksheet.Cells[rowNumber, 6].Value = row.Log_Date_Time;

                        using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                        {
                            range.Style.Font.Bold = false;
                            range.Style.ShrinkToFit = false;
                            range.Style.Numberformat.Format = "@";

                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
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
        [Route("exportaudittrailpdf")]
        public async Task<IActionResult> ExportAuditListPDF([FromQuery] AuditTrailPaginationParams filter)
        {
            try
            {
                var result = _mapper.Map<auditTrailPagedResultVM>(
                    await _reportService.ExportAuditTrail(
                        filter.user_ID,
                        filter.status,
                        filter.from,
                        filter.to));

                if (result.audittrails.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                Document doc = new Document(PageSize.LETTER, 2, 2, 10, 10);
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
                pdfWriter.CloseStream = false;

                PdfPCell cell;
                var FontTahoma6Bold = FontFactory.GetFont("tahoma", 6, Font.BOLD);
                var FontTahoma6Normal = FontFactory.GetFont("tahoma", 6, Font.NORMAL);
                var FontTahoma7Bold = FontFactory.GetFont("tahoma", 7, Font.BOLD);
                var FontTahoma11Bold = FontFactory.GetFont("tahoma", 11, Font.BOLD);

                doc.Open();

                //header     
                PdfPTable t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                //cell = new PdfPCell(new Phrase(DateTime.Now.ToLongDateString(), FontTahoma7Bold));
                //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                //cell.Border = 0;
                //t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                Paragraph p2 = new Paragraph("", FontTahoma11Bold);
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(new Paragraph(p2));

                Paragraph p4 = new Paragraph("Audit Trail Report", FontTahoma7Bold);
                p4.Alignment = Element.ALIGN_CENTER;
                //Blank Space
                p4.SpacingAfter = 20;
                doc.Add(new Paragraph(p4));

                //table for from and end date
                t1 = new PdfPTable(3);
                t1.DefaultCell.Border = 0;
                t1.HeaderRows = 0;
                cell = new PdfPCell(new Phrase("From: " + filter.from.ToString("yyyy-MM-dd"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                t1.AddCell(cell);
                cell = new PdfPCell(new Phrase("To: " + filter.to.ToString("yyyy-MM-dd"), FontTahoma7Bold));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = 0;
                t1.AddCell(cell);
                t1.SpacingAfter = 5;
                doc.Add(t1);

                //table
                PdfPTable table = new PdfPTable(6);

                cell = new PdfPCell(new Phrase("User", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Source", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Category", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Status", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Message", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date and Time", FontTahoma6Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                table.HeaderRows = 1;

                foreach (AuditTrailReportVM row in result.audittrails)
                {
                    table.AddCell(new Phrase(row.User_Name, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Source, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Category, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Log_Level, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Message, FontTahoma6Normal));
                    table.AddCell(new Phrase(row.Log_Date_Time, FontTahoma6Normal));
                }

                doc.Add(table);
                doc.Close();

                stream.Flush();
                stream.Position = 0;

                return File(stream, "application/pdf", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        /* ------------------------------------------------------------------------------------------------------------- */

        [HttpGet("pdf_timeandattendance_student")]
        public async Task<timeAndAttendanceStudentPagedResultVM> pdfTimeAndAttendanceStudent([FromQuery] timeAttendanceEmployeeFilter filter)
        {
            return _mapper.Map<timeAndAttendanceStudentPagedResultVM>(
               await _reportService.ExportTimeAndAttendanceStudent(
                   (filter.logstat == null || filter.logstat == "undefined" ? "" : filter.logstat.ToLower().Equals("null") ? "" : filter.logstat),
                   (filter.keyword == null ? string.Empty : filter.keyword),
                   filter.terminal_ID,
                   filter.area_ID,
                   filter.campus_ID,
                   filter.from,
                   filter.to));
        }

        [HttpGet("pdf_timeandattendance_visitor")]
        public async Task<timeAndAttendanceVisitorPagedResultVM> pdfTimeAndAttendanceVisitor([FromQuery] timeAttendanceEmployeeFilter filter)
        {
            return _mapper.Map<timeAndAttendanceVisitorPagedResultVM>(
               await _reportService.ExportTimeAndAttendanceVisitor(
                   (filter.logstat == null || filter.logstat == "undefined" ? "" : filter.logstat.ToLower().Equals("null") ? "" : filter.logstat),
                   (filter.keyword == null ? string.Empty : filter.keyword),
                   filter.terminal_ID,
                   filter.area_ID,
                   filter.campus_ID,
                   filter.from,
                   filter.to));
        }

        [HttpGet("export_timeandattendance_visitor")]
        public async Task<IActionResult> exportTimeAndAttendanceVisitor([FromQuery] timeAttendanceEmployeeFilter filter)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Visitor Time And Attendance Report");

                sb.AppendLine("");

                sb.AppendLine("From: " + filter.from.ToString("yyyy-MM-dd") + ",,," + "To: " + filter.to.ToString("yyyy-MM-dd"));

                sb.AppendLine("");

                sb.AppendLine("IDNumber,Name,Status,Log,CampusName,AreaName,TerminalName");

                var result = _mapper.Map<timeAndAttendanceVisitorPagedResultVM>(
                    await _reportService.ExportTimeAndAttendanceVisitor(
                        (filter.logstat == null || filter.logstat == "undefined" ? "" : filter.logstat.ToLower().Equals("null") ? "" : filter.logstat),
                        (filter.keyword == null ? string.Empty : filter.keyword),
                        filter.terminal_ID,
                        filter.area_ID,
                        filter.campus_ID,
                        filter.from,
                        filter.to));

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                foreach (TimeAndAttendanceVisitorVM row in result.dailylogs)
                {
                    sb.AppendLine(row.ID_Number + "," + row.First_Name + " " + row.Last_Name + "," + row.Status + "," + row.Log_Date.ToString() + "," + row.Campus_Name + "," + row.Area_Name + "," + row.Terminal_Name);
                }

                return File(new System.Text.UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("timeattendance_visitor")]
        public async Task<timeAndAttendanceVisitorPagedResultVM> GetVisitor([FromQuery] timeAttendanceEmployeeFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<timeAndAttendanceVisitorPagedResultVM>(
                await _reportService.GetAllTimeAttendanceVisitor(
                    (filter.logstat == null || filter.logstat == "undefined" ? "" : filter.logstat.ToLower().Equals("null") ? "" : filter.logstat),
                    (filter.keyword == null ? "" : filter.keyword),
                    filter.terminal_ID,
                    filter.area_ID,
                    filter.campus_ID,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [HttpGet("alarm_visitor")]
        public async Task<AlarmVisitorPagedResultVM> GetAlarmVisitor([FromQuery] timeAttendanceEmployeeFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<AlarmVisitorPagedResultVM>(await _reportService.GetAllAlarmVisitor(
                    (filter.logstat == null || filter.logstat == "undefined" ? "" : filter.logstat.ToLower().Equals("null") ? "" : filter.logstat),
                    (filter.keyword == null ? "" : filter.keyword),
                    filter.terminal_ID,
                    filter.area_ID,
                    filter.campus_ID,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [HttpGet("pdf_alarm_visitor")]
        public async Task<AlarmVisitorPagedResultVM> ExportPDFAlarmVisitor([FromQuery] timeAttendanceEmployeeFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<AlarmVisitorPagedResultVM>(await _reportService.ExportAllAlarmVisitor(
                (filter.logstat == null || filter.logstat == "undefined" ? "" : filter.logstat.ToLower().Equals("null") ? "" : filter.logstat),
                (filter.keyword == null ? "" : filter.keyword),
                filter.terminal_ID,
                filter.area_ID,
                filter.campus_ID,
                filter.from,
                filter.to));
        }

        [HttpGet("export_alarm_visitor")]
        public async Task<IActionResult> ExportAlarmVisitor([FromQuery] timeAttendanceEmployeeFilter filter)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Visitor Alarm Time And Attendance Report");

            sb.AppendLine("");

            sb.AppendLine("From: " + filter.from.ToString("yyyy-MM-dd") + ",,," + "To: " + filter.to.ToString("yyyy-MM-dd"));

            sb.AppendLine("");

            sb.AppendLine("IDNumber,Name,Status,Log,CampusName,AreaName,TerminalName");

            var result = _mapper.Map<AlarmVisitorPagedResultVM>(await _reportService.ExportAllAlarmVisitor(
                    (filter.logstat == null || filter.logstat == "undefined" ? "" : filter.logstat.ToLower().Equals("null") ? "" : filter.logstat),
                    (filter.keyword == null ? "" : filter.keyword),
                    filter.terminal_ID,
                    filter.area_ID,
                    filter.campus_ID,
                    filter.from,
                    filter.to));

            if (result.dailylogs.Count <= 0)
                return BadRequest(new { message = "No data available." });

            foreach (AlarmVisitorVM row in result.dailylogs)
            {
                sb.AppendLine(row.ID_Number + "," + row.First_Name + " " + row.Last_Name + "," + row.Status + "," + row.Log_Date.ToString() + "," + row.Campus_Name + "," + row.Area_Name + "," + row.Terminal_Name);
            }

            return File(new System.Text.UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "export");
        }

        [HttpGet("audituserlist")]
        public async Task<ICollection<userAuditTrailVM>> GetAuditUserList()
        {
            return _mapper.Map<ICollection<userAuditTrailVM>>(await _reportService.GetAuditTrailUserList());
        }

        [HttpGet("auditformlist")]
        public async Task<ICollection<formAuditTrailVM>> GetAuditFormList()
        {
            return _mapper.Map<ICollection<formAuditTrailVM>>(await _reportService.GetAllForms());
        }

        [HttpGet("card_report")]
        public async Task<cardReportPagedResultVM> GetCardReport([FromQuery] cardReportFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            if (filter.persontype == "STUDENT")
                filter.persontype = "S";
            else if (filter.persontype == "EMPLOYEE")
                filter.persontype = "E";
            else
                filter.persontype = "O";

            return _mapper.Map<cardReportPagedResultVM>(
                await _reportService.GetAllCardReport(
                    filter.persontype,
                    (filter.keyword == null ? string.Empty : filter.keyword),
                    (filter.cardstatus == null ? string.Empty : filter.cardstatus.ToLower().Equals("null") ? "" : filter.cardstatus),
                    filter.from,
                    filter.to,
                    pageNo,
                    pageSize));
        }

        [HttpGet("pdf_card_report")]
        public async Task<cardReportPagedResultVM> ExportPDFCardReport([FromQuery] cardReportFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            if (filter.persontype == "STUDENT")
                filter.persontype = "S";
            else if (filter.persontype == "EMPLOYEE")
                filter.persontype = "E";
            else
                filter.persontype = "O";

            return _mapper.Map<cardReportPagedResultVM>(
                await _reportService.ExportCardReport(
                    filter.persontype,
                    (filter.keyword == null ? string.Empty : filter.keyword),
                    (filter.cardstatus == null ? string.Empty : filter.cardstatus.ToLower().Equals("null") ? "" : filter.cardstatus),
                    filter.from,
                    filter.to));
        }

        [HttpGet("export_card_report")]
        public async Task<IActionResult> exportGetCard([FromQuery] cardReportFilter filter)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Card Report");

                sb.AppendLine("");

                sb.AppendLine("From: " + filter.from.ToString("yyyy-MM-dd") + ",,," + "To: " + filter.to.ToString("yyyy-MM-dd"));

                sb.AppendLine("");

                sb.AppendLine("IDNumber,Name,CardNumber,IssuedDate,ExpiryDate,CardStatus,Remarks");

                if (filter.persontype == "STUDENT")
                    filter.persontype = "S";
                else if (filter.persontype == "EMPLOYEE")
                    filter.persontype = "E";
                else
                    filter.persontype = "O";

                var result = _mapper.Map<cardReportPagedResultVM>(
                    await _reportService.ExportCardReport(
                        filter.persontype,
                        (filter.keyword == null ? string.Empty : filter.keyword),
                        (filter.cardstatus == null ? string.Empty : filter.cardstatus.ToLower().Equals("null") ? "" : filter.cardstatus),
                        filter.from,
                        filter.to));

                if (result.carddetails.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                foreach (CardReportVM row in result.carddetails)
                {
                    sb.AppendLine(row.IDNumber + "," + row.LastName + " " + row.FirstName + "," + row.CardNumber.ToString() + "," + row.IssuedDate.ToString() + "," + row.ExpiryDate + "," + row.CardStatus + "," + row.Remarks);
                }

                return File(new System.Text.UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("card")]
        public async Task<ICollection<CardVM>> GetAllCardReports([FromQuery] cardFilterVM filter)
        {
            return _mapper.Map<ICollection<CardVM>>(await _reportService.GetAllCardReport(filter));
        }

        [HttpGet("export_card_report_excel")]
        public async Task<IActionResult> ExportGetCardExcel([FromQuery] cardReportFilter filter)
        {
            try
            {
                if (filter.persontype == "STUDENT")
                    filter.persontype = "S";
                else if (filter.persontype == "EMPLOYEE")
                    filter.persontype = "E";
                else
                    filter.persontype = "O";

                var result = _mapper.Map<cardReportPagedResultVM>(
                    await _reportService.ExportCardReport(
                        filter.persontype,
                        (filter.keyword == null ? string.Empty : filter.keyword),
                        (filter.cardstatus == null ? string.Empty : filter.cardstatus.ToLower().Equals("null") ? "" : filter.cardstatus),
                        filter.from,
                        filter.to));

                if (result.carddetails.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                byte[] file = null;

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.CardColHeader;
                    string wsTitle = ExcelVar.CardTitle;
                    int rowHeader = 5;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells[1, 1].Value = wsTitle;
                    worksheet.Cells[3, 1].Value = "From: " + filter.from.ToString("yyyy-MM-dd");
                    worksheet.Cells[3, 4].Value = "To: " + filter.to.ToString("yyyy-MM-dd");

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 6;
                    foreach (CardReportVM row in result.carddetails)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.IDNumber;
                        worksheet.Cells[rowNumber, 2].Value = row.LastName + ", " + row.FirstName;
                        worksheet.Cells[rowNumber, 3].Value = row.CardNumber;
                        worksheet.Cells[rowNumber, 4].Value = row.IssuedDate;
                        worksheet.Cells[rowNumber, 5].Value = row.ExpiryDate;
                        worksheet.Cells[rowNumber, 6].Value = row.CardStatus;
                        worksheet.Cells[rowNumber, 7].Value = row.Remarks;

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

        [HttpGet("totalhoursofemployee")]
        public async Task<totalHoursOfEmployeePagedResultVM> GetTotalHours([FromQuery] totalHoursEmployeeFilter filter, [FromQuery] int pageNo, int pageSize)
        {
            return _mapper.Map<totalHoursOfEmployeePagedResultVM>(
                await _reportService.GetTotalHoursOfEmployee(
                    (filter.emptype == null || filter.emptype == "undefined" ? "" : filter.emptype.ToLower().Equals("null") ? "" : filter.emptype == "Teaching" ? "1" : "0"),
                    (filter.keyword == null ? "" : filter.keyword),
                    filter.department_ID,
                    filter.from,
                    filter.to,
                    pageNo, pageSize));
        }

        [HttpGet("export_totalhoursofemployee_excel")]
        public async Task<IActionResult> exporttotalhoursofemployeeexcelfile([FromQuery] totalHoursEmployeeFilter filter)
        {
            try
            {
                var result = _mapper.Map<totalHoursOfEmployeePagedResultVM>(
                    await _reportService.ExportTotalHoursOfEmployee(
                        (filter.emptype == null || filter.emptype == "undefined" ? "" : filter.emptype.ToLower().Equals("null") ? "" : filter.emptype == "Teaching" ? "1" : "0"),
                        (filter.keyword == null ? "" : filter.keyword),
                        filter.department_ID,
                        filter.from,
                        filter.to));
                byte[] file = null;

                if (result.dailylogs.Count <= 0)
                    return BadRequest(new { message = "No data available." });

                using (var package = new ExcelPackage())
                {
                    string[] ColHeader = ExcelVar.TotalHoursColHeader;
                    string wsTitle = ExcelVar.TotalHoursTitle;
                    int rowHeader = 5;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                    worksheet.Cells[1, 1].Value = wsTitle;
                    worksheet.Cells[3, 1].Value = "From: " + filter.from.ToString("yyyy-MM-dd");
                    worksheet.Cells[3, 4].Value = "To: " + filter.to.ToString("yyyy-MM-dd");

                    for (int i = 1; i <= ColHeader.Length; i++)
                    {
                        worksheet.Cells[rowHeader, i].Value = ColHeader[i - 1];
                    }

                    using (var range = worksheet.Cells[1, 1, rowHeader, ColHeader.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    int rowNumber = 6;
                    foreach (TotalHoursOfEmployeeVM row in result.dailylogs)
                    {
                        worksheet.Cells[rowNumber, 1].Value = row.ID_Number;
                        worksheet.Cells[rowNumber, 2].Value = row.Full_Name;
                        worksheet.Cells[rowNumber, 3].Value = row.Department_Name;
                        worksheet.Cells[rowNumber, 4].Value = row.Employee_Type;
                        worksheet.Cells[rowNumber, 5].Value = row.Total_Hours;

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
    }
}
