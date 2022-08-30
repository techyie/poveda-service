using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Helpers.Constants;
using MyCampusV2.Helpers.ExcelHelper;
using MyCampusV2.Helpers.PageResult;
using MyCampusV2.IServices;
using MyCampusV2.Models;
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
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : BaseController
    {
        private readonly ICardDetailsService _card;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;
        private readonly IBatchUploadService _batchUploadService;
        private readonly BatchUploadSettings _batchSettings;
        private readonly ExcelHelper _excelHelper;

        public CardController(ICardDetailsService card, IMapper mapper, IAuditTrailService audit, IBatchUploadService batchUploadService, IOptions<BatchUploadSettings> batchSettings, IOptions<ExcelHelper> excelHelper)
        {
            this._card = card;
            this._mapper = mapper;
            this._audit = audit;
            this._batchUploadService = batchUploadService;
            this._batchSettings = batchSettings.Value;
            _excelHelper = excelHelper.Value;
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        public async Task<ICollection<cardVM>> GetAll()
        {
            return _mapper.Map<ICollection<cardVM>>(await _card.GetAll());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("{id:int}")]
        public async Task<cardVM> GetByID(int id)
        {
            try
            {
                return _mapper.Map<cardVM>(await _card.GetCard(id));
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("serial/{cardSerial}")]
        public async Task<cardVM> GetByCardSerial(string cardSerial)
        {
            try
            {
                var test = _mapper.Map<cardVM>(await _card.GetByCardSerial(cardSerial));
                return test;
            } 
            catch(Exception err)
            {
                throw err;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("assigncard")]
        public async Task<ResultModel> AssignCard([FromBody]cardVM card)
        {
            try
            {
                card.uid = card.uid.Replace(" ", string.Empty);
                card.uid = Int64.Parse(card.uid, System.Globalization.NumberStyles.HexNumber).ToString();
                card.uid = card.uid.PadLeft(10, '0');

                //string newID = new String(card.idNumber.ToCharArray().Where(id => Char.IsDigit(id)).ToArray());
                string PAN = DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm") + "0000000000" + "01" + Convert.ToDateTime(card.expiryDate).ToString("yyyyMM");
                PAN = PAN.PadRight(8, '0');
                
                var newCard = _mapper.Map<cardDetailsEntity>(card);
                newCard.PAN = PAN;

                return await _card.AssignCard(newCard, GetUserId());
            } 
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        //[Authorize]
        [HttpPost]
        [Route("reassign")]
        public async Task<ResultModel> ReassignCard([FromBody] cardVM card)
        {
            try
            {
                var oldCard = _mapper.Map<cardDetailsEntity>(card);

                card.uid = card.uid.Replace(" ", string.Empty);
                card.uid = Int64.Parse(card.uid, System.Globalization.NumberStyles.HexNumber).ToString();
                card.uid = card.uid.PadLeft(10, '0');

                string PAN = DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm") + "0000000000" + "01" + Convert.ToDateTime(card.expiryDate).ToString("yyyyMM");
                PAN = PAN.PadRight(8, '0');

                var newCard = _mapper.Map<cardDetailsEntity>(card);
                newCard.PAN = PAN;

                return await _card.ReassignCard(oldCard, newCard, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpPut]
        public async Task<ResultModel> Update([FromBody]cardVM card)
        {
            try
            {
                var newCard = _mapper.Map<cardDetailsEntity>(card);
                return await _card.UpdateCard(newCard, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpPut]
        [Route("deactivate")]
        public async Task<ResultModel> Deactivate([FromBody] cardVM card)
        {
            try
            {
                var newCard = _mapper.Map<cardDetailsEntity>(card);
                return await _card.DeactivateCard(newCard, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("batch/deactive/{id:int}")]
        public async Task<IActionResult> BatchDeactivate(int id)
        {
            string message = "Batch file has been successfully processed.";
            var entity = await _batchUploadService.GetByIdSync(id);
            var response = new BatchUploadResponse();
            try
            {
                await ValidateBatchProcess(entity);
                int lastProcess = entity.ProcessCount;

                IEnumerable<cardBatchDeactiveVM> records = _batchUploadService.GetCardDeactivateRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();

                response = await _card.BatchDeactivate(process, GetUserId(), entity.ID, lastProcess);

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

        public async Task ValidateBatchProcess(batchUploadEntity upload)
        {
            // Validate if valid file.
            if (upload.Status == Constants.BATCH_UPLOAD_STATUS_CANCELLED)
                throw new Exception("Upload has been cancelled.");
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpPost("batch/deactive")]
        public async Task<IActionResult> BatchDeactivate(IFormFile file)
        {
            string message = "Batch file has been successfully uploaded.";
            var response = new BatchUploadResponse();
            var entity = new batchUploadEntity();
            try
            {
                entity.Form_ID = (int)Form.Campus_Card;
                await ValidateDeactiveBatchFile(file, entity);
                await _batchUploadService.Upload(file, entity, GetUserId());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message, response, key = entity.ID });
        }

        public async Task ValidateDeactiveBatchFile(IFormFile file, batchUploadEntity upload)
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
            catch (Exception)
            {

                throw new Exception("File contains invalid data or missing fields.");
            }
            if (upload.TotalCount == 0)
                throw new Exception("File is empty!");

        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("batch/update/{id:int}")]
        public async Task<IActionResult> BatchUpdate(int id)
        {
            string message = "Batch file has been successfully processed.";
            var entity = await _batchUploadService.GetByIdSync(id);
            var response = new BatchUploadResponse();
            try
            {
                await ValidateBatchProcess(entity);
                
                int lastProcess = entity.ProcessCount;
                    
                IEnumerable<cardBatchUpdateVM> records = _batchUploadService.GetCardUpdateRecords(entity.Path);
                var process = records.Skip(entity.ProcessCount).Take(_batchSettings.ProcessCount).ToList();
                // Do the update process here.
                response = await _card.BatchUpdate(process, GetUserId(), entity.ID, lastProcess);
                    
                entity.ProcessCount = entity.ProcessCount + process.Count();
                entity.Status = entity.ProcessCount == entity.TotalCount ? Constants.BATCH_UPLOAD_STATUS_SUCCESS : entity.Status;
                if(entity.Status != Constants.BATCH_UPLOAD_STATUS_SUCCESS)
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

        [Authorize]
        //[CustomAuthorize]
        [HttpPost("batch/update")]
        public async Task<IActionResult> BatchUpdate(IFormFile file)
        {
            string message = "Batch file has been successfully uploaded.";
            var response = new BatchUploadResponse();
            var entity = new batchUploadEntity();
            try
            {
                entity.Form_ID = (int)Form.Campus_Card;
                entity.Date_Time_Added = DateTime.Now;
                await ValidateUpdateBatchFile(file, entity);
                await _batchUploadService.Upload(file, entity, GetUserId());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message, response, key = entity.ID });
        }

        public async Task ValidateUpdateBatchFile(IFormFile file, batchUploadEntity upload)
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
            catch (Exception)
            {

                throw new Exception("File contains invalid data or missing fields.");
            }

            if (upload.TotalCount == 0)
                throw new Exception("File is empty!");
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("person/{id}")]
        public async Task<cardVM> GetByPersonID(long id)
        {
            return _mapper.Map<cardVM>(await _card.GetByPerson(id));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("person")]
        public async Task<cardVM> GetByIdNumber(string idNumber)
        {
            return _mapper.Map<cardVM>(await _card.GetByIdNumber(idNumber));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("personall/{id}")]
        public async Task<ICollection<cardVM>> GetAllByPersonID(long id)
        {
            return _mapper.Map<ICollection<cardVM>>(await _card.GetAllByPerson(id));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("exportcardexcelfile")]
        public async Task<IActionResult> ExportList([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<cardPagedResultVM>(await _card.ExportCards(param.Keyword));
                byte[] file = null;

                if (result.cards.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.UpdateColHeader;
                        string wsTitle = ExcelVar.UpdateTitle;
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
                        foreach (cardVM row in result.cards)
                        {
                            //worksheet.Cells[rowNumber, 1].Value = row.idNumber;
                            //worksheet.Cells[rowNumber, 2].Value = row.expiryDate;
                            //worksheet.Cells[rowNumber, 3].Value = row.Status;
                            //worksheet.Cells[rowNumber, 4].Value = row.Remarks;

                            using (var range = worksheet.Cells[rowNumber, 1, rowNumber, ColHeader.Length])
                            {
                                range.Style.Font.Bold = false;
                                range.Style.ShrinkToFit = false;
                                range.Style.Numberformat.Format = "@";
                            }

                            using (var range = worksheet.Cells[rowNumber, 2, rowNumber, 2])
                            {
                                range.Style.Font.Bold = false;
                                range.Style.ShrinkToFit = false;
                                range.Style.Numberformat.Format = ExcelVar.dateFormat;
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
        [HttpGet("exportcarddeacexcelfile")]
        public async Task<IActionResult> ExportDeactivateList([FromQuery] PaginationParams param)
        {
            try
            {
                var result = _mapper.Map<cardPagedResultVM>(await _card.ExportCards(param.Keyword));
                byte[] file = null;
                
                if (result.cards.Count != 0)
                {
                    using (var package = new ExcelPackage())
                    {
                        string[] ColHeader = ExcelVar.DeactivateColHeader;
                        string wsTitle = ExcelVar.DeactivateTitle;
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
                        foreach (cardVM row in result.cards)
                        {
                            //worksheet.Cells[rowNumber, 1].Value = row.idNumber;
                            //worksheet.Cells[rowNumber, 2].Value = row.Remarks;

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
        [HttpGet("downloadtemplatedeactivate")]
        public async Task<IActionResult> DownloadTemplateDeactivate([FromQuery] PaginationParams param)
        {
            try
            {
                byte[] file = null;
                file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.DeactivateColHeader, ExcelVar.DeactivateSampleData, ExcelVar.DeactivateTitle));
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("downloadtemplateupdate")]
        public async Task<IActionResult> DownloadTemplateUpdate([FromQuery] PaginationParams param)
        {
            try
            {
                byte[] file = null;
                file = await Task.Run(() => _excelHelper.ExportTemplate(ExcelVar.UpdateColHeader, ExcelVar.UpdateSampleData, ExcelVar.UpdateTitle));
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
