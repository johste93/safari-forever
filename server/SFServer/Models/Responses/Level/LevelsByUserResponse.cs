using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.DTO;

namespace SFServer.Models.Responses.LevelResponses
{
    public class LevelsByUserResponse
    {
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public int LevelsPrPage { get; set; }
        public List<LevelInfoDTO> Levels { get; set; }
    }
}
