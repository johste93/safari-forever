using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.DTO;

namespace SFServer.Models.DB
{
    public class Level
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string LevelId { get; set; }

        public string Name { get; set; }
        public bool Blacklisted { get; set; }
        public bool VerifiedUpload { get; set; }

        public string SerializedLevel { get; set; } //Raw Json

        //Stats
        public int Plays { get; set; }
        public int Deaths { get; set; }
        public int Wins { get; set; }
        public int Likes { get; set; }
        public int LifetimeLikes { get; set; }
        public int Dislikes { get; set; }
        public int Record_Seconds { get; set; }
        public int Record_Milliseconds { get; set; }
        public string RecordHolder { get; set; }
        public int CoinsInvested { get; set; }

        //LevelView
        public string GameVersion { get; set; }
        public int MajorGameVersion { get; set; }
        public int MinorGameVersion { get; set; }

        public string CreatorUserId { get; set; }
        public byte[] Thumbnail { get; set; }
        public byte[] MiniThumbnail { get; set; }

        public string MainColor { get; set; }
        public string SubColor { get; set; }
        public string WallColor { get; set; }
        public string PatternColor { get; set; }

        public DateTimeOffset DailyChallengeOn { get; set; } = DateTimeOffset.MinValue;
        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

        [Obsolete] public double Record { get; set; } = -1;

        public string GetShareUrl(string Scheme, string Host)
        {
            string url = $"{Scheme}://{Host}/{LevelId}";
            return url.Replace("api2", "play");
        }
    }
}
