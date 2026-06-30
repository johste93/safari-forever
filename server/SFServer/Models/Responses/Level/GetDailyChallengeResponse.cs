using System;
using SFServer.Models.DTO;

namespace SFServer.Models.Responses.LevelResponses
{
    public class GetDailyChallengeResponse
    {
        public bool HasChampion { get; set; }
        public string CurrentLeaderNickname { get; set; }
        public string CurrentHighscore { get; set; }

        public LevelInfoDTO Level { get; set; }
    }
}
