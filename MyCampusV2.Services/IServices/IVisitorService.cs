using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{
    public interface IVisitorService
    {
        Task<ICollection<visitorInformationEntity>> getAll();
        Task<visitorInformationEntity> getById(int id);
        Task addVisitor(visitorInformationEntity visitor);
        Task updateVisitor(visitorInformationEntity visitor, int id);
        Task deleteVisitor(int id);
    }
}
