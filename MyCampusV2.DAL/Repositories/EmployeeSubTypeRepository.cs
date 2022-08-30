using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
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
    public class EmployeeSubTypeRepository : BaseRepository<employeeSubTypeEntity>, IEmployeeSubTypeRepository
    {
        private readonly MyCampusCardContext context;

        public EmployeeSubTypeRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<IList<employeeSubTypeEntity>> GetEmployeeSubTypesUsingEmployeeTypeId(int id)
        {
            try
            {
                return await this.context.EmployeeSubTypeEntity
                    .Include(q => q.EmployeeType)
                    .ThenInclude(q => q.CampusEntity)
                    .Where(x => x.EmpType_ID == id && x.IsActive == true && x.ToDisplay == true).ToListAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<IList<employeeSubTypeEntity>> GetEmployeeSubTypes()
        {
            return await this.context.EmployeeSubTypeEntity.Where(q => q.ToDisplay == true).ToListAsync();
        }

        public async Task<employeeSubTypeEntity> GetEmployeeSubTypeById(int id)
        {
            try
            {
                return await this.context.EmployeeSubTypeEntity
                    .Include(q => q.EmployeeType)
                    .ThenInclude(q => q.CampusEntity)
                    .Where(x => x.EmpSubtype_ID == id).SingleOrDefaultAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<employeeSubTypePagedResult> GetAllEmployeeSubType(int pageNo, int pageSize, string keyword)
        {
            var result = new employeeSubTypePagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            if (keyword == null || keyword == "")
                result.RowCount = this.context.EmployeeSubTypeEntity
                    .Include(q => q.EmployeeType)
                    .ThenInclude(q => q.CampusEntity)
                    .Where(q => q.ToDisplay == true).Count();
            else
                result.RowCount = this.context.EmployeeSubTypeEntity
                    .Include(q => q.EmployeeType)
                    .ThenInclude(q => q.CampusEntity)
                    .Where(q => q.EmpSubTypeDesc.Contains(keyword) 
                        ||  q.EmployeeType.EmpTypeDesc.Contains(keyword)
                        || q.EmployeeType.CampusEntity.Campus_Name.Contains(keyword)
                        && (q.ToDisplay == true)).Count();

            var skip = (pageNo - 1) * pageSize;
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            if (keyword == null || keyword == "")
                result.employeeSubTypeList = await this.context.EmployeeSubTypeEntity
                    .Include(q => q.EmployeeType)
                    .ThenInclude(x => x.CampusEntity)
                    .Where(q => q.ToDisplay == true)
                    .OrderByDescending(c => c.Last_Updated)
                    .Skip(skip).Take(pageSize).ToListAsync();
            else
                result.employeeSubTypeList = await this.context.EmployeeSubTypeEntity
                    .Include(q => q.EmployeeType)
                    .ThenInclude(x => x.CampusEntity)
                    .Where(q => q.EmpSubTypeDesc.Contains(keyword) 
                        || q.EmployeeType.EmpTypeDesc.Contains(keyword)
                        || q.EmployeeType.CampusEntity.Campus_Name.Contains(keyword)
                        && q.ToDisplay == true)
                    .Include(q => q.EmployeeType)
                    .OrderByDescending(c => c.Last_Updated)
                    .Skip(skip).Take(pageSize).ToListAsync();

            return result;
        }
    }
}