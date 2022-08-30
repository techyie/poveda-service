using Microsoft.AspNetCore.Http;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace MyCampusV2.Services.IServices
{
    public interface IDigitalReferencesService
    {
        Task<ResultModel> AddDigitalReference(digitalReferencesEntity digitalReferencesEntity, int userId);
        Task<ResultModel> UpdateDigitalReference(digitalReferencesEntity digitalReferencesEntity, int userId);
        Task<digitalReferencesEntity> GetDigitalReferenceByCode(string code);
        Task<digitalReferencesPagedResult> GetAll(int pageNo, int pageSize, string keyword);
        Task<ResultModel> DeleteDigitalReferencePermanent(string digitalReferenceCode, int userId);
        Task<digitalReferencesPagedResult> Export(string keyword);
        Task<ResultModel> Upload(IFormFile file, string filePath);
    }
}
