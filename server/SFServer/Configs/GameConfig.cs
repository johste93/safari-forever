using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Configs
{
    public class GameConfig
    {
        public int MaxPlaysInNewCategory { get; set; }
        public int LevelsPrPage { get; set; }
        public int NotificationsPrPage { get; set; }
        public int UsersPrPage { get; set; }
        public int EndlessLeaderboardUsersPrPage { get; set; }
        public int EndlessChallengeSkipCost { get; set; }
    }
}