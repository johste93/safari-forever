using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SFServer.Models.Configs;
using SFServer.Utility;
using System;

namespace SFServer.Filters
{
    public class ClientVersionValidationAttribute : ActionFilterAttribute
    {
        private VersionConfig _versionConfig;

        public ClientVersionValidationAttribute(IOptions<VersionConfig> versionConfig)
        {
            if (versionConfig == null)
            {
                Console.WriteLine("ERROR, Unable to parse Client Version");
                return;
            }

            _versionConfig = versionConfig.Value;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Result != null) return;   //Another filter has already returned a result so pass it on

            string version = filterContext.HttpContext.Request.Headers["ClientVersion"];

            if (string.IsNullOrWhiteSpace(version))
            {
                filterContext.Result = new ContentResult()
                {
                    StatusCode = 400,
                    Content = "Client version missing"
                };
                return;
            }

            if (!ClientVersion.Parse(version, out ClientVersion clientVersion))
            {
                filterContext.Result = new ContentResult()
                {
                    StatusCode = 400,
                    Content = "'ClientVersion' has bad SYNTAX"
                };
                return;
            }

            if (!clientVersion.IsNewerThanOrEqual(_versionConfig.requiredVersion))
            {
                filterContext.Result = new ContentResult()
                {
                    StatusCode = 403,
                    Content = "Client outdated, Please update"
                };
                return;
            }
        }
    }
}
