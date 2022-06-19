using JuanMartin.Models.Gallery;
using JuanMartin.PhotoGallery.Models;
using JuanMartin.PhotoGallery.Services;
using JuanMartin.Kernel.Extesions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            GetPhotographyIdBounds(_photoService);

            TempData["isSignedIn"] = Startup.IsSignedIn;

            return View(model);
        }

        [HttpPost]
        public IActionResult Detail(long id, long firstId, long lastId,   GalleryDetailViewModel galleryDetail)
        {
            string message = "";
            galleryDetail.PageId = SetGalleryNavigationIds(id, firstId, lastId);
            
            int sessionUserId = -1;
            if (Convert.ToBoolean(Startup.IsSignedIn))
                sessionUserId = (int)HttpContext.Session.GetInt32("UserID");

            //process submitted tag
            if (galleryDetail != null && !string.IsNullOrEmpty(galleryDetail.Tag))
            {
                var tag = galleryDetail.Tag.Trim();
                switch (galleryDetail.SelectedTagListAction)
                {
                    case "add":
                        {
                            //throw new Exception($"add ({tag}) for {id}.");
                            var s = _photoService.AddTag(tag, id);
                            switch (s)
                            {
                                case -1:
                                    message = $"Error inserting '{tag}' for {id} in database.";
                                    break;
                                case -2:
                                    message = $"Tag '{tag}' is already associated to this image ({id}).";
                                    break;
                                default:
                                    galleryDetail.Image.Tags.Add(tag);
                                    break;
                            }

                            break;
                        }
                    case "remove":
                        {
                            var s = _photoService.RemoveTag(tag, id);
                            if (s != -1)
                                galleryDetail.Image.Tags.Remove(tag);
                             else
                                message = $"Error deleting '{tag}' for {id} in database.";
                            break;
                        }
                    default:
                        break;
                }
            }
            // process submitted rank.
            //                if (galleryDetail.SelectedTagListAction == "none")
            if (galleryDetail != null && galleryDetail.SelectedRank != 0)
            {
                var remoteHostName = HttpUtility.GetClientRemoteId(HttpContext);
                string queryString = HttpContext.Request.QueryString.Value;
               
                var redirectInfo = _photoService.SetRedirectInfo(userId: sessionUserId, remoteHost: remoteHostName,controller:  "Gallery",action: "Detail", routeId: id, queryString: queryString);
                if (redirectInfo != null && !Convert.ToBoolean(Startup.IsSignedIn))
                    RedirectToAction(controllerName: "Login", actionName: "Login", routeValues: redirectInfo.RouteData);

                var rank = galleryDetail.SelectedRank;
                    //throw new InvalidDataException($"wrong rank: ({rank}) for {id}.");
                var s = _photoService.UpdatePhotographyRanking(id, sessionUserId, rank);

                if (s == -1)
                    message = $"Error inserting '{rank}' for {id} in database.";
            }
            TempData["isSignedIn"] = Startup.IsSignedIn;
            ViewBag.Message = message;

            return View(PrepareDetailViewModel(id, galleryDetail.PageId));
        }

        [HttpGet]
        public IActionResult Detail(long id, long firstId,  long lastId, int pageId)
        {
            pageId = SetGalleryNavigationIds(id, firstId, lastId);
            TempData["isSignedIn"] = Startup.IsSignedIn;
            return View(PrepareDetailViewModel(id, pageId));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void GetPhotographyIdBounds(IPhotoService photoService)
        {
            (long lower, long upper) = photoService.GetPhotographyIdBounds();

            ViewBag.FirstId = lower;
            ViewBag.LastId = upper;

        }

        private int SetGalleryNavigationIds(long id, long firstId, long lastId)
        {
            ViewBag.FirstId = firstId;
            ViewBag.LastId = lastId;
            ViewBag.NextId = (id == lastId) ? -1 : id + 1;
            ViewBag.PrevId = (id == firstId) ? -1 : id - 1;

            return (int)(id / PhotoService.PageSize) + 1; //pageId
        }


        private static List<SelectListItem> SetTagsforDisplay(List<string> tags)
        {
            List<SelectListItem> items = new();
            foreach (var tag in tags)
                items.Add(new SelectListItem { Text = tag, Selected = false });

            return items;
        }

        private GalleryDetailViewModel PrepareDetailViewModel(long id, int pageId)
        {
            var image = _photoService.GetPhotographyById(id, GalleryIndexViewModel.UserID);

            var model = new GalleryDetailViewModel
            {
                Image = image,
                PageId = pageId,
                Tags = SetTagsforDisplay(image.Tags)
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