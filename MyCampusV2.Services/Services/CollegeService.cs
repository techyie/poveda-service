using Microsoft.EntityFrameworkCore;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services
{
    public class CollegeService : BaseService, ICollegeService
    {
        private string _collegeBatch = AppDomain.CurrentDomain.BaseDirectory + @"College\";
        private ResultModel result = new ResultModel();

        public CollegeService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<IList<collegeEntity>> GetCollegesUsingEducationalLevelId(int id)
        {
            return await _unitOfWork.CollegeRepository.GetCollegesUsingEducationalLevelId(id);
        }

        public async Task<collegeEntity> GetCollegeById(int id)
        {
            return await _unitOfWork.CollegeRepository.GetCollegeById(id);
        }

        public async Task<ResultModel> AddCollege(collegeEntity college)
        {
            try
            {
                college.College_Status = "Active";
                college.IsActive = true;

                var exist = await _unitOfWork.CollegeRepository.FindAsync(q => q.College_Name == college.College_Name && q.IsActive == true && q.Level_ID == college.Level_ID);

                if (exist != null)
                    return CreateResult("409", COLLEGE_EXIST, false);

                var data = await _unitOfWork.CollegeRepository.AddAsyncWithBase(college);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(college.Added_By, (int)Form.Campus_College, "Add College", "INSERT", false, "Failed: " + college.College_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "College", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(college.Added_By, (int)Form.Campus_College, "Add College", "INSERT", true, "Success: " + college.College_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "College" + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateCollege(collegeEntity college)
        {
            try
            {
                var exist = await _unitOfWork.CollegeRepository.FindAsync(q => q.College_Name == college.College_Name && q.IsActive == true && q.Level_ID == college.Level_ID && q.College_ID != college.College_ID);

                if (exist != null)
                    return CreateResult("409", COLLEGE_EXIST, false);

                var updateCheck = await _unitOfWork.CollegeRepository.FindAsync(q => q.College_ID == college.College_ID && q.Campus_ID == college.Campus_ID && q.Level_ID == college.Level_ID);

                if (updateCheck == null)
                {
                    var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.College_ID == college.College_ID);

                    if (recordCountPerson != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(college.Updated_By, (int)Form.Campus_College, "Update College", "UPDATE", false, "Failed due to active record: " + college.College_Name, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }

                    var recordCountCourse = await _unitOfWork.CourseRepository.FindAsync(q => q.College_ID == college.College_ID && q.IsActive == true);

                    if (recordCountCourse != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(college.Updated_By, (int)Form.Campus_College, "Update College", "UPDATE", false, "Failed due to active record: " + college.College_Name, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }
                }

                college.College_Status = "Active";

                var data = await _unitOfWork.CollegeRepository.UpdateAsyncWithBase(college, college.College_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(college.Updated_By, (int)Form.Campus_College, "Update College", "UPDATE", false, "Failed: " + college.College_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "College", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(college.Updated_By, (int)Form.Campus_College, "Update College", "UPDATE", true, "Success: College ID: " + college.College_ID + " Area Name: " + college.College_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "College" + Constants.SuccessMessageUpdate, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteCollegePermanent(int id, int user)
        {
            try
            {
                collegeEntity college = await GetCollegeById(id);

                var checkIfCourseExists = await _unitOfWork.CourseRepository.FindAsync(a => a.College_ID == id && a.ToDisplay == true);
                if (checkIfCourseExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_College, "Permanent Delete College", "PERMANENT DELETE", false, "Unable to permanent delete " + college.College_Name + " due to existing active course", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Unable to permanent delete.", false);
                }

                college.Updated_By = user;

                var data = await _unitOfWork.CollegeRepository.DeleteAsyncPermanent(college, college.College_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(college.Updated_By, (int)Form.Campus_College, "Delete Permanently College", "PERMANENT DELETE", false, "Failed: " + college.College_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "College", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(college.Updated_By, (int)Form.Campus_College, "Delete Permanently College", "PERMANENT DELETE", true, "Success: " + college.College_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "College" + Constants.SuccessMessagePermanentDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteCollegeTemporary(int id, int user)
        {
            try
            {
                collegeEntity college = await GetCollegeById(id);

                var checkIfCourseExists = await _unitOfWork.CourseRepository.FindAsync(a => a.College_ID == id && a.IsActive == true);
                if (checkIfCourseExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_College, "Deactivate College", "DEACTIVATE", false, "Unable to deactivate " + college.College_Name + " due to existing active course/s.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", ITEM_IN_USE, false);
                }

                college.Updated_By = user;
                college.College_Status = "Inactive";

                var data = await _unitOfWork.CollegeRepository.DeleteAsyncTemporary(college, college.College_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(college.Updated_By, (int)Form.Campus_College, "Deactivate College", "DEACTIVATE COLLEGE", false, "Failed: " + college.College_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "College", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(college.Updated_By, (int)Form.Campus_College, "Deactivate College", "DEACTIVATE COLLEGE", true, "Success: " + college.College_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "College" + Constants.SuccessMessageTemporaryDelete, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> RetrieveCollege(collegeEntity college)
        {
            try
            {
                var newEntity = await _unitOfWork.CollegeRepository.GetAsync(college.College_ID);

                var checkIfEducLevelIsActive = await _unitOfWork.EducationLevelRepository.FindAsync(a => a.Level_ID == newEntity.Level_ID);
                if (!checkIfEducLevelIsActive.IsActive)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_College, "Activate College", "ACTIVATE COLLEGE", false, "Unable to activate " + newEntity.College_Name + " due to inactive Educational Level.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", UNABLE_ACTIVATE("Educational Level", checkIfEducLevelIsActive.Level_Name), false);
                }

                newEntity.IsActive = true;
                newEntity.College_Status = "Active";

                var data = await _unitOfWork.CollegeRepository.RetrieveAsync(newEntity, newEntity.College_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_College, "Activate College", "ACTIVATE COLLEGE", false, "Failed: " + newEntity.College_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "College", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_College, "Activate College", "ACTIVATE COLLEGE", true, "Success: " + newEntity.College_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "College" + Constants.SuccessMessageRetrieve, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<collegePagedResult> GetAllCollege(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.CollegeRepository.GetAllCollege(pageNo, pageSize, keyword);
        }

        public async Task<collegePagedResult> ExportAllColleges(string keyword)
        {
            return await _unitOfWork.CollegeRepository.ExportAllColleges(keyword);
        }

        public async Task<BatchUploadResponse> BatchUpload(ICollection<collegeBatchUploadVM> colleges, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "College_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = colleges.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var collegeVM in colleges)
            {
                i++;

                if (collegeVM.CampusName == null || collegeVM.CampusName == string.Empty)
                {
                    importLog.Logging(_collegeBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (collegeVM.EducationalLevelName == null || collegeVM.EducationalLevelName == string.Empty)
                {
                    importLog.Logging(_collegeBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (collegeVM.CollegeName == null || collegeVM.CollegeName == string.Empty)
                {
                    importLog.Logging(_collegeBatch, fileName, "Row: " + i.ToString() + " ---> College Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!Global.ValidateStr(collegeVM.CollegeName.Trim()))
                {
                    importLog.Logging(_collegeBatch, fileName, "Row: " + i.ToString() + " ---> College Name does not accept special characters except space, dash, apostrophe and underscore.");
                    response.Failed++;
                    continue;
                }
                else if (collegeVM.CollegeName.Trim().Length > 150)
                {
                    importLog.Logging(_collegeBatch, fileName, "Row: " + i.ToString() + " ---> College Name accepts 150 characters only.");
                    response.Failed++;
                    continue;
                }

                var campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == collegeVM.CampusName && x.IsActive == true);

                if (campus == null)
                {
                    importLog.Logging(_collegeBatch, fileName, "Row: " + i.ToString() + " ---> Campus " + collegeVM.CampusName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                var educlevel = await _unitOfWork.EducationLevelRepository.FindAsync(x => x.Level_Name == collegeVM.EducationalLevelName && x.Campus_ID == campus.Campus_ID && x.IsActive == true);

                if (educlevel == null)
                {
                    importLog.Logging(_collegeBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level " + collegeVM.EducationalLevelName + " under Campus " + collegeVM.CampusName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                if (!educlevel.hasCourse)
                {
                    importLog.Logging(_collegeBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level " + collegeVM.EducationalLevelName + " under Campus " + collegeVM.CampusName + " is not valid for College.");
                    response.Failed++;
                    continue;
                }

                collegeEntity college = await _unitOfWork.CollegeRepository.FindAsync(x => x.College_Name == collegeVM.CollegeName && x.IsActive == true && x.Level_ID == educlevel.Level_ID);

                if (college != null)
                {
                    var updateCheck = await _unitOfWork.CollegeRepository.FindAsync(q => q.College_ID == college.College_ID && q.Campus_ID == college.Campus_ID && q.Level_ID == college.Level_ID);

                    if (updateCheck == null)
                    {
                        var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.College_ID == college.College_ID);

                        if (recordCountPerson != null)
                        {
                            importLog.Logging(_collegeBatch, fileName, "Row: " + i.ToString() + " ---> College " + collegeVM.CollegeName + ": " + UNABLE_EDIT);
                            response.Failed++;
                            continue;
                        }

                        var recordCountCourse = await _unitOfWork.CourseRepository.FindAsync(q => q.College_ID == college.College_ID && q.IsActive == true);

                        if (recordCountCourse != null)
                        {
                            importLog.Logging(_collegeBatch, fileName, "Row: " + i.ToString() + " ---> College " + collegeVM.CollegeName + ": " + UNABLE_EDIT);
                            response.Failed++;
                            continue;
                        }
                    }

                    college.College_Name = collegeVM.CollegeName;
                    college.Level_ID = educlevel.Level_ID;
                    college.Updated_By = user;

                    var isSuccess = await _unitOfWork.CollegeRepository.UpdateAsyncWithBase(college, college.College_ID);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_College, "Update College By Batch", "UPDATE", true, "Success: " + college.College_Name, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_collegeBatch, fileName, "College " + college.College_Name + " successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_collegeBatch, fileName, "College " + college.College_Name + " failed to update.");
                        response.Failed++;
                        continue;
                    }
                }
                else
                {
                    college = new collegeEntity();
                    college.College_Name = collegeVM.CollegeName;
                    college.Level_ID = educlevel.Level_ID;
                    college.College_Status = "Active";
                    college.Added_By = user;
                    college.Updated_By = user;
                    college.IsActive = true;

                    var isSuccess = await _unitOfWork.CollegeRepository.AddAsyncWithBase(college);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_College, "Insert College By Batch", "INSERT", true, "Success: " + college.College_Name, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_collegeBatch, fileName, "College " + college.College_Name + " successfully added.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_collegeBatch, fileName, "College " + college.College_Name + " failed to add.");
                        response.Failed++;
                        continue;
                    }
                }
            }

            return response;
        }
    }
}
