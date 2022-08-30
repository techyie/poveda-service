using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IDepartmentReportRepository : IBaseReportRepository<departmentEntity>
    {
        Task<departmentPagedResult> ExportDepartmentList(string keyword);
    }
}
