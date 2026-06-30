using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SFServer.Configs;
using SFServer.Contexts;
using SFServer.Filters;
using SFServer.Models.Configs;
using SFServer.Models.DB;
using SFServer.Models.DTO;
using SFServer.Models.Responses.Endless;
using SFServer.Services;
using SFServer.Models.Enums;

namespace SFServer.Controllers
{
    [Route("[controller]")]
    public class EndlessController : Controller
    {
        private readonly PostgreSqlContext _db;
        private IWebHostEnvironment _env;
        private EndlessService _endlessService;
        private TransactionService _transactionService;
        private readonly GameConfig _gameConfig;

        public EndlessController(PostgreSqlContext db, IWebHostEnvironment env, EndlessService endlessService, TransactionService transactionService, IOptions<GameConfig> gameConfig)
        {
            _db = db;
            _env = env;
            _endlessService = endlessService;
            _transactionService = transactionService;
            _gameConfig = gameConfig.Value;
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("challenge")]
        public async Task<IActionResult> FetchChallenge()
        {
            User user = (User)Request.HttpContext.Items["user"];

            //Check if user already has a challenge
            EndlessChallenge challenge = await _endlessService.GetChallenge(user);

            //if not generate a challenge
            if (challenge == null || challenge.Completed || challenge.Skipped)
                challenge = await _endlessService.GenerateChallenge(user);

            if (challenge == null)
                return Ok(new EndlessChallengeResponse() { Error = EndlessError.NoMoreLevels, PersonalScore = user.EndlessScore, RerollCost = _gameConfig.EndlessChallengeSkipCost });

            Level level = await _db.Levels.Where(x => x.LevelId == challenge.LevelId).FirstOrDefaultAsync();

            if(level.Blacklisted)
                challenge = await _endlessService.GenerateChallenge(user);

            if (challenge == null)
                return Ok(new EndlessChallengeResponse() { Error = EndlessError.NoMoreLevels, PersonalScore = user.EndlessScore, RerollCost = _gameConfig.EndlessChallengeSkipCost });

            EndlessChallengeResponse response = await _endlessService.CreateResponse(challenge);
            response.PersonalScore = user.EndlessScore;
            response.RerollCost = _gameConfig.EndlessChallengeSkipCost;
            response.PublishedLevelMeta.ShareUrl = GetShareUrl(response.PublishedLevelMeta.LevelId);

            return Ok(response);
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("skip")]
        public async Task<IActionResult> SkipChallenge()
        {
            User user = (User)Request.HttpContext.Items["user"];

            //Check if user already has a challenge
            EndlessChallenge challenge = await _endlessService.GetChallenge(user);

            if (challenge == null)
                BadRequest("Have no challenge to skip");

            //Check if can afford to skip
            if (_gameConfig.EndlessChallengeSkipCost > user.Coins)
                return Ok(new EndlessChallengeResponse() { Error = EndlessError.CantAffordToSkip });

            await _endlessService.SkipChallenge(user);

            //Subtract cost
            await _transactionService.SpendCoins(user, _gameConfig.EndlessChallengeSkipCost, $"Skipped endless challenge: {challenge.endlessChallengeId}");

            challenge = await _endlessService.GenerateChallenge(user);

            EndlessChallengeResponse response = await _endlessService.CreateResponse(challenge);
            response.PersonalScore = user.EndlessScore;
            response.RerollCost = _gameConfig.EndlessChallengeSkipCost;
            response.PublishedLevelMeta.ShareUrl = GetShareUrl(response.PublishedLevelMeta.LevelId);

            return Ok(response);
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("leaderboard/{page}")]
        public async Task<IActionResult> FetchLeaderboard(int page)
        {
            User user = (User)Request.HttpContext.Items["user"];

            var leaderboard = await _db.Users
                .Where(x => !x.Banned && x.EndlessScore > 0)
                .OrderByDescending(x => x.EndlessScore)
                .ThenByDescending(x => x.EndlessScoreLastUpdated)
                .ToListAsync();

            int personalRank = -1;
            if (leaderboard.Any(x => x.UserId == user.UserId))
                personalRank = leaderboard.FindIndex(x => x.UserId == user.UserId) + 1;

            page = Math.Max(0, page);

            leaderboard = leaderboard
                .Skip(_gameConfig.EndlessLeaderboardUsersPrPage * page)
                .Take(_gameConfig.EndlessLeaderboardUsersPrPage)
                .ToList();

            List<EndlessLeaderboardDTO> result = leaderboard.Select((x, i) => new EndlessLeaderboardDTO() { Nickname = x.Nickname, Identifier = x.Identifier, Score = x.EndlessScore, Rank = (_gameConfig.EndlessLeaderboardUsersPrPage * page) + i + 1}).ToList();

            return Ok(new FetchLeaderboardResponse() { Leaderboard = result, PersonalScore = user.EndlessScore, PersonalRank = personalRank } );
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("rank")]
        public async Task<IActionResult> FetchLeaderboardRank()
        {
            User user = (User)Request.HttpContext.Items["user"];

            int rank = await _endlessService.GetRank(user);

            return Ok(new FetchRankResponse() { Score = user.EndlessScore, Rank = rank });
        }

        private string GetShareUrl(string LevelId)
        {
            string Scheme = _env.IsDevelopment() ? "http" : "https";
            string url = $"{Scheme}://{this.Request.Host}/{LevelId}";
            return url.Replace("api2", "play");
        }
    }
}
