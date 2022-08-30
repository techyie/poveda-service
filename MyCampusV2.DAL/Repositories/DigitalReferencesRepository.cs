using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class DigitalReferencesRepository : BaseRepository<digitalReferencesEntity>, IDigitalReferencesRepository
    {
        private readonly MyCampusCardContext context;
        private ResultModel resultModel;
        private Response response;
        private string RepositoryName = "Digital Reference";

        public DigitalReferencesRepository(MyCampusCardContext Context)
            : base(Context)
        {
            this.context = Context;
        }

        public async Task<ResultModel> AddDigitalReference(digitalReferencesEntity digitalReferencesEntity, int userId)
        {
            response = new Response();

            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    digitalReferencesEntity.IsActive = true;
                    digitalReferencesEntity.ToDisplay = true;
                    digitalReferencesEntity.Added_By = userId;
                    digitalReferencesEntity.Updated_By = userId;
                    digitalReferencesEntity.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                    digitalReferencesEntity.Last_Updated = DateTime.UtcNow.ToLocalTime();

                    digitalReferencesEntity.Digital_Reference_Code = System.Guid.NewGuid().ToString();

                    await _context.DigitalReferencesEntity.AddAsync(digitalReferencesEntity);

                    await _context.SaveChangesAsync();

                    contextTransaction.Commit();

                    return response.CreateResponse("200", Constants.SuccessDigitalFileUpload, true);
                }
                catch (Exception err)
                {
                    contextTransaction.Rollback();
                    return response.CreateResponse("500", Constants.FailedDigitalFileUpload, false);
                }
            }
        }

        public async Task<ResultModel> UpdateDigitalReference(digitalReferencesEntity digitalReferencesEntity, int userId)
        {
            response = new Response();

            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var digitalReferenceData = _context.DigitalReferencesEntity.SingleOrDefault(x => x.Digital_Reference_Code == digitalReferencesEntity.Digital_Reference_Code);

                    if (digitalReferenceData != null)
                    {
                        digitalReferenceData.Title = digitalReferencesEntity.Title;
                        digitalReferenceData.File_Type = digitalReferencesEntity.File_Type;
                        digitalReferenceData.File_Path = digitalReferencesEntity.File_Path;
                        digitalReferenceData.File_Name = digitalReferencesEntity.File_Name;

                        digitalReferenceData.Updated_By = userId;
                        digitalReferenceData.Last_Updated = DateTime.UtcNow.ToLocalTime();
                        digitalReferenceData.Date_Uploaded = digitalReferencesEntity.Date_Uploaded;

                        _context.Entry(digitalReferenceData).Property(x => x.Date_Time_Added).IsModified = false;
                        _context.Entry(digitalReferenceData).Property(x => x.Added_By).IsModified = false;
                        _context.Entry(digitalReferenceData).Property(x => x.Last_Updated).IsModified = true;

                        await _context.SaveChangesAsync();
                        dbcxtransaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return response.CreateResponse("500", Constants.FailedUpdateDigitalFileUpload, false);
                }
            }

            return response.CreateResponse("200", Constants.SuccessUpdateDigitalFileUpload, true);
        }

        public async Task<ResultModel> DeleteDigitalReferencePermanent(string digitalReferenceCode, int userId)
        {
            response = new Response();

            using (IDbContextTransaction dbcxtransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var digitalReferenceData = _context.DigitalReferencesEntity.SingleOrDefault(x => x.Digital_Reference_Code == digitalReferenceCode);

                    digitalReferenceData.IsActive = false;
                    digitalReferenceData.ToDisplay = false;
                    digitalReferenceData.Updated_By = userId;
                    digitalReferenceData.Last_Updated = DateTime.UtcNow.ToLocalTime();

                    _context.Entry(digitalReferenceData).Property(x => x.Date_Time_Added).IsModified = false;
                    _context.Entry(digitalReferenceData).Property(x => x.Added_By).IsModified = false;
                    _context.Entry(digitalReferenceData).Property(x => x.Last_Updated).IsModified = true;

                    await _context.SaveChangesAsync();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return response.CreateResponse("500", Constants.FailedArchivedDigitalFileUpload, false);
                }
            }

            return response.CreateResponse("200", Constants.SuccessArchivedDigitalFileUpload, true);
        }

        public async Task<digitalReferencesPagedResult> GetAll(int pageNo, int pageSize, string keyword)
        {
            try
            {
                var result = new digitalReferencesPagedResult();
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                if (keyword == null || keyword == "")
                    result.RowCount = this.context.DigitalReferencesEntity.Where(a => a.ToDisplay == true).Count();
                else
                    result.RowCount = this.context.DigitalReferencesEntity
                        .Where(q => q.ToDisplay == true && (q.Title.Contains(keyword)
                        || q.Date_Uploaded.ToString().Contains(keyword)
                        || q.File_Type.Contains(keyword))).Count();

                var pageCount = (double)result.RowCount / pageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageNo - 1) * pageSize;

                if (keyword == null || keyword == "")
                    result.digitalReferences = await this.context.DigitalReferencesEntity.Where(a => a.ToDisplay == true)
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();
                else
                    result.digitalReferences = await this.context.DigitalReferencesEntity
                        .Where(q => q.ToDisplay == true && (q.Title.Contains(keyword)
                        || q.Date_Uploaded.ToString().Contains(keyword)
                        || q.File_Type.Contains(keyword)))
                        .OrderByDescending(c => c.Last_Updated)
                        .Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<digitalReferencesEntity> GetDigitalReferenceByCode(string code)
        {
            return await this.context.DigitalReferencesEntity.Where(x => x.Digital_Reference_Code == code).FirstOrDefaultAsync();
        }

        public async Task<digitalReferencesPagedResult> Export(string keyword)
        {
            try
            {
                var result = new digitalReferencesPagedResult();

                if (keyword == null || keyword == "")
                {
                    result.digitalReferences = await _context.DigitalReferencesEntity
                        .Where(b => b.ToDisplay == true)
                        .OrderBy(c => c.Last_Updated).ToListAsync();
                }
                else
                {
                    result.digitalReferences = await _context.DigitalReferencesEntity
                        .Where(q => q.ToDisplay == true && (q.Title.Contains(keyword)
                        || q.Date_Uploaded.ToString().Contains(keyword)
                        || q.File_Type.Contains(keyword)))
                       .OrderBy(c => c.Last_Updated).ToListAsync();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
