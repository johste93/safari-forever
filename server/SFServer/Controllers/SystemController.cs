using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using SFServer.Contexts;
using SFServer.Models;
using System.IO;
using Microsoft.Extensions.Configuration;
using SFServer.Models.Configs;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SFServer.Models.DB;
using SFServer.Services;
using SFServer.Models.Transaction;
using SFServer.Models.Notifications;
using SFServer.Models.DailyChallenge;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SFServer.Controllers
{
    [Route("[controller]")]
    public class SystemController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly VersionConfig _versionConfig;

        private readonly PostgreSqlContext _db;

        private TransactionService _transactionService;
        private NotificationService _notificationService;
        private DailyChallengeService _dailyChallengeService;

        public SystemController(PostgreSqlContext db, IWebHostEnvironment env, IOptions<VersionConfig> versionConfig, TransactionService transactionService, NotificationService notificationService, DailyChallengeService dailyChallengeService)
        {
            _db = db;
            _env = env;
            _versionConfig = versionConfig.Value;
            _transactionService = transactionService;
            _notificationService = notificationService;
            _dailyChallengeService = dailyChallengeService;
        }

        [HttpGet]
        [Route("~/api")]
        public string Home()
        {
            Console.WriteLine("Power! Unlimited power!");
            Console.WriteLine(_versionConfig.PrivacyPolicyUpdated);
            return $"Power! Unlimited power!\n\nTerms Of Service Agreement Updated: {_versionConfig.TermsOfServiceAgreementUpdated}\nPrivacy Policy Updated: {_versionConfig.PrivacyPolicyUpdated}";
        }

        [HttpGet]
        [Route("~/heartbeat")]
        public ActionResult HeartBeat()
        {
            return Ok(DateTimeOffset.UtcNow.ToString("o"));
        }

        [HttpGet]
        [Route("~/client/version")]
        public ActionResult LatestAppVersion()
        {
            return Ok(_versionConfig.RequiredVersion);
        }

        [HttpGet]
        [Route("~/time")]
        public ActionResult ServerTime()
        {
            return Ok(DateTimeOffset.UtcNow.ToString("o"));
        }

        [HttpGet]
        [Route("~/currentdir")]
        public ActionResult GetDir()
        {
            return Ok(Directory.GetCurrentDirectory());
        }

        [HttpGet]
        [Route("~/.well-known/apple-app-site-association")]
        public ActionResult AppleAppSiteAssociation()
        {
            var file = Path.Combine(_env.WebRootPath, ".well-known", "apple-app-site-association");
            return PhysicalFile(file, "application/json");
        }

        [HttpGet]
        [Route("~/.well-known/assetlinks.json")]
        public ActionResult AndroidAssetLinks()
        {
            var file = Path.Combine(_env.WebRootPath, ".well-known", "assetlinks.json");
            return PhysicalFile(file, "application/json");
        }

        [HttpGet]
        [Route("~/user-no-longer-exsist")]
        public ActionResult UserNoLongerExsist()
        {
            return BadRequest("User No Longer Exsist");
        }

        /*
        [HttpGet]
        [Authorize(Roles = "Player,Admin,Moderator")]
        [Route("~/test-notification")]
        public async Task<ActionResult> TestNotification()
        {
            User user = (User)Request.HttpContext.Items["user"];

            string title = $"Someone you follow published a level";
            string body = $"{user.Nickname} published \"Quivering Pagan\"";
            string shareUrl = "https://play.safariforever.com/cyVRdt";

            await _notificationService.CreateNotification(user.UserId, title, body, NotificationType.FollowedPublishedLevel, shareUrl, new List<NotificationLink>() { new NotificationLink("View Level", shareUrl) });
            await _db.SaveChangesAsync();

            return Ok();
        }
        */
        /*
        [HttpGet]
        [Route("~/migrate")]
        public async Task<ActionResult> Migrate([FromForm] string password)
        {
            if (password != "[REDACTED]")
                return Unauthorized();

            DateTime yesterday = DateTime.UtcNow.AddDays(-1);
            List<DailyChallenge> challenges = await _db.DailyChallenges
                .Where(x => x.CreatedOn.Date < yesterday)
                .ToListAsync();

            foreach(DailyChallenge challenge in challenges)
            {
                DailyChallengeParticipant participant = await _dailyChallengeService.GetTodaysChampion(challenge);
                if (participant == null)
                    continue;

                User winner = _db.Users.Where(x => x.UserId == participant.UserId).FirstOrDefault();
                if (winner == null)
                    continue;

                winner.DailyChallengesWon++;
                _db.UpdateUser(winner);
            }

            await _db.SaveChangesAsync();

            return Ok("Migration Complete");
        }
        */
        /*
        [HttpGet]
        [Route("~/migrate")]
        public async Task<ActionResult> Migrate([FromForm] string password)
        {
            if (password != "[REDACTED]")
                return Unauthorized();

            await _db.LevelUserStats.ForEachAsync(x =>
            {
                x.HighscoreUpdatedOn = x.UpdatedOn;
            });

            await _db.SaveChangesAsync();

            return Ok("Migration Complete");
        }
        */

    }
}
