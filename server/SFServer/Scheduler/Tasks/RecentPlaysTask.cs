using System;
using System.Threading;
using System.Threading.Tasks;
using SFServer.Contexts;
using SFServer.Scheduler.Scheduling;
using SFServer.Services;
using SFServer.Models.DB;
using System.Linq;

namespace SFServer.Scheduler.Tasks
{
    public class RecentPlaysTask : IScheduledTask
    {
        public string Schedule => "0 5 * * *";

        public async Task Invoke(PostgreSqlContext db, CancellationToken cancellationToken)
        {
            var oneWeekAgo = DateTimeOffset.Now.AddDays(-7);
            db.RecentPlays.RemoveRange(db.RecentPlays.Where(x => x.CreatedOn < oneWeekAgo));
            await db.SaveChangesAsync();
        }
    }
}