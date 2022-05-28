using JuanMartin.PhotoGallery.Models;
using JuanMartin.PhotoGallery.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class LoginController : Controller
    {
        private readonly IPhotoService _photoService;

        public LoginController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = _photoService.GetUser(model.UserName, model.Password);


                if (u != null)
                {
                    var sessionId = _photoService.LoadSession(u.UserId);
                    HttpContext.Session.Set("SessionID", Encoding.UTF8.GetBytes(sessionId.ToString()));
                    HttpContext.Session.Set("UserID", Encoding.UTF8.GetBytes(u.UserId.ToString()));

                    var clientId = TempData["clientId"].ToString();

                    var redirect = _photoService.GetRedirectInfo(clientId);
                    if (redirect.RemoteHost != "")
                        return RedirectToAction(controllerName: redirect.Controller, actionName: redirect.Action, routeValues: redirect.RouteData);
                }
                else if (u == null || u.UserId == -1)
                {
                    ViewBag.Message = "Incorrect usert name and/or password specified. Please try again.";
                }
            }

            return View(model);
        }
    }
}
