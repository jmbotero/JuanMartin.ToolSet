
using JuanMartin.Kernel;
using JuanMartin.Kernel.Adapters;
using JuanMartin.Kernel.Messaging;
using JuanMartin.Models.Gallery;
using System;
using System.Collections.Generic;

namespace JuanMartin.PhotoGallery.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IExchangeRequestReply _dbAdapter;

        public const int PageSize = 8;

        public PhotoService()
        {
            _dbAdapter = new AdapterMySql(Startup.ConnectionString);
        }
        internal static void LoadPhotographies(IExchangeRequestReply dbAdapter, string directory, string acceptedExtensions, bool directoryIsLink)
        {
            IPhotoService.LoadPhotographies(dbAdapter, directory, acceptedExtensions, directoryIsLink);
        }

        public IEnumerable<Photography> GetAllPhotographies(int userId, int pageId = 1)
        {
            var photographies = new List<Photography>();
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("PhotoGraphies", $"uspGetAllPhotographies({pageId},{PhotoService.PageSize},{userId})"));
            request.AddSender("uspGetAllPhotographies", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder record in reply.Data.Annotations)
                {
                    // be sure record.GetAnnotation("...") exists and is not null
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

                    photography.AddKeywords(keywords);

                    photographies.Add(photography);
                }
            }

            return photographies;
        }

        public int GetGalleryPageCount(int pageSize)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetPageCount", $"uspGetPageCount({pageSize})"));
            request.AddSender("Gallery", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            // be sure record.GetAnnotation("...") exists and is not null
            var pageCount = (int)reply.Data.GetAnnotationByValue(1).GetAnnotation("pageCount").Value;

            return pageCount;
        }

        public IEnumerable<Photography> GetPhotographiesByKeyword(string keywords, int userId, int pageId = 1)
        {
            throw new NotImplementedException();
        }

        public Photography GetPhotographyById(long photographyId, int userId)
        {
            Photography photography=null;

            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetPotography", $"uspGetPotography({photographyId},{userId})"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder record in reply.Data.Annotations)
                {
                    // be sure record.GetAnnotation("...") exists and is not null
                    var id = (long)record.GetAnnotation("Id").Value;

                    if (id == -1)
                        return null;

                    var source = Convert.ToInt32(record.GetAnnotation("Source").Value);
                    var path = (string)record.GetAnnotation("Path").Value;
                    var fileName = (string)record.GetAnnotation("Filename").Value;
                    var title = (string)record.GetAnnotation("Title").Value;
                    var location = (string)record.GetAnnotation("Location").Value;
                    var rank = (long)record.GetAnnotation("Rank").Value;
                    var keywords = (string)record.GetAnnotation("Keywords").Value;

                    photography = new Photography
                    {
                        UserId = Models.GalleryIndexViewModel.UserID,
                        Id = id,
                        FileName = fileName,
                        Path = path,
                        Source = (Photography.PhysicalSource)source,
                        Title = title,
                        Location = location,
                        Rank = rank
                    };

                    photography.AddKeywords(keywords);
                }
            }

            return photography;
        }

        public double GetAverageRanking(long id)
        {
            throw new NotImplementedException();
        }

        public int UpdatePhotographyRanking(long id, int userId, int rank)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspUpdateRanking", $"uspUpdateRanking({userId},{id},{rank})"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            // be sure record.GetAnnotation("...") exists and is not null
            var rankingId = (int)reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value;

            return rankingId;
        }
    }
}
