using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFServer.Configs;
using SFServer.Contexts;
using SFServer.Models.Configs;
using SFServer.Models.DB;
using SFServer.Models.DTO;
using SFServer.Models.Responses.Endless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Services
{
    public class EndlessService
    {
        private PostgreSqlContext _db;
        private TransactionService _transactionService;
        private readonly GameConfig _gameConfig;
        private readonly VersionConfig _versionConfig;

        public EndlessService(PostgreSqlContext db, TransactionService transactionService, IOptions<GameConfig> gameConfig, IOptions<VersionConfig> versionConfig)
        {
            _db = db;
            _transactionService = transactionService;
            _gameConfig = gameConfig.Value;
            _versionConfig = versionConfig.Value;
        }

        public async Task<EndlessChallenge> GetChallenge(User user)
        {
            return await _db.EndlessChallenge.Where(x => x.UserId == user.UserId).OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync<EndlessChallenge>();
        }

        public async Task<EndlessChallenge> GenerateChallenge(User user)
        {
            List<string> levelsToExclude = await _db.EndlessChallenge.Where(x => x.UserId == user.UserId).Select(x => x.LevelId).ToListAsync();

            List<Level> possibleLevels = await _db.Levels
                .Where(x => x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)) && !levelsToExclude.Contains(x.LevelId) && x.Wins > 0)
                .ToListAsync();

            if (possibleLevels.Count == 0)
                return null;

            Random rand = new Random();
            int toSkip = rand.Next(0, possibleLevels.Count());
            Level level = possibleLevels.Skip(toSkip).First();

            EndlessChallenge newChallenge = new EndlessChallenge()
            {
                UserId = user.UserId,
                LevelId = level.LevelId,
            };

            _db.EndlessChallenge.Add(newChallenge);
            await _db.SaveChangesAsync();

            return newChallenge;
        }

        public async Task SkipChallenge(User user)
        {
            EndlessChallenge currentChallenge = await GetChallenge(user);
            if (currentChallenge == null)
                return;

            if (currentChallenge.Completed)
                return;

            currentChallenge.Skipped = true;

            _db.UpdateEndlessChallenge(currentChallenge);
            await _db.SaveChangesAsync();
        }

        public async Task CompleteChallenge(User user)
        {
            EndlessChallenge currentChallenge = await GetChallenge(user);
            if (currentChallenge == null)
                return;

            if (currentChallenge.Skipped)
                return;

            if (currentChallenge.Completed)
                return;

            currentChallenge.Completed = true;

            user.EndlessScore++;
            user.LifetimeEndlessScore++;
            _db.UpdateUser(user);

            _db.UpdateEndlessChallenge(currentChallenge);
            await _db.SaveChangesAsync();
        }

        public async Task<int> GetRank(User user)
        {
            var leaderboard = await _db.Users
                .Where(x => x.EndlessScore > 0)
                .OrderByDescending(x => x.EndlessScore)
                .ThenByDescending(x => x.EndlessScoreLastUpdated)
                .ToListAsync();

            if (!leaderboard.Any(x => x.UserId == user.UserId))
                return -1;

            return leaderboard.FindIndex(x => x.UserId == user.UserId) + 1;
        }

        public async Task<EndlessChallengeResponse> CreateResponse(EndlessChallenge challenge)
        {
            Level level = await _db.Levels.FindAsync(challenge.LevelId);
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
                CreatedOn = level.CreatedOn.Ticks,
            };

            return new EndlessChallengeResponse(level.SerializedLevel, meta, level.Thumbnail);
        }
    }
}
