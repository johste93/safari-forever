using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using SFServer.Contexts;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFServer.Models;
using SFServer.Models.DTO;
using RazorLight;
using SFServer.Infrastructure;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using SFServer.Utility;
using SFServer.Services;
using SFServer.Filters;
using System.Globalization;
using SFServer.Models.Notifications;
using Microsoft.Extensions.Hosting;
using SFServer.Models.DailyChallenge;
using Microsoft.EntityFrameworkCore;
using SFServer.Models.Requests.LevelRequests;
using SFServer.Models.Responses.LevelResponses;
using SFServer.Models.Enums;
using SFServer.Models.DB;
using SFServer.Models.Transaction;
using Microsoft.Extensions.Options;
using SFServer.Models.Configs;
using SFServer.Configs;

namespace SFServer.Controllers
{
    [Route("[controller]")]
    public class LevelController : Controller
    {
        private readonly PostgreSqlContext _db;
        private RazorLightEngine _razorEngine;
        private IWebHostEnvironment _env;
        private NotificationService _notificationService;
        private DailyChallengeService _dailyChallengeService;
        private TransactionService _transactionService;
        private RewardService _rewardService;
        private GameStatisticsService _gameStatisticsService;
        private LevelStatsService _levelStatsService;
        private DiscordService _discordService;
        private EndlessService _endlessService;

        private readonly GameConfig _gameConfig;
        private readonly RewardConfig _rewardConfig;
        private readonly VersionConfig _versionConfig;


        public LevelController(PostgreSqlContext db, DiscordService discordService, TransactionService transactionService, EndlessService endlessService, IWebHostEnvironment env, NotificationService notificationService, DailyChallengeService dailyChallengeService, IOptions<RewardConfig> rewardConfig, IOptions<GameConfig> gameConfig, IOptions<VersionConfig> versionConfig, RewardService rewardService, GameStatisticsService gameStatisticsService, LevelStatsService levelStatsService)
        {
            _db = db;
            _env = env;
            _notificationService = notificationService;
            _dailyChallengeService = dailyChallengeService;
            _transactionService = transactionService;
            _razorEngine = new RazorLightEngineBuilder()
               .UseFilesystemProject(Path.Combine(env.WebRootPath, "Templates"))
               .UseMemoryCachingProvider()
               .Build();

            _gameConfig = gameConfig.Value;
            _rewardConfig = rewardConfig.Value;
            _versionConfig = versionConfig.Value;
            _rewardService = rewardService;
            _gameStatisticsService = gameStatisticsService;
            _levelStatsService = levelStatsService;
            _discordService = discordService;
            _endlessService = endlessService;
        }

        private List<string> taunts = new List<string>()
        {
            "{0} took the lead from {1} by {2} seconds! ({3})",
            "{0} wiped the floor with {1} by {2} seconds! Will {1} be able to recover?! ({3})",
            "{0} annihilated {1} by {2} seconds! Will {1} take the highscore back?! ({3})",
            "{0} has obliterated {1} by {2} seconds! Devastating! ({3})",
            "{0} has chipped away {1}'s highscore by {2} seconds! Tonight {1} dines in hell. ({3})",
            "It has taken blood, sweat and RGB, but {0} has just beaten {1}'s highscore by {2} seconds! ({3})",
            "{0} is on FIRE and {1}'s highscore has gone up in flames by {2} seconds! BIG OOF {1}!! ({3})",
            "{0} the usurper has knocked {1} off the throne by {2} seconds! The people are rioting! ({3})",
            "Shots fired!! Pew! pew! {0} has sparked a revolution, {1} has been replaced by {2} seconds! ({3})",
            "{0} has ended {1} reign by {2} seconds! Pepe approves! ({3})",
            "{0} for president! {1} has been impeached by {2} seconds! Unheard-of! ({3})",
            "{0} has beaten {1} by {2} seconds! Outrageous! ({3})",
            "When {0} snapped his fingers; {1} was turned to dust by {2} seconds! Marvelous! ({3})",
            "{0} used his masterball and caught {1}'s highscore by {2} seconds! _**surprised pikachu face**_ ({3})",
            "{0} has humiliated {1} by {2} seconds! Akward!! ({3})",
            "Have I told you about the time {0} crushed {1} highscore by {2} seconds? Oh wait, that was just now! ({3})",
            "{0} has poisoned the well and taken {1}'s highscore by {2} seconds! Despicable! ({3})",
            "{0} has stolen the podium from {1} by {2} seconds! The crowd is ecstatic! ({3})",
            "{0} must have made a deal with the devil because he just took the lead from {1} by {2} seconds! ({3})",
            "{0} has instigated a mutiny and {1}'s highscore has been overthrown by {2} seconds! Revolting! ({3})",
            "{0} has coup d'état by {2} seconds and made it clear to everyone that {1} is unfit to rule! Embarrassing! ({3})",
            "{0} has executed {1} by {2} seconds! Who volunteers as a tribute?! ({3})",
            "May I have your attention, please?\n{0} is the real slim shady taking the lead from {1} by {2} seconds! ({3})",
            "Public service announcement #406:\n{0} is in the lead. {1} never stood a chance and was beaten by {2} seconds!\nThat is all.\nThank you ({3})",
            "{0} used quantum physics to beat {1}'s time by {2}! An outstanding contribution to science! ({3})",
            "{0} intimidated {1} with an angry face, resulting in {1} being slower then them by {2}! ({3})",
            "While {1} slacked off, {0} mastered the blade ...and beat their time by {2}! ({3})",
            "You just can't make this stuff up! {0} beat {1}'s time by {2}, and then proceeded beat them with the goal flag! ({3})",
            "{1} tried to bribe {0} out of taking the first place from them. {0} knew better and rightfully claimed first place with the time: {2}! ({3})",
            "{0} decided to show {1} how it's done {2} faster. ({3})",
            "{1} You'll keep first place they said! {0} will never beat your time by {2} they said! ({3})",
            "{0} cut {1}'s time by {2} seconds using the awesome power of friendship and love! ({3})",
            "{0} must have h4xx3d {1}'s phone, resulting in them being slower than {1} by {2} ({3})",
            "{0} cut {1}'s time by {2} with a laser. Pew! Pew! ({3})"
        };

