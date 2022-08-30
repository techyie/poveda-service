using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using System;

namespace MyCampusV2.Services
{
    public class AuditTrailService : IAuditTrailService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuditTrailService(IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
        }

        public void Audit(int user, int form, string message)
        {
           auditTrailEntity aud = new auditTrailEntity { User_ID = user, Action = message, Form_ID = form, Date = DateTime.Now };
           _unitOfWork.AuditTrailRepository.Audit(aud);
        }
     
    }
}
