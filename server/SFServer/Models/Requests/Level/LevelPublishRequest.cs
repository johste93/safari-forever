using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using SFServer.Utility;
using SFServer.Models.DB;

namespace SFServer.Models.Requests.LevelRequests
{
    public class LevelPublishRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string GameVersion { get; set; }
        [Required]
        public string SerializedLevel { get; set; }
        [Required]
        public string Thumbnail { get; set; }
        
        public string MiniThumbnail { get; set; }

        public Level ToLevel(string levelId, string creatorUserId)
        {
            Level result = new Level();

            result.Name = Name;
            result.LevelId = levelId;
            result.CreatorUserId = creatorUserId;
            result.SerializedLevel = SerializedLevel;
            result.Record_Seconds = -1;
            result.Record_Milliseconds = -1;

            result.GameVersion = GameVersion;

            if (ClientVersion.Parse(GameVersion, out ClientVersion parsedVersion))
            {
                result.MajorGameVersion = parsedVersion.Major;
                result.MinorGameVersion = parsedVersion.Minor;
            }

            result.Thumbnail = Convert.FromBase64String(Thumbnail);
            result.MiniThumbnail = Convert.FromBase64String(MiniThumbnail);

            JObject jsonLevel = JsonConvert.DeserializeObject<JObject>(SerializedLevel);
            result.MainColor = (string)jsonLevel["palette"]["main"]["hex"];
            result.SubColor = (string)jsonLevel["palette"]["main"]["hex"];
            result.WallColor = (string)jsonLevel["palette"]["main"]["hex"];
            result.PatternColor = (string)jsonLevel["palette"]["main"]["hex"];

            return result;
        }
    }
}
