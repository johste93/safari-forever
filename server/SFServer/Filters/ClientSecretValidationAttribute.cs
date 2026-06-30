using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFServer.Models;
using SFServer.Models.DTO;
using SFServer.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFServer.Filters
{
    public class ClientSecretValidationAttribute : ActionFilterAttribute
    {
        private ClientSecretAuthenticator _clientSecretAuthenticator;

        public ClientSecretValidationAttribute(ClientSecretAuthenticator clientSecretAuthenticator)
        {
            _clientSecretAuthenticator = clientSecretAuthenticator;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Result != null) return;   //Another filter has already returned a result so pass it on

            string ClientSecret = filterContext.HttpContext.Request.Headers["ClientSecret"];
            string ClientId = filterContext.HttpContext.Request.Headers["ClientId"];

            if (string.IsNullOrWhiteSpace(ClientSecret))
            {
                filterContext.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "ClientSecret is missing"
                };
                return;
            }

            if (string.IsNullOrWhiteSpace(ClientId))
            {
                filterContext.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "ClientId is missing"
                };
                return;
            }

            //Verify secret
            bool secretValid = _clientSecretAuthenticator.ValidateSecret(ClientSecret, ClientId);
            if (!secretValid)
            {
                filterContext.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}