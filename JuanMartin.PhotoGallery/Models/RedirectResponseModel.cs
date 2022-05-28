using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JuanMartin.PhotoGallery.Models
{
    public class RedirectResponseModel
    {
        public string RemoteHost { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string JsonViewModel { get; set; }
        public RouteValueDictionary RouteData { get; set; }
    }
}
