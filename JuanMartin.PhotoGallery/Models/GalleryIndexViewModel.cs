using JuanMartin.Models.Gallery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JuanMartin.PhotoGallery.Models
{
    public class GalleryIndexViewModel
    {
        public const int UserID = 1;
        public List<Photography> Album { get; set; }
        public long PhotographyCount{ get; set; }
        public int PageId { get; set; }
        public int BlockId { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Locations { get; set; }
        public string ShoppingCartAction { get; set; }
        public string CartItemsSequence { get; set; }
        public string DragImageIndex { get; set; }
        public string DropImageIndex { get; set; }

    }
}
