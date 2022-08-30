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
    public class AreaRepository : BaseRepository<areaEntity>, IAreaRepository
    {
        private readonly MyCampusCardContext context;
        private ResultModel resultModel;

        public AreaRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<IList<areaEntity>> GetAreasUsingCampusId(int id)
        {
            return await this.context.AreaEntity.Where(x => x.Campus_ID == id && x.IsActive == true && x.ToDisplay == true).ToListAsync();
        }

        public async Task<IList<areaEntity>> GetAreasUsingCampusName(string campus)
        {
            return await this.context.AreaEntity.Include(q => q.CampusEntity).Where(x => x.CampusEntity.Campus_Name == campus && x.IsActive == true && x.ToDisplay == true).ToListAsync();
        }

        public async Task<areaEntity> GetAreaById(int id)
        {
            try
            {
                return await this.context.AreaEntity
                    .Include(x => x.CampusEntity)
                    .Where(x => x.Area_ID == id).SingleOrDefaultAsync();
            }
            catch(Exception err)
            {
                return null;
            }
        }

        public async Task<areaPagedResult> GetAllArea(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new areaPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.AreaEntity
                        .Include(x => x.CampusEntity)
                        .Where(a => a.ToDisplay == true)
                        .Count();
                else
                    result.RowCount = this.context.AreaEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.ToDisplay == true && (q.Area_Name.Contains(keyword)
                        || q.Area_Description.Contains(keyword)
                        || q.CampusEntity.Campus_Name.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.areas = await this.context.AreaEntity
                        .Include(x => x.CampusEntity)
                        .Where(a => a.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.areas = await this.context.AreaEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.ToDisplay == true && (q.Area_Name.Contains(keyword)
                        || q.Area_Description.Contains(keyword)
                        || q.CampusEntity.Campus_Name.Contains(keyword)))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<areaPagedResult> ExportAllAreas(string keyword)
        {
            try
            {
                var result = new areaPagedResult();

                if (keyword == null || keyword == "")
                {
                    result.areas = await _context.AreaEntity
                        .Include(a => a.CampusEntity)
                        .Where(b => b.ToDisplay == true)
                        .OrderBy(c => c.Area_ID).ToListAsync();
                }
                else
                {
                    result.areas = await _context.AreaEntity
                        .Include(a => a.CampusEntity)
                        .Where(b => b.ToDisplay == true && (b.Area_Name.Contains(keyword)
                            || b.Area_Description.Contains(keyword)
                            || b.CampusEntity.Campus_Name.Contains(keyword)))
                       .OrderBy(c => c.Area_ID).ToListAsync();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Boolean> UpdateAreaWithBoolReturn(areaEntity area, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.AreaEntity.SingleOrDefault(x => x.Area_ID == area.Area_ID);

                    if (result != null)
                    {
                        result.Area_Name = area.Area_Name;
                        result.Area_Description = area.Area_Description;
                        result.Campus_ID = area.Campus_ID;

                        result.Updated_By = user;
                        result.Last_Updated = DateTime.Now;
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

        public async Task<Boolean> AddAreaWithBoolReturn(areaEntity area, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    area.Added_By = user;
                    area.Date_Time_Added = DateTime.Now;
                    area.Last_Updated = DateTime.Now;
                    area.Updated_By = user;

                    await _context.AreaEntity.AddAsync(area);

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
    }
}
