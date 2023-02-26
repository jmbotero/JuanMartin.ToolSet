using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using JuanMartin.Models.Gallery;
using JuanMartin.Kernel;
using JuanMartin.Kernel.Adapters;
using JuanMartin.Kernel.Messaging;
using JuanMartin.Kernel.Utilities;
using JuanMartin.PhotoGallery.Models;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace JuanMartin.PhotoGallery.Services
{
    public interface IPhotoService
    {
        enum ImageListSource
        {
            gallery = 0,
            searchResults = 1,
            shoppingCart = 2
        };

        int BlockSize { get; set; }
        int PageSize { get; set; }
        int MobilePageSize { get; set; }

        void AddAuditMessage(int useerId, string meessage, string source = "", int isError = 0);
        User VerifyEmail(string email);
        void StoreActivationCode(int userId, string activationCode);
        (int, User) VerifyActivationCode(string activationCode);
        User UpdateUserPassword(int userId, string userName, string password);
        User AddUser(string userName, string password, string email);
        int LoadSession(int userId);
        void EndSession(int sessionId);
        RedirectResponseModel GetRedirectInfo(int userId,string remoteHost);
        Dictionary<string, object> GenerateRouteValues(long routeId, string queryString, Dictionary<string, string> additionalQueryStringItems = null);
        RedirectResponseModel SetRedirectInfo(int userId,string remoteHost, string controller, string action, long routeId = -1, string queryString = "");
        void ConnectUserAndRemoteHost(int userId, string remoteHost);
        User GetUser(string userName, string password);
        int GetGalleryPageCount(int pageSize);
        public (string ImageIdsList, long RowCount) GetPhotographyIdsList(int userID, ImageListSource source, string searchQuery, int OrderId);
        IEnumerable<Photography> GetAllPhotographies(int userId, int pageId = 1);
        IEnumerable<Photography> GetPhotographiesBySearch(int userId, string query, int pageId = 1);
        Photography GetPhotographyById(long id, int userId);
        int UpdatePhotographyRanking(long id, int userId, int rank);
        int UpdatePhotographyDetails(long id, int userId, string location);
        int AddTag(int userId, string tag, long id);
        int AddTag(string connectionString, int userId, string tag, long id);
        void AddTags(string connectionString, int userId, string tags, IEnumerable<Photography> photographies);
        int RemoveTag(int userId, string tag, long id);
        IEnumerable<string> GetAllTags(int pageId = 1);
        IEnumerable<string> GetAllLocations(int pageId = 1);
        IRecordSet ExecuteSqlStatement(string statement);
        Order GetCurrentActiveOrder(int userId);
        Order GetOrder(int userId, int OrderId);
        Order AddOrder(int userId);
        int RemoveOrder(int orderId, int userId);
        bool IsPhotographyInOrder(int orerId, long photographyId, int userId);
        IEnumerable<Photography> GetOrderPhotographies(int userId, int orderId, int pageId = 1);
        int AddPhotographyToOrder(long id, int orderId, int userId);
        int RemovePhotographyFromOrder(long id, int orderId, int userId);
        bool UpdateOrderItemsIndices(int userId, int orderId, GalleryIndexViewModel model);
        void UpdateOrderIndex(int userId, int orderId, long photographyId, int index);
        IEnumerable<Photography> LoadPhotographies(string connectionString, string directory, string acceptedExtensions, bool directoryIsLink);
        IEnumerable<Photography> LoadPhotographiesWithLocation(string connectionString, string directory, string acceptedExtensions, bool directoryIsLink, int userId, string location);
        long AddPhotography(AdapterMySql dbAdapter, string name, string path, string title);
    }
}
