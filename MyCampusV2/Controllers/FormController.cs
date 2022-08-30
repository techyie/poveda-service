using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.V2.entity;
using MyCampusV2.Services;

namespace MyCampusV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormController : ControllerBase
    {
        private readonly IFormService _form;
        private readonly IMapper _mapper;

        public FormController(IFormService _form, IMapper mapper)
        {
            this._form = _form;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ICollection<formVM>> Get()
        {
            try
            {
                return _mapper.Map<ICollection<formVM>>(await _form.GetAllForm());
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [Authorize]
        [HttpGet("checkchanges")]
        public bool CheckChanges()
        {
            var currentUser = HttpContext.User;
            int userID = 0;

            if (currentUser.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                userID = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            }

            if(userID == 1)
                return true;

            if (_form.CheckForm(userID) == true)
                return true;

            return false;
        }

        [Authorize]
        [HttpGet("get")]
        public ICollection<formEntity> GetUserForm()
        {
            var currentUser = HttpContext.User;
            int userID = 0;

            if (currentUser.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                userID = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            }
            return _form.GetUserForm(userID);
        }
    }
}