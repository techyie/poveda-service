using Microsoft.EntityFrameworkCore;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class BatchUploadRepository : BaseRepository<batchUploadEntity>, IBatchUploadRepository
    {
        public BatchUploadRepository(MyCampusCardContext context) : base(context)
        {
        }

        public async Task<batchUploadEntity> GetByID_(int id)
        {
            return await _context.tbl_batch_upload
                .Include(a => a.tbl_form)
                .Include(a => a.tbl_user)
                .Where(a => a.ID == id).FirstOrDefaultAsync();
        }

        public async Task<ICollection<batchUploadEntity>> GetByUserOnProcess(int userId)
        {
            return await _context.tbl_batch_upload
                .Where(a => a.User_ID == userId 
                && ((a.Status == "UPLOAD") || (a.Status =="ON PROCESS")))
                .ToListAsync();
        }
    }
}
