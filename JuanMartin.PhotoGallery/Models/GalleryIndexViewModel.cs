using JuanMartin.Models.Gallery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JuanMartin.PhotoGallery.Models
{
    public class GalleryIndexViewModel
    {
        public List<Photography> Album { get; set; }
        public string SearchQuery { get; set; }
    }
}
