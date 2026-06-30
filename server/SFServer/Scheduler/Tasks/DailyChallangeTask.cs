using System;
using System.Threading;
using System.Threading.Tasks;
using SFServer.Scheduler.Scheduling;
using SFServer.Services;

namespace SFServer.Scheduler.Tasks
{
    public class DailyChallengeTask : IScheduledTask
    {
        public string Schedule => "0 8 * * *";

        public async Task Invoke(DailyChallengeService dailyChallangeService, CancellationToken cancellationToken)
        {
            await dailyChallangeService.GenerateNewDailyChallenge();
        }
    }
}