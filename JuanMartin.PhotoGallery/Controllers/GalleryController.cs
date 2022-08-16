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
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlX.XDevAPI;

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
        public IActionResult Index(string searchQuery, int pageId = 1, int blockId = 1)
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

                GetPhotographyIdsList(sessionUserId, _photoService, searchQuery);
                ViewBag.CurrentPage = pageId;
                ViewBag.BlockId = blockId;
                ViewBag.BlockSize = PhotoService.BlockSize;
            }
            else
            {
                model = ProcessSearchQuery(pageId, blockId, searchQuery, model, sessionUserId);
                //throw new InvalidDataException($"page {pageId} of {searchQuery} with{model.PhotographyCount} images.");
            }

            TempData["isSignedIn"] = Startup.IsSignedIn;
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(int pageId, int blockId, string searchQuery)
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
                model = ProcessSearchQuery(pageId, blockId, searchQuery, model, sessionUserId);

                _photoService.AddAuditMessage(sessionUserId, $"Search for ({searchQuery}) returned {model.PhotographyCount} results.");

                TempData["isSignedIn"] = Startup.IsSignedIn;
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Detail(long id, string searchQuery,  GalleryDetailViewModel model)
        {
            string message = "";
            int pageId = (int)(id / PhotoService.PageSize) + 1;

            SetViewRedirectInfo("Gallery", "Detail", out int sessionUserId, out RedirectResponseModel redirectInfo, id);

            if (!Convert.ToBoolean(Startup.IsSignedIn))
            {
                //throw new InvalidDataException($"logged in {Convert.ToBoolean(Startup.IsSignedIn)}.");
                return RedirectToAction(controllerName: "Login", actionName: "Login", routeValues: redirectInfo?.RouteData);
            }

            if (model != null && !string.IsNullOrEmpty(model.Location))
            {
                _photoService.UpdatePhotographyDetails(id,sessionUserId,model.Location);
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

            var galleryIdList = HttpContext.Session.GetString("galleryIdList");
            ViewBag.GalleryIdList = galleryIdList;
            // perirst searchQuery for redirects
            ViewBag.SearchQuery = searchQuery;

            return View(PrepareDetailViewModel(id, searchQuery, pageId));
        }

        [HttpGet]
        public IActionResult Detail(long id, int pageId, string searchQuery)
        {
            int newPageId = (int)(id / PhotoService.PageSize) + 1;
            SetViewRedirectInfo("Gallery", "Detail", out _, out _, id);
            
            TempData["isSignedIn"] = Startup.IsSignedIn;

            var galleryIdList = HttpContext.Session.GetString("galleryIdList");
            ViewBag.GalleryIdList = galleryIdList;
            // perirst searchQuery for redirects
            ViewBag.SearchQuery = searchQuery;

            return View(PrepareDetailViewModel(id, searchQuery, newPageId));
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

        private GalleryIndexViewModel  ProcessSearchQuery(int pageId, int blockId, string searchQuery, GalleryIndexViewModel model, int sessionUserId)
        {
            string message = "";

            if (searchQuery.Contains(' '))
                message = "Search query must not contain spaces!";
            else
            {
                searchQuery = searchQuery.Replace(',', '|'); // prepare search for use as regular expression
                model.Album = (List<Photography>)_photoService.GetPhotographiesBySearch(sessionUserId, searchQuery, pageId);
            }
            ViewBag.Message = message;

            var count = GetPhotographyIdsList(sessionUserId, _photoService, searchQuery);
            searchQuery = searchQuery.Replace("|", ",");
            ViewBag.InfoMessage = $">> {count} results for images with tags  in ({searchQuery}).";
            model.PhotographyCount = count;
            ViewBag.CurrentPage = pageId;
            ViewBag.BlockId = blockId;
            ViewBag.BlockSize = PhotoService.BlockSize;

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

        private int GetPhotographyIdsList(int userID, IPhotoService photoService, string searchQuery)
        {
            if (!string.IsNullOrEmpty(searchQuery))
                searchQuery = searchQuery.Replace(',', '|'); // prepare search for use as regular expression
            else
                searchQuery = "";

            (string galleryIdList, long rowCount) = photoService.uspGetPhotographyIdsList(userID, searchQuery);

            HttpContext.Session.SetString("galleryIdList", galleryIdList);
            ViewBag.PageCount = (int)(rowCount / PhotoService.PageSize) + 1;

            return (int)rowCount;
        }

        private int SetGalleryNavigationIds(long id)
        {
            int pageId = (int)(id / PhotoService.PageSize) + 1;

            // correct border cases
            if (id % PhotoService.PageSize == 0)
                pageId--;
            
            return pageId;
        }

        private GalleryDetailViewModel PrepareDetailViewModel(long id, string searchQuery, int pageId)
        {
            var image = _photoService.GetPhotographyById(id, GalleryIndexViewModel.UserID);

            var model = new GalleryDetailViewModel
            {
                Image = image,
                Location = image.Location,
                PageId = pageId,
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