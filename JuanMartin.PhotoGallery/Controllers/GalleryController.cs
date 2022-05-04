using JuanMartin.Kernel;
using JuanMartin.Kernel.Adapters;
using JuanMartin.Kernel.Messaging;
using JuanMartin.Kernel.Utilities;
using JuanMartin.Models.Gallery;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            var path = Directory.GetCurrentDirectory() + @"\wwwroot\photos.lnk";
            var model = new Gallery
            {
                Album = LoadPhotographies(path, ".jpg", true)
            };

            return View(model);
        }


        private static List<PhotoGraphy> LoadPhotographies(string directory, string acceptedExtensions,bool directoryIsLink)
        {
            var photographies = new List<PhotoGraphy>();
            AdapterMySql dbAdapter = new("localhost", "photogallery", "root", "yala");
            var files = UtilityFile.GetAllFiles(directory, directoryIsLink);

            foreach (FileInfo file in files)
            {
                if (!acceptedExtensions.Contains(file.Extension))
                    continue;

                const string archiveTag = @"Archive\";
                const string photosTag = @"\photos";
                string name = file.Name;
                string path = file.DirectoryName;
                //preproces for web project
                if (path.Contains(archiveTag))
                {
                    int i = path.IndexOf(archiveTag) + archiveTag.Length - 1;
                    path = "~" + photosTag + path[i..];
                }
                else if (path.Contains(photosTag))
                {
                    int i = path.IndexOf(photosTag);
                    path = "~" + path[i..];
                }
                string title = "";
                long id = AddPhotography(dbAdapter, name, path, title);

                photographies.Add(new PhotoGraphy { Id = id, FileName = name, Path = path, Title = title });
            }

            return photographies;
        }
        private static string GetProjectDirectory(Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary debugData=null)
        {
            string folder = Directory.GetCurrentDirectory();
            if (debugData != null) debugData["folder" ] = folder;

            for (int s = 0; s < 2; s++)
            {
                folder = folder.Substring(0, folder.LastIndexOf(@"\"));
            }
            if (debugData != null) debugData["path"] = folder;

            return folder;
        }

        private static long AddPhotography(AdapterMySql DbAdapter, string name, string path, string title)
        {
            int source = (int)GetPhotographySource(path);

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("PhotoGraphy", $"uspAddPhotoGraphy({source},'{name}','{path}','{title}')"));
            request.AddSender("AddPhotoGraphy", typeof(PhotoGraphy).ToString());

            DbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)DbAdapter.Receive();

            var Id = (long)reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value;

            return Id;
        }

        private static PhotoGraphy.PhysicalSource GetPhotographySource(string path)
        {
            if (path.Contains(@"slide"))
                return PhotoGraphy.PhysicalSource.slide;

            return PhotoGraphy.PhysicalSource.negative;
        }
    }
}
