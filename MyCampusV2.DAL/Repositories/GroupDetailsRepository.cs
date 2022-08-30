using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
    public class GroupDetailsRepository : BaseRepository<fetcherGroupDetailsEntity>, IGroupDetailsRepository
    {
        private readonly MyCampusCardContext context;
        private ResultModel resultModel;

        public GroupDetailsRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<fetcherGroupDetailsPagedResult> GetAllStudentAssignedToGroup(int pageNo, int pageSize, string keyword, int groupId)
        {
            try
            {
                var result = new fetcherGroupDetailsPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if(keyword == null || keyword == "")
                    result.RowCount = this.context.FetcherGroupDetailsEntity.Where(q => q.Fetcher_Group_ID == groupId).Count();
                else
                    result.RowCount = this.context.FetcherGroupDetailsEntity.Where(q => q.Fetcher_Group_ID == groupId && 
                    (q.PersonEntity.ID_Number.Contains(keyword) || q.PersonEntity.Last_Name.Contains(keyword) || q.PersonEntity.First_Name.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.groupsDetails = await this.context.FetcherGroupDetailsEntity
                    .Include(x => x.FetcherGroupEntity)
                    .Include(x => x.PersonEntity)
                    .Where(q => q.Fetcher_Group_ID == groupId)
                    .OrderByDescending(c => c.Last_Updated)
                    .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.groupsDetails = await this.context.FetcherGroupDetailsEntity
                    .Include(x => x.FetcherGroupEntity)
                    .Include(x => x.PersonEntity)
                    .Where(q => q.Fetcher_Group_ID == groupId &&
                    (q.PersonEntity.ID_Number.Contains(keyword) || q.PersonEntity.Last_Name.Contains(keyword) || q.PersonEntity.First_Name.Contains(keyword)))
                    .OrderByDescending(c => c.Last_Updated)
                    .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<fetcherGroupDetailsEntity> GetGroupDetailById(int id, int personId)
        {
            try
            {
                return await this.context.FetcherGroupDetailsEntity
                    .Where(b => b.Fetcher_Group_ID == id && b.Person_ID == personId).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
