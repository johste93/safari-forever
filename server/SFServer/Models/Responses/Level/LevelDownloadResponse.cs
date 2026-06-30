using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SFServer.Models.DTO;

namespace SFServer.Models.Responses.LevelResponses
{
    public class LevelDownloadResponse
    {
        public JObject SerializedLevel { get; set; }
        public PublishedLevelMetaDTO PublishedLevelMeta { get; set; }

        public LevelDownloadResponse(string serializedLevel, PublishedLevelMetaDTO PublishedLevelMetaDTO)
        {
            this.SerializedLevel = JObject.Parse(serializedLevel);
            this.PublishedLevelMeta = PublishedLevelMetaDTO;
        }
    }
}
