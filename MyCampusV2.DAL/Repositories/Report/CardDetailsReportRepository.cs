using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.DAL.IRepositories.Report;
using MyCampusV2.Models.V2.entity;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories.Report
{

    public class CardDetailsReportRepository : BaseReportRepository<cardDetailsEntity>, ICardDetailsReportRepository
    {
        public CardDetailsReportRepository(MyCampusCardReportContext context) : base(context)
        {
        }

        public async Task<cardPagedResult> ExportCards(string keyword)
        {
            var result = new cardPagedResult();

            if (keyword == null || keyword == "")
                result.cards = await _context.CardDetailsEntity
                    .Include(a => a.PersonEntity)
                    .OrderBy(e => e.PersonEntity.ID_Number)
                    .Where(a => a.IsActive)
                    .ToListAsync();
            else
                result.cards = await _context.CardDetailsEntity
                    .Include(a => a.PersonEntity)
                    .OrderBy(e => e.PersonEntity.ID_Number)
                    .Where(q => q.Card_Serial.Contains(keyword) && q.IsActive)
                    .ToListAsync();

            return result;
        }

    }
}
