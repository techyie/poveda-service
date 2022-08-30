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
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services.IServices;

namespace MyCampusV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitorController : ControllerBase
    {
        private readonly IVisitorService _visitor;
        private readonly IMapper _mapper;

        public VisitorController(IVisitorService visitor, IMapper mapper)
        {
            this._visitor = visitor;
            this._mapper = mapper;
        }

        //Get By ID
        [HttpGet("{id:int}")]
        public async Task<visitorInformationEntity> GetByID(int id)
        {
            var result = _mapper.Map<visitorInformationEntity>(await _visitor.getById(id));

            return result;
        }

        [HttpPost]
        public async Task Create(visitorInformationEntity visitor)
        {
            try {
                await _visitor.addVisitor(visitor);
            } catch (Exception e) {
            }
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] visitorInformationEntity visitor)
        {
            await _visitor.updateVisitor(visitor, id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _visitor.deleteVisitor(id);
            return Ok();
        }



    }
}