using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.LevelResponses
{
    public class LevelPublishResponse
    {
        public string LevelId { get; set; }

        public LevelPublishResponse(string levelId)
        {
            this.LevelId = levelId;
        }
    }
}
