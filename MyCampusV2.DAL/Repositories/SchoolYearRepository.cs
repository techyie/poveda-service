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
    public class SchoolYearRepository : BaseRepository<schoolYearEntity>, ISchoolYearRepository
    {
        private readonly MyCampusCardContext context;

        public SchoolYearRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<schoolYearPagedResult> GetAll(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new schoolYearPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.SchoolYearEntity.Count();
                else
                    result.RowCount = this.context.SchoolYearEntity
                        .Where(q => q.School_Year.Contains(keyword)).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.schoolyear = await _context.SchoolYearEntity
                        .OrderByDescending(c => c.School_Year)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.schoolyear = await _context.SchoolYearEntity
                        .Where(q => q.School_Year.Contains(keyword))
                        .OrderByDescending(c => c.School_Year)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<schoolYearPagedResult> GetFiltered()
        {
            try
            {
                var result = new schoolYearPagedResult();
 
                result.schoolyear = await _context.SchoolYearEntity
                    .Where(q => q.Start_Date != null && q.End_Date != null)
                    .OrderByDescending(c => c.School_Year).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<schoolYearEntity> GetById(int id)
        {
            try
            {
                return await this.context.SchoolYearEntity
                        .Where(z => z.School_Year_ID == id).SingleOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
