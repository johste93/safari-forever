using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFServer.Contexts;
using SFServer.Models.DB;
using SFServer.Scheduler.Scheduling;

namespace SFServer.Scheduler.Tasks
{
    public class EraseInactiveTask : IScheduledTask
    {
        public string Schedule => "0 7 * * *";

        public async Task Invoke(PostgreSqlContext db, CancellationToken cancellationToken)
        {
            DateTimeOffset referanceTime = DateTimeOffset.Now;
            DateTimeOffset oneYearAgo = referanceTime.AddYears(-1);
            List<User> inactiveUsers = await db.Users.Where(x => x.LastActive < oneYearAgo).ToListAsync();

            if (inactiveUsers != null && inactiveUsers.Count > 0)
            {
                foreach (User user in inactiveUsers)
                {
                    TimeSpan timeInactive = referanceTime - user.LastActive;
                    Console.WriteLine($"{user.Nickname} has been inactive for {timeInactive.Days} days, {timeInactive.Hours} hours and {timeInactive.Minutes} minutes.");
                }
            }
            else
            {
                Console.WriteLine($"No inactive users to erase!");
            }
        }
    }
}
