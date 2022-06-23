using JuanMartin.Models.Gallery;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JuanMartin.PhotoGallery.Models
{
    public class GalleryDetailViewModel
    {
        public int PageId { get; set; }
        public int SelectedRank { get; set; }
        public Photography Image { get; set; }
        //public List<SelectListItem> Tags { get; set; }
        public string Tag { get; set; }
        public string SelectedTagListAction { get; set; }
    }
}
