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
        private readonly bool _guestModeEnabled = false;

        public GalleryController(IPhotoService photoService, IConfiguration configuration)
        {
            _photoService = photoService;
            _configuration = configuration;
            
            _guestModeEnabled = Convert.ToBoolean(configuration["GuestModeEnabled"]);
        }

        [HttpGet]
        public IActionResult Index(int pageId = 1)
        {
            int sessionId = -1;
            if (Convert.ToBoolean(Startup.IsSignedIn))
            {
                sessionId = (int)HttpContext.Session.GetInt32("SessionID").Value;
            }

            SetViewRedirectInfo("Gallery", "Index", out int sessionUserId, out RedirectResponseModel redirectInfo);

            if (!_guestModeEnabled)
            {
                if (!Convert.ToBoolean(Startup.IsSignedIn))
                    return RedirectToAction(controllerName: "Login", actionName: "Login", routeValues: redirectInfo?.RouteData);
            }

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
        public IActionResult Detail(long id, long firstId, long lastId,   GalleryDetailViewModel model)
        {
            string message = "";
            model.PageId = SetGalleryNavigationIds(id, firstId, lastId);

            SetViewRedirectInfo("Gallery", "Detail", out int sessionUserId, out RedirectResponseModel redirectInfo, id);

            //throw new InvalidDataException($"user {sessionUserId}/{Startup.IsSignedIn} {model.SelectedTagListAction} tag: ({model.Tag}) for {id}.");
            //process submitted tag
            if (model != null && !string.IsNullOrEmpty(model.Tag))
            {
                var tag = model.Tag.Trim();
                //throw new InvalidDataException($"user {sessionUserId}/{Startup.IsSignedIn} {model.SelectedTagListAction} tag: ({tag}) for {id}.");
                switch (model.SelectedTagListAction)
                {
                    case "add":
                        {
                            var s = _photoService.AddTag(tag, id);
                            switch (s)
                            {
                                case -1:
                                    message = $"Error inserting tag '{tag}' for image {id} in database.";
                                    break;
                                case -2:
                                    message = $"Tag '{tag}' is already associated to this image ({id}).";
                                    break;
                                default:
                                    model.Image.Tags.Add(tag);
                                    break;
                            }

                            break;
                        }
                    case "remove":
                        {
                            var s = _photoService.RemoveTag(tag, id);
                            if (s != -1)
                                model.Image.Tags.Remove(tag);
                            else
                                message = $"Error deleting tag '{tag}' for image {id} in database.";
                            break;
                        }
                    default:
                        break;
                }
            }
            // process submitted rank.
            if (model != null && model.SelectedRank != 0)
            {
                if (!Convert.ToBoolean(Startup.IsSignedIn))
                {
                    //throw new InvalidDataException($"logged in {Convert.ToBoolean(Startup.IsSignedIn)}.");
                    return RedirectToAction(controllerName: "Login", actionName: "Login", routeValues: redirectInfo?.RouteData);
                }
                var rank = model.SelectedRank;
                var s = _photoService.UpdatePhotographyRanking(id, sessionUserId, rank);

                if (s == -1)
                {
                    //throw new InvalidDataException($"user {sessionUserId}/{Startup.IsSignedIn} wrong rank: ({galleryDetail.SelectedRank}) for {id}.");
                    message = $"Error inserting rank={rank} for image {id} in database.";
                }
            }

            TempData["isSignedIn"] = Startup.IsSignedIn;
            ViewBag.Message = message;

            return View(PrepareDetailViewModel(id, model.PageId));
        }

        [HttpGet]
        public IActionResult Detail(long id, long firstId,  long lastId, int pageId)
        {
            SetGalleryNavigationIds(id, firstId, lastId);
            SetViewRedirectInfo("Gallery", "Detail", out _, out _, id);

            TempData["isSignedIn"] = Startup.IsSignedIn;
            return View(PrepareDetailViewModel(id, pageId));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void SetViewRedirectInfo(string controlerName, string actionName,  out int sessionUserId, out RedirectResponseModel redirectInfo, long routeId = -1)
        {
            sessionUserId = -1;
            if (Convert.ToBoolean(Startup.IsSignedIn))
                sessionUserId = (int)HttpContext.Session.GetInt32("UserID");

            var remoteHostName = HttpUtility.GetClientRemoteId(HttpContext);
            string queryString = HttpContext.Request.QueryString.Value;
            redirectInfo = _photoService.SetRedirectInfo(userId: sessionUserId, remoteHost: remoteHostName, controller: controlerName, action: actionName, routeId: routeId, queryString: queryString);
            //throw new Exception($"{actionName}: {remoteHostName},{queryString},{id}");
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
            
            int pageId = (int)(id / PhotoService.PageSize) + 1;

            return pageId;
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
                //Tags = SetTagsforDisplay(image.Tags)
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