using Microsoft.EntityFrameworkCore;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Services
{    public class FormService : IFormService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FormService(IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
        }

        public async Task<ICollection<formEntity>> GetAllForm(){
            return await _unitOfWork.FormRepository.GetAllForms().ToListAsync();
        }

        public List<formEntity> getAll()
        {
            return  BuildTreeAndReturnRootNodes(_unitOfWork.FormRepository.getForms()); 
            // return await _unitOfWork.FormRepository.GetAllAsyn();
        }

        public bool CheckForm(int user)
        {
            return _unitOfWork.FormRepository.CheckForm(user);
        }

        public List<formEntity> GetUserForm(int user)
        {
            return BuildTreeAndReturnRootNodes(_unitOfWork.FormRepository.GetUserForm(user));
        }

        static List<formEntity> BuildTreeAndReturnRootNodes(List<formEntity> items)
        {
            var lookup = items.ToLookup(i => i.Form_ID);
            foreach (var item in items)
            {
                if (item.Parent_ID != null)
                {
                    var parent = lookup[item.Parent_ID.Value].FirstOrDefault();
                    parent.children.Add(item);
                }
            }
            return items.Where(i => i.Parent_ID == null).ToList();
        }

        public async Task<formEntity> getById(int id)
        {
            return await _unitOfWork.FormRepository.GetAsync(id);
        }

        public async Task addform(formEntity form)
        {
            try
            {
                await _unitOfWork.FormRepository.AddAsyn(form);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task updateform(formEntity form, int id)
        {
            try
            {
                form.Form_ID = id;
                await _unitOfWork.FormRepository.UpdateAsyn(form, form.Form_ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
