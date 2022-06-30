using JuanMartin.Kernel;
using JuanMartin.Kernel.Adapters;
using JuanMartin.Kernel.Messaging;
using JuanMartin.Kernel.Utilities;
using JuanMartin.Models.Gallery;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.Sandbox.Objects
{
    public class PhotoService
    {
        public static void ConnectUserAndRemoteHost(int userId, string remoteHost)
        {
            AdapterMySql _dbAdapter = new AdapterMySql("localhost", "photogallery", "root", "yala");
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspConnectUserAndRemoteHost", $"uspConnectUserAndRemoteHost({userId},'{remoteHost}')"));
            request.AddSender("User", typeof(User).ToString());

            _dbAdapter.Send(request);
        }

        public static IEnumerable<Photography> GetAllPhotographies(int userId, int pageId = 1)
        {
            AdapterMySql _dbAdapter = new AdapterMySql("localhost", "photogallery", "root", "yala");
            var photographies = new List<Photography>();
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection nnnot set.");

            Message request = new Message("Command", Type: System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("PhotoGraphy", $"uspGetAllPhotographies({pageId},8,{userId})"));
            request.AddSender("AddPhotoGraphy", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder record in reply.Data.Annotations)
                {
                    var id = (long)record.GetAnnotation("Id").Value;
                    var source = Convert.ToInt32(record.GetAnnotation("Source").Value);
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

                    photography.ParseTags(keywords);

                    photographies.Add(photography);
                }
            }

            return photographies;
        }
        public static int GetGalleryPageCount(int pageSize)
        {
            AdapterMySql _dbAdapter = new AdapterMySql("localhost", "photogallery", "root", "yala");
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection nnnot set.");

            Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetPageCount", $"uspGetPageCount({pageSize})"));
            request.AddSender("Gallery", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            var pageCount = (int)reply.Data.GetAnnotationByValue(1).GetAnnotation("pageCount").Value;

            return pageCount;
        }
        public static int UpdatePhotographyRanking(long id, int userId, int rank)
        {
            AdapterMySql _dbAdapter = new AdapterMySql("localhost", "photogallery", "root", "yala");
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection nnnot set.");

            Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspUpdateRanking", $"uspUpdateRanking({id},{userId},{rank})"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            // be sure record.GetAnnotation("...") exists and is not null
            var rankingId = (int)reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value;

            return rankingId;
        }
        public static User AddUser(string userName, string password, string email)
        {
            AdapterMySql _dbAdapter = new AdapterMySql("localhost", "photogallery", "root", "yala");
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspAddUser", $"uspAddUser('{userName}','{password}','{email}')"));
            request.AddSender("User", typeof(User).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            var Id = Convert.ToInt32(reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value);

            User user = new User()
            {
                UserId = Id,
                UserName = userName,
                Password = "",
                Email = email
            };
            return user;
        }
        public static RedirectResponseModel SetRedirectInfo(int userId, string remoteHost, string controller, string action, long routeId = -1, string queryString = "")
        {
            AdapterMySql _dbAdapter = new AdapterMySql("localhost", "photogallery", "root", "yala");
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspSetCurrentClientRedirectInfo", $"uspSetCurrentClientRedirectInfo({userId},'{remoteHost}','{controller}','{action}',{routeId},'{queryString}')"));
            request.AddSender("RedirectRequestModel", typeof(RedirectResponseModel).ToString());

            _dbAdapter.Send(request);

            RedirectResponseModel requestModel = new RedirectResponseModel()
            {
                RemoteHost = remoteHost,
                Controller = controller,
                Action = action
            };
            return requestModel;
        }

        public static int AddTag(string tag, long id)
        {
            AdapterMySql _dbAdapter = new AdapterMySql("localhost", "photogallery", "root", "yala");
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspAddTag", $"uspAddTag('{tag}',{id})"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            var Id = Convert.ToInt32(reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value);

            return Id;
        }

        public static void AddAuditMessage(int userId, string meessage)
        {
            AdapterMySql _dbAdapter = new AdapterMySql("localhost", "photogallery", "root", "yala");
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspAddAuditMessage", $"uspAddAuditMessage({userId},'{meessage}')"));
            request.AddSender("User", typeof(User).ToString());

            _dbAdapter.Send(request);
        }
        public static void LoadPhotographies(IExchangeRequestReply dbAdapter, string directory, string acceptedExtensions, bool directoryIsLink)
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
                    path = "~" + photosTag + path.Substring(i);
                }
                else if (path.Contains(photosTag))
                {
                    int i = path.IndexOf(photosTag);
                    path = "~" + path.Substring(i);
                }
                string title = "";
                long id = AddPhotography((AdapterMySql)dbAdapter, name, path, title);
            }
        }

        private static long AddPhotography(AdapterMySql DbAdapter, string name, string path, string title)
        {
            int source = (int)GetPhotographySource(path);

            Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspAddPhotoGraphy", $"uspAddPhotoGraphy('{source}','{name}','{path}','{title}')"));
            request.AddSender("PhotoGraphy", typeof(Photography).ToString());

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
