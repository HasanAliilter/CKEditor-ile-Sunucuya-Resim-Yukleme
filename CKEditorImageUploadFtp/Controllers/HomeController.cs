using CKEditorImageUploadFtp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace CKEditorImageUploadFtp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        public HomeController(ILogger<HomeController> logger, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> UploadCKEDITOR(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetFileName(upload.FileName);
                var ftpAddress = "";
                var ftpUserName = "";
                var ftpPassword = "";
                var ftpFilePath = ftpAddress + fileName;

                try
                {
                    // FTP baðlantýsý ve dosya yükleme
                    using (var client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                        using (var fileStream = new MemoryStream())
                        {
                            await upload.CopyToAsync(fileStream);
                            fileStream.Position = 0;
                            client.UploadData(ftpFilePath, fileStream.ToArray());
                        }
                    }

                    var url = $"{ftpAddress}{fileName}";
                    return Json(new { uploaded = true, url });
                }
                catch (Exception ex)
                {
                    // Hata yönetimi
                    return Json(new { uploaded = false, url = "", message = ex.Message });
                }
            }
            return Json(new { uploaded = false, url = "" });
        }


        public async Task<IActionResult> FileBrowserCKEDITOR()
        {
            var ftpAddress = "";
            var ftpUserName = "";
            var ftpPassword = "";

            List<string> fileList = new List<string>();

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpAddress);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string line = reader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    fileList.Add(line);
                    line = reader.ReadLine();
                }
            }

            ViewBag.fileList = fileList;
            return View("FileBrowserCKEDITOR");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
