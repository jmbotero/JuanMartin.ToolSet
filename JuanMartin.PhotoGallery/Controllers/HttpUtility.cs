using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using JuanMartin.Kernel.Extesions;
using Microsoft.AspNetCore.Http.Features;

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
        /// <summary>
        /// With the help of Regular Expressions (Regex), one can easily 
        /// detect and find whether the User is using a Desktop browser 
        /// or mobile phone browser (mobile device) in ASP.Net MVC Razor.
        /// <see cref="https://www.aspsnippets.com/Articles/Detect-Mobile-Browser-Mobile-Device-in-ASPNet-MVC.aspx#:~:text=ASP.Net%20does%20have%20in-built%20mobile%20phone%20browser%20detection,browser%20or%20Desktop%20browser%20using%20the%20Request.Browser.IsMobileDevice%20property."/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static (bool IsMobile, string DeviceInfo) IsMobileDevice(HttpContext context)
        {
            string userAgent = context.GetServerVariable("HTTP_USER_AGENT");
            if (userAgent != null)
            {
                Regex OS = new(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Regex device = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string device_info = string.Empty;
                if (OS.IsMatch(userAgent))
                {
                    device_info = OS.Match(userAgent).Groups[0].Value;
                }
                if (device.IsMatch(userAgent.Substring(0, 4)))
                {
                    device_info += device.Match(userAgent).Groups[0].Value;
                }
                if (!string.IsNullOrEmpty(device_info))
                {
                    return (true, device_info);
                }
                else
                {
                    return (true, "");
                }
            }
            return (false, null);
        }
        public static string GetClientRemoteId(HttpContext context)
        {
            string id = context.GetServerVariable("REMOTE_HOST");

            if (string.IsNullOrEmpty(id))
                id = context.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
                //id = context.GetServerVariable("REMOTE_ADDR");
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
