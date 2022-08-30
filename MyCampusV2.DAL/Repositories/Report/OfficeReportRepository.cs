using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.Repositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories.Report
{
    public class OfficeReportRepository : BaseReportRepository<officeEntity>, IOfficeReportRepository
    {
        private readonly MyCampusCardReportContext context;

        public OfficeReportRepository(MyCampusCardReportContext Context)
                : base(Context)
        {
            this.context = Context;
            // use this to set command timeout 
            //this.context.Database.SetCommandTimeout(300);
        }

        public async Task<officePagedResult> ExportOffice(string keyword)
        {
            var result = new officePagedResult();
            try
            {
                if (keyword == null || keyword == "")
                    result.offices = await _context.OfficeEntity
                        .Include(x => x.CampusEntity)
                        .Where(q => q.ToDisplay == true)
                        .OrderBy(c => c.Office_Name).ToListAsync();
                else
                    result.offices = await _context.OfficeEntity
                        .Include(x => x.CampusEntity)
                        .OrderBy(c => c.Office_Name)
                        .Where(q => q.ToDisplay == true && (q.Office_Name.Contains(keyword)
                        || q.CampusEntity.Campus_Name.Contains(keyword))).ToListAsync();

                return result;

            } catch (Exception err)
            {
                return null;
            }
        }

    }
}