        private string GetShareUrl(string LevelId)
        {
            string Scheme = _env.IsDevelopment() ? "http" : "https";
            string url = $"{Scheme}://{this.Request.Host}/{LevelId}";
            return url.Replace("api2", "play");
        }

        [HttpPost("upload")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ModelValidation]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [Authorize(Roles = "Player,Admin,Moderator")]
        public async Task<IActionResult> Upload([FromBody] LevelPublishRequest levelPublishDTO)
        {
            User user = (User)Request.HttpContext.Items["user"];

            string randomId = IdGenerator.Generate(6);
            while (await _db.Levels.FindAsync(randomId) != null)
                randomId = IdGenerator.Generate(6);

            Level newLevel = levelPublishDTO.ToLevel(randomId, user.UserId);
            user.Levels.Add(newLevel);
            _db.UpdateUser(user);

            await _db.SaveChangesAsync();
            return Ok(new LevelPublishResponse(newLevel.LevelId));
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("upload/verify/{levelId}")]
        public async Task<ActionResult> VerifyUpload(string levelId)
        {
            User user = (User)Request.HttpContext.Items["user"];
            Level level = await _db.Levels.FindAsync(levelId);
            if (level == null)
                return NotFound();

            if (level.VerifiedUpload)
                return BadRequest();

            if (user.UserId != level.CreatorUserId)
                return Unauthorized();

            level.VerifiedUpload = true;

            _db.UpdateLevel(level);
            await _db.SaveChangesAsync();

            string shareUrl = GetShareUrl(level.LevelId).Replace("api2", "play");

            List<Following> followings = await _db.Followings.Where(x => x.UserBeingFollowedId == user.UserId).ToListAsync();
            if (followings != null && followings.Count > 0)
            {
                string title = $"Someone you follow published a level";
                string body = $"{user.Nickname} published \"{level.Name}\"";
                foreach (Following f in followings)
                {
                    await _notificationService.CreateNotification(f.UserFollowingId, title, body, NotificationType.FollowedPublishedLevel, shareUrl, new List<NotificationLink>() { new NotificationLink("View Level", shareUrl) });
                }
            }

            return Ok(new VerifyUploadResponse(shareUrl));
        }

        [NoCache]
        [HttpGet("~/{levelId}")]
        public async Task<ActionResult> Preview(string levelId)
        {
            Level level = await _db.Levels.FindAsync(levelId);
            if (level == null)
                return NotFound();

            if (!level.VerifiedUpload)
                return NotFound();
            
            var url = "";
            if(_env.IsDevelopment())
                url += "http://";
            else
                url += "https://";

            url += Request.Host.ToUriComponent();

            string displayUrl = Request.GetDisplayUrl();
            string playButtonUrl = displayUrl.Contains("api2.safariforever.com", StringComparison.OrdinalIgnoreCase) ? "https://safariforever.com" : GetShareUrl(level.LevelId).Replace("play", "api2");

            User creator = await _db.Users.FindAsync(level.CreatorUserId);

            User recordHolder = await _db.Users.FindAsync(level.RecordHolder);

            DailyChallenge dailyChallenge = await _dailyChallengeService.GetCurrentChallenge();
            if (dailyChallenge != null)
            {
                if (dailyChallenge.LevelId == level.LevelId)
                {
                    level.Record_Seconds = -1;
                    level.Record_Milliseconds = -1;
                    level.RecordHolder = string.Empty;
                }
            }

            Difficulty difficulty = await _levelStatsService.CalculateDifficulty(level.LevelId);

            var cacheResult = _razorEngine.TemplateCache.RetrieveTemplate("ShareCodePreview.cshtml");

            string result = string.Empty;
            if (cacheResult.Success)
                result = await _razorEngine.RenderTemplateAsync(cacheResult.Template.TemplatePageFactory(), new LevelViewDTO(level, creator, recordHolder, url, playButtonUrl, difficulty));
            else
                result = await _razorEngine.CompileRenderAsync("ShareCodePreview.cshtml", new LevelViewDTO(level, creator, recordHolder, url, playButtonUrl, difficulty));

            return new ContentResult()
            {
                Content = result,
                ContentType = "text/html",
            };
        }
        /*
        [NoCache]
        [HttpGet("~/webgl/{levelId}")]
        public async Task<ActionResult> PlayInBrowser(string levelId)
        {
            Level level = await _db.Levels.FindAsync(levelId);
            if (level == null)
                return NotFound();

            if (!level.VerifiedUpload)
                return NotFound();

            var url = "";
            if (_env.IsDevelopment())
                url += "http://";
            else
                url += "https://";

            url += Request.Host.ToUriComponent();

            User creator = await _db.Users.FindAsync(level.CreatorUserId);
            User recordHolder = await _db.Users.FindAsync(level.RecordHolder);

            DailyChallenge dailyChallenge = await _dailyChallengeService.GetCurrentChallenge();
            if (dailyChallenge != null)
            {
                if (dailyChallenge.LevelId == level.LevelId)
                {
                    level.Record_Seconds = -1;
                    level.Record_Milliseconds = -1;
                    level.RecordHolder = string.Empty;
                }
            }

            var cacheResult = _razorEngine.TemplateCache.RetrieveTemplate("PlayWebGL.cshtml");

            string result = string.Empty;
            if (cacheResult.Success)
                result = await _razorEngine.RenderTemplateAsync(cacheResult.Template.TemplatePageFactory(), new LevelViewDTO(level, creator, recordHolder, url, ""));
            else
                result = await _razorEngine.CompileRenderAsync("PlayWebGL.cshtml", new LevelViewDTO(level, creator, recordHolder, url, ""));

            return new ContentResult()
            {
                Content = result,
                ContentType = "text/html",
            };
        }
        */

