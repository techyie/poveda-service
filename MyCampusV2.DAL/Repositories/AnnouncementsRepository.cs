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
    public class AnnouncementsRepository : BaseRepository<announcementsEntity>, IAnnouncementsRepository
    {
        private readonly MyCampusCardContext context;
        private ResultModel resultModel;
        private Response response;
        private string RepositoryName = "Announcement";

        public AnnouncementsRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }


        public async Task<ResultModel> AddAnnouncement(announcementsEntity announcementsEntity, int userId)
        {
            response = new Response();

            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    announcementsEntity.IsActive = true;
                    announcementsEntity.ToDisplay = true;
                    announcementsEntity.Added_By = userId;
                    announcementsEntity.Updated_By = userId;
                    announcementsEntity.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                    announcementsEntity.Last_Updated = DateTime.UtcNow.ToLocalTime();
                    announcementsEntity.Date_Sent = DateTime.UtcNow.ToLocalTime();

                    announcementsEntity.Announcement_Code = System.Guid.NewGuid().ToString();
                    announcementsEntity.IsSent = false;

                    var isAll = announcementsEntity.Recipients.Where(q => q.key == "All").FirstOrDefault();

                    if (isAll != null)
                    {
                        announcementsEntity.IsAll = true;

                        announcementsEntity.Year_Level = "All";

                        await _context.AnnouncementsEntity.AddAsync(announcementsEntity);
                    }
                    else
                    {
                        announcementsEntity.Year_Level = string.Empty;

                        foreach (var recipient in announcementsEntity.Recipients)
                        {
                            announcementsEntity.Year_Level += ", " + recipient.key;
                        }

                        if (announcementsEntity.Year_Level.Substring(0, 1).IndexOf(',') >= 0)
                        {
                            announcementsEntity.Year_Level = announcementsEntity.Year_Level.Substring(1, announcementsEntity.Year_Level.Length - 1);
                            announcementsEntity.Year_Level = announcementsEntity.Year_Level.Trim();
                        }

                        await _context.AnnouncementsEntity.AddAsync(announcementsEntity);

                        foreach (var recipient in announcementsEntity.Recipients)
                        {
                            announcementsRecipientsEntity announcementsRecipientsEntity = new announcementsRecipientsEntity();

                            yearSectionEntity yearSecInfo = await _context.YearSectionEntity.Where(x => x.YearSec_Name == recipient.key && x.IsActive == true).FirstOrDefaultAsync();

                            announcementsRecipientsEntity.Announcement_ID = announcementsEntity.ID;
                            announcementsRecipientsEntity.Year_Level_ID = yearSecInfo.YearSec_ID;

                            await _context.AnnouncementsRecipientsEntity.AddAsync(announcementsRecipientsEntity);
                        }
                    }

                    await _context.SaveChangesAsync();

                    contextTransaction.Commit();

                    return response.CreateResponse("200", Constants.SuccessAnnouncementAdd, true);
                }
                catch (Exception err)
                {
                    contextTransaction.Rollback();
                    return response.CreateResponse("500", Constants.FailedMessageCreate + RepositoryName + ".", false);
                }
            }
        }

        public async Task<ResultModel> UpdateAnnouncement(announcementsEntity announcementsEntity, int userId)
        {
            response = new Response();

            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var announcementData = _context.AnnouncementsEntity.SingleOrDefault(x => x.Announcement_Code == announcementsEntity.Announcement_Code);

                    if (announcementData != null)
                    {
                        announcementData.Title = announcementsEntity.Title;
                        announcementData.Body = announcementsEntity.Body;
                        announcementData.Updated_By = userId;
                        announcementData.Last_Updated = DateTime.UtcNow.ToLocalTime();

                        var isAll = announcementsEntity.Recipients.Where(q => q.key == "All").FirstOrDefault();

                        _context.Entry(announcementData).Property(x => x.Date_Time_Added).IsModified = false;
                        _context.Entry(announcementData).Property(x => x.Added_By).IsModified = false;
                        _context.Entry(announcementData).Property(x => x.Last_Updated).IsModified = true;

                        ICollection<announcementsRecipientsEntity> announcementsRecipients = await _context.AnnouncementsRecipientsEntity.Include(q => q.AnnouncementsEntity).Where(q => q.AnnouncementsEntity.Announcement_Code == announcementsEntity.Announcement_Code).ToListAsync();

                        if (announcementsRecipients != null)
                        {
                            _context.AnnouncementsRecipientsEntity.RemoveRange(announcementsRecipients);
                        }

                        if (isAll != null)
                        {
                            announcementData.IsAll = true;

                            announcementData.Year_Level = "All";
                        }
                        else
                        {
                            announcementData.Year_Level = string.Empty;
                            announcementData.IsAll = false;

                            foreach (var recipient in announcementsEntity.Recipients)
                            {
                                announcementData.Year_Level += ", " + recipient.key;
                            }

                            if (announcementData.Year_Level.Substring(0, 1).IndexOf(',') >= 0)
                            {
                                announcementData.Year_Level = announcementData.Year_Level.Substring(1, announcementData.Year_Level.Length - 1);
                                announcementData.Year_Level = announcementData.Year_Level.Trim();
                            }

                            foreach (var recipient in announcementsEntity.Recipients)
                            {
                                announcementsRecipientsEntity newAnnouncementsRecipients = new announcementsRecipientsEntity();

                                yearSectionEntity yearSecInfo = await _context.YearSectionEntity.Where(x => x.YearSec_Name == recipient.key && x.IsActive == true).FirstOrDefaultAsync();

                                newAnnouncementsRecipients.Announcement_ID = announcementData.ID;
                                newAnnouncementsRecipients.Year_Level_ID = yearSecInfo.YearSec_ID;

                                await _context.AnnouncementsRecipientsEntity.AddAsync(newAnnouncementsRecipients);
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

        public async Task<ResultModel> DeleteAnnouncementsPermanent(string announcementCode, int userId)
        {
            response = new Response();

            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var announcementsData = _context.AnnouncementsEntity.SingleOrDefault(x => x.Announcement_Code == announcementCode);

                    announcementsData.IsActive = false;
                    announcementsData.ToDisplay = false;
                    announcementsData.Updated_By = userId;
                    announcementsData.Last_Updated = DateTime.UtcNow.ToLocalTime();

                    _context.Entry(announcementsData).Property(x => x.Date_Time_Added).IsModified = false;
                    _context.Entry(announcementsData).Property(x => x.Added_By).IsModified = false;
                    _context.Entry(announcementsData).Property(x => x.Last_Updated).IsModified = true;

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return response.CreateResponse("500", Constants.FailedArchivingAnnouncement, false);
                }
            }

            return response.CreateResponse("200", Constants.SuccessArchivingAnnouncement, true);
        }

        public async Task<announcementsPagedResult> GetAll(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new announcementsPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.AnnouncementsEntity.Where(a => a.ToDisplay == true).Count();
                else
                    result.RowCount = this.context.AnnouncementsEntity
                        .Where(q => q.ToDisplay == true && (q.Title.Contains(keyword)
                        || q.Body.Contains(keyword)
                        || q.Date_Sent.ToString().Contains(keyword)
                        || q.Year_Level.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.announcements = await this.context.AnnouncementsEntity.Where(a => a.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.announcements = await this.context.AnnouncementsEntity
                        .Where(q => q.ToDisplay == true && (q.Title.Contains(keyword)
                        || q.Body.Contains(keyword)
                        || q.Date_Sent.ToString().Contains(keyword)
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

        public async Task<announcementsEntity> GetAnnouncementByAnnouncementCode(string code)
        {
            var announcementData = await this.context.AnnouncementsEntity.Where(x => x.Announcement_Code == code).FirstOrDefaultAsync();

            if (announcementData != null)
            {
                var linkedRecipients = await this.context.AnnouncementsRecipientsEntity.Include(q => q.AnnouncementsEntity).Include(q => q.YearSectionEntity).Where(x => x.AnnouncementsEntity.Announcement_Code == code).ToListAsync();
                announcementData.Recipients = new List<recipients>();

                foreach (var data in linkedRecipients)
                {
                    recipients recipients = new recipients();
                    recipients.key = data.YearSectionEntity.YearSec_Name;
                    recipients.value = data.YearSectionEntity.YearSec_Name;
                    announcementData.Recipients.Add(recipients);
                }
            }

            if(announcementData.IsAll)
            {
                announcementData.Recipients = new List<recipients>();
                recipients recipients = new recipients();
                recipients.key = "All";
                recipients.value = "All";
                announcementData.Recipients.Add(recipients);
            }

            return announcementData;
        }

        public async Task<IList<yearSectionEntity>> GetYearLevelList()
        {
            return await this.context.YearSectionEntity.Where(x => x.IsActive == true && x.ToDisplay == true).ToListAsync();
        }

        public async Task<announcementsPagedResult> Export(string keyword)
        {
            try
            {
                var result = new announcementsPagedResult();

                if (keyword == null || keyword == "")
                {
                    result.announcements = await _context.AnnouncementsEntity
                        .Where(b => b.ToDisplay == true)
                        .OrderBy(c => c.Last_Updated).ToListAsync();
                }
                else
                {
                    result.announcements = await _context.AnnouncementsEntity
                        .Where(q => q.ToDisplay == true && (q.Title.Contains(keyword)
                        || q.Body.Contains(keyword)
                        || q.Date_Sent.ToString().Contains(keyword)
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
