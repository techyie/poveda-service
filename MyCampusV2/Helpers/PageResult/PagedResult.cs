using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Helpers.PageResult
{
    public class PagedResult<T>
    {
        public class PagingInfo
        {
            public int PageNo { get; set; }

            public int PageSize { get; set; }

            public int PageCount { get; set; }

            public long TotalRecordCount { get; set; }

        }
        public List<T> Data { get; private set; }

        public PagingInfo Paging { get; private set; }

        public PagedResult(IQueryable<T> items, int pageNo, int pageSize, long totalRecordCount)
        {
            Data = new List<T>(items);
            Paging = new PagingInfo
            {
                PageNo = pageNo,
                PageSize = pageSize,
                TotalRecordCount = totalRecordCount,
                PageCount = totalRecordCount > 0
                    ? (int)Math.Ceiling(totalRecordCount / (double)pageSize)
                    : 0
            };
        }
    }

    public class PaginationParams
    {
        public int PageNo { get; set; }

        public int PageSize { get; set; }

        public int PageCount { get; set; }

        public long TotalRecordCount { get; set; }

        public string Keyword { get; set; }

        //public bool Descending { get; set; }
    }

    public class AuditTrailPaginationParams
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public long TotalRecordCount { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public int user_ID { get; set; }
        public int form_ID { get; set; }
        public string status { get; set; }
    }
}
