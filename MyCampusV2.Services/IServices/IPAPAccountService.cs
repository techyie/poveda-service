using MyCampusV2.Common;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.IServices
{
    public interface IPAPAccountService
    {
        Task<ResultModel> AddMobileAppAccount(papAccountEntity papAccount, EmailSettings emailSettings, int userId);
        Task<ResultModel> UpdateMobileAppAccount(papAccountEntity papAccount, int userId);
        Task<ResultModel> DeleteMobileAppTemporary(string accountCode, int userId);
        Task<papAccountPagedResult> GetAll(int pageNo, int pageSize, string keyword);
        Task<papAccountEntity> GetPapAccountByAccountCode(string code);
        Task<IList<personEntity>> GetStudentList();
        Task<ResultModel> SendEmail(EmailSettings emailSettings, string email, string accountCode, int userId);
        Task<ResultModel> ValidateChangePasswordLink(EmailSettings emailSettings, string accountCode);
        Task<ResultModel> AccountActivation(string password, string accountCode, int userId);
        Task<papAccountPagedResult> Export(string keyword);
        Task<BatchUploadResponse> BatchUpload(ICollection<papAccountBatchUploadVM> papAccounts, int user, int uploadID, int row);
        Task<papAccountEntity> SignIn(PapAccountSignIn accountSignIn);
        Task<ICollection<papAccountLinkedStudentsEntity>> GetPapAccountLinkedStudentsByAccountCode(string code);
    }
}
