using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.Common;
using MyCampusV2.Common.Helpers;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MyCampusV2.Services.Services
{
    public class CardDetailsService : BaseService, ICardDetailsService
    {

        private string _cardBatch = AppDomain.CurrentDomain.BaseDirectory + @"Card\";

        private readonly IDataSyncService _dataSyncService;
        private readonly ITerminalWhiteListService _terminalWhiteListService;

        private ActivityWriter _logger = new ActivityWriter();
        private ErrorLogging _errorLogging = new ErrorLogging();
        private string _formName = "Card";

        public CardDetailsService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user, IDataSyncService dataSyncService, ITerminalWhiteListService terminalWhiteListService) : base(unitOfWork, audit, user)
        {
            this._dataSyncService = dataSyncService;
            this._terminalWhiteListService = terminalWhiteListService;
        }

        public async Task<ResultModel> AssignCard(cardDetailsEntity cardEntity, int user)
        {
            try
            {
                bool deleteInDataSyncFetcher = await CheckPairing(cardEntity.Person_ID, cardEntity.Card_Person_Type);

                cardEntity.Added_By = user;
                cardEntity.Updated_By = user;

                return await _unitOfWork.CardDetailsRepository.AssignCard(cardEntity, deleteInDataSyncFetcher);

            } catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> ReassignCard(cardDetailsEntity oldCardEntity, cardDetailsEntity newCardEntity, int user)
        {
            try
            {
                bool deleteInDataSyncFetcher = await CheckPairing(newCardEntity.Person_ID, newCardEntity.Card_Person_Type);

                if (newCardEntity.Card_Person_Type == "Fetcher")
                {
                    try
                    {
                        var fetcherSched = await _unitOfWork.CardDetailsRepository.GetFetcherScheduleDetails(newCardEntity.Person_ID);

                        if (fetcherSched != null)
                        {
                            return CreateResult("409", "Cannot re-assign fetcher due to active student/s connected! Please remove all of the connected students before proceeding.", false);
                        }
                    } 
                    catch (Exception err)
                    {
                        _errorLogging.Error("ReassignCard Service GetFetcherScheduleDetails", err.Message);
                        return CreateResult("500", err.Message, false);
                    }
                }

                var card = await _unitOfWork.CardDetailsRepository.FindAsync(q => q.Person_ID == oldCardEntity.Person_ID && q.IsActive == true);

                card.Card_Person_Type = oldCardEntity.Card_Person_Type;
                card.Remarks = "Card deactivated due to reassign of new card";
                card.Updated_By = user;

                newCardEntity.Remarks = "Card reassigned due to " + newCardEntity.Remarks;
                newCardEntity.Added_By = user;
                newCardEntity.Updated_By = user;

                return await _unitOfWork.CardDetailsRepository.ReassignCard(deleteInDataSyncFetcher, card, newCardEntity);
            }
            catch (Exception err)
            {
                _errorLogging.Error("ReassignCard Service", err.Message);
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ICollection<cardDetailsEntity>> GetAll()
        {
            return await _unitOfWork.CardDetailsRepository.GetAllAsyn();
        }

        public async Task<cardDetailsEntity> GetByCardSerial(string cardSerial)
        {
            return await _unitOfWork.CardDetailsRepository.GetByCardSerial(cardSerial);
        }
       
        public async Task<ResultModel> UpdateCard(cardDetailsEntity cardEntity, int user)
        {
            try
            {
                bool deleteInDataSyncFetcher = await CheckPairing(cardEntity.Person_ID, cardEntity.Card_Person_Type);

                cardEntity.Remarks = "Card Updated";
                cardEntity.Updated_By = user;

                return await _unitOfWork.CardDetailsRepository.UpdateCard(cardEntity, deleteInDataSyncFetcher);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeactivateCard(cardDetailsEntity cardEntity, int user)
        {
            try
            {
                bool deleteInDataSyncFetcher = await CheckPairing(cardEntity.Person_ID, cardEntity.Card_Person_Type);

                cardEntity.Remarks = "Card deactivated due to " + cardEntity.Remarks;
                cardEntity.Updated_By = user;

                return await _unitOfWork.CardDetailsRepository.DeactivateCard(cardEntity, deleteInDataSyncFetcher);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<bool> CheckPairing(int personId, string personType)
        {
            bool deleteInDataSyncFetcher = false;

            if (personType == "Student")
            {
                try
                {
                    var studentSched = await _unitOfWork.CardDetailsRepository.GetStudentScheduleDetails(personId);

                    if (studentSched != null)
                    {
                        deleteInDataSyncFetcher = true;
                    }
                }
                catch (Exception err)
                {
                    _errorLogging.Error("Card Service GetStudentScheduleDetails", err.Message);
                }
            }
            else if (personType == "Fetcher")
            {
                try
                {
                    var fetcherSched = await _unitOfWork.CardDetailsRepository.GetFetcherScheduleDetails(personId);

                    if (fetcherSched != null)
                    {
                        deleteInDataSyncFetcher = true;
                    }
                }
                catch (Exception err)
                {
                    _errorLogging.Error("Card Service GetFetcherScheduleDetails", err.Message);
                }
            }

            return deleteInDataSyncFetcher;
        }

        public async Task<cardDetailsEntity> GetCard(long id)
        {
            return await _unitOfWork.CardDetailsRepository.GetAsync(id);
        }

        public async Task<cardDetailsEntity> GetByPerson(long personID)
        {
            return await _unitOfWork.CardDetailsRepository.GetByPersonID(personID);
        }

        public async Task<cardDetailsEntity> GetByIdNumber(string idNumber)
        {
            return await _unitOfWork.CardDetailsRepository.GetByIdNumber(idNumber);
        }
        public async Task<ICollection<cardDetailsEntity>> GetAllByPerson(long personID)
        {
            return await _unitOfWork.CardDetailsRepository.GetAllByPersonID(personID);
        }

        public async Task<BatchUploadResponse> BatchUpdate(ICollection<cardBatchUpdateVM> cards, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "Card_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = cards.Count();
            response.FileName = fileName;

            int i = 1 + row;

            foreach (var cardVM in cards)
            {

                i++;

                if (cardVM.IDNumber == null || cardVM.IDNumber == string.Empty)
                {
                    importLog.Logging(_cardBatch, fileName, "Row: " + i.ToString() + " ---> ID Number is required.");
                    response.Failed++;
                    continue;
                }

                var person = await _unitOfWork.PersonRepository.FindAsync(a => a.ID_Number == cardVM.IDNumber && a.IsActive == true);
                if (person == null)
                {
                    importLog.Logging(_cardBatch, fileName, "Reference: " + cardVM.IDNumber + " ---> Card does not exist.");
                    response.Failed++;
                    continue;
                }

                var card =  await _unitOfWork.CardDetailsRepository.GetByPersonID(person.Person_ID);
                if (card == null)
                {
                    importLog.Logging(_cardBatch, fileName, "Reference: " + cardVM.IDNumber + " ---> Card does not exist.");
                    response.Failed++;
                    continue;
                }
                
                if (!card.IsActive)
                {
                    importLog.Logging(_cardBatch, fileName, "Reference: " + cardVM.IDNumber + " ---> Card already deactivated.");
                    response.Failed++;
                    continue;
                }

                DateTime dt;
                if (cardVM.ExpiryDate == "" || cardVM.ExpiryDate == null)
                {
                    importLog.Logging(_cardBatch, fileName, "Reference: " + cardVM.IDNumber + " ---> Expiry Date is required.");
                    response.Failed++;
                    continue;
                }
                else if (!DateTime.TryParse(cardVM.ExpiryDate, out dt))
                {
                    importLog.Logging(_cardBatch, fileName, "Reference: " + cardVM.IDNumber + " ---> Expiry Date is invalid.");
                    response.Failed++;
                    continue;
                }

                if (card != null)
                {
                    card.Remarks = "Card Updated";
                    card.Expiry_Date = dt;
                    card.Updated_By = user;
                    card.Card_Person_Type = person.Person_Type;

                    bool deleteInDataSyncFetcher = false;

                    if (card.Card_Person_Type == "Student")
                    {
                        try
                        {
                            var studentSched = await _unitOfWork.CardDetailsRepository.GetStudentScheduleDetails(card.Person_ID);

                            if (studentSched != null)
                            {
                                deleteInDataSyncFetcher = true;
                            }
                        }
                        catch (Exception err)
                        {
                            importLog.Logging(_cardBatch, fileName, _formName + " " + cardVM.IDNumber + " failed to update.");
                            _errorLogging.Error("BatchUpdateCard Service GetStudentScheduleDetails", err.Message);
                            response.Failed++;
                            continue;
                        }
                    }

                    var result = await _unitOfWork.CardDetailsRepository.UpdateCard(card, deleteInDataSyncFetcher);

                    if (result.isSuccess)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Office, "Update Card By Batch", "UPDATE", true, "Success: " + cardVM.IDNumber, DateTime.UtcNow.ToLocalTime()));
                        importLog.Logging(_cardBatch, fileName, _formName + " " + cardVM.IDNumber + " successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_cardBatch, fileName, _formName + " " + cardVM.IDNumber + " failed to update.");
                        response.Failed++;
                        continue;
                    }
                }
            }

            return response;
        }
        
        public async Task<BatchUploadResponse> BatchDeactivate(ICollection<cardBatchDeactiveVM> cards, int user, int uploadID, int row)
        {

            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "Card_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = cards.Count();
            response.FileName = fileName;

            int i = 1 + row;

            var terminals = await _unitOfWork.TerminalRepository.FindAllAsync(q => q.Terminal_Category == Common.Helpers.Encryption.Encrypt("Time And Attendance Terminal") && q.IsActive == true);

            foreach (var cardVM in cards)
            {
                i++;

                if (cardVM.IdNumber == null || cardVM.IdNumber == string.Empty)
                {
                    importLog.Logging(_cardBatch, fileName, "Row: " + i.ToString() + " ---> ID Number is required.");
                    response.Failed++;
                    continue;
                }

                var person = await _unitOfWork.PersonRepository.FindAsync(a => a.ID_Number == cardVM.IdNumber && a.IsActive == true);

                if (person == null)
                {
                    importLog.Logging(_cardBatch, fileName, "Reference: " + cardVM.IdNumber + " ---> Person does not exist.");
                    response.Failed++;
                    continue;
                }

                var card = await _unitOfWork.CardDetailsRepository.GetByPersonID(person.Person_ID);

                if (card == null)
                {
                    importLog.Logging(_cardBatch, fileName, "Reference: " + cardVM.IdNumber + " ---> Card does not exist.");
                    response.Failed++;
                    continue;
                }
                else if (!card.IsActive)
                {
                    importLog.Logging(_cardBatch, fileName, "Reference: " + cardVM.IdNumber + " ---> Card is already deactivated.");
                    response.Failed++;
                    continue;
                }

                if (card != null)
                {
                    card.Remarks = "Card Deactivated";
                    card.Updated_By = user;
                    card.Card_Person_Type = person.Person_Type;

                    bool deleteInDataSyncFetcher = false;

                    if (card.Card_Person_Type == "Student")
                    {
                        try
                        {
                            var studentSched = await _unitOfWork.CardDetailsRepository.GetStudentScheduleDetails(card.Person_ID);

                            if (studentSched != null)
                            {
                                deleteInDataSyncFetcher = true;
                            }
                        }
                        catch (Exception err)
                        {
                            importLog.Logging(_cardBatch, fileName, _formName + " " + cardVM.IdNumber + " failed to update.");
                            _errorLogging.Error("BatchDeactivateCard Service GetStudentScheduleDetails", err.Message);
                            response.Failed++;
                            continue;
                        }
                    }

                    var result = await _unitOfWork.CardDetailsRepository.DeactivateCard(card, deleteInDataSyncFetcher);

                    if (result.isSuccess)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Office, "Deactivate Card By Batch", "DEACTIVATE", true, "Success: " + cardVM.IdNumber, DateTime.UtcNow.ToLocalTime()));
                        importLog.Logging(_cardBatch, fileName, _formName + " " + cardVM.IdNumber + " successfully deactivated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_cardBatch, fileName, _formName + " " + cardVM.IdNumber + " failed to deactivate.");
                        response.Failed++;
                        continue;
                    }
                }
            }

            return response;
        }


        public async Task<cardPagedResult> ExportCards(string keyword)
        {
            return await _unitOfWork.CardDetailsReportRepository.ExportCards(keyword);
        }
    }
}
