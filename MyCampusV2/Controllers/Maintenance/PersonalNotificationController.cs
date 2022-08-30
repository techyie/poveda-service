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

namespace MyCampusV2.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalNotificationController : BaseController
    {
        private readonly INotificationService _notification;
        private readonly IMapper _mapper;
        protected readonly IAuditTrailService _audit;

        public PersonalNotificationController(INotificationService notification, IMapper mapper, IAuditTrailService audit)
        {
            _notification = notification;
            _mapper = mapper;
            _audit = audit;
        }
    }
}
