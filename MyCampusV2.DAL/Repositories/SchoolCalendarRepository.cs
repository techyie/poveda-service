using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class SchoolCalendarRepository : BaseRepository<schoolCalendarEntity>, ISchoolCalendarRepository
    {
        private readonly MyCampusCardContext context;

        public SchoolCalendarRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<schoolCalendarResult> GetCalendarDates(string schoolyear)
        {
            try
            {
                var schoolCalendar = new List<schoolCalendarEntity>();
                var result = new List<schoolCalendarVM>();
                var calendarVM = new schoolCalendarResult();

                schoolCalendar = await _context.SchoolCalendarEntity
                        .Where(q => q.School_Year == schoolyear)
                        .ToListAsync();
                
                for (int i = 0; i < schoolCalendar.Count; i++)
                {
                    List<string> days = schoolCalendar[i].Days.Split(',').ToList<string>();
                    
                    for (int a = 0; a < days.Count; a++)
                    {
                        string formatDay = days[a].PadLeft(2, '0');
                        string formatMonth = schoolCalendar[i].Month.ToString().PadLeft(2, '0');

                        schoolCalendarVM calendar = new schoolCalendarVM();
                        calendar.date = schoolCalendar[i].Year.ToString() + '-' + formatMonth + '-' + formatDay;
                        calendar.className = "blue";
                        result.Add(calendar);
                    }
                }

                calendarVM.schoolcalendar = result;
                return calendarVM;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<schoolCalendarDatesVM>> GetCalendarList(string schoolyear)
        {
            try
            {
                var schoolCalendar = new List<schoolCalendarEntity>();
                var result = new List<schoolCalendarDatesVM>();

                schoolCalendar = await _context.SchoolCalendarEntity
                        .Where(q => q.School_Year == schoolyear)
                        .ToListAsync();

                for (int i = 0; i < schoolCalendar.Count; i++)
                {
                    int maxDays = DateTime.DaysInMonth(schoolCalendar[i].Year, schoolCalendar[i].Month);
                    List<string> days = schoolCalendar[i].Days.Split(',').ToList<string>();

                    int currentDay = 0;
                    int loopArray = 0;
                    for (int x = 1; x <= maxDays; x++)
                    {
                        schoolCalendarDatesVM calendar = new schoolCalendarDatesVM();
                        calendar.date = new DateTime(schoolCalendar[i].Year, schoolCalendar[i].Month, x);

                       try
                       {
                            currentDay = Convert.ToInt32(days[loopArray]);
                            if (x == currentDay)
                            {
                                calendar.dateValue = 1;
                                loopArray++;
                            }
                            else
                            {
                                calendar.dateValue = 0;
                            }
                        }
                        catch (Exception e)
                        {
                            calendar.dateValue = 0;
                        }
                        result.Add(calendar);
                    }
                }
                
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public async Task<Boolean> AddWithBoolReturn(schoolCalendarEntity calendar, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    calendar.Added_By = user;
                    calendar.Date_Time_Added = DateTime.Now;
                    calendar.Last_Updated = DateTime.Now;
                    calendar.Updated_By = user;
                    calendar.IsActive = true;
                    calendar.ToDisplay = true;

                    await _context.SchoolCalendarEntity.AddAsync(calendar);

                    await _context.SaveChangesAsync();

                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public async Task<Boolean> UpdateWithBoolReturn(schoolCalendarEntity calendar, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.SchoolCalendarEntity.SingleOrDefault(x => x.School_Calendar_ID == calendar.School_Calendar_ID);

                    if (result != null)
                    {
                        result.School_Year = calendar.School_Year;
                        result.Year = calendar.Year;
                        result.Month = calendar.Month;
                        result.Days = calendar.Days;

                        result.IsActive = true;
                        result.Updated_By = user;
                        result.Last_Updated = DateTime.Now;
                        result.ToDisplay = true;
                    }
                    _context.Entry(result).Property(x => x.Date_Time_Added).IsModified = false;
                    _context.Entry(result).Property(x => x.Added_By).IsModified = false;
                    _context.Entry(result).Property(x => x.Last_Updated).IsModified = true;
                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public async Task<List<schoolCalendarEntity>> GetBySchoolYear(string calendar)
        {
            try
            {
                return await _context.SchoolCalendarEntity
                        .Where(z => z.School_Year == calendar).ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