        [HttpGet("{levelId}/img")]
        public IActionResult PreviewImage(string levelId)
        {
            Level level = _db.Levels.Find(levelId);

            if (level == null)
                return NotFound();

            return new FileContentResult(level.Thumbnail, "image/gif");
        }

        //[Authorize(Roles = "Player,Admin,Moderator")] //We dont require authorization to make webgl version work.
        //[ServiceFilter(typeof(LegalVerificationAttribute))] //not required because of webgl version.
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("{levelId}/download")]
        public async Task<IActionResult> Download(string levelId)
        {
            Level level = await _db.Levels.FindAsync(levelId);

            if (level == null)
                return NotFound();

            if (!level.VerifiedUpload)
                return NotFound();

            User user = (User)Request.HttpContext.Items["user"];

            User creator = await _db.Users.FindAsync(level.CreatorUserId);

            PublishedLevelMetaDTO meta = new PublishedLevelMetaDTO
            {
                LevelId = level.LevelId,
                Name = level.Name,
                CreatorUserId = level.CreatorUserId,
                CreatorUserName = creator == null ? "?" : $"{creator.Nickname}#{creator.Identifier.ToString("0000")}",
                Deaths = level.Deaths,
                Wins = level.Wins,
                Likes = level.Likes,
                CoinsInvested = level.CoinsInvested,
                CanBeBoosted = false,
                HasGraduated = level.Plays >= _gameConfig.MaxPlaysInNewCategory,
                GameVersion = level.GameVersion,
                ShareUrl = GetShareUrl(level.LevelId),
                CreatedOn = level.CreatedOn.Ticks,
            };

            if (user != null)
                meta.CanBeBoosted = level.Plays < _gameConfig.MaxPlaysInNewCategory && level.CreatorUserId == user.UserId && level.Blacklisted == false && level.VerifiedUpload && ((level.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (level.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && level.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor));
            
            return Ok(JsonConvert.SerializeObject(new LevelDownloadResponse(level.SerializedLevel, meta)));
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("{levelId}/play")]
        public async Task<IActionResult> Play(string levelId)
        {
            Level level = await _db.Levels.FindAsync(levelId);

            if (level == null)
                return NotFound();

            User user = (User)Request.HttpContext.Items["user"];
            if (user.UserId == level.CreatorUserId)
                return StatusCode(403, "You cant influence your own level statistics");

            RecentPlay recentPlay = await _db.RecentPlays.FirstOrDefaultAsync(x => x.UserId == user.UserId && x.LevelId == levelId);
            if (recentPlay == null)
            {
                //User havent played this level recently
                _db.RecentPlays.Add(new RecentPlay() { UserId = user.UserId, LevelId = levelId });
            }
            else
            {
                return Ok("User has played this level before."); //No point in checking the stats if we played this level recently
            }
                
            LevelUserStats stats = await _db.LevelUserStats.FirstOrDefaultAsync(x => x.UserId == user.UserId && x.LevelId == levelId);
            if (stats != null)
            {
                stats.Attempts++;
                _db.UpdateLevelUserStats(stats);
                await _db.SaveChangesAsync();

                return Ok("User has played this level before.");
            }   

            _db.LevelUserStats.Add(new LevelUserStats(levelId, user.UserId) { Attempts = 0} );

            level.Plays++;
            _db.UpdateLevel(level);

            Following following = await _db.Followings.Where(x => x.UserFollowingId == level.CreatorUserId && x.UserBeingFollowedId == user.UserId).FirstOrDefaultAsync();
            
            if(following != null)
            { 
                string title = $"Someone you follow played your level";
                string body = $"{user.Nickname} played {level.Name}";
                string deeplinkUrl = $"https://play.safariforever.com/{level.LevelId}";
                await _notificationService.CreateNotification(level.CreatorUserId, title, body, NotificationType.FollowedPlayedLevel, deeplinkUrl, new List<NotificationLink>() { new NotificationLink("View Level", deeplinkUrl) });
            }

            await _db.SaveChangesAsync();

            return Ok();
        }


        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("{levelId}/like")]
        public async Task<IActionResult> Like(string levelId)
        {
            Level level = await _db.Levels.FindAsync(levelId);

            if (level == null)
                return NotFound();

            User user = (User)Request.HttpContext.Items["user"];
            if (user.UserId == level.CreatorUserId)
                return StatusCode(403, "You cant influence your own level statistics");

            LevelUserStats stats = await _db.LevelUserStats.FirstOrDefaultAsync(x => x.UserId == user.UserId && x.LevelId == levelId);
            if (stats == null)
            {
                //User has not had an option on this level before.
                stats = new LevelUserStats(levelId, user.UserId)
                {
                    Opinion = LevelOpinion.Liked
                };
                _db.LevelUserStats.Add(stats);
                level.Likes++;
                level.LifetimeLikes++;
                _db.UpdateLevel(level);
            }
            else
            {
                if (stats.Opinion == LevelOpinion.Liked)
                    return Ok("User already liked this level");

                if (stats.Opinion == LevelOpinion.Disliked)
                {
                    level.Dislikes--;
                }

                stats.Opinion = LevelOpinion.Liked;
                _db.UpdateLevelUserStats(stats);

                level.Likes++;
                _db.UpdateLevel(level);

                Following following = await _db.Followings.Where(x => x.UserFollowingId == level.CreatorUserId && x.UserBeingFollowedId == user.UserId).FirstOrDefaultAsync();

                if (following != null)
                {
                    string title = $"Someone you follow liked your level";
                    string body = $"{user.Nickname} liked {level.Name}";
                    string deeplinkUrl = $"https://play.safariforever.com/{level.LevelId}";
                    await _notificationService.CreateNotification(level.CreatorUserId, title, body, NotificationType.FollowedLikedLevel, deeplinkUrl, new List<NotificationLink>() { new NotificationLink("View Level", deeplinkUrl) });
                }
            }
            
            if(!stats.HasGivenLikeBefore)
            {
                if (level.LifetimeLikes <= _rewardConfig.MaxLikeRewards)
                {
                    User creator = await _db.Users.FirstOrDefaultAsync(x => x.UserId == level.CreatorUserId);
                    if (creator != null)
                        await _transactionService.AddCoins(creator, _rewardConfig.RecivedLike, $"Level {level.LevelId} recived a like", TransactionType.RecivedLike, level.Name);
                }
                stats.HasGivenLikeBefore = true;
                _db.UpdateLevelUserStats(stats);
            }
            

            await _db.SaveChangesAsync();

            return Ok();
        }

        
        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("{levelId}/dislike")]
        public async Task<IActionResult> Dislike(string levelId)
        {
            Level level = await _db.Levels.FindAsync(levelId);

            if (level == null)
                return NotFound();

            User user = (User)Request.HttpContext.Items["user"];
            if (user.UserId == level.CreatorUserId)
                return StatusCode(403, "You cant influence your own level statistics");

            LevelUserStats stats = await _db.LevelUserStats.FirstOrDefaultAsync(x => x.UserId == user.UserId && x.LevelId == levelId);
            if (stats == null)
            {
                stats = new LevelUserStats(levelId, user.UserId)
                {
                    Opinion = LevelOpinion.Disliked
                };
                _db.LevelUserStats.Add(stats);

                level.Dislikes++;
                _db.UpdateLevel(level);
            }
            else
            {
                if (stats.Opinion == LevelOpinion.Disliked)
                    return Ok("User already disliked this level");

                if (stats.Opinion == LevelOpinion.Liked)
                {
                    level.Likes--;
                }

                stats.Opinion = LevelOpinion.Disliked;
                _db.UpdateLevelUserStats(stats);

                level.Dislikes++;
                _db.UpdateLevel(level);
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("{levelId}/clear-opinion")]
        public async Task<IActionResult> ClearOpinion(string levelId)
        {
            Level level = await _db.Levels.FindAsync(levelId);

            if (level == null)
                return NotFound();

            User user = (User)Request.HttpContext.Items["user"];
            if (user.UserId == level.CreatorUserId)
                return StatusCode(403, "You cant influence your own level statistics");

            LevelUserStats stats = await _db.LevelUserStats.FirstOrDefaultAsync(x => x.UserId == user.UserId && x.LevelId == levelId);
            if (stats == null)
            {
                return NotFound();
            }
            else
            {
                if (stats.Opinion == LevelOpinion.None)
                    return Ok("User already has no opinion of this level");

                if (stats.Opinion == LevelOpinion.Liked)
                {
                    level.Likes--;
                }

                if (stats.Opinion == LevelOpinion.Disliked)
                {
                    level.Dislikes--;
                }

                stats.Opinion = LevelOpinion.None;
                _db.UpdateLevelUserStats(stats);
                _db.UpdateLevel(level);
            }

            await _db.SaveChangesAsync();

            return Ok();
        }


        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [HttpGet("{levelId}/death")]
        public async Task<IActionResult> CountDeath(string levelId)
        {
            Level level = await _db.Levels.FindAsync(levelId);

            if (level == null)
                return NotFound();

            User user = (User)Request.HttpContext.Items["user"];
            if (user.UserId == level.CreatorUserId)
                return StatusCode(403, "You cant influence your own level statistics");

            level.Deaths++;
            _db.UpdateLevel(level);

            await _db.SaveChangesAsync();

            _gameStatisticsService.CountDeath();

            return Ok();
        }


        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [HttpPost("{levelId}/complete")]
        public async Task<IActionResult> LevelComplete(string levelId, [FromBody] LevelCompleteRequest request)
        {
            Level level = await _db.Levels.FindAsync(levelId);

            if (level == null)
                return NotFound();

            if (!request.Seconds.HasValue)
                return BadRequest("Seconds Missing");

            if (!request.Milliseconds.HasValue)
                return BadRequest("Milliseconds Missing");

            if (!request.Deaths.HasValue)
                return BadRequest("Deaths Missing");

            if (!request.Jumps.HasValue)
                return BadRequest("Jumps Missing");

            Highscore highscore = new Highscore(request.Seconds.Value, request.Milliseconds.Value);

            bool dbModified = false;

            //Update personal record.
            User user = (User)Request.HttpContext.Items["user"];

            if (user.UserId != level.CreatorUserId)
            {
                //If player is not creator.
                RecentPlay recentPlay = await _db.RecentPlays.FirstOrDefaultAsync(x => x.UserId == user.UserId && x.LevelId == levelId);
                if (recentPlay == null)
                {
                    //User havent played this level recently
                    _db.RecentPlays.Add(new RecentPlay() { UserId = user.UserId, LevelId = levelId, HasWon = true});

                    level.Wins++;
                    _db.UpdateLevel(level);
                    dbModified = true;
                }
                else
                {
                    if(!recentPlay.HasWon)
                    {
                        recentPlay.HasWon = true;
                        _db.UpdateRecentPlay(recentPlay);

                        level.Wins++;
                        _db.UpdateLevel(level);
                        dbModified = true;
                    }
                }
            }

            var dailyChallenge = await _dailyChallengeService.GetCurrentChallenge();
            bool thisIsTheDailyChallenge = false;


            if (dailyChallenge != null)
            {
                //If there is a daily challenge.
                thisIsTheDailyChallenge = dailyChallenge.LevelId == levelId;
                if(thisIsTheDailyChallenge)
                {
                    //And Daily challenge is this level.

                    DailyChallengeParticipant currentChampion = await _dailyChallengeService.GetTodaysChampion(dailyChallenge);
                    if (currentChampion == null)
                    {
                        string message = string.Format("{0} has taken the highscore with the time: {1}!", user.Nickname, highscore.ToString());
                        await _discordService.PostToDiscord(DailyChallengeService.discordChannelId, message, null);
                    }
                    else
                    {
                        if (currentChampion.UserId != user.UserId)
                        {
                            Highscore currentHighscore = new Highscore(currentChampion.Seconds, currentChampion.Milliseconds);

                            if (highscore.IsLowerThan(currentHighscore))
                            {
                                string difference = currentHighscore.GetScoreSpan(highscore).ToString();
                                string message = string.Format(taunts[new Random().Next(0, taunts.Count)], user.Nickname, currentChampion.Nickname, difference, highscore.ToString());
                                await _discordService.PostToDiscord(DailyChallengeService.discordChannelId, message, null);
                            }
                        }
                    }

                    //Player played daily challenge!
                    DailyChallengeParticipant participant = await _db.DailyChallengeParticipants.FirstOrDefaultAsync<DailyChallengeParticipant>(x => x.DailyChallengeId == dailyChallenge.DailyChallengeId && x.UserId == user.UserId);
                    if(participant != null)
                    {
                        //Player has participated before!
                        Highscore participantHighscore = new Highscore(participant.Seconds, participant.Milliseconds);

                        //update
                        if(highscore.IsLowerThan(participantHighscore))
                        {
                            //If player has beaten his own record.
                            participant.Seconds = highscore.Seconds;
                            participant.Milliseconds = highscore.Milliseconds;
                            _db.UpdateDailyChallengeParticipant(participant);
                            dbModified = true;
                        }
                    }
                    else
                    {
                        //This is the first time player is participating.
                        participant = new DailyChallengeParticipant()
                        {
                            DailyChallengeId = dailyChallenge.DailyChallengeId,
                            UserId = user.UserId,
                            Nickname = user.Nickname,
                            Seconds = highscore.Seconds,
                            Milliseconds = highscore.Milliseconds
                        };

                        //add
                        _db.DailyChallengeParticipants.Add(participant);

                        await _transactionService.AddCoins(user, _rewardConfig.CompletedLevelBaseReward * _rewardConfig.DailyChallengeParticipationMultiplier, "Daily challenge Participation", TransactionType.DailyChallengeParticipation, level.Name);

                        dbModified = true;
                    }
                }
            }

            bool statsCreated = false;
            bool updateUserStats = false;
            LevelUserStats stats = _db.LevelUserStats.FirstOrDefault(x => x.UserId == user.UserId && x.LevelId == levelId);
            if (stats == null)
            {
                statsCreated = true;
                stats = new LevelUserStats(levelId, user.UserId);
            }

            if(!stats.Completed)
            {
                updateUserStats = true;

                stats.Seconds = highscore.Seconds;
                stats.Milliseconds = highscore.Milliseconds;
                stats.Deaths = request.Deaths.Value;
                stats.Jumps = request.Jumps.Value;

                if (user.UserId != level.CreatorUserId) //If player is not creator.
                {
                    if (!thisIsTheDailyChallenge) //If this is not the daily challenge.
                    {
                        if (level.Plays < _gameConfig.MaxPlaysInNewCategory)
                        {
                            await _transactionService.AddCoins(user, _rewardConfig.CompletedLevelBaseReward * _rewardConfig.CompletedNewUserLevelMultipler, "Completed level in the new section.", TransactionType.BeatNewLevel, level.Name);
                        }
                        else
                        {
                            await _transactionService.AddCoins(user, _rewardConfig.CompletedLevelBaseReward, "Completed a level outside the new section", TransactionType.BeatLevel, level.Name);
                        }
                    }
                }
            }
            else
            {
                Highscore previousPersonalBest = new Highscore(stats.Seconds, stats.Milliseconds);

                if (highscore.IsLowerThan(previousPersonalBest))
                {
                    stats.Seconds = highscore.Seconds;
                    stats.Milliseconds = highscore.Milliseconds;
                    stats.HighscoreUpdatedOn = DateTimeOffset.Now;
                    updateUserStats = true;
                }

                if (request.Deaths < stats.Deaths || stats.Deaths < 0)
                {
                    stats.Deaths = request.Deaths.Value;
                    updateUserStats = true;
                }

                if (request.Jumps < stats.Jumps || stats.Jumps < 0)
                {
                    stats.Jumps = request.Jumps.Value;
                    updateUserStats = true;
                }
            }

            if (updateUserStats)
            {
                if (statsCreated)
                    _db.LevelUserStats.Add(stats);
                else
                    _db.UpdateLevelUserStats(stats);

                dbModified = true;
            }

            Highscore previousRecord = new Highscore(level.Record_Seconds, level.Record_Milliseconds);

            if (highscore.IsLowerThan(previousRecord))
            {
                if (string.IsNullOrWhiteSpace(level.RecordHolder))
                {
                    string title = $"You set the best time for {level.Name}";
                    string body = $"Your new best time: {highscore.ToString()}";
                    string deeplinkUrl = $"https://play.safariforever.com/{level.LevelId}";
                    await _notificationService.CreateNotification(user.UserId, title, body, NotificationType.DontSendPushNotification, deeplinkUrl, new List<NotificationLink>() { new NotificationLink("View Level", deeplinkUrl) });
                }
                else
                {

                    if (level.RecordHolder == user.UserId)
                    {
                        //Player beat himself.
                        string title = $"You beat the world best time for {level.Name}";
                        string body = $"Your previous best time: {new Highscore(level.Record_Seconds, level.Record_Milliseconds).ToString()}\nYour new best time: {highscore.ToString()}";
                        string deeplinkUrl = $"https://play.safariforever.com/{level.LevelId}";
                        await _notificationService.CreateNotification(user.UserId, title, body, NotificationType.DontSendPushNotification, deeplinkUrl, new List<NotificationLink>() { new NotificationLink("View Level", deeplinkUrl) });
                    }
                    else
                    {
                        User PreviousRecordHolder = await _db.Users.FindAsync(level.RecordHolder);

                        if(PreviousRecordHolder != null)
                        {
                            string deeplinkUrl = $"https://play.safariforever.com/{level.LevelId}";

                            string title1 = $"You beat the world best time for {level.Name}";
                            string body1 = $"{PreviousRecordHolder.Nickname}'s previous best time: {new Highscore(level.Record_Seconds, level.Record_Milliseconds).ToString()}\nYour new best time: {highscore.ToString()}";
                            await _notificationService.CreateNotification(user.UserId, title1, body1, NotificationType.DontSendPushNotification, deeplinkUrl, new List<NotificationLink>() { new NotificationLink("View Level", deeplinkUrl) });

                            string title2 = $"{user.Nickname} beat your best time for {level.Name}";
                            string body2 = $"{user.Nickname}'s new best time: {highscore.ToString()}\nYour previous best time: {new Highscore(level.Record_Seconds, level.Record_Milliseconds).ToString()}";
                            await _notificationService.CreateNotification(level.RecordHolder, title2, body2, NotificationType.WorldRecordBeaten, deeplinkUrl, new List<NotificationLink>() { new NotificationLink("View Level", deeplinkUrl) });
                        }
                    }
                }

                level.Record_Seconds = highscore.Seconds;
                level.Record_Milliseconds = highscore.Milliseconds;
                level.RecordHolder = user.UserId;
                _db.UpdateLevel(level);
                dbModified = true;
            }

            #region EndlessChallenge

            EndlessChallenge challenge = await _endlessService.GetChallenge(user);
            if(challenge != null)
            {
                if(challenge.LevelId == level.LevelId)
                {
                    await _endlessService.CompleteChallenge(user);
                }
            }

            #endregion


            if (dbModified)
                await _db.SaveChangesAsync();

            if (thisIsTheDailyChallenge)
            {
                //Hide World Record
                level.Record_Seconds = -1;
                level.Record_Milliseconds = -1;
                level.RecordHolder = string.Empty;
            }
         
            List<int> jumps = await _db.LevelUserStats.Where(x => x.LevelId == level.LevelId && x.Jumps >= 0).Select(x => x.Jumps).ToListAsync();
            int averageNumberOfJumps = -1;
            int totalJumps = -1;
            if(jumps.Count > 0)
            {
                totalJumps = jumps.Sum();
                averageNumberOfJumps = (int)((float)totalJumps / (float)jumps.Count);
            }
                
            List<int> deaths = await _db.LevelUserStats.Where(x => x.LevelId == level.LevelId && x.Deaths >= 0).Select(x => x.Deaths).ToListAsync();
            int averageNumberOfDeaths = -1;
            if (deaths.Count > 0)
                averageNumberOfDeaths = (int)((float)deaths.Sum() / (float)deaths.Count);

            Difficulty difficulty = await _levelStatsService.CalculateDifficulty(level.LevelId);

            string recordHolderNickname = "?";
            if (!string.IsNullOrWhiteSpace(level.RecordHolder))
            {
                User recordHolder = _db.Users.Where(x => x.UserId == level.RecordHolder).FirstOrDefault();
                if (recordHolder != null)
                    recordHolderNickname = $"{recordHolder.Nickname}#{recordHolder.Identifier.ToString("0000")}";
            }

            LevelStatsResponse result = new LevelStatsResponse(level.Plays, level.Wins, averageNumberOfDeaths, averageNumberOfJumps, level.Deaths, totalJumps, level.Likes, level.Record_Seconds, level.Record_Milliseconds, level.RecordHolder, recordHolderNickname, difficulty);

            if (thisIsTheDailyChallenge)
            {
                result.Record = "Secret";
                result.RecordHolderNicknameAndIdentifier = "Secret";
            }

            return Ok(result);
        }


        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("{levelId}/user-stats")]
        public async Task<IActionResult> GetUserStats(string levelId)
        {
            Level level = await _db.Levels.FindAsync(levelId);

            if (level == null)
                return NotFound();

            User user = (User)Request.HttpContext.Items["user"];
            LevelUserStats stats = _db.LevelUserStats.FirstOrDefault(x => x.UserId == user.UserId && x.LevelId == levelId);

            if (stats != null)
                return Ok(stats);
            else
                return Ok(new LevelUserStats(level.LevelId, user.UserId));
        }

        [HttpGet("{levelId}/stats")]
        public async Task<IActionResult> GetStats(string levelId)
        {
            Level level = await _db.Levels.FindAsync(levelId);

            if (level == null)
                return NotFound();

            if (!level.VerifiedUpload)
                return NotFound();

            var dailyChallenge = await _dailyChallengeService.GetCurrentChallenge();

            bool thisIsTheDailyChallenge = dailyChallenge != null && level.LevelId == dailyChallenge.LevelId;

            if(thisIsTheDailyChallenge)
            {
                //Hide World Record.
                level.Record_Seconds = -1;
                level.Record_Milliseconds = -1;
                level.RecordHolder = string.Empty;
            }

            List<int> jumps = await _db.LevelUserStats.Where(x => x.LevelId == level.LevelId && x.Jumps >= 0).Select(x => x.Jumps).ToListAsync();
            int averageNumberOfJumps = -1;
            int totalJumps = -1;
            if (jumps.Count > 0)
            {
                totalJumps = jumps.Sum();
                averageNumberOfJumps = (int)((float)totalJumps / (float)jumps.Count);
            }

            List<int> deaths = await _db.LevelUserStats.Where(x => x.LevelId == level.LevelId && x.Deaths >= 0).Select(x => x.Deaths).ToListAsync();
            int averageNumberOfDeaths = -1;
            if (deaths.Count > 0)
            {
                averageNumberOfDeaths = (int)((float)deaths.Sum() / (float)deaths.Count);
            }

            Difficulty difficulty = await _levelStatsService.CalculateDifficulty(level.LevelId);

            string recordHolderNickname = "?";
            if (!string.IsNullOrWhiteSpace(level.RecordHolder))
            {
                User recordHolder = _db.Users.Where(x => x.UserId == level.RecordHolder).FirstOrDefault();
                if (recordHolder != null)
                    recordHolderNickname = $"{recordHolder.Nickname}#{recordHolder.Identifier.ToString("0000")}";
            }

            LevelStatsResponse result = new LevelStatsResponse(level.Plays, level.Wins, averageNumberOfDeaths, averageNumberOfJumps, level.Deaths, totalJumps, level.Likes, level.Record_Seconds, level.Record_Milliseconds, level.RecordHolder, recordHolderNickname, difficulty);

            if (thisIsTheDailyChallenge)
            {
                result.Record = "Secret";
                result.RecordHolderNicknameAndIdentifier = "Secret";
            }

            return Ok(result);
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("by-user")]
        public async Task<IActionResult> GetLevelsByUser([FromBody] LevelsByUserRequest levelsByUserRequest)
        {
            if(levelsByUserRequest.FromIndex < 0)
                return BadRequest("FromIndex outside of scope");

            User user = await _db.Users.FindAsync(levelsByUserRequest.UserId);

            if (user == null)
                return NotFound();

            var result = await _db.Levels
                .Where(x => x.Blacklisted == false && x.VerifiedUpload)
                .Select(x => new LevelInfoDTO()
                {
                    LevelId = x.LevelId,
                    Name = x.Name,
                    CreatorUserId = x.CreatorUserId,
                    Creator = user.Nickname,
                    GameVersion = x.GameVersion,
                    CreatedOn = x.CreatedOn,
                    Thumbnail = x.Thumbnail,
                    MiniThumbnail = x.MiniThumbnail,
                    CoinsInvested = x.CoinsInvested,
                    Likes = x.Likes,
                    Plays = x.Plays,
                    CanBeBoosted = x.Plays < _gameConfig.MaxPlaysInNewCategory && x.CreatorUserId == user.UserId && x.Blacklisted == false && x.VerifiedUpload && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)),
                    HasGraduated = x.Plays >= _gameConfig.MaxPlaysInNewCategory,
                    RewardMultipler = _rewardService.GetRewardMultiplier(x.Plays),
                    Difficulty = Difficulty.Unrated
                })
                .Where(x => x.CreatorUserId == levelsByUserRequest.UserId)
                .OrderByDescending(x => x.CreatedOn)
                .Skip(levelsByUserRequest.FromIndex)
                .Take(_gameConfig.LevelsPrPage)
                .ToListAsync();

            foreach (LevelInfoDTO levelInfo in result)
            {
                levelInfo.Difficulty = await _levelStatsService.CalculateDifficulty(levelInfo.LevelId);
            }

            var response = new LevelsByUserResponse()
            {
                UserId = user.UserId,
                Nickname = user.Nickname,
                LevelsPrPage = _gameConfig.LevelsPrPage,
                Levels = result
            };

            return Ok(response);
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("level-of-the-week")]
        public async Task<IActionResult> GetLevelofTheWeek()
        {
            LevelOfTheWeek levelOfTheWeek = await _db.LevelsOfTheWeek.OrderByDescending(p => p.CreatedOn).FirstOrDefaultAsync();
            if (levelOfTheWeek == null)
            {
                return StatusCode(404, "Level of the week not found.");
            }

            Level level = await _db.Levels
                .Where(x => x.LevelId == levelOfTheWeek.LevelId && x.Blacklisted == false && x.VerifiedUpload)
                .FirstOrDefaultAsync();

            if(level == null)
            {
                return NotFound();
            }

            User user = (User)Request.HttpContext.Items["user"];

            LevelInfoDTO levelInfo = await _db.Levels
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
                    Difficulty = Difficulty.Unrated,
                })
                .Where(x => x.LevelId == levelOfTheWeek.LevelId)
                .FirstOrDefaultAsync();

            //Find creator name.
            levelInfo.Creator = await _db.Users.Where(u => u.UserId == levelInfo.CreatorUserId)
                .Select(u => u.Nickname)
                .SingleOrDefaultAsync();

            levelInfo.Difficulty = await _levelStatsService.CalculateDifficulty(levelInfo.LevelId);

            return Ok(new GetLevelOfTheWeekResponse()
            {
                Level = levelInfo
            });
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("daily-challenge")]
        public async Task<IActionResult> GetDailyChallenge()
        {
            var dailyChallenge = await _dailyChallengeService.GetCurrentChallenge();
            if (dailyChallenge == null)
            {
                return StatusCode(404, "Daily Challenge not found.");
            }

            Level level = await _db.Levels
                .Where(x => x.LevelId == dailyChallenge.LevelId && x.Blacklisted == false && x.VerifiedUpload)
                .FirstOrDefaultAsync();

            if (level == null)
            {
                return NotFound();
            }

            User user = (User)Request.HttpContext.Items["user"];

            LevelInfoDTO levelInfo = await _db.Levels
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
                    RewardMultipler = _rewardConfig.DailyChallengeParticipationMultiplier,
                    Difficulty = Difficulty.Unrated
                })
                .Where(x => x.LevelId == dailyChallenge.LevelId)
                .FirstOrDefaultAsync();

            //Find creator name.
            levelInfo.Creator = await _db.Users.Where(u => u.UserId == levelInfo.CreatorUserId)
                .Select(u => u.Nickname)
                .SingleOrDefaultAsync();

            levelInfo.Difficulty = await _levelStatsService.CalculateDifficulty(levelInfo.LevelId);

            DailyChallengeParticipant champion = await _dailyChallengeService.GetTodaysChampion(dailyChallenge);

            string championNickname = "";
            string time = "--:--";
            if(champion != null)
            {
                championNickname = champion.Nickname;
                time = new Highscore(champion.Seconds, champion.Milliseconds).ToString();
            }

            var response = new GetDailyChallengeResponse()
            {
                Level = levelInfo,
                HasChampion = champion != null,
                CurrentLeaderNickname = championNickname,
                CurrentHighscore = time,
            };

            return Ok(response);
        }
    }
}
