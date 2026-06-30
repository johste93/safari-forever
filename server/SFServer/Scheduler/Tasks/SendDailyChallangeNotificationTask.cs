using System;
using System.Threading;
using System.Threading.Tasks;
using SFServer.Scheduler.Scheduling;
using SFServer.Services;

namespace SFServer.Scheduler.Tasks
{
    public class SendDailyChallangeNotificationTask : IScheduledTask
    {
        public string Schedule => "30 9 * * *";

        public async Task Invoke(DailyChallengeService dailyChallangeService, CancellationToken cancellationToken)
        {
            await dailyChallangeService.SendNewDailyChallengeNotification();
        }
    }
}