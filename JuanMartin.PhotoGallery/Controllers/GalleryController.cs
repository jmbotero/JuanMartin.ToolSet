using JuanMartin.Models.Gallery;
using JuanMartin.PhotoGallery.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IPhotoService _photoService;
        
        const int UserID = 1;

        public GalleryController(IPhotoService photoService)
        {
            _photoService = photoService;
        }
        public IActionResult Index(int pageId = 1)
        {
            var model = new Gallery
            {
                Album = (List<Photography>)_photoService.GetAllPhotographies(UserID, pageId)

            };

            ViewBag.PageCount = 3;
            ViewBag.CurrentPage = pageId;

            return View(model);
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