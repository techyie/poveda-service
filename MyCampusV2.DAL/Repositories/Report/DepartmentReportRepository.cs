using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.DAL.IRepositories.Report;
using MyCampusV2.Models.V2.entity;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories.Report
{
    public class DepartmentReportRepository : BaseReportRepository<departmentEntity>, IDepartmentReportRepository
    {
        private readonly MyCampusCardReportContext context;

        public DepartmentReportRepository(MyCampusCardReportContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<departmentPagedResult> ExportDepartmentList(string keyword)
        {
            var result = new departmentPagedResult();

            if (keyword == null || keyword == "")
                result.departments = await _context.DepartmentEntity
                    .Include(x => x.CampusEntity)
                    .OrderBy(c => c.Department_Name).ToListAsync();
            else
                result.departments = await _context.DepartmentEntity
                    .Include(x => x.CampusEntity)
                    .OrderBy(c => c.Department_Name)
                    .Where(q => q.Department_Name.Contains(keyword)
                    || q.CampusEntity.Campus_Name.Contains(keyword)).ToListAsync();

            return result;
        }
    }
}
