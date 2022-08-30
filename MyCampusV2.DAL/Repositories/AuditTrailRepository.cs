using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace MyCampusV2.DAL.Repositories
{
    public class AuditTrailRepository : BaseRepository<auditTrailEntity>, IAuditTrailRepository
    {
        private readonly MyCampusCardContext context;

        public AuditTrailRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

       /* public async Task<ICollection<tbl_area>> GetAllAuditTrail()
        {
            return await _context.tbl_area.Include(x => x.tbl_campus).ToListAsync();
        }*/

        public void Audit(auditTrailEntity auditTrail)
        {
                using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.AuditTrailEntity.Add(auditTrail);
                        _context.SaveChanges();
                        dbcxtransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbcxtransaction.Rollback();
                        throw ex;
                    }
                }
        }

        public async Task AuditAsync(int user, int form, string message)
        {
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    auditTrailEntity auditTrail = new auditTrailEntity { User_ID = user, Action = message, Form_ID = form, Date = DateTime.Now };

                    await _context.AuditTrailEntity.AddAsync(auditTrail);
                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    throw ex;
                }
            }
        }


    }
}
