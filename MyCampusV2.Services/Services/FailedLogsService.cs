using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MyCampusV2.Services
{
    public class FailedLogService : IFailedLogsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public  FailedLogService(IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
        }


    }
}
