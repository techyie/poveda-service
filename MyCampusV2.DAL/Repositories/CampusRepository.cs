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
    public class CampusRepository : BaseRepository<campusEntity>, ICampusRepository
    {
        private readonly MyCampusCardContext context;

        public CampusRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<campusPagedResult> GetAllCampuses(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new campusPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = _context.CampusEntity.Count();

                else if (keyword.ToLower().Contains("inactive") || keyword.ToLower().Contains("active"))
                    result.RowCount = _context.CampusEntity
                        .Where(q => q.IsActive.Equals(keyword.ToLower().Contains("inactive") ? false : true)).Count();

                else
                    result.RowCount = _context.CampusEntity.Where(q => q.ToDisplay == true 
                        && (q.Campus_Name.Contains(keyword)
                        || q.Campus_Code.Contains(keyword)
                        || q.Campus_Address.Contains(keyword)
                        || q.Campus_ContactNo.Contains(keyword)
                        || q.DivisionEntity.Name.Contains(keyword)
                        || q.DivisionEntity.RegionEntity.Name.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.campuses = await _context.CampusEntity
                        .Include(x => x.DivisionEntity)
                        .ThenInclude(z => z.RegionEntity)
                        .Where(q => q.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                else if (keyword.ToLower().Contains("inactive") || keyword.ToLower().Contains("active"))
                    result.campuses = await _context.CampusEntity
                        .Where(q => q.IsActive.Equals(keyword.ToLower().Contains("inactive") ? false : true))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                else
                    result.campuses = await this.context.CampusEntity
                       .Include(q => q.DivisionEntity)
                       .ThenInclude(x => x.RegionEntity)
                       .Where(q => q.ToDisplay == true 
                       && (q.Campus_Name.Contains(keyword)
                        || q.Campus_Code.Contains(keyword)
                        || q.Campus_Address.Contains(keyword)
                        || q.Campus_ContactNo.Contains(keyword)
                        || q.DivisionEntity.Name.Contains(keyword)
                        || q.DivisionEntity.RegionEntity.Name.Contains(keyword)))
                       .OrderByDescending(c => c.Last_Updated).Skip(skip).Take(pageSize).ToListAsync();



                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ICollection<regionEntity>> GetRegion()
        {
            return await _context.RegionEntity.ToListAsync();
        }
        public async Task<ICollection<divisionEntity>> GetDivisionByRegion(int id)
        {
            return await _context.DivisionEntity.Where(d => d.Region_ID == id).ToListAsync();
        }

        public async Task<campusEntity> GetCampusByID(int id)
        {
            return await _context.CampusEntity.Include(x => x.DivisionEntity).ThenInclude(z => z.RegionEntity).Where(c => c.Campus_ID == id).FirstOrDefaultAsync();
        }

        public IQueryable<campusEntity> GetCampuses()
        {
            try
            {
                return _context.CampusEntity
                    .Include(a => a.DivisionEntity)
                    .ThenInclude(b => b.RegionEntity)
                    .Where(c => c.IsActive == true && c.ToDisplay == true);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ICollection<educationalLevelEntity>> GetCampusIfActive(int id)
        {
            return await _context.EducationalLevelEntity.Where(ed => ed.Campus_ID == id && ed.IsActive == true).ToListAsync();
        }

        public async Task<Boolean> UpdateWithBoolReturn(campusEntity campus, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.CampusEntity.SingleOrDefault(x => x.Campus_ID == campus.Campus_ID);

                    if (result != null)
                    {
                        result.Campus_Status = campus.Campus_Status.ToUpper() == "ACTIVE" ? "Active" : "Inactive";
                        result.Campus_ID = campus.Campus_ID;

                        result.Updated_By = user;
                        result.Last_Updated = DateTime.Now;
                        result.IsActive = result.Campus_Status.ToUpper() == "ACTIVE" ? true : false;
                        result.ToDisplay = true;
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

        public async Task<Boolean> AddWithBoolReturn(campusEntity campus, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    campus.Added_By = user;
                    campus.Date_Time_Added = DateTime.Now;
                    campus.Last_Updated = DateTime.Now;
                    campus.Updated_By = user;
                    campus.Campus_Status = campus.Campus_Status.ToUpper() == "ACTIVE" ? "Active" : "Inactive";

                    campus.IsActive = campus.Campus_Status.ToUpper() == "ACTIVE" ? true : false;
                    campus.ToDisplay = true;

                    await _context.CampusEntity.AddAsync(campus);

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
