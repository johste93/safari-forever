using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFServer.Configs;
using SFServer.Contexts;
using SFServer.Filters;
using SFServer.Models.Configs;
using SFServer.Models.DB;
using SFServer.Models.DTO;
using SFServer.Models.Requests.Boost;
using SFServer.Models.Responses.Boost;
using SFServer.Services;

namespace SFServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BoostController : ControllerBase
    {
        private PostgreSqlContext _db;
        private TransactionService _transactionService;
        private readonly GameConfig _gameConfig;
        private readonly VersionConfig _versionConfig;

        private BoostService _boostService;

        

        public BoostController(BoostService boostService, PostgreSqlContext db, TransactionService transactionService, IOptions<GameConfig> gameConfig, IOptions<VersionConfig> versionConfig)
        {
            _db = db;
            _transactionService = transactionService;
            _gameConfig = gameConfig.Value;
            _versionConfig = versionConfig.Value;
            _boostService = boostService;
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("ranking")]
        public async Task<IActionResult> GetRanking()
        {
            List<Tuple<int, long>> result = await _db.Levels
               .Where(x => x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)) && x.Plays < _gameConfig.MaxPlaysInNewCategory)
               .OrderByDescending(x => x.CoinsInvested)
               .ThenBy(x => x.CreatedOn)
               .Select(x => new Tuple<int, long>(x.CoinsInvested, x.CreatedOn.Ticks))
               .ToListAsync();

            return Ok(new NewRankingsResponse() { Rankings = result });
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("rank")]
        public async Task<IActionResult> GetRank([FromBody] RankRequest request)
        {
            int result = await _boostService.FindRank(request.AmountInvested, request.CreatedOn);

            return Ok(new RankResponse() { Rank = result });
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("boost")]
        public async Task<IActionResult> BoostLevel([FromBody] BoostRequest request)
        {
            Level level = await _db.Levels.FindAsync(request.LevelId);

            if(level.MajorGameVersion < _versionConfig.levelCompabilityVersion.Major || (level.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && level.MinorGameVersion < _versionConfig.levelCompabilityVersion.Minor))
                return BadRequest("Cant boost outdated level.");

            if (request.Amount < 0)
                return StatusCode(600, "Cant boost with a negative amount.");

            if (request.Amount > 999999999)
                return StatusCode(600, "Amount to large.");

            //Check if level found
            if (level == null)
                return BadRequest("Level not found.");

            //Check if level has less than Max Plays
            if (level.Plays > _gameConfig.MaxPlaysInNewCategory)
                return BadRequest("Level does not qualify for boosting.");

            User user = (User)Request.HttpContext.Items["user"];

            //Make sure player owns this level.
            if (level.CreatorUserId != user.UserId)
                return BadRequest("Level not created by user.");

            //Check if player can afford to boost level.
            if (user.Coins < request.Amount)
                return StatusCode(600, "You dont have that many bananas");

            await _transactionService.SpendCoins(user, request.Amount, $"Boosted Level: {level.LevelId}");

            level.CoinsInvested += request.Amount;

            _db.UpdateLevel(level);

            await _db.SaveChangesAsync();

            _boostService.ClearCache();

            return Ok();
        }
    }
}