using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class DepedReportRepository : BaseRepository<schoolCalendarEntity>, IDepedReportRepository
    {
        private readonly MyCampusCardContext context;

        public DepedReportRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<List<ScheduleVM>> GetSchedule(string schoolYear, string month, int sectionId)
        {
            try
            {
                var scheduleList = new List<ScheduleVM>();
                int imonth = DateTime.ParseExact(month, "MMMM", CultureInfo.CurrentCulture).Month;

                var schoolCalendar = await _context.SchoolCalendarEntity
                        .Where(q => q.School_Year == schoolYear && q.Month == imonth)
                        .FirstOrDefaultAsync();

                var section = await _context.StudentSectionEntity
                        .Where(z => z.StudSec_ID == sectionId).FirstOrDefaultAsync();
                
                int gracePeriod = section.Grace_Period == null || section.Grace_Period.Trim() == string.Empty ? 0 : Convert.ToInt32(section.Grace_Period);

                List<string> days = schoolCalendar.Days.Split(',').ToList<string>();
                int maxDays = DateTime.DaysInMonth(schoolCalendar.Year, imonth);
                int loopArray = 0, currentDay = 0;

                var sectionSchedule = await _context.SectionScheduleEntity
                        .Where(z => z.StudSec_ID == sectionId && z.Schedule_Date.Year == schoolCalendar.Year
                        && z.Schedule_Date.Month == schoolCalendar.Month).ToListAsync();
                
                for (int x = 1; x <= maxDays; x++)
                {
                    var schedule = new ScheduleVM();
                    schedule.Date = new DateTime(schoolCalendar.Year, schoolCalendar.Month, x);
                    schedule.NameOfTheDay = schedule.Date.DayOfWeek.ToString() == "Thursday" ? "Th" : schedule.Date.DayOfWeek.ToString().Substring(0, 1);
                    
                    try
                    {
                        currentDay = Convert.ToInt32(days[loopArray]);
                        if (x == currentDay)
                        {
                            schedule.withClasses = true;
                            loopArray++;
                        }
                        else
                        {
                            schedule.withClasses = false;
                        }
                    }
                    catch (Exception)
                    {
                        schedule.withClasses = false;
                    }

                    var custom = sectionSchedule.FirstOrDefault(q => q.Schedule_Date.Date == schedule.Date.Date);

                    if (custom != null)
                    {
                        schedule.withClasses = true;
                        schedule.StartTime = custom.Start_Time;
                        schedule.EndTime = custom.End_Time;
                        schedule.HalfDay = custom.Half_Day;
                        schedule.GracePeriod = custom.Grace_Period == null || custom.Grace_Period.Trim() == string.Empty ? 0 : Convert.ToInt32(custom.Grace_Period);
                        schedule.isExcused = custom.IsExcused;
                    }
                    else
                    {
                        schedule.StartTime = section.Start_Time;
                        schedule.EndTime = section.End_Time;
                        schedule.HalfDay = section.Half_Day;
                        schedule.GracePeriod = gracePeriod;
                        schedule.isExcused = false;
                    }

                    scheduleList.Add(schedule);
                }
                return scheduleList.Where(a => a.withClasses == true).OrderBy(a => a.Date).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<DepedReportHeaderVM> GetHeaders(string schoolYear, string month, int sectionId)
        {
            try
            {
                var section = await _context.StudentSectionEntity
                        .Include(x => x.YearSectionEntity)
                        .ThenInclude(c => c.EducationalLevelEntity)
                        .ThenInclude(v => v.CampusEntity)
                        .ThenInclude(d => d.DivisionEntity)
                        .ThenInclude(r => r.RegionEntity)
                        .Where(z => z.StudSec_ID == sectionId).FirstOrDefaultAsync();
                
                var schoolCalendar = await _context.SchoolYearEntity
                        .Where(q => q.School_Year == schoolYear)
                        .FirstOrDefaultAsync();
                
                var headers = new DepedReportHeaderVM();
                headers.Region = section.YearSectionEntity.EducationalLevelEntity.CampusEntity.DivisionEntity.RegionEntity.Code;
                headers.Division = section.YearSectionEntity.EducationalLevelEntity.CampusEntity.DivisionEntity.Name;
                headers.SchoolID = "406620";
                headers.SchoolYear = schoolYear;
                headers.SchoolName = section.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name;
                headers.Month = month;
                headers.GradeLevel = section.YearSectionEntity.YearSec_Name;
                headers.Section = section.Description;
                headers.Password = section.Password;
                headers.EnrollmentEnd = (DateTime)schoolCalendar.End_Date;
                
                return headers;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<RecordsVM>> GetAttendance(List<ScheduleVM> schedule, string schoolYear, string month, int sectionId)
        {
            try
            {
                var result = new List<RecordsVM>();

                var studentList = await _context.PersonEntity
                    .Where(z => z.Person_Type == "S" && z.StudSec_ID == sectionId)
                    .OrderBy(z => z.Last_Name).ToListAsync();

                if (studentList != null && studentList.Count > 0)
                {
                    var attendanceList = await _context.ReportAttendanceLogsEntity
                        .Where(z => z.Person_Type == "S" && z.StudSec_ID == sectionId)
                        .OrderBy(z => z.PersonEntity.Last_Name).ToListAsync();

                    for (int x = 0; x < studentList.Count; x++)
                    {
                        var record = new RecordsVM();
                        record.Name = studentList[x].Last_Name + ", " + studentList[x].First_Name + " " + studentList[x].Middle_Name;
                        record.Gender = studentList[x].Gender;
                        record.TransferredIn = studentList[x].IsTransferredIn;
                        record.TransferredOut = studentList[x].IsTransferred;
                        record.DropOut = studentList[x].IsDropOut;
                        record.DropOutCode = studentList[x].IsDropOut ? studentList[x].DropOutCode.Substring(0, 4) : "";
                        record.DateEnrolled = studentList[x].DateEnrolled;

                        if (studentList[x].IsTransferredIn && studentList[x].IsTransferred)
                            record.SchoolName = studentList[x].TransferredInSchoolName + "/" + studentList[x].TransferredSchoolName;
                        else if (studentList[x].IsTransferredIn)
                            record.SchoolName = studentList[x].TransferredInSchoolName;
                        else if (studentList[x].IsTransferred)
                            record.SchoolName = studentList[x].TransferredSchoolName;
                        else
                            record.SchoolName = string.Empty;

                        string datesList = string.Empty;
                        string statusList = string.Empty;

                        for (int i = 0; i < schedule.Count; i++)
                        {
                            datesList = datesList + "," + schedule[i].Date.ToShortDateString();

                            var excused = await _context.ExcusedStudentEntity
                                .Where(q => q.IDNumber == studentList[x].ID_Number && q.Excused_Date.Date == schedule[i].Date.Date).FirstOrDefaultAsync();

                            if (excused != null)
                            {
                                statusList = statusList + ",P";
                            }
                            else
                            {
                                var custom = attendanceList.FirstOrDefault(q => q.LogDate.Date == schedule[i].Date.Date && q.Person_ID == studentList[x].Person_ID);

                                if (custom != null && !(custom.LogIn == null || custom.LogIn == string.Empty))
                                {
                                    TimeSpan gracePeriod = TimeSpan.FromMinutes(schedule[i].GracePeriod);
                                    TimeSpan loginTime = TimeSpan.Parse(custom.LogIn);
                                    TimeSpan fromHalfDay = schedule[i].HalfDay.Subtract(TimeSpan.FromMinutes(30));
                                    TimeSpan toHalfDay = schedule[i].HalfDay.Add(TimeSpan.FromMinutes(30));
                                    var newStartTime = schedule[i].StartTime.Add(gracePeriod);

                                    if (loginTime <= newStartTime)
                                    {
                                        statusList = statusList + ",P";
                                    }
                                    else if (loginTime >= fromHalfDay && loginTime <= toHalfDay)
                                    {
                                        statusList = statusList + ",H";
                                    }
                                    else
                                    {
                                        statusList = statusList + ",T";
                                    }
                                }
                                else
                                {
                                    statusList = statusList + ",A";
                                }
                            }
                        }
                        record.Dates = datesList.Remove(0, 1);
                        record.Status = statusList.Remove(0, 1);
                        record.Absent = statusList.Count(s => s == 'A') + (statusList.Count(s => s == 'H') * .5);
                        record.Tardy = statusList.Count(s => s == 'T');
                        result.Add(record);
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
