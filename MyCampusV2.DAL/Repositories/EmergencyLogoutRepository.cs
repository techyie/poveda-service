using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.Common.Helpers;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.Helpers;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class EmergencyLogoutRepository : BaseRepository<emergencyLogoutEntity>, IEmergencyLogoutRepository
    {
        private readonly MyCampusCardContext context;
        private ResultModel result = new ResultModel();
        private Response response = new Response();
        private ErrorLogging logger = new ErrorLogging();

        public EmergencyLogoutRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<emergencyLogoutPagedResult> GetAll(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new emergencyLogoutPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.EmergencyLogoutEntity
                        .Include(x => x.PersonEntity)
                        .Where(a => a.ToDisplay == true)
                        .Count();
                else
                    result.RowCount = this.context.EmergencyLogoutEntity
                        .Include(x => x.PersonEntity)
                        .Where(q => q.ToDisplay == true && (q.PersonEntity.ID_Number.Contains(keyword)
                            || q.PersonEntity.Last_Name.Contains(keyword)
                            || q.PersonEntity.First_Name.Contains(keyword)
                            || q.EffectivityDate.ToString().Contains(keyword)
                            || q.Remarks.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.emergencyLogouts = await this.context.EmergencyLogoutEntity
                        .Include(x => x.PersonEntity)
                        .Where(a => a.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.emergencyLogouts = await this.context.EmergencyLogoutEntity
                        .Include(x => x.PersonEntity)
                        .Where(q => q.ToDisplay == true && (q.PersonEntity.ID_Number.Contains(keyword)
                            || q.PersonEntity.Last_Name.Contains(keyword)
                            || q.PersonEntity.First_Name.Contains(keyword)
                            || q.EffectivityDate.ToString().Contains(keyword)
                            || q.Remarks.Contains(keyword)))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                logger.Error("Emergency Log Out Get All", e.Message);
                return null;
            }
        }

        public async Task<studentListPagedResult> GetStudentList(int campusId, int educLevelId, int yearSecId, int studSecId)
        {
            try
            {
                var result = new studentListPagedResult();

                if(yearSecId != 0 && studSecId != 0)
                    result.studentList = await _context.PersonEntity
                        .OrderBy(e => e.ID_Number)
                        .Where(x => x.Campus_ID == campusId
                            && x.Educ_Level_ID == educLevelId
                            && x.Year_Section_ID == yearSecId
                            && x.StudSec_ID == studSecId
                            && x.IsActive == true 
                            && x.Person_Type == "S").ToListAsync();
                else if (yearSecId != 0 && studSecId == 0)
                    result.studentList = await _context.PersonEntity
                        .OrderBy(e => e.ID_Number)
                        .Where(x => x.Campus_ID == campusId
                            && x.Educ_Level_ID == educLevelId
                            && x.Year_Section_ID == yearSecId
                            && x.IsActive == true
                            && x.Person_Type == "S").ToListAsync();
                else if (yearSecId == 0 && studSecId != 0)
                    result.studentList = await _context.PersonEntity
                        .OrderBy(e => e.ID_Number)
                        .Where(x => x.Campus_ID == campusId
                            && x.Educ_Level_ID == educLevelId
                            && x.StudSec_ID == studSecId
                            && x.IsActive == true
                            && x.Person_Type == "S").ToListAsync();
                else
                    result.studentList = await _context.PersonEntity
                        .OrderBy(e => e.ID_Number)
                        .Where(x => x.Campus_ID == campusId
                            && x.Educ_Level_ID == educLevelId
                            && x.IsActive == true
                            && x.Person_Type == "S").ToListAsync();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ResultModel> EmergencyLogout(string[] studentList, string studentRemarks, string studentEffectiveDate, int user)
        {
            result = new ResultModel();

            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var terminals = await _context.TerminalEntity.Where(q => q.Terminal_Category == Encryption.Encrypt("Time And Attendance Terminal") && q.IsActive == true).ToListAsync();

                    for (int i = 0; i < studentList.Length; i++)
                    {
                        var student = await _context.PersonEntity.Where(q => q.Person_ID == Convert.ToInt32(studentList[i])).FirstOrDefaultAsync();

                        if(student != null)
                        {
                            var cardDetails = await _context.CardDetailsEntity.Where(q => q.Person_ID == student.Person_ID && q.IsActive == true).SingleOrDefaultAsync();

                            if (cardDetails != null)
                            {
                                var emergencyExist = await _context.EmergencyLogoutEntity.Where(q => q.Person_ID == student.Person_ID && (q.EffectivityDate != null ? DateTime.Parse(q.EffectivityDate.ToString("yyyy-MM-dd")) : q.EffectivityDate) == DateTime.Parse(studentEffectiveDate.ToString()) && q.IsCancelled == false).FirstOrDefaultAsync();
                                
                                if (emergencyExist == null)
                                {
                                    emergencyLogoutEntity elEntity = new emergencyLogoutEntity();
                                    elEntity.Person_ID = Convert.ToInt32(studentList[i]);
                                    elEntity.Remarks = studentRemarks;
                                    elEntity.EffectivityDate = DateTime.Parse(studentEffectiveDate.ToString());
                                    elEntity.IsCancelled = false;
                                    elEntity.User_ID = user;
                                    elEntity.Added_By = user;
                                    elEntity.Updated_By = user;
                                    elEntity.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                                    elEntity.Last_Updated = DateTime.UtcNow.ToLocalTime();
                                    elEntity.IsActive = true;
                                    elEntity.ToDisplay = true;

                                    await _context.EmergencyLogoutEntity.AddAsync(elEntity);

                                    foreach (var terminal in terminals)
                                    {
                                        datasyncEmergencyEntity datasyncElEntity = new datasyncEmergencyEntity();
                                        datasyncElEntity.DS_Action = "A";
                                        datasyncElEntity.EffectivityDate = DateTime.Parse(studentEffectiveDate.ToString());
                                        datasyncElEntity.StudentSerial = cardDetails.Card_Serial;
                                        datasyncElEntity.Terminal_ID = terminal.Terminal_ID;
                                        datasyncElEntity.User_ID = user;

                                        await _context.DataSyncEmergencyEntity.AddAsync(datasyncElEntity);
                                    }
                                }
                                else
                                {
                                    emergencyExist.Remarks = studentRemarks;
                                    emergencyExist.Updated_By = user;
                                    emergencyExist.Last_Updated = DateTime.UtcNow.ToLocalTime();
                                    emergencyExist.IsActive = true;
                                    emergencyExist.ToDisplay = true;

                                    _context.EmergencyLogoutEntity.Update(emergencyExist);

                                    foreach (var terminal in terminals)
                                    {
                                        datasyncEmergencyEntity datasyncElEntity = new datasyncEmergencyEntity();
                                        datasyncElEntity.DS_Action = "U";
                                        datasyncElEntity.EffectivityDate = DateTime.Parse(studentEffectiveDate.ToString());
                                        datasyncElEntity.StudentSerial = cardDetails.Card_Serial;
                                        datasyncElEntity.Terminal_ID = terminal.Terminal_ID;
                                        datasyncElEntity.User_ID = user;

                                        await _context.DataSyncEmergencyEntity.AddAsync(datasyncElEntity);
                                    }
                                }
                            }
                        }
                    }
                       

                    await _context.SaveChangesAsync();
                    contextTransaction.Commit();

                    return response.CreateResponse("200", "Items in Emergency Logout By Batch" + Constants.SuccessMessageAdd, true);
                }
                catch (Exception err)
                {
                    contextTransaction.Rollback();
                    logger.Error("Emergency Log Out Get All", err.Message);
                    return response.CreateResponse("500", err.Message, false);
                }
            }
        }

        public async Task<emergencyLogoutEntity> GetEmergencyLogoutById(int id)
        {
            try
            {
                return await this.context.EmergencyLogoutEntity.Include(q => q.PersonEntity).Where(x => x.Emergency_logout_ID == id).SingleOrDefaultAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<emergencyLogoutPagedResult> ExportEmergencyLogoutStudentsExcelFile(
            int campusId,
            int educLevelId,
            int yearSecId,
            int studSecId)
        {
            var result = new emergencyLogoutPagedResult();

            result.emergencyLogouts = await _context.EmergencyLogoutEntity
                .Include(a => a.PersonEntity)
                .Where(q => q.ToDisplay == true
                    && q.PersonEntity.Person_Type == "S"
                    && (campusId.Equals(0) ? q.PersonEntity.Campus_ID != 0 : q.PersonEntity.Campus_ID == campusId)
                    && (educLevelId.Equals(0) ? q.PersonEntity.Educ_Level_ID != 0 : q.PersonEntity.Educ_Level_ID == educLevelId)
                    && (yearSecId.Equals(0) ? q.PersonEntity.Year_Section_ID != 0 : q.PersonEntity.Year_Section_ID == yearSecId)
                    && (studSecId.Equals(0) ? q.PersonEntity.StudSec_ID != 0 : q.PersonEntity.StudSec_ID == studSecId)).ToListAsync();

            return result;
        }
    }
}
