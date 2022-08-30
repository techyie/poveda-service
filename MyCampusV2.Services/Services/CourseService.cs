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
    public class CourseService : BaseService, ICourseService
    {
        private string _courseBatch = AppDomain.CurrentDomain.BaseDirectory + @"Course\";
        private ResultModel result = new ResultModel();

        public CourseService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<IList<courseEntity>> GetCoursesUsingCollegeId(int id)
        {
            return await _unitOfWork.CourseRepository.GetCoursesUsingCollegeId(id);
        }

        public async Task<courseEntity> GetCourseById(int id)
        {
            return await _unitOfWork.CourseRepository.GetCourseById(id);
        }

        public async Task<ResultModel> AddCourse(courseEntity course)
        {
            try
            {
                course.Course_Status = "Active";
                course.IsActive = true;

                var exist = await _unitOfWork.CourseRepository.FindAsync(q => q.Course_Name == course.Course_Name && q.IsActive == true && q.College_ID == course.College_ID);

                if (exist != null)
                    return CreateResult("409", COURSE_EXIST, false);

                var data = await _unitOfWork.CourseRepository.AddAsyncWithBase(course);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(course.Added_By, (int)Form.Campus_Course, "Add Course", "INSERT", false, "Failed: " + course.Course_Name, DateTime.UtcNow.ToLocalTime()));
                    result = new ResultModel();
                    result.resultCode = "409";
                    result.isSuccess = false;
                    return CreateResult("409", "Course", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(course.Added_By, (int)Form.Campus_Course, "Add Course", "INSERT", true, "Success: " + course.Course_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Course" + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateCourse(courseEntity course)
        {
            try
            {
                var exist = await _unitOfWork.CourseRepository.FindAsync(q => q.Course_Name == course.Course_Name && q.IsActive == true && q.College_ID == course.College_ID && q.Course_ID != course.Course_ID);

                if (exist != null)
                    return CreateResult("409", COURSE_EXIST, false);

                var updateCheck = await _unitOfWork.CourseRepository.FindAsync(q => q.Course_ID == course.Course_ID && q.College_ID == course.College_ID);

                if (updateCheck == null)
                {
                    var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.Course_ID == course.Course_ID);

                    if (recordCountPerson != null)
                    {
                        await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(course.Updated_By, (int)Form.Campus_Course, "Update Course", "UPDATE", false, "Failed due to active record: " + course.Course_Name, DateTime.UtcNow.ToLocalTime()));
                        return CreateResult("409", UNABLE_EDIT, false);
                    }
                }

                course.Course_Status = "Active";

                var data = await _unitOfWork.CourseRepository.UpdateAsyncWithBase(course, course.Course_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(course.Updated_By, (int)Form.Campus_Course, "Update Course", "UPDATE", false, "Failed: " + course.Course_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Course", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(course.Updated_By, (int)Form.Campus_Course, "Update Course", "UPDATE", true, "Success: Course ID: " + course.Course_ID + " Course Name: " + course.Course_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Course" + Constants.SuccessMessageUpdate, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteCoursePermanent(int id, int user)
        {
            try
            {
                courseEntity course = await GetCourseById(id);

                var checkIfStudentExists = await _unitOfWork.PersonRepository.FindAsync(a => a.Course_ID == id && a.Person_Type == "S" && a.ToDisplay == true);
                if (checkIfStudentExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Course, "Permanent Delete Course", "PERMANENT DELETE", false, "Unable to permanent delete " + course.Course_Name + " due to existing active student", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Unable to permanent delete.", false);
                }

                course.Updated_By = user;

                var data = await _unitOfWork.CourseRepository.DeleteAsyncPermanent(course, course.Course_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(course.Updated_By, (int)Form.Campus_Course, "Delete Permanently Course", "PERMANENT DELETE", false, "Failed: " + course.Course_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Course", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(course.Updated_By, (int)Form.Campus_Course, "Delete Permanently Course", "PERMANENT DELETE", true, "Success: " + course.Course_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Course" + Constants.SuccessMessagePermanentDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteCourseTemporary(int id, int user)
        {
            try
            {
                courseEntity course = await _unitOfWork.CourseRepository.FindAsync(q => q.Course_ID == id);

                var checkIfStudentExists = await _unitOfWork.PersonRepository.FindAsync(a => a.Course_ID == id && a.Person_Type == "S" && a.IsActive == true);
                if (checkIfStudentExists != null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Course, "Deactivate Course", "DEACTIVATE", false, "Unable to deactivate " + course.Course_Name + " due to existing active student.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", ITEM_IN_USE, false);
                }

                course.Course_Status = "Inactive";

                var data = await _unitOfWork.CourseRepository.DeleteAsyncTemporary(course, course.Course_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(course.Updated_By, (int)Form.Campus_Course, "Deactivate Course", "DEACTIVATE", false, "Failed: " + course.Course_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Course", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(course.Updated_By, (int)Form.Campus_Course, "Deactivate Course", "DEACTIVATE", true, "Success: " + course.Course_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Course" + Constants.SuccessMessageTemporaryDelete, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> RetrieveCourse(courseEntity course)
        {
            try
            {
                var newEntity = await _unitOfWork.CourseRepository.GetAsync(course.Course_ID);

                var checkIfCollegeIsActive = await _unitOfWork.CollegeRepository.FindAsync(a => a.College_ID == newEntity.College_ID);
                if (!checkIfCollegeIsActive.IsActive)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Course, "Activate College", "ACTIVATE COLLEGE", false, "Unable to activate " + newEntity.Course_Name + " due to inactive College.", DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", UNABLE_ACTIVATE("College", checkIfCollegeIsActive.College_Name), false);
                }

                newEntity.IsActive = true;
                newEntity.Course_Status = "Active";

                var data = await _unitOfWork.CourseRepository.RetrieveAsync(newEntity, newEntity.Course_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Course, "Activate Course", "ACTIVATE COURSE", false, "Failed: " + newEntity.Course_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Course", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(newEntity.Updated_By, (int)Form.Campus_Course, "Activate Course", "ACTIVATE COURSE", true, "Success: " + newEntity.Course_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Course" + Constants.SuccessMessageRetrieve, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<coursePagedResult> GetAllCourse(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.CourseRepository.GetAllCourse(pageNo, pageSize, keyword);
        }

        public async Task<coursePagedResult> ExportAllCourses(string keyword)
        {
            return await _unitOfWork.CourseRepository.ExportAllCourses(keyword);
        }

        public async Task<BatchUploadResponse> BatchUpload(ICollection<courseBatchUploadVM> courses, int user, int uploadID, int row)
        {
            ImportLog importLog = new ImportLog();
            var response = new BatchUploadResponse();

            string fileName = "Course_Import_Logs_" + uploadID.ToString("000000000000000") + ".txt";

            response.Details = new List<BatchUploadResponse.UploadDetails>();
            response.Success = 0;
            response.Failed = 0;
            response.Total = courses.Count();
            response.FileName = fileName;

            int i = 1 + row;
            foreach (var courseVM in courses)
            {
                i++;

                if (courseVM.CampusName == null || courseVM.CampusName == string.Empty)
                {
                    importLog.Logging(_courseBatch, fileName, "Row: " + i.ToString() + " ---> Campus Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (courseVM.EducationalLevelName == null || courseVM.EducationalLevelName == string.Empty)
                {
                    importLog.Logging(_courseBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (courseVM.CollegeName == null || courseVM.CollegeName == string.Empty)
                {
                    importLog.Logging(_courseBatch, fileName, "Row: " + i.ToString() + " ---> College Name is a required field.");
                    response.Failed++;
                    continue;
                }

                if (courseVM.CourseName == null || courseVM.CourseName == string.Empty)
                {
                    importLog.Logging(_courseBatch, fileName, "Row: " + i.ToString() + " ---> Course Name is a required field.");
                    response.Failed++;
                    continue;
                }
                else if (!Global.ValidateStr(courseVM.CourseName.Trim()))
                {
                    importLog.Logging(_courseBatch, fileName, "Row: " + i.ToString() + " ---> Course Name does not accept special characters except space, dash, apostrophe and underscore.");
                    response.Failed++;
                    continue;
                }
                else if (courseVM.CourseName.Trim().Length > 125)
                {
                    importLog.Logging(_courseBatch, fileName, "Row: " + i.ToString() + " ---> Course Name accepts 125 characters only.");
                    response.Failed++;
                    continue;
                }

                var campus = await _unitOfWork.CampusRepository.FindAsync(x => x.Campus_Name == courseVM.CampusName && x.IsActive == true);

                if (campus == null)
                {
                    importLog.Logging(_courseBatch, fileName, "Row: " + i.ToString() + " ---> Campus " + courseVM.CampusName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                var educlevel = await _unitOfWork.EducationLevelRepository.FindAsync(x => x.Level_Name == courseVM.EducationalLevelName && x.Campus_ID == campus.Campus_ID && x.IsActive == true && x.hasCourse == true);

                if (educlevel == null)
                {
                    importLog.Logging(_courseBatch, fileName, "Row: " + i.ToString() + " ---> Educational Level " + courseVM.EducationalLevelName + " under Campus " + courseVM.CampusName + " does not exist or not a college educational level.");
                    response.Failed++;
                    continue;
                }

                var college = await _unitOfWork.CollegeRepository.FindAsync(x => x.College_Name == courseVM.CollegeName && x.Level_ID == educlevel.Level_ID && x.IsActive == true);

                if (college == null)
                {
                    importLog.Logging(_courseBatch, fileName, "Row: " + i.ToString() + " ---> College " + courseVM.CollegeName + " under Campus " + courseVM.CampusName + " -> Educational Level " + courseVM.EducationalLevelName + " does not exist.");
                    response.Failed++;
                    continue;
                }

                courseEntity course = await _unitOfWork.CourseRepository.FindAsync(x => x.Course_Name == courseVM.CourseName && x.IsActive == true && x.College_ID == college.College_ID);

                if (course != null)
                {
                    var updateCheck = await _unitOfWork.CourseRepository.FindAsync(q => q.Course_ID == course.Course_ID && q.College_ID == course.College_ID);

                    if (updateCheck == null)
                    {
                        var recordCountPerson = await _unitOfWork.PersonRepository.FindAsync(q => q.Course_ID == course.Course_ID);

                        if (recordCountPerson != null)
                        {
                            importLog.Logging(_courseBatch, fileName, "Row: " + i.ToString() + " ---> Course " + courseVM.CourseName + ": " + UNABLE_EDIT);
                            response.Failed++;
                            continue;
                        }
                    }

                    course.Course_Name = courseVM.CourseName;
                    course.College_ID = college.College_ID;
                    course.Updated_By = user;

                    var isSuccess = await _unitOfWork.CourseRepository.UpdateAsyncWithBase(course, course.Course_ID);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Course, "Update Course By Batch", "UPDATE", true, "Success: " + course.Course_Name, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_courseBatch, fileName, "Course " + course.Course_Name + " successfully updated.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_courseBatch, fileName, "Course " + course.Course_Name + " failed to update.");
                        response.Failed++;
                        continue;
                    }
                }
                else
                {
                    course = new courseEntity();
                    course.Course_Name = courseVM.CourseName;
                    course.College_ID = college.College_ID;
                    course.IsActive = true;
                    course.Updated_By = user;
                    course.Added_By = user;
                    course.Course_Status = "Active";

                    var isSuccess = await _unitOfWork.CourseRepository.AddAsyncWithBase(course);
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(user, (int)Form.Campus_Course, "Insert Course By Batch", "INSERT", true, "Success: " + course.Course_Name, DateTime.UtcNow.ToLocalTime()));

                    if (isSuccess != null)
                    {
                        importLog.Logging(_courseBatch, fileName, "Course " + course.Course_Name + " successfully added.");
                        response.Success++;
                        continue;
                    }
                    else
                    {
                        importLog.Logging(_courseBatch, fileName, "Course " + course.Course_Name + " failed to add.");
                        response.Failed++;
                        continue;
                    }
                }
            }

            return response;
        }
    }
}
