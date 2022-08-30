using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class SectionScheduleRepository : BaseRepository<sectionScheduleEntity>, ISectionScheduleRepository
    {
        private readonly MyCampusCardContext context;

        public SectionScheduleRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }
        
        public async Task<sectionSchedulePagedResult> GetAllSchedule(int sectionId, int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new sectionSchedulePagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                DateTime dateValue;
                DateTime.TryParse(keyword, out dateValue);

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.SectionScheduleEntity.Where(q => q.StudSec_ID == sectionId && q.IsActive == true).Count();
                else if (keyword.ToUpper().Contains("YES") || keyword.ToUpper().Contains("NO"))
                    result.RowCount = this.context.SectionScheduleEntity
                        .Where(q => q.StudSec_ID == sectionId
                            && q.IsActive == true
                            && q.IsExcused == (keyword.ToUpper().Contains("YES") == true || keyword.ToUpper().Contains("NO") == false))
                        .Count();
                else
                    result.RowCount = this.context.SectionScheduleEntity
                        .Where(q => q.StudSec_ID == sectionId
                            && q.IsActive == true
                            && q.Schedule_Date.Date == dateValue.Date)
                        .Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.schedules = await _context.SectionScheduleEntity
                        .Where(q => q.StudSec_ID == sectionId
                            && q.IsActive == true)
                        .OrderByDescending(c => c.Schedule_Date)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else if (keyword.ToUpper().Contains("YES") || keyword.ToUpper().Contains("NO"))
                    result.schedules = await _context.SectionScheduleEntity
                        .Where(q => q.StudSec_ID == sectionId
                            && q.IsActive == true
                            && q.IsExcused == (keyword.ToUpper().Contains("YES") == true || keyword.ToUpper().Contains("NO") == false))
                        .OrderByDescending(c => c.Schedule_Date)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.schedules = await _context.SectionScheduleEntity
                        .Where(q => q.StudSec_ID == sectionId
                            && q.IsActive == true
                            && q.Schedule_Date.Date == dateValue.Date)
                        .OrderByDescending(c => c.Schedule_Date)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<sectionScheduleEntity> GetSectionScheduleById(int id)
        {
            try
            {
                return await this.context.SectionScheduleEntity
                        .Include(x => x.StudentSectionEntity)
                        .Where(z => z.ID == id).SingleOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
