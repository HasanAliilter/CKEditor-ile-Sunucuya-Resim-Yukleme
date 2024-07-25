using CKEditorImageUploadFtp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace CKEditorImageUploadFtp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
                var ftpAddress = "ftp sunucu adresi";
                var ftpUserName = "Kullanýcý adýnýz";
                var ftpPassword = "Þifreniz";
                var ftpToLocalFileAddress = "http://localhost:8080/images/";
                var ftpFilePath = ftpAddress + fileName;

                try
                {
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
                    return Json(new { uploaded = false, url = "", message = ex.Message });
                }
            }
            return Json(new { uploaded = false, url = "" });
        }


        public async Task<IActionResult> FileBrowserCKEDITOR()
        {
            var ftpAddress = "ftp sunucu adresi";
            var ftpUserName = "Kullanýcý adýnýz";
            var ftpPassword = "Þifreniz";

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
