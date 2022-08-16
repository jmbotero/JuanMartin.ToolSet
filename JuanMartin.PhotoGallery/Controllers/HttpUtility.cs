using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using JuanMartin.Kernel.Extesions;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class HttpUtility
    {
        public enum ImageRelativePosition
        {
            Previous = 0,
            Next
        };

        public static long GetImageId(long currenId, string idList, ImageRelativePosition position)
        {
            long result = -1;

            string id = $",{currenId},";
            int i = idList.IndexOf(id);

            if (i > -1)
            {
                switch (position)
                {
                    case ImageRelativePosition.Previous:
                        {
                            if (i < 1)
                                result = -1;
                            else
                            {
                                string before = idList.Substring(0, i);
                                var digit = before.Substring(before.LastIndexOf(',') + 1);
                                result = Convert.ToInt64(digit);
                            }
                            break;
                        }
                    case ImageRelativePosition.Next:
                        {
                            var endOfId = i + id.Length;
                            if (endOfId == idList.Length)
                                result = -1;
                            else
                            {
                                string after = idList.Substring(endOfId);
                                var digit = after.Substring(0,after.IndexOf(','));
                                result = Convert.ToInt64(digit);
                            }
                            break;
                        }
                    default:
                        {
                            result = -1;
                            break;
                        }
                }
            }
            return result;
        }

        public static List<SelectListItem> SetListOfItemsforDisplay(List<string> listOfItems, string selectedItem)
        {
            List<SelectListItem> items = new();
            foreach (var  i in listOfItems)
                items.Add(new SelectListItem { Text = i, Selected = (selectedItem == i) });

            return items;
        }


        public static string GetClientRemoteId(HttpContext context)
        {
            string id = context.GetServerVariable("REMOTE_HOST");

            if (string.IsNullOrEmpty(id))
                id = context.GetServerVariable("REMOTE_ADDR");
            if (string.IsNullOrEmpty(id))
                id = context.GetServerVariable("REMOTE_USER");

            return id;
        }

        public static void SendVerificationEmail(string mailTo, string passwordResetLink, IConfiguration configuration)
        {
            var toEmail = new MailAddress(mailTo);
            var fromEmail = new MailAddress(configuration.GetSection("SmtpClient")["SenderEmailId"], "JuanMarttin.PhotoGallery");
            var fromEmailPassword = configuration.GetSection("SmtpClient")["OutgoingEmailAccountPassword"];
            //throw new Exception($"{configuration.GetSection("SmtpClient")["HostName"]}");
            SmtpClient smtp = new(host: configuration.GetSection("SmtpClient")["HostName"],
                port: Convert.ToInt32(configuration.GetSection("SmtpClient")["SmtpPort"]))
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };
            string subject = "Reset Password";
            string body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
                "<br/><br/>Click this <a href=" + passwordResetLink + ">reset password link</a>";

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
