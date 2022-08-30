using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.Common;
using MyCampusV2.Common.Helpers;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.Helpers;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class PAPAccountRepository : BaseRepository<papAccountEntity>, IPAPAccountRepository
    {
        private readonly MyCampusCardContext context;
        private ResultModel resultModel;
        private Response response;
        private string RepositoryName = "Mobile App Account";

        public PAPAccountRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<papAccountEntity> GetPapAccountByAccountCode(string code)
        {
            var papAccountData = await this.context.PapAccountEntity.Where(x => x.Account_Code == code).FirstOrDefaultAsync();

            if (papAccountData != null)
            {
                var linkedStudents = await this.context.PapAccountLinkedStudentsEntity.Include(q => q.PapAccountEntity).Include(q => q.PersonEntity).Where(x => x.PapAccountEntity.Account_Code == code).ToListAsync();
                papAccountData.Linked_Students = new List<students>();

                foreach (var data in linkedStudents)
                {
                    students students = new students();
                    students.key = data.PersonEntity.ID_Number;
                    students.value = (data.PersonEntity.First_Name + " " + (data.PersonEntity.Middle_Name != null || data.PersonEntity.First_Name != "" ? data.PersonEntity.Middle_Name.Substring(0, 1) : string.Empty) + " " + data.PersonEntity.Last_Name + "(" + data.PersonEntity.ID_Number + ")");
                    papAccountData.Linked_Students.Add(students);
                }
            }

            return papAccountData;
        }

        public async Task<papAccountPagedResult> GetAll(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new papAccountPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.PapAccountEntity.Where(a => a.ToDisplay == true).Count();
                else
                    result.RowCount = this.context.PapAccountEntity
                        .Where(q => q.ToDisplay == true && (q.Email_Address.Contains(keyword)
                        || q.First_Name.Contains(keyword)
                        || q.Last_Name.Contains(keyword)
                        || q.Middle_Name.Contains(keyword)
                        || q.Mobile_Number.Contains(keyword)
                        || q.Username.Contains(keyword)
                        || q.Email_Address.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.papAccounts = await this.context.PapAccountEntity.Where(a => a.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.papAccounts = await this.context.PapAccountEntity
                        .Where(q => q.ToDisplay == true && (q.Email_Address.Contains(keyword)
                        || q.First_Name.Contains(keyword)
                        || q.Last_Name.Contains(keyword)
                        || q.Middle_Name.Contains(keyword)
                        || q.Mobile_Number.Contains(keyword)
                        || q.Username.Contains(keyword)
                        || q.Email_Address.Contains(keyword)))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            } catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<String> AddMobileAppAccount(papAccountEntity papAccount, int userId)
        {
            response = new Response();

            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    papAccount.IsActive = true;
                    papAccount.ToDisplay = true;
                    papAccount.Added_By = userId;
                    papAccount.Updated_By = userId;
                    papAccount.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                    papAccount.Last_Updated = DateTime.UtcNow.ToLocalTime();
                    papAccount.Username = papAccount.First_Name.Substring(0, 1) +
                    (papAccount.Middle_Name == null || papAccount.Middle_Name == "" ? papAccount.First_Name.Substring(1, 1) : papAccount.Middle_Name.Substring(0, 1)) +
                    papAccount.Last_Name;
                    papAccount.ConstUsername = papAccount.Username;
                    papAccount.IsPending = true;

                    var mobileAccountUsername = await _context.PapAccountEntity.Where(x => x.ConstUsername == papAccount.ConstUsername && x.IsActive == true).LastOrDefaultAsync();

                    if (mobileAccountUsername != null)
                    {
                        string[] splittedStr = Regex.Split(mobileAccountUsername.Username, @"\D+");

                        if (splittedStr[1].ToString() != "")
                        {
                            papAccount.Username = papAccount.ConstUsername + Convert.ToString(Convert.ToInt32(splittedStr[1]) + 1);
                        }
                        else
                        {
                            papAccount.Username = papAccount.Username + "1";
                        }
                    }

                    papAccount.Username = papAccount.Username.ToLower();
                    papAccount.Username = papAccount.Username.Replace(" ", "");
                    papAccount.ConstUsername = papAccount.ConstUsername.ToLower();
                    papAccount.Account_Code = System.Guid.NewGuid().ToString();
                    papAccount.Password = DateTime.Now.ToString("yyMMddHHmmss");

                    papAccount.Lkd_Students = string.Empty;

                    foreach (var student in papAccount.Linked_Students)
                    {
                        papAccount.Lkd_Students += ", " + student.value;
                    }

                    if (papAccount.Lkd_Students.Substring(0, 1).IndexOf(',') >= 0)
                    {
                        papAccount.Lkd_Students = papAccount.Lkd_Students.Substring(1, papAccount.Lkd_Students.Length - 1);
                        papAccount.Lkd_Students = papAccount.Lkd_Students.Trim();
                    }

                    await _context.PapAccountEntity.AddAsync(papAccount);

                    foreach (var student in papAccount.Linked_Students)
                    {
                        papAccountLinkedStudentsEntity papAccountLinkedStudents = new papAccountLinkedStudentsEntity();

                        personEntity studentInfo = await _context.PersonEntity.Where(x => x.ID_Number == student.key && x.IsActive == true).FirstOrDefaultAsync();

                        papAccountLinkedStudents.PAP_Account_ID = papAccount.ID;
                        papAccountLinkedStudents.Person_ID = studentInfo.Person_ID;

                        await _context.PapAccountLinkedStudentsEntity.AddAsync(papAccountLinkedStudents);
                    }

                    await _context.SaveChangesAsync();

                    contextTransaction.Commit();

                    //return response.CreateResponse("200", Constants.SuccessPasswordResetEmailSend + " " + papAccount.Email_Address, true);
                    return papAccount.Account_Code;
                }
                catch (Exception err)
                {
                    contextTransaction.Rollback();
                    //return response.CreateResponse("500", Constants.FailedMessageCreate + RepositoryName + ".", false);
                    return string.Empty;
                }
            }
        }

        public async Task<ResultModel> UpdateMobileAppAccount(papAccountEntity papAccount, int userId)
        {
            response = new Response();

            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var papAccountData = _context.PapAccountEntity.SingleOrDefault(x => x.Account_Code == papAccount.Account_Code);

                    if (papAccountData != null)
                    {
                        papAccountData.First_Name = papAccount.First_Name;
                        papAccountData.Middle_Name = papAccount.Middle_Name;
                        papAccountData.Last_Name = papAccount.Last_Name;
                        papAccountData.Email_Address = papAccount.Email_Address;
                        papAccountData.Mobile_Number = papAccount.Mobile_Number;
                        papAccountData.IsActive = papAccount.IsActive;
                        papAccountData.ToDisplay = papAccount.ToDisplay;

                        papAccountData.Updated_By = userId;
                        papAccountData.Last_Updated = DateTime.UtcNow.ToLocalTime();

                        papAccount.Lkd_Students = string.Empty;

                        foreach (var student in papAccount.Linked_Students)
                        {
                            papAccount.Lkd_Students += ", " + student.value;
                        }

                        if (papAccount.Lkd_Students.Substring(0, 1).IndexOf(',') >= 0)
                        {
                            papAccount.Lkd_Students = papAccount.Lkd_Students.Substring(1, papAccount.Lkd_Students.Length - 1);
                            papAccount.Lkd_Students = papAccount.Lkd_Students.Trim();
                        }
                    }

                    _context.Entry(papAccountData).Property(x => x.Date_Time_Added).IsModified = false;
                    _context.Entry(papAccountData).Property(x => x.Added_By).IsModified = false;
                    _context.Entry(papAccountData).Property(x => x.Last_Updated).IsModified = true;

                    ICollection<papAccountLinkedStudentsEntity> papAccountLinkedStudents = await _context.PapAccountLinkedStudentsEntity.Include(q => q.PapAccountEntity).Where(q => q.PapAccountEntity.Account_Code == papAccount.Account_Code).ToListAsync();

                    if(papAccountLinkedStudents != null)
                    {
                        _context.PapAccountLinkedStudentsEntity.RemoveRange(papAccountLinkedStudents);
                    }

                    foreach (var student in papAccount.Linked_Students)
                    {
                        papAccountLinkedStudentsEntity newPapAccountLinkedStudents = new papAccountLinkedStudentsEntity();

                        personEntity studentInfo = await _context.PersonEntity.Where(x => x.ID_Number == student.key && x.IsActive == true).FirstOrDefaultAsync();

                        newPapAccountLinkedStudents.PAP_Account_ID = papAccountData.ID;
                        newPapAccountLinkedStudents.Person_ID = studentInfo.Person_ID;

                        await _context.PapAccountLinkedStudentsEntity.AddAsync(newPapAccountLinkedStudents);
                    }

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();

                    
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return response.CreateResponse("500", Constants.FailedMessageUpdate + RepositoryName + ".", false);
                }
            }

            return response.CreateResponse("200", Constants.SuccessUpdateAccount, true);
        }

        public async Task<ResultModel> DeleteMobileAppTemporary(string accountCode, int userId)
        {
            response = new Response();

            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var papAccountData = _context.PapAccountEntity.SingleOrDefault(x => x.Account_Code == accountCode);

                    papAccountData.IsActive = false;
                    papAccountData.Updated_By = userId;
                    papAccountData.Last_Updated = DateTime.UtcNow.ToLocalTime();

                    _context.Entry(papAccountData).Property(x => x.Date_Time_Added).IsModified = false;
                    _context.Entry(papAccountData).Property(x => x.Added_By).IsModified = false;
                    _context.Entry(papAccountData).Property(x => x.Last_Updated).IsModified = true;

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return response.CreateResponse("500", Constants.FailedMessageDelete + RepositoryName + ".", false);
                }
            }

            return response.CreateResponse("200", RepositoryName + Constants.SuccessMessageDelete, true);
        }

        public async Task<ResultModel> AccountActivation(string password, string accountCode, int userId)
        {
            response = new Response();
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var papAccountData = _context.PapAccountEntity.SingleOrDefault(x => x.Account_Code == accountCode);

                    if (papAccountData != null)
                    {
                        papAccountData.IsPending = false;
                        papAccountData.Password = password;

                        papAccountData.Updated_By = userId;
                        papAccountData.Last_Updated = DateTime.UtcNow.ToLocalTime();
                    }

                    _context.Entry(papAccountData).Property(x => x.Date_Time_Added).IsModified = false;
                    _context.Entry(papAccountData).Property(x => x.Added_By).IsModified = false;
                    _context.Entry(papAccountData).Property(x => x.Last_Updated).IsModified = true;

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return response.CreateResponse("500", Constants.FailedActivation, false);
                }
            }

            return response.CreateResponse("200", Constants.SuccessActivation, true);
        }

        public async Task<ResultModel> RequestChangePassword(string accountCode, int userId)
        {
            response = new Response();
            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var papAccountData = _context.PapAccountEntity.SingleOrDefault(x => x.Account_Code == accountCode);

                    if (papAccountData != null)
                    {
                        papAccountData.IsRequestChangePassword = true;

                        papAccountData.Updated_By = userId;
                        papAccountData.Last_Updated = DateTime.UtcNow.ToLocalTime();
                    }

                    _context.Entry(papAccountData).Property(x => x.Date_Time_Added).IsModified = false;
                    _context.Entry(papAccountData).Property(x => x.Added_By).IsModified = false;
                    _context.Entry(papAccountData).Property(x => x.Last_Updated).IsModified = true;

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return response.CreateResponse("500", Constants.FailedActivation, false);
                }
            }

            return response.CreateResponse("200", Constants.SuccessActivation, true);
        }

        public async Task<papAccountEntity> ValidateChangePasswordLink(string accountCode)
        {
            return await this.context.PapAccountEntity.Where(x => x.Account_Code == accountCode && x.IsActive == true).FirstOrDefaultAsync();
        }

        public async Task<IList<personEntity>> GetStudentList()
        {
            return await this.context.PersonEntity.Where(x => x.IsActive == true && x.ToDisplay == true && x.Person_Type == "S").ToListAsync();
        }

        public async Task<papAccountPagedResult> Export(string keyword)
        {
            try
            {
                var result = new papAccountPagedResult();

                if (keyword == null || keyword == "")
                {
                    result.papAccounts = await _context.PapAccountEntity
                        .Where(b => b.ToDisplay == true)
                        .OrderBy(c => c.Last_Updated).ToListAsync();
                }
                else
                {
                    result.papAccounts = await _context.PapAccountEntity
                        .Where(q => q.ToDisplay == true && (q.Email_Address.Contains(keyword)
                        || q.First_Name.Contains(keyword)
                        || q.Last_Name.Contains(keyword)
                        || q.Middle_Name.Contains(keyword)
                        || q.Mobile_Number.Contains(keyword)
                        || q.Username.Contains(keyword)))
                       .OrderBy(c => c.Last_Updated).ToListAsync();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<papAccountEntity> SignIn(PapAccountSignIn accountSignIn)
        {
            return await this.context.PapAccountEntity.Where(x => x.Username == accountSignIn.username && x.Password == accountSignIn.password && x.IsPending == false && x.IsActive == true).FirstOrDefaultAsync();
        }
    }
}
