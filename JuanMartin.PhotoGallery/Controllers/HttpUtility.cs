﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class HttpUtility
    {
        public static string GetClientRemoteId(HttpContext context)
        {
            string id = context.GetServerVariable("REMOTE_HOST");

            if (string.IsNullOrEmpty(id))
                id = context.GetServerVariable("REMOTE_ADDR");
            if (string.IsNullOrEmpty(id))
                id = context.GetServerVariable("REMOTE_USER");

            return id;
        }

        public static void SendEmail(string mailTo, string passwordResetLink, IConfiguration configuration)
        {
            var toEmail = new MailAddress(mailTo);
            var fromEmail = new MailAddress(configuration.GetSection("SmtpClient")["SenderEmailId"], "JuanMarttin.PhotoGallery");
            var fromEmailPassword = configuration.GetSection("SmtpClient")["OutgoingEmailAccountPassword"]; 

            SmtpClient smtp = new()
            {
                Host = configuration.GetSection("SmtpClient")["Hostame"],
                Port = Convert.ToInt32(configuration.GetSection("SmtpClient")["SmtpPort"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            string subject = "Reset Password";
            string body = "Hi,<br/>br/>We got request for reset your account password. Please click on the below link to reset your password" +
                "<br/><br/><a href=" + passwordResetLink + ">Reset Password link</a>";

            using var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            smtp.Send(message);
        }
    }
}