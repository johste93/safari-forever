using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SFServer.Contexts;
using SFServer.Models.DB;
using SFServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
namespace SFServer.Filters
{
    public class LegalVerificationAttribute : ActionFilterAttribute
    {
        private PostgreSqlContext _db;
        private DateTimeOffset _lastTermsOfServiceUpdate;
        private DateTimeOffset _lastPrivacyPolicyUpdate;

        public LegalVerificationAttribute(PostgreSqlContext db)
        {
            _db = db;

        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            if (filterContext.Result != null) return;   //Another filter has already returned a result so pass it on

            User user = (User)filterContext.HttpContext.Request.HttpContext.Items["user"];

            //Temp code
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

            if (clientVersion.IsNewerThanOrEqual(new ClientVersion(0, 90)))
            {
                TermsOfServiceAgreement tOS = await _db.TermsOfServiceAgreement.Where(x => x.UserId == user.UserId).OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync();

                if (tOS == null || tOS.Agreed == false)
                {
                    filterContext.Result = new ContentResult()
                    {
                        StatusCode = 403,
                        Content = "User has not agreed to ToS: Filter"
                    };
                    return;
                }

                PrivacyPolicyAgreement pPA = await _db.PrivacyPolicyAgreement.Where(x => x.UserId == user.UserId).OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync();

                if (pPA == null || pPA.Agreed == false)
                {
                    filterContext.Result = new ContentResult()
                    {
                        StatusCode = 403,
                        Content = "User has not agreed to Privacy Policy"
                    };
                    return;
                }
            }
            else
            {
                if (!user.AgreedToTermsOfService.HasValue)
                {
                    filterContext.Result = new ContentResult()
                    {
                        StatusCode = 403,
                        Content = "User has not agreed to ToS"
                    };
                    return;
                }
            }

            await next();
        }
    }
}
