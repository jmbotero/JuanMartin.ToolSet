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
        public string SearchQuery { get; set; }
        public int PageId { get; set; }
        public string Location { get; set; }
        public int SelectedRank { get; set; }
        public int PlaceHolderRank { get; set; }
        public Photography Image { get; set; }
        public string ImageIdList { get; set; }
        //public List<SelectListItem> Tags { get; set; }
        public string Tag { get; set; }
        public string SelectedTagListAction { get; set; }
        public string ShoppingCartAction { get; set; }
        public string PlaceHolderSource { get; set; }
    }
}
