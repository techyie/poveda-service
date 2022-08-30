using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class EventLoggingRepository : BaseRepository<eventLoggingEntity>, IEventLoggingRepository
    {
        private readonly MyCampusCardContext context;

        public EventLoggingRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<eventLoggingPagedResult> GetAllEventLogging(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new eventLoggingPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.EventLoggingEntity.Count();
                else
                    result.RowCount = this.context.EventLoggingEntity
                        .Include(x => x.UserEntity)
                        .Include(x => x.FormEntity)
                        .Where(q =>
                        q.UserEntity.User_Name.Contains(keyword)
                        || q.Category.Contains(keyword)
                        || q.Source.Contains(keyword)).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.eventLoggingList = await this.context.EventLoggingEntity
                        .Include(x => x.UserEntity)
                        .Include(x => x.FormEntity)
                        .OrderByDescending(c => c.Log_Date_Time)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.eventLoggingList = await this.context.EventLoggingEntity
                        .Include(x => x.UserEntity)
                        .Include(x => x.FormEntity)
                        .Where(q =>
                        q.UserEntity.User_Name.Contains(keyword)
                        || q.Category.Contains(keyword)
                        || q.Source.Contains(keyword))
                        .OrderByDescending(c => c.Log_Date_Time)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

