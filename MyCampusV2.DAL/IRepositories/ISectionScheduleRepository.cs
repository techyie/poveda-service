using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface ISectionScheduleRepository : IBaseRepository<sectionScheduleEntity>
    {
        Task<sectionSchedulePagedResult> GetAllSchedule(int sectionId, int pageNo, int pageSize, string keyword);
        Task<sectionScheduleEntity> GetSectionScheduleById(int id);
    }
}
