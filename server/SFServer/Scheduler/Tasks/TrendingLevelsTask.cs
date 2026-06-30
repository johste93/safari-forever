using System;
using System.Threading;
using System.Threading.Tasks;
using SFServer.Contexts;
using SFServer.Scheduler.Scheduling;
using SFServer.Services;
using SFServer.Models.DB;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace SFServer.Scheduler.Tasks
{
    public class TrendingLevelsTask : IScheduledTask
    {
        public string Schedule => "45 * * * *";

        public async Task Invoke(PostgreSqlContext db, CancellationToken cancellationToken)
        {
            List<string> LevelIds = await db.RecentPlays
                .Select(x => x.LevelId)
                .Distinct()
                .ToListAsync();

            await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE public.\"TrendingLevels\"");

            foreach (string levelId in LevelIds)
            {
                int count = await db.RecentPlays.Select(x => x.LevelId).Where(x => x == levelId).CountAsync();
                await db.TrendingLevels.AddAsync(new TrendingLevel() { LevelId = levelId, Score = count });
            }
            
            await db.SaveChangesAsync();
        }
        
    }
}
