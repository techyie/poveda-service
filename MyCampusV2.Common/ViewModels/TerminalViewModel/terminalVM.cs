using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class terminalVM
    {
        public int? terminalId { get; set; }
        public string terminalCode { get; set; }
        public string terminalName { get; set; }
        public string terminalIp { get; set; }
        public string terminalCategory { get; set; }
        public bool terminalStatus { get; set; }
        public bool isForFetcher { get; set; }
        public bool withFetcher { get; set; }
        public string HostIP { get; set; }
        public int? areaId { get; set; }
        public string areaName { get; set; }
        public int? campusId { get; set; }
        public string campusName { get; set; }
        public bool? isActive { get; set; }
        public bool? toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
        public string mobileTerminalCode { get; set; }
        public string mobileTerminalName { get; set; }
        public string mobileTerminalIp { get; set; }
        public string currentCampus { get; set; }
    }

    public class terminalPagedResult
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }

        public int FirstRowOnPage
        {

            get { return (CurrentPage - 1) * PageSize + 1; }
        }

        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }

        public IList<terminalEntity> terminals { get; set; }
    }

    public class terminalPagedResultVM
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }

        public int FirstRowOnPage
        {

            get { return (CurrentPage - 1) * PageSize + 1; }
        }

        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }

        public virtual ICollection<terminalVM> terminals { get; set; }
    }
}
