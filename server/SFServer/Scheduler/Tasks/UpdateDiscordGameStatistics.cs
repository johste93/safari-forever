using System;
using System.Threading;
using System.Threading.Tasks;
using SFServer.Scheduler.Scheduling;
using SFServer.Services;

namespace SFServer.Scheduler.Tasks
{
    public class UpdateDiscordGameStatistics : IScheduledTask
    {
        public string Schedule => "*/15 * * * *";

        public async Task Invoke(GameStatisticsService gameStatisticsService, CancellationToken cancellationToken)
        {
            await gameStatisticsService.UpdateDiscord();
        }
    }
}
