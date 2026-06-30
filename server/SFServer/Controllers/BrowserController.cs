using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFServer.Configs;
using SFServer.Contexts;
using SFServer.Filters;
using SFServer.Models.Configs;
using SFServer.Models.DailyChallenge;
using SFServer.Models.DB;
using SFServer.Models.DTO;
using SFServer.Models.Enums;
using SFServer.Models.Requests.Browser;
using SFServer.Models.Responses.Browser;
using SFServer.Services;
using SFServer.Utility;

namespace SFServer.Controllers
{
    [Route("browse")]
    public class BrowserController : Controller
    {
        private readonly PostgreSqlContext _db;
        private readonly VersionConfig _versionConfig;

        private readonly GameConfig _gameConfig;
        private RewardService _rewardService;
        private BoostService _boostService;
        private LevelStatsService _levelStatsService;

        public BrowserController(PostgreSqlContext db, BoostService boostService, IOptions<VersionConfig> versionConfig, IOptions<GameConfig> gameConfig, RewardService rewardService, LevelStatsService levelStatsService)
        {
            _db = db;
            _versionConfig = versionConfig.Value;
            _gameConfig = gameConfig.Value;
            _rewardService = rewardService;
            _boostService = boostService;
            _levelStatsService = levelStatsService;
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("personal")]
        public async Task<IActionResult> GetPersonalFeed()
        {
            return Ok();
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("featured")]
        public async Task<IActionResult> GetFeatured()
        {
            return Ok();
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("previous-daily-challenges")]
        public async Task<IActionResult> GetPreviousDailyChallengesFeed([FromBody] PreviousDailyChallengesRequest previousDailyChallengesRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            int Amount = Math.Clamp(previousDailyChallengesRequest.Amount, 1, _gameConfig.LevelsPrPage);

            if (previousDailyChallengesRequest.Amount == 0)
                Amount = _gameConfig.LevelsPrPage;

            List<LevelInfoDTO> levels = await _db.DailyChallenges
                .Join(
                    _db.Levels,
                    challenge => challenge.LevelId,
                    level => level.LevelId,
                    (challenge, level) => new
                    {
                        LevelId = challenge.LevelId,
                        CreatedOn = challenge.CreatedOn,
                        Blacklisted = level.Blacklisted,
                        VerifiedUpload = level.VerifiedUpload,
                        MajorGameVersion = level.MajorGameVersion,
                        MinorGameVersion = level.MinorGameVersion,
                        Name = level.Name,
                        CreatorUserId = level.CreatorUserId,
                        GameVersion = level.GameVersion,
                        Thumbnail = level.Thumbnail,
                        MiniThumbnail = level.MiniThumbnail,
                        CoinsInvested = level.CoinsInvested,
                        Likes = level.Likes,
                        Plays = level.Plays,
                    })
                .Where(x => x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)))
                .OrderByDescending(x => x.CreatedOn)
                .Skip(previousDailyChallengesRequest.FromIndex)
                .Take(Amount)
                .Select(x => new LevelInfoDTO()
                {
                    LevelId = x.LevelId,
                    Name = x.Name,
                    CreatorUserId = x.CreatorUserId,
                    GameVersion = x.GameVersion,
                    CreatedOn = x.CreatedOn,
                    Thumbnail = x.Thumbnail,
                    CoinsInvested = x.CoinsInvested,
                    Likes = x.Likes,
                    Plays = x.Plays,
                    CanBeBoosted = x.Plays < _gameConfig.MaxPlaysInNewCategory && x.CreatorUserId == user.UserId && x.Blacklisted == false && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)),
                    HasGraduated = x.Plays >= _gameConfig.MaxPlaysInNewCategory,
                    RewardMultipler = _rewardService.GetRewardMultiplier(x.Plays),
                    Difficulty = Difficulty.Unrated
                })
                .ToListAsync();


            //Find creator username
            foreach (LevelInfoDTO levelInfo in levels)
            {
                levelInfo.Creator = await _db.Users.Where(u => u.UserId == levelInfo.CreatorUserId)
                    .Select(u => u.Nickname)
                    .SingleOrDefaultAsync();

                levelInfo.Difficulty = await _levelStatsService.CalculateDifficulty(levelInfo.LevelId);
            }

            return Ok(new PreviousDailyChallengesResponse() { LevelsPrPage = _gameConfig.LevelsPrPage, Levels = levels });
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("previous-levels-of-the-week")]
        public async Task<IActionResult> GetPreviousWeeklyLevelsFeed([FromBody] PreviousLevelsOfTheWeekRequest previousWeeklyLevelsRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            int Amount = Math.Clamp(previousWeeklyLevelsRequest.Amount, 1, _gameConfig.LevelsPrPage);

            if (previousWeeklyLevelsRequest.Amount == 0)
                Amount = _gameConfig.LevelsPrPage;

            List<LevelInfoDTO> levels = await _db.LevelsOfTheWeek
                .Join(
                    _db.Levels,
                    challenge => challenge.LevelId,
                    level => level.LevelId,
                    (weeklyLevel, level) => new
                    {
                        LevelId = weeklyLevel.LevelId,
                        CreatedOn = weeklyLevel.CreatedOn,
                        Blacklisted = level.Blacklisted,
                        VerifiedUpload = level.VerifiedUpload,
                        MajorGameVersion = level.MajorGameVersion,
                        MinorGameVersion = level.MinorGameVersion,
                        Name = level.Name,
                        CreatorUserId = level.CreatorUserId,
                        GameVersion = level.GameVersion,
                        Thumbnail = level.Thumbnail,
                        MiniThumbnail = level.MiniThumbnail,
                        CoinsInvested = level.CoinsInvested,
                        Likes = level.Likes,
                        Plays = level.Plays,
                    })
                .Where(x => x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)))
                .OrderByDescending(x => x.CreatedOn)
                .Skip(previousWeeklyLevelsRequest.FromIndex)
                .Take(Amount)
                .Select(x => new LevelInfoDTO()
                {
                    LevelId = x.LevelId,
                    Name = x.Name,
                    CreatorUserId = x.CreatorUserId,
                    GameVersion = x.GameVersion,
                    CreatedOn = x.CreatedOn,
                    Thumbnail = x.Thumbnail,
                    CoinsInvested = x.CoinsInvested,
                    Likes = x.Likes,
                    Plays = x.Plays,
                    CanBeBoosted = x.Plays < _gameConfig.MaxPlaysInNewCategory && x.CreatorUserId == user.UserId && x.Blacklisted == false && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)),
                    HasGraduated = x.Plays >= _gameConfig.MaxPlaysInNewCategory,
                    RewardMultipler = _rewardService.GetRewardMultiplier(x.Plays),
                    Difficulty = Difficulty.Unrated
                })
                .ToListAsync();

            //Find creator username
            foreach (LevelInfoDTO levelInfo in levels)
            {
                levelInfo.Creator = await _db.Users.Where(u => u.UserId == levelInfo.CreatorUserId)
                    .Select(u => u.Nickname)
                    .SingleOrDefaultAsync();

                levelInfo.Difficulty = await _levelStatsService.CalculateDifficulty(levelInfo.LevelId);
            }

            return Ok(new PreviousLevelsOfTheWeekResponse() { LevelsPrPage = _gameConfig.LevelsPrPage, Levels = levels });
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("new")]
        public async Task<IActionResult> GetNewsFeed([FromBody] NewFeedRequest newFeedRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            int Amount = Math.Clamp(newFeedRequest.Amount, 1, _gameConfig.LevelsPrPage);

            if (newFeedRequest.Amount == 0)
                Amount = _gameConfig.LevelsPrPage;

            List<LevelInfoDTO> result = await _db.Levels
               .Where(x => x.Blacklisted == false && x.VerifiedUpload && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)))
               .Select(x => new LevelInfoDTO()
               {
                   LevelId = x.LevelId,
                   Name = x.Name,
                   CreatorUserId = x.CreatorUserId,
                   GameVersion = x.GameVersion,
                   CreatedOn = x.CreatedOn,
                   Thumbnail = x.Thumbnail,
                   MiniThumbnail = x.MiniThumbnail,
                   CoinsInvested = x.CoinsInvested,
                   Likes = x.Likes,
                   Plays = x.Plays,
                   CanBeBoosted = x.Plays < _gameConfig.MaxPlaysInNewCategory && x.CreatorUserId == user.UserId && x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)),
                   HasGraduated = x.Plays >= _gameConfig.MaxPlaysInNewCategory,
                   RewardMultipler = _rewardService.GetRewardMultiplier(x.Plays),
                   Difficulty = Difficulty.Unrated
               })
               .OrderByDescending(x => x.CreatedOn)
               .Skip(newFeedRequest.FromIndex)
               .Take(Amount)
               .ToListAsync();

            //Find creator username
            foreach (LevelInfoDTO levelInfo in result)
            {
                levelInfo.Creator = await _db.Users.Where(u => u.UserId == levelInfo.CreatorUserId)
                    .Select(u => u.Nickname)
                    .SingleOrDefaultAsync();

                levelInfo.Difficulty = await _levelStatsService.CalculateDifficulty(levelInfo.LevelId);
            }

            return Ok(new NewFeedResponse() { LevelsPrPage = _gameConfig.LevelsPrPage, Levels = result });
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("unbeat")]
        public async Task<IActionResult> GetUnbeatFeed([FromBody] NewFeedRequest newFeedRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            int Amount = Math.Clamp(newFeedRequest.Amount, 1, _gameConfig.LevelsPrPage);

            if (newFeedRequest.Amount == 0)
                Amount = _gameConfig.LevelsPrPage;

            List<LevelInfoDTO> result = await _db.Levels
               .Where(x => x.Blacklisted == false && x.VerifiedUpload && x.VerifiedUpload && x.Wins == 0 && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)))
               .Select(x => new LevelInfoDTO()
               {
                   LevelId = x.LevelId,
                   Name = x.Name,
                   CreatorUserId = x.CreatorUserId,
                   GameVersion = x.GameVersion,
                   CreatedOn = x.CreatedOn,
                   Thumbnail = x.Thumbnail,
                   MiniThumbnail = x.MiniThumbnail,
                   CoinsInvested = x.CoinsInvested,
                   Likes = x.Likes,
                   Plays = x.Plays,
                   CanBeBoosted = x.Plays < _gameConfig.MaxPlaysInNewCategory && x.CreatorUserId == user.UserId && x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)),
                   HasGraduated = x.Plays >= _gameConfig.MaxPlaysInNewCategory,
                   RewardMultipler = _rewardService.GetRewardMultiplier(x.Plays),
                   Difficulty = Difficulty.Unrated
               })
               .OrderByDescending(x => x.CreatedOn)
               .Skip(newFeedRequest.FromIndex)
               .Take(Amount)
               .ToListAsync();

            //Find creator username
            foreach (LevelInfoDTO levelInfo in result)
            {
                levelInfo.Creator = await _db.Users.Where(u => u.UserId == levelInfo.CreatorUserId)
                    .Select(u => u.Nickname)
                    .SingleOrDefaultAsync();

                levelInfo.Difficulty = await _levelStatsService.CalculateDifficulty(levelInfo.LevelId);
            }

            return Ok(new NewFeedResponse() { LevelsPrPage = _gameConfig.LevelsPrPage, Levels = result });
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("boosted")]
        public async Task<IActionResult> GetBoostedFeed([FromBody] BoostedFeedRequest boostedFeedRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            int Amount = Math.Clamp(boostedFeedRequest.Amount, 1, _gameConfig.LevelsPrPage);

            if (boostedFeedRequest.Amount == 0)
                Amount = _gameConfig.LevelsPrPage;

            List<LevelInfoDTO> result = await _db.Levels
               .Where(x => x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)) && x.Plays < _gameConfig.MaxPlaysInNewCategory && (x.CoinsInvested > 0 || x.CreatorUserId == user.UserId))
               .Select(x => new LevelInfoDTO()
               {
                   LevelId = x.LevelId,
                   Name = x.Name,
                   CreatorUserId = x.CreatorUserId,
                   GameVersion = x.GameVersion,
                   CreatedOn = x.CreatedOn,
                   Thumbnail = x.Thumbnail,
                   MiniThumbnail = x.MiniThumbnail,
                   CoinsInvested = x.CoinsInvested,
                   Likes = x.Likes,
                   Plays = x.Plays,
                   CanBeBoosted = x.Plays < _gameConfig.MaxPlaysInNewCategory && x.CreatorUserId == user.UserId && x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)),
                   HasGraduated = x.Plays >= _gameConfig.MaxPlaysInNewCategory,
                   RewardMultipler = _rewardService.GetRewardMultiplier(x.Plays),
                   Difficulty = Difficulty.Unrated
               })
               .OrderByDescending(x => x.CoinsInvested)
               .ThenBy(x => x.CreatedOn)
               .Skip(boostedFeedRequest.FromIndex)
               .Take(Amount)
               .ToListAsync();

            //Find creator username
            foreach (LevelInfoDTO levelInfo in result)
            {
                levelInfo.Creator = await _db.Users.Where(u => u.UserId == levelInfo.CreatorUserId)
                    .Select(u => u.Nickname)
                    .SingleOrDefaultAsync();

                levelInfo.Difficulty = await _levelStatsService.CalculateDifficulty(levelInfo.LevelId);
            }

            return Ok(new BoostedFeedResponse() { LevelsPrPage = _gameConfig.LevelsPrPage, Levels = result });
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("boostable")]
        public async Task<IActionResult> GetPlayersBoostableLevelsFeed([FromBody] BoostedFeedRequest boostedFeedRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            int Amount = Math.Clamp(boostedFeedRequest.Amount, 1, _gameConfig.LevelsPrPage);

            if (boostedFeedRequest.Amount == 0)
                Amount = _gameConfig.LevelsPrPage;

            List<LevelInfoDTO> result = await _db.Levels
               .Where(x => x.Plays < _gameConfig.MaxPlaysInNewCategory && x.CreatorUserId == user.UserId && x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)))
               .Select(x => new LevelInfoDTO()
               {
                   LevelId = x.LevelId,
                   Name = x.Name,
                   CreatorUserId = x.CreatorUserId,
                   GameVersion = x.GameVersion,
                   CreatedOn = x.CreatedOn,
                   Thumbnail = x.Thumbnail,
                   MiniThumbnail = x.MiniThumbnail,
                   CoinsInvested = x.CoinsInvested,
                   Likes = x.Likes,
                   Plays = x.Plays,
                   CanBeBoosted = true,
                   HasGraduated = false,
                   RewardMultipler = _rewardService.GetRewardMultiplier(x.Plays),
                   Creator = user.Nickname,
                   Difficulty = Difficulty.Unrated
               })
               .OrderByDescending(x => x.CoinsInvested)
               .ThenBy(x => x.CreatedOn)
               .Skip(boostedFeedRequest.FromIndex)
               .Take(Amount)
               .ToListAsync();

            foreach (LevelInfoDTO levelInfo in result)
            {
                levelInfo.BoostedRank = await _boostService.FindRank(levelInfo.CoinsInvested, levelInfo.CreatedOn.Ticks);
                levelInfo.Difficulty = await _levelStatsService.CalculateDifficulty(levelInfo.LevelId);
            }

            return Ok(new BoostedFeedResponse() { LevelsPrPage = _gameConfig.LevelsPrPage, Levels = result });
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("popular")]
        public async Task<IActionResult> GetPopularFeed([FromBody] PopularFeedRequest popularFeedRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            int Amount = Math.Clamp(popularFeedRequest.Amount, 1, _gameConfig.LevelsPrPage);

            if (popularFeedRequest.Amount == 0)
                Amount = _gameConfig.LevelsPrPage;

            List<LevelInfoDTO> result = await _db.Levels
               .Where(x => x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)))
               .Select(x => new LevelInfoDTO()
               {
                   LevelId = x.LevelId,
                   Name = x.Name,
                   CreatorUserId = x.CreatorUserId,
                   GameVersion = x.GameVersion,
                   CreatedOn = x.CreatedOn,
                   Thumbnail = x.Thumbnail,
                   MiniThumbnail = x.MiniThumbnail,
                   CoinsInvested = x.CoinsInvested,
                   Likes = x.Likes,
                   Plays = x.Plays,
                   CanBeBoosted = x.Plays < _gameConfig.MaxPlaysInNewCategory && x.CreatorUserId == user.UserId && x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)),
                   HasGraduated = x.Plays >= _gameConfig.MaxPlaysInNewCategory,
                   RewardMultipler = _rewardService.GetRewardMultiplier(x.Plays),
                   Difficulty = Difficulty.Unrated
               })
               .OrderByDescending(x => x.Likes)
               .Skip(popularFeedRequest.FromIndex)
               .Take(Amount)
               .ToListAsync();

            //Find creator username
            foreach (LevelInfoDTO levelInfo in result)
            {
                levelInfo.Creator = await _db.Users.Where(u => u.UserId == levelInfo.CreatorUserId)
                    .Select(u => u.Nickname)
                    .SingleOrDefaultAsync();

                levelInfo.Difficulty = await _levelStatsService.CalculateDifficulty(levelInfo.LevelId);
            }

            return Ok(new PopularFeedResponse() { LevelsPrPage = _gameConfig.LevelsPrPage, Levels = result });
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("trending")]
        public async Task<IActionResult> GetTrendingFeed([FromBody] TrendingFeedRequest trendingFeedRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            int Amount = Math.Clamp(trendingFeedRequest.Amount, 1, _gameConfig.LevelsPrPage);

            if (trendingFeedRequest.Amount == 0)
                Amount = _gameConfig.LevelsPrPage;
            

            List<LevelInfoDTO> result = await _db.Levels
                .Where(x => /*trendingLevels.ContainsKey(x.LevelId) &&*/ x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)))
                .Join(_db.TrendingLevels, x => x.LevelId, y => y.LevelId, (x, y) => new { Level = x, Trend = y })
                .Where(x => x.Trend != null)
                .OrderByDescending(x => x.Trend.Score)
                .Select(x => new LevelInfoDTO()
                {
                    LevelId = x.Level.LevelId,
                    Name = x.Level.Name,
                    CreatorUserId = x.Level.CreatorUserId,
                    GameVersion = x.Level.GameVersion,
                    CreatedOn = x.Level.CreatedOn,
                    Thumbnail = x.Level.Thumbnail,
                    MiniThumbnail = x.Level.MiniThumbnail,
                    CoinsInvested = x.Level.CoinsInvested,
                    Likes = x.Level.Likes,
                    Plays = x.Level.Plays,
                    CanBeBoosted = x.Level.Plays < _gameConfig.MaxPlaysInNewCategory && x.Level.CreatorUserId == user.UserId && x.Level.Blacklisted == false && x.Level.VerifiedUpload && ((x.Level.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.Level.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.Level.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)),
                    HasGraduated = x.Level.Plays >= _gameConfig.MaxPlaysInNewCategory,
                    RewardMultipler = _rewardService.GetRewardMultiplier(x.Level.Plays),
                    Difficulty = Difficulty.Unrated
                })
                .Skip(trendingFeedRequest.FromIndex)
                .Take(Amount)
                .ToListAsync();

            //Find creator username
            foreach (LevelInfoDTO levelInfo in result)
            {
                levelInfo.Creator = await _db.Users.Where(u => u.UserId == levelInfo.CreatorUserId)
                    .Select(u => u.Nickname)
                    .SingleOrDefaultAsync();

                levelInfo.Difficulty = await _levelStatsService.CalculateDifficulty(levelInfo.LevelId);
            }

            return Ok(new TrendingFeedResponse() { LevelsPrPage = _gameConfig.LevelsPrPage, Levels = result });
        }
    }
}
