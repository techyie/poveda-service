using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Helpers.Constants;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.IServices;
using MyCampusV2.Helpers.ActionFilters;
using MyCampusV2.Helpers.PageResult;
using MyCampusV2.Common;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using OfficeOpenXml;
using MyCampusV2.Helpers.ExcelHelper;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml.Style;
using System.Drawing;

namespace MyCampusV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : BaseController
    {
        private readonly IPersonService _person;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _environment;
        private readonly IBatchUploadService _batchUploadService;
        private readonly BatchUploadSettings _batchSettings;
        private readonly ExcelHelper _excelHelper;

        public PersonController(IPersonService person, IMapper mapper, IHostingEnvironment environment, IBatchUploadService batchUploadService, IOptions<BatchUploadSettings> batchSettings, IOptions<ExcelHelper> excelHelper)
        {
            _person = person;
            _mapper = mapper;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _batchUploadService = batchUploadService;
            _batchSettings = batchSettings.Value;
            _excelHelper = excelHelper.Value;
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("exportemployeelist")]
        public async Task<employeePagedResultVM> ExportEmployeeList([FromQuery] string keyword)
        {

            var result = _mapper.Map<employeePagedResultVM>(await _person.ExportAllEmployees(keyword == null ? string.Empty : keyword));
            return result;
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("exportvisitorlist")]
        public async Task<visitorPagedResultVM> ExportVisitorList([FromQuery] string keyword)
        {
            return _mapper.Map<visitorPagedResultVM>(await _person.ExportAllVisitors(keyword == null ? string.Empty : keyword));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("exportvisitexcelfile")]
        public async Task<IActionResult> ExportVisitorExcelFile([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<visitorPagedResultVM>(await _person.ExportAllVisitors(param.Keyword == null ? string.Empty : param.Keyword));
                byte[] file = null;
                
                if (result.personvisitors.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.VisitorColHeader;
                        string wsTitle = ExcelVar.VisitorTitle;
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                        for (int i = 1; i <= ColHeader.Length; i++)
                        {
                            worksheet.Cells[1, i].Value = ColHeader[i - 1].Replace("*", string.Empty);

                            if (ColHeader[i - 1].Contains("*"))
                            {
                                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.IndianRed);
                            }
                        }

                        using (var range = worksheet.Cells[1, 1, 1, ColHeader.Length])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.ShrinkToFit = false;
                        }

                        int rowNumber = 2;
                        foreach (personVisitorVM row in result.personvisitors)
                        {
                            worksheet.Cells[rowNumber, 1].Value = row.idNumber;
                            worksheet.Cells[rowNumber, 2].Value = row.firstName;
                            worksheet.Cells[rowNumber, 3].Value = row.middleName;
                            worksheet.Cells[rowNumber, 4].Value = row.lastName;
                            worksheet.Cells[rowNumber, 5].Value = row.birthdate;
                            worksheet.Cells[rowNumber, 6].Value = row.gender;
                            worksheet.Cells[rowNumber, 7].Value = row.address;
                            worksheet.Cells[rowNumber, 8].Value = row.contactNumber;
                            worksheet.Cells[rowNumber, 9].Value = row.emailAddress;

                            using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                            {
                                range.Style.Font.Bold = false;
                                range.Style.ShrinkToFit = false;
                                range.Style.Numberformat.Format = "@";
                            }

                            rowNumber++;
                        }

                        for (int i = 1; i <= ColHeader.Length; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        package.Workbook.Properties.Title = wsTitle;
                        package.Workbook.Properties.Author = ExcelVar.Author;
                        package.Workbook.Properties.Company = ExcelVar.Company;

                        file = package.GetAsByteArray();
                    }
                }
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[Authorize]
        ////[CustomAuthorize]
        //[HttpGet("downloadtemplateemployee")]
        //public async Task<IActionResult> DownloadTemplateEmployee([FromQuery] PaginationParams param)
        //{
        //    try
        //    {
        //        byte[] file = null;
        //        file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.EmployeeColHeader, ExcelVar.EmployeeSampleData, ExcelVar.EmployeeTitle));
        //        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("downloadtemplatevisitor")]
        public async Task<IActionResult> DownloadTemplateVisitor([FromQuery] PaginationParams param)
        {
            try
            {
                byte[] file = null;
                file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.VisitorColHeader, ExcelVar.VisitorSampleData, ExcelVar.VisitorTemplateTitle));
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("personstudentlist")]
        public async Task<studentPagedResultVM> GetAllStudentList([FromQuery] string keyword, int pageNo, int pageSize)
        {
            return _mapper.Map<studentPagedResultVM>(await _person.GetAllStudents(pageNo, pageSize, keyword == null ? string.Empty : keyword));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("personemployeelist")]
        public async Task<employeePagedResultVM> GetAllEmployeeList([FromQuery] string keyword, int pageNo, int pageSize)
        {
            try
            {
                return _mapper.Map<employeePagedResultVM>(await _person.GetAllEmployees(pageNo, pageSize, keyword == null ? string.Empty : keyword));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("personvisitorlist")]
        public async Task<visitorPagedResultVM> GetAllPersonVisitor([FromQuery] string keyword, int pageNo, int pageSize)
        {
            return _mapper.Map<visitorPagedResultVM>(await _person.GetAllVisitors(pageNo, pageSize, keyword));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("personvisitor/{id:int}")]
        public async Task<personVisitorTableVM> GetByPersonVisitorID(int id)
        {
            var result = _mapper.Map<personVisitorTableVM>(await _person.GetByPersonVisitorID(id));
            return result;
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("employee/{id}")]
        public async Task<employeeTableVM> GetEmployeeByID(string id)
        {
            return _mapper.Map<employeeTableVM>(await _person.GetEmployeeByIDNumber(id));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("student/{id}")]
        public async Task<studentTableVM> GetStudentByID(string id)
        {
            studentTableVM stud = _mapper.Map<studentTableVM>(await _person.GetStudentByIDNumber(id));
            if (stud.courseId == 0) {
                stud.courseId = null;
            }
            if (stud.collegeId == 0) {
                stud.collegeId = null;
            }
            return stud;
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("personfetcherlist")]
        public async Task<fetcherPagedResultVM> GetAllPersonFetcher([FromQuery] PaginationParams param)
        {
            return _mapper.Map<fetcherPagedResultVM>(await _person.GetAllFetchers(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getFetcherById/{id}")]
        public async Task<personFetcherTableVM> GetFetcherById(int id)
        {
            return _mapper.Map<personFetcherTableVM>(await _person.GetFetcherById(id));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("createfetcher")]
        public async Task<ResultModel> CreateFetcher([FromForm] fetcherVM fetcher)
        {
            if (fetcher.filePhoto != null)
            {
                await SavePhotoAsync(fetcher.filePhoto, fetcher.idNumber, "Photo");
            }

            if (fetcher.fileSignature != null)
            {
                await SavePhotoAsync(fetcher.fileSignature, fetcher.idNumber, "Signature");
            }

            return await _person.AddFetcher(_mapper.Map<personEntity>(fetcher), GetUserId()); 
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("updatefetcher")]
        public async Task<ResultModel> UpdateFetcher([FromForm] fetcherVM fetcher)
        {
            if (fetcher.filePhoto != null && fetcher.isChangePhoto == true)
            {
                await SavePhotoAsync(fetcher.filePhoto, fetcher.idNumber, "Photo");
            }
            else if(fetcher.filePhoto == null && fetcher.isChangePhoto == true)
            {
                DeletePhoto(fetcher.idNumber, "Photo");
            }

            if (fetcher.fileSignature != null && fetcher.isChangeSignature == true)
            {
                await SavePhotoAsync(fetcher.fileSignature, fetcher.idNumber, "Signature");
            }
            else if (fetcher.fileSignature == null && fetcher.isChangeSignature == true)
            {
                DeletePhoto(fetcher.idNumber, "Signature");
            }

            return await _person.UpdateFetcher(_mapper.Map<personEntity>(fetcher), GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("temporaryDeleteFetcher/{id}")]
        public async Task<ResultModel> TemporaryDeleteFetcher(int id)
        {
            return await _person.DeleteTemporaryFetcher(id, GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("permanentDeleteFetcher/{id}")]
        public async Task<ResultModel> PermanentDeleteFetcher(int id)
        {
            return await _person.DeletePermanentFetcher(id, GetUserId());
        }

        [HttpGet]
        [Route("downloadtemplatefetcher")]
        public async Task<IActionResult> DownloadTemplateFetcher([FromQuery] PaginationParams param, string type)
        {
            try
            {
                byte[] file = null;
                file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.FetcherColHeader, ExcelVar.FetcherSampleData, ExcelVar.FetcherTemplateTitle));

                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("getfetcherimages/{type}")]
        public async Task<IActionResult> DownloadFetcherImages(string type)
        {
            try
            {
                ICollection<personEntity> fetcherlist = await _person.GetFetcherList();
                int counter = 0;
                string dumpdir = DateTime.Now.ToString("yyyyMMddHHmm");

                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);

                foreach (string i in fetcherlist.Select(q => q.ID_Number))
                {
                    if (System.IO.File.Exists(PhotoStatSettings.photopath + "\\" + i + (type == "Photo" ? "_Photo.jpg" : "_Signature.jpg")))
                    {
                        using (var sourceStream = new FileStream(PhotoStatSettings.photopath + "\\" + i + (type == "Photo" ? "_Photo.jpg" : "_Signature.jpg"), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                        using (var destinationStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + "\\" + i + (type == "Photo" ? "_Photo.jpg" : "_Signature.jpg"), FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                            await sourceStream.CopyToAsync(destinationStream);
                        counter++;
                    }
                }

                if (counter == 0)
                    throw new ArgumentException("No " + type.ToLower() + " available.");

                string[] filenames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);

                using (ZipOutputStream OutputStream
                    = new ZipOutputStream(System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + ".zip")))
                {
                    int CompressionLevel = 9;

                    OutputStream.SetLevel(CompressionLevel);

                    byte[] buffer = new byte[4096];

                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));

                        entry.DateTime = DateTime.Now;
                        OutputStream.PutNextEntry(entry);

                        using (FileStream fs = System.IO.File.OpenRead(file))
                        {
                            int sourceByte;

                            do
                            {
                                sourceByte = fs.Read(buffer, 0, buffer.Length);
                                await OutputStream.WriteAsync(buffer, 0, sourceByte);
                            } while (sourceByte > 0);
                        }
                    }

                    OutputStream.Finish();

                    OutputStream.Close();
                }

                var net = new System.Net.WebClient();
                var directory = AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + ".zip";
                var data = net.DownloadData(directory);
                var content = new System.IO.MemoryStream(data);
                var contentType = "APPLICATION/octet-stream";
                var fileName = type == "Photo" ? "FetcherImagesPhoto" : "FetcherImagesSignature";

                DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);

                foreach (FileInfo f in dirInfo.GetFiles())
                {
                    f.Delete();
                }

                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + ".zip");

                return File(content, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("exportfetcherexcelfile")]
        public async Task<IActionResult> ExportFetcherExcelFile([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<fetcherPagedResultVM>(await _person.ExportAllFetchers(param.Keyword == null ? string.Empty : param.Keyword));
                byte[] file = null;

                if (result.personfetchers.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.FetcherColHeader;
                        string wsTitle = ExcelVar.FetcherTitle;
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                        for (int i = 1; i <= ColHeader.Length; i++)
                        {
                            worksheet.Cells[1, i].Value = ColHeader[i - 1].Replace("*", string.Empty);

                            if (ColHeader[i - 1].Contains("*"))
                            {
                                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.IndianRed);
                            }
                        }

                        using (var range = worksheet.Cells[1, 1, 1, ColHeader.Length])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.ShrinkToFit = false;
                        }

                        int rowNumber = 2;
                        foreach (personFetcherVM row in result.personfetchers)
                        {
                            worksheet.Cells[rowNumber, 1].Value = row.idNumber;
                            worksheet.Cells[rowNumber, 2].Value = row.firstName;
                            worksheet.Cells[rowNumber, 3].Value = row.middleName;
                            worksheet.Cells[rowNumber, 4].Value = row.lastName;
                            worksheet.Cells[rowNumber, 5].Value = row.birthdate;
                            worksheet.Cells[rowNumber, 6].Value = row.gender;
                            worksheet.Cells[rowNumber, 7].Value = row.contactNumber;
                            worksheet.Cells[rowNumber, 8].Value = row.emailAddress;
                            worksheet.Cells[rowNumber, 9].Value = row.address;
                            worksheet.Cells[rowNumber, 10].Value = row.fetcherRelationship;
                            worksheet.Cells[rowNumber, 11].Value = row.campusName;

                            using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                            {
                                range.Style.Font.Bold = false;
                                range.Style.ShrinkToFit = false;
                                range.Style.Numberformat.Format = "@";
                            }

                            rowNumber++;
                        }

                        for (int i = 1; i <= ColHeader.Length; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        package.Workbook.Properties.Title = wsTitle;
                        package.Workbook.Properties.Author = ExcelVar.Author;
                        package.Workbook.Properties.Company = ExcelVar.Company;

                        file = package.GetAsByteArray();
                    }
                }
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("batchupload/personfetchers/{id:int}")]
        public async Task<IActionResult> BatchUploadFetcherProcess(int id)
        {
            string message = "Batch file has been successfully process.";
            var entity = await _batchUploadService.GetByIdSync(id);
            var response = new BatchUploadResponse();
            try
            {
                await ValidateBatchFetcherProcess(entity);
                int lastProcess = entity.ProcessCount;

                IEnumerable<personFetcherBatchUploadVM> records = _batchUploadService.GetFetcherRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                // Do the update process here.
                response = await _person.PersonFetcherBatchUpload(process, GetUserId(), entity.ID, lastProcess);

                entity.ProcessCount = entity.ProcessCount + process.Count();
                entity.Status = entity.ProcessCount == entity.TotalCount ? Constants.BATCH_UPLOAD_STATUS_SUCCESS : entity.Status;
                if (entity.Status != Constants.BATCH_UPLOAD_STATUS_SUCCESS)
                    message = string.Format("Processing {0} to {1}", lastProcess, entity.ProcessCount);

                await _batchUploadService.UpdateBatchUplaod(entity, GetUserId());

                response.ProcessCount = entity.ProcessCount;
                response.TotalCount = entity.TotalCount;
                response.Status = entity.Status;
                return Ok(new { message = message, response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public async Task ValidateBatchFetcherProcess(batchUploadEntity upload)
        {
            // Validate if valid file.
            if (upload.Status == Constants.BATCH_UPLOAD_STATUS_CANCELLED)
                throw new Exception("Upload has been cancelled.");
        }

        [HttpPost]
        [Route("batchupload/personfetchers")]
        public async Task<IActionResult> BatchFetcherUpload(IFormFile file)
        {
            string message = "Batch file has been successfully uploaded.";
            var response = new BatchUploadResponse();
            var entity = new batchUploadEntity();
            try
            {
                entity.Form_ID = (int)Form.People;
                entity.Date_Time_Added = DateTime.Now;
                await ValidateBatchFetcherFile(file, entity);
                await _batchUploadService.Upload(file, entity, GetUserId());
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message, response, key = entity.ID });
        }

        public async Task ValidateBatchFetcherFile(IFormFile file, batchUploadEntity upload)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
            var fileType = Path.GetExtension(originalFileName);

            // Validate if valid file.
            if (fileType != ".xlsx\"")
                throw new Exception("Invalid file! Please download the valid template.");

            try
            {
                upload.TotalCount = _batchUploadService.GetRecordsCount(file);
            }
            catch (Exception ex)
            {
                throw new Exception("File contains invalid data or missing fields.");
            }

            if (upload.TotalCount == 0)
                throw new Exception("File is empty!");
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("personotheraccesslist")]
        public async Task<otherAccessPagedResultVM> GetAllPersonOtherAccess([FromQuery] PaginationParams param)
        {
            return await _person.GetAllOtherAccesses(param.PageNo, param.PageSize, param.Keyword);
        }

        //[CustomAuthorize]
        [HttpGet]
        [Route("getOtherAccessById/{id}")]
        public async Task<personOtherAccessTableVM> GetOtherAccessById(int id)
        {
            return _mapper.Map<personOtherAccessTableVM>(await _person.GetOtherAccessById(id));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("createotheraccess")]
        public async Task<ResultModel> CreateOtherAccess([FromForm] otherAccessVM otheraccess)
        {
            if (otheraccess.filePhoto != null)
            {
                await SavePhotoAsync(otheraccess.filePhoto, otheraccess.idNumber, "Photo");
            }

            if (otheraccess.fileSignature != null)
            {
                await SavePhotoAsync(otheraccess.fileSignature, otheraccess.idNumber, "Signature");
            }

            personEntity person = _mapper.Map<personEntity>(otheraccess);
            emergencyContactEntity emergency = _mapper.Map<emergencyContactEntity>(otheraccess);

            return await _person.AddOtherAccess(person, emergency, GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("updateotheraccess")]
        public async Task<ResultModel> UpdateOtherAccess([FromForm] otherAccessVM otheraccess)
        {
            if (otheraccess.filePhoto != null && otheraccess.isChangePhoto == true)
            {
                await SavePhotoAsync(otheraccess.filePhoto, otheraccess.idNumber, "Photo");
            }
            else if (otheraccess.filePhoto == null && otheraccess.isChangePhoto == true)
            {
                DeletePhoto(otheraccess.idNumber, "Photo");
            }

            if (otheraccess.fileSignature != null && otheraccess.isChangeSignature == true)
            {
                await SavePhotoAsync(otheraccess.fileSignature, otheraccess.idNumber, "Signature");
            }
            else if (otheraccess.fileSignature == null && otheraccess.isChangeSignature == true)
            {
                DeletePhoto(otheraccess.idNumber, "Signature");
            }

            personEntity person = _mapper.Map<personEntity>(otheraccess);
            emergencyContactEntity emergency = _mapper.Map<emergencyContactEntity>(otheraccess);

            return await _person.UpdateOtherAccess(person, emergency, GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("temporaryDeleteOtherAccess/{id}")]
        public async Task<ResultModel> TemporaryDeleteOtherAccess(int id)
        {
            return await _person.DeleteTemporaryOtherAccess(id, GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("permanentDeleteOtherAccess/{id}")]
        public async Task<ResultModel> PermanentDeleteOtherAccess(int id)
        {
            return await _person.DeletePermanentOtherAccess(id, GetUserId());
        }

        [HttpGet]
        [Route("downloadtemplateotheraccess")]
        public async Task<IActionResult> DownloadTemplateOtherAccess([FromQuery] PaginationParams param, string type)
        {
            try
            {
                byte[] file = null;
                file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.OtherAccessColHeader, ExcelVar.OtherAccessSampleData, ExcelVar.OtherAccessTemplateTitle));

                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("getotheraccessimages/{type}")]
        public async Task<IActionResult> DownloadOtherAccessImages(string type)
        {
            try
            {
                ICollection<personEntity> otheraccesslist = await _person.GetOtherAccessList();
                int counter = 0;
                string dumpdir = DateTime.Now.ToString("yyyyMMddHHmm");

                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);

                foreach (string i in otheraccesslist.Select(q => q.ID_Number))
                {
                    if (System.IO.File.Exists(PhotoStatSettings.photopath + "\\" + i + (type == "Photo" ? "_Photo.jpg" : "_Signature.jpg")))
                    {
                        using (var sourceStream = new FileStream(PhotoStatSettings.photopath + "\\" + i + (type == "Photo" ? "_Photo.jpg" : "_Signature.jpg"), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                        using (var destinationStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + "\\" + i + (type == "Photo" ? "_Photo.jpg" : "_Signature.jpg"), FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                            await sourceStream.CopyToAsync(destinationStream);
                        counter++;
                    }
                }

                if (counter == 0)
                    throw new ArgumentException("No " + type.ToLower() + " available.");

                string[] filenames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);

                using (ZipOutputStream OutputStream
                    = new ZipOutputStream(System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + ".zip")))
                {
                    int CompressionLevel = 9;

                    OutputStream.SetLevel(CompressionLevel);

                    byte[] buffer = new byte[4096];

                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));

                        entry.DateTime = DateTime.Now;
                        OutputStream.PutNextEntry(entry);

                        using (FileStream fs = System.IO.File.OpenRead(file))
                        {
                            int sourceByte;

                            do
                            {
                                sourceByte = fs.Read(buffer, 0, buffer.Length);
                                await OutputStream.WriteAsync(buffer, 0, sourceByte);
                            } while (sourceByte > 0);
                        }
                    }

                    OutputStream.Finish();

                    OutputStream.Close();
                }

                var net = new System.Net.WebClient();
                var directory = AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + ".zip";
                var data = net.DownloadData(directory);
                var content = new System.IO.MemoryStream(data);
                var contentType = "APPLICATION/octet-stream";
                var fileName = type == "Photo" ? "OtherAccessImagesPhoto" : "OtherAccessImagesSignature";

                DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);

                foreach (FileInfo f in dirInfo.GetFiles())
                {
                    f.Delete();
                }

                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + ".zip");

                return File(content, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("exportotheraccessexcelfile")]
        public async Task<IActionResult> ExportOtherAccessExcelFile([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<otherAccessPagedResultVM>(await _person.ExportAllOtherAccess(param.Keyword == null ? string.Empty : param.Keyword));
                byte[] file = null;

                if (result.personotheraccess.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.OtherAccessColHeader;
                        string wsTitle = ExcelVar.OtherAccessTitle;
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                        for (int i = 1; i <= ColHeader.Length; i++)
                        {
                            worksheet.Cells[1, i].Value = ColHeader[i - 1].Replace("*", string.Empty);

                            if (ColHeader[i - 1].Contains("*"))
                            {
                                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.IndianRed);
                            }
                        }

                        using (var range = worksheet.Cells[1, 1, 1, ColHeader.Length])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.ShrinkToFit = false;
                        }

                        int rowNumber = 2;
                        foreach (personOtherAccessVM row in result.personotheraccess)
                        {
                            worksheet.Cells[rowNumber, 1].Value = row.idNumber;
                            worksheet.Cells[rowNumber, 2].Value = row.firstName;
                            worksheet.Cells[rowNumber, 3].Value = row.middleName;
                            worksheet.Cells[rowNumber, 4].Value = row.lastName;
                            worksheet.Cells[rowNumber, 5].Value = row.birthdate;
                            worksheet.Cells[rowNumber, 6].Value = row.gender;
                            worksheet.Cells[rowNumber, 7].Value = row.contactNumber;
                            worksheet.Cells[rowNumber, 8].Value = row.telephoneNumber;
                            worksheet.Cells[rowNumber, 9].Value = row.emailAddress;
                            worksheet.Cells[rowNumber, 10].Value = row.address;
                            worksheet.Cells[rowNumber, 11].Value = row.campusName;
                            worksheet.Cells[rowNumber, 12].Value = row.departmentName;
                            worksheet.Cells[rowNumber, 13].Value = row.positionName;
                            worksheet.Cells[rowNumber, 14].Value = row.officeName;
                            worksheet.Cells[rowNumber, 15].Value = row.emergencyFullname;
                            worksheet.Cells[rowNumber, 16].Value = row.emergencyAddress;
                            worksheet.Cells[rowNumber, 17].Value = row.emergencyContact;
                            worksheet.Cells[rowNumber, 18].Value = row.emergencyRelationship;

                            using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                            {
                                range.Style.Font.Bold = false;
                                range.Style.ShrinkToFit = false;
                                range.Style.Numberformat.Format = "@";
                            }

                            rowNumber++;
                        }

                        for (int i = 1; i <= ColHeader.Length; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        package.Workbook.Properties.Title = wsTitle;
                        package.Workbook.Properties.Author = ExcelVar.Author;
                        package.Workbook.Properties.Company = ExcelVar.Company;

                        file = package.GetAsByteArray();
                    }
                }
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("batchupload/personotheraccess/{id:int}")]
        public async Task<IActionResult> BatchUploadOtherAccessProcess(int id)
        {
            string message = "Batch file has been successfully process.";
            var entity = await _batchUploadService.GetByIdSync(id);
            var response = new BatchUploadResponse();
            try
            {
                await ValidateBatchOtherAccessProcess(entity);
                int lastProcess = entity.ProcessCount;

                IEnumerable<personOtherAccessBatchUploadVM> records = _batchUploadService.GetOtherAccessRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                // Do the update process here.
                response = await _person.PersonOtherAccessBatchUpload(process, GetUserId(), entity.ID, lastProcess);

                entity.ProcessCount = entity.ProcessCount + process.Count();
                entity.Status = entity.ProcessCount == entity.TotalCount ? Constants.BATCH_UPLOAD_STATUS_SUCCESS : entity.Status;
                if (entity.Status != Constants.BATCH_UPLOAD_STATUS_SUCCESS)
                    message = string.Format("Processing {0} to {1}", lastProcess, entity.ProcessCount);

                await _batchUploadService.UpdateBatchUplaod(entity, GetUserId());

                response.ProcessCount = entity.ProcessCount;
                response.TotalCount = entity.TotalCount;
                response.Status = entity.Status;
                return Ok(new { message = message, response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public async Task ValidateBatchOtherAccessProcess(batchUploadEntity upload)
        {
            // Validate if valid file.
            if (upload.Status == Constants.BATCH_UPLOAD_STATUS_CANCELLED)
                throw new Exception("Upload has been cancelled.");
        }

        [HttpPost]
        [Route("batchupload/personotheraccess")]
        public async Task<IActionResult> BatchOtherAccessUpload(IFormFile file)
        {
            string message = "Batch file has been successfully uploaded.";
            var response = new BatchUploadResponse();
            var entity = new batchUploadEntity();
            try
            {
                entity.Form_ID = (int)Form.People;
                entity.Date_Time_Added = DateTime.Now;
                await ValidateBatchOtherAccessFile(file, entity);
                await _batchUploadService.Upload(file, entity, GetUserId());
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message, response, key = entity.ID });
        }

        public async Task ValidateBatchOtherAccessFile(IFormFile file, batchUploadEntity upload)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
            var fileType = Path.GetExtension(originalFileName);

            // Validate if valid file.
            if (fileType != ".xlsx\"")
                throw new Exception("Invalid file! Please download the valid template.");

            try
            {
                upload.TotalCount = _batchUploadService.GetRecordsCount(file);
            }
            catch (Exception ex)
            {
                throw new Exception("File contains invalid data or missing fields.");
            }

            if (upload.TotalCount == 0)
                throw new Exception("File is empty!");
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getVisitorById/{id}")]
        public async Task<personVisitorTableVM> GetVisitorById(int id)
        {
            return _mapper.Map<personVisitorTableVM>(await _person.GetVisitorById(id));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("createvisitor")]
        public async Task<ResultModel> CreateVisitor([FromForm] personVisitorVM visitor)
        {
            return await _person.AddVisitor(_mapper.Map<personEntity>(visitor), GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("updatevisitor")]
        public async Task<ResultModel> UpdateVisitor([FromForm] personVisitorVM visitor)
        {
            return await _person.UpdateVisitor(_mapper.Map<personEntity>(visitor), GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("temporaryDeleteVisitor/{id}")]
        public async Task<ResultModel> TemporaryDeleteVisitor(int id)
        {
            return await _person.DeleteTemporaryVisitor(id, GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("permanentDeleteVisitor/{id}")]
        public async Task<ResultModel> PermanentDeleteVisitor(int id)
        {
            return await _person.DeletePermanentVisitor(id, GetUserId());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet] 
        [Route("getEmployeeById/{id}")]
        public async Task<employeeVM> GetEmployeeById(int id)
        {
            return _mapper.Map<employeeVM>(await _person.GetEmployeeById(id));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("createemployee")]
        public async Task<ResultModel> CreateEmployee([FromForm] employeeVM employee)
        {
            if (employee.filePhoto != null)
            {
                await SavePhotoAsync(employee.filePhoto, employee.idNumber, "Photo");
            }

            if (employee.fileSignature != null)
            {
                await SavePhotoAsync(employee.fileSignature, employee.idNumber, "Signature");
            }

            var emergencyContact = new emergencyContactEntity
            {
                Address = employee.emergencyAddress,
                Contact_Number = employee.emergencyContact,
                Relationship = employee.emergencyRelationship,
                Full_Name = employee.emergencyFullname
            };

            var govIds = new govIdsEntity
            {
                SSS = employee.sss,
                TIN = employee.tin,
                PhilHealth = employee.philhealth,
                PAG_IBIG = employee.pagibig
            };

            return await _person.AddEmployee(_mapper.Map<personEntity>(employee), emergencyContact, govIds, GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("updateemployee")]
        public async Task<ResultModel> UpdateEmployee([FromForm] employeeVM employee)
        {
            if (employee.filePhoto != null && employee.isChangePhoto == true)
            {
                await SavePhotoAsync(employee.filePhoto, employee.idNumber, "Photo");
            }
            else if (employee.filePhoto == null && employee.isChangePhoto == true)
            {
                DeletePhoto(employee.idNumber, "Photo");
            }


            if (employee.fileSignature != null && employee.isChangeSignature == true)
            {
                await SavePhotoAsync(employee.fileSignature, employee.idNumber, "Signature");
            }
            else if (employee.fileSignature == null && employee.isChangeSignature == true)
            {
                DeletePhoto(employee.idNumber, "Signature");
            }


            var emergencyContact = new emergencyContactEntity
            {
                Address = employee.emergencyAddress,
                Contact_Number = employee.emergencyContact,
                Relationship = employee.emergencyRelationship,
                Full_Name = employee.emergencyFullname
            };

            var govIds = new govIdsEntity
            {
                SSS = employee.sss,
                TIN = employee.tin,
                PhilHealth = employee.philhealth,
                PAG_IBIG = employee.pagibig
            };

            return await _person.UpdateEmployee(_mapper.Map<personEntity>(employee), emergencyContact, govIds, GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("temporaryDeleteEmployee/{id}")]
        public async Task<ResultModel> TemporaryDeleteEmployee(int id)
        {
            return await _person.DeleteTemporaryEmployee(id, GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("permanentDeleteEmployee/{id}")]
        public async Task<ResultModel> PermanentDeleteEmployee(int id)
        {
            return await _person.DeletePermanentEmployee(id, GetUserId());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("downloadtemplateemployee")]
        public async Task<IActionResult> DownloadTemplateEmployee([FromQuery] PaginationParams param, string type)
        {
            try
            {
                byte[] file = null;
                file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.EmployeeColHeader, ExcelVar.EmployeeSampleData, ExcelVar.EmployeeTemplateTitle));

                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getemployeeimages/{type}")]
        public async Task<IActionResult> DownloadEmployeeImages(string type)
        {
            try
            {
                ICollection<personEntity> employeelist = await _person.GetEmployeeList();
                int counter = 0;
                string dumpdir = DateTime.Now.ToString("yyyyMMddHHmm");

                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);

                foreach (string i in employeelist.Select(q => q.ID_Number))
                {
                    if (System.IO.File.Exists(PhotoStatSettings.photopath + "\\" + i + (type == "Photo" ? "_Photo.jpg" : "_Signature.jpg")))
                    {
                        using (var sourceStream = new FileStream(PhotoStatSettings.photopath + "\\" + i + (type == "Photo" ? "_Photo.jpg" : "_Signature.jpg"), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                        using (var destinationStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + "\\" + i + (type == "Photo" ? "_Photo.jpg" : "_Signature.jpg"), FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                            await sourceStream.CopyToAsync(destinationStream);
                        counter++;
                    }
                }

                if (counter == 0)
                    throw new ArgumentException("No " + type.ToLower() + " available.");

                string[] filenames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);

                using (ZipOutputStream OutputStream
                    = new ZipOutputStream(System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + ".zip")))
                {
                    int CompressionLevel = 9;

                    OutputStream.SetLevel(CompressionLevel);

                    byte[] buffer = new byte[4096];

                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));

                        entry.DateTime = DateTime.Now;
                        OutputStream.PutNextEntry(entry);

                        using (FileStream fs = System.IO.File.OpenRead(file))
                        {
                            int sourceByte;

                            do
                            {
                                sourceByte = fs.Read(buffer, 0, buffer.Length);
                                await OutputStream.WriteAsync(buffer, 0, sourceByte);
                            } while (sourceByte > 0);
                        }
                    }

                    OutputStream.Finish();

                    OutputStream.Close();
                }

                var net = new System.Net.WebClient();
                var directory = AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + ".zip";
                var data = net.DownloadData(directory);
                var content = new System.IO.MemoryStream(data);
                var contentType = "APPLICATION/octet-stream";
                var fileName = type == "Photo" ? "EmployeeImagesPhoto" : "EmployeeImagesSignature";

                DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);

                foreach (FileInfo f in dirInfo.GetFiles())
                {
                    f.Delete();
                }

                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + ".zip");

                return File(content, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("exportemployeeexcelfile")]
        public async Task<IActionResult> ExportEmployeeExcelFile([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<employeePagedResultVM>(await _person.ExportAllEmployees(param.Keyword == null ? string.Empty : param.Keyword));
                byte[] file = null;

                if (result.employees.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.EmployeeColHeader;
                        string wsTitle = ExcelVar.EmployeeTitle;
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                        for (int i = 1; i <= ColHeader.Length; i++)
                        {
                            worksheet.Cells[1, i].Value = ColHeader[i - 1].Replace("*", string.Empty);

                            if (ColHeader[i - 1].Contains("*"))
                            {
                                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.IndianRed);
                            }
                        }

                        using (var range = worksheet.Cells[1, 1, 1, ColHeader.Length])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.ShrinkToFit = false;
                        }

                        int rowNumber = 2;
                        foreach (employeeVM row in result.employees)
                        {
                            worksheet.Cells[rowNumber, 1].Value = row.idNumber;
                            worksheet.Cells[rowNumber, 2].Value = row.firstName;
                            worksheet.Cells[rowNumber, 3].Value = row.middleName;
                            worksheet.Cells[rowNumber, 4].Value = row.lastName;
                            worksheet.Cells[rowNumber, 5].Value = row.birthdate;
                            worksheet.Cells[rowNumber, 6].Value = row.gender;
                            worksheet.Cells[rowNumber, 7].Value = row.contactNumber;
                            worksheet.Cells[rowNumber, 8].Value = row.telephoneNumber;
                            worksheet.Cells[rowNumber, 9].Value = row.emailAddress;
                            worksheet.Cells[rowNumber, 10].Value = row.address;
                            worksheet.Cells[rowNumber, 11].Value = row.campusName;
                            worksheet.Cells[rowNumber, 12].Value = row.employeeTypeDesc;
                            worksheet.Cells[rowNumber, 13].Value = row.employeeSubTypeDesc;
                            worksheet.Cells[rowNumber, 14].Value = row.departmentName;
                            worksheet.Cells[rowNumber, 15].Value = row.positionName;
                            worksheet.Cells[rowNumber, 16].Value = row.emergencyFullname;
                            worksheet.Cells[rowNumber, 17].Value = row.emergencyAddress;
                            worksheet.Cells[rowNumber, 18].Value = row.emergencyContact;
                            worksheet.Cells[rowNumber, 19].Value = row.emergencyRelationship;
                            worksheet.Cells[rowNumber, 20].Value = row.sss;
                            worksheet.Cells[rowNumber, 21].Value = row.pagibig;
                            worksheet.Cells[rowNumber, 22].Value = row.tin;
                            worksheet.Cells[rowNumber, 23].Value = row.philhealth;

                            using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                            {
                                range.Style.Font.Bold = false;
                                range.Style.ShrinkToFit = false;
                                range.Style.Numberformat.Format = "@";
                            }

                            rowNumber++;
                        }

                        for (int i = 1; i <= ColHeader.Length; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        package.Workbook.Properties.Title = wsTitle;
                        package.Workbook.Properties.Author = ExcelVar.Author;
                        package.Workbook.Properties.Company = ExcelVar.Company;

                        file = package.GetAsByteArray();
                    }
                }
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("batchupload/personemployees/{id:int}")]
        public async Task<IActionResult> BatchUploadEmployeeProcess(int id)
        {
            string message = "Batch file has been successfully process.";
            var entity = await _batchUploadService.GetByIdSync(id);
            var response = new BatchUploadResponse();
            try
            {
                await ValidateBatchEmployeeProcess(entity);
                int lastProcess = entity.ProcessCount;
             
                IEnumerable<personEmployeeBatchUploadVM> records = _batchUploadService.GetEmployeeRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                // Do the update process here.
                response = await _person.PersonEmployeeBatchUpload(process, GetUserId(), entity.ID, lastProcess);

                entity.ProcessCount = entity.ProcessCount + process.Count();
                entity.Status = entity.ProcessCount == entity.TotalCount ? Constants.BATCH_UPLOAD_STATUS_SUCCESS : entity.Status;
                if (entity.Status != Constants.BATCH_UPLOAD_STATUS_SUCCESS)
                    message = string.Format("Processing {0} to {1}", lastProcess, entity.ProcessCount);

                await _batchUploadService.UpdateBatchUplaod(entity, GetUserId());

                response.ProcessCount = entity.ProcessCount;
                response.TotalCount = entity.TotalCount;
                response.Status = entity.Status;
                return Ok(new { message = message, response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public async Task ValidateBatchEmployeeProcess(batchUploadEntity upload)
        {
            // Validate if valid file.
            if (upload.Status == Constants.BATCH_UPLOAD_STATUS_CANCELLED)
                throw new Exception("Upload has been cancelled.");
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("batchupload/personemployees")]
        public async Task<IActionResult> BatchEmployeeUpload(IFormFile file)
        {
            string message = "Batch file has been successfully uploaded.";
            var response = new BatchUploadResponse();
            var entity = new batchUploadEntity();
            try
            {
                entity.Form_ID = (int)Form.People;
                entity.Date_Time_Added = DateTime.Now;
                await ValidateBatchEmployeeFile(file, entity);
                await _batchUploadService.Upload(file, entity, GetUserId());
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message, response, key = entity.ID });
        }

        public async Task ValidateBatchEmployeeFile(IFormFile file, batchUploadEntity upload)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
            var fileType = Path.GetExtension(originalFileName);

            // Validate if valid file.
            if (fileType != ".xlsx\"")
                throw new Exception("Invalid file! Please download the valid template.");

            try
            {
                upload.TotalCount = _batchUploadService.GetRecordsCount(file);
            }
            catch (Exception ex)
            {
                throw new Exception("File contains invalid data or missing fields.");
            }

            if (upload.TotalCount == 0)
                throw new Exception("File is empty!");
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getStudentById/{id}")]
        public async Task<studentVM> GetStudentById(int id)
        {
            return _mapper.Map<studentVM>(await _person.GetStudentById(id));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("createstudent")]
        public async Task<ResultModel> CreateStudent([FromForm] studentVM student)
        {
            if (student.filePhoto != null)
            {
                await SavePhotoAsync(student.filePhoto, student.idNumber, "Photo");
            }

            if (student.fileSignature != null)
            {
                await SavePhotoAsync(student.fileSignature, student.idNumber, "Signature");
            }

            var emergencyContact = new emergencyContactEntity
            {
                Address = student.emergencyAddress,
                Contact_Number = student.emergencyContact,
                Relationship = student.emergencyRelationship,
                Full_Name = student.emergencyFullname
            };

            return await _person.AddStudent(_mapper.Map<personEntity>(student), emergencyContact, GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("updatestudent")]
        public async Task<ResultModel> UpdateStudent([FromForm] studentVM student)
        {
            if (student.filePhoto != null && student.isChangePhoto == true)
            {
                await SavePhotoAsync(student.filePhoto, student.idNumber, "Photo");
            }
            else if(student.filePhoto == null && student.isChangePhoto == true)
            {
                DeletePhoto(student.idNumber, "Photo");
            }

            if (student.fileSignature != null && student.isChangeSignature == true)
            {
                await SavePhotoAsync(student.fileSignature, student.idNumber, "Signature");
            }
            else if(student.fileSignature == null && student.isChangeSignature == true)
            {
                DeletePhoto(student.idNumber, "Signature");
            }

            var emergencyContact = new emergencyContactEntity
            {
                Address = student.emergencyAddress,
                Contact_Number = student.emergencyContact,
                Relationship = student.emergencyRelationship,
                Full_Name = student.emergencyFullname
            };

            return await _person.UpdateStudent(_mapper.Map<personEntity>(student), emergencyContact, GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("temporaryDeleteStudent/{id}")]
        public async Task<ResultModel> TemporaryDeleteStudent(int id)
        {
            return await _person.DeleteTemporaryStudent(id, GetUserId());
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("permanentDeleteStudent/{id}")]
        public async Task<ResultModel> PermanentDeleteStudent(int id)
        {
            return await _person.DeletePermanentStudent(id, GetUserId());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("excuse/{id}")]
        public async Task<excusePagedResultVM> GetExcuse(string id, [FromQuery] PaginationParams param)
        {
            try
            {
                return _mapper.Map<excusePagedResultVM>(await _person.GetAllExcuses(id, param.PageNo, param.PageSize, param.Keyword));
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [CustomAuthorize]
        [HttpPost]
        [Route("createexcuse")]
        public async Task<ResultModel> CreateExcuse([FromForm] excuseVM excuse)
        {
            try
            {
                return await _person.AddExcuse(_mapper.Map<excusedStudentEntity>(excuse), GetUserId());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("deleteExcuseStudent/{id}")]
        public async Task<ResultModel> DeleteExcuseStudent(int id)
        {
            return await _person.DeleteExcuseStudent(id, GetUserId());
        }

        [Authorize] 
        //[CustomAuthorize]
        [HttpGet]
        [Route("alldropoutcode")]
        public async Task<ICollection<dropoutCodeVM>> GetDropOutCode()
        {
            return _mapper.Map<ICollection<dropoutCodeVM>>(await _person.GetDropOutCode());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("allfetcherstudents")]
        public async Task<ICollection<fetcherStudentsVM>> GetFetcherStudents()
        {
            return _mapper.Map<ICollection<fetcherStudentsVM>>(await _person.GetFetcherStudents());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("allemergencylogoutstudents")]
        public async Task<ICollection<emergencyLogoutStudentsVM>> GetEmergencyLogoutStudents()
        {
            return _mapper.Map<ICollection<emergencyLogoutStudentsVM>>(await _person.GetEmergencyLogoutStudents());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("reportpersonname/{type}")]
        public async Task<ICollection<reportPersonListVM>> GetReportPersonList(string type)
        {
            return _mapper.Map<ICollection<reportPersonListVM>>(await _person.GetReportPersonList(type));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("updatestudentstatus")]
        public async Task<ResultModel> UpdateStudentStatus([FromForm] studentVM student)
        {
            try
            {
                return await _person.UpdateStudentStatus(_mapper.Map<personEntity>(student), GetUserId());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("downloadtemplatestudent")]
        public async Task<IActionResult> DownloadTemplateStudent([FromQuery] PaginationParams param, string type)
        {
            try
            {
                byte[] file = null;
                if (type == "college")
                    file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.StudentCollegeColHeader, ExcelVar.StudentCollegeSampleData, ExcelVar.StudentTemplateTitle));
                else
                    file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.StudentColHeader, ExcelVar.StudentSampleData, ExcelVar.StudentTemplateTitle));
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("getstudentimages/{type}")]
        public async Task<IActionResult> DownloadStudentImages(string type)
        {
            try
            {
                ICollection<personEntity> studentlist = await _person.GetStudentList();
                int counter = 0;
                string dumpdir = DateTime.Now.ToString("yyyyMMddHHmm");

                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);

                foreach (string i in studentlist.Select(q => q.ID_Number))
                {
                    if (System.IO.File.Exists(PhotoStatSettings.photopath + "\\" + i + (type == "Photo" ? "_Photo.jpg" : "_Signature.jpg")))
                    {
                        using (var sourceStream = new FileStream(PhotoStatSettings.photopath + "\\" + i + (type == "Photo" ? "_Photo.jpg" : "_Signature.jpg"), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                        using (var destinationStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + "\\" + i + (type == "Photo" ? "_Photo.jpg" : "_Signature.jpg"), FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                            await sourceStream.CopyToAsync(destinationStream);
                        counter++;
                    }
                }

                if (counter == 0)
                    throw new ArgumentException("No " + type.ToLower() + " available.");

                string[] filenames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);

                using (ZipOutputStream OutputStream
                    = new ZipOutputStream(System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + ".zip")))
                {
                    int CompressionLevel = 9;

                    OutputStream.SetLevel(CompressionLevel);

                    byte[] buffer = new byte[4096];

                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));

                        entry.DateTime = DateTime.Now;
                        OutputStream.PutNextEntry(entry);

                        using (FileStream fs = System.IO.File.OpenRead(file))
                        {
                            int sourceByte;

                            do
                            {
                                sourceByte = fs.Read(buffer, 0, buffer.Length);
                                await OutputStream.WriteAsync(buffer, 0, sourceByte);
                            } while (sourceByte > 0);
                        }
                    }

                    OutputStream.Finish();

                    OutputStream.Close();
                }

                var net = new System.Net.WebClient();
                var directory = AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + ".zip";
                var data = net.DownloadData(directory);
                var content = new System.IO.MemoryStream(data);
                var contentType = "APPLICATION/octet-stream";
                var fileName = type == "Photo" ? "StudentImagesPhoto" : "StudentImagesSignature";

                DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);

                foreach (FileInfo f in dirInfo.GetFiles())
                {
                    f.Delete();
                }

                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir);
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"Dump\" + dumpdir + ".zip");

                return File(content, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("exportstudentexcelfile")]
        public async Task<IActionResult> ExportStudentExcelFile([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<studentPagedResultVM>(await _person.ExportAllStudents(param.Keyword == null ? string.Empty : param.Keyword, false));
                byte[] file = null;

                if (result.students.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.StudentColHeader;
                        string wsTitle = ExcelVar.StudentTitle;
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                        for (int i = 1; i <= ColHeader.Length; i++)
                        {
                            worksheet.Cells[1, i].Value = ColHeader[i - 1].Replace("*", string.Empty);

                            if (ColHeader[i - 1].Contains("*"))
                            {
                                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.IndianRed);
                            }
                        }

                        using (var range = worksheet.Cells[1, 1, 1, ColHeader.Length])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.ShrinkToFit = false;
                        }

                        int rowNumber = 2;
                        foreach (studentVM row in result.students)
                        {
                            worksheet.Cells[rowNumber, 1].Value = row.idNumber;
                            worksheet.Cells[rowNumber, 2].Value = row.firstName;
                            worksheet.Cells[rowNumber, 3].Value = row.middleName;
                            worksheet.Cells[rowNumber, 4].Value = row.lastName;
                            worksheet.Cells[rowNumber, 5].Value = row.birthdate;
                            worksheet.Cells[rowNumber, 6].Value = row.gender;
                            worksheet.Cells[rowNumber, 7].Value = row.contactNumber;
                            worksheet.Cells[rowNumber, 8].Value = row.telephoneNumber;
                            worksheet.Cells[rowNumber, 9].Value = row.emailAddress;
                            worksheet.Cells[rowNumber, 10].Value = row.address;
                            worksheet.Cells[rowNumber, 11].Value = row.campusName;
                            worksheet.Cells[rowNumber, 12].Value = row.educLevelName;
                            worksheet.Cells[rowNumber, 13].Value = row.yearSecName;
                            worksheet.Cells[rowNumber, 14].Value = row.description;
                            worksheet.Cells[rowNumber, 15].Value = row.dateEnrolled;
                            worksheet.Cells[rowNumber, 16].Value = row.emergencyFullname;
                            worksheet.Cells[rowNumber, 17].Value = row.emergencyAddress;
                            worksheet.Cells[rowNumber, 18].Value = row.emergencyContact;
                            worksheet.Cells[rowNumber, 19].Value = row.emergencyRelationship;

                            using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                            {
                                range.Style.Font.Bold = false;
                                range.Style.ShrinkToFit = false;
                                range.Style.Numberformat.Format = "@";
                            }

                            rowNumber++;
                        }

                        for (int i = 1; i <= ColHeader.Length; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        package.Workbook.Properties.Title = wsTitle;
                        package.Workbook.Properties.Author = ExcelVar.Author;
                        package.Workbook.Properties.Company = ExcelVar.Company;

                        file = package.GetAsByteArray();
                    }
                }
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("exportstudentcollegeexcelfile")]
        public async Task<IActionResult> ExportStudentCollegeExcelFile([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<studentPagedResultVM>(await _person.ExportAllStudents(param.Keyword == null ? string.Empty : param.Keyword, true));
                byte[] file = null;

                if (result.students.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.StudentCollegeColHeader;
                        string wsTitle = ExcelVar.StudentTitle;
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(wsTitle);

                        for (int i = 1; i <= ColHeader.Length; i++)
                        {
                            worksheet.Cells[1, i].Value = ColHeader[i - 1].Replace("*", string.Empty);

                            if (ColHeader[i - 1].Contains("*"))
                            {
                                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.IndianRed);
                            }
                        }

                        using (var range = worksheet.Cells[1, 1, 1, ColHeader.Length])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.ShrinkToFit = false;
                        }

                        int rowNumber = 2;
                        foreach (studentVM row in result.students)
                        {
                            worksheet.Cells[rowNumber, 1].Value = row.idNumber;
                            worksheet.Cells[rowNumber, 2].Value = row.firstName;
                            worksheet.Cells[rowNumber, 3].Value = row.middleName;
                            worksheet.Cells[rowNumber, 4].Value = row.lastName;
                            worksheet.Cells[rowNumber, 5].Value = row.birthdate;
                            worksheet.Cells[rowNumber, 6].Value = row.gender;
                            worksheet.Cells[rowNumber, 7].Value = row.contactNumber;
                            worksheet.Cells[rowNumber, 8].Value = row.telephoneNumber;
                            worksheet.Cells[rowNumber, 9].Value = row.emailAddress;
                            worksheet.Cells[rowNumber, 10].Value = row.address;
                            worksheet.Cells[rowNumber, 11].Value = row.campusName;
                            worksheet.Cells[rowNumber, 12].Value = row.educLevelName;
                            worksheet.Cells[rowNumber, 13].Value = row.yearSecName;
                            worksheet.Cells[rowNumber, 14].Value = row.description;
                            worksheet.Cells[rowNumber, 15].Value = row.collegeName;
                            worksheet.Cells[rowNumber, 16].Value = row.courseName;
                            worksheet.Cells[rowNumber, 17].Value = row.dateEnrolled;
                            worksheet.Cells[rowNumber, 18].Value = row.emergencyFullname;
                            worksheet.Cells[rowNumber, 19].Value = row.emergencyAddress;
                            worksheet.Cells[rowNumber, 20].Value = row.emergencyContact;
                            worksheet.Cells[rowNumber, 21].Value = row.emergencyRelationship;

                            using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                            {
                                range.Style.Font.Bold = false;
                                range.Style.ShrinkToFit = false;
                                range.Style.Numberformat.Format = "@";
                            }

                            rowNumber++;
                        }

                        for (int i = 1; i <= ColHeader.Length; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        package.Workbook.Properties.Title = wsTitle;
                        package.Workbook.Properties.Author = ExcelVar.Author;
                        package.Workbook.Properties.Company = ExcelVar.Company;

                        file = package.GetAsByteArray();
                    }
                }
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        [Route("batchupload/personstudents/{id:int}")]
        public async Task<IActionResult> BatchUploadStudentProcess(int id)
        {
            string message = "Batch file has been successfully process.";
            var entity = await _batchUploadService.GetByIdSync(id);
            var response = new BatchUploadResponse();
            try
            {
                await ValidateBatchStudentProcess(entity);
                int lastProcess = entity.ProcessCount;

                IEnumerable<personStudentBatchUploadVM> records = _batchUploadService.GetStudentRecords(entity.Path, ExcelVar.StudentColHeader.Count(), ExcelVar.StudentCollegeColHeader.Count());
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                // Do the update process here.
                response = await _person.PersonStudentBatchUpload(process, GetUserId(), entity.ID, lastProcess);

                entity.ProcessCount = entity.ProcessCount + process.Count();
                entity.Status = entity.ProcessCount == entity.TotalCount ? Constants.BATCH_UPLOAD_STATUS_SUCCESS : entity.Status;
                if (entity.Status != Constants.BATCH_UPLOAD_STATUS_SUCCESS)
                    message = string.Format("Processing {0} to {1}", lastProcess, entity.ProcessCount);

                await _batchUploadService.UpdateBatchUplaod(entity, GetUserId());

                response.ProcessCount = entity.ProcessCount;
                response.TotalCount = entity.TotalCount;
                response.Status = entity.Status;
                return Ok(new { message = message, response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public async Task ValidateBatchStudentProcess(batchUploadEntity upload)
        {
            // Validate if valid file.
            if (upload.Status == Constants.BATCH_UPLOAD_STATUS_CANCELLED)
                throw new Exception("Upload has been cancelled.");
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("batchupload/personstudents")]
        public async Task<IActionResult> BatchStudentUpload(IFormFile file)
        {
            string message = "Batch file has been successfully uploaded.";
            var response = new BatchUploadResponse();
            var entity = new batchUploadEntity();
            try
            {
                entity.Form_ID = (int)Form.People;
                entity.Date_Time_Added = DateTime.Now;
                await ValidateBatchStudentFile(file, entity);
                await _batchUploadService.Upload(file, entity, GetUserId());
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message, response, key = entity.ID });
        }

        public async Task ValidateBatchStudentFile(IFormFile file, batchUploadEntity upload)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
            var fileType = Path.GetExtension(originalFileName);

            // Validate if valid file.
            if (fileType != ".xlsx\"")
                throw new Exception("Invalid file! Please download the valid template.");

            try
            {
                upload.TotalCount = _batchUploadService.GetRecordsCount(file);
            }
            catch (Exception ex)
            {
                throw new Exception("File contains invalid data or missing fields.");
            }

            if (upload.TotalCount == 0)
                throw new Exception("File is empty!");
        }

        private async Task SavePhotoAsync(IFormFile file, string idnumber, string status)
        {
            if (file != null || file.Length != 0)
            { 
                var path = Path.Combine(PhotoStatSettings.photopath, status == "Photo" ? idnumber + "_Photo.jpg" : idnumber + "_Signature.jpg");

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            else
            {

            }
        }

        private void DeletePhoto(string idnumber, string status)
        {
            var path = Path.Combine(PhotoStatSettings.photopath, status == "Photo" ? idnumber + "_Photo.jpg" : idnumber + "_Signature.jpg");

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        private async Task SavePhotoAsync1(string encodedstr, string idnumber)
        {
            var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "photo",
                            idnumber + "_Photo.jpg")
                            ;
            await System.IO.File.WriteAllBytesAsync(path, Convert.FromBase64String(encodedstr));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("nocardlist")]
        public async Task<personPagedResultVM> GetAllWithNoCardList([FromQuery] string keyword, int pageNo, int pageSize)
        {
            return _mapper.Map<personPagedResultVM>(await _person.GetAllWithOutCardList(pageNo, pageSize, (keyword == null ? string.Empty : keyword)));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("withcardlist")]
        public async Task<personPagedResultVM> GetAllWithCardList([FromQuery] string keyword, int pageNo, int pageSize)
        {
            return _mapper.Map<personPagedResultVM>(await _person.GetAllWithCardList(pageNo, pageSize, (keyword == null ? string.Empty : keyword)));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("nocard")]
        public async Task<ICollection<personVM>> GetAllWithNoCard()
        {
            try
            {
                return _mapper.Map<ICollection<personVM>>(await _person.GetAllWithOutCard());
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("withcard")]
        public async Task<ICollection<personVM>> GetAllWithCard(int campusID)
        {
            return _mapper.Map<ICollection<personVM>>(await _person.GetAllWithCard());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("{id}")]
        public async Task<personVM> GetByID(long id)
        {
            return _mapper.Map<personVM>(await _person.GetPersonByID(id));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("getByIdNumber/{idNumber}")]
        public async Task<personVM> GetByIDNumber(string idNumber)
        {
            return _mapper.Map<personVM>(await _person.GetPersonByIDNumber(idNumber));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("getPersonImage/{idNumber}")]
        public async Task<IActionResult> GetPersonImage(string idNumber)
        {
            string path = Environment.CurrentDirectory + @"\Photo\" + idNumber + "_Photo.jpg";
            Byte[] imgByte = await System.IO.File.ReadAllBytesAsync(path);
            return File(imgByte, "image/jpeg");
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete("deletePersonVisitor/{id}")]
        public async Task<IActionResult> DeletePersonVisitor(long id)
        {
            await _person.DeletePersonVisitor(id, GetUserId());

            return Ok(new { message = "Person Visitor status" + SuccessMessageDelete });
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost("personvisitor")]
        public async Task<IActionResult> SavePersonVisitor([FromForm] personVisitorVM personvisitor)
        {
            string message = "Person Visitor" + SuccessMessageAdd;
            try
            {
                personEntity person = _mapper.Map<personEntity>(personvisitor);

                if (personvisitor.personId.HasValue)
                {
                    await _person.UpdatePersonVisitor(person, GetUserId());
                    message = "Person Visitor" + SuccessMessageUpdate;
                }
                else
                {
                    await _person.AddPersonVisitor(person, GetUserId());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message });
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("personstudents")]
        public async Task<studentPagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            return _mapper.Map<studentPagedResultVM>(await _person.GetAllStudents(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("personemployees")]
        public async Task<employeePagedResultVM> GetAllEmployees([FromQuery] PaginationParams param)
        {
            return _mapper.Map<employeePagedResultVM>(await _person.GetAllEmployees(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("personvisitors")]
        public async Task<visitorPagedResultVM> GetAllVisitors([FromQuery] PaginationParams param)
        {
            return _mapper.Map<visitorPagedResultVM>(await _person.GetAllVisitors(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("batchupload/personvisitors/{id:int}")]
        public async Task<IActionResult> BatchUploadVisitorProcess(int id)
        {
            string message = "Batch file has been successfully process.";
            var entity = await _batchUploadService.GetByIdSync(id);
            var response = new BatchUploadResponse();
            try
            {
                await ValidateBatchVisitorProcess(entity);
                int lastProcess = entity.ProcessCount;
                
                IEnumerable<visitorPersonBatchUploadVM> records = _batchUploadService.GetVisitorRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                // Do the update process here.
                response = await _person.BatchUpload(process, GetUserId(), entity.ID, lastProcess);

                entity.ProcessCount = entity.ProcessCount + process.Count();
                entity.Status = entity.ProcessCount == entity.TotalCount ? Constants.BATCH_UPLOAD_STATUS_SUCCESS : entity.Status;
                if (entity.Status != Constants.BATCH_UPLOAD_STATUS_SUCCESS)
                    message = string.Format("Processing {0} to {1}", lastProcess, entity.ProcessCount);

                await _batchUploadService.UpdateBatchUplaod(entity, GetUserId());

                response.ProcessCount = entity.ProcessCount;
                response.TotalCount = entity.TotalCount;
                response.Status = entity.Status;
                return Ok(new { message = message, response });
                    
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public async Task ValidateBatchVisitorProcess(batchUploadEntity upload)
        {

            // Validate if valid file.
            if (upload.Status == Constants.BATCH_UPLOAD_STATUS_CANCELLED)
                throw new Exception("Upload has been cancelled.");

        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost("batchupload/personvisitors")]
        public async Task<IActionResult> BatchUploadVisitor(IFormFile file)
        {
            string message = "Batch file has been successfully uploaded.";
            var response = new BatchUploadResponse();
            var entity = new batchUploadEntity();
            try
            {
                entity.Form_ID = (int)Form.People;
                entity.Date_Time_Added = DateTime.Now;
                await ValidateBatchVisitorFile(file, entity);
                await _batchUploadService.Upload(file, entity, GetUserId());
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message, response, key = entity.ID });
        }

        public async Task ValidateBatchVisitorFile(IFormFile file, batchUploadEntity upload)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
            var fileType = Path.GetExtension(originalFileName);

            // Validate if valid file.
            if (fileType != ".xlsx\"")
                throw new Exception("Invalid file! Please download the valid template.");
            
            try
            {
                upload.TotalCount = _batchUploadService.GetRecordsCount(file);
            }
            catch (Exception ex)
            {
                throw new Exception("File contains invalid data or missing fields.");
            }

            if (upload.TotalCount == 0)
                throw new Exception("File is empty!");
        }
    }
}