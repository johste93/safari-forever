using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFServer.Contexts;
using SFServer.Models.DB;
using SFServer.Security;
using Microsoft.AspNetCore.Authorization;
using SFServer.Utility;
using RestSharp;
using Microsoft.EntityFrameworkCore;
using SFServer.Models.Notifications;
using SFServer.Filters;
using SFServer.Models.Requests.NotificationRequests;
using SFServer.Models.Responses.NotificationResponses;
using Microsoft.Extensions.Options;
using SFServer.Configs;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SFServer.Controllers
{
    [Route("[controller]")]
    public class NotificationController : Controller
    {
        private readonly PostgreSqlContext _db;
        private readonly GameConfig _gameConfig;

        public NotificationController(PostgreSqlContext db, IOptions<GameConfig> gameConfig)
        {
            _db = db;
            _gameConfig = gameConfig.Value;
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("peek")]
        public async Task<IActionResult> PeekNew()
        {
            User user = (User)Request.HttpContext.Items["user"];

            int newCount = await _db.Notifications.CountAsync(x => x.UserId == user.UserId && x.Read == false);

            return Ok(newCount);
        }

        //Remove this one once 0.96 is out.
        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("get")]
        public async Task<IActionResult> GetNotificationsOld([FromHeader] int fromIndex)
        {
            User user = (User)Request.HttpContext.Items["user"];

            List<Notification> result = await _db.Notifications.Include(x => x.Links).Where(x => x.UserId == user.UserId).OrderByDescending(x => x.CreatedOn).Skip(fromIndex).Take(10).ToListAsync();

            return Ok(result);
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("get-latest")]
        public async Task<IActionResult> GetNotifications([FromHeader] int fromIndex)
        {
            User user = (User)Request.HttpContext.Items["user"];

            List<Notification> result = await _db.Notifications.Include(x => x.Links).Where(x => x.UserId == user.UserId).OrderByDescending(x => x.CreatedOn).Skip(fromIndex).Take(_gameConfig.NotificationsPrPage).ToListAsync();

            return Ok(new NotificationFeedResponse() { NotifcationsPrPage = _gameConfig.NotificationsPrPage, Notifications = result });
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPatch("mark-as-read")]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadRequest request)
        {
            await _db.Notifications.Where(x => request.ReadNotificationsIds.Contains(x.NotificationId)).ForEachAsync(x => x.Read = true);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPatch("toggle-notifications")]
        public async Task<IActionResult> ToggleNotificationType([FromBody] ToggleNotificationsRequest request)
        {
            User user = (User)Request.HttpContext.Items["user"];

            if(request.Notification_DailyChallengeResults.HasValue)
                user.Notification_DailyChallengeResults = request.Notification_DailyChallengeResults.Value;

            if (request.Notification_FollowedPlayedLevel.HasValue)
                user.Notification_FollowedPlayedLevel = request.Notification_FollowedPlayedLevel.Value;

            if (request.Notification_FollowedPlayedLevel.HasValue)
                user.Notification_FollowedLikedLevel = request.Notification_FollowedLikedLevel.Value;

            if (request.Notification_FollowedPublishedLevel.HasValue)
                user.Notification_FollowedPublishedLevel = request.Notification_FollowedPublishedLevel.Value;

            if (request.Notification_MyLevelIsLevelOfTheWeek.HasValue)
                user.Notification_MyLevelIsLevelOfTheWeek = request.Notification_MyLevelIsLevelOfTheWeek.Value;

            if (request.Notification_NewDailyChallenge.HasValue)
                user.Notification_NewDailyChallenge = request.Notification_NewDailyChallenge.Value;

            if (request.Notification_NewFollower.HasValue)
                user.Notification_NewFollower = request.Notification_NewFollower.Value;

            if (request.Notification_WorldRecordBeaten.HasValue)
                user.Notification_WorldRecordBeaten = request.Notification_WorldRecordBeaten.Value;

            _db.UpdateUser(user);
            
            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("status")]
        public IActionResult GetPushNotificationStatus()
        {
            User user = (User)Request.HttpContext.Items["user"];

            PushNotificationStatusResponse response = new PushNotificationStatusResponse()
            {
                Notification_DailyChallengeResults = user.Notification_DailyChallengeResults,
                Notification_FollowedPlayedLevel = user.Notification_FollowedPlayedLevel,
                Notification_FollowedLikedLevel = user.Notification_FollowedLikedLevel,
                Notification_FollowedPublishedLevel = user.Notification_FollowedPublishedLevel,
                Notification_MyLevelIsLevelOfTheWeek = user.Notification_MyLevelIsLevelOfTheWeek,
                Notification_NewDailyChallenge = user.Notification_NewDailyChallenge,
                Notification_NewFollower = user.Notification_NewFollower,
                Notification_WorldRecordBeaten = user.Notification_WorldRecordBeaten
            };

            return Ok(response);
        }
    }
}
