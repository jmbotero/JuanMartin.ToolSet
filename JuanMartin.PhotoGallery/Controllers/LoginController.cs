using JuanMartin.Kernel.Utilities;
using JuanMartin.PhotoGallery.Models;
using JuanMartin.PhotoGallery.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class LoginController : Controller
    {
        private readonly IPhotoService _photoService;
        private readonly IConfiguration _configuration;

        public LoginController(IPhotoService photoService, IConfiguration configuration)
        {
            _photoService = photoService;
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        /// <summary>
        /// Verify Email, Generate Reset password link, Send Email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ForgotPassword(LoginViewModel model)
        {
            var user = _photoService.VerifyEmail(model.Email); 
            string message;

            if (user.UserId != -1)
            {
                //Send email for reset password
                string resetCode = Guid.NewGuid().ToString();
                HttpUtility.SendVerificationEmail(user.Email, PasswordResetLink(HttpContext, resetCode), _configuration);
                _photoService.StoreActivationCode(user.UserId, resetCode);
                message = "Reset password link has been sent to the specifiied email.";
            }
            else
            {
                message = $"No user, asosciated to {model.Email} was found.";
            }

            ViewBag.Message = message;
            return View(model);
        }

        /// <summary>
        /// Verify the reset password link, find account associated with this link, redirect to reset password page
        /// </summary>
        /// <param name="id">reset guid</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            ViewBag.IsCodevalid = false;
            string message;
            ResetPasswordViewModel model = new();
            model.ResetCode = id;

            if (string.IsNullOrWhiteSpace(id) && !UtilityString.IsGuid(id))
            {
                message = "Request contained an invalid activation code.";
            }
            else
            {
                (int code, JuanMartin.Models.Gallery.User user) = _photoService.VerifyActivationCode(id);

                model.UserId = user.UserId;
                model.UserName = user.UserName;

                if (code == 1)
                {
                    ViewBag.IsCodevalid = true;
                    return View(model);
                }
                else if (code == -1)
                {
                    message = "Activation code was not matched in database.";
                }
                else
                {
                    message = "Activation code expired.";
                }
            }

            ViewBag.Message = message;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            string message = "";
            if (ModelState.IsValid)
            {
                if (model.NewPassword == model.ConfirmPassword)
                {
                    var user = _photoService.UpdateUserPassword(model.UserId, model.UserName, model.NewPassword);

                    if (user != null)
                        message = $"New password for {user.UserName} updated successfully!";
                    else
                        message = "Database error: password was not updated.";
                }
            }
            else
            {
                message = "Password reset failed.";
            }

            ViewBag.Message = message;
            ViewBag.IsCodevalid = true;
            return View(model);
        }

        [HttpGet]
        public ActionResult OnPasswordUdateSuccess(int id)
        {
            StartNewSession(id);
            return DisplayGallery(id);
        }

        [HttpGet]
        public ActionResult Register()
        {
            TempData["isSignedIn"] = Startup.IsSignedIn;
            return View();
        }
        [HttpPost]
        public ActionResult Register(LoginViewModel model)
        {
            var user = _photoService.AddUser(model.UserName, model.Password, model.Email);

            if (user.UserId == -2)
                ViewBag.Message = "User already exists, please try a different user name.";
            if (user.UserId > 0)    //userCreated = true;
                return RedirectToAction("Gallery", "Index");
            if (user.UserId == -1)
                ViewBag.Message = "Error occurred in backend while creating user, please contact the site owner.";

            StartNewSession(user);
            TempData["isSignedIn"] = Startup.IsSignedIn;
            return View(model);
        }

        [HttpPost]
        public ActionResult Logout()
        {
            int sessionId = (int)HttpContext.Session.GetInt32("SessionID");
            _photoService.EndSession(sessionId);
            HttpContext.Session.Clear();
            Startup.IsSignedIn = "false";

            TempData["isSignedIn"] = Startup.IsSignedIn;
            return RedirectToAction(controllerName: "Gallery", actionName: "Index");

        }
        public ActionResult Login()
        {
            TempData["isSignedIn"] = Startup.IsSignedIn;
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
                    StartNewSession(u);

                    return DisplayGallery(u.UserId);
                }
                else if (u == null || u.UserId == -1)
                {
                    ViewBag.Message = "Incorrect usert name and/or password specified. Please try again.";
                }
            } 
            TempData["isSignedIn"] = Startup.IsSignedIn;

            return View(model);
        }

        private void StartNewSession(JuanMartin.Models.Gallery.User u)
        {
            int userId = u.UserId;
            var remoteHostName = HttpUtility.GetClientRemoteId(HttpContext);

            var sessionId = _photoService.LoadSession(userId);
            HttpContext.Session.SetInt32("SessionID", sessionId);
            HttpContext.Session.SetInt32("UserID",userId);
            _photoService.ConnectUserAndRemoteHost(userId, remoteHostName);
            Startup.IsSignedIn = "true";
        }

        private void StartNewSession(int userId)
        {
            var sessionId = _photoService.LoadSession(userId);
            HttpContext.Session.SetInt32("SessionID", sessionId);
            HttpContext.Session.SetInt32("UserID", userId);
            Startup.IsSignedIn = "true";
        }

        private static string PasswordResetLink(HttpContext context, string resetCode)
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";

            return $"{baseUrl}/Login/ResetPassword/{resetCode}";
        }

        private ActionResult DisplayGallery(int userId)
        {
            var remoteHostName = HttpUtility.GetClientRemoteId(HttpContext);

            var redirect = _photoService.GetRedirectInfo(userId, remoteHostName);
            if (redirect != null && redirect.RemoteHost != "")
                return RedirectToAction(controllerName: redirect.Controller, actionName: redirect.Action, routeValues: new RouteValueDictionary(redirect.RouteData));
            else
                return RedirectToAction(controllerName: "Gallery", actionName: "Index");
        }
    }
}
