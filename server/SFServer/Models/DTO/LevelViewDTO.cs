using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.DB;
using SFServer.Models.Enums;
using SFServer.Utility;

namespace SFServer.Models.DTO
{
    public class LevelViewDTO
    {
        public string LevelId { get; set; }
        public string GameVersion { get; set; }
        public string CreatorUserId { get; set; }
        public string PathToThumbnail { get; set; }
        public string PlayButtonUrl { get; set; }
        public string WebGLUrl { get; set; }

        public string MainColor { get; set; }
        public string SubColor { get; set; }
        public string WallColor { get; set; }
        public string PatternColor { get; set; }

        public string Description { get; set; }

        public LevelViewDTO(Level level, User user, User recordHolder, string url, string playButtonUrl, Difficulty difficulty)
        {
            LevelId = level.LevelId;
            GameVersion = level.GameVersion;
            CreatorUserId = level.CreatorUserId;
            PathToThumbnail = $"{url}/level/{LevelId}/img"; //http should not be hardcoded but should instead be included in url!
            WebGLUrl = $"{url}/webgl/{LevelId}";
            PlayButtonUrl = playButtonUrl;

            MainColor = level.MainColor;
            SubColor = level.SubColor;
            WallColor = level.WallColor;
            PatternColor = level.PatternColor;

            float ClearRate = 1f;
            if (level.Wins < level.Plays)
                ClearRate = (float)level.Wins / (float)(level.Plays);

            ClearRate *= 100f;
            int clearRatePercent = (int)ClearRate;

            string highscore = "--:--";

            if(level.Record_Seconds > 0 || level.Record_Milliseconds > 0)
            {
                highscore = new Highscore(level.Record_Seconds, level.Record_Milliseconds).ToString();
            }

            Description = $"{level.Name}" +
                $"\nBy: {user.Nickname}" +
                $"\nDifficulty: {difficulty}" +
                $"\nPlays: {level.Plays}" +
                $"\nLikes: {level.Likes}" +
                $"\nClear Rate: {clearRatePercent}%" +
                $"\nBest Time: {highscore}{(recordHolder != null ? $" by {recordHolder.Nickname}" : "")}" +
                $"\nDeaths: {level.Deaths}" ;
        }
    }
}
