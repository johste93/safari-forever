using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Mvc;
using SFServer.Discord;
using SFServer.Filters;
using SFServer.Services;
using Newtonsoft.Json;
using SFServer.Models.DailyChallenge;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using SFServer.Models.DB;
using SFServer.Security;
using SFServer.Models.Notifications;
using SFServer.Contexts;
using Microsoft.EntityFrameworkCore;
using SFServer.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SFServer.Controllers
{
    [Route("[controller]")]
    public class MiscController : Controller
    {
        private readonly PostgreSqlContext _db;
        private DiscordClient _discordClient;
        private DailyChallengeService _dailyChallengeService;
        private NewLevelOfTheWeekService _newLevelOfTheWeekService;
        private NotificationService _notificationService;

        public MiscController(PostgreSqlContext db, DiscordClient discordClient, DailyChallengeService dailyChallengeService, NewLevelOfTheWeekService newLevelOfTheWeekService, NotificationService notificationService)
        {
            _db = db;
            _discordClient = discordClient;
            _dailyChallengeService = dailyChallengeService;
            _newLevelOfTheWeekService = newLevelOfTheWeekService;
            _notificationService = notificationService;
        }
        /*
        [ModelValidation]
        [HttpGet("~/sunnfolk/{username}")]
        public async Task<IActionResult> JoinSunnhordalandRole(string username)
        {
            var guild = await _discordClient.GetGuildAsync(449014097175117854);
            var members = await guild.GetAllMembersAsync();

            DiscordMember user = null;
            foreach (DiscordMember member in members)
            {
                Console.WriteLine($"{member.Username.ToLower()}#{member.Discriminator}");
                if ($"{member.Username.ToLower()}#{member.Discriminator}" == username)
                {
                    user = member;
                }
            }

            if (user == null)
                return BadRequest($"{username} is not a member of Safari Forever Community.");

            if (user.IsOwner)
                return Forbid();

            DiscordRole role = guild.GetRole(624207321815580702);

            await user.GrantRoleAsync(role);

            return Ok("Done!");
        }
        */
        /*
        [HttpGet("~/sunnfolk")]
        public ActionResult SunnhordalandFHS()
        {
            return View("Views/Misc/sunnfolk.cshtml");
        }
        */

        [HttpGet("~/daily/{dailyChallengeId}")]
        public async Task<ActionResult> GetDailyChallengeResults(string dailyChallengeId)
        {
            DailyChallenge challenge = await _dailyChallengeService.GetChallengeById(dailyChallengeId);

            if (challenge == null)
                return NotFound("Challenge not found");

            List<DailyChallengeParticipant> participants = await _dailyChallengeService.GetParticipants(challenge);

            if (participants == null)
                return NotFound("Participants not found");

            if (participants.Count == 0)
                return Ok("There was no winner.");

            int rank = 1;

            int numberOfGroups = (int)Math.Floor(participants.Count / 10f) + 1;
            if (participants.Count % 10 == 0)
                numberOfGroups--;

            StringBuilder rankings = new StringBuilder();

            for (int groupIndex = 0; groupIndex < numberOfGroups; groupIndex++)
            {
                int participantsInGroup = Math.Min(participants.Count - (groupIndex * 10), 10);

                for (int participantIndex = (groupIndex * 10); participantIndex < (groupIndex * 10) + participantsInGroup; participantIndex++)
                {
                    Highscore score = new Highscore(participants[participantIndex].Seconds, participants[participantIndex].Milliseconds);
                    string entry = $"{rank}. {participants[participantIndex].Nickname} - {score.ToString()}";
                    int spaceCount = 28 - entry.Length;

                    string whitespace = "";
                    for (int i = 0; i < spaceCount; i++)
                        whitespace += " ";

                    rankings.Append($"{rank}. {participants[participantIndex].Nickname}{whitespace}{score.ToString()}\n");
                    rank++;
                }

                rankings.Append("\n");
            }

            return Ok(rankings.ToString());
        }

        private string FormatTime(double input)
        {
            int seconds = (int)Math.Floor(input);
            int miliseconds = (int)Math.Floor((input - ((int)Math.Floor(input))) * 100);

            return $"{seconds.ToString("D2")}:{miliseconds.ToString("D2")}";
        }

        /*
        [Authorize(Roles = "Player,Admin,Moderator")]
        [TermsOfServiceVerification]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [ModelValidation]
        [HttpGet("~/test-notification")]
        public async Task<IActionResult> TestNotification()
        {
            User user = (User)Request.HttpContext.Items["user"];
            Token token = (Token)Request.HttpContext.Items["token"];

            await _notificationService.CreateNotification(user.UserId, "Hello World", "This is a test!", Models.Notifications.NotificationType.Unknown);

            return Ok();
        }
        */

        /*
        [HttpGet("post-new-level-of-the-week")]
        public async Task<ActionResult> PostLevelOfTheWeek()
        {
            await _newLevelOfTheWeekService.PostNewLevelOfTheWeek();

            return Ok();
        }
        
        [HttpGet("generate-daily-challenge")]
        public async Task<ActionResult> GenerateDailyChallenge()
        {
            await _dailyChallengeService.GenerateNewDailyChallenge();

            return Ok();
        }

        [HttpGet("get-daily-challenge")]
        public async Task<ActionResult> GetDailyChallenge()
        {
            var challenge = await _dailyChallengeService.GetCurrentChallenge();
            var json = JsonConvert.SerializeObject(challenge);

            return Ok(json);
        }

        [HttpGet("daily-challenge-generated")]
        public async Task<ActionResult> ChallengeAlreadyGenerated()
        {
            var generated = await _dailyChallengeService.ChallengeAlreadyGenerated();
            return Ok(generated);
        }


        [ModelValidation]
        [HttpGet("post-daily-challenge-results/{challengeId}")]
        public async Task<ActionResult> PostDailyChallengeResults(string challengeId)
        {
            await _dailyChallengeService.PostResults(challengeId);

            return Ok();
        }
        */

        [HttpGet("~/test-webrequest")]
        public async Task<IActionResult> TestWebRequest()
        {
            await Task.Delay(10000);

            return Ok("Hello World!");
        }
    }
}
