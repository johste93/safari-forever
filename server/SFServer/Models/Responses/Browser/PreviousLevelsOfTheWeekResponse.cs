using SFServer.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.Browser
{
    public class PreviousLevelsOfTheWeekResponse
    {
        public int LevelsPrPage { get; set; }
        public List<LevelInfoDTO> Levels { get; set; }
    }
}
