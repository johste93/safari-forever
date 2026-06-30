using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.DTO;
using SFServer.Models.DB;

namespace SFServer.Models.Responses.UserResponses
{
    public class ProfileResponse
    {
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public int Identifier { get; set; }
        public string Color { get; set; }
        public int FollowerCount { get; set; }
        public bool IsFollowed { get; set; }
        public List<LevelInfoDTO> Levels { get; set; }
        public bool AlphaAccount { get; set; }
        public bool BetaAccount { get; set; }
        public int Coins { get; set; }
        public string ShareUrl { get; set; }

        public int EndlessScore { get; set; }
        public int EndlessRank { get; set; }
        public int DailyChallengesWon { get; set; }

        public ProfileResponse(User user, List<LevelInfoDTO> levels, bool isFollowed, string shareUrl, int endlessRank)
        {
            UserId = user.UserId;
            Nickname = user.Nickname;
            Identifier = user.Identifier;
            Color = user.Color;
            Levels = levels;
            FollowerCount = user.FollowerCount;
            IsFollowed = isFollowed;
            AlphaAccount = user.CreatedOn < new DateTimeOffset(new DateTime(2020, 1, 31, 23, 59, 59));
            BetaAccount = !AlphaAccount && user.CreatedOn < new DateTimeOffset(new DateTime(2020, 6, 18, 0, 0, 0));
            ShareUrl = shareUrl;
            EndlessScore = user.EndlessScore;
            EndlessRank = endlessRank;
            DailyChallengesWon = user.DailyChallengesWon;
        }
    }
}
