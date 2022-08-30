using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class PAPAccountLinkedStudentsRepository : BaseRepository<papAccountLinkedStudentsEntity>, IPAPAccountLinkedStudentsRepository
    {
        private readonly MyCampusCardContext context;

        public PAPAccountLinkedStudentsRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<ICollection<papAccountLinkedStudentsEntity>> GetPapAccountLinkedStudentsByAccountCode(string code)
        {
            return await this.context.PapAccountLinkedStudentsEntity
                .Include(q => q.PapAccountEntity)
                .Include(q => q.PersonEntity).ThenInclude(q => q.StudentSectionEntity).ThenInclude(q => q.YearSectionEntity).Where(x => x.PapAccountEntity.Account_Code == code).ToListAsync();
        }
    }
}
