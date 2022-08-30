using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.IServices
{
    public interface IAuditTrailService
    {
       void Audit(int user, int form, string message);
    }
}
