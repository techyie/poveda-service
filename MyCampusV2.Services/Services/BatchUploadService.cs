using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Castle.Core.Internal;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Common.ViewModels.OfficeViewModel;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;

namespace MyCampusV2.Services.Services
{
    public class BatchUploadService : BaseService, IBatchUploadService
    {
        private readonly BatchUploadSettings _batchSettings;

        public BatchUploadService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user, IOptions<BatchUploadSettings> batchSettings) : base(unitOfWork, audit, user)
        {
            _batchSettings = batchSettings.Value;
        }

        public async Task AddBatchUplaod(batchUploadEntity entity, int userID)
        {
            try
            {
                await _unitOfWork.BatchUploadRepository.AddAsyn(entity);
                await _unitOfWork.AuditTrailRepository.AuditAsync(userID, (int)Form.Batch_Upload, string.Format("Added: Batch Upload: {0}, Upload Module: {1}", entity.Filename, entity.Form_ID));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteBatchUplaod(int id, int user)
        {
            try
            {
                batchUploadEntity entity = await GetByIdSync(id);

                if (entity.IsActive)
                {
                    entity.IsActive = false;
                }
                else
                {
                    entity.IsActive = true;
                }

                await _unitOfWork.BatchUploadRepository.UpdateAsyn(entity, entity.ID);
                await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Batch_Upload, string.Format("Update: Batch Upload: {0}, Upload Module: {1}", entity.Filename, entity.Form_ID));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ICollection<batchUploadEntity>> GetAll()
        {
            return await _unitOfWork.BatchUploadRepository.GetAllAsyn();
        }

        public batchUploadEntity GetById(int id)
        {
            return _unitOfWork.BatchUploadRepository.Get(id);
        }

        public async Task<batchUploadEntity> GetByIdSync(int id)
        {
            return await _unitOfWork.BatchUploadRepository.GetByID_(id);
        }

        public async Task UpdateBatchUplaod(batchUploadEntity entity, int user)
        {
            try
            {
                batchUploadEntity oldBatch = await GetByIdSync(entity.ID);
                await _unitOfWork.BatchUploadRepository.UpdateAsyn(entity, entity.ID);
                await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Batch_Upload, string.Format("Update: Batch Upload: {0}, Upload Module: {1} Status:{2} Old: {3}, Upload Module: {4} Status:{5}", entity.Filename, entity.Form_ID, entity.Status, oldBatch.Filename, oldBatch.Form_ID, oldBatch.Status));
                //await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Campus_Course, string.Format("Updated: Course Code: {4} to {5}, Course Name: {0} to {2}, Course Description: {1} to {3} ", oldCourse.Course_Name, oldCourse.Course_Desc, course.Course_Name, course.Course_Desc, oldCourse.Course_Code, course.Course_Code));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Upload(IFormFile file, batchUploadEntity entity, int userID)
        {
            // Cancel all the user onprocess of upload status.
            var uplaodOnProcess = await _unitOfWork.BatchUploadRepository.GetByUserOnProcess(userID);
            foreach (var process in uplaodOnProcess)
            {
                process.Status = "CANCELLED";
                await _unitOfWork.BatchUploadRepository.UpdateAsyn(process, process.ID);
            }

            var filesPath = _batchSettings.Loc;
            if (!Directory.Exists(filesPath))
                Directory.CreateDirectory(filesPath);
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
            var fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + userID + Path.GetExtension(originalFileName);


            fileName = fileName.Contains("\\")
                ? fileName.Trim('"').Substring(fileName.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                : fileName.Trim('"');

            var fullFilePath = Path.Combine(filesPath, fileName);
            if (file.Length > 0)
            {

                entity.Filename = originalFileName;
                entity.Path = fullFilePath;
                entity.User_ID = userID;
                entity.Status = "UPLOAD";
                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            await _unitOfWork.BatchUploadRepository.AddAsyn(entity);
        }
        
        public int GetRecordsCount(IFormFile file)
        {
            var count = 0;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var fileStream = file.OpenReadStream())
            {
                using (var reader = ExcelReaderFactory.CreateReader(fileStream))
                {
                    count = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    }).Tables[0].Rows.Count;
                }
            }

            return count;
        }

        public DataTable GetDataForUpload(string path)
        {
            var result = new DataTable();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    }).Tables[0];
                }
            }
            return result;
        }

        #region Campus

        public IEnumerable<campusBatchUploadVM> GetCampusRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<campusBatchUploadVM>();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                var temp = new campusBatchUploadVM()
                {
                    CampusCode = row["CampusCode"].ToString(),
                    CampusName = row["CampusName"].ToString(),
                    CampusStatus = row["CampusStatus"].ToString(),
                    CampusAddress = row["CampusAddress"].ToString(),
                    CampusContactNo = row["CampusContactNo"].ToString(),
                    Region = row["Region"].ToString(),
                    Division = row["Division"].ToString()
                };
                retList.Add(temp);
            }
            return retList;
        }
        
        public IEnumerable<educLevelBatchUploadVM> GetEducLevelRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<educLevelBatchUploadVM>();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                var temp = new educLevelBatchUploadVM()
                {
                    CampusName = row["CampusName"].ToString(),
                    EducationalLevelName = row["EducationalLevelName"].ToString(),
                    //EducationalLevelStatus = row["EducationalLevelStatus"].ToString(),
                    College = row["College"].ToString()
                };
                retList.Add(temp);
            }
            return retList;
        }

        public IEnumerable<yearSectionBatchUploadVM> GetYearSectionRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<yearSectionBatchUploadVM>();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                var temp = new yearSectionBatchUploadVM()
                {
                    CampusName = row["CampusName"].ToString(),
                    EducationalLevelName = row["EducationalLevelName"].ToString(),
                    YearSecName = row["YearLevelName"].ToString()
                };
                retList.Add(temp);
            }
            return retList;
        }

        public IEnumerable<studentSectionBatchUploadVM> GetSectionRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<studentSectionBatchUploadVM>();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                var temp = new studentSectionBatchUploadVM()
                {
                    CampusName = row["CampusName"].ToString(),
                    EducationalLevelName = row["EducationalLevelName"].ToString(),
                    YearLevelName = row["YearLevelName"].ToString(),
                    SectionName = row["SectionName"].ToString(),
                    StartTime = row["StartTime"].ToString(),
                    EndTime = row["EndTime"].ToString(),
                    HalfDay = row["HalfDay"].ToString(),
                    GracePeriod = row["GracePeriod"].ToString()
                };
                retList.Add(temp);
            }
            return retList;
        }
        
        public IEnumerable<collegeBatchUploadVM> GetCollegeRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<collegeBatchUploadVM>();

            if (result.Columns.Count == 3)
            {
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    var row = result.Rows[i];
                    var temp = new collegeBatchUploadVM()
                    {
                        CampusName = row["CampusName"].ToString(),
                        EducationalLevelName = row["EducationalLevelName"].ToString(),
                        CollegeName = row["CollegeName"].ToString()
                    };
                    retList.Add(temp);
                }
            }
            else
            {
                throw new Exception("File contains invalid or missing fields. Please download the valid template.");
            }
            return retList;
        }
        
        public IEnumerable<courseBatchUploadVM> GetCourseRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<courseBatchUploadVM>();

            if (result.Columns.Count == 4)
            {
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    var row = result.Rows[i];
                    var temp = new courseBatchUploadVM()
                    {
                        CampusName = row["CampusName"].ToString(),
                        EducationalLevelName = row["EducationalLevelName"].ToString(),
                        CollegeName = row["CollegeName"].ToString(),
                        CourseName = row["CourseName"].ToString()
                    };
                    retList.Add(temp);
                }
            }
            else
            {
                throw new Exception("File contains invalid or missing fields. Please download the valid template.");
            }
            return retList;
        }

        public IEnumerable<departmentBatchUploadVM> GetDepartmentRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<departmentBatchUploadVM>();

            if (result.Columns.Count == 2)
            {
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    var row = result.Rows[i];
                    var temp = new departmentBatchUploadVM()
                    {
                        CampusName = row["CampusName"].ToString(),
                        DepartmentName = row["DepartmentName"].ToString(),
                    };
                    retList.Add(temp);
                }
            }
            else
            {
                throw new Exception("File contains invalid or missing fields. Please download the valid template.");
            }
            return retList;
        }

        public IEnumerable<officeBatchUploadVM> GetOfficeRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<officeBatchUploadVM>();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                var temp = new officeBatchUploadVM()
                {
                    CampusName = row["CampusName"].ToString(),
                    OfficeName = row["OfficeName"].ToString(),
                    //OfficeStatus = row["OfficeStatus"].ToString(),
                };
                retList.Add(temp);
            }
            return retList;
        }

        public IEnumerable<positionBatchUploadVM> GetPositionRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<positionBatchUploadVM>();

            if (result.Columns.Count == 3)
            {
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    var row = result.Rows[i];
                    var temp = new positionBatchUploadVM()
                    {
                        CampusName = row["CampusName"].ToString(),
                        DepartmentName = row["DepartmentName"].ToString(),
                        PositionName = row["PositionName"].ToString(),
                        //PositionDescription = row["PositionDescription"].ToString()
                    };
                    retList.Add(temp);
                }
            }
            else
            {
                throw new Exception("File contains invalid or missing fields. Please download the valid template.");
            }
            return retList;
        }

        public IEnumerable<areaBatchUploadVM> GetAreaRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<areaBatchUploadVM>();

            if (result.Columns.Count == 3)
            {
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    var row = result.Rows[i];
                    var temp = new areaBatchUploadVM()
                    {
                        CampusName = row["CampusName"].ToString(),
                        AreaName = row["AreaName"].ToString(),
                        AreaDescription = row["AreaDescription"].ToString()
                    };
                    retList.Add(temp);
                }
            }
            else
            {
                throw new Exception("File contains invalid or missing fields. Please download the valid template.");
            }
            return retList;
        }

        public IEnumerable<papAccountBatchUploadVM> GetPapAccountRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<papAccountBatchUploadVM>();

            if (result.Columns.Count == 6)
            {
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    var row = result.Rows[i];
                    var temp = new papAccountBatchUploadVM()
                    {
                        FirstName = row["FirstName"].ToString(),
                        MiddleName = row["MiddleName"].ToString(),
                        LastName = row["LastName"].ToString(),
                        EmailAddress = row["EmailAddress"].ToString(),
                        MobileNumber = row["MobileNumber"].ToString(),
                        LinkedStudents = row["LinkedStudents"].ToString()
                    };
                    retList.Add(temp);
                }
            }
            else
            {
                throw new Exception("File contains invalid or missing fields. Please download the valid template.");
            }
            return retList;
        }

        public IEnumerable<schoolCalendarBatchUploadVM> GetCalendarRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<schoolCalendarBatchUploadVM>();

            if (result.Columns.Count != 33)
            {
                throw new Exception("File contains invalid or missing fields. Please download the valid template.");
            }

            string firstYear = "";
            string lastYear = "";

            int xx = 0;
            while (xx < result.Rows.Count)
            {
                if (!string.IsNullOrEmpty(result.Rows[xx][0].ToString()))
                {
                    firstYear = result.Rows[xx][0].ToString();
                    break;
                }
                xx++;
            }

            int y = result.Rows.Count - 1;
            while (y != xx)
            {
                if (!string.IsNullOrEmpty(result.Rows[y][0].ToString()))
                {
                    lastYear = result.Rows[y][0].ToString();
                    break;
                }
                y--;
            }

            string schoolYear = firstYear + " - " + lastYear;

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                string dayList = string.Empty;
                int currYear = string.IsNullOrEmpty(row["Year"].ToString()) ? 0 : Convert.ToInt32(row["Year"]);
                int currMonth = string.IsNullOrEmpty(row["Month"].ToString()) ? 0 : DateTime.ParseExact(row["Month"].ToString(), "MMMM", CultureInfo.CurrentCulture).Month;
                int maxDays = 0;

                if (currYear != 0 && currMonth != 0)
                    maxDays = DateTime.DaysInMonth(currYear, currMonth);

                for (int x = 1; x <= maxDays; x++)
                {
                    if (row[""+x+""].ToString() == "1")
                    {
                        dayList = dayList + "," + x;
                    }
                }

                dayList = string.IsNullOrEmpty(dayList) ? string.Empty : dayList.Substring(1, dayList.Length - 1);

                var temp = new schoolCalendarBatchUploadVM()
                {
                    SchoolYear = schoolYear,
                    Year = currYear,
                    Month = currMonth,
                    Days = dayList
                };
                retList.Add(temp);
            }
            return retList;
        }

        #endregion

        #region Person

        public IEnumerable<personStudentBatchUploadVM> GetStudentRecords(string path, int studcols, int studcollegecols)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<personStudentBatchUploadVM>();

            if (result.Columns.Count == studcols)
            {
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    var row = result.Rows[i];
                    var temp = new personStudentBatchUploadVM()
                    {
                        IDNumber = row["IDNumber"].ToString(),
                        FirstName = row["FirstName"].ToString(),
                        MiddleName = row["MiddleName"].ToString(),
                        LastName = row["LastName"].ToString(),
                        BirthDate = row["BirthDate"].ToString(),
                        Gender = row["Gender"].ToString(),
                        ContactNumber = row["ContactNumber"].ToString(),
                        TelephoneNumber = row["TelephoneNumber"].ToString(),
                        EmailAddress = row["EmailAddress"].ToString(),
                        Address = row["Address"].ToString(),
                        CampusName = row["CampusName"].ToString(),
                        EducationalLevelName = row["EducationalLevelName"].ToString(),
                        YearLevelName = row["YearLevelName"].ToString(),
                        SectionName = row["SectionName"].ToString(),
                        DateEnrolled = row["DateEnrolled"].ToString(),
                        EmergencyFullname = row["EmergencyFullname"].ToString(),
                        EmergencyAddress = row["EmergencyAddress"].ToString(),
                        EmergencyContactNo = row["EmergencyContactNo"].ToString(),
                        EmergencyRelationship = row["EmergencyRelationship"].ToString(),
                        IsCollegeTemplate = false
                    };
                    retList.Add(temp);
                }
            }
            else if (result.Columns.Count == studcollegecols)
            {
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    var row = result.Rows[i];
                    var temp = new personStudentBatchUploadVM()
                    {
                        IDNumber = row["IDNumber"].ToString(),
                        FirstName = row["FirstName"].ToString(),
                        MiddleName = row["MiddleName"].ToString(),
                        LastName = row["LastName"].ToString(),
                        BirthDate = row["BirthDate"].ToString(),
                        Gender = row["Gender"].ToString(),
                        ContactNumber = row["ContactNumber"].ToString(),
                        TelephoneNumber = row["TelephoneNumber"].ToString(),
                        EmailAddress = row["EmailAddress"].ToString(),
                        Address = row["Address"].ToString(),
                        CampusName = row["CampusName"].ToString(),
                        EducationalLevelName = row["EducationalLevelName"].ToString(),
                        YearLevelName = row["YearLevelName"].ToString(),
                        SectionName = row["SectionName"].ToString(),
                        CollegeName = row["CollegeName"].ToString(),
                        CourseName = row["CourseName"].ToString(),
                        DateEnrolled = row["DateEnrolled"].ToString(),
                        EmergencyFullname = row["EmergencyFullname"].ToString(),
                        EmergencyAddress = row["EmergencyAddress"].ToString(),
                        EmergencyContactNo = row["EmergencyContactNo"].ToString(),
                        EmergencyRelationship = row["EmergencyRelationship"].ToString(),
                        IsCollegeTemplate = true
                    };
                    retList.Add(temp);
                }
            }
            else
            {
                throw new Exception("File contains invalid or missing fields. Please download the valid template.");
            }

            return retList;
        }
    
        public IEnumerable<personEmployeeBatchUploadVM> GetEmployeeRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<personEmployeeBatchUploadVM>();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                var temp = new personEmployeeBatchUploadVM()
                {
                    IDNumber = row["IDNumber"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    MiddleName = row["MiddleName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    BirthDate = row["BirthDate"].ToString(),
                    Gender = row["Gender"].ToString(),
                    ContactNumber = row["ContactNumber"].ToString(),
                    TelephoneNumber = row["TelephoneNumber"].ToString(),
                    EmailAddress = row["EmailAddress"].ToString(),
                    Address = row["Address"].ToString(),
                    CampusName = row["CampusName"].ToString(),
                    EmployeeTypeName = row["EmployeeTypeName"].ToString(),
                    EmployeeSubTypeName = row["EmployeeSubTypeName"].ToString(),
                    DepartmentName = row["DepartmentName"].ToString(),
                    PositionName = row["PositionName"].ToString(),
                    EmergencyFullname = row["EmergencyFullname"].ToString(),
                    EmergencyMobileNo = row["EmergencyMobileNo"].ToString(),
                    EmergencyRelationship = row["EmergencyRelationship"].ToString(),
                    EmergencyAddress = row["EmergencyAddress"].ToString(),
                    SSS = row["SSS"].ToString(),
                    TIN = row["TIN"].ToString(),
                    PAGIBIG = row["PAGIBIG"].ToString(),
                    Philhealth = row["Philhealth"].ToString()
                };
                retList.Add(temp);
            }
            return retList;
        }

        public IEnumerable<visitorPersonBatchUploadVM> GetVisitorRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<visitorPersonBatchUploadVM>();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                var temp = new visitorPersonBatchUploadVM()
                {
                    IDNumber = row["IDNumber"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    MiddleName = row["MiddleName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    Gender = row["Gender"].ToString(),
                    BirthDate = row["BirthDate"].ToString(),
                    Address = row["Address"].ToString(),
                    ContactNumber = row["ContactNumber"].ToString(),
                    EmailAddress = row["EmailAddress"].ToString()
                };
                retList.Add(temp);
            }
            return retList;
        }

        public IEnumerable<personFetcherBatchUploadVM> GetFetcherRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<personFetcherBatchUploadVM>();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                var temp = new personFetcherBatchUploadVM()
                {
                    IDNumber = row["IDNumber"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    MiddleName = row["MiddleName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    BirthDate = row["BirthDate"].ToString(),
                    Gender = row["Gender"].ToString(),
                    ContactNumber = row["ContactNumber"].ToString(),
                    EmailAddress = row["EmailAddress"].ToString(),
                    Address = row["Address"].ToString(),
                    FetcherRelationship = row["Relationship"].ToString(),
                    CampusName = row["CampusName"].ToString()
                };
                retList.Add(temp);
            }

            return retList;
        }

        public IEnumerable<personOtherAccessBatchUploadVM> GetOtherAccessRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<personOtherAccessBatchUploadVM>();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                var temp = new personOtherAccessBatchUploadVM()
                {
                    IDNumber = row["IDNumber"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    MiddleName = row["MiddleName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    BirthDate = row["BirthDate"].ToString(),
                    Gender = row["Gender"].ToString(),
                    ContactNumber = row["ContactNumber"].ToString(),
                    TelephoneNumber = row["TelephoneNumber"].ToString(),
                    EmailAddress = row["EmailAddress"].ToString(),
                    Address = row["Address"].ToString(),
                    CampusName = row["CampusName"].ToString(),
                    DepartmentName = row["DepartmentName"].ToString(),
                    PositionName = row["PositionName"].ToString(),
                    OfficeName = row["OfficeName"].ToString(),
                    EmergencyFullname = row["EmergencyFullname"].ToString(),
                    EmergencyMobileNo = row["EmergencyMobileNo"].ToString(),
                    EmergencyRelationship = row["EmergencyRelationship"].ToString(),
                    EmergencyAddress = row["EmergencyAddress"].ToString()
                };
                retList.Add(temp);
            }
            return retList;
        }

        #endregion

        #region Card

        public IEnumerable<cardBatchUpdateVM> GetCardUpdateRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<cardBatchUpdateVM>();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                var temp = new cardBatchUpdateVM()
                {
                    IDNumber = row["IDNumber"].ToString(),
                    ExpiryDate = row["ExpiryDate"].ToString()
                };
                retList.Add(temp);
            }
            return retList;
        }

        public IEnumerable<cardBatchDeactiveVM> GetCardDeactivateRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<cardBatchDeactiveVM>();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                var temp = new cardBatchDeactiveVM()
                {
                    IdNumber = row["IdNumber"].ToString()
                };
                retList.Add(temp);
            }
            return retList;
        }

        #endregion

        #region Notification

        public IEnumerable<notificationBatchUploadVM> GetGeneralNotifRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<notificationBatchUploadVM>();
            
            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                var temp = new notificationBatchUploadVM()
                {
                    Message = row["Message"].ToString(), 
                    DateFrom = row["DateFrom"].ToString(),
                    DateTo = row["DateTo"].ToString()
                };
                retList.Add(temp);
            }
            return retList;
        }

        public IEnumerable<personalNotificationBatchUploadVM> GetPersonalNotifRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<personalNotificationBatchUploadVM>();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                var temp = new personalNotificationBatchUploadVM()
                {
                    Message = row["Message"].ToString(),
                    DateFrom = row["DateFrom"].ToString(),
                    DateTo = row["DateTo"].ToString(),
                    IDNumber = row["IDNumber"].ToString()
                };
                retList.Add(temp);
            }
            return retList;
        }

        #endregion

        #region Fetcher

        public IEnumerable<emergencyLogoutBatchUploadVM> GetEmergencyLogoutRecords(string path)
        {
            DataTable result = GetDataForUpload(path);
            var retList = new List<emergencyLogoutBatchUploadVM>();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var row = result.Rows[i];
                var temp = new emergencyLogoutBatchUploadVM()
                {
                    Student = row["Student"].ToString(),
                    Remarks = row["Remarks"].ToString(),
                    Date = row["Date"].ToString()
                };
                retList.Add(temp);
            }
            return retList;
        }

        #endregion
    }
}
