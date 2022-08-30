using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.Common.Helpers;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.Helpers;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class TerminalRepository : BaseRepository<terminalEntity>, ITerminalRepository
    {
        private readonly MyCampusCardContext context;
        private ResultModel result = new ResultModel();
        private Response response = new Response();

        public TerminalRepository(MyCampusCardContext Context) : base(Context)
        {
            this.context = Context;
        }

        public async Task<terminalEntity> GetById(int? id)
        {
            return await this.context.TerminalEntity.Include(x => x.AreaEntity).ThenInclude(c => c.CampusEntity).Where(d => d.Terminal_ID == id).FirstOrDefaultAsync();
        }

        public IQueryable<terminalEntity> GetAllTerminal()
        {
            //return _context.terminalEntity.Include(area => area.tbl_area).ThenInclude(b => b.tbl_campus).Include(cat => cat.tbl_terminal_category).OrderBy(u => u.Terminal_Code);
            return null;
        }

        public async Task<ICollection<terminalEntity>> GetAllTerminalOnly(long campusID)
        {
            // 2 is terminal only.
            //return await _context.terminalEntity.Where(a => a.Terminal_Category_ID == 2 && a.tbl_area.Campus_ID == campusID).OrderBy(u => u.Terminal_Code).ToListAsync();
            return null;
        }

        public async Task<ICollection<terminalEntity>> GetAllTerminalPerso()
        {
            // 2 .
            //return await _context.terminalEntity.Where(a => a.Terminal_Category_ID == 2 && a.tbl_area.Campus_ID != 1).OrderBy(u => u.Terminal_Code).ToListAsync();
            return null;
        }

        public async Task AddWhitelistItem(int terminalid, int carddetailsid, int user)
        {
            //if (!_context.tbl_card_details.Any(x => x.ID == carddetailsid && x.IsActive == true))
            //{
            //    throw new ArgumentException("Card details does not exist.");
            //}

            //if (!_context.terminalEntity.Any(x => x.ID == terminalid && x.IsActive == true))
            //{
            //    throw new ArgumentException("Terminal does not exist.");
            //}

            //if (_context.tbl_terminal_whitelist.Any(x => x.ID == terminalid && x.Card_Details_ID == carddetailsid))
            //{
            //    throw new ArgumentException("The card does not belong in the terminal whitelist. Use terminal sync to update the terminal whitelist.");
            //}

            //tbl_terminal_whitelist terminalwihtelist = await _context.tbl_terminal_whitelist.SingleOrDefaultAsync(x => x.Terminal_ID == terminalid && x.Card_Details_ID == carddetailsid);

            //if (terminalwihtelist == null)
            //{
            //    throw new ArgumentException("The card does not belong in the terminal whitelist. Use terminal sync to update the terminal whitelist.");
            //}

            //tbl_card_details carddetails = await _context.tbl_card_details.Include(x => x.tbl_person).Where(q => q.ID == carddetailsid && q.IsActive == true).FirstOrDefaultAsync();

            //if (carddetails != null)
            //{
            //    using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            //    {
            //        //try
            //        //{
            //        //    tbl_data_sync datasync = new tbl_data_sync();

            //        //    datasync.Added_By = user;
            //        //    datasync.Date_Time_Added = DateTime.Now;
            //        //    datasync.Updated_By = user;
            //        //    datasync.Last_Updated = DateTime.Now;
            //        //    datasync.IsActive = true;

            //        //    terminalwihtelist.Updated_By = user;
            //        //    terminalwihtelist.Last_Updated = DateTime.Now;
            //        //    terminalwihtelist.IsActive = true;

            //        //    datasync.Card_Serial = carddetails.Card_Serial;
            //        //    datasync.Blocked = carddetails.Blocked;
            //        //    datasync.On_Hold = carddetails.On_Hold;
            //        //    datasync.Is_Upload = false;
            //        //    datasync.Action = "A";
            //        //    datasync.Expiry_Date = carddetails.Expiry_date;
            //        //    datasync.Person_Type = carddetails.tbl_person.Person_Type;
            //        //    datasync.Terminal_ID = terminalid;

            //        //    terminalwihtelist.Terminal_ID = terminalid;
            //        //    terminalwihtelist.Card_Details_ID = carddetailsid;

            //        //    await _context.tbl_data_sync.AddAsync(datasync);

            //        //    //await _context.tbl_terminal_whitelist.AddAsync(terminalwihtelist);

            //        //    await _context.SaveChangesAsync();

            //        //    dbcxtransaction.Commit();
            //        //}
            //        //catch (Exception ex)
            //        //{
            //        //    dbcxtransaction.Rollback();
            //        //    throw ex;
            //        //}
            //    }
            //}
            //else
            //{
            //    throw new ArgumentException("Card details does not exist.");
            //}
        }

        public async Task RemoveWhitelistItem(int terminalid, int carddetailsid, int user)
        {
            try
            {
                //if (!_context.tbl_card_details.Any(x => x.ID == carddetailsid && x.IsActive == true))
                //{
                //    throw new ArgumentException("Card details does not exist.");
                //}

                //if (!_context.terminalEntity.Any(x => x.ID == terminalid && x.IsActive == true))
                //{
                //    throw new ArgumentException("Terminal does not exist.");
                //}

                //tbl_card_details carddetails = await _context.tbl_card_details.Include(x => x.tbl_person).Where(q => q.ID == carddetailsid && q.IsActive == true).FirstOrDefaultAsync();

                //if (carddetails != null)
                //{
                //    using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
                //    {
                //        try
                //        {
                //            tbl_terminal_whitelist terminalwhitelist = await _context.tbl_terminal_whitelist.Where(x => x.Terminal_ID == terminalid && x.Card_Details_ID == carddetailsid).FirstOrDefaultAsync();

                //            if (terminalwhitelist != null)
                //            {
                //                tbl_card_details cardrdetails = await _context.tbl_card_details.SingleOrDefaultAsync(q => q.ID == carddetailsid);

                //                if (carddetails != null)
                //                {
                //                    terminalwhitelist.IsActive = false;
                //                    terminalwhitelist.Last_Updated = DateTime.Now;
                //                    terminalwhitelist.Updated_By = user;

                //                    tbl_data_sync datasync = new tbl_data_sync();
                //                    datasync.Added_By = user;
                //                    datasync.Date_Time_Added = DateTime.Now;
                //                    datasync.Updated_By = user;
                //                    datasync.Last_Updated = DateTime.Now;
                //                    datasync.IsActive = true;

                //                    datasync.Card_Serial = carddetails.Card_Serial;
                //                    datasync.Blocked = carddetails.Blocked;
                //                    datasync.On_Hold = carddetails.On_Hold;
                //                    datasync.Is_Upload = false;
                //                    datasync.Action = "D";
                //                    datasync.Expiry_Date = carddetails.Expiry_date;
                //                    datasync.Person_Type = carddetails.tbl_person.Person_Type;
                //                    datasync.Terminal_ID = terminalid;

                //                    await _context.tbl_data_sync.AddAsync(datasync);
                //                }
                //            }

                //            await _context.SaveChangesAsync();
                //            dbcxtransaction.Commit();
                //        }
                //        catch (Exception ex)
                //        {
                //            dbcxtransaction.Rollback();
                //            throw ex;
                //        }
                //    }
                //}
                //else
                //{
                //    throw new ArgumentException("Card details does not exist.");
                //}
            }
            catch (Exception e)
            {
                throw new ArgumentException("Failed to remove card in terminal whitelist.");
            }
        }

        public async Task<ResultModel> SyncLatestCards(int terminalid, int user)
        {
            try
            {
                var cardids = await _context.TerminalWhitelistEntity.Where(q => q.Terminal_ID == terminalid).Select(v => v.Person_ID).ToListAsync();

                var carddetailslist = await _context.CardDetailsEntity.Include(x => x.PersonEntity).Where(q => !cardids.Contains(q.Person_ID) && (q.IsActive == true) && (q.PersonEntity.Person_Type == "S")).ToListAsync();

                if(carddetailslist.Count != 0)
                {
                    foreach (cardDetailsEntity data in carddetailslist)
                    {
                        using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                        {
                            try
                            {
                                datasyncEntity datasync = new datasyncEntity();
                                terminalWhitelistEntity terminalwhitelist = new terminalWhitelistEntity();

                                datasync.Added_By = user;
                                datasync.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                                datasync.Updated_By = user;
                                datasync.Last_Updated = DateTime.UtcNow.ToLocalTime();
                                datasync.IsActive = true;
                                datasync.DS_Action = "A";
                                datasync.Card_Serial = data.Card_Serial;
                                datasync.Expiry_Date = data.Expiry_Date;
                                datasync.Person_Type = data.PersonEntity.Person_Type;
                                datasync.Terminal_ID = terminalid;
                                datasync.User_ID = user;

                                terminalwhitelist.Terminal_ID = terminalid;
                                terminalwhitelist.Person_ID = data.Person_ID;
                                terminalwhitelist.Added_By = user;
                                terminalwhitelist.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                                terminalwhitelist.Updated_By = user;
                                terminalwhitelist.Last_Updated = DateTime.UtcNow.ToLocalTime();
                                terminalwhitelist.IsActive = true;

                                await _context.DatasyncEntity.AddAsync(datasync);

                                await _context.TerminalWhitelistEntity.AddAsync(terminalwhitelist);

                                await _context.SaveChangesAsync();

                                transaction.Commit();

                            }
                            catch (Exception err)
                            {
                                transaction.Rollback();
                                return response.CreateResponse("500", err.Message, false);
                            }
                        }
                    }
                    return response.CreateResponse("200", "Terminal Sync has been successfully completed!", true);
                }

                return response.CreateResponse("200", "No cards to be synced!", true); ;

            } catch (Exception err)
            {
                return response.CreateResponse("500", err.Message, false);
            }

            //if (carddetailslist != null)
            //{
            //    foreach (tbl_card_details dtls in carddetailslist)
            //    {
            //        using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                tbl_data_sync datasync = new tbl_data_sync();
            //                tbl_terminal_whitelist terminalwhitelist = new tbl_terminal_whitelist();

            //                datasync.Added_By = user;
            //                datasync.Date_Time_Added = DateTime.Now;
            //                datasync.Updated_By = user;
            //                datasync.Last_Updated = DateTime.Now;
            //                datasync.IsActive = true;

            //                terminalwhitelist.Added_By = user;
            //                terminalwhitelist.Date_Time_Added = DateTime.Now;
            //                terminalwhitelist.Updated_By = user;
            //                terminalwhitelist.Last_Updated = DateTime.Now;
            //                terminalwhitelist.IsActive = true;

            //                datasync.Card_Serial = dtls.Card_Serial;
            //                datasync.Blocked = dtls.Blocked;
            //                datasync.On_Hold = dtls.On_Hold;
            //                datasync.Is_Upload = false;
            //                datasync.Action = "A";
            //                datasync.Expiry_Date = dtls.Expiry_date;
            //                datasync.Person_Type = dtls.tbl_person.Person_Type;
            //                datasync.Terminal_ID = terminalid;

            //                terminalwhitelist.Terminal_ID = terminalid;
            //                terminalwhitelist.Card_Details_ID = dtls.ID;

            //                await _context.tbl_data_sync.AddAsync(datasync);

            //                await _context.tbl_terminal_whitelist.AddAsync(terminalwhitelist);

            //                await _context.SaveChangesAsync();

            //                dbcxtransaction.Commit();
            //            }
            //            catch (Exception ex)
            //            {
            //                dbcxtransaction.Rollback();
            //                throw ex;
            //            }
            //        }
            //    }
            //}
        }


        public async Task<ResultModel> AddTerminal(terminalEntity entity, eventLoggingEntity eventEntity)
        {
            result = new ResultModel();

            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    terminalEntity terminalNewEntity = new terminalEntity
                    {
                        Terminal_ID = entity.Terminal_ID,
                        Terminal_Name = entity.Terminal_Name,
                        Terminal_Code = entity.Terminal_Code,
                        Terminal_IP = entity.Terminal_IP,
                        Terminal_Category = entity.Terminal_Category,
                        Terminal_Status = entity.Terminal_Status,
                        IsForFetcher = entity.IsForFetcher,
                        Area_ID = entity.Area_ID,
                        Added_By = entity.Added_By,
                        Updated_By = entity.Updated_By,
                        ToDisplay = true,
                        IsActive = true,
                        Date_Time_Added = DateTime.UtcNow.ToLocalTime(),
                        Last_Updated = DateTime.UtcNow.ToLocalTime(),
                    };

                    await this.context.TerminalEntity.AddAsync(terminalNewEntity);

                    await this.context.SaveChangesAsync();

                    areaEntity AreaEntity = await this.context.AreaEntity.Where(q => q.Area_ID == entity.Area_ID).FirstOrDefaultAsync();
                    campusEntity CampusEntity = await this.context.CampusEntity.Where(q => q.Campus_ID == AreaEntity.Campus_ID).FirstOrDefaultAsync();

                    terminalConfigurationEntity terminalConfigurationNewEntity = new terminalConfigurationEntity
                    {
                        Terminal_Code = Encryption.Encrypt("CB" + DateTime.UtcNow.ToLocalTime().ToString("HHmmss")),
                        Terminal_Schedule = "",
                        School_Name = CampusEntity.Campus_Name,
                        TerminalID = terminalNewEntity.Terminal_ID,
                        Host_IPAddress1 = Encryption.Encrypt("127.0.0.1"),
                        Host_Port1 = "3306",
                        Host_IPAddress2 = Encryption.Encrypt("127.0.0.1"),
                        Host_Port2 = "3306",
                        Viewer_Address = "",
                        Viewer_Port = "3306",
                        Reader_Name1 = "",
                        Reader_Direction1 = 1,
                        Enable_Antipassback1 = true,
                        Reader_Name2 = "",
                        Reader_Direction2 = 1,
                        Enable_Antipassback2 = true,
                        Loop_Delay = 10,
                        Turnstile_Delay = 10,
                        Terminal_Sync_Interval = 10,
                        Viewer_DB = "",
                        Terminal_ID = terminalNewEntity.Terminal_ID,
                        Date_Time_Uploaded = DateTime.UtcNow.ToLocalTime()
                    };

                    await this.context.TerminalConfigurationEntity.AddAsync(terminalConfigurationNewEntity);

                    eventEntity.Log_Date_Time = DateTime.UtcNow.ToLocalTime();
                    eventEntity.Source = "Add Terminal";
                    eventEntity.Message = "Success: " + entity.Terminal_Name;
                    await _context.EventLoggingEntity.AddAsync(eventEntity);

                    await this.context.SaveChangesAsync();

                    contextTransaction.Commit();

                    return response.CreateResponse("200", "Terminal" + Constants.SuccessMessageAdd, true);
                }
                catch (Exception err)
                {
                    contextTransaction.Rollback();
                    return response.CreateResponse("500", err.Message, false);
                }
            }
        }

        public async Task<ResultModel> UpdateTerminal(terminalEntity entity, eventLoggingEntity eventEntity)
        {
            result = new ResultModel();

            using (IDbContextTransaction contextTransaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    terminalEntity oldTerminalEntity = await this.context.TerminalEntity.Where(q => q.Terminal_ID == entity.Terminal_ID).FirstOrDefaultAsync();

                    oldTerminalEntity.Terminal_ID = entity.Terminal_ID;
                    oldTerminalEntity.Terminal_Name = entity.Terminal_Name;
                    oldTerminalEntity.Terminal_Code = entity.Terminal_Code;
                    oldTerminalEntity.Terminal_IP = entity.Terminal_IP;
                    oldTerminalEntity.Terminal_Category = entity.Terminal_Category;
                    oldTerminalEntity.Terminal_Status = entity.Terminal_Status;
                    oldTerminalEntity.Area_ID = entity.Area_ID;
                    oldTerminalEntity.Added_By = oldTerminalEntity.Added_By;
                    oldTerminalEntity.Updated_By = entity.Updated_By;
                    oldTerminalEntity.ToDisplay = entity.ToDisplay;
                    oldTerminalEntity.IsActive = entity.IsActive;
                    oldTerminalEntity.IsForFetcher = entity.IsForFetcher;
                    oldTerminalEntity.Date_Time_Added = oldTerminalEntity.Date_Time_Added;
                    oldTerminalEntity.Last_Updated = DateTime.UtcNow.ToLocalTime();

                    areaEntity AreaEntity = await this.context.AreaEntity.Where(q => q.Area_ID == entity.Area_ID).FirstOrDefaultAsync();
                    campusEntity CampusEntity = await this.context.CampusEntity.Where(q => q.Campus_ID == AreaEntity.Campus_ID).FirstOrDefaultAsync();

                    terminalConfigurationEntity oldTerminalConfigurationEntity = await this.context.TerminalConfigurationEntity.Where(q => q.Terminal_ID == oldTerminalEntity.Terminal_ID).FirstOrDefaultAsync();

                    if (oldTerminalConfigurationEntity != null)
                    {
                        oldTerminalConfigurationEntity.School_Name = CampusEntity.Campus_Name;
                        oldTerminalConfigurationEntity.Date_Time_Uploaded = DateTime.UtcNow.ToLocalTime();
                    }

                    eventEntity.Log_Date_Time = DateTime.UtcNow.ToLocalTime();
                    eventEntity.Source = "Update Terminal";
                    eventEntity.Message = "Success: " + entity.Terminal_Name;
                    await _context.EventLoggingEntity.AddAsync(eventEntity);

                    await this.context.SaveChangesAsync();

                    contextTransaction.Commit();

                    return response.CreateResponse("200", "Terminal" + Constants.SuccessMessageUpdate, true);
                }
                catch (Exception err)
                {
                    contextTransaction.Rollback();
                    return response.CreateResponse("500", err.Message, false);
                }
            }
        }

        public async Task<terminalPagedResult> GetAllTerminals(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new terminalPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.TerminalEntity.Count();
                else
                    result.RowCount = this.context.TerminalEntity
                        .Include(q => q.AreaEntity)
                        .ThenInclude(b => b.CampusEntity)
                        .Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.terminals = await _context.TerminalEntity
                        .Include(q => q.AreaEntity)
                        .ThenInclude(b => b.CampusEntity)
                        .OrderByDescending(c => c.Last_Updated).ToListAsync();
                //.Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.terminals = await _context.TerminalEntity
                        .Include(q => q.AreaEntity)
                        .ThenInclude(b => b.CampusEntity)
                        .OrderByDescending(c => c.Last_Updated).ToListAsync();
                //.Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<List<terminalEntity>> GetTerminalByCampusId(int campusId)
        {
            return await _context.TerminalEntity.Where(a => a.Terminal_Category == Encryption.Encrypt("PC/Display Terminal") 
                && a.AreaEntity.Campus_ID == campusId).OrderBy(u => u.Terminal_Name).ToListAsync();
        }
    }
}