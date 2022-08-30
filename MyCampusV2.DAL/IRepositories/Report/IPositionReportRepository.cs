using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories.Report
{
    public interface IPositionReportRepository : IBaseReportRepository<positionEntity>
    {
        Task<positionPagedResult> ExportPositions(string keyword);
    }
}
