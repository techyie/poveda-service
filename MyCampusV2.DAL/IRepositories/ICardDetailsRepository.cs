using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.IRepositories
{
    public interface ICardDetailsRepository : IBaseRepository<cardDetailsEntity>
    {
        Task<cardDetailsEntity> GetByCardSerial(string cardSerial);
        Task<cardDetailsEntity> GetByPersonID(long person_ID);
        Task<ICollection<cardDetailsEntity>> GetAllByPersonID(long person_ID);
        Task<cardDetailsEntity> GetByIdNumber(string idNumber);
        Task<fetcherScheduleDetailsEntity> GetFetcherScheduleDetails(int fetcherId);
        Task<fetcherScheduleDetailsEntity> GetStudentScheduleDetails(int studentId);
        Task<ResultModel> AssignCard(cardDetailsEntity card, bool deleteInDataSyncFetcher);
        Task<ResultModel> ReassignCard(bool deleteInDataSyncFetcher, cardDetailsEntity card, cardDetailsEntity newCard);
        Task<ResultModel> UpdateCard(cardDetailsEntity card, bool deleteInDataSyncFetcher);
        Task<ResultModel> DeactivateCard(cardDetailsEntity card, bool deleteInDataSyncFetcher);
    }
}
