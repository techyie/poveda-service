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
    public class GroupRepository : BaseRepository<fetcherGroupEntity>, IGroupRepository
    {
        private readonly MyCampusCardContext context;
        private ResultModel resultModel;

        public GroupRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<fetcherGroupPagedResult> GetAllGroups(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new fetcherGroupPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.FetcherGroupEntity.Where(q => q.IsActive == true).Count();
                else
                    result.RowCount = this.context.FetcherGroupEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.IsActive == true && q.Group_Name.Contains(keyword)).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.groups = await this.context.FetcherGroupEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.IsActive == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.groups = await this.context.FetcherGroupEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.IsActive == true && q.Group_Name.Contains(keyword))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IQueryable<fetcherGroupEntity> GetGroups()
        {
            try
            {
                return _context.FetcherGroupEntity
                    .Where(c => c.IsActive == true && c.ToDisplay == true);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<fetcherGroupEntity> GetGroupByID(int id)
        {
            return await _context.FetcherGroupEntity
                .Include(x => x.CampusEntity)
                .Where(c => c.Fetcher_Group_ID == id).FirstOrDefaultAsync();
        }
    }
}
