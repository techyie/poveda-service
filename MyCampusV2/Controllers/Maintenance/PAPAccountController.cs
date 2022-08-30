using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyCampusV2.Common.Helpers;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Common;
using MyCampusV2.Helpers.ActionFilters;
using MyCampusV2.Helpers.ExcelHelper;
using MyCampusV2.Helpers.PageResult;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.IServices;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyCampusV2.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class PAPAccountController : BaseController
    {
        private readonly IPAPAccountService _papAccountService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;
        private readonly IBatchUploadService _batchUploadService;
        private readonly BatchUploadSettings _batchSettings;
        private readonly EmailSettings _emailSettings;
        private readonly ExcelHelper _excelHelper;
        private readonly AppSettings _appSettings;
        private ResultModel result = new ResultModel();
        

        public PAPAccountController(IPAPAccountService papAccountService, IUserService userService, IMapper mapper, IAuditTrailService audit, IBatchUploadService batchUploadService, IOptions<BatchUploadSettings> batchSettings, IOptions<ExcelHelper> excelHelper, IOptions<EmailSettings> emailSettings, IOptions<AppSettings> appSettings)
        {
            this._papAccountService = papAccountService;
            this._userService = userService;
            _mapper = mapper;
            _audit = audit;
            _batchUploadService = batchUploadService;
            _batchSettings = batchSettings.Value;
            _excelHelper = excelHelper.Value;
            _emailSettings = emailSettings.Value;
            _appSettings = appSettings.Value;
        }

        [Authorize]
        [HttpGet]
        [Route("all")]
        public async Task<papAccountPagedResultVM> GetAll([FromQuery] PaginationParams param)
        {
            return _mapper.Map<papAccountPagedResultVM>(await _papAccountService.GetAll(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        [HttpGet]
        [Route("get")]
        public async Task<papAccountVM> GetPapAccount([FromQuery] string accountcode)
        {
            return _mapper.Map<papAccountVM>(await _papAccountService.GetPapAccountByAccountCode(accountcode));
        }

        [HttpGet]
        [Route("getStudents")]
        public async Task<IList<studentsVM>> GetStudents()
        {
            return _mapper.Map<IList<studentsVM>>(await _papAccountService.GetStudentList());
        }


        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] papAccountVM papAccount)
        {
            try
            {
                return await _papAccountService.AddMobileAppAccount(_mapper.Map<papAccountEntity>(papAccount), _emailSettings, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        //UpdateMobileAppAccount(papAccountEntity papAccount, int userId)

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("update")]
        public async Task<ResultModel> Update([FromBody] papAccountVM papAccount)
        {
            try
            {
                return await _papAccountService.UpdateMobileAppAccount(_mapper.Map<papAccountEntity>(papAccount), GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("delete")]
        public async Task<ResultModel> Delete([FromQuery] string accountCode)
        {
            try
            {
                return await _papAccountService.DeleteMobileAppTemporary(accountCode, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("sendemail/{accountCode}")]
        public async Task<ResultModel> SendEmail(string accountCode)
        {
            try
            {
                papAccountEntity account = await _papAccountService.GetPapAccountByAccountCode(accountCode);

                return await _papAccountService.SendEmail(_emailSettings, account.Email_Address, accountCode, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("validatechangepasswordlink/{accountCode}")]
        public async Task<ResultModel> ValidateChangePasswordLink(string accountCode)
        {
            try
            {
                return await _papAccountService.ValidateChangePasswordLink(_emailSettings, accountCode);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("activation")]
        public async Task<ResultModel> Activation([FromBody] AccountActivation accountActivation)
        {
            try
            {
                return await _papAccountService.AccountActivation(accountActivation.password, accountActivation.accountCode, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [HttpGet]
        [Route("export")]
        public async Task<IActionResult> Export([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<papAccountPagedResultVM>(await _papAccountService.Export(param.Keyword == null ? string.Empty : param.Keyword));
                byte[] file = null;

                if (result.papAccounts.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.AccountColHeader;
                        string wsTitle = ExcelVar.AccountTitle;
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
                        foreach (papAccountVM row in result.papAccounts)
                        {
                            worksheet.Cells[rowNumber, 1].Value = row.firstName;
                            worksheet.Cells[rowNumber, 2].Value = row.middleName;
                            worksheet.Cells[rowNumber, 3].Value = row.lastName;
                            worksheet.Cells[rowNumber, 4].Value = row.emailAddress;
                            worksheet.Cells[rowNumber, 5].Value = row.mobileNumber;
                            worksheet.Cells[rowNumber, 6].Value = row.username;
                            worksheet.Cells[rowNumber, 7].Value = row.lnkStudents.Replace(',', ';');
                            worksheet.Cells[rowNumber, 8].Value = (row.isPending == true ? "Pending" : (row.isActive == true ? "Active" : "Inactive"));   

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
        [Route("batchupload/{id:int}")]
        public async Task<IActionResult> BatchUploadProcess(int id)
        {
            string message = "Batch file has been successfully process.";
            var entity = await _batchUploadService.GetByIdSync(id);
            var response = new BatchUploadResponse();
            try
            {
                ValidateBatchProcess(entity);
                int lastProcess = entity.ProcessCount;

                IEnumerable<papAccountBatchUploadVM> records = _batchUploadService.GetPapAccountRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                // Do the update process here.
                response = await _papAccountService.BatchUpload(process, GetUserId(), entity.ID, lastProcess);

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

        public void ValidateBatchProcess(batchUploadEntity upload)
        {
            if (upload.Status == Constants.BATCH_UPLOAD_STATUS_CANCELLED)
                throw new Exception("Upload has been cancelled.");
        }

        [HttpPost]
        [Route("batchupload")]
        public async Task<IActionResult> BatchUpload(IFormFile file)
        {
            string message = "Batch file has been successfully uploaded.";
            var response = new BatchUploadResponse();
            var entity = new batchUploadEntity();
            try
            {
                entity.Form_ID = (int)Form.Mobile_App_Account;
                entity.Date_Time_Added = DateTime.Now;
                ValidateBatchFile(file, entity);
                await _batchUploadService.Upload(file, entity, GetUserId());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message, response, key = entity.ID });
        }

        public void ValidateBatchFile(IFormFile file, batchUploadEntity upload)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
            var fileType = Path.GetExtension(originalFileName);

            if (fileType != ".xlsx\"")
                throw new Exception("Invalid file! Please download the valid template.");

            try
            {
                upload.TotalCount = _batchUploadService.GetRecordsCount(file);
            }
            catch (Exception)
            {
                throw new Exception("File contains invalid data or missing fields.");
            }

            if (upload.TotalCount == 0)
                throw new Exception("File is empty!");
        }

        [HttpGet]
        //[Route("downloadtemplatearea")]
        [Route("downloadtemplate")]
        public async Task<IActionResult> DownloadTemplateArea([FromQuery] PaginationParams param, string type)
        {
            try
            {
                byte[] file = null;
                file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.AccountColHeaderSample, ExcelVar.AccountSampleData, ExcelVar.AccountTemplateTitle));

                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("signin")]
        public async Task<ResultModel> SignIn([FromBody] PapAccountSignIn papAccountSignIn)
        {
            try
            {
                papAccountSignIn.password = Encryption.Encrypt(papAccountSignIn.password);
                var account = await _papAccountService.SignIn(papAccountSignIn);

                if (account == null)
                    return CreateResult("401", "Invalid Credentials!", false);

                var user = await _userService.AuthenticateAdmin(papAccountSignIn.guestUsername, papAccountSignIn.guestPassword);

                if (user == null)
                    return CreateResult("401", "Invalid Guest Account!", false);

                var generatedToken = BuildToken(user.User_ID.ToString(), user.Person_ID.ToString(), user.Role_ID.ToString(), user.PersonEntity.Email_Address, account.Account_Code);

                return CreateResult("200", Newtonsoft.Json.JsonConvert.SerializeObject(new { token = generatedToken, accountCode = account.Account_Code }), true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        private string BuildToken(string userId, string personId, string userRole, string email, string accountCode)
        {
            var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.PrimarySid, personId),
            new Claim(ClaimTypes.Role, userRole),
            new Claim(ClaimTypes.Name, accountCode),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[JwtRegisteredClaimNames.Sub] = ClaimTypes.Upn;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_appSettings.Issuer,
              _appSettings.Audience,
              claims,
              expires: DateTime.Now.AddDays(1),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpGet]
        [Authorize]
        [CustomAuthorize]
        [Route("linkedstudentlist")]
        public async Task<ICollection<papAccountLinkedStudentsVM>> LinkedStudentList([FromQuery] string accountcode)
        {
            try
            {
                var data = _mapper.Map<ICollection<papAccountLinkedStudentsVM>>(await _papAccountService.GetPapAccountLinkedStudentsByAccountCode(accountcode));

                if (data != null)
                {
                    foreach(papAccountLinkedStudentsVM newData in data)
                    {
                        newData.imageByte = Convert.ToBase64String(await System.IO.File.ReadAllBytesAsync(PhotoStatSettings.photopath + "\\" + newData.idNumber + "_Photo.jpg"));
                    }
                }

                return data;
            } 
            catch(Exception err)
            {
                return null;
            }
        }
    }
}
