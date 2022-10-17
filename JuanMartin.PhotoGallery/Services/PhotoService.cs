
using JuanMartin.Kernel;
using JuanMartin.Kernel.Adapters;
using JuanMartin.Kernel.Messaging;
using JuanMartin.Models.Gallery;
using JuanMartin.PhotoGallery.Models;
using Microsoft.Extensions.Configuration;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using static JuanMartin.PhotoGallery.Services.IPhotoService;
using Order = JuanMartin.Models.Gallery.Order;

namespace JuanMartin.PhotoGallery.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IExchangeRequestReply _dbAdapter;
        
        public int PageSize { get; set; }

        // # pages in a block
        public int BlockSize { get; set; }

        public PhotoService()
        {
            _dbAdapter = new AdapterMySql(Startup.ConnectionString);
        }
        public PhotoService(IConfiguration configuration) : this()
        {
            BlockSize = Convert.ToInt32(configuration["GalleryBlockSize"]);
            PageSize = Convert.ToInt32(configuration["GalleryPageSize"]);
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

            request.AddData(new ValueHolder("PhotoGraphies", $"uspGetAllPhotographies({pageId},{this.PageSize},{userId})"));
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

        public IRecordSet ExecuteSqlStatement(string statement)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.Text.ToString());

            request.AddData(new ValueHolder("GetPhotographies", statement));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);

            return (IRecordSet)_dbAdapter.Receive();
        }

        public (string ImageIdsList, long RowCount) GetPhotographyIdsList(int userID, ImageListSource source, string searchQuery, int OrderId)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetPhotographyIdsList", $"uspGetPhotographyIdsList('{userID}','{(int)source}','{searchQuery}','{OrderId}')"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            var galleryIdsList = (string)reply.Data.GetAnnotationByValue(1).GetAnnotation("Ids").Value;
            var rowCount = (long)reply.Data.GetAnnotationByValue(1).GetAnnotation("RowCount").Value;

            return ($",{galleryIdsList},", rowCount);
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

            return (photographies.Count == 0) ? null : photographies[0];
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
        public int UpdatePhotographyDetails(long id, int userId, string location)
        {
            if (userId == -1)
                return -1;

            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspUpdatePhotographyDetails", $"uspUpdatePhotographyDetails('{userId}','{id}','{location}')"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            // be sure record.GetAnnotation("...") exists and is not null
            var locationId = (int)reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value;

            return locationId;
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
                        RouteData = queryDic
                    };
                }
            }

            return requestModel;
        }

        public RedirectResponseModel SetRedirectInfo(int userId, string remoteHost, string controller, string action, long routeId = -1, string queryString = "")
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
                    errorCode = (int)record.GetAnnotation("ErrorCode").Value;
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

        private static Order MapOrderFromDatabaseReplyToEntityModel(int userId, IRecordSet reply)
        {
            Order order= null;

            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder record in reply.Data.Annotations)
                {
                    // be sure record.GetAnnotation("...") exists and is not null
                    int id = Convert.ToInt32(record.GetAnnotation("Id").Value);

                    string n = (string)record.GetAnnotation("Number").Value;
                    if (string.IsNullOrEmpty(n))
                        n = Guid.Empty.ToString();

                    Guid number = Guid.Parse(n);
                    DateTime createdDtm = Convert.ToDateTime(record.GetAnnotation("CreatedDtm").Value);
                    string statusString = (string)record.GetAnnotation("Status").Value;
                    int count = Convert.ToInt32(record.GetAnnotation("Count").Value);

                    Order.OrderStatus status;
                    switch (statusString)
                    {
                        case "inProcess":
                            {
                                status = Order.OrderStatus.inProcess;
                                break;
                            }
                        case "complete":
                            {
                                status = Order.OrderStatus.complete;
                                break;
                            }
                        default:
                            {
                                status = Order.OrderStatus.pending;
                                break;
                            }
                    }

                    order = new Order(id, userId, number, createdDtm, count, status);
                }
            }

            return order;
        }


        private static List<Photography> MapPhotographyListFromDatabaseReplyToEntityModel(int userId, IRecordSet reply)
        {
            var photographies = new List<Photography>();

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

        public int RemoveTag(int userId, string tag, long id)
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

        public void AddAuditMessage(int userId, string meessage, string source = "", int isError = 0)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspAddAuditMessage", $"uspAddAuditMessage('{userId}','{meessage}','{source}','{isError}'"));
            request.AddSender("User", typeof(User).ToString());

            _dbAdapter.Send(request);
        }

        public IEnumerable<Photography> GetPhotographiesBySearch(int userId, string query, int pageId = 1)
        {
            //throw new ApplicationException($"uspGetPhotographiesBySearch('{userId}','{query}','{pageId}','{PhotoService.PageSize}')");
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetPhotographiesBySearch", $"uspGetPhotographiesBySearch('{userId}','{query}','{pageId}','{this.PageSize}')"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            return MapPhotographyListFromDatabaseReplyToEntityModel(userId, reply);
        }

        public IEnumerable<string> GetAllTags(int pageId = 1)
        {
            //throw new ApplicationException($"uspGetPhotographiesBySearch('{userId}','{tags}','{pageId}','{this.PageSize}')");
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetTags", $"uspGetTags('{pageId}','{this.PageSize}')"));
            request.AddSender("Tag", typeof(string).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder record in reply.Data.Annotations)
                {
                    var tag = (string)record.GetAnnotation("Tag").Value;

                    yield return tag;
                }
            }
        }

        public IEnumerable<string> GetAllLocations(int pageId = 1)
        {
            //throw new ApplicationException($"uspGetPhotographiesBySearch('{userId}','{locations}','{pageId}','{this.PageSize}')");
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetLocations", $"uspGetLocations('{pageId}','{this.PageSize}')"));
            request.AddSender("Location", typeof(string).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder record in reply.Data.Annotations)
                {
                    var location = (string)record.GetAnnotation("Location").Value;

                    yield return location;
                }
            }
        }

        public Order GetCurrentActiveOrder(int userId)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetCurrentActiveOrder", $"uspGetCurrentActiveOrder('{userId}')"));
            request.AddSender("Order", typeof(Order).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            return MapOrderFromDatabaseReplyToEntityModel(userId, reply);
        }

        public Order GetOrder(int userId, int orderId)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetOrder", $"uspGetOrder('{orderId}','{userId}','-1')"));
            request.AddSender("Order", typeof(Order).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            return MapOrderFromDatabaseReplyToEntityModel(userId, reply);
        }

        public Order AddOrder(int userId)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspAddOrder", $"uspAddOrder('{userId}')"));
            request.AddSender("Order", typeof(Order).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            return MapOrderFromDatabaseReplyToEntityModel(userId, reply);
        }

        public bool IsPhotographyInOrder(int orerId, long photographyId, int userId)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspIsPhotographyInOrder", $"uspIsPhotographyInOrder('{orerId}','{photographyId}','{userId}')"));
            request.AddSender("Order", typeof(Order).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            return (reply != null && reply.Data != null);
        }

        public IEnumerable<Photography> GetOrderPhotographies(int userId, int orderId, int pageId = 1)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspGetOrderPhotographies", $"uspGetOrderPhotographies('{userId}','{orderId}','{pageId}','{this.PageSize}')"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            return MapPhotographyListFromDatabaseReplyToEntityModel(userId, reply);
        }

        public int AddPhotographyToOrder(long id, int orderId, int userId)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspAddPhotographyToOrder", $"uspAddPhotographyToOrder('{id}','{orderId}',{userId})"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            var Id = Convert.ToInt32(reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value);

            return Id;
        }

        public int RemovePhotographyFromOrder(long id, int orderId, int userId)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspRemovePhotographyFromOrder", $"uspRemovePhotographyFromOrder('{id}','{orderId}',{userId})"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            var Id = Convert.ToInt32(reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value);

            return Id;
        }

        public int RemoveOrder(int orderId, int userId)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspRemoveOrder", $"uspRemoveOrder('{orderId}',{userId})"));
            request.AddSender("Photography", typeof(Photography).ToString());

            _dbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)_dbAdapter.Receive();

            var Id = Convert.ToInt32(reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value);

            return Id;
        }

        /// <summary>
        /// Update index of all images in order.
        /// </summary>
        /// <param name="indices">Comma separated list of images and their indices
        /// within the order, like: id:index,id:index,id:index,....</param>
        public bool UpdateOrderItemsIndices(int userId, int orderId, GalleryIndexViewModel model)
        {
            if (model == null && !string.IsNullOrEmpty(model.CartItemsSequence)) return false;

            var listOfImagesIndexInOrder = model.CartItemsSequence.Split(',')
              .Select(s => s.Split(':'))
              .ToDictionary(a => Convert.ToInt64(a[0].Trim()), a => Convert.ToInt32(a[1].Trim()));

            foreach (var index in listOfImagesIndexInOrder)
            {
                UpdateOrderIndex(userId, orderId, index.Key, index.Value);
            }

            return true;
        }

        public void UpdateOrderIndex(int userId, int orderId, long photographyId, int index)
        {
            if (_dbAdapter == null)
                throw new ApplicationException("MySql connection not set.");

            Message request = new("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("uspUpdateOrderIndex", $"uspUpdateOrderIndex('{userId}','{orderId}','{photographyId}','{index}')"));
            request.AddSender("Order", typeof(Order).ToString());

            _dbAdapter.Send(request);
        }
    }
}