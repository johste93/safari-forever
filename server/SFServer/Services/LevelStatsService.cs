using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFServer.Contexts;
using SFServer.Models.DB;
using SFServer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Services
{
    public class LevelStatsService
    {
        private PostgreSqlContext _db;
        private ILogger<GameStatisticsService> _logger;

        public LevelStatsService(PostgreSqlContext db, ILoggerFactory loggerFactory)
        {
            _db = db;
            _logger = loggerFactory.CreateLogger<GameStatisticsService>();
        }

        public async Task<Difficulty> CalculateDifficulty(string levelId)
        {
            List<LevelUserStats> stats = await _db.LevelUserStats.Where(x => x.LevelId == levelId).Where(x => x.Deaths >= 0).ToListAsync();

            if(stats.Count < 5)
                return Difficulty.Unrated;

            int avergageNumberOfDeaths = stats.Sum(x => x.Deaths) / stats.Count;

            if (avergageNumberOfDeaths <= 10)
                return Difficulty.Beginner;

            if (avergageNumberOfDeaths <= 25)
                return Difficulty.Intermediate;

            if (avergageNumberOfDeaths <= 50)
                return Difficulty.Expert;

            return Difficulty.Savant;
        }
    }
}
