using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IPersonReportRepository : IBaseReportRepository<personEntity>
    {
        Task<studentPagedResult> ExportAllStudents(string keyword, bool isCollege);
        Task<visitorPagedResult> ExportAllVisitors(string keyword);
        Task<employeePagedResult> ExportAllEmployees(string keyword);
    }
}
