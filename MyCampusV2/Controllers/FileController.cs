using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCampusV2.Common;
using MyCampusV2.Models;
using MyCampusV2.Services;

namespace MyCampusV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public FileController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // GET api/files/sample.png
        [HttpGet("photo/{fileName}")]
        public IActionResult Get(string fileName)
        {
            try
            {
                string path = PhotoStatSettings.photopath + "/" + fileName + "_Photo.jpg";
                byte[] b = System.IO.File.ReadAllBytes(path);
                return Ok("data:image/jpeg;base64," + Convert.ToBase64String(b));
            }
            catch (Exception ex)
            {
                string noimagepath = PhotoStatSettings.photopath + @"\noimage.jpg";
                byte[] b = System.IO.File.ReadAllBytes(noimagepath);
                return Ok("data:image/jpeg;base64," + Convert.ToBase64String(b));
            }
        }

        [HttpGet("signature/{fileName}")]
        public IActionResult GetSignature(string fileName)
        {
            try
            {
                string path = PhotoStatSettings.photopath + "/" + fileName + "_Signature.jpg";
                byte[] b = System.IO.File.ReadAllBytes(path);
                return Ok("data:image/jpeg;base64," + Convert.ToBase64String(b));
            }
            catch (Exception ex)
            {
                string nosignaturepath = PhotoStatSettings.photopath + @"\nosignature.jpg";
                byte[] b = System.IO.File.ReadAllBytes(nosignaturepath);
                return Ok("data:image/jpeg;base64," + Convert.ToBase64String(b));
            }
        }

        [HttpGet("downloadimage/{idnumber}")]
        public async Task<IActionResult> DownloadImage(string idnumber)
        {
            try
            {
                if (System.IO.File.Exists(PhotoStatSettings.photopath + "\\" + idnumber + ".jpg"))
                    return File(await System.IO.File.ReadAllBytesAsync(PhotoStatSettings.photopath + "\\" + idnumber + ".jpg"), "image/jpeg", "TRYIFILE.jpg");
                else
                    return BadRequest(new { Error = false, message = "No image available." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("downloadimportedlogs")]
        public async Task<IActionResult> DownloadImportLogs([FromQuery] string mod, [FromQuery] string fileName)
        {
            try
            {
                return File(await System.IO.File.ReadAllBytesAsync(AppDomain.CurrentDomain.BaseDirectory + mod + @"\" + fileName), "text/plain");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
