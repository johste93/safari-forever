using SFServer.Scheduler.Scheduling;
using SFServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFServer.Scheduler.Tasks
{
    public class NewLevelOfTheWeekTask : IScheduledTask
    {
        public string Schedule => "0 11 * * 5";

        public async Task Invoke(NewLevelOfTheWeekService newLevelOfTheWeekService, CancellationToken cancellationToken)
        {
            await newLevelOfTheWeekService.PostNewLevelOfTheWeek();
        }
    }

}
