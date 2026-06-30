using DSharpPlus;
using DSharpPlus.Entities;
using SFServer.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFServer.Models.Configs;

namespace SFServer.Services
{
    public class GameStatisticsService
    {
        private PostgreSqlContext _db;
        private DiscordClient _discordClient;
        private ILogger<GameStatisticsService> _logger;
        private VersionConfig _versionConfig;

        private DiscordChannel deathCountChannel;
        private DiscordChannel levelCountChannel;
        private DiscordChannel bananaCountChannel;

        private const ulong deathCountChannelId = 723704402048974938;
        private const ulong levelCountChannelId = 726404814287798283;
        private const ulong bananaCountChannelId = 726409155824320602;

        private bool isInitalized;

        private static int deathCount;
        private static int levelCount;
        private static int bananaCount;

        public GameStatisticsService(PostgreSqlContext db, DiscordClient discordClient, IOptions<VersionConfig> versionConfig, ILoggerFactory loggerFactory)
        {
            _db = db;
            _discordClient = discordClient;
            _logger = loggerFactory.CreateLogger<GameStatisticsService>();
            _versionConfig = versionConfig.Value;
        }

        private async Task Init()
        {
            if (isInitalized)
                return;

            deathCountChannel = await _discordClient.GetChannelAsync(deathCountChannelId);
            levelCountChannel = await _discordClient.GetChannelAsync(levelCountChannelId);
            bananaCountChannel = await _discordClient.GetChannelAsync(bananaCountChannelId);

            isInitalized = true;

            await Sync();
        }

        public async Task Sync()
        {
            deathCount = await _db.Levels.SumAsync(x => x.Deaths);
            levelCount = await _db.Levels.Where(x => x.Blacklisted == false && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor))).CountAsync();
            bananaCount = await _db.Users.SumAsync(x => x.LifetimeCoins);

            //_logger.LogInformation($"Sync() DeathCount: {deathCount}");
            //_logger.LogInformation($"Sync() LevelCount: {levelCount}");
            //_logger.LogInformation($"Sync() BananaCount: {bananaCount}");
        }

        public async Task UpdateDiscord()
        {
            //_logger.LogInformation($"UpdateDiscord() DeathCount: {deathCount}");
            //_logger.LogInformation($"UpdateDiscord() LevelCount: {levelCount}");
            //_logger.LogInformation($"UpdateDiscord() BananaCount: {bananaCount}");

            await Init();
            await deathCountChannel.ModifyAsync(name: $"💀: {deathCount}");
            await levelCountChannel.ModifyAsync(name: $"🚀: {levelCount}");
            await bananaCountChannel.ModifyAsync(name: $"🍌: {bananaCount}");
        }

        public void CountDeath()
        {
            deathCount = deathCount + 1;
            //_logger.LogInformation($"CountDeath() DeathCount: {deathCount}");
        }

    }
}
