using MyCampusV2.Common;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.Services.Services
{
    public class DataSyncService : BaseService, IDataSyncService
    {
        public DataSyncService(IUnitOfWork unitOfWork, IAuditTrailService audit, ICurrentUser user) : base(unitOfWork, audit, user)
        {
        }

        public async Task AddDataSync(datasyncEntity dataSync, int user)
        {
            dataSync.Date_Time_Added = DateTime.Now;
            dataSync.Added_By = user;
            await _unitOfWork.DataSyncRepository.AddAsyn(dataSync);
        }

        public async Task DataSyncAdd(datasyncEntity dataSync, int user)
        {
           dataSync.DS_Action = "A";
           await AddDataSync(dataSync, user);
        }

        public async Task DataSyncDelete(datasyncEntity dataSync, int user)
        {
            dataSync.DS_Action = "D";
            await AddDataSync(dataSync, user);
        }

        public async Task DataSyncUpdate(datasyncEntity dataSync, int user)
        {
             dataSync.DS_Action = "U";
            await AddDataSync(dataSync, user);
        }

        public async Task DeleteDataSync(long id, int user)
        {
            try
            {
                datasyncEntity data = await GetDataSync(id);

                if (data.IsActive)
                {
                    data.IsActive = false;
                }
                else
                {
                    data.IsActive = true;
                }

                await _unitOfWork.DataSyncRepository.UpdateAsyn(data, data.DataSync_ID);
               // await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Card_Card, string.Format("Updated Card {1} status to {0}", card.IsActive ? "Active" : "Inactive", card.Card_Serial));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<datasyncEntity> GetDataSync(long id)
        {
            return await _unitOfWork.DataSyncRepository.GetAsync(id);
        }

        public async Task UpdateDataSync(datasyncEntity dataSync, int user)
        {
            try
            {
                datasyncEntity oldDatasync = await GetDataSync(dataSync.DataSync_ID);
                dataSync.Updated_By = user;
                dataSync.Last_Updated = DateTime.Now;
                await _unitOfWork.DataSyncRepository.UpdateAsyn(dataSync, dataSync.DataSync_ID);
                //await _unitOfWork.AuditTrailRepository.AuditAsync(user, (int)Form.Campus_Campus, string.Format("Updated: Card Serial: {0}, Issued Date: {1}, Expiry Date: {2},Remarks: {3} , PersonID:{4}", card.Card_Serial, card.Issued_Date, card.Expiry_date, card.Remarks, card.Person_ID));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
