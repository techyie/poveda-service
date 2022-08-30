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
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class CardDetailsRepository : BaseRepository<cardDetailsEntity>, ICardDetailsRepository
    {
        private ResultModel result = new ResultModel();
        private Response response = new Response();

        public CardDetailsRepository(MyCampusCardContext context) : base(context)
        {
        }

        public async Task<cardDetailsEntity> GetByCardSerial(string cardSerial)
        {
            return await _context.CardDetailsEntity.Where(a => a.Card_Serial == cardSerial && a.IsActive == true).FirstOrDefaultAsync();
        }

        public async Task<cardDetailsEntity> GetByPersonID(long person_ID)
        {
            return await _context.CardDetailsEntity.Where(a => a.Person_ID == person_ID && a.IsActive == true)
                .OrderBy(e => e.PersonEntity.ID_Number).FirstOrDefaultAsync();
        }
        public async Task<ICollection<cardDetailsEntity>> GetAllByPersonID(long person_ID)
        {
            return await _context.CardDetailsEntity.Where(a => a.Person_ID == person_ID)
                .OrderBy(e => e.PersonEntity.ID_Number).ToListAsync();
        }

        public async Task<cardDetailsEntity> GetByIdNumber(string idNumber)
        {
            return await _context.CardDetailsEntity.Include(a=>a.PersonEntity).Where(a => a.PersonEntity.ID_Number == idNumber)
                .OrderBy(e => e.PersonEntity.ID_Number).FirstOrDefaultAsync();
        }

        public async Task<fetcherScheduleDetailsEntity> GetFetcherScheduleDetails(int fetcherId)
        {
            return await _context.FetcherScheduleDetailsEntity.Where(x => x.FetcherScheduleEntity.Fetcher_ID == fetcherId && x.IsActive == true).FirstOrDefaultAsync();
        }

        public async Task<fetcherScheduleDetailsEntity> GetStudentScheduleDetails(int studentId)
        {
            return await _context.FetcherScheduleDetailsEntity.Where(x => x.Person_ID == studentId && x.IsActive == true).FirstOrDefaultAsync();
        }

        public async Task<cardPagedResult> ExportCards(string keyword)
        {
            var result = new cardPagedResult();

            if (keyword == null || keyword == "")
                result.cards = await _context.CardDetailsEntity
                    .Include(a=>a.PersonEntity)
                    .OrderBy(e => e.PersonEntity.ID_Number)
                    .Where(a=>a.IsActive)
                    .ToListAsync();
            else
                result.cards = await _context.CardDetailsEntity
                    .Include(a => a.PersonEntity)
                    .OrderBy(e => e.PersonEntity.ID_Number)
                    .Where(q => q.Card_Serial.Contains(keyword) && q.IsActive)
                    .ToListAsync();

            return result;
        }

        public async Task<ResultModel> AssignCard(cardDetailsEntity card, bool deleteInDataSyncFetcher)
        {
            result = new ResultModel();

            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var terminals = await GetTerminalList(card.Card_Person_Type);

                    string personType = card.Card_Person_Type.Substring(0, 1).ToUpper();
                    if (personType == "S" && !deleteInDataSyncFetcher)
                        personType = null;

                    card.IsActive = true;
                    card.ToDisplay = true;
                    card.Added_By = card.Added_By;
                    card.Updated_By = card.Updated_By;
                    card.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                    card.Last_Updated = DateTime.UtcNow.ToLocalTime();
                    card.Issued_Date = DateTime.UtcNow.ToLocalTime();
                    
                    await _context.CardDetailsEntity.AddAsync(card);

                    foreach (terminalEntity terminal in terminals)
                    {
                        datasyncEntity newDataSync = new datasyncEntity();
                        newDataSync.Card_Serial = card.Card_Serial;
                        newDataSync.DS_Action = "A";
                        newDataSync.Person_Type = personType;
                        newDataSync.Expiry_Date = card.Expiry_Date;
                        newDataSync.Terminal_ID = terminal.Terminal_ID;
                        newDataSync.User_ID = card.Added_By;

                        newDataSync.IsActive = true;
                        newDataSync.ToDisplay = true;
                        newDataSync.Added_By = card.Added_By;
                        newDataSync.Updated_By = card.Updated_By;
                        newDataSync.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                        newDataSync.Last_Updated = DateTime.UtcNow.ToLocalTime();

                        await _context.DatasyncEntity.AddAsync(newDataSync);

                        terminalWhitelistEntity terminalWhitelist = new terminalWhitelistEntity();
                        terminalWhitelist.Terminal_ID = terminal.Terminal_ID;
                        terminalWhitelist.Person_ID = card.Person_ID;
                        terminalWhitelist.Date_Time_Uploaded = DateTime.UtcNow.ToLocalTime();

                        terminalWhitelist.IsActive = true;
                        terminalWhitelist.ToDisplay = true;
                        terminalWhitelist.Added_By = card.Added_By;
                        terminalWhitelist.Updated_By = card.Updated_By;
                        terminalWhitelist.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                        terminalWhitelist.Last_Updated = DateTime.UtcNow.ToLocalTime();

                        await _context.TerminalWhitelistEntity.AddAsync(terminalWhitelist);
                    }

                    await _context.SaveChangesAsync();

                    contextTransaction.Commit();

                    return response.CreateResponse("200", "Card has been successfully assigned!", true);
                }
                catch (Exception err)
                {
                    contextTransaction.Rollback();
                    return response.CreateResponse("500", err.Message, false);
                }
            }
        }

        public async Task<ResultModel> ReassignCard(bool deleteInDataSyncFetcher, cardDetailsEntity card, cardDetailsEntity newCard)
        {
            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Get terminal list based on person type
                    var terminals = await GetTerminalList(card.Card_Person_Type);

                    // Remove person type of student if it has no pairing
                    string personType = newCard.Card_Person_Type.Substring(0, 1).ToUpper();
                    if (personType == "S" && !deleteInDataSyncFetcher)
                        personType = null;

                    card.Card_Person_Type = personType;

                    // Delete and Insert in Data Sync Fetcher
                    if (deleteInDataSyncFetcher && card.Card_Person_Type == "S")
                    {
                        var studentSched = await _context.FetcherScheduleDetailsEntity.Where(x => x.Person_ID == newCard.Person_ID && x.IsActive == true).ToListAsync();
                        var studentInfo = await _context.PersonEntity.Where(x => x.Person_ID == newCard.Person_ID && x.IsActive == true).FirstOrDefaultAsync();

                        foreach (fetcherScheduleDetailsEntity item in studentSched)
                        {
                            var fetcherSched = await _context.FetcherScheduleEntity.Where(x => x.Fetcher_Sched_ID == item.Fetcher_Sched_ID && x.IsActive == true).FirstOrDefaultAsync();
                            var schedule = await _context.ScheduleEntity.Where(x => x.Schedule_ID == fetcherSched.Schedule_ID && x.IsActive == true).FirstOrDefaultAsync();
                            var fetcherCard = await _context.CardDetailsEntity.Where(x => x.Person_ID == fetcherSched.Fetcher_ID && x.IsActive == true).FirstOrDefaultAsync();
                            var fetcherInfo = await _context.PersonEntity.Where(x => x.Person_ID == fetcherSched.Fetcher_ID && x.IsActive == true).FirstOrDefaultAsync();
                            var schedDays = GetScheduleDays(schedule.Schedule_Days);

                            foreach (terminalEntity terminal in terminals)
                            {
                                var deleteData = new datasyncFetcherEntity
                                {
                                    FetcherSerial = fetcherCard.Card_Serial,
                                    StudentSerial = card.Card_Serial,
                                    SchedDays = schedDays,
                                    TimeFrom = schedule.Schedule_Time_From,
                                    TimeTo = schedule.Schedule_Time_To,
                                    DS_Action = "D",
                                    Terminal_ID = terminal.Terminal_ID,
                                    StudID = studentInfo.ID_Number,
                                    FetcherID = fetcherInfo.ID_Number,
                                    User_ID = newCard.Updated_By
                                };

                                await _context.DatasyncFetcherEntity.AddAsync(deleteData);
                            }

                            await _context.SaveChangesAsync();

                            foreach (terminalEntity terminal in terminals)
                            {
                                var insertData = new datasyncFetcherEntity
                                {
                                    FetcherSerial = fetcherCard.Card_Serial,
                                    StudentSerial = newCard.Card_Serial,
                                    SchedDays = schedDays,
                                    TimeFrom = schedule.Schedule_Time_From,
                                    TimeTo = schedule.Schedule_Time_To,
                                    DS_Action = "A",
                                    Terminal_ID = terminal.Terminal_ID,
                                    StudID = studentInfo.ID_Number,
                                    FetcherID = fetcherInfo.ID_Number,
                                    User_ID = newCard.Updated_By
                                };

                                await _context.DatasyncFetcherEntity.AddAsync(insertData);
                            }

                            await _context.SaveChangesAsync();
                        }
                    }
                    else if (deleteInDataSyncFetcher && card.Card_Person_Type == "F")
                    {
                        var fetcherSched = await _context.FetcherScheduleEntity.Where(x => x.Fetcher_ID == newCard.Person_ID && x.IsActive == true).ToListAsync();
                        var fetcherInfo = await _context.PersonEntity.Where(x => x.Person_ID == newCard.Person_ID && x.IsActive == true).FirstOrDefaultAsync();

                        foreach (fetcherScheduleEntity sched in fetcherSched)
                        {
                            var fetcherSchedDetails = await _context.FetcherScheduleDetailsEntity.Where(x => x.Fetcher_Sched_ID == sched.Fetcher_Sched_ID && x.IsActive == true).ToListAsync();
                            var schedule = await _context.ScheduleEntity.Where(x => x.Schedule_ID == sched.Schedule_ID && x.IsActive == true).FirstOrDefaultAsync();
                            var schedDays = GetScheduleDays(schedule.Schedule_Days);

                            foreach (fetcherScheduleDetailsEntity details in fetcherSchedDetails)
                            {
                                var studentCard = await _context.CardDetailsEntity.Where(x => x.Person_ID == details.Person_ID && x.IsActive == true).FirstOrDefaultAsync();
                                var studentInfo = await _context.PersonEntity.Where(x => x.Person_ID == details.Person_ID && x.IsActive == true).FirstOrDefaultAsync();

                                foreach (terminalEntity terminal in terminals)
                                {
                                    var deleteData = new datasyncFetcherEntity
                                    {
                                        FetcherSerial = card.Card_Serial,
                                        StudentSerial = studentCard.Card_Serial,
                                        SchedDays = schedDays,
                                        TimeFrom = schedule.Schedule_Time_From,
                                        TimeTo = schedule.Schedule_Time_To,
                                        DS_Action = "D",
                                        Terminal_ID = terminal.Terminal_ID,
                                        StudID = studentInfo.ID_Number,
                                        FetcherID = fetcherInfo.ID_Number,
                                        User_ID = newCard.Updated_By
                                    };

                                    await _context.DatasyncFetcherEntity.AddAsync(deleteData);
                                }

                                await _context.SaveChangesAsync();

                                foreach (terminalEntity terminal in terminals)
                                {
                                    var insertData = new datasyncFetcherEntity
                                    {
                                        FetcherSerial = newCard.Card_Serial,
                                        StudentSerial = studentCard.Card_Serial,
                                        SchedDays = schedDays,
                                        TimeFrom = schedule.Schedule_Time_From,
                                        TimeTo = schedule.Schedule_Time_To,
                                        DS_Action = "A",
                                        Terminal_ID = terminal.Terminal_ID,
                                        StudID = studentInfo.ID_Number,
                                        FetcherID = fetcherInfo.ID_Number,
                                        User_ID = newCard.Updated_By
                                    };

                                    await _context.DatasyncFetcherEntity.AddAsync(insertData);
                                }

                                await _context.SaveChangesAsync();
                            }
                        }
                    }

                    // Deactivate Card
                    var existingCard = await _context.CardDetailsEntity.Where(x => x.Cardholder_ID == card.Cardholder_ID && x.IsActive == true).OrderByDescending(x => x.Last_Updated).FirstOrDefaultAsync();
                    existingCard.Remarks = card.Remarks;
                    existingCard.IsActive = false;
                    existingCard.ToDisplay = false;
                    existingCard.Updated_By = card.Updated_By;
                    existingCard.Last_Updated = DateTime.UtcNow.ToLocalTime();

                    // Remove Deactivated Card in Terminal Whitelist
                    var whitelist = await _context.TerminalWhitelistEntity.Where(x => x.Person_ID == card.Person_ID).ToListAsync();
                    _context.TerminalWhitelistEntity.RemoveRange(whitelist);

                    // Delete Deactivated Card in Terminals
                    foreach (terminalEntity terminal in terminals)
                    {
                        datasyncEntity newDataSync = new datasyncEntity();
                        newDataSync.Card_Serial = existingCard.Card_Serial;
                        newDataSync.DS_Action = "D";
                        newDataSync.Person_Type = card.Card_Person_Type;
                        newDataSync.Expiry_Date = existingCard.Expiry_Date;
                        newDataSync.Terminal_ID = terminal.Terminal_ID;
                        newDataSync.User_ID = card.Updated_By;
                        newDataSync.IsActive = true;
                        newDataSync.ToDisplay = true;
                        newDataSync.Added_By = card.Updated_By;
                        newDataSync.Updated_By = card.Updated_By;
                        newDataSync.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                        newDataSync.Last_Updated = DateTime.UtcNow.ToLocalTime();

                        await _context.DatasyncEntity.AddAsync(newDataSync);
                    }

                    await _context.SaveChangesAsync();

                    // Insert New Card
                    newCard.IsActive = true;
                    newCard.ToDisplay = true;
                    newCard.Added_By = newCard.Added_By;
                    newCard.Updated_By = newCard.Updated_By;
                    newCard.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                    newCard.Last_Updated = DateTime.UtcNow.ToLocalTime();
                    newCard.Issued_Date = DateTime.UtcNow.ToLocalTime();
                    await _context.CardDetailsEntity.AddAsync(newCard);

                    // Add New Card in Terminals and Terminal Whitelist
                    foreach (terminalEntity terminal in terminals)
                    {
                        datasyncEntity newDataSync = new datasyncEntity();
                        newDataSync.Card_Serial = newCard.Card_Serial;
                        newDataSync.DS_Action = "A";
                        newDataSync.Person_Type = card.Card_Person_Type;
                        newDataSync.Expiry_Date = newCard.Expiry_Date;
                        newDataSync.Terminal_ID = terminal.Terminal_ID;
                        newDataSync.User_ID = newCard.Added_By;

                        newDataSync.IsActive = true;
                        newDataSync.ToDisplay = true;
                        newDataSync.Added_By = newCard.Added_By;
                        newDataSync.Updated_By = newCard.Updated_By;
                        newDataSync.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                        newDataSync.Last_Updated = DateTime.UtcNow.ToLocalTime();

                        await this._context.DatasyncEntity.AddAsync(newDataSync);

                        terminalWhitelistEntity terminalWhitelist = new terminalWhitelistEntity();
                        terminalWhitelist.Terminal_ID = terminal.Terminal_ID;
                        terminalWhitelist.Person_ID = newCard.Person_ID;
                        terminalWhitelist.Date_Time_Uploaded = DateTime.UtcNow.ToLocalTime();

                        terminalWhitelist.IsActive = true;
                        terminalWhitelist.ToDisplay = true;
                        terminalWhitelist.Added_By = newCard.Added_By;
                        terminalWhitelist.Updated_By = newCard.Updated_By;
                        terminalWhitelist.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                        terminalWhitelist.Last_Updated = DateTime.UtcNow.ToLocalTime();

                        await _context.TerminalWhitelistEntity.AddAsync(terminalWhitelist);
                    }

                    await _context.SaveChangesAsync();

                    contextTransaction.Commit();

                    return response.CreateResponse("200", "Card has been successfully reassigned!", true);
                }
                catch (Exception err)
                {
                    contextTransaction.Rollback();
                    return response.CreateResponse("500", err.Message, false);
                }
            }
        }

        public async Task<ResultModel> UpdateCard(cardDetailsEntity card, bool deleteInDataSyncFetcher)
        {
            result = new ResultModel();

            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var terminals = await GetTerminalList(card.Card_Person_Type);

                    string personType = card.Card_Person_Type.Substring(0, 1).ToUpper();
                    if (personType == "S" && !deleteInDataSyncFetcher)
                        personType = null;

                    card.Card_Person_Type = personType;

                    var existingCard = await _context.CardDetailsEntity.Where(x => x.Cardholder_ID == card.Cardholder_ID && x.IsActive == true).OrderByDescending(x => x.Last_Updated).FirstOrDefaultAsync();
                    existingCard.Expiry_Date = card.Expiry_Date;
                    existingCard.Remarks = card.Remarks;
                    existingCard.Updated_By = card.Updated_By;
                    existingCard.Last_Updated = DateTime.UtcNow.ToLocalTime();

                    foreach (terminalEntity terminal in terminals)
                    {
                        datasyncEntity newDataSync = new datasyncEntity();
                        newDataSync.Card_Serial = existingCard.Card_Serial;
                        newDataSync.DS_Action = "U";
                        newDataSync.Person_Type = card.Card_Person_Type;
                        newDataSync.Expiry_Date = card.Expiry_Date;
                        newDataSync.Terminal_ID = terminal.Terminal_ID;
                        newDataSync.User_ID = card.Updated_By;
                        newDataSync.IsActive = true;
                        newDataSync.ToDisplay = true;
                        newDataSync.Added_By = card.Updated_By;
                        newDataSync.Updated_By = card.Updated_By;
                        newDataSync.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                        newDataSync.Last_Updated = DateTime.UtcNow.ToLocalTime();

                        await _context.DatasyncEntity.AddAsync(newDataSync);
                    }

                    await _context.SaveChangesAsync();
                    contextTransaction.Commit();

                    return response.CreateResponse("200", "Card" + Constants.SuccessMessageUpdate, true);
                }
                catch (Exception err)
                {
                    contextTransaction.Rollback();
                    return response.CreateResponse("500", err.Message, false);
                }
            }
        }
        public async Task<ResultModel> DeactivateCard(cardDetailsEntity card, bool deleteInDataSyncFetcher)
        {
            result = new ResultModel();

            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var terminals = await GetTerminalList(card.Card_Person_Type);

                    string personType = card.Card_Person_Type.Substring(0, 1).ToUpper();
                    if (personType == "S" && !deleteInDataSyncFetcher)
                        personType = null;

                    card.Card_Person_Type = personType;

                    if (card.Card_Is_Separated == true)
                    {
                        var person = await _context.PersonEntity.Where(x => x.Person_ID == card.Person_ID && x.IsActive == true).FirstOrDefaultAsync();
                        person.Separated_Date = (DateTime)card.Card_Separated_Date;
                        person.Updated_By = card.Updated_By;
                        person.Last_Updated = DateTime.UtcNow.ToLocalTime();
                    }

                    if (card.Card_Person_Type == "F")
                    {
                        var fetcherSched = await _context.FetcherScheduleEntity.Where(x => x.Fetcher_ID == card.Person_ID && x.IsActive == true).ToListAsync();

                        foreach (fetcherScheduleEntity sched in fetcherSched)
                        {
                            var fetcherSchedDetails = await _context.FetcherScheduleDetailsEntity.Where(x => x.Fetcher_Sched_ID == sched.Fetcher_Sched_ID && x.IsActive == true).ToListAsync();

                            foreach (fetcherScheduleDetailsEntity details in fetcherSchedDetails)
                            {
                                details.IsActive = false;
                                details.ToDisplay = false;
                                details.Updated_By = card.Updated_By;
                                details.Last_Updated = DateTime.UtcNow.ToLocalTime();
                            }
                        }
                    }
                    else if (card.Card_Person_Type == "S")
                    {
                        var fetcherSchedDetails = await _context.FetcherScheduleDetailsEntity.Where(x => x.Person_ID == card.Person_ID && x.IsActive == true).ToListAsync();

                        foreach (fetcherScheduleDetailsEntity details in fetcherSchedDetails)
                        {
                            details.IsActive = false;
                            details.ToDisplay = false;
                            details.Updated_By = card.Updated_By;
                            details.Last_Updated = DateTime.UtcNow.ToLocalTime();
                        }
                    }

                    if (deleteInDataSyncFetcher && card.Card_Person_Type == "S")
                    {
                        var studentSched = await _context.FetcherScheduleDetailsEntity.Where(x => x.Person_ID == card.Person_ID && x.IsActive == true).ToListAsync();
                        var studentInfo = await _context.PersonEntity.Where(x => x.Person_ID == card.Person_ID && x.IsActive == true).FirstOrDefaultAsync();

                        foreach (fetcherScheduleDetailsEntity item in studentSched)
                        {
                            var fetcherSched = await _context.FetcherScheduleEntity.Where(x => x.Fetcher_Sched_ID == item.Fetcher_Sched_ID && x.IsActive == true).FirstOrDefaultAsync();
                            var schedule = await _context.ScheduleEntity.Where(x => x.Schedule_ID == fetcherSched.Schedule_ID && x.IsActive == true).FirstOrDefaultAsync();
                            var fetcherCard = await _context.CardDetailsEntity.Where(x => x.Person_ID == fetcherSched.Fetcher_ID && x.IsActive == true).FirstOrDefaultAsync();
                            var fetcherInfo = await _context.PersonEntity.Where(x => x.Person_ID == fetcherSched.Fetcher_ID && x.IsActive == true).FirstOrDefaultAsync();
                            var schedDays = GetScheduleDays(schedule.Schedule_Days);

                            foreach (terminalEntity terminal in terminals)
                            {
                                var deleteData = new datasyncFetcherEntity
                                {
                                    FetcherSerial = fetcherCard.Card_Serial,
                                    StudentSerial = card.Card_Serial,
                                    SchedDays = schedDays,
                                    TimeFrom = schedule.Schedule_Time_From,
                                    TimeTo = schedule.Schedule_Time_To,
                                    DS_Action = "D",
                                    Terminal_ID = terminal.Terminal_ID,
                                    StudID = studentInfo.ID_Number,
                                    FetcherID = fetcherInfo.ID_Number,
                                    User_ID = card.Updated_By
                                };

                                await _context.DatasyncFetcherEntity.AddAsync(deleteData);
                            }
                        }
                    }
                    else if (deleteInDataSyncFetcher && card.Card_Person_Type == "F")
                    {
                        var fetcherSched = await _context.FetcherScheduleEntity.Where(x => x.Fetcher_ID == card.Person_ID && x.IsActive == true).ToListAsync();
                        var fetcherInfo = await _context.PersonEntity.Where(x => x.Person_ID == card.Person_ID && x.IsActive == true).FirstOrDefaultAsync();

                        foreach (fetcherScheduleEntity sched in fetcherSched)
                        {
                            var fetcherSchedDetails = await _context.FetcherScheduleDetailsEntity.Where(x => x.Fetcher_Sched_ID == sched.Fetcher_Sched_ID && x.IsActive == true).ToListAsync();
                            var schedule = await _context.ScheduleEntity.Where(x => x.Schedule_ID == sched.Schedule_ID && x.IsActive == true).FirstOrDefaultAsync();
                            var schedDays = GetScheduleDays(schedule.Schedule_Days);

                            foreach (fetcherScheduleDetailsEntity details in fetcherSchedDetails)
                            {
                                var studentCard = await _context.CardDetailsEntity.Where(x => x.Person_ID == details.Person_ID && x.IsActive == true).FirstOrDefaultAsync();
                                var studentInfo = await _context.PersonEntity.Where(x => x.Person_ID == details.Person_ID && x.IsActive == true).FirstOrDefaultAsync();

                                foreach (terminalEntity terminal in terminals)
                                {
                                    var deleteData = new datasyncFetcherEntity
                                    {
                                        FetcherSerial = card.Card_Serial,
                                        StudentSerial = studentCard.Card_Serial,
                                        SchedDays = schedDays,
                                        TimeFrom = schedule.Schedule_Time_From,
                                        TimeTo = schedule.Schedule_Time_To,
                                        DS_Action = "D",
                                        Terminal_ID = terminal.Terminal_ID,
                                        StudID = studentInfo.ID_Number,
                                        FetcherID = fetcherInfo.ID_Number,
                                        User_ID = card.Updated_By
                                    };

                                    await _context.DatasyncFetcherEntity.AddAsync(deleteData);
                                }
                            }
                        }
                    }

                    var existingCard = await _context.CardDetailsEntity.Where(x => x.Cardholder_ID == card.Cardholder_ID && x.IsActive == true).OrderByDescending(x => x.Last_Updated).FirstOrDefaultAsync();
                    existingCard.Remarks = card.Remarks;
                    existingCard.IsActive = false; 
                    existingCard.ToDisplay = false;
                    existingCard.Updated_By = card.Updated_By;
                    existingCard.Last_Updated = DateTime.UtcNow.ToLocalTime();

                    foreach (terminalEntity terminal in terminals)
                    {
                        datasyncEntity newDataSync = new datasyncEntity();
                        newDataSync.Card_Serial = existingCard.Card_Serial;
                        newDataSync.DS_Action = "D";
                        newDataSync.Person_Type = card.Card_Person_Type;
                        newDataSync.Expiry_Date = existingCard.Expiry_Date;
                        newDataSync.Terminal_ID = terminal.Terminal_ID;
                        newDataSync.User_ID = card.Updated_By;
                        newDataSync.IsActive = true;
                        newDataSync.ToDisplay = true;
                        newDataSync.Added_By = card.Updated_By;
                        newDataSync.Updated_By = card.Updated_By;
                        newDataSync.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                        newDataSync.Last_Updated = DateTime.UtcNow.ToLocalTime();

                        await _context.DatasyncEntity.AddAsync(newDataSync);
                    }

                    var whitelist = await _context.TerminalWhitelistEntity.Where(x => x.Person_ID == card.Person_ID).ToListAsync();
                    _context.TerminalWhitelistEntity.RemoveRange(whitelist);

                    await _context.SaveChangesAsync();
                    contextTransaction.Commit();

                    return response.CreateResponse("200", "Card" + Constants.SuccessMessageTemporaryDelete, true);
                }
                catch (Exception err)
                {
                    contextTransaction.Rollback();
                    return response.CreateResponse("500", err.Message, false);
                }
            }
        }

        private async Task<List<terminalEntity>> GetTerminalList(string personType)
        {
            if (personType == "Student")
            {
                return await _context.TerminalEntity.Where(q => (q.Terminal_Category == Common.Helpers.Encryption.Encrypt("Time And Attendance Terminal")
                    || q.Terminal_Category == Common.Helpers.Encryption.Encrypt("Mobile Terminal"))
                    && q.IsForFetcher == false
                    && q.IsActive == true).ToListAsync();
            }
            else if (personType == "Fetcher")
            {
                return await _context.TerminalEntity.Where(q => q.Terminal_Category == Common.Helpers.Encryption.Encrypt("Time And Attendance Terminal")
                    && q.IsForFetcher == true
                    && q.IsActive == true).ToListAsync();
            }
            else
            {
                return await _context.TerminalEntity.Where(q => q.Terminal_Category == Common.Helpers.Encryption.Encrypt("Time And Attendance Terminal")
                    && q.IsForFetcher == false
                    && q.IsActive == true).ToListAsync();
            }
        }

        private string GetScheduleDays(string schedDays)
        {
            if (schedDays.Contains("MON"))
                schedDays = schedDays.Replace("MON", "1");
            if (schedDays.Contains("TUE"))
                schedDays = schedDays.Replace("TUE", "2");
            if (schedDays.Contains("WED"))
                schedDays = schedDays.Replace("WED", "3");
            if (schedDays.Contains("THU"))
                schedDays = schedDays.Replace("THU", "4");
            if (schedDays.Contains("FRI"))
                schedDays = schedDays.Replace("FRI", "5");
            if (schedDays.Contains("SAT"))
                schedDays = schedDays.Replace("SAT", "6");
            if (schedDays.Contains("SUN"))
                schedDays = schedDays.Replace("SUN", "0");

            return schedDays;
        }

    }
}
