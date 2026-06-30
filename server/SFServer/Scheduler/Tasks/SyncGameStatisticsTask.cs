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
    public class SyncGameStatisticsTask : IScheduledTask
    {
        public string Schedule => "0 * * * *";

        public async Task Invoke(GameStatisticsService gameStatisticsService, CancellationToken cancellationToken)
        {
            await gameStatisticsService.Sync();
        }
    }
}
