
using JuanMartin.Kernel;
using JuanMartin.Kernel.Adapters;
using JuanMartin.Kernel.Messaging;
using JuanMartin.Models.Gallery;
using JuanMartin.PhotoGallery.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

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
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("PhotoGraphies", $"uspGetAllPhotographies({pageId},{PhotoService.PageSize},{userId})"));
            request.AddSender("uspGetAllPhotographies", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            return MapPhotographyListFromDatabaseReplyToEntityModel(userId, reply);
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

        public (long Lower, long Upper) GetPhotographyIdBounds()
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetPhotographyIdBounds", $"uspGetPhotographyIdBounds()"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            var lower = (long)reply.Data.GetAnnotationByValue(1).GetAnnotation("Lower").Value;
            var upper = (long)reply.Data.GetAnnotationByValue(1).GetAnnotation("Upper").Value;

            return (lower, upper);
        }

        public Photography GetPhotographyById(long photographyId, int userId)
        {

            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetPotography", $"uspGetPotography({photographyId},{userId})"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            var photographies = MapPhotographyListFromDatabaseReplyToEntityModel(userId, reply);

            return (photographies.Count==0)?null:photographies[0] ;
        }

        public int UpdatePhotographyRanking(long id, int userId, int rank)
        {
            if (userId == -1)
                return -1;

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

        public User GetUser(string userName, string password)
        {
            User user = null;

            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetUser", $"uspGetUser('{userName}','{password}')"));
            request.AddSender("User", typeof(User).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder record in reply.Data.Annotations)
                {
                    // be sure record.GetAnnotation("...") exists and is not null
                    var id = (int)record.GetAnnotation("Id").Value;

                    if (id == -1)
                        return null;

                    var email = (string)record.GetAnnotation("Email").Value;

                    user = new User
                    {
                        UserId = id,
                        UserName = userName,
                        Password = password,
                        Email = email
                    };
                }
            }

            return user;
        }

        public User VerifyEmail(string email)
        {
            User user = null;

            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspVerifyEmail", $"uspVerifyEmail('{email}')"));
            request.AddSender("User", typeof(User).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder record in reply.Data.Annotations)
                {
                    var id = (int)record.GetAnnotation("Id").Value;
                    var userName = (string)record.GetAnnotation("Login").Value;

                    user = new User
                    {
                        UserId = id,
                        UserName = userName,
                        Email = email
                    };
                }
            }

            return user;
        }

        public int LoadSession(int userId)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspAddSession", $"uspAddSession({userId})"));
            request.AddSender("Session", typeof(Microsoft.AspNetCore.Http.ISession).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            var Id = (int)reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value;

            return Id;
        }

        public RedirectResponseModel GetRedirectInfo(int userId, string remoteHost)
        {
            RedirectResponseModel requestModel = null;

            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetUserRedirectInfo", $"uspGetUserRedirectInfo('{remoteHost}',{userId})"));
            request.AddSender("RedirectRequestModel", typeof(RedirectResponseModel).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder record in reply.Data.Annotations)
                {
                    // be sure record.GetAnnotation("...") exists and is not null
                    var remoteHostName = (string)record.GetAnnotation("RemoteHost").Value;

                    if (remoteHostName == "")
                        return null;

                    var controller = (string)record.GetAnnotation("Controller").Value;
                    var action = (string)record.GetAnnotation("Action").Value;
                    var routeId = (int)record.GetAnnotation("RouteID").Value;
                    var queryString = (string)record.GetAnnotation("QueryString").Value;
                    var queryDic = GenerateRouteValues(routeId, queryString);

                    requestModel = new RedirectResponseModel
                    {
                        RemoteHost = remoteHostName,
                        Controller = controller,
                        Action = action,
                        RouteData =  queryDic
                    };
                }
            }

            return requestModel;
        }

        public RedirectResponseModel SetRedirectInfo(int userId,string remoteHost, string controller, string action, long routeId = -1, string queryString = "")
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());
            request.AddData(new ValueHolder("uspSetUserRedirectInfo", $"uspSetUserRedirectInfo('{userId}','{remoteHost}','{controller}','{action}',{routeId},'{queryString}')"));
            request.AddSender("RedirectRequestModel", typeof(RedirectResponseModel).ToString());

            _dbAdapter.Send(request);

            var queryDic = GenerateRouteValues(routeId, queryString);

            RedirectResponseModel requestModel = new()
            {
                RemoteHost = remoteHost,
                Controller = controller,
                Action = action,
                RouteData = queryDic
            };
            return requestModel;
        }

        /// <summary>
        /// <see cref="https://stackoverflow.com/questions/1257482/redirecttoaction-with-parameter"/>
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public Dictionary<string, object> GenerateRouteValues(long routeId, string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
                return null;

            NameValueCollection parsedValues = new();
            var querystringDic = new Dictionary<string, object>();

            if (queryString.Length > 1)
            {
                parsedValues = System.Web.HttpUtility.ParseQueryString(queryString);
                if (parsedValues != null)
                {
                    foreach (var key in parsedValues.AllKeys)
                    {
                        if (key == null) continue;
                        object value = (object)parsedValues[key];
                        querystringDic.Add(key, value);
                    }
                    //querystringDic = parsedValues.AllKeys.ToDictionary(k => k, k => (object)parsedValues[k]);
                }
            }

            if (routeId > 0 && parsedValues["id"] == null)
                querystringDic.Add("id", routeId);

            return querystringDic;
        }

        public User AddUser(string userName, string password, string email)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspAddUser", $"uspAddUser('{userName}','{password}','{email}')"));
            request.AddSender("User", typeof(User).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            var Id = (int)reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value;

            User user = new()
            {
                UserId = Id,
                UserName = userName,
                Password = "",
                Email = email
            };
            return user;
        }

        public User UpdateUserPassword(int userId, string userName, string password)
        {
            User user = null;

            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspUpdateUserPassword", $"uspUpdateUserPassword({userId},'{userName}','{password}')"));
            request.AddSender("User", typeof(User).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder record in reply.Data.Annotations)
                {
                    var id = Convert.ToInt32(record.GetAnnotation("Id").Value);
                    var email = (string)record.GetAnnotation("Email").Value;

                    user = new User
                    {
                        UserId = id,
                        UserName = userName,
                        Password = password,
                        Email = email
                    };
                }
            }

            return user;
        }

        public void EndSession(int sessionId)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspEndSession", $"uspEndSession({sessionId})"));
            request.AddSender("Session", typeof(int).ToString());

            _dbAdapter.Send(request);
        }

        public void StoreActivationCode(int userId, string activationCode)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspStoreActivationCode", $"uspStoreActivationCode({userId},'{activationCode}')"));
            request.AddSender("ActivationCode", typeof(string).ToString());

            _dbAdapter.Send(request);
        }

        public (int, User) VerifyActivationCode(string activationCode)
        {
            User user = null;
            int errorCode = -1;

            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspVerifyActivationCode", $"uspVerifyActivationCode('{activationCode}')"));
            request.AddSender("ActivationCode", typeof(System.Guid).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder record in reply.Data.Annotations)
                {
                    errorCode= (int)record.GetAnnotation("ErrorCode").Value;
                    var id = (int)record.GetAnnotation("Id").Value;
                    var login = (string)record.GetAnnotation("Login").Value;

                    user = new User
                    {
                        UserId = id,
                        UserName = login,
                        Password = "",
                        Email = ""
                    };
                }
            }

            return (errorCode, user);
        }

        private static List<Photography> MapPhotographyListFromDatabaseReplyToEntityModel(int userId, IRecordSet reply)
        {
            var photographies = new List<Photography>();

            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach(ValueHolder record in reply.Data.Annotations)
                {
                    var id = (long)record.GetAnnotation("Id").Value;
                    var source = Convert.ToInt32(record.GetAnnotation("Source").Value);
                    var path = (string)record.GetAnnotation("Path").Value;
                    var fileName = (string)record.GetAnnotation("Filename").Value;
                    var title = (string)record.GetAnnotation("Title").Value;
                    var location = (string)record.GetAnnotation("Location").Value;
                    var rank = (long)record.GetAnnotation("Rank").Value;
                    var averageRank = (double)record.GetAnnotation("AverageRank").Value;
                    var tags = (string)record.GetAnnotation("Tags").Value;

                    var photography = new Photography
                    {
                        UserId = userId,
                        Id = id,
                        FileName = fileName,
                        Path = path,
                        Source = (Photography.PhysicalSource)source,
                        Title = title,
                        Location = location,
                        Rank = rank,
                        AverageRank = averageRank
                    };

                    photography.ParseTags(tags);

                    photographies.Add(photography);
                }

            }

            return photographies;
        }

        public int AddTag(int userId, string tag, long id)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspAddTag", $"uspAddTag('{userId}','{tag}',{id})"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            var Id = Convert.ToInt32(reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value);

            return Id;
        }

        public int RemoveTag(int userId,  string tag, long id)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspRemoveTag", $"uspRemoveTag('{userId}','{tag}',{id})"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            var Id = Convert.ToInt32(reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value);

            return Id;
        }

        public void ConnectUserAndRemoteHost(int userId, string remoteHost)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspConnectUserAndRemoteHost", $"uspConnectUserAndRemoteHost({userId},'{remoteHost}')"));
            request.AddSender("User", typeof(User).ToString());

            _dbAdapter.Send(request);
        }

        public void AddAuditMessage(int userId, string meessage)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspAddAuditMessage", $"uspAddAuditMessage('{userId}','{meessage}')"));
            request.AddSender("User", typeof(User).ToString());

            _dbAdapter.Send(request);
        }

        public IEnumerable<Photography> GetPhotographiesByTags(int userId, string tags, int pageId = 1)
        {
            //throw new ApplicationException($"uspGetPhotographiesByTags('{userId}','{tags}','{pageId}','{PhotoService.PageSize}')");
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetPhotographiesByTags", $"uspGetPhotographiesByTags('{userId}','{tags}','{pageId}','{PhotoService.PageSize}')"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            return MapPhotographyListFromDatabaseReplyToEntityModel(userId, reply);
        }
    }
}
