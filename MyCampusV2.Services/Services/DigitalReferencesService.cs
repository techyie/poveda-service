using Microsoft.AspNetCore.Http;
using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.Services
{
    public class DigitalReferencesService : BaseService, IDigitalReferencesService
    {
        private string _digitalReferencesBatch = AppDomain.CurrentDomain.BaseDirectory + @"Digital References\";
        private ResultModel result = new ResultModel();

        public DigitalReferencesService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user)
            : base(unitOfWork, audit, user)
        {

        }

        public async Task<ResultModel> AddDigitalReference(digitalReferencesEntity digitalReferencesEntity, int userId)
        {
            try
            {
                var data = await _unitOfWork.DigitalReferencesRepository.FindAsync(q => q.Title == digitalReferencesEntity.Title && q.IsActive == true);

                if (data != null)
                {
                    return CreateResult("409", "Conflict on data.", false);
                }

                return await _unitOfWork.DigitalReferencesRepository.AddDigitalReference(digitalReferencesEntity, userId);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteDigitalReferencePermanent(string digitalReferenceCode, int userId)
        {
            try
            {
                return await _unitOfWork.DigitalReferencesRepository.DeleteDigitalReferencePermanent(digitalReferenceCode, userId);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateDigitalReference(digitalReferencesEntity digitalReferencesEntity, int userId)
        {
            try
            {
                return await _unitOfWork.DigitalReferencesRepository.UpdateDigitalReference(digitalReferencesEntity, userId);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<digitalReferencesPagedResult> GetAll(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.DigitalReferencesRepository.GetAll(pageNo, pageSize, keyword);
        }

        public async Task<digitalReferencesEntity> GetDigitalReferenceByCode(string code)
        {
            return await _unitOfWork.DigitalReferencesRepository.GetDigitalReferenceByCode(code);
        }

        public async Task<digitalReferencesPagedResult> Export(string keyword)
        {
            return await _unitOfWork.DigitalReferencesRepository.Export(keyword);
        }

        public async Task<ResultModel> Upload(IFormFile file, string filePath)
        {
            try
            {

                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
                var fileName = DateTime.UtcNow.ToLocalTime().ToString("yyyyMMddhhmmssfff") + Path.GetExtension(originalFileName);

                fileName = fileName.Contains("\\")
                    ? fileName.Trim('"').Substring(fileName.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                    : fileName.Trim('"');

                var fullFilePath = Path.Combine(filePath, fileName);
                if (file.Length > 0)
                {
                    using (var stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                return CreateResult("200", fileName, true);
            } 
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }
    }
}
