using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.DTO;

namespace SFServer.Models.Responses.Browser
{
    public class PreviousDailyChallengesResponse
    {
        public int LevelsPrPage { get; set; }
        public List<LevelInfoDTO> Levels { get; set; }
    }
}
