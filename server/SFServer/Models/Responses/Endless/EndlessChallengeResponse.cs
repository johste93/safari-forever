using Newtonsoft.Json.Linq;
using SFServer.Models.DTO;
using SFServer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.Endless
{
    public class EndlessChallengeResponse
    {
        public JObject SerializedLevel { get; set; }
        public PublishedLevelMetaDTO PublishedLevelMeta { get; set; }
        public EndlessError Error { get; set; }
        public byte[] Thumbnail { get; set; }

        public int PersonalScore { get; set; }

        public int RerollCost { get; set; }

        public EndlessChallengeResponse() { }

        public EndlessChallengeResponse(string serializedLevel, PublishedLevelMetaDTO publishedLevelMetaDTO, byte[] thumbnail)
        {
            this.SerializedLevel = JObject.Parse(serializedLevel);
            this.PublishedLevelMeta = publishedLevelMetaDTO;
            this.Thumbnail = thumbnail;
        }
    }
}
