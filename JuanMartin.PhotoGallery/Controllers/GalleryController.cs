using JuanMartin.Models.Gallery;
using JuanMartin.Kernel;
using JuanMartin.Kernel.Adapters;
using JuanMartin.Kernel.Messaging;
using JuanMartin.Kernel.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            var model = new Gallery
            {
                Album = LoadPhotographies(@"C:\Temp\photos")
            };

            return View(model);
        }

        private static List<PhotoGraphy> LoadPhotographies(string directory)
        {
            var photographies = new List<PhotoGraphy>();
            AdapterMySql dbAdapter = new("localhost", "photogallery", "root", "yala");
            var files = UtilityFile.GetAllFiles(directory);

            foreach (FileInfo file in files)
            {
                string name = file.Name;
                string path = file.DirectoryName;
                string title = "";
                long id = AddPhotography(dbAdapter, name, path, title);

                photographies.Add(new PhotoGraphy { Id = id, FileName = name, Path = path, Title = title });
            }

            return photographies;
        }

        private static long AddPhotography(AdapterMySql DbAdapter, string name, string path, string title)
        {
            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("PhotoGraphy", $"uspAddPhotoGraphy('{name}','{path}','{title}')"));
            request.AddSender("AddPhotoGraphy", typeof(PhotoGraphy).ToString());

            DbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)DbAdapter.Receive();

            var Id = (long)reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value;

            return Id;
        }

    }
}
