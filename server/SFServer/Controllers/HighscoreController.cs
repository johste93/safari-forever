using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFServer.Contexts;
using Microsoft.AspNetCore.Authorization;
using SFServer.Models.DB;
using SFServer.Services;
using SFServer.Security;
using Microsoft.EntityFrameworkCore;
using SFServer.Filters;
using System.Globalization;
using SFServer.Models.Requests.Highscore;
using SFServer.Utility;

namespace SFServer.Controllers
{
    [Route("[controller]")]
    public class HighscoreController : Controller
    {
        private readonly PostgreSqlContext _db;
        private NotificationService _notificationService;
        private ClientSecretAuthenticator _clientSecretAuthenticator;
        private TransactionService _transactionService;

        public HighscoreController(PostgreSqlContext db, NotificationService notificationService, TransactionService transactionService, ClientSecretAuthenticator clientSecretAuthenticator)
        {
            _db = db;
            _notificationService = notificationService;
            _transactionService = transactionService;
            _clientSecretAuthenticator = clientSecretAuthenticator;
        }

        
        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ModelValidation]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [ServiceFilter(typeof(ClientSecretValidationAttribute))]
        [HttpPost("campaign/{world}/{index}/upload")]
        public async Task<IActionResult> UploadCampaignTime(int world, int index, [FromBody] UploadCampaignTimeRequest request)
        {
            if (!request.Seconds.HasValue)
                return BadRequest("Seconds Missing");

            if (!request.Milliseconds.HasValue)
                return BadRequest("Milliseconds Missing");

            Highscore highscore = new Highscore(request.Seconds.Value, request.Milliseconds.Value);

            if (highscore.Seconds + highscore.Milliseconds <= 0)
                return Forbid();

            User user = (User)Request.HttpContext.Items["user"];

            if(_db.CampaignTimes.Any(x => x.UserId == user.UserId && x.World == world && x.LevelIndex == index))
            {
                CampaignTime exsistingTime = _db.CampaignTimes.FirstOrDefault(x => x.UserId == user.UserId && x.World == world && x.LevelIndex == index);

                //If level has no highscore or if seconds are less than exsisting time seconds OR seconds are equal, but milliseconds are less than exsisting seconds.

                Highscore exsistingHighscore = new Highscore(exsistingTime.Seconds, exsistingTime.Milliseconds);
                if (highscore.IsLowerThan(exsistingHighscore))
                {
                    exsistingTime.Seconds = highscore.Seconds;
                    exsistingTime.Milliseconds = highscore.Milliseconds;
                }

                _db.UpdateCampaignTime(exsistingTime);
            }
            else
            {
                //First time user completes level
                //await _transactionService.AddCoins(user, _transactionService.rewardConfig.Value.CompletedCampaignLevel, "Completed campaign level."); //Disabled singleplayer rewards.

                _db.CampaignTimes.Add(new CampaignTime()
                {
                    LevelIndex = index,
                    World = world,
                    Seconds = highscore.Seconds,
                    Milliseconds = highscore.Milliseconds,
                    UserId = user.UserId
                });
            }

            await _db.SaveChangesAsync();
            return Ok();
        }

        
        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("campaign/{world}/{index}/average")]
        public async Task<IActionResult> GetAverage(int world, int index)
        {
            var bestTimes = await _db.CampaignTimes.Where(x => x.World == world && x.LevelIndex == index).Select(x => new { x.Seconds, x.Milliseconds }).ToListAsync();

            if (bestTimes.Count == 0)
                return Ok(-1);

            int millisecondsSum = bestTimes.Where(x => x.Milliseconds > 0).Sum(x => x.Milliseconds);

            int restSeconds = (int) Math.Floor(millisecondsSum / 100f);

            millisecondsSum -= (restSeconds * 100);

            int secondsSum = bestTimes.Where(x => x.Seconds > 0).Sum(x => x.Seconds) + restSeconds;

            if (Double.TryParse($"{secondsSum}.{millisecondsSum}", out double sum))
            {
                Console.WriteLine("Error: failed to parse average time sum.");
                return Ok(-1);
            }

            if (sum < 0)
                return Ok(-1);

            return Ok(sum / bestTimes.Count);
        }

       
        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("campaign/{world}/{index}/best")]
        public async Task<IActionResult> GetHighscore(int world, int index)
        {
            CampaignTime highscore = await _db.CampaignTimes.Where(x => x.World == world && x.LevelIndex == index).OrderBy(x => x.Seconds).ThenBy(x => x.Milliseconds).FirstOrDefaultAsync();

            if (highscore.Seconds < 0 || highscore.Milliseconds < 0)
                return Ok(-1);

            return Ok($"{highscore.Seconds}.{highscore.Milliseconds}");
        }
    }
}