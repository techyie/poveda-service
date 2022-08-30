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
using MyCampusV2.Helpers;
using Microsoft.AspNetCore.Authorization;
using MyCampusV2.Models.V2.entity;

namespace MyCampusV2.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class TerminalConfigurationController : BaseController
    {
        private readonly ITerminalConfigurationService _terminalConfiguration;
        private readonly ITerminalService _terminalService;
        private readonly IMapper _mapper;
        private Pinger pinger = new Pinger();
        private Logger log = new Logger();

        public TerminalConfigurationController(ITerminalConfigurationService terminalConfiguration, ITerminalService terminalService, IMapper mapper)
        {
            this._terminalConfiguration = terminalConfiguration;
            this._terminalService = terminalService;
            this._mapper = mapper;
        }

        [HttpGet]
        [Route("configurationDetails/{id}")]
        public async Task<terminalConfigVM> GetTerminalConfigurationDetails(int id)
        {
            return _mapper.Map<terminalConfigVM>(await _terminalConfiguration.GetTerminalConfigurationDetails(id));
        }


        [HttpPut]
        [Route("updateConfiguration")]
        public async Task<ResultModel> UpdateTerminalConfiguration([FromBody] terminalConfigVM entity)
        {
            try
            {
                terminalConfigurationEntity _terminalConfig = _mapper.Map<terminalConfigurationEntity>(entity);
                _terminalConfig.Date_Time_Added = DateTime.UtcNow.ToLocalTime();
                _terminalConfig.Last_Updated = DateTime.UtcNow.ToLocalTime();
                _terminalConfig.Added_By = GetUserId();
                _terminalConfig.Updated_By = GetUserId();

                var terminalData = await _terminalService.GetById(entity.terminalId);

                var data = await _terminalConfiguration.UpdateTerminalConfiguration(_terminalConfig);

                if (data.resultCode == "200")
                {
                    if (pinger.TerminalConfigBuilder(terminalData, _terminalConfig))
                        return data;
                    else
                        return CreateResult("500", "Failed to update the Terminal Configuration in the Device.", false);
                }
                return CreateResult("500", "Failed to update the Terminal Configuration.", false);

            }
            catch (Exception err)
            {
                return CreateResult("500", err.Message, false);
            }
        }

    }
}
