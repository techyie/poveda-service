using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace MyCampusV2.DAL.Repositories
{
    public class PositionRepository : BaseRepository<positionEntity>, IPositionRepository
    {
        private readonly MyCampusCardContext context;

        public PositionRepository(MyCampusCardContext Context) : base(Context)
        {
            this.context = Context;
        }

        public async Task<IList<positionEntity>> GetPositionsUsingDepartmentId(int id)
        {
            try
            {
                return await this.context.PositionEntity.Include(q => q.DepartmentEntity).Where(x => x.Department_ID == id && x.IsActive == true && x.ToDisplay == true).ToListAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<positionEntity> GetPositionById(int id)
        {
            try
            {
                return await this.context.PositionEntity
                    .Include(q => q.DepartmentEntity)
                    .ThenInclude(x => x.CampusEntity)
                    .Where(x => x.Position_ID == id)
                    .SingleOrDefaultAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<positionPagedResult> GetAllPosition(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new positionPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = _context.PositionEntity
                        .Where(a => a.ToDisplay == true)
                        .Count();
                else
                    result.RowCount = _context.PositionEntity
                        .Include(x => x.DepartmentEntity)
                        .ThenInclude(x => x.CampusEntity)
                        .Where(q => q.ToDisplay == true && (q.Position_Name.Contains(keyword)
                        || q.Position_Name.Contains(keyword)
                        || q.DepartmentEntity.Department_Name.Contains(keyword)
                        || q.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.positions = await _context.PositionEntity
                        .Include(x => x.DepartmentEntity)
                        .ThenInclude(x => x.CampusEntity)
                        .Where(c => c.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.positions = await _context.PositionEntity
                        .Include(x => x.DepartmentEntity)
                        .ThenInclude(x => x.CampusEntity)
                        .Where(q => q.ToDisplay == true && (q.Position_Name.Contains(keyword)
                        || q.Position_Name.Contains(keyword)
                        || q.DepartmentEntity.Department_Name.Contains(keyword)
                        || q.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<positionPagedResult> ExportAllPositions(string keyword)
        {
            try
            {
                var result = new positionPagedResult();

                if (keyword == null || keyword == "")
                {
                    result.positions = await _context.PositionEntity
                        .Include(q => q.DepartmentEntity)
                        .ThenInclude(x => x.CampusEntity)
                        .Where(b => b.ToDisplay == true)
                        .OrderBy(e => e.Position_ID).ToListAsync();
                }
                else
                {
                    result.positions = await _context.PositionEntity
                        .Include(a => a.DepartmentEntity)
                        .ThenInclude(b => b.CampusEntity)
                        .Where(c => c.ToDisplay == true && (c.Position_Name.Contains(keyword)
                            || c.DepartmentEntity.Department_Name.Contains(keyword)
                            || c.DepartmentEntity.CampusEntity.Campus_Name.Contains(keyword)))
                       .OrderBy(e => e.Position_ID).ToListAsync();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Boolean> UpdatePositionWithBoolReturn(positionEntity position, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.PositionEntity.SingleOrDefault(x => x.Position_ID == position.Position_ID);

                    if (result != null)
                    {
                        result.Position_Name = position.Position_Name;
                        result.Department_ID = position.Department_ID;

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

        public async Task<Boolean> AddPositionWithBoolReturn(positionEntity position, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    position.Added_By = user;
                    position.Date_Time_Added = DateTime.Now;
                    position.Last_Updated = DateTime.Now;
                    position.Updated_By = user;

                    await _context.PositionEntity.AddAsync(position);

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