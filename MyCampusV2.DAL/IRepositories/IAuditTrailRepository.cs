using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IAuditTrailRepository : IBaseRepository<auditTrailEntity>
    {
        void Audit(auditTrailEntity auditTrail);
        Task AuditAsync(int user, int form, string message);
    }
}
