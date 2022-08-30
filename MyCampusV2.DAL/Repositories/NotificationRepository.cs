using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.Common.Helpers;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.Helpers;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class NotificationRepository : BaseRepository<notificationEntity>, INotificationRepository
    {
        private readonly MyCampusCardContext context;
        private ResultModel result = new ResultModel();
        private Response response = new Response();

        private ErrorLogging error = new ErrorLogging();

        public NotificationRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<notificationEntity> GetById(int id)
        {
            try
            {
                return await this.context.NotificationEntity
                .Include(x => x.TerminalEntity)
                .ThenInclude(c => c.AreaEntity)
                .ThenInclude(v => v.CampusEntity)
                .Where(q => q.Notification_ID == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                error.Error(this.GetType().Name + " --> GetById", ex.Message.ToString());
                return null;
            }
        }
        
        public async Task<notificationEntity> GetByGuid(string guid)
        {
            try
            {
                return await this.context.NotificationEntity
                    .Include(x => x.TerminalEntity)
                    .ThenInclude(c => c.AreaEntity)
                    .ThenInclude(v => v.CampusEntity)
                    .Where(q => q.GUID == guid).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                error.Error(this.GetType().Name + " --> GetByGuid", ex.Message.ToString());
                return null;
            }
        }

        public IQueryable<personEntity> GetAllActivePerson()
        {
            try
            {
                return _context.PersonEntity.Where(x => x.Person_ID != 1 && x.Person_ID != 2);
            }
            catch (Exception ex)
            {
                error.Error(this.GetType().Name + " --> GetAllActivePerson", ex.Message.ToString());
                throw ex;
            }
        }

        public async Task<ResultModel> AddNotification(notificationEntity entity, eventLoggingEntity eventEntity)
        {
            result = new ResultModel();

            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var exist = await this.context.NotificationEntity.Where(q =>
                            q.Notification_Message == entity.Notification_Message &&
                            q.Date_To_Display_From == entity.Date_To_Display_From &&
                            q.Date_To_Display_To == entity.Date_To_Display_To &&
                            q.Terminal_ID == entity.Terminal_ID).FirstOrDefaultAsync();

                    if (exist != null)
                    {
                        contextTransaction.Rollback();
                        return response.CreateResponse("409", "General Notification already exists!", false);
                    }

                    string guid = Guid.NewGuid().ToString();

                    notificationEntity newEntity = new notificationEntity
                    {
                        Person_ID = entity.Person_ID,
                        Notification_Message = entity.Notification_Message,
                        Date_To_Display_From = entity.Date_To_Display_From,
                        Date_To_Display_To = entity.Date_To_Display_To,
                        GUID = guid,
                        Terminal_ID = entity.Terminal_ID,
                        Added_By = entity.Added_By,
                        Updated_By = entity.Updated_By,
                        ToDisplay = true,
                        IsActive = true,
                        Date_Time_Added = DateTime.UtcNow.ToLocalTime(),
                        Last_Updated = DateTime.UtcNow.ToLocalTime(),
                    };

                    var notification = await this.context.NotificationEntity.AddAsync(newEntity);

                    if (notification.Entity.Notification_ID == 0)
                    {
                        eventEntity.Log_Date_Time = DateTime.UtcNow.ToLocalTime();
                        eventEntity.Source = "Add General Notification";
                        eventEntity.Message = "Failed: " + entity.Notification_Message;
                        await this.context.EventLoggingEntity.AddAsync(eventEntity);
                        return response.CreateResponse("409", "General Notification", false);
                    }

                    //await this.context.SaveChangesAsync();

                    eventEntity.Log_Date_Time = DateTime.UtcNow.ToLocalTime();
                    eventEntity.Source = "Add General Notification";
                    eventEntity.Message = "Success: " + entity.Notification_Message;
                    await _context.EventLoggingEntity.AddAsync(eventEntity);

                    await this.context.SaveChangesAsync();

                    contextTransaction.Commit();

                    return response.CreateResponse("200", "General Notification" + Constants.SuccessMessageAdd, true);
                }
                catch (Exception ex)
                {
                    contextTransaction.Rollback();
                    error.Error(this.GetType().Name + " --> AddNotification", ex.Message.ToString());
                    return response.CreateResponse("500", ex.Message, false);
                }
            }
        }

        public async Task<ResultModel> AddPersonalNotification(notificationEntity entity, eventLoggingEntity eventEntity, ICollection<terminalEntity> terminalEntity)
        {
            result = new ResultModel();

            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var exist = await this.context.NotificationEntity.Where(q =>
                            q.Person_ID == entity.Person_ID &&
                            q.Notification_Message == entity.Notification_Message &&
                            q.Date_To_Display_From == entity.Date_To_Display_From &&
                            q.Date_To_Display_To == entity.Date_To_Display_To).FirstOrDefaultAsync();

                    if (exist != null)
                    {
                        contextTransaction.Rollback();
                        return response.CreateResponse("409", "The data already exist!", false);
                    }

                    string guid = Guid.NewGuid().ToString();

                    foreach (var terminal in terminalEntity)
                    {
                        notificationEntity newEntity = new notificationEntity
                        {
                            Person_ID = entity.Person_ID,
                            Notification_Message = entity.Notification_Message,
                            Date_To_Display_From = entity.Date_To_Display_From,
                            Date_To_Display_To = entity.Date_To_Display_To,
                            GUID = guid,
                            Terminal_ID = terminal.Terminal_ID,
                            Added_By = entity.Added_By,
                            Updated_By = entity.Updated_By,
                            ToDisplay = true,
                            IsActive = true,
                            Date_Time_Added = DateTime.UtcNow.ToLocalTime(),
                            Last_Updated = DateTime.UtcNow.ToLocalTime(),
                        };

                        var notification = await this.context.NotificationEntity.AddAsync(newEntity);

                        if (notification.Entity.Notification_ID == 0)
                        {
                            eventEntity.Log_Date_Time = DateTime.UtcNow.ToLocalTime();
                            eventEntity.Source = "Add Personal Notification";
                            eventEntity.Message = "Failed: " + entity.Notification_Message;
                            await this.context.EventLoggingEntity.AddAsync(eventEntity);
                            return response.CreateResponse("409", "General Notification", false);
                        }

                        //await this.context.SaveChangesAsync();
                    }

                    eventEntity.Log_Date_Time = DateTime.UtcNow.ToLocalTime();
                    eventEntity.Source = "Add Personal Notification";
                    eventEntity.Message = "Success: " + entity.Notification_Message;
                    await _context.EventLoggingEntity.AddAsync(eventEntity);

                    await this.context.SaveChangesAsync();

                    contextTransaction.Commit();

                    return response.CreateResponse("200", "Personal Notification" + Constants.SuccessMessageAdd, true);
                }
                catch (Exception ex)
                {
                    contextTransaction.Rollback();
                    error.Error(this.GetType().Name + " --> AddPersonalNotification", ex.Message.ToString());
                    return response.CreateResponse("500", ex.Message, false);
                }
            }
        }

        public async Task<notificationPagedResultVM> GetGeneralNotificationsPagination(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var notificationEntity = new List<notificationVM>();
                int pageCount = 0;
                int rowCount = 0;
                using (MySqlConnection conn = new MySqlConnection(this.context.Database.GetDbConnection().ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "get_general_notification_list_v2";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pageNo", pageNo);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                        cmd.Parameters.AddWithValue("@keyword", keyword);

                        cmd.Parameters.AddWithValue("@rowCount", MySqlDbType.Int32);
                        cmd.Parameters["@rowCount"].Direction = ParameterDirection.Output;

                        cmd.Parameters.AddWithValue("@pageCount", MySqlDbType.Int32);
                        cmd.Parameters["@pageCount"].Direction = ParameterDirection.Output;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                notificationEntity.Add(new notificationVM()
                                {
                                    notificationId = Convert.ToInt32(reader["Notification_ID"]),
                                    notificationMessage = reader["Notification_Message"].ToString(),
                                    dateToDisplayFrom = Convert.ToDateTime(reader["Date_To_Display_From"]).ToString("yyyy-MM-dd"),
                                    dateToDisplayTo = Convert.ToDateTime(reader["Date_To_Display_To"]).ToString("yyyy-MM-dd"),
                                    guid = reader["GUID"].ToString(),
                                    terminalName = Encryption.Decrypt(Convert.ToString(reader["Terminal_Name"])),
                                    isDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                                    isActive = Convert.ToBoolean(reader["IsActive"])
                                });
                            }
                        }

                        rowCount = Convert.ToInt32(cmd.Parameters["@rowCount"].Value);
                        pageCount = Convert.ToInt32(cmd.Parameters["@pageCount"].Value);
                    }
                }

                notificationPagedResultVM notifPagedResultVM = new notificationPagedResultVM();
                notifPagedResultVM.PageCount = pageCount;
                notifPagedResultVM.RowCount = rowCount;
                notifPagedResultVM.notifications = notificationEntity;
                notifPagedResultVM.CurrentPage = pageNo;
                notifPagedResultVM.PageSize = pageSize;

                return notifPagedResultVM;

            }
            catch (Exception ex)
            {
                error.Error(this.GetType().Name + " --> GetGeneralNotificationsPagination", ex.Message.ToString());
                return null;
            }
        }

        public async Task<notificationPagedResultVM> GetPersonalNotificationsPagination(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var notificationEntity = new List<notificationVM>();
                int pageCount = 0;
                int rowCount = 0;
                using (MySqlConnection conn = new MySqlConnection(this.context.Database.GetDbConnection().ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "get_personal_notification_list";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pageNo", pageNo);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                        cmd.Parameters.AddWithValue("@keyword", keyword);

                        cmd.Parameters.AddWithValue("@rowCount", MySqlDbType.Int32);
                        cmd.Parameters["@rowCount"].Direction = ParameterDirection.Output;

                        cmd.Parameters.AddWithValue("@pageCount", MySqlDbType.Int32);
                        cmd.Parameters["@pageCount"].Direction = ParameterDirection.Output;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                notificationEntity.Add(new notificationVM()
                                {
                                    notificationMessage = reader["Notification_Message"].ToString(),
                                    dateToDisplayFrom = Convert.ToDateTime(reader["Date_To_Display_From"]).ToString("yyyy-MM-dd"),
                                    dateToDisplayTo = Convert.ToDateTime(reader["Date_To_Display_To"]).ToString("yyyy-MM-dd"),
                                    guid = reader["GUID"].ToString(),
                                    isDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                                    isActive = Convert.ToBoolean(reader["IsActive"]),
                                    firstName = Convert.ToString(reader["First_Name"]),
                                    lastName = Convert.ToString(reader["Last_Name"]),
                                    middleName = Convert.ToString(reader["Middle_Name"]),
                                    idNumber = Convert.ToString(reader["ID_Number"]),
                                    //terminalName = Encryption.Decrypt(Convert.ToString(reader["Terminal_Name"]))
                                });
                            }
                        }

                        rowCount = Convert.ToInt32(cmd.Parameters["@rowCount"].Value);
                        pageCount = Convert.ToInt32(cmd.Parameters["@pageCount"].Value);
                    }
                }

                notificationPagedResultVM notifPagedResultVM = new notificationPagedResultVM();
                notifPagedResultVM.PageCount = pageCount;
                notifPagedResultVM.RowCount = rowCount;
                notifPagedResultVM.notifications = notificationEntity;
                notifPagedResultVM.CurrentPage = pageNo;
                notifPagedResultVM.PageSize = pageSize;

                return notifPagedResultVM;

            }
            catch (Exception ex)
            {
                //error.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName, ex.Message.ToString()); 
                error.Error(this.GetType().Name + " --> GetPersonalNotificationsPagination", ex.Message.ToString());
                throw ex;
            }
        }
    }
}
