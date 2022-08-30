using System;
using System.Collections.Generic;
using System.Linq;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.DAL.Context;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;

namespace MyCampusV2.DAL.Repositories
{
    public class DepartmentRepository : BaseRepository<departmentEntity>, IDepartmentRepository
    {
        private readonly MyCampusCardContext context;

        public DepartmentRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<IList<departmentEntity>> GetDepartmentsUsingCampusId(int id)
        {
            try
            {
                return await this.context.DepartmentEntity.Include(q => q.CampusEntity).Where(x => x.Campus_ID == id && x.IsActive == true && x.ToDisplay == true).ToListAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<departmentEntity> GetDepartmentById(int id)
        {
            try
            {
                return await this.context.DepartmentEntity.Include(q => q.CampusEntity).Where(x => x.Department_ID == id).SingleOrDefaultAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<departmentPagedResult> GetAllDepartment(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new departmentPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = _context.DepartmentEntity
                        .Include(x => x.CampusEntity)
                        .Where(a => a.ToDisplay == true)
                        .Count();
                else
                    result.RowCount = _context.DepartmentEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.ToDisplay == true && (q.Department_Name.Contains(keyword)
                        || q.CampusEntity.Campus_Name.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.departments = await _context.DepartmentEntity
                        .Include(x => x.CampusEntity)
                        .Where(a => a.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.departments = await _context.DepartmentEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.ToDisplay == true && (q.Department_Name.Contains(keyword)
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

        public async Task<departmentPagedResult> ExportAllDepartments(string keyword)
        {
            try
            {
                var result = new departmentPagedResult();

                if (keyword == null || keyword == "")
                {
                    result.departments = await _context.DepartmentEntity
                        .Include(a => a.CampusEntity)
                        .Where(b => b.ToDisplay == true)
                        .OrderBy(c => c.Department_ID).ToListAsync();
                }
                else
                {
                    result.departments = await _context.DepartmentEntity
                        .Include(a => a.CampusEntity)
                        .Where(b => b.ToDisplay == true && (b.Department_Name.Contains(keyword)
                            || b.CampusEntity.Campus_Name.Contains(keyword)))
                       .OrderBy(c => c.Department_ID).ToListAsync();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Boolean> UpdateDepartmentWithBoolReturn(departmentEntity department, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _context.DepartmentEntity.SingleOrDefault(x => x.Department_ID == department.Department_ID);

                    if (result != null)
                    {
                        result.Department_Name = department.Department_Name;
                        result.Campus_ID = department.Campus_ID;

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

        public async Task<Boolean> AddDepartmentWithBoolReturn(departmentEntity department, int user)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    department.Added_By = user;
                    department.Date_Time_Added = DateTime.Now;
                    department.Last_Updated = DateTime.Now;
                    department.Updated_By = user;

                    await _context.DepartmentEntity.AddAsync(department);

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
