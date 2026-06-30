using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.DTO
{
    public class PublishedLevelMetaDTO
    {
        public string LevelId { get; set; }
        public string Name { get; set; }

        public string CreatorUserId { get; set; }
        public string CreatorUserName { get; set; }

        public int Deaths { get; set; }
        public int Wins { get; set; }
        public int Likes { get; set; }
        public int Plays { get; set; }
        public int CoinsInvested { get; set; }
        public bool CanBeBoosted { get; set; }
        public bool HasGraduated { get; set; }

        public string GameVersion { get; set; }
        public string ShareUrl { get; set; }
        public long CreatedOn { get; set; }
    }
}
