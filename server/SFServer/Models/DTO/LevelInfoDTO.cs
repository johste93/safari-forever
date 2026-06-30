using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SFServer.Models.Enums;

namespace SFServer.Models.DTO
{
    public class LevelInfoDTO
    {
        public string LevelId { get; set; }
        public string Name { get; set; }
        public string CreatorUserId { get; set; }
        public string Creator { get; set; }
        public string GameVersion { get; set; }
        public int CoinsInvested { get; set; }
        public int Likes { get; set; }
        public int Plays { get; set; }
        public int RewardMultipler { get; set; } = 1;
        public bool CanBeBoosted { get; set; }
        public bool HasGraduated { get; set; }
        public int BoostedRank { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public byte[] Thumbnail { get; set; }
        public byte[] MiniThumbnail { get; set; }
        public Difficulty Difficulty { get; set; }
    }
}
