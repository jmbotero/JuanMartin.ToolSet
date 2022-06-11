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
        private readonly bool _startInGuestMode = false;

        public GalleryController(IPhotoService photoService, IConfiguration configuration)
        {
            _photoService = photoService;
            _configuration = configuration;
            
            _startInGuestMode = Convert.ToBoolean(configuration["StrartInGuestMode"]);
        }

        [HttpGet]
        public IActionResult Index(int pageId = 1)
        {
            int sessionId = -1;
            int sessionUserId = -1;
            if (Convert.ToBoolean(Startup.IsSignedIn))
            {
                sessionId = (int)HttpContext.Session.GetInt32("SessionID").Value;
                sessionUserId = (int)HttpContext.Session.GetInt32("UserID").Value;
            }
            var remoteHostName = HttpUtility.GetClientRemoteId(HttpContext);
            string queryString = HttpContext.Request.QueryString.Value;
            var redirectInfo = _photoService.SetRedirectInfo(userId: sessionUserId, remoteHost: remoteHostName, controller: "Gallery", action: "Index", queryString: queryString);

            if (!_startInGuestMode && redirectInfo != null && !Convert.ToBoolean(Startup.IsSignedIn))
                RedirectToAction(controllerName: "Login", actionName: "Login", routeValues: redirectInfo.RouteData);

            ViewBag.Suid = sessionUserId;
            ViewBag.Sid = sessionId;

            var model = new GalleryIndexViewModel
            {
                Album = (List<Photography>)_photoService.GetAllPhotographies(sessionUserId, pageId)
            };

            ViewBag.PageCount = _photoService.GetGalleryPageCount(PhotoService.PageSize);
            ViewBag.CurrentPage = pageId;

            TempData["isSignedIn"] = Startup.IsSignedIn;

            return View(model);
        }

        [HttpPost]
        public IActionResult Detail(long id,  GalleryDetailViewModel galleryDetail)
        {
            int sessionUserId = -1;
            if (Convert.ToBoolean(Startup.IsSignedIn))
                sessionUserId = (int)HttpContext.Session.GetInt32("UserID");

            if (galleryDetail != null && galleryDetail.SelectedRank != 0)
            {
                var remoteHostName = HttpUtility.GetClientRemoteId(HttpContext);
                string queryString = HttpContext.Request.QueryString.Value;

                var redirectInfo = _photoService.SetRedirectInfo(userId: sessionUserId, remoteHost: remoteHostName,controller:  "Gallery",action: "Detail", routeId: id, queryString: queryString);
                if (redirectInfo != null && !Convert.ToBoolean(Startup.IsSignedIn))
                    RedirectToAction(controllerName: "Login", actionName: "Login", routeValues: redirectInfo.RouteData);

                _photoService.UpdatePhotographyRanking(id, sessionUserId, galleryDetail.SelectedRank);
                //throw new InvalidDataException($"wrong rank: ({galleryDetail.SelectedRank}) for {id}.");
            }
            TempData["isSignedIn"] = Startup.IsSignedIn;
            
            return View(PrepareDetailViewModel(id, galleryDetail.PageId));
        }

        [HttpGet]
        public IActionResult Detail(long id, int pageId)
        {
            TempData["isSignedIn"] = Startup.IsSignedIn;

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
     }
}