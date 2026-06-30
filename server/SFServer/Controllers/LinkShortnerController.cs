using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFServer.Filters;

namespace SFServer.Controllers
{
    [Route("[controller]")]
    public class LinkShortnerController : Controller
    {
        [HttpGet]
        [Route("~/g/{id}")]
        public ActionResult GifRedirect(string id)
        {
            return RedirectPermanentPreserveMethod($"https://media.giphy.com/media/{id}/giphy.gif");
        }

        [HttpGet]
        [Route("~/u/{nickname}/{id}")]
        public ActionResult UserRedirect(string nickname, string id)
        {
            return RedirectPermanentPreserveMethod($"https://user.safariforever.com/{nickname}/{id}");
        }

        [HttpGet]
        [Route("~/l/{id}")]
        public ActionResult LevelRedirect(string id)
        {
            return RedirectPermanentPreserveMethod($"https://play.safariforever.com/{id}");
        }
    }
}
