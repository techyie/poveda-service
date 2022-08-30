using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{
    public interface ICardDetailsService
    {
        Task<ICollection<cardDetailsEntity>> GetAll();
        Task<cardDetailsEntity> GetCard(long id);
        Task<cardDetailsEntity> GetByCardSerial(string serial);
        Task<cardDetailsEntity> GetByPerson(long personID);
        Task<ICollection<cardDetailsEntity>> GetAllByPerson(long personID);
        Task<cardDetailsEntity> GetByIdNumber(string idNumber);
        Task<BatchUploadResponse> BatchUpdate(ICollection<cardBatchUpdateVM> cards, int user,int uploadID,int row);
        Task<BatchUploadResponse> BatchDeactivate(ICollection<cardBatchDeactiveVM> cards, int user, int uploadID, int row);
        Task<cardPagedResult> ExportCards(string keyword);

        Task<ResultModel> AssignCard(cardDetailsEntity cardEntity, int user);
        Task<ResultModel> UpdateCard(cardDetailsEntity cardEntity, int user);
        Task<ResultModel> DeactivateCard(cardDetailsEntity cardEntity, int user);
        Task<ResultModel> ReassignCard(cardDetailsEntity oldCardEntity, cardDetailsEntity newCardEntity, int user);
    }
}
