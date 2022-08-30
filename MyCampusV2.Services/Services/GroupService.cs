using MyCampusV2.Common;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.Helpers;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyCampusV2.Services.Services
{
    public class GroupService : BaseService, IGroupService
    {
        private ResultModel result = new ResultModel();

        public GroupService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user) { }

        public async Task<ResultModel> AddGroup(fetcherGroupEntity entity)
        {
            try
            {
                var exist = await _unitOfWork.GroupRepository.FindAsync(q => q.Group_Name == entity.Group_Name && q.IsActive == true && q.ToDisplay == true);

                if (exist != null)
                    return CreateResult("409", GROUP_EXIST, false);

                var data = await _unitOfWork.GroupRepository.AddAsyncWithBase(entity);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Added_By, (int)Form.Fetcher_Group, "Add Group", "INSERT", false, "Failed: " + entity.Group_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Group", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Added_By, (int)Form.Fetcher_Group, "Add Group", "INSERT", true, "Success: " + entity.Group_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Group" + Constants.SuccessMessageAdd, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> UpdateGroup(fetcherGroupEntity entity)
        {
            try
            {
                var data = await _unitOfWork.GroupRepository.UpdateAsyncWithBase(entity, entity.Fetcher_Group_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Updated_By, (int)Form.Fetcher_Group, "Update Group", "UPDATE", false, "Failed: " + entity.Group_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Group", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(entity.Updated_By, (int)Form.Fetcher_Group, "Update Group", "UPDATE", true, "Success: Fetcher Group ID: " + entity.Fetcher_Group_ID + " Group Name: " + entity.Group_Name, DateTime.UtcNow.ToLocalTime()));

                return CreateResult("200", "Group" + Constants.SuccessMessageUpdate, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteGroupPermanent(int id, int user)
        {
            try
            {
                fetcherGroupEntity group = await GetGroupByID(id);

                group.Updated_By = user;

                var data = await _unitOfWork.GroupRepository.DeleteAsyncPermanent(group, group.Fetcher_Group_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(group.Updated_By, (int)Form.Fetcher_Group, "Delete Permanently Fetcher Group", "PERMANENT DELETE", false, "Failed: " + group.Group_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Fetcher Group", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(group.Updated_By, (int)Form.Fetcher_Group, "Delete Permanently Fetcher Group", "PERMANENT DELETE", true, "Success: " + group.Group_Name, DateTime.UtcNow.ToLocalTime()));
                return CreateResult("200", "Fetcher Group" + Constants.SuccessMessagePermanentDelete, true);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<ResultModel> DeleteGroupTemporary(int id, int user)
        {
            try
            {
                fetcherGroupEntity group = await GetGroupByID(id);

                group.Updated_By = user;

                var data = await _unitOfWork.GroupRepository.DeleteAsyncTemporary(group, group.Fetcher_Group_ID);

                if (data == null)
                {
                    await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(group.Updated_By, (int)Form.Fetcher_Group, "Deactivate Fetcher Group", "DEACTIVATE", false, "Failed: " + group.Group_Name, DateTime.UtcNow.ToLocalTime()));
                    return CreateResult("409", "Fetcher Group", false);
                }

                await _unitOfWork.EventLoggingRepository.AddAsyn(fillEventLogging(group.Updated_By, (int)Form.Fetcher_Group, "Deactivate Fetcher Group", "DEACTIVATE", true, "Success: " + group.Group_Name, DateTime.UtcNow.ToLocalTime()));
                return CreateResult("200", "Fetcher Group" + Constants.SuccessMessageTemporaryDelete, true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        public async Task<fetcherGroupPagedResult> GetAllGroups(int pageNo, int pageSize, string keyword)
        {
            return await _unitOfWork.GroupRepository.GetAllGroups(pageNo, pageSize, keyword);
        }

        public async Task<ICollection<fetcherGroupEntity>> GetGroups()
        {
            try
            {
                return await _unitOfWork.GroupRepository.GetGroups().ToListAsync();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public async Task<fetcherGroupEntity> GetGroupByID(int id)
        {
            return await _unitOfWork.GroupRepository.GetGroupByID(id);
        }
    }
}
