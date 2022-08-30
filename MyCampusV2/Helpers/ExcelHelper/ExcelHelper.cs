using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Globalization;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using MyCampusV2.Common.ViewModels;
using ClosedXML.Excel;
using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace MyCampusV2.Helpers.ExcelHelper
{
    public class ExcelHelper
    {
        private static int startDatarow = 2;
        private Logger logger;
        
        public byte[] ExportTemplate(string[] ColHeader, string[] ColSampleData, string wsTitle)
        {
            byte[] file = null;
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);
                for (int i = 1; i <= ColHeader.Length; i++)
                {
                    worksheet.Cells[1, i].Value = ColHeader[i - 1].Replace("*", string.Empty);
                    worksheet.Cells[startDatarow, i].Value = ColSampleData[i - 1];

                    if (ColHeader[i - 1].Contains("*"))
                    {
                        worksheet.Cells[1, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.IndianRed);
                    }
                }

                using (var range = worksheet.Cells[1, 1, 1, ColHeader.Length])
                {
                    range.Style.Font.Bold = true;
                    range.Style.ShrinkToFit = false;
                }
                
                using (var range = worksheet.Cells[startDatarow, 1, startDatarow, ColHeader.Length])
                {
                    range.Style.Font.Bold = false;
                    range.Style.ShrinkToFit = false;
                    range.Style.Numberformat.Format = "@";
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

            return file;
        }

        public byte[] ExportCalendarTemplate(string wsTitle, string schoolYear, string startingMonth)
        {
            List<string> list = new List<string>();
            list.Add("Year");
            list.Add("Month");

            for (int i = 1; i <= 31; i++)
                list.Add(i.ToString());

            String[] ColHeader = list.ToArray();
            int startingYear = Convert.ToInt32(schoolYear.Substring(0, 4));
            wsTitle = wsTitle + " (" + schoolYear + ")";

            byte[] file = null;
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                for (int i = 1; i <= ColHeader.Length; i++)
                {
                    worksheet.Cells[1, i].Value = ColHeader[i - 1];

                    if (i > 2)
                    {
                        worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(91, 155, 213));
                    }
                    else
                    {
                        worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                    }
                }
                
                DateTime currDate = new DateTime(startingYear, DateTime.ParseExact(startingMonth, "MMMM", CultureInfo.CurrentCulture).Month, 1);

                for (int d = startDatarow; d < (startDatarow+12); d++)
                {
                    worksheet.Cells[d, 1].Value = currDate.Year.ToString();
                    worksheet.Cells[d, 2].Value = currDate.ToString("MMMM");

                    int maxDays = DateTime.DaysInMonth(currDate.Year, currDate.Month);
                    
                    var dd = worksheet.Cells[d, 3, d, maxDays + 2].DataValidation.AddListDataValidation() as ExcelDataValidationList;
                    dd.AllowBlank = false;
                    dd.Formula.Values.Add("0");
                    dd.Formula.Values.Add("1");
                    
                    worksheet.Cells[d, 3, d, maxDays + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[d, 3, d, maxDays + 2].Style.Font.Color.SetColor(Color.White);

                    for (int x = 1; x <= maxDays; x++)
                    {
                        currDate = new DateTime(currDate.Year, currDate.Month, x);
                        
                        var address = new ExcelAddress(worksheet.Cells[d, (x + 2)].Address);

                        var condition = worksheet.ConditionalFormatting.AddExpression(address);
                        condition.Formula = "IF(" + worksheet.Cells[d, (x + 2)].Address + "=1, TRUE, FALSE)";
                        condition.Style.Fill.BackgroundColor.Color = Color.FromArgb(0, 176, 80);

                        var condition2 = worksheet.ConditionalFormatting.AddExpression(address);
                        condition2.Formula = "IF(" + worksheet.Cells[d, (x + 2)].Address + "=0, TRUE, FALSE)";
                        condition2.Style.Fill.BackgroundColor.Color = Color.FromArgb(192, 0, 0);
                        
                        if ((currDate.DayOfWeek == DayOfWeek.Saturday) || (currDate.DayOfWeek == DayOfWeek.Sunday))
                        {
                            worksheet.Cells[d, (x + 2)].Value = 0;
                        }
                        else
                        {
                            worksheet.Cells[d, (x + 2)].Value = 1;
                        }
                    }
                    currDate = currDate.AddMonths(1);
                }
                
                using (var range = worksheet.Cells[1, 1, 1, ColHeader.Length])
                {
                    range.Style.Font.Bold = true;
                }

                for (int i = 3; i <= ColHeader.Length; i++)
                {
                    worksheet.Column(i).Width = 4;
                }
                
                worksheet.Cells["A1:B13"].AutoFitColumns();
                worksheet.Cells["A1:AG13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                package.Workbook.Properties.Title = wsTitle;
                package.Workbook.Properties.Author = ExcelVar.Author;
                package.Workbook.Properties.Company = ExcelVar.Company;

                file = package.GetAsByteArray();
            }

            return file;
        }

        public byte[] ExportCalendarList(string wsTitle, string schoolYear, List<schoolCalendarDatesVM> calendarVM)
        {
            List<string> list = new List<string>();
            list.Add("Year");
            list.Add("Month");

            for (int i = 1; i <= 31; i++)
                list.Add(i.ToString());

            String[] ColHeader = list.ToArray();
            wsTitle = wsTitle + " (" + schoolYear + ")";

            byte[] file = null;
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                for (int i = 1; i <= ColHeader.Length; i++)
                {
                    worksheet.Cells[1, i].Value = ColHeader[i - 1];

                    if (i > 2)
                    {
                        worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(91, 155, 213));
                    }
                    else
                    {
                        worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                    }
                }
                
                DateTime currDate = calendarVM.Min(a => a.date);

                for (int d = startDatarow; d < (startDatarow + 12); d++)
                {
                    worksheet.Cells[d, 1].Value = currDate.Year.ToString();
                    worksheet.Cells[d, 2].Value = currDate.ToString("MMMM");

                    int maxDays = DateTime.DaysInMonth(currDate.Year, currDate.Month);

                    var dd = worksheet.Cells[d, 3, d, maxDays + 2].DataValidation.AddListDataValidation() as ExcelDataValidationList;
                    dd.AllowBlank = false;
                    dd.Formula.Values.Add("0");
                    dd.Formula.Values.Add("1");

                    worksheet.Cells[d, 3, d, maxDays + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[d, 3, d, maxDays + 2].Style.Font.Color.SetColor(Color.White);

                    for (int x = 1; x <= maxDays; x++)
                    {
                        currDate = new DateTime(currDate.Year, currDate.Month, x);
                        var dbDates = calendarVM.Where(q => q.date.Month == currDate.Month).OrderBy(q => q.date).ToList();
                        
                        var address = new ExcelAddress(worksheet.Cells[d, (x + 2)].Address);

                        var condition = worksheet.ConditionalFormatting.AddExpression(address);
                        condition.Formula = "IF(" + worksheet.Cells[d, (x + 2)].Address + "=1, TRUE, FALSE)";
                        condition.Style.Fill.BackgroundColor.Color = Color.FromArgb(0, 176, 80);

                        var condition2 = worksheet.ConditionalFormatting.AddExpression(address);
                        condition2.Formula = "IF(" + worksheet.Cells[d, (x + 2)].Address + "=0, TRUE, FALSE)";
                        condition2.Style.Fill.BackgroundColor.Color = Color.FromArgb(192, 0, 0);

                        if (dbDates[x-1].date == currDate)
                        {
                            worksheet.Cells[d, (x + 2)].Value = dbDates[x - 1].dateValue;
                        }
                    }
                    currDate = currDate.AddMonths(1);
                }

                using (var range = worksheet.Cells[1, 1, 1, ColHeader.Length])
                {
                    range.Style.Font.Bold = true;
                }

                for (int i = 3; i <= ColHeader.Length; i++)
                {
                    worksheet.Column(i).Width = 4;
                }

                worksheet.Cells["A1:B13"].AutoFitColumns();
                worksheet.Cells["A1:AG13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                package.Workbook.Properties.Title = wsTitle;
                package.Workbook.Properties.Author = ExcelVar.Author;
                package.Workbook.Properties.Company = ExcelVar.Company;

                file = package.GetAsByteArray();
            }

            return file;
        }

        public byte[] ExportDepedReport(List<ScheduleVM> schedule, DepedReportHeaderVM header, List<RecordsVM> attendance)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\Templates\Deped_Report.xlsx";

                var femaleAttendance = attendance.Where(a => a.Gender == "F").OrderBy(a => a.Name).ToList();
                var maleAttendance = attendance.Where(a => a.Gender == "M").OrderBy(a => a.Name).ToList();

                int maxDays = DateTime.DaysInMonth(header.EnrollmentEnd.Year, header.EnrollmentEnd.Month);
                DateTime lastDayOfMonth = new DateTime(header.EnrollmentEnd.Year, header.EnrollmentEnd.Month, maxDays);

                int maleLateEnrollees = maleAttendance.Where(a => a.DateEnrolled > header.EnrollmentEnd && a.DateEnrolled <= lastDayOfMonth).ToList().Count();
                int femaleLateEnrollees = femaleAttendance.Where(a => a.DateEnrolled > header.EnrollmentEnd && a.DateEnrolled <= lastDayOfMonth).ToList().Count();
                int maleEnrollees = maleAttendance.Where(a => a.DateEnrolled <= lastDayOfMonth).ToList().Count();
                int femaleEnrollees = femaleAttendance.Where(a => a.DateEnrolled <= lastDayOfMonth).ToList().Count();
                int maleConsecutiveAbsent = 0;
                int femaleConsecutiveAbsent = 0;

                XLWorkbook workbook = new XLWorkbook(path);
                IXLWorksheet worksheet = workbook.Worksheet(1);

                worksheet.Cell(4, 4).Value = header.Region;
                worksheet.Cell(4, 20).Value = header.Division;
                worksheet.Cell(6, 3).Value = header.SchoolID;
                worksheet.Cell(6, 15).Value = header.SchoolYear;
                worksheet.Cell(6, 28).Value = header.Month;
                worksheet.Cell(8, 3).Value = header.SchoolName;
                worksheet.Cell(8, 28).Value = header.GradeLevel;
                worksheet.Cell(8, 35).Value = header.Section;

                int maleStartingRow = 14;
                int femaleStartingRow = 16 + maleAttendance.Count;

                for (int i = 1; i <= schedule.Count; i++)
                {
                    worksheet.Cell(11, i + 3).Value = 1;
                    worksheet.Cell(12, i + 3).Value = schedule[i - 1].Date.ToString("MM/dd");
                    worksheet.Cell(13, i + 3).Value = schedule[i - 1].NameOfTheDay;
                }

                var maleStartRow = worksheet.Row(maleStartingRow);

                if (maleAttendance.Count > 0)
                    maleStartRow.InsertRowsBelow(maleAttendance.Count);

                var femaleStartRow = worksheet.Row(femaleStartingRow);

                if (femaleAttendance.Count > 0)
                    femaleStartRow.InsertRowsBelow(femaleAttendance.Count);

                for (int i = 1; i <= maleAttendance.Count; i++)
                {
                    worksheet.Cell(maleStartingRow, 1).Value = i;
                    worksheet.Cell(maleStartingRow, 2).Value = maleAttendance[i - 1].Name;
                    worksheet.Cell(maleStartingRow, 35).Value = maleAttendance[i - 1].Absent;
                    worksheet.Cell(maleStartingRow, 36).Value = maleAttendance[i - 1].Tardy;
                    worksheet.Cell(maleStartingRow, 37).Value = maleAttendance[i - 1].TransferredIn ? "/" : "";
                    worksheet.Cell(maleStartingRow, 38).Value = maleAttendance[i - 1].TransferredOut ? "/" : "";
                    worksheet.Cell(maleStartingRow, 39).Value = maleAttendance[i - 1].DropOut ? "/" : "";
                    worksheet.Cell(maleStartingRow, 40).Value = maleAttendance[i - 1].DropOutCode;
                    worksheet.Cell(maleStartingRow, 41).Value = maleAttendance[i - 1].SchoolName;

                    List<string> status = maleAttendance[i - 1].Status.Split(',').ToList<string>();

                    int counter = 0;
                    bool checker = false;

                    for (int a = 0; a < status.Count; a++)
                    {
                        worksheet.Cell(maleStartingRow, a + 4).Value = status[a];

                        if (status[a] == "A")
                        {
                            counter++;

                            if (!checker && counter >= 5)
                            {
                                maleConsecutiveAbsent++;
                                checker = true;
                            }
                        }
                        else
                        {
                            counter = 0;
                        }
                    }

                    //Merge Cells
                    worksheet.Range("AO:AP").Row(maleStartingRow).Merge();

                    //// Formula for Total Absent and Tardy
                    //var cellMaleAbsent = worksheet.Cell(maleStartingRow, 35);
                    //cellMaleAbsent.FormulaA1 = "=COUNTIF(D" + maleStartingRow + ":AH" + maleStartingRow + ", \"a\") + (1 / 2 * COUNTIF(D" + maleStartingRow + ":AH" + maleStartingRow + ", \"h\"))";
                    //worksheet.Cell(maleStartingRow, 35).Value = cellMaleAbsent.Value;

                    //var cellMaleTardy = worksheet.Cell(maleStartingRow, 36);
                    //cellMaleTardy.FormulaA1 = "=COUNTIF(D" + maleStartingRow + ":AH" + maleStartingRow + ", \"t\")";
                    //worksheet.Cell(maleStartingRow, 36).Value = cellMaleTardy.Value;

                    maleStartingRow++;
                }

                for (int i = 1; i <= femaleAttendance.Count; i++)
                {
                    worksheet.Cell(femaleStartingRow, 1).Value = i;
                    worksheet.Cell(femaleStartingRow, 2).Value = femaleAttendance[i - 1].Name;
                    worksheet.Cell(femaleStartingRow, 35).Value = femaleAttendance[i - 1].Absent;
                    worksheet.Cell(femaleStartingRow, 36).Value = femaleAttendance[i - 1].Tardy;
                    worksheet.Cell(femaleStartingRow, 37).Value = femaleAttendance[i - 1].TransferredIn ? "/" : "";
                    worksheet.Cell(femaleStartingRow, 38).Value = femaleAttendance[i - 1].TransferredOut ? "/" : "";
                    worksheet.Cell(femaleStartingRow, 39).Value = femaleAttendance[i - 1].DropOut ? "/" : "";
                    worksheet.Cell(femaleStartingRow, 40).Value = femaleAttendance[i - 1].DropOutCode;
                    worksheet.Cell(femaleStartingRow, 41).Value = femaleAttendance[i - 1].SchoolName;

                    List<string> status = femaleAttendance[i - 1].Status.Split(',').ToList<string>();

                    int counter = 0;
                    bool checker = false;

                    for (int a = 0; a < status.Count; a++)
                    {
                        worksheet.Cell(femaleStartingRow, a + 4).Value = status[a];

                        if (status[a] == "A")
                        {
                            counter++;

                            if (!checker && counter >= 5)
                            {
                                femaleConsecutiveAbsent++;
                                checker = true;
                            }
                        }
                        else
                        {
                            counter = 0;
                        }
                    }

                    //Merge Cells
                    worksheet.Range("AO:AP").Row(femaleStartingRow).Merge();

                    //// Formula for Total Absent and Tardy
                    //var cellFemaleAbsent = worksheet.Cell(femaleStartingRow, 35);
                    //cellFemaleAbsent.FormulaA1 = "=COUNTIF(D" + femaleStartingRow + ":AH" + femaleStartingRow + ", \"a\") + (1 / 2 * COUNTIF(D" + femaleStartingRow + ":AH" + femaleStartingRow + ", \"h\"))";
                    //worksheet.Cell(femaleStartingRow, 35).Value = cellFemaleAbsent.Value;

                    //var cellFemaleTardy = worksheet.Cell(femaleStartingRow, 36);
                    //cellFemaleTardy.FormulaA1 = "=COUNTIF(D" + femaleStartingRow + ":AH" + femaleStartingRow + ", \"t\")";
                    //worksheet.Cell(femaleStartingRow, 36).Value = cellFemaleTardy.Value;

                    femaleStartingRow++;
                }

                // Formula for Total Per Day
                maleStartingRow = 14;
                femaleStartingRow = 16 + maleAttendance.Count;

                int cellNumberMale = maleStartingRow + (maleAttendance.Count > 1 ? (maleAttendance.Count - 1) : 0);
                int cellNumberFemale = femaleStartingRow + (femaleAttendance.Count > 1 ? (femaleAttendance.Count - 1) : 0);
                int totalMaleNumber = cellNumberMale + (maleAttendance.Count > 0 ? 2 : 1);
                int totalFemaleNumber = cellNumberFemale + (femaleAttendance.Count > 0 ? 2 : 1);

                for (int i = 1; i <= schedule.Count; i++)
                {
                    string letter = GetColumnName(i + 2);

                    string startCell = letter + maleStartingRow;
                    string endCell = letter + cellNumberMale;

                    var cellMaleTotal = worksheet.Cell(totalMaleNumber, i + 3);
                    cellMaleTotal.FormulaA1 = "=COUNTIF(" + startCell + ":" + endCell + ", \"p\") + (1 / 2 * COUNTIF(" + startCell + ":" + endCell + ", \"h\")) + COUNTIF(" + startCell + ":" + endCell + ", \"t\")";
                    worksheet.Cell(totalMaleNumber, i + 3).Value = cellMaleTotal.Value;

                    startCell = letter + femaleStartingRow;
                    endCell = letter + cellNumberFemale;

                    var cellFemaleTotal = worksheet.Cell(totalFemaleNumber, i + 3);
                    cellFemaleTotal.FormulaA1 = "=COUNTIF(" + startCell + ":" + endCell + ", \"p\") + (1 / 2 * COUNTIF(" + startCell + ":" + endCell + ", \"h\")) + COUNTIF(" + startCell + ":" + endCell + ", \"t\")";
                    worksheet.Cell(totalFemaleNumber, i + 3).Value = cellFemaleTotal.Value;

                }

                // Formula for Total Transferred In, Transferred Out and Dropped Out
                for (int i = 37; i <= 39; i++)
                {
                    string letter = GetColumnName(i - 1);

                    string startCell = letter + maleStartingRow;
                    string endCell = letter + cellNumberMale;

                    var cellMaleTotal = worksheet.Cell(totalMaleNumber, i);
                    cellMaleTotal.FormulaA1 = "=COUNTIF(" + startCell + ":" + endCell + ", \"/\")";
                    worksheet.Cell(totalMaleNumber, i).Value = cellMaleTotal.Value;

                    startCell = letter + femaleStartingRow;
                    endCell = letter + cellNumberFemale;

                    var cellFemaleTotal = worksheet.Cell(totalFemaleNumber, i);
                    cellFemaleTotal.FormulaA1 = "=COUNTIF(" + startCell + ":" + endCell + ", \"/\")";
                    worksheet.Cell(totalFemaleNumber, i).Value = cellFemaleTotal.Value;
                }

                // Other Formula
                worksheet.Cell("AN" + totalMaleNumber).Value = "";
                worksheet.Cell("AO" + totalMaleNumber).Value = "";
                worksheet.Cell("AN" + totalFemaleNumber).Value = "";
                worksheet.Cell("AO" + totalFemaleNumber).Value = "";


                // Footer
                worksheet.Cell("AI11").Value = schedule.Count();

                worksheet.Cell(totalFemaleNumber + 5, 40).Value = maleAttendance.Count;
                worksheet.Cell(totalFemaleNumber + 5, 41).Value = femaleAttendance.Count;

                worksheet.Cell(totalFemaleNumber + 7, 40).Value = maleLateEnrollees;
                worksheet.Cell(totalFemaleNumber + 7, 41).Value = femaleLateEnrollees;

                worksheet.Cell(totalFemaleNumber + 9, 40).Value = maleEnrollees;
                worksheet.Cell(totalFemaleNumber + 9, 41).Value = femaleEnrollees;

                worksheet.Cell(totalFemaleNumber + 17, 40).Value = maleConsecutiveAbsent;
                worksheet.Cell(totalFemaleNumber + 17, 41).Value = femaleConsecutiveAbsent;

                worksheet.Column(42).Width = 15;

                //worksheet.PageSetup.PrintAreas.Add("A1:AR" + (maleAttendance.Count + femaleAttendance.Count + 52));
                //worksheet.PageSetup.AddHorizontalPageBreak(maleAttendance.Count + femaleAttendance.Count + 52);
                //worksheet.PageSetup.AddVerticalPageBreak(44);

                workbook.ReferenceStyle = XLReferenceStyle.A1;
                workbook.CalculateMode = XLCalculateMode.Auto;

                workbook.Properties.Title = ExcelVar.DepedReportTitle;
                workbook.Properties.Author = ExcelVar.Author;
                workbook.Properties.Company = ExcelVar.Company;

                byte[] file = null;
                MemoryStream stream = new MemoryStream();

                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms, false, false);
                    file = ms.ToArray();
                    ms.CopyTo(stream);
                }

                MemoryStream outputMemStream = new MemoryStream();
                ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

                zipStream.SetLevel(3);
                zipStream.Password = header.Password;

                var newEntry = new ZipEntry("SY " + header.SchoolYear + "_" + header.Month + "_DEPED_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
                newEntry.DateTime = DateTime.Now;
                zipStream.PutNextEntry(newEntry);

                MemoryStream inStream = new MemoryStream(file);
                StreamUtils.Copy(inStream, zipStream, new byte[4096]);
                inStream.Close();
                zipStream.CloseEntry();

                zipStream.IsStreamOwner = false;
                zipStream.Close();

                outputMemStream.Position = 0;
                byte[] zipfile = outputMemStream.ToArray();

                return zipfile;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return null;
            }
        }

        static string GetColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var value = "";

            if (index >= letters.Length)
                value += letters[index / letters.Length - 1];

            value += letters[index % letters.Length];

            return value;
        }
    }
}
