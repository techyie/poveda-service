using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.DAL.Repositories
{
    public class CalendarRecipientsRepository : BaseRepository<calendarRecipientsEntity>, ICalendarRecipientsRepository
    {
        private readonly MyCampusCardContext context;

        public CalendarRecipientsRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }
    }
}
