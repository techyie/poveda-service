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
    public class EmployeeTypeRepository : BaseRepository<empTypeEntity>, IEmployeeTypeRepository
    {
        private readonly MyCampusCardContext context;

        public EmployeeTypeRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<IList<empTypeEntity>> GetEmployeeTypesUsingCampusId(int id)
        {
            try
            {
                return await this.context.EmpTypeEntity
                    .Include(q => q.CampusEntity)
                    .Where(x => x.Campus_ID == id && x.IsActive == true && x.ToDisplay == true).ToListAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<empTypeEntity> GetEmployeeTypeById(int id)
        {
            try
            {
                return await this.context.EmpTypeEntity
                    .Include(q => q.CampusEntity)
                    .Where(x => x.EmpType_ID == id).SingleOrDefaultAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<IList<empTypeEntity>> GetEmployeeTypes()
        {
            return await this.context.EmpTypeEntity.Where(q => q.ToDisplay == true).ToListAsync();
        }

        public async Task<employeeTypePagedResult> GetAllEmployeeType(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new employeeTypePagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                var test = this.context.EmpTypeEntity;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.EmpTypeEntity.Where(q => q.ToDisplay == true).Count();
                else
                    result.RowCount = this.context.EmpTypeEntity
                        .Include(q => q.CampusEntity)
                        .Where(q =>
                        q.EmpTypeDesc.Contains(keyword) ||
                        q.CampusEntity.Campus_Name.Contains(keyword) && q.ToDisplay == true).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.employeeTypeList = await this.context.EmpTypeEntity
                        .Include(q => q.CampusEntity)
                        .Where(q => q.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated).Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.employeeTypeList = await this.context.EmpTypeEntity
                        .Include(q => q.CampusEntity)
                        .Where(q =>
                        q.EmpTypeDesc.Contains(keyword) ||
                        q.CampusEntity.Campus_Name.Contains(keyword) && (q.ToDisplay == true))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            } catch (Exception err)
            {
                throw err;
            }

        }
    }
}