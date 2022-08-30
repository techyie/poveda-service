using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Linq;

namespace MyCampusV2.DAL.IRepositories
{
    public interface IFormRepository : IBaseRepository<formEntity>
    {
       List<formEntity> getForms();
       List<formEntity> GetUserForm(int role);
       IQueryable<formEntity> GetAllForms();
       bool CheckForm(int user);
    }
}
