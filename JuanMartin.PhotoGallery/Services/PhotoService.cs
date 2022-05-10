
using JuanMartin.Models.Gallery;
using System;
using System.Collections.Generic;

namespace JuanMartin.PhotoGallery.Services
{
    public class PhotoService : IPhotoService
    {
        public const int PageSize = 8;

        internal static void LoadPhotographies(string directory, string acceptedExtensions, bool directoryIsLink)
        {
            IPhotoService.LoadPhotographies(directory, acceptedExtensions, directoryIsLink);
        }

        internal static IEnumerable<Photography> GetAllPhotographies(int userID, int pageId)
        {
            return IPhotoService.GetAllPhotographies(userID, pageId);
        }
    }
}
