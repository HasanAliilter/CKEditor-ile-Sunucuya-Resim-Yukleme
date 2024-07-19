using CKEditorImageUploadFtp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
                var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploaded");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(fileStream);
                }

                var url = $"/uploaded/{fileName}";
                return Json(new { uploaded = true, url });
            }
            return Json(new { uploaded = false, url = "" });
        }


        public async Task<IActionResult> FileBrowserCKEDITOR(IFormFile upload)
        {
            var dir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), _hostingEnvironment.WebRootPath, "uploaded"));
            ViewBag.fileInfos = dir.GetFiles();
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
