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
        int GetGalleryPageCount(int pageSize);
        double GetAverageRanking(long id);
        IEnumerable<Photography> GetAllPhotographies(int userId, int pageId = 1);
        Photography GetPhotographyById(long id, int userId);
        int UpdatePhotographyRanking(long id, int userId, int rank);
        IEnumerable<Photography> GetPhotographiesByKeyword(string keywords, int userId, int pageId = 1);

        static void LoadPhotographies(IExchangeRequestReply dbAdapter, string directory, string acceptedExtensions, bool directoryIsLink)
        {
            var photographies = new List<Photography>();
            //AdapterMySql dbAdapter = new(Startup.ConnectionString);//("localhost", "photogallery", "root", "yala");
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
                long id = AddPhotography((AdapterMySql)dbAdapter, name, path, title);
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
