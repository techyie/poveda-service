using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyCampusV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PCSCController : ControllerBase
    {
        [HttpGet("download")]
        public async Task<IActionResult> DowloadPCSCService()
        {
            var net = new System.Net.WebClient();
            var directory = Directory.GetCurrentDirectory() + @"\Application\SetupMCCV2PCSCToolService.msi";
            var data = net.DownloadData(directory);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/octet-stream";
            var fileName = "SetupMCCV2PCSCToolService.msi";
            return File(content, contentType, fileName);
        }
    }
}
