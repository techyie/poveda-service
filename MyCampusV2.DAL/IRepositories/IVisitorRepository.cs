using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IVisitorRepository : IBaseRepository<visitorInformationEntity>
    {
        Task<ICollection<visitorInformationEntity>> GetAllVisitorsAsync();
    }
}