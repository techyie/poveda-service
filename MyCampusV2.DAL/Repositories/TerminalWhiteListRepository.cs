﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyCampusV2.Common.Helpers;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.Helpers;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.Models;
using MyCampusV2.Models.V2.entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public class TerminalWhiteListRepository : BaseRepository<terminalWhitelistEntity>, ITerminalWhiteListRepository
    {
        private ResultModel result = new ResultModel();
        private Response response = new Response();

        public TerminalWhiteListRepository(MyCampusCardContext context) : base(context)
        {
        }

        public async Task<ICollection<terminalWhitelistEntity>> GetAllByCardID(long card_Details_Id)
        {
            //return await _context.terminalWhitelistEntity.Where(a => a.Card_Details_ID == card_Details_Id).ToListAsync();
            return null;
        }

        public async Task<ResultModel> RemoveFromTerminalWhitelist(terminalWhitelistEntity terminalWhitelistEntity, datasyncEntity datasyncEntity, eventLoggingEntity eventLogging)
        {
            result = new ResultModel();

            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await this._context.DatasyncEntity.AddAsync(datasyncEntity);

                    this._context.TerminalWhitelistEntity.Remove(terminalWhitelistEntity);

                    await this._context.EventLoggingEntity.AddAsync(eventLogging);

                    await this._context.SaveChangesAsync();

                    contextTransaction.Commit();

                    return response.CreateResponse("200", "Terminal Whitelist" + Constants.SuccessMessageDelete, true);
                }
                catch (Exception err)
                {
                    contextTransaction.Rollback();
                    return response.CreateResponse("500", err.Message, false);
                }
            }
        }

        public async Task<ResultModel> AddToTerminalWhitelist(terminalWhitelistEntity terminalWhitelistEntity, datasyncEntity datasyncEntity, eventLoggingEntity eventLogging)
        {
            result = new ResultModel();

            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await this._context.DatasyncEntity.AddAsync(datasyncEntity);

                    await this._context.TerminalWhitelistEntity.AddAsync(terminalWhitelistEntity);

                    await this._context.EventLoggingEntity.AddAsync(eventLogging);

                    await this._context.SaveChangesAsync();

                    contextTransaction.Commit();

                    return response.CreateResponse("200", "Terminal Whitelist" + Constants.SuccessMessageAdd, true);
                }
                catch (Exception err)
                {
                    contextTransaction.Rollback();
                    return response.CreateResponse("500", err.Message, false);
                }
            }
        }

        public async Task<terminalWhitelistPagedResult> GetAllTerminalWhitelist(long id, int pageNo, int pageSize, string keyword)
        {
            var result = new terminalWhitelistPagedResult();
            result.CurrentPage = pageNo;
            result.PageSize = pageSize;

            result.RowCount = Convert.ToInt32(_context.tbl_terminal_whitelist_count.FromSql("call spTerminal_Whitelist_Count({0}, {1});", id, keyword)
                                                .FirstOrDefault().Count);

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            result.whitelist = await _context.terminalWhitelistVM.FromSql(
                                    "call spTerminal_Whitelist_Search(@terminalid, @searchword, @pageno, @pagesize)",
                                    new MySqlParameter("@terminalid", id),
                                    new MySqlParameter("@searchword", keyword),
                                    new MySqlParameter("@pageno", pageNo),
                                    new MySqlParameter("@pagesize", pageSize)).ToListAsync();

            return result;
        }

        public async Task<terminalWhitelistPagedResult> GetTerminalWhitelistV2(int pageNo, int pageSize, string keyword, int fetcheruse, int terminalid)
        {
            try
            {
                var entity = new List<terminalWhitelistVM>();
                int pageCount = 0;
                int rowCount = 0;
                using (MySqlConnection conn = new MySqlConnection(this._context.Database.GetDbConnection().ConnectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "get_terminal_whitelist";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pageNo", pageNo);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                        cmd.Parameters.AddWithValue("@keyword", keyword);
                        cmd.Parameters.AddWithValue("@fetcheruse", fetcheruse);
                        cmd.Parameters.AddWithValue("@terminalid", terminalid);

                        cmd.Parameters.AddWithValue("@rowCount", MySqlDbType.Int32);
                        cmd.Parameters["@rowCount"].Direction = ParameterDirection.Output;

                        cmd.Parameters.AddWithValue("@pageCount", MySqlDbType.Int32);
                        cmd.Parameters["@pageCount"].Direction = ParameterDirection.Output;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                entity.Add(new terminalWhitelistVM()
                                {
                                    personId = (reader["personId"] == null ? 0 : Convert.ToInt32(reader["personId"])),
                                    idNumber = Convert.ToString(reader["idNumber"]),
                                    firstName = Convert.ToString(reader["firstName"]),
                                    lastName = Convert.ToString(reader["lastName"]),
                                    cardSerial = (reader["cardSerial"] == null ? string.Empty : Convert.ToString(reader["cardSerial"])),
                                    status = (reader["status"] == null ? 0 : Convert.ToInt32(reader["status"])),
                                    whitelistId = (reader["whitelistId"] == null ? 0 : Convert.ToInt32(reader["whitelistId"])),
                                    datasyncId = (reader["datasyncId"] == null ? 0 : Convert.ToInt32(reader["datasyncId"])),
                                    cardStatus = (reader["cardstatus"] == null ? 0 : Convert.ToInt32(reader["cardstatus"])),
                                });
                            }
                        }

                        rowCount = Convert.ToInt32(cmd.Parameters["@rowCount"].Value);
                        pageCount = Convert.ToInt32(cmd.Parameters["@pageCount"].Value);
                    }
                }

                terminalWhitelistPagedResult result = new terminalWhitelistPagedResult();
                result.PageCount = pageCount;
                result.RowCount = rowCount;
                result.whitelist = entity;
                result.CurrentPage = pageNo;
                result.PageSize = pageSize;

                return result;
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public IQueryable<terminalWhitelistVM> GetWhitelist(long id, string keyword)
        {
            return _context.terminalWhitelistVM.FromSql<terminalWhitelistVM>
                ("call spTerminal_Whitelist_Search(@terminalid, @searchword)",
                new MySqlParameter("@terminalid", id),
                new MySqlParameter("@searchword", keyword)
                ).AsNoTracking();
        }

    }

}
