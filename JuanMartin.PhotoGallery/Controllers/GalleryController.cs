using JuanMartin.Models.Gallery;
using JuanMartin.PhotoGallery.Models;
using JuanMartin.PhotoGallery.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IPhotoService _photoService;
        
        public GalleryController(IPhotoService photoService)
        {
            _photoService = photoService;
        }
        public IActionResult Index(int pageId = 1)
        {
            var model = new GalleryIndexViewModel
            {
                Album = (List<Photography>)_photoService.GetAllPhotographies(GalleryIndexViewModel.UserID, pageId)

            };

            ViewBag.PageCount = _photoService.GetGalleryPageCount(PhotoService.PageSize);
            ViewBag.CurrentPage = pageId;

            return View(model);
        }

        public IActionResult Detail(long id, int pageId)
        {
            var image = _photoService.GetPhotographyById(id,GalleryIndexViewModel.UserID);

            var model = new GalleryDetailViewModel
            {
                Image = image,
                PageId = pageId
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        private static string GetProjectDirectory(Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary debugData = null)
        {
            string folder = Directory.GetCurrentDirectory();
            if (debugData != null) debugData["folder"] = folder;

            for (int s = 0; s < 2; s++)
            {
                folder = folder.Substring(0, folder.LastIndexOf(@"\"));
            }
            if (debugData != null) debugData["path"] = folder;

            return folder;
        }
    }
}