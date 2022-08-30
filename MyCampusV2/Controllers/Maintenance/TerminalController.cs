using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.IServices;
using MyCampusV2.Models;
using MyCampusV2.Services.IServices;
using MyCampusV2.Helpers.Encryption;
using MyCampusV2.Helpers.PageResult;
using Microsoft.AspNetCore.Authorization;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Helpers.ActionFilters;
using MyCampusV2.Helpers;
using System.Net.NetworkInformation;
using System.Net;

namespace MyCampusV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TerminalController : BaseController
    {
        private readonly ITerminalService _terminal;
        private readonly ITerminalWhiteListService _terminalWhitelist;
        private readonly IMapper _mapper;
        private TerminalCommand terminalCommand = new TerminalCommand();
        private Logger logger = new Logger();

        public TerminalController(ITerminalService terminal, ITerminalWhiteListService terminalWhitelist, IMapper mapper)
        {
            this._terminal = terminal;
            this._terminalWhitelist = terminalWhitelist;
            this._mapper = mapper;
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet]
        public async Task<ICollection<terminalVM>> GetAll()
        {
            return _mapper.Map<ICollection<terminalVM>>(await _terminal.GetAll());
        }

        //[Authorize]
        //[CustomAuthorize]
        //[HttpGet]
        //[Route(("terminals"))]
        //public async Task<terminalPagedResultVM> GetTerminalList([FromQuery] PaginationParams param)
        //{
        //    return _mapper.Map<terminalPagedResultVM>(await _terminal.GetAllTerminals(param.PageNo, param.PageSize, param.Keyword));
        //}

        [HttpGet]
        [Route(("terminals"))]
        public async Task<terminalPagedResultVM> GetTerminalList()
        {
            PaginationParams param = new PaginationParams();
            param.PageNo = 1;
            param.PageSize = 5;
            param.Keyword = string.Empty;
            return _mapper.Map<terminalPagedResultVM>(await _terminal.GetAllTerminals(param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("area/{id:int}")]
        public async Task<ICollection<terminalVM>> GetByArea(int id)
        {
            return _mapper.Map<ICollection<terminalVM>>(await _terminal.GetByArea(id));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("area/terminal/{id:int}")]
        public async Task<ICollection<terminalVM>> GetTerminalByArea(int id)
        {
            return _mapper.Map<ICollection<terminalVM>>(await _terminal.GetTerminalByArea(id));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("campus/{id:int}")]
        public async Task<List<terminalVM>> GetTerminalByCampusId(int id)
        {
            return _mapper.Map<List<terminalVM>>(await _terminal.GetTerminalByCampusId(id));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("area/{id:int}/{isnotification:int}")]
        public async Task<ICollection<terminalVM>> GetByArea(int id, int isnotification)
        {
            if (isnotification == 1)
                return _mapper.Map<ICollection<terminalVM>>(await _terminal.GetByArea(id, isnotification));
            else
                return _mapper.Map<ICollection<terminalVM>>(await _terminal.GetByArea(id));
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("{id:int}")]
        public async Task<terminalVM> GetByID(int id)
        {
            return _mapper.Map<terminalVM>(await _terminal.GetById(id));
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPost]
        [Route("create")]
        public async Task<ResultModel> Create([FromBody] terminalVM entity)
        {
            try
            {
                entity.addedBy = GetUserId();
                entity.updatedBy = GetUserId();
                return await _terminal.AddTerminal(_mapper.Map<terminalEntity>(entity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpPut]
        [Route("update")]
        public async Task<ResultModel> Update([FromBody] terminalVM entity)
        {
            try
            {
                entity.updatedBy = GetUserId();
                return await _terminal.UpdateTerminal(_mapper.Map<terminalEntity>(entity));
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [CustomAuthorize]
        [HttpDelete]
        [Route("{id}")]
        public async Task<ResultModel> Delete(int id)
        {
            
            return await _terminal.DeleteTerminal(id, GetUserId());
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpGet("whitelist/{id:int}")]
        public async Task<terminalWhitelistPagedResultVM> GetWhitelist(long id, [FromQuery] PaginationParams param)
        {
            return _mapper.Map<terminalWhitelistPagedResultVM>(await _terminalWhitelist.GetAllTerminalWhitelist(id, param.PageNo, param.PageSize, param.Keyword));
        }

        [Authorize]
        [HttpGet("getterminalwhitelist/{id}")]
        public async Task<terminalWhitelistPagedResult> GetTerminalWhitelist(int id, [FromQuery] PaginationParams param)
        {
            return await _terminalWhitelist.GetTerminalWhitelistV2(param.PageNo, param.PageSize, param.Keyword, id);
        }

        [Authorize]
        [HttpPost("rebootTerminal/{id}")]
        public async Task<ResultModel> RebootTerminal(int id)
        {
            try
            {

                logger.Activity("Reboot Command Terminal ID: " + id);

                var terminalData = await _terminal.GetById(id);

                bool result = terminalCommand.RebootCommand(terminalData);

                if (result)
                    return CreateResult("200", "Command successfully sent!", true);
                else
                    return CreateResult("500", "Failed to send the command!", false);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [HttpPost("shutdownTerminal/{id}")]
        public async Task<ResultModel> ShutdownTerminal(int id)
        {
            try
            {
                var terminalData = await _terminal.GetById(id);

                logger.Activity("Shutdown Command Terminal: " + terminalData.Terminal_Name);

                bool result = terminalCommand.ShutdownCommand(terminalData);

                if (result)
                    return CreateResult("200", "Command successfully sent!", true);
                else
                    return CreateResult("500", "Failed to send the command!", false);
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [HttpPost("shutdownTerminalAll")]
        public async Task<ResultModel> ShutdownTerminalAll()
        {
            try
            {
                var terminals = await _terminal.GetAllActive();

                int successCount = 0;

                foreach(var terminal in terminals)
                {
                    logger.Activity("Shutting down Terminal: " + terminal.Terminal_Name);

                    bool result = terminalCommand.ShutdownCommand(terminal);

                    if (result)
                        successCount++;
                }

                return CreateResult("200", successCount + " out of " + terminals.Count + " has been successfully shutdown!", true);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpPost("addwhitelistitem")]
        public async Task<IActionResult> AddWhitelistItem(int terminalid, int carddetailsid)
        {
            string message = "The card has been successfully added to the terminal whitelist.";
            try
            {
                await _terminal.AddWhitelistItem(terminalid, carddetailsid, GetUserId());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message });
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpDelete("removewhitelistitem")]
        public async Task<IActionResult> RemoveWhitelistItem(int terminalid, int carddetailsid)
        {
            string message = "The card has been successfully removed in the terminal whitelist.";
            try
            {
                await _terminal.RemoveWhitelistItem(terminalid, carddetailsid, GetUserId());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = message });
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpDelete("removefromterminalwhitelist")]
        public async Task<ResultModel> RemoveFromTerminalWhitelist(int terminalid, int personid, string cardserial)
        {
            try
            {
                return await _terminalWhitelist.RemoveWhitelistItem(terminalid, personid, cardserial, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        //[CustomAuthorize]
        [HttpPost("addtoterminalwhitelist")]
        public async Task<ResultModel> AddToTerminalwhitelist(int terminalid, int personid, string cardserial)
        {
            try
            {
                return await _terminalWhitelist.AddWhitelistItem(terminalid, personid, cardserial, GetUserId());
            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

        [Authorize]
        [HttpGet("testConnection/{ipaddress}")]
        public async Task<ResultModel> TestConnection(string ipaddress)
        {
            try
            {
                Ping x = new Ping();
                PingReply reply = await x.SendPingAsync(IPAddress.Parse(ipaddress));

                if (reply.Status == IPStatus.Success)
                    return CreateResult("200", "Terminal is online!", false);

                return CreateResult("500", "Terminal is not connected!", false);
            }
            catch(Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }


        [Authorize]
        //[CustomAuthorize]
        [HttpPut("synclatestcards/{id}")]
        public async Task<ResultModel> SyncLatestCards(int id)
        {
            return await _terminal.SyncLatestCards(id, GetUserId());
        }
    }
}
