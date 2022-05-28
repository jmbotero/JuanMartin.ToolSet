using JuanMartin.Models.Gallery;
using JuanMartin.PhotoGallery.Models;
using JuanMartin.PhotoGallery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using System;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IPhotoService _photoService;
        private readonly IConfiguration _configuration;
        private readonly bool _strartInGuestMode = false;

        public GalleryController(IPhotoService photoService, IConfiguration configuration)
        {
            _photoService = photoService;
            _configuration = configuration;

            _strartInGuestMode = Convert.ToBoolean(configuration["StrartInGuestMode"]);
        }
        public IActionResult Index(int pageId = 1)
        {
            int sessionUserId = -1;

            TempData["clientId"] = GetClientRemoteId();
            var clientId = TempData["clientId"].ToString();
            string queryString = HttpContext.Request.QueryString.Value;

            _photoService.SetRedirectInfo(remoteHost: clientId, controller:  "Gallery",action:  "Index", routeData: queryString);
            
            if (!_strartInGuestMode)
            {
                if(!HttpContext.Session.IsAvailable)
                    RedirectToAction(controllerName: "Login", actionName: "Login");
                else
                    sessionUserId = HttpContext.Session.GetInt32("UserID").Value;
            }

            var model = new GalleryIndexViewModel
            {
                Album = (List<Photography>)_photoService.GetAllPhotographies(sessionUserId, pageId)
            };

            ViewBag.PageCount = _photoService.GetGalleryPageCount(PhotoService.PageSize);
            ViewBag.CurrentPage = pageId;

            return View(model);
        }

        [HttpPost]
        public IActionResult Detail(long id,  GalleryDetailViewModel galleryDetail)
        {
            int sessionUserId = -1;
            
            if (galleryDetail != null && galleryDetail.SelectedRank != 0)
            {
                var clientId = GetClientRemoteId();
                string queryString = HttpContext.Request.QueryString.Value;

                _photoService.SetRedirectInfo(remoteHost: clientId,controller:  "Gallery",action: "Detail", model: galleryDetail, routeData: queryString);
                if (!_strartInGuestMode)
                {
                    if (!HttpContext.Session.IsAvailable)
                        RedirectToAction(controllerName: "Login", actionName: "Login");
                    else
                        sessionUserId = HttpContext.Session.GetInt32("UserID").Value;
                }
                _photoService.UpdatePhotographyRanking(id, sessionUserId, galleryDetail.SelectedRank);
                //throw new InvalidDataException($"wrong rank: ({galleryDetail.SelectedRank}) for {id}.");
            }
            return View(PrepareDetailViewModel(id, galleryDetail.PageId));
        }

        [HttpGet]
        public IActionResult Detail(long id, int pageId)
        {
            return View(PrepareDetailViewModel(id,pageId));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private GalleryDetailViewModel PrepareDetailViewModel(long id, int pageId)
        {
            var image = _photoService.GetPhotographyById(id, GalleryIndexViewModel.UserID);

            var model = new GalleryDetailViewModel
            {
                Image = image,
                PageId = pageId
            };

            return model;
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
        private string GetClientRemoteId()
        {
            string id = HttpContext.GetServerVariable("REMOTE_HOST");

            if (string.IsNullOrEmpty(id))
                id = HttpContext.GetServerVariable("REMOTE_ADDR");
            if (string.IsNullOrEmpty(id))
                id = HttpContext.GetServerVariable("REMOTE_USER");

            return id;
        }
    }
}