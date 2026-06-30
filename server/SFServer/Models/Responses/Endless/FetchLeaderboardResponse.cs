using SFServer.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.Endless
{
    public class FetchLeaderboardResponse
    {
        public List<EndlessLeaderboardDTO> Leaderboard { get; set; }

        public int PersonalScore { get; set; }
        public int PersonalRank { get; set; }
    }
}
