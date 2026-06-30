using System;
using System.Collections.Generic;
using SFServer.Models.DTO;

namespace SFServer.Models.Responses.Browser
{
    public class PopularFeedResponse
    {
        public int LevelsPrPage { get; set; }
        public List<LevelInfoDTO> Levels { get; set; }
    }
}
