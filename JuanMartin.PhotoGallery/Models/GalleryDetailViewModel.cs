using JuanMartin.Models.Gallery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JuanMartin.PhotoGallery.Models
{
    public class GalleryDetailViewModel
    {
        public int PageId { get; set; }
        public Photography Image { get; set; }
    }
}
