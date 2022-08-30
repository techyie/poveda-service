using Microsoft.EntityFrameworkCore;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class VisitorRepository : BaseRepository<visitorInformationEntity>, IVisitorRepository
    {
        private readonly MyCampusCardContext context;

        public VisitorRepository(MyCampusCardContext context) : base(context)
        {
            this.context = context;

        }

        public bool AddVisitorInfo(visitorInformationEntity visitor)
        {
            //try {
            //    context.visitor.Add(visitor.Visitor);
            //    context.visitorInformationEntity.Add(visitor);
            //    return true;
            //}
            //catch (Exception) {
            //}
            //return false;
            return true;
        }

        public async Task<ICollection<visitorInformationEntity>> GetAllVisitorsAsync()
        {
            //return await context.visitorInformationEntity
            //   .Include(x => x.Visitor).ThenInclude(x => x.tbl_campus)
            //   .ToListAsync();
            return null;
        }

    }
}