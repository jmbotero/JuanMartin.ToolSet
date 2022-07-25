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
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Diagnostics;

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
        public IActionResult Index(string searchQuery, int pageId = 1)
        {
            if (string.IsNullOrEmpty(searchQuery))  // case asp.net mvc name matching does not  work use request querystring
                searchQuery = HttpContext.Request.Query["searchQuery"].ToString();

            if (HttpContext.Session is null)
                Startup.IsSignedIn = "false";

            int sessionId = -1;
            if (Convert.ToBoolean(Startup.IsSignedIn))
            {
                try
                {
                    sessionId = (int)HttpContext.Session.GetInt32("SessionID").Value;
                }
                catch
                {
                    Startup.IsSignedIn = "false";
                    sessionId = -1;
                }
            }
            SetViewRedirectInfo("Gallery", "Index", out int sessionUserId, out RedirectResponseModel redirectInfo);

            if (!_guestModeEnabled)
            {
                if (!Convert.ToBoolean(Startup.IsSignedIn))
                    return RedirectToAction(controllerName: "Login", actionName: "Login", routeValues: redirectInfo?.RouteData);
            }

            ViewBag.Suid = sessionUserId;
            ViewBag.Sid = sessionId;

           var model = new GalleryIndexViewModel();

            //if(pageId > 1)
            //    throw new InvalidDataException($"search: '{searchQuery}'.");
    
            if (string.IsNullOrEmpty(searchQuery))
            {
                model = new GalleryIndexViewModel
                {
                    Album = (List<Photography>)_photoService.GetAllPhotographies(sessionUserId, pageId)
                };

                ViewBag.CurrentPage = pageId;
                var count = GetPhotographyIdBounds(_photoService, searchQuery);
                ViewBag.PageCount = (int)(count / PhotoService.PageSize) + 1;
            }
            else
            {
                model = ProcessSearchQuery(pageId, searchQuery, model, sessionUserId);
                //throw new InvalidDataException($"page {pageId} of {searchQuery} with{model.PhotographyCount} images.");
            }

            TempData["isSignedIn"] = Startup.IsSignedIn;
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(int pageId, string searchQuery)
        {
            int sessionUserId = -1;
            if (Convert.ToBoolean(Startup.IsSignedIn))
            {
                sessionUserId = (int)HttpContext.Session.GetInt32("UserID");
            }

            var model = new GalleryIndexViewModel();

            if (string.IsNullOrEmpty(searchQuery))
            {
                return ViewGalleryIndex(sessionUserId);
            }
            else
            {
                model = ProcessSearchQuery(pageId, searchQuery, model, sessionUserId);
                _photoService.AddAuditMessage(sessionUserId, $"Search for ({searchQuery}) returned {model.PhotographyCount} results.");

                TempData["isSignedIn"] = Startup.IsSignedIn;
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Detail(long id, long firstId, long lastId, string searchQuery,  GalleryDetailViewModel model)
        {
            string message = "";
            int newPageId = SetGalleryNavigationIds(id, firstId, lastId, searchQuery);

            SetViewRedirectInfo("Gallery", "Detail", out int sessionUserId, out RedirectResponseModel redirectInfo, id);

            if (!Convert.ToBoolean(Startup.IsSignedIn))
            {
                //throw new InvalidDataException($"logged in {Convert.ToBoolean(Startup.IsSignedIn)}.");
                return RedirectToAction(controllerName: "Login", actionName: "Login", routeValues: redirectInfo?.RouteData);
            }

            if (model != null && !string.IsNullOrEmpty(model.Tag))
            {
                message = ProcessSubmittedTag(id, model, message, sessionUserId);
            }
            if (model != null && model.SelectedRank != 0)
            {
                message = ProcessSubmittedRank(id, model, message, sessionUserId);
            }

            TempData["isSignedIn"] = Startup.IsSignedIn;
            ViewBag.Message = message;

            // perirst firstId, LastId, searchQuery for redirects
            ViewBag.FirstId = firstId;
            ViewBag.LastId = lastId;
            ViewBag.SearchQuery = searchQuery;

            return View(PrepareDetailViewModel(id, firstId, lastId,searchQuery, newPageId));
        }

        [HttpGet]
        public IActionResult Detail(long id, long firstId,  long lastId, int pageId, string searchQuery)
        {
            int newPageId  = SetGalleryNavigationIds(id, firstId, lastId, searchQuery);
            SetViewRedirectInfo("Gallery", "Detail", out _, out _, id);

            TempData["isSignedIn"] = Startup.IsSignedIn;

            // perirst firstId, LastId, searchQuery for redirects
            ViewBag.FirstId = firstId;
            ViewBag.LastId = lastId;
            ViewBag.SearchQuery = searchQuery;

            return View(PrepareDetailViewModel(id, firstId, lastId, searchQuery, newPageId));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            int sessionUserId = -1;
            if (Convert.ToBoolean(Startup.IsSignedIn))
            {
                sessionUserId = (int)HttpContext.Session.GetInt32("UserID");
            }
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionDetails is not null)
            {
                var message = exceptionDetails.Error.Message;
                var source = exceptionDetails.Path;

                _photoService.AddAuditMessage(sessionUserId, message, source, 1);

                ViewBag.ErrorMessage = $"Path {source} threw an exception: '{message}'.";
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private GalleryIndexViewModel  ProcessSearchQuery(int pageId, string searchQuery, GalleryIndexViewModel model, int sessionUserId)
        {
            string message = "";

            if (searchQuery.Contains(' '))
                message = "Search query must not contain spaces!";
            else
            {
                searchQuery = searchQuery.Replace(',', '|'); // prepare search for use as regular expression
                model.Album = (List<Photography>)_photoService.GetPhotographiesByTags(sessionUserId, searchQuery, pageId);
            }
            ViewBag.Message = message;

            var count = GetPhotographyIdBounds(_photoService, searchQuery);
            searchQuery = searchQuery.Replace("|", ",");
            ViewBag.InfoMessage = $">> {count} results for images with tags  in ({searchQuery}).";
            model.PhotographyCount = count;
            ViewBag.PageCount = (int)(count / PhotoService.PageSize) + 1;
            ViewBag.CurrentPage = pageId;

            // perirst searchQuery for redirects
            ViewBag.SearchQuery = searchQuery;

            return model;
        }

        private string ProcessSubmittedRank(long id, GalleryDetailViewModel model, string message, int sessionUserId)
        {
            var rank = model.SelectedRank;
            var s = _photoService.UpdatePhotographyRanking(id, sessionUserId, rank);

            if (s == -1)
            {
                //throw new InvalidDataException($"user {sessionUserId}/{Startup.IsSignedIn} wrong rank: ({galleryDetail.SelectedRank}) for {id}.");
                message = $"Error inserting rank={rank} for image {id} in database.";
            }

            return message;
        }

        private string ProcessSubmittedTag(long id, GalleryDetailViewModel model, string message, int sessionUserId)
        {
            var tag = model.Tag.Trim();
            switch (model.SelectedTagListAction)
            {
                case "add":
                    {
                        //throw new InvalidDataException($"user {sessionUserId}/{Startup.IsSignedIn} {model.SelectedTagListAction} tag: ({tag}) for {id}.");
                        var s = _photoService.AddTag(sessionUserId, tag, id);
                        switch (s)
                        {
                            case -1:
                                message = $"Could not insert tag '{tag}' for image {id}, image may have been deleted.";
                                break;
                            case -2:
                                message = $"Tag '{tag}' is already associated to this image ({id}).";
                                break;
                            default:
                                model.Image = _photoService.GetPhotographyById(id, sessionUserId);
                                model.Image.Tags.Add(tag);
                                break;
                        }

                        break;
                    }
                case "remove":
                    {
                        var s = _photoService.RemoveTag(sessionUserId, tag, id);
                        if (s != -1)
                            model.Image.Tags.Remove(tag);
                        else
                            message = $"Error deleting tag '{tag}' for image {id} in database.";
                        break;
                    }
                default:
                    break;
            }

            return message;
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

        private long GetPhotographyIdBounds(IPhotoService photoService, string searchQuery)
        {
            if (!string.IsNullOrEmpty(searchQuery))
                searchQuery = searchQuery.Replace(',', '|'); // prepare search for use as regular expression
            else
                searchQuery = "";

            (long lower, long upper,long rowCount) = photoService.GetPhotographyIdBounds(searchQuery);

            ViewBag.FirstId = lower;
            ViewBag.LastId = upper;
            
            return rowCount;
        }

        private int SetGalleryNavigationIds(long id, long firstId, long lastId, string searchQuery)
        {
            ViewBag.FirstId = firstId;
            ViewBag.LastId = lastId;
            if (!string.IsNullOrEmpty(searchQuery))
            {
                ViewBag.NextId = -1;
                ViewBag.PrevId = -1;
            }
            else
            {
                ViewBag.NextId = (id == lastId) ? -1 : id + 1;
                ViewBag.PrevId = (id == 1 || id == firstId) ? -1 : id - 1;
            }
            int pageId = (int)(id / PhotoService.PageSize) + 1;

            // correct border cases
            if (id % PhotoService.PageSize == 0)
                pageId--;

            return pageId;
        }


        private static List<SelectListItem> SetTagsforDisplay(List<string> tags)
        {
            List<SelectListItem> items = new();
            foreach (var tag in tags)
                items.Add(new SelectListItem { Text = tag, Selected = false });

            return items;
        }

        private GalleryDetailViewModel PrepareDetailViewModel(long id, long firstId, long lastId, string searchQuery, int pageId)
        {
            var image = _photoService.GetPhotographyById(id, GalleryIndexViewModel.UserID);

            var model = new GalleryDetailViewModel
            {
                Image = image,
                PageId = pageId,
                FistId = firstId,
                LastId=lastId,
                SearchQuery=searchQuery
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

        /// <summary>
        /// Duplicate in login controller!
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private ActionResult ViewGalleryIndex(int userId)
        {
            var remoteHostName = HttpUtility.GetClientRemoteId(HttpContext);

            var redirect = _photoService.GetRedirectInfo(userId, remoteHostName);
            TempData["isSignedIn"] = Startup.IsSignedIn;
            if (redirect != null && redirect.RemoteHost != "")
                return RedirectToAction(controllerName: redirect.Controller, actionName: redirect.Action, routeValues: new RouteValueDictionary(redirect.RouteData));
            else
                return RedirectToAction(controllerName: "Gallery", actionName: "Index");
        }
    }
}