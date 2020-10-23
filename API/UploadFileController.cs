using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UploadProgress_AspnetCore.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly IHostingEnvironment _webHostEnvironment;
        public UploadFileController(IHostingEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region ValidateUpload
        private static bool IsExtensionValidVideo(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            return string.Equals(extension, ".MP4", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(extension, ".FLV", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(extension, ".AVI", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(extension, ".WebM", StringComparison.OrdinalIgnoreCase)||
                   string.Equals(extension, ".MKV", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(extension, ".WMV", StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        /// <summary>
        /// Upload video for form
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("~/api/UploadFile/UploadVideo")]
        public async Task<IActionResult> UploadVideo(IFormFile source)
        {
            int limitSizeFile = 111048576;
            if (source == null)
            {
                return BadRequest("Không có file upload");
            }
            // Check Permission
            //Claim UserId = HttpContext.User.Claims.Where(x => x.Type == MeetConstant.USERID).FirstOrDefault();
            //string userId = UserId.Value;
            //if (userId == null) return BadRequest("Bạn không có quyền upload File");
            try
            {
                string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');
                if (filename.Contains("\\"))
                {
                    filename = filename.Substring(filename.LastIndexOf("\\") + 1);
                }
                // Check Format File
                if (IsExtensionValidVideo(filename))
                {
                    // Check Size File
                   if (source.Length > limitSizeFile)
                      return BadRequest("Dung lượng file vượt quá giới hạn");

                    string fullPathFolder = _webHostEnvironment.WebRootPath + "//uploads//video//";
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPathFolder));
                    string fileNamePlus = "VideoRecord_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(filename);
                    string path = fullPathFolder + fileNamePlus;
                    using (FileStream output = System.IO.File.Create(path))
                    {
                        await source.CopyToAsync(output);
                    }
                    return Ok("/uploads/video/" + fileNamePlus);
                }
                else
                {
                    return BadRequest("File type support .MP4 .FLV .AVI .Webm .MKV .WMV");
                }
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }


    }
}