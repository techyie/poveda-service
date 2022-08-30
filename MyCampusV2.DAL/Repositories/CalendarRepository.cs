using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.Common.Helpers;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.Helpers;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class CalendarRepository : BaseRepository<calendarEntity>, ICalendarRepository
    {
        private readonly MyCampusCardContext context;
        private ResultModel resultModel;
        private Response response;
        private string RepositoryName = "Calendar";

        public CalendarRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<ResultModel> AddCalendar(calendarEntity calendarEntity, int userId)
        {
            response = new Response();

            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    calendarEntity.IsActive = true;
                    calendarEntity.ToDisplay = true;
                    calendarEntity.Added_By = userId;
                    calendarEntity.Updated_By = userId;
                    calendarEntity.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                    calendarEntity.Last_Updated = DateTime.UtcNow.ToLocalTime();

                    calendarEntity.Calendar_Code = System.Guid.NewGuid().ToString(); 
                    calendarEntity.IsSent = false;

                    var isAll = calendarEntity.Recipients.Where(q => q.key == "All").FirstOrDefault();

                    if (isAll != null)
                    {
                        calendarEntity.IsAll = true;

                        calendarEntity.Year_Level = "All";

                        await _context.CalendarEntity.AddAsync(calendarEntity);
                    }
                    else
                    {
                        calendarEntity.Year_Level = string.Empty;

                        foreach (var recipient in calendarEntity.Recipients)
                        {
                            calendarEntity.Year_Level += ", " + recipient.key;
                        }

                        if (calendarEntity.Year_Level.Substring(0, 1).IndexOf(',') >= 0)
                        {
                            calendarEntity.Year_Level = calendarEntity.Year_Level.Substring(1, calendarEntity.Year_Level.Length - 1);
                            calendarEntity.Year_Level = calendarEntity.Year_Level.Trim();
                        }

                        await _context.CalendarEntity.AddAsync(calendarEntity);

                        foreach (var recipient in calendarEntity.Recipients)
                        {
                            calendarRecipientsEntity calendarRecipientsEntity = new calendarRecipientsEntity();

                            yearSectionEntity yearSecInfo = await _context.YearSectionEntity.Where(x => x.YearSec_Name == recipient.key && x.IsActive == true).FirstOrDefaultAsync();

                            calendarRecipientsEntity.Calendar_ID = calendarEntity.ID;
                            calendarRecipientsEntity.Year_Level_ID = yearSecInfo.YearSec_ID;

                            await _context.CalendarRecipientsEntity.AddAsync(calendarRecipientsEntity);
                        }
                    }

                    await _context.SaveChangesAsync();

                    contextTransaction.Commit();

                    return response.CreateResponse("200", Constants.SuccessCalendarAdd, true);
                }
                catch (Exception err)
                {
                    contextTransaction.Rollback();
                    return response.CreateResponse("500", Constants.FailedCalendarAdd, false);
                }
            }
        }

        public async Task<ResultModel> UpdateCalendar(calendarEntity calendarEntity, int userId)
        {
            response = new Response();

            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var calendarData = _context.CalendarEntity.SingleOrDefault(x => x.Calendar_Code == calendarEntity.Calendar_Code);

                    if (calendarData != null)
                    {
                        calendarData.Title = calendarEntity.Title;
                        calendarData.Body = calendarEntity.Body;
                        calendarData.Post_Date = calendarEntity.Post_Date;
                        //calendarData.Post_Date = DateTime.UtcNow.ToLocalTime();

                        calendarData.Updated_By = userId;
                        calendarData.Last_Updated = DateTime.UtcNow.ToLocalTime();

                        var isAll = calendarEntity.Recipients.Where(q => q.key == "All").FirstOrDefault();

                        context.Entry(calendarData).Property(x => x.Date_Time_Added).IsModified = false;
                        _context.Entry(calendarData).Property(x => x.Added_By).IsModified = false;
                        _context.Entry(calendarData).Property(x => x.Last_Updated).IsModified = true;

                        ICollection<calendarRecipientsEntity> calendarRecipients = await _context.CalendarRecipientsEntity.Include(q => q.CalendarEntity).Where(q => q.CalendarEntity.Calendar_Code == calendarEntity.Calendar_Code).ToListAsync();

                        if (calendarRecipients != null)
                        {
                            _context.CalendarRecipientsEntity.RemoveRange(calendarRecipients);
                        }

                        if (isAll != null)
                        {
                            calendarData.IsAll = true;

                            calendarData.Year_Level = "All";
                        }
                        else
                        {
                            calendarData.Year_Level = string.Empty;
                            calendarData.IsAll = false;

                            foreach (var recipient in calendarEntity.Recipients)
                            {
                                calendarData.Year_Level += ", " + recipient.key;
                            }

                            if (calendarData.Year_Level.Substring(0, 1).IndexOf(',') >= 0)
                            {
                                calendarData.Year_Level = calendarData.Year_Level.Substring(1, calendarData.Year_Level.Length - 1);
                                calendarData.Year_Level = calendarData.Year_Level.Trim();
                            }

                            foreach (var recipient in calendarEntity.Recipients)
                            {
                                calendarRecipientsEntity newCalendarRecipients = new calendarRecipientsEntity();

                                yearSectionEntity yearSecInfo = await _context.YearSectionEntity.Where(x => x.YearSec_Name == recipient.key && x.IsActive == true).FirstOrDefaultAsync();

                                newCalendarRecipients.Calendar_ID = calendarData.ID;
                                newCalendarRecipients.Year_Level_ID = yearSecInfo.YearSec_ID;

                                await _context.CalendarRecipientsEntity.AddAsync(newCalendarRecipients);
                            }
                        }

                        await _context.SaveChangesAsync();
                        dbcxtransaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return response.CreateResponse("500", Constants.FailedMessageUpdate + RepositoryName + ".", false);
                }
            }

            return response.CreateResponse("200", RepositoryName + Constants.SuccessMessageUpdate, true);
        }

        public async Task<ResultModel> DeleteCalendarPermanent(string calendarCode, int userId)
        {
            response = new Response();

            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var calendarData = _context.CalendarEntity.SingleOrDefault(x => x.Calendar_Code == calendarCode);

                    calendarData.IsActive = false;
                    calendarData.ToDisplay = false;
                    calendarData.Updated_By = userId;
                    calendarData.Last_Updated = DateTime.UtcNow.ToLocalTime();

                    _context.Entry(calendarData).Property(x => x.Date_Time_Added).IsModified = false;
                    _context.Entry(calendarData).Property(x => x.Added_By).IsModified = false;
                    _context.Entry(calendarData).Property(x => x.Last_Updated).IsModified = true;

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return response.CreateResponse("500", Constants.FailedMessageUpdate + RepositoryName + ".", false);
                }
            }

            return response.CreateResponse("200", RepositoryName + Constants.SuccessMessageDelete, true);
        }

        public async Task<calendarPagedResult> GetAll(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new calendarPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.CalendarEntity.Where(a => a.ToDisplay == true).Count();
                else
                    result.RowCount = this.context.CalendarEntity
                        .Where(q => q.ToDisplay == true && (q.Title.Contains(keyword)
                        || q.Body.Contains(keyword)
                        || q.Post_Date.ToString().Contains(keyword)
                        || q.Year_Level.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.calendar = await this.context.CalendarEntity.Where(a => a.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.calendar = await this.context.CalendarEntity
                        .Where(q => q.ToDisplay == true && (q.Title.Contains(keyword)
                        || q.Body.Contains(keyword)
                        || q.Post_Date.ToString().Contains(keyword)
                        || q.Year_Level.Contains(keyword)))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<calendarEntity> GetCalendarByCalendarCode(string code)
        {
            var calendarData = await this.context.CalendarEntity.Where(x => x.Calendar_Code == code).FirstOrDefaultAsync();

            if (calendarData != null)
            {
                var linkedRecipients = await this.context.CalendarRecipientsEntity.Include(q => q.CalendarEntity).Include(q => q.YearSectionEntity).Where(x => x.CalendarEntity.Calendar_Code == code).ToListAsync();
                calendarData.Recipients = new List<recipients>();

                foreach (var data in linkedRecipients)
                {
                    recipients recipients = new recipients();
                    recipients.key = data.YearSectionEntity.YearSec_Name;
                    recipients.value = data.YearSectionEntity.YearSec_Name;
                    calendarData.Recipients.Add(recipients);
                }
            }

            if(calendarData.IsAll)
            {
                calendarData.Recipients = new List<recipients>();
                recipients recipients = new recipients();
                recipients.key = "All";
                recipients.value = "All";
                calendarData.Recipients.Add(recipients);
            }

            return calendarData;
        }

        public async Task<calendarPagedResult> Export(string keyword)
        {
            try
            {
                var result = new calendarPagedResult();

                if (keyword == null || keyword == "")
                {
                    result.calendar = await _context.CalendarEntity
                        .Where(b => b.ToDisplay == true)
                        .OrderBy(c => c.Last_Updated).ToListAsync();
                }
                else
                {
                    result.calendar = await _context.CalendarEntity
                        .Where(q => q.ToDisplay == true && (q.Title.Contains(keyword)
                        || q.Body.Contains(keyword)
                        || q.Post_Date.ToString().Contains(keyword)
                        || q.Year_Level.Contains(keyword)))
                       .OrderBy(c => c.Last_Updated).ToListAsync();
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
