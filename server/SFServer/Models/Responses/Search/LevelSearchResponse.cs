using SFServer.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.SearchResponses
{
    public class LevelSearchResponse
    {
        public List<LevelInfoDTO> levels { get; set; }

        public LevelSearchResponse(List<LevelInfoDTO> levels)
        {
            this.levels = levels;
        }
    }
}
