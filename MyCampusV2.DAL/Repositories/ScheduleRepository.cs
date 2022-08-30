using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCampusV2.Models.V2.entity;
using Microsoft.EntityFrameworkCore.Storage;
using MySql.Data.MySqlClient;
using System.Data;

namespace MyCampusV2.DAL.Repositories
{
    public class ScheduleRepository : BaseRepository<scheduleEntity>, IScheduleRepository
    {
        private readonly MyCampusCardContext context;

        public ScheduleRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<schedulePagedResult> GetAllSchedule(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new schedulePagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.ScheduleEntity.Count();
                else
                    result.RowCount = this.context.ScheduleEntity
                        .Where(q =>
                        q.Schedule_Name.Contains(keyword)
                        || q.Schedule_Days.Contains(keyword)
                        || q.Schedule_Status.Contains(keyword)).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.schedules = await this.context.ScheduleEntity.OrderByDescending(c => c.Last_Updated).Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.schedules = await this.context.ScheduleEntity
                        .Where(q =>
                        q.Schedule_Name.Contains(keyword)
                        || q.Schedule_Days.Contains(keyword)
                        || q.Schedule_Status.Contains(keyword))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<ICollection<scheduleEntity>> GetSchedules()
        {
            var test = await _context.ScheduleEntity.Where(c => c.IsActive == true && c.ToDisplay == true).ToListAsync();
            return await _context.ScheduleEntity.Where(c => c.IsActive == true && c.ToDisplay == true).ToListAsync();
        }

        public IQueryable<fetcherGroupDetailsEntity> GetGroupsByScheduleGroupId(int groupId)
        {
            try
            {
                return _context.FetcherGroupDetailsEntity
                    .Where(c => c.Fetcher_Group_ID == groupId && c.IsActive == true && c.ToDisplay == true);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<scheduleEntity> GetScheduleByID(int id)
        {
            return await _context.ScheduleEntity.Where(c => c.Schedule_ID == id).FirstOrDefaultAsync();
        }

        public async Task<fetcherSchedulePagedResult> GetScheduleByFetcherID(string id, int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new fetcherSchedulePagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                {
                    result.RowCount = _context.FetcherScheduleEntity
                        .Include(a => a.ScheduleEntity)
                        .Where(x => x.Fetcher_ID.ToString() == id && x.IsActive == true)
                        .Count();
                }
                else
                {
                    result.RowCount = _context.FetcherScheduleEntity
                        .Include(a => a.ScheduleEntity)
                        .Where(x => x.Fetcher_ID.ToString() == id 
                            && x.IsActive == true 
                            && x.ScheduleEntity.Schedule_Name.Contains(keyword))
                        .Count();
                }

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                {
                    result.fetcherSchedules = await _context.FetcherScheduleEntity
                        .Include(a => a.ScheduleEntity)
                        .Where(x => x.Fetcher_ID.ToString() == id && x.IsActive == true && x.ToDisplay == true)
                        .OrderBy(e => e.Fetcher_Sched_ID)
                        .Skip(skip).Take(pageSize).ToListAsync();
                }
                else
                {
                    result.fetcherSchedules = await _context.FetcherScheduleEntity
                        .Include(a => a.ScheduleEntity)
                        .Where(x => x.Fetcher_ID.ToString() == id 
                            && x.IsActive == true 
                            && x.ScheduleEntity.Schedule_Name.Contains(keyword))
                        .OrderBy(e => e.Fetcher_Sched_ID)
                        .Skip(skip).Take(pageSize).ToListAsync();
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<fetcherScheduleDetailsPagedResult> GetStudentByFetcherScheduleID(string id, int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new fetcherScheduleDetailsPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                {
                    result.RowCount = _context.FetcherScheduleDetailsEntity
                        .Include(b => b.FetcherScheduleEntity)
                        .Include(c => c.PersonEntity)
                        .Where(x => x.Fetcher_Sched_ID.ToString() == id 
                            && x.IsActive == true
                            && x.Fetcher_Group_ID == 0)
                        .Count();
                }
                else
                {
                    result.RowCount = _context.FetcherScheduleDetailsEntity
                        .Include(b => b.FetcherScheduleEntity)
                        .Include(c => c.PersonEntity)
                        .Where(x => x.Fetcher_Sched_ID.ToString() == id && x.IsActive == true
                            && (x.PersonEntity.ID_Number.Contains(keyword)
                            || x.PersonEntity.Last_Name.Contains(keyword)
                            || x.PersonEntity.First_Name.Contains(keyword))
                            && x.Fetcher_Group_ID == 0)
                        .Count();
                }

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                {
                    result.fetcherStudents = await _context.FetcherScheduleDetailsEntity
                        .Include(b => b.FetcherScheduleEntity)
                        .Include(c => c.PersonEntity)
                        .Where(x => x.Fetcher_Sched_ID.ToString() == id 
                            && x.IsActive == true
                            && x.Fetcher_Group_ID == 0)
                        .OrderBy(e => e.PersonEntity.ID_Number)
                        .Skip(skip).Take(pageSize).ToListAsync();
                }
                else
                {
                    result.fetcherStudents = await _context.FetcherScheduleDetailsEntity
                        .Include(b => b.FetcherScheduleEntity)
                        .Include(c => c.PersonEntity)
                        .Where(x => x.Fetcher_Sched_ID.ToString() == id && x.IsActive == true
                            && (x.PersonEntity.ID_Number.Contains(keyword)
                            || x.PersonEntity.Last_Name.Contains(keyword)
                            || x.PersonEntity.First_Name.Contains(keyword))
                            && x.Fetcher_Group_ID == 0)
                        .OrderBy(e => e.PersonEntity.ID_Number)
                        .Skip(skip).Take(pageSize).ToListAsync();
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //public async Task<fetcherScheduleDetailsPagedResult> GetGroupByFetcherScheduleID(string id, int pageNo, int pageSize, string keyword)
        //{
        //    try
        //    {
        //        var result = new fetcherScheduleDetailsPagedResult();
        //        result.CurrentPage = pageNo;
        //        result.PageSize = pageSize;

        //        if (keyword == null || keyword == "")
        //        {
        //            result.RowCount = _context.FetcherScheduleDetailsEntity
        //                .Include(a => a.FetcherGroupEntity)
        //                .Include(b => b.FetcherScheduleEntity)
        //                .Where(x => x.Fetcher_Sched_ID.ToString() == id 
        //                    && x.IsActive == true
        //                    && x.Fetcher_Group_ID != 0)
        //                .Count();
        //        }
        //        else
        //        {
        //            result.RowCount = _context.FetcherScheduleDetailsEntity
        //                .Include(a => a.FetcherGroupEntity)
        //                .Include(b => b.FetcherScheduleEntity)
        //                .Where(x => x.Fetcher_Sched_ID.ToString() == id && x.IsActive == true
        //                    && x.FetcherGroupEntity.Group_Name.Contains(keyword)
        //                    && x.Fetcher_Group_ID != 0)
        //                .Count();
        //        }

        //        var pageCount = (double)result.RowCount / pageSize;
        //        result.PageCount = (int)Math.Ceiling(pageCount);

        //        var skip = (pageNo - 1) * pageSize;

        //        if (keyword == null || keyword == "")
        //        {
        //            result.fetcherStudents = await _context.FetcherScheduleDetailsEntity
        //                .Include(a => a.FetcherGroupEntity)
        //                .Include(b => b.FetcherScheduleEntity)
        //                .Where(x => x.Fetcher_Sched_ID.ToString() == id 
        //                    && x.IsActive == true
        //                    && x.Fetcher_Group_ID != 0)
        //                .OrderBy(e => e.FetcherGroupEntity.Group_Name)
        //                .Skip(skip).Take(pageSize).ToListAsync();
        //        }
        //        else
        //        {
        //            result.fetcherStudents = await _context.FetcherScheduleDetailsEntity
        //                .Include(a => a.FetcherGroupEntity)
        //                .Include(b => b.FetcherScheduleEntity)
        //                .Where(x => x.Fetcher_Sched_ID.ToString() == id && x.IsActive == true
        //                    && x.Fetcher_Group_ID != 0
        //                    && x.FetcherGroupEntity.Group_Name.Contains(keyword))
        //                .OrderBy(x => x.FetcherGroupEntity.Group_Name)
        //                .Skip(skip).Take(pageSize).ToListAsync();
        //        }

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
        
        public async Task<fetcherScheduleDetailsPagedResultVM> GetGroupByFetcherScheduleID(string id, int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new List<fetcherScheduleDetailsVM>();
                int pageCount = 0;
                int rowCount = 0;
                using (MySqlConnection conn = new MySqlConnection(this.context.Database.GetDbConnection().ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "get_fetcher_schedule_group_list";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@fetcherSchedId", id);
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
                                result.Add(new fetcherScheduleDetailsVM()
                                {
                                    fetcherSchedId = Convert.ToInt32(reader["Fetcher_Sched_ID"]),
                                    fetcherGroupId = Convert.ToInt32(reader["Fetcher_Group_ID"]),
                                    groupName = reader["Group_Name"].ToString()
                                });
                            }
                        }

                        rowCount = Convert.ToInt32(cmd.Parameters["@rowCount"].Value);
                        pageCount = Convert.ToInt32(cmd.Parameters["@pageCount"].Value);
                    }
                }

                fetcherScheduleDetailsPagedResultVM fetchersched = new fetcherScheduleDetailsPagedResultVM();
                fetchersched.PageCount = pageCount;
                fetchersched.RowCount = rowCount;
                fetchersched.fetcherStudents = result;
                fetchersched.CurrentPage = pageNo;
                fetchersched.PageSize = pageSize;

                return fetchersched;

            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<fetcherScheduleEntity> GetFetcherScheduleByID(int id)
        {
            return await _context.FetcherScheduleEntity.Where(c => c.Fetcher_Sched_ID == id).FirstOrDefaultAsync();
        }

        public async Task<fetcherScheduleEntity> GetFetcherSchedule(int fetcherId, int scheduleId)
        {
            try
            {
                return await this.context.FetcherScheduleEntity
                    .Where(b => b.Fetcher_ID == fetcherId && b.Schedule_ID == scheduleId && b.IsActive == true && b.ToDisplay == true).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task AddFetcherSchedule(fetcherScheduleEntity entity)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    entity.Date_Time_Added = DateTime.Now;
                    entity.Last_Updated = DateTime.Now;

                    entity.IsActive = true;
                    entity.ToDisplay = true;

                    await _context.FetcherScheduleEntity.AddAsync(entity);

                    await _context.SaveChangesAsync();

                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task DeleteFetcherSchedule(fetcherScheduleEntity entity, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.FetcherScheduleEntity.SingleOrDefault(x => x.Fetcher_Sched_ID == entity.Fetcher_Sched_ID);

                    if (result != null)
                    {
                        result.IsActive = false;
                        result.ToDisplay = false;
                        result.Updated_By = user;
                        result.Last_Updated = DateTime.Now;
                    }
                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<fetcherScheduleDetailsEntity> GetFetcherScheduleStudent(int scheduleId, int groupId, int personId)
        {
            try
            {
                if (groupId == 0)
                {
                    return await this.context.FetcherScheduleDetailsEntity
                        .Where(b => b.Fetcher_Sched_ID == scheduleId && b.Person_ID == personId && b.IsActive == true).SingleOrDefaultAsync();
                }
                else
                {
                    return await this.context.FetcherScheduleDetailsEntity
                        .Where(b => b.Fetcher_Sched_ID == scheduleId && b.Fetcher_Group_ID == groupId && b.IsActive == true).SingleOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<fetcherScheduleDetailsEntity> GetFetcherScheduleStudentByID(int id)
        {
            return await _context.FetcherScheduleDetailsEntity.Where(c => c.Fetcher_Sched_Dtl_ID == id).FirstOrDefaultAsync();
        }

        public async Task<List<fetcherScheduleDetailsEntity>> GetFetcherScheduleGroup(int id)
        {
            return await _context.FetcherScheduleDetailsEntity.Where(c => c.Fetcher_Group_ID == id && c.IsActive == true).ToListAsync();
        }

        public IQueryable<fetcherScheduleDetailsEntity> GetFetcherScheduleStudentByGroupIDAndPersonID(int groupId, int personId)
        {
            try
            {
                return _context.FetcherScheduleDetailsEntity.Where(c => c.Fetcher_Group_ID == groupId && c.Person_ID == personId && c.IsActive == true);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IQueryable<fetcherScheduleDetailsEntity> GetFetcherScheduleStudentByGroupID(int groupId)
        {
            try
            {
                return _context.FetcherScheduleDetailsEntity.Where(c => c.Fetcher_Group_ID == groupId && c.IsActive == true).GroupBy(d => d.Fetcher_Sched_ID).Select(g => g.First());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task AddFetcherScheduleStudent(fetcherScheduleDetailsEntity entity) 
        {
            try
            {
                await _context.FetcherScheduleDetailsEntity.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteFetcherScheduleStudent(fetcherScheduleDetailsEntity entity, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (!entity.Fetcher_Group_ID.Equals(0))
                    {
                        var result = _context.FetcherScheduleDetailsEntity.SingleOrDefault(x => x.Fetcher_Sched_Dtl_ID == entity.Fetcher_Sched_Dtl_ID && x.Fetcher_Group_ID == entity.Fetcher_Group_ID);

                        if (result != null)
                        {
                            result.IsActive = false;
                            result.ToDisplay = false;
                            result.Updated_By = user;
                            result.Last_Updated = DateTime.Now;
                        }
                    }
                    else
                    {
                        var result = _context.FetcherScheduleDetailsEntity.SingleOrDefault(x => x.Fetcher_Sched_Dtl_ID == entity.Fetcher_Sched_Dtl_ID);

                        if (result != null)
                        {
                            result.IsActive = false;
                            result.ToDisplay = false;
                            result.Updated_By = user;
                            result.Last_Updated = DateTime.Now;
                        }
                    }

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task DeleteFetcherScheduleGroup(List<fetcherScheduleDetailsEntity> entity, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in entity)
                    {
                        var result = _context.FetcherScheduleDetailsEntity.SingleOrDefault(x => x.Fetcher_Sched_Dtl_ID == item.Fetcher_Sched_Dtl_ID);

                        if (result != null)
                        {
                            result.IsActive = false;
                            result.ToDisplay = false;
                            result.Updated_By = user;
                            result.Last_Updated = DateTime.Now;
                        }
                    }

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
