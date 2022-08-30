using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services
{
    public interface IFormService
    {
        List<formEntity> getAll();
        List<formEntity> GetUserForm(int user);
        Task<formEntity> getById(int id);
        Task addform(formEntity form);
        Task<ICollection<formEntity>> GetAllForm();
        bool CheckForm(int user);
    }
}
