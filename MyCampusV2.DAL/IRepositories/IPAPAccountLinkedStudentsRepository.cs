using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IPAPAccountLinkedStudentsRepository : IBaseRepository<papAccountLinkedStudentsEntity>
    {
        Task<ICollection<papAccountLinkedStudentsEntity>> GetPapAccountLinkedStudentsByAccountCode(string code);
    }
}
