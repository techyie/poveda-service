using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.ViewModels
{
    public class digitalReferencesVM
    {
        public int id { get; set; }
        public string digitalReferenceCode { get; set; }
        public string title { get; set; }
        public string fileType { get; set; }
        public string filePath { get; set; }
        public string dateUploaded { get; set; }
        public string fileName { get; set; }
        public string fileDisplay { get; set; }
        public bool isActive { get; set; }
        public bool toDisplay { get; set; }
        public int? addedBy { get; set; }
        public int? updatedBy { get; set; }
        public DateTime dateTimeAdded { get; set; }
        public DateTime lastUpdated { get; set; }
        public string dateAdded { get; set; }
    }

    public class digitalReferencesPagedResult
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

        public IList<digitalReferencesEntity> digitalReferences { get; set; }
    }
    public class digitalReferencesPagedResultVM
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

        public virtual ICollection<digitalReferencesVM> digitalReferences { get; set; }
    }
}
