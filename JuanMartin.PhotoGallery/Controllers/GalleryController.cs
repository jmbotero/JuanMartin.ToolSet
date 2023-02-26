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
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Diagnostics;
using System.Linq;
using static JuanMartin.PhotoGallery.Services.IPhotoService;
using Order = JuanMartin.Models.Gallery.Order;
using JuanMartin.Kernel.Extesions;
using System.Security.Cryptography;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IPhotoService _photoService;
        private readonly IConfiguration _configuration;
        private readonly bool _guestModeEnabled = false;
        
        private  enum SelectedItemAction
        {
            none = 0,
            update = 1,
            cancel = 2
        };
        private const string NoActionSelected = "none";

        public GalleryController(IPhotoService photoService, IConfiguration configuration)
        {
            _photoService = photoService;
            _configuration = configuration;
            
            _guestModeEnabled = Convert.ToBoolean(configuration["GuestModeEnabled"]);
        }

        [HttpGet]
        public IActionResult Index(string searchQuery, int pageId = 1, int blockId = 1, bool cartView = false, string orderAction = NoActionSelected)
        {
            string message = "";

            if (string.IsNullOrEmpty(searchQuery))  // case asp.net mvc name matching does not  work use request querystring
                searchQuery = HttpContext.Request.Query["searchQuery"].ToString();
            
            if (HttpContext.Session is null)
                Startup.IsSignedIn = false;

            int sessionId = GetCurrentSessionId();
            SetViewRedirectInfo("Gallery", "Index", out int sessionUserId, out RedirectResponseModel redirectInfo);

            if (!_guestModeEnabled)
            {
                if (!Startup.IsSignedIn)
                    return RedirectToAction(controllerName: "Login", actionName: "Login", routeValues: redirectInfo?.RouteData);
            }

            //message = $" User = ({sessionUserId}),  Session = ({sessionId})";

           var model = new GalleryIndexViewModel();
            // set iformation for toolbar
            (_, Order order) = SetLayoutViewHeaderInformation(userId: sessionUserId, cartView: cartView);

            //if(pageId > 1)
            //    throw new InvalidDataException($"search: '{searchQuery}'.");
            if (orderAction == SelectedItemAction.cancel.ToString())
            {
                message = RemoveCurrentOrder(sessionUserId);
            }


            if (string.IsNullOrEmpty(searchQuery))
            {
                model = new GalleryIndexViewModel
                {
                    Album = (List<Photography>)_photoService.GetAllPhotographies(sessionUserId, pageId)
                };

                var orderId = (!cartView && string.IsNullOrEmpty(searchQuery)) ? -1 : order.OrderId;
                GetPhotographyIdsList(sessionUserId, _photoService, searchQuery, orderId);
                ViewBag.CurrentPage = pageId;
                ViewBag.BlockId = blockId;
                ViewBag.BlockSize = _photoService.BlockSize;
            }
            else
            {
                model = ProcessSearchQuery(pageId, blockId, searchQuery, model, sessionUserId);
                //throw new InvalidDataException($"page {pageId} of {searchQuery} with{model.PhotographyCount} images.");
            }

            model.Tags = _photoService.GetAllTags(pageId).OrderBy(x => x).ToList();
            model.Locations = _photoService.GetAllLocations(pageId).OrderBy(x => x).ToList();

            ViewBag.Message = message;
            TempData["isSignedIn"] = Startup.IsSignedIn;
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(int pageId, int blockId, string searchQuery,  GalleryIndexViewModel model,  bool cartView = false)
        {
            string message = "";

            // set iformation for toolbar
            (int sessionUserId, Order order) = SetLayoutViewHeaderInformation(cartView: cartView);
            
            if (string.IsNullOrEmpty(searchQuery) && !cartView)
            {
                return RedirectViewGalleryIndex(sessionUserId);
            }
            else
            {
                if (cartView)
                {
                    if (model.ShoppingCartAction == SelectedItemAction.update.ToString())
                        if (!_photoService.UpdateOrderItemsIndices(sessionUserId, order.OrderId, model))
                            message = "No re-orderind detected.";

                    model = DisplayPhotographyOrders(order, pageId, blockId, model, sessionUserId);
                }
                else
                {
                    model = ProcessSearchQuery(pageId, blockId, searchQuery, model, sessionUserId);
                    _photoService.AddAuditMessage(sessionUserId, $"Search for ({searchQuery}) returned {model.PhotographyCount} results.");
                }
                model.Tags = _photoService.GetAllTags(pageId).OrderBy(x=>x).ToList();
                model.Locations = _photoService.GetAllLocations(pageId).OrderBy(x => x).ToList();
            }

            ViewBag.Message = message;
            TempData["isSignedIn"] = Startup.IsSignedIn;
            return View(model);
        }

        [HttpPost]
        public IActionResult Detail(long id, int pageId, int blockId, string searchQuery,  GalleryDetailViewModel model, bool cartView = false, string orderAction = NoActionSelected)
        {
            string message = "";

            //int pageId = (int)(id / _photoService.PageSize) + 1;

            SetViewRedirectInfo("Gallery", "Detail", out int sessionUserId, out RedirectResponseModel redirectInfo, id);

            if (!Startup.IsSignedIn)
            {
                //throw new InvalidDataException($"logged in {Convert.ToBoolean(Startup.IsSignedIn)}.");
                return RedirectToAction(controllerName: "Login", actionName: "Login", routeValues: redirectInfo?.RouteData);
            }

            if (orderAction == SelectedItemAction.cancel.ToString())
            {
                message = RemoveCurrentOrder(sessionUserId);
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
            if (model != null && model.ShoppingCartAction != null && model.ShoppingCartAction != NoActionSelected)
            {
                message = ProcessShoppingCartAction(id, model, message, sessionUserId);
            }

            // set iformation for toolbar
            (_, _) = SetLayoutViewHeaderInformation(userId: sessionUserId, cartView: cartView, photographyId: id);

            TempData["isSignedIn"] = Startup.IsSignedIn;
            ViewBag.Message = message;

            var galleryIdList = HttpContext.Session.GetString("galleryIdList");
            ViewBag.GalleryIdList = galleryIdList;
            // perirst searchQuery for redirects and blockId
            ViewBag.SearchQuery = searchQuery;
            ViewBag.BlockId = blockId;

            ViewBag.IsPhotographyInOrder = IsPhotographyInOrder(id, sessionUserId);

            return View(PrepareDetailViewModel(id, searchQuery, pageId));
        }

        [HttpGet]
        public IActionResult Detail(long id, int pageId, int blockId, string searchQuery, bool cartView = false)
        {
            //int newPageId = (int)(id / _photoService.PageSize) + 1;
            SetViewRedirectInfo("Gallery", "Detail", out _, out _, id);
            
            TempData["isSignedIn"] = Startup.IsSignedIn;

            var galleryIdList = HttpContext.Session.GetString("galleryIdList");
            ViewBag.GalleryIdList = galleryIdList;
            // perirst searchQuery and blockId for redirects
            ViewBag.SearchQuery = searchQuery;
            ViewBag.BlockId = blockId;

            // set iformation for toolbar
            (_, _) = SetLayoutViewHeaderInformation(userId: -1, cartView: cartView, photographyId: id);

            return View(PrepareDetailViewModel(id, searchQuery, pageId));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            int sessionUserId = -1;
            if (Startup.IsSignedIn)
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

        private string RemoveCurrentOrder(int userId)
        {
            string message = "";
            Order order = GetCurrentActiveOrder(userId);

            string label1 = order.Number.ToString();
            int label2 = order.Count;
            if (_photoService.RemoveOrder(order.OrderId, userId) > 0)
            {
                HttpContext.Session.SetInt32("OrderID", -1);
                message = $"Order {label1} with {label2} photographs, has been removed.";
            }

            return message;
        }

        private (int UserId, Order Order) SetLayoutViewHeaderInformation(int userId = -1, long photographyId=-1, bool cartView = false)
        {
            ViewBag.Version = Startup.Version;
            (var isMobile, _) = HttpUtility.IsMobileDevice(HttpContext);
            ViewBag.IsMobile = isMobile;
            Startup.IsMobile = isMobile;

            Order order;
            int uid = (int)((userId == -1) ? GetCurrentUserId() : userId);
            int orderId = GetCurrentSessionOrderId();

            if (orderId == -1)
            {
                order = GetCurrentActiveOrder(uid);
                orderId = order.OrderId;
            }
            else
                order = _photoService.GetOrder(uid, orderId);

            ViewBag.CartRedirectUrl = GetRedirectUrl(HttpUtility.GalleryViewTypes.Index, uid);
            ViewBag.DisplayPhotogrphiesAsOrder = cartView;
            ViewBag.PhotographyCount = (order != null) ? order.Count : 0;
            ViewBag.HasCurrentActiveOrder = orderId != -1;
            ViewBag.IsPhotographyInOrder = IsPhotographyInOrder(photographyId, uid);

            return (uid, order);
        }

        private int GetCurrentSessionId()
        {
            int sessionId = -1;
            if (Startup.IsSignedIn)
            {
                try
                {
                    sessionId = (int)HttpContext.Session.GetInt32("SessionID").Value;
                }
                catch
                {
                    Startup.IsSignedIn = false;
                    sessionId = -1;
                }
            }

            return sessionId;
        }
        private int GetCurrentUserId()
        {
            int sessionUserId = -1;
            if (Startup.IsSignedIn)
            {
                try
                {
                    sessionUserId = (int)HttpContext.Session.GetInt32("UserID");
                }
                catch
                {
                    sessionUserId = -1;
                }
            }

            return sessionUserId;
        }

        private int GetCurrentSessionOrderId()
        {
            int sessionOrderId = -1;
            if (Startup.IsSignedIn)
            {
                try
                {
                    sessionOrderId = (int)HttpContext.Session.GetInt32("OrderID");
                }
                catch
                {
                    sessionOrderId = -1;
                }
            }

            return sessionOrderId;
        }

        private bool IsPhotographyInOrder(long id, int userId)
        {
            if(id ==  -1 || userId==-1)
                return false;

            var order = _photoService.GetCurrentActiveOrder(userId);
            int orderId = (order != null) ? order.OrderId : -1;

            return _photoService.IsPhotographyInOrder(orderId, id, userId);
        }

        private GalleryIndexViewModel ProcessSearchQuery(int pageId, int blockId, string searchQuery, GalleryIndexViewModel model, int sessionUserId)
        {
            searchQuery = searchQuery.Replace(',', '|'); // prepare search for use as regular expression
            model.Album = (List<Photography>)_photoService.GetPhotographiesBySearch(sessionUserId, searchQuery, pageId);
            var orderId = -1;
            var count = GetPhotographyIdsList(sessionUserId, _photoService, searchQuery, orderId);
            searchQuery = searchQuery.Replace("|", ",");
            ViewBag.InfoMessage = $">> {count} images  were found with tags: ({searchQuery}).";
            model.PhotographyCount = count;
            model.BlockId = blockId;
            model.PageId = pageId;
            ViewBag.CurrentPage = pageId;
            ViewBag.BlockId = blockId;
            ViewBag.BlockSize = _photoService.BlockSize;

            // perirst searchQuery for redirects
            ViewBag.SearchQuery = searchQuery;

            return model;
        }

        private GalleryIndexViewModel DisplayPhotographyOrders(Order order, int pageId, int blockId, GalleryIndexViewModel model, int sessionUserId)
        {
            model.Album = (List<Photography>)_photoService.GetOrderPhotographies(sessionUserId, order.OrderId, pageId);
            var count = GetPhotographyIdsList(sessionUserId, _photoService, "", order.OrderId);
            string label = (count == 1) ? "photography" : "photographies";
            ViewBag.InfoMessage = $">> ({count}) {label} in<br/>Order #{order.Number}<br/>created on {order.CreatedDtm}";
            model.CartItemsSequence = "";
            model.ShoppingCartAction = SelectedItemAction.none.ToString();
            model.PhotographyCount = count;
            model.BlockId = blockId;
            model.PageId = pageId;
            ViewBag.CurrentPage = pageId;
            ViewBag.BlockId = blockId;
            ViewBag.BlockSize = _photoService.BlockSize;
            ViewBag.SearchQuery = "";

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
        
        private string ProcessShoppingCartAction(long id, GalleryDetailViewModel model, string message, int sessionUserId)
        {
            Order order = GetCurrentActiveOrder(sessionUserId);

            switch (model.ShoppingCartAction)
            {
                case "add":
                    {
                        string label;

                        if (order.OrderId == -1)
                        {
                            order = _photoService.AddOrder(sessionUserId);
                            if (order.OrderId == -2)
                            {
                                message = "Cannot create a new order! ...</br>" +
                                                "there is already a current, active order, if you cannnot</br>" +
                                                "cancel it first  then contact the administrator.";
                                return message;
                            }
                            label = "new";
                        }
                        else
                        {
                            label = "existing";
                        }
                        _photoService.AddPhotographyToOrder(id, order.OrderId, sessionUserId);
                        message = $"Photography ({id}) has been added to {label} order #{order.Number} created on {order.CreatedDtm}.";
                        break;
                    }
                case "remove":
                    {
                        _photoService.RemovePhotographyFromOrder(id, order.OrderId, sessionUserId);
                        message = $"This photography ({id}) has been removed from order #{order.Number}.";
                        break; 
                    }
                    default : break;
            }

            return message;
        }

        private void SetViewRedirectInfo(string controlerName, string actionName,  out int sessionUserId, out RedirectResponseModel redirectInfo, long routeId = -1)
        {
            sessionUserId = GetCurrentUserId();

            var remoteHostName = HttpUtility.GetClientRemoteId(HttpContext);
            string queryString = HttpContext.Request.QueryString.Value;
            redirectInfo = _photoService.SetRedirectInfo(userId: sessionUserId, remoteHost: remoteHostName, controller: controlerName, action: actionName, routeId: routeId, queryString: queryString);
            //throw new Exception($"{actionName}: {remoteHostName},{queryString},{id}");
        }
        
        private Order GetCurrentActiveOrder(int sessionUserId)
        {
            var order = _photoService.GetCurrentActiveOrder(sessionUserId);

            order ??= new Order(-1, sessionUserId);
            
            if(HttpContext != null && HttpContext.Session != null)
                HttpContext.Session.SetInt32("OrderID", order.OrderId);

            return order;
        }

        private int GetPhotographyIdsList(int userID, IPhotoService photoService, string searchQuery, int OrderId)
        {
            if (!string.IsNullOrEmpty(searchQuery))
                searchQuery = searchQuery.Replace(',', '|'); // prepare search for use as regular expression

             IPhotoService.ImageListSource source = ImageListSource.gallery;

            if (!string.IsNullOrEmpty(searchQuery))
                source = ImageListSource.searchResults;
            else if (OrderId != -1)
                source = ImageListSource.shoppingCart;
            else
                searchQuery = "";

            (string galleryIdList, long rowCount) = photoService.GetPhotographyIdsList(userID, source, searchQuery, OrderId);

            HttpContext.Session.SetString("galleryIdList", galleryIdList);
            ViewBag.PageCount = (int)(rowCount / _photoService.PageSize) + 1;

            return (int)rowCount;
        }

        private int SetGalleryNavigationIds(long id)
        {
            int pageId = (int)(id / _photoService.PageSize) + 1;

            // correct border cases
            if (id % _photoService.PageSize == 0)
                pageId--;
            
            return pageId;
        }

        private GalleryDetailViewModel PrepareDetailViewModel(long id, string searchQuery, int pageId)
        {
            var image = _photoService.GetPhotographyById(id, GalleryIndexViewModel.UserID);

            var model = new GalleryDetailViewModel
            {
                Image = image,
                Location = (image is null) ? "" : image.Location,
                PageId = pageId,
                SearchQuery = searchQuery,
                SelectedTagListAction = NoActionSelected,
                ShoppingCartAction = NoActionSelected
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
                folder = folder[..folder.LastIndexOf(@"\")];
            }
            if (debugData != null) debugData["path"] = folder;

            return folder;
        }

        /// <summary>
        /// Duplicate in login controller!
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private ActionResult RedirectViewGalleryIndex(int userId)
        {
            var remoteHostName = HttpUtility.GetClientRemoteId(HttpContext);

            var redirect = _photoService.GetRedirectInfo(userId, remoteHostName);
            TempData["isSignedIn"] = Startup.IsSignedIn;
            if (redirect != null && redirect.RemoteHost != "")
                return RedirectToAction(controllerName: redirect.Controller, actionName: redirect.Action, routeValues: new RouteValueDictionary(redirect.RouteData));
            else
                return RedirectToAction(controllerName: "Gallery", actionName: "Index");
        }

        private string GetRedirectUrl(HttpUtility.GalleryViewTypes overwriteView, int userId)
        {
            var remoteHostName = HttpUtility.GetClientRemoteId(HttpContext);

            var redirect = _photoService.GetRedirectInfo(userId, remoteHostName);

            return HttpUtility.GetRedirectUrl(redirect, overwriteAction: overwriteView);
        }

    }
}