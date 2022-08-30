﻿using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories.Report
{
    public interface IOfficeReportRepository : IBaseReportRepository<officeEntity>
    {
        Task<officePagedResult> ExportOffice(string keyword);
    }
}
