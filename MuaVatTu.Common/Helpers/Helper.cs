using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuaVatTu.Common.Helpers;
using System;
using System.Collections.Generic;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;

namespace MuaVatTu.Common
{
    public class Helper
    {
        /// <summary>
        /// Transform data to http response
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ActionResult TransformData(Response data)
        {
            var result = new ObjectResult(data) { StatusCode = (int)data.Code };
            return result;
        }

        /// <summary>
        /// Get user info in token and headder
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static RequestUser GetRequestInfo(HttpRequest request)
        {
            throw new NotImplementedException();
        }
        public class RequestUser
        {
            public Guid UserId { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string PhoneNumber { get; set; }
            public Guid ApplicationId { get; set; }
            public List<string> ListApps { get; set; }
            public List<string> ListRoles { get; set; }
            public List<string> ListRights { get; set; }
            public bool isAdmin { get; set; }
            public bool isStaff { get; set; }
            public int Level { get; set; }
            public string Language { get; set; }
            public string Currency { get; set; }
        }
    }
}