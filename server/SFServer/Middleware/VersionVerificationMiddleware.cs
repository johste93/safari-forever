using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Utility;
using SFServer.Models.Configs;
using Microsoft.Extensions.Options;

namespace SFServer.Middleware
{
    public class VersionVerificationMiddleware
    {
        private readonly RequestDelegate _next;
        private VersionConfig _versionConfig;


        public VersionVerificationMiddleware(RequestDelegate next, IOptions<VersionConfig> versionConfig)
        {
            _next = next;

            if (versionConfig == null)
            {
                Console.WriteLine("ERROR, Unable to parse Client Version");
                return;
            }

            _versionConfig = versionConfig.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string version = context.Request.Headers["ClientVersion"];

            if (string.IsNullOrWhiteSpace(version))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Client version missing");
                return;
            }

            if(!ClientVersion.Parse(version, out ClientVersion clientVersion))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("'ClientVersion' has bad SYNTAX");
                return;
            }

            if (!clientVersion.IsNewerThanOrEqual(_versionConfig.requiredVersion))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Client outdated, Please update the game.");
                return;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
