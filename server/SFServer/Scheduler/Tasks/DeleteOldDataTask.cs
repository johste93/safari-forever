using System;
using System.Threading;
using System.Threading.Tasks;
using SFServer.Scheduler.Scheduling;
using SFServer.Scheduler;
using SFServer.Contexts;
using System.Linq;

namespace SFServer.Scheduler.Tasks
{
    public class DeleteOldDataTask : IScheduledTask
    {
        public string Schedule => "* * * * *";

        public async Task Invoke(PostgreSqlContext db, CancellationToken cancellationToken)
        {
            Console.WriteLine("Number of users in db: " + db.Users.Count());
        }
    }
}
