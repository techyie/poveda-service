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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Encryption = MyCampusV2.Services.Helpers.Encryption;

namespace MyCampusV2.Services.Services
{
    public class PAPAccountService : BaseService, IPAPAccountService
    {
        private string _papAccountBatch = AppDomain.CurrentDomain.BaseDirectory + @"Mobile Account\";
        private ResultModel result = new ResultModel();
        private EmailService emailService = new EmailService();

        public PAPAccountService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user)
            : base(unitOfWork, audit, user)
        {

        }

        public async Task<papAccountEntity> GetPapAccountByAccountCode(string code)
        {
            return await _unitOfWork.PAPAccountRepository.GetPapAccountByAccountCode(code);
        }

        public async Task<papAccountPagedResult> GetAll(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.PAPAccountRepository.GetAll(pageNo, pageSize, keyword);
        }

        public async Task<ResultModel> AddMobileAppAccount(papAccountEntity papAccount, EmailSettings emailSettings, int userId)
        {
            try
            {
                var data = await _unitOfWork.PAPAccountRepository.FindAsync(x => x.Email_Address == papAccount.Email_Address.Trim());

                if (data != null)
                {
                    return CreateResult("409", "Email already exist!", false);
                }

                var accountResult = await _unitOfWork.PAPAccountRepository.AddMobileAppAccount(papAccount, userId);

                if (accountResult == string.Empty)
                    return CreateResult("500", Common.Helpers.Constants.FailedMessageCreate + "Mobile App Account!", false);
                else
                {
                    var emailResult = await SendEmail(emailSettings, papAccount.Email_Address, accountResult, userId);

                    return emailResult;
                }
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateMobileAppAccount(papAccountEntity papAccount, int userId)
        {
            try
            {
                return await _unitOfWork.PAPAccountRepository.UpdateMobileAppAccount(papAccount, userId);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteMobileAppTemporary(string accountCode, int userId)
        {
            try
            {
                return await _unitOfWork.PAPAccountRepository.DeleteMobileAppTemporary(accountCode, userId);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<IList<personEntity>> GetStudentList()
        {
            return await _unitOfWork.PAPAccountRepository.GetStudentList();
        }

        public async Task<ResultModel> AccountActivation(string password, string accountCode, int userId)
        {
            try
            {
                string encyptedPassword = Encryption.Encrypt(password);
                return await _unitOfWork.PAPAccountRepository.AccountActivation(encyptedPassword, accountCode, userId);
            } catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> ValidateChangePasswordLink(EmailSettings emailSettings, string accountCode)
        {
            try
            {
                var data = await _unitOfWork.PAPAccountRepository.ValidateChangePasswordLink(accountCode);

                if (data != null)
                {
                    if (data.IsRequestChangePassword)
                    {
                        return CreateResult("200", emailSettings.ActivationUrl + accountCode, true);
                    }
                    else
                    {
                        return CreateResult("404", "Sorry, the link you're trying to access is invalid.", false);
                    }
                }
                else
                {
                    return CreateResult("404", "Account Code Not Found!", false);
                }
                    
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> SendEmail(EmailSettings emailSettings, string email, string accountCode, int userId)
        {
            string Host = emailSettings.Host;
            int Port = emailSettings.Port;
            string Username = emailSettings.Username;
            string Password = emailSettings.Password;
            string SenderEmail = emailSettings.SenderEmail;
            string SenderName = emailSettings.SenderName;
            string Title = emailSettings.EmailTitle;
            string Logo = Path.Combine(Directory.GetCurrentDirectory(), emailSettings.Logo);
            string Template = Path.Combine(Directory.GetCurrentDirectory(), emailSettings.Template);
            string ActivationUrl = emailSettings.ActivationUrl;
            string LinkValidationUrl = emailSettings.LinkValidationUrl;

            using (Image image = Image.FromFile(Logo))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    Logo = "data:image/png;base64," + Convert.ToBase64String(imageBytes);
                }
            }

            string body = new FileReader().ReadText(Template);

            body = body.Replace("{PovedaLogo}", Logo);
            body = body.Replace("{Email}", email);
            body = body.Replace("{LinkValidationUrl}", LinkValidationUrl + accountCode);

            var emailResult = await emailService.SendMessage(email, Title, body, SenderName, SenderEmail, Username, Password, Host, Port);

            if (emailResult)
            {
                await _unitOfWork.PAPAccountRepository.RequestChangePassword(accountCode, userId);

                return CreateResult("200", Common.Helpers.Constants.SuccessPasswordResetEmailSend + " " + email, true);
            }
            else
            {
                return CreateResult("400", Common.Helpers.Constants.FailedEmailSend, true);
            }
        }

        public async Task<papAccountPagedResult> Export(string keyword)
        {
            return await _unitOfWork.PAPAccountRepository.Export(keyword);
        }

        public async Task<BatchUploadResponse> BatchUpload(ICollection<papAccountBatchUploadVM> papAccounts, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "Account_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = papAccounts.Count();
            response.FileName = fileName;

            Regex regexName = new Regex("^[ñÑa-zA-Z -.]+$");
            Regex regexEmail = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                + "@"
                + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$",
            RegexOptions.CultureInvariant | RegexOptions.Singleline);

            int i = 1 + row;
            foreach (var accountVM in papAccounts)
            {
                i++;

                if (accountVM.FirstName == null || accountVM.FirstName == string.Empty)
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> First Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!regexName.IsMatch(accountVM.FirstName.Trim()))
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> First Name is invalid.");
                    response.Failed++;
                    continue;
                }

                if (accountVM.MiddleName != null && accountVM.MiddleName != string.Empty) {
                    if (!regexName.IsMatch(accountVM.MiddleName.Trim()))
                    {
                        importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> Middle Name is invalid.");
                        response.Failed++;
                        continue;
                    }
                }

                if (accountVM.LastName == null || accountVM.LastName == string.Empty)
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> Last Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!regexName.IsMatch(accountVM.LastName.Trim()))
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> Last Name is invalid.");
                    response.Failed++;
                    continue;
                }

                if (accountVM.EmailAddress == null || accountVM.EmailAddress == string.Empty)
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> Email Address is a required field.");
                    response.Failed++;
                    continue;
                }

                if (accountVM.MobileNumber == null || accountVM.MobileNumber == string.Empty)
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> Mobile Number is a required field.");
                    response.Failed++;
                    continue;
                }

                if (accountVM.LinkedStudents == null || accountVM.LinkedStudents == string.Empty)
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> Linked Students is a required field.");
                    response.Failed++;
                    continue;
                }

                if (!Global.ValidateStr(accountVM.FirstName.Trim()))
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> First Name does not accept special characters except period and dash.");
                    response.Failed++;
                    continue;
                }
                else if (accountVM.FirstName.Trim().Length > 100)
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> First Name accepts 100 characters only.");
                    response.Failed++;
                    continue;
                }

                if (!Global.ValidateStr(accountVM.LastName.Trim()))
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> Last Name does not accept special characters except period and dash.");
                    response.Failed++;
                    continue;
                }
                else if (accountVM.LastName.Trim().Length > 100)
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> Last Name accepts 100 characters only.");
                    response.Failed++;
                    continue;
                }

                if (accountVM.EmailAddress.Trim().Length > 100)
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> Email Address accepts 100 characters only.");
                    response.Failed++;
                    continue;
                } 
                else if (!regexEmail.IsMatch(accountVM.EmailAddress.Trim()))
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> Email Address is invalid.");
                    response.Failed++;
                    continue;
                }

                if (accountVM.MobileNumber.Trim().Length != 11)
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> Mobile number is invalid.");
                    response.Failed++;
                    continue;
                }

                var papAccount = await _unitOfWork.PAPAccountRepository.FindAsync(x => x.Email_Address == accountVM.EmailAddress.Trim());

                if (papAccount != null)
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> Email Address already exist.");
                    response.Failed++;
                    continue;
                }

                papAccountEntity newAccount = new papAccountEntity();
                newAccount.First_Name = accountVM.FirstName;
                newAccount.Middle_Name = accountVM.MiddleName == null || accountVM.MiddleName == string.Empty ? string.Empty : accountVM.MiddleName;
                newAccount.Last_Name = accountVM.LastName;
                newAccount.Email_Address = accountVM.EmailAddress;
                newAccount.Mobile_Number = accountVM.MobileNumber;
                newAccount.Linked_Students = new List<students>();

                string[] idnumbers = accountVM.LinkedStudents.Split(';');
                bool noErrorInLinkedStudents = true;

                for (int q = 0; q < idnumbers.Length; q++)
                {
                    var infoExist = await _unitOfWork.PersonRepository.FindAsync(x => x.ID_Number == idnumbers[q].ToString().Trim());

                    if(infoExist != null)
                    {
                        students newStudent = new students();
                        newStudent.key = infoExist.ID_Number;
                        newStudent.value = (infoExist.First_Name + " " + infoExist.Last_Name + " (" + infoExist.ID_Number + ")");
                        newAccount.Linked_Students.Add(newStudent);
                    }
                    else
                    {
                        noErrorInLinkedStudents = false;
                        break;
                    }
                }

                if (noErrorInLinkedStudents == false)
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> One of the ID Number in Linked Students is not yet registered.");
                    response.Failed++;
                    continue;
                }

                var result = await _unitOfWork.PAPAccountRepository.AddMobileAppAccount(newAccount, user);

                if (result != string.Empty)
                {
                    importLog.Logging(_papAccountBatch, fileName, "Row: " + i.ToString() + " ---> Account with " + newAccount.Email_Address + " email address has been successfully added.");
                    response.Success++;
                    continue;
                }
            }

            return response;
        }

        public async Task<papAccountEntity> SignIn(PapAccountSignIn accountSignIn)
        {
            return await _unitOfWork.PAPAccountRepository.SignIn(accountSignIn);
        }

        public async Task<ICollection<papAccountLinkedStudentsEntity>> GetPapAccountLinkedStudentsByAccountCode(string code)
        {
            return await _unitOfWork.PAPAccountLinkedStudentsRepository.GetPapAccountLinkedStudentsByAccountCode(code);
        }
    }
}
