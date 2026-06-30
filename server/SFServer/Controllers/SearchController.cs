using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFServer.Contexts;
using SFServer.Filters;
using SFServer.Models.Configs;
using SFServer.Models.DB;
using SFServer.Models.DTO;
using SFServer.Models.Responses.SearchResponses;
using SFServer.Services;
using SFServer.Models.Enums;
using SFServer.Configs;

namespace SFServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly PostgreSqlContext _db;
        private readonly GameConfig _gameConfig;

        private RewardService _rewardService;
        private VersionConfig _versionConfig;
        private LevelStatsService _levelStatsService;

        public SearchController(PostgreSqlContext db, IOptions<GameConfig> gameConfig, RewardService rewardService, IOptions<VersionConfig> versionConfig, LevelStatsService levelStatsService)
        {
            _db = db;
            _gameConfig = gameConfig.Value;
            _rewardService = rewardService;
            _versionConfig = versionConfig.Value;
            _levelStatsService = levelStatsService;
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("user/{nickname}/{identifier}")]
        public async Task<IActionResult> GetProfile(string nickname, int identifier)
        {
            ProfileInfoDTO userToFind = await _db.Users
                .Where(x => x.Nickname == nickname && x.Identifier == identifier)
                .Select(x => new ProfileInfoDTO 
                {
                    UserId = x.UserId,
                    Nickname = x.Nickname,
                    Identifier = x.Identifier,
                    Color = x.Color,
                    LastActive = x.LastActive
                })
                .FirstOrDefaultAsync();

            if (userToFind == null)
                return StatusCode(404, $"User not found {nickname}#{identifier}");

            return Ok(new PlayerSearchResponse(userToFind));
        }
        
        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("level/{levelNameOrId}")]
        public async Task<IActionResult> SearchByLevelName(string levelNameOrId)
        {
            User user = (User)Request.HttpContext.Items["user"];

            List<LevelInfoDTO> result;

            if (levelNameOrId.Contains(" "))
            {
                result = await _db.Levels
                    .Where(x => x.Name == levelNameOrId && x.VerifiedUpload)
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
                    .ToListAsync();
            }
            else
            {
                result = await _db.Levels
                   .Where(x => x.LevelId == levelNameOrId && x.VerifiedUpload)
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
                   .ToListAsync();
            }

            //Find creator username
            foreach (LevelInfoDTO levelInfo in result)
            {
                levelInfo.Creator = await _db.Users.Where(u => u.UserId == levelInfo.CreatorUserId)
                    .Select(u => u.Nickname)
                    .SingleOrDefaultAsync();

                levelInfo.Difficulty = await _levelStatsService.CalculateDifficulty(levelInfo.LevelId);
            }

            return Ok(new LevelSearchResponse(result));
        }
    }
}
