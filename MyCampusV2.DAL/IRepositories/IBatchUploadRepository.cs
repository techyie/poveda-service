using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IBatchUploadRepository : IBaseRepository<batchUploadEntity>
    {
        Task<batchUploadEntity> GetByID_(int id);
        Task<ICollection<batchUploadEntity>> GetByUserOnProcess(int userId);
    }
}
