using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Services.Services
{
    public class PAPAccountLinkedStudentsService : BaseService, IPAPAccountLinkedStudentsService
    {
        private string _areaBatch = AppDomain.CurrentDomain.BaseDirectory + @"PapAccount\";
        private ResultModel result = new ResultModel();

        public PAPAccountLinkedStudentsService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user)
            : base(unitOfWork, audit, user)
        {

        }
    }
}
