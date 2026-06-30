using System;
using System.Threading;
using System.Threading.Tasks;
using SFServer.Scheduler.Scheduling;

namespace SFServer.Scheduler.Tasks
{
    public class TaskTemplate : IScheduledTask
    {
        public string Schedule => "0 2 * * *";

        public async Task Invoke(CancellationToken cancellationToken)
        {
            Console.WriteLine();
            Console.WriteLine("Scheduled: " + DateTimeOffset.Now);
            Console.WriteLine();
        }
    }
}
