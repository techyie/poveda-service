using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.DAL.Repositories
{
    public class AnnouncementsRecipientsRepository : BaseRepository<announcementsRecipientsEntity>, IAnnouncementsRecipientsRepository
    {
        private readonly MyCampusCardContext context;

        public AnnouncementsRecipientsRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }
    }
}
