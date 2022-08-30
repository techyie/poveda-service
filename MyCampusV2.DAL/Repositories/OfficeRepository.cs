using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class OfficeRepository : BaseRepository<officeEntity>, IOfficeRepository
    {
        private readonly MyCampusCardContext context;

        public OfficeRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<IList<officeEntity>> GetOfficesUsingCampusId(int id)
        {
            try
            {
                return await this.context.OfficeEntity.Include(q => q.CampusEntity).Where(x => x.Campus_ID == id && x.IsActive == true && x.ToDisplay == true).ToListAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }
        public async Task<officeEntity> GetOfficeById(int id)
        {
            try
            {
                return await this.context.OfficeEntity.Include(q => q.CampusEntity).Where(x => x.Office_ID == id).SingleOrDefaultAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<officePagedResult> GetAllOffice(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new officePagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.OfficeEntity.Count();
                else if (keyword.ToLower().Contains("inactive") || keyword.ToLower().Contains("active"))
                    result.RowCount = _context.OfficeEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.IsActive.Equals(keyword.ToLower().Contains("inactive") ? false : true)).Count();
                else
                    result.RowCount = this.context.OfficeEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.ToDisplay == true 
                        && (q.Office_Name.Contains(keyword)
                        || q.CampusEntity.Campus_Name.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.offices = await this.context.OfficeEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                else if (keyword.ToLower().Contains("inactive") || keyword.ToLower().Contains("active"))
                    result.offices = await _context.OfficeEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.IsActive.Equals(keyword.ToLower().Contains("inactive") ? false : true))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                else
                    result.offices = await this.context.OfficeEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.ToDisplay == true 
                        && (q.Office_Name.Contains(keyword)
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

        public async Task<Boolean> UpdateWithBoolReturn(officeEntity office, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.OfficeEntity.SingleOrDefault(x => x.Office_ID == office.Office_ID);

                    if (result != null)
                    {
                        result.Office_Status = office.Office_Status.ToUpper() == "ACTIVE" ? "Active" : "Inactive";
                        result.Campus_ID = office.Campus_ID;

                        result.Updated_By = user;
                        result.Last_Updated = DateTime.Now;
                        result.IsActive = result.Office_Status.ToUpper() == "ACTIVE" ? true : false;
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

        public async Task<Boolean> AddWithBoolReturn(officeEntity office, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    office.Added_By = user;
                    office.Date_Time_Added = DateTime.Now;
                    office.Last_Updated = DateTime.Now;
                    office.Updated_By = user;
                    office.Office_Status = office.Office_Status.ToUpper() == "ACTIVE" ? "Active" : "Inactive";

                    office.IsActive = office.Office_Status.ToUpper() == "ACTIVE" ? true : false;
                    office.ToDisplay = true;

                    await _context.OfficeEntity.AddAsync(office);

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
