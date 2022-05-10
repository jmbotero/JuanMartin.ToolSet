using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using JuanMartin.Models.Gallery;
using JuanMartin.Kernel;
using JuanMartin.Kernel.Adapters;
using JuanMartin.Kernel.Messaging;
using JuanMartin.Kernel.Utilities;

namespace JuanMartin.PhotoGallery.Services
{
    public interface IPhotoService
    {
        static IEnumerable<Photography> GetAllPhotographies(int userId, int pageId = 1)
        {
            var photographies = new List<Photography>();
            AdapterMySql dbAdapter = new("localhost", "photogallery", "root", "yala");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("PhotoGraphy", $"uspGetAllPhotographies({pageId},{PhotoService.PageSize},{userId})"));
            request.AddSender("AddPhotoGraphy", typeof(Photography).ToString());

            dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)dbAdapter.Receive();

            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder record in reply.Data.Annotations)
                {
                    var id = (long)record.GetAnnotation("Id").Value;
                    var source = (int)record.GetAnnotation("Source").Value;
                    var path = (string)record.GetAnnotation("Path").Value;
                    var fileName = (string)record.GetAnnotation("Filename").Value;
                    var title = (string)record.GetAnnotation("Title").Value;
                    var location = (string)record.GetAnnotation("Location").Value;
                    var rank = (long)record.GetAnnotation("Rank").Value;
                    var keywords = (string)record.GetAnnotation("Keywords").Value;

                    var photography = new Photography
                    {
                        UserId = userId,
                        Id = id,
                        FileName = fileName,
                        Path = path,
                        Source = (Photography.PhysicalSource)source,
                        Title = title,
                        Location = location,
                        Rank = rank
                    };

                    photography.AddKeywords(keywords);

                    photographies.Add(photography);
                }
            }

            return photographies;
        }

        static void LoadPhotographies(string directory, string acceptedExtensions, bool directoryIsLink)
        {
            var photographies = new List<Photography>();
            AdapterMySql dbAdapter = new("localhost", "photogallery", "root", "yala");
            var files = UtilityFile.GetAllFiles(directory, directoryIsLink);

            // paginate and exclude uaccepted etensions
            files = files.Where(f => acceptedExtensions.Contains(f.Extension)).ToList();

            const string archiveTag = @"Archive\";
            const string photosTag = @"\photos";

            foreach (FileInfo file in files)
            {
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
            }
        }

        private static long AddPhotography(AdapterMySql DbAdapter, string name, string path, string title)
        {
            int source = (int)GetPhotographySource(path);

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("PhotoGraphy", $"uspAddPhotoGraphy({source},'{name}','{path}','{title}')"));
            request.AddSender("AddPhotoGraphy", typeof(Photography).ToString());

            DbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)DbAdapter.Receive();

            var Id = (long)reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value;

            return Id;
        }

        private static Photography.PhysicalSource GetPhotographySource(string path)
        {
            if (path.Contains(@"slide"))
                return Photography.PhysicalSource.slide;

            return Photography.PhysicalSource.negative;
        }
    }
}
