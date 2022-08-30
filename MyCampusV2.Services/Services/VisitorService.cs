using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using MyCampusV2.Models;
using System.Threading.Tasks;
using MyCampusV2.DAL.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using MyCampusV2.IServices;
using MyCampusV2.Common;
using System.Linq;

namespace MyCampusV2.Services.Services
{
    //public class VisitorService : BaseService, IVisitorService
    //{
    //    public VisitorService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

    //    private IQueryable<Visitor> GetData(IQueryable<Visitor> query)
    //    {
    //        return !_user.MasterAccess ? query.Where(o => o.Campus_ID == _user.Campus) : query;
    //    }

    //    public Task addVisitor(VisitorInformation visitor)
    //    {
    //        return _unitOfWork.VisitorRepository.AddAsyn(visitor);
    //    }

    //    public Task deleteVisitor(int id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public async Task<ICollection<VisitorInformation>> getAll()
    //    {
    //        return await _unitOfWork.VisitorRepository.GetAllAsyn();
    //    }

    //    public async Task<ICollection<Visitor>> GetAllVisitors()
    //    {
    //        return await _unitOfWork.VisitorRepository.GetAllVisitors().ToListAsync();
    //    }

    //    public Task<VisitorInformation> getById(int id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task updateVisitor(VisitorInformation visitor, int id)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
