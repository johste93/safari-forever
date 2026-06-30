using System;
using System.Collections.Generic;
using System.Linq;
using SFServer.Security;
using SFServer.Models.Requests.UserRequests;
using SFServer.Models.DB;

namespace SFServer.Models.Requests.Misc
{
    public class RightToAccessRequest
    {
        public string UserId { get; set; }
        public List<string> Tokens { get; set; } //Redacted
        public string Role { get; set; }

        public string Nickname { get; set; }
        public int Identifier { get; set; }
        public string Color { get; set; }
        public bool Banned { get; set; }

        public int Coins { get; set; }
        public int LifetimeCoins { get; set; }
        public int CoinsSpentOnLottery { get; set; }

        public int EndlessScore { get; set; }
        public int LifetimeEndlessScore { get; set; }
        public DateTimeOffset EndlessScoreLastUpdated { get; set; }

        public int DailyChallengesWon { get; set; }

        public int FollowerCount { get; set; }
        public List<FollowingDTO> Followed { get; set; }

        public List<string> Levels { get; set; }
        public List<LevelUserStats> UserStats { get; set; }
        public UserSaveData UserSaveData { get; set; }

        public List<DB.Transaction> Transactions { get; set; }

        public bool Notification_FollowedPlayedLevel { get; set; }
        public bool Notification_FollowedLikedLevel { get; set; }
        public bool Notification_MyLevelIsLevelOfTheWeek { get; set; }
        public bool Notification_NewFollower { get; set; }
        public bool Notification_WorldRecordBeaten { get; set; }
        public bool Notification_NewDailyChallenge { get; set; }
        public bool Notification_DailyChallengeResults { get; set; }
        public bool Notification_FollowedPublishedLevel { get; set; }

        public List<TermsOfServiceAgreement> TermsOfServiceStatus{ get; set; }
        public List<PrivacyPolicyAgreement> PrivacyPolicyStatus { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastActive { get; set; }

        public RightToAccessRequest(User user, List<LevelUserStats> LevelUserStats, List<TermsOfServiceAgreement> tOSStatus, List<PrivacyPolicyAgreement> pPStatus, List<DB.Transaction> transactions )
        {
            UserId = user.UserId;
            Tokens = new List<string>();
            foreach (Token t in user.Tokens)
                Tokens.Add("[Redacted]");
            Role = user.Role.ToString();

            Nickname = user.Nickname;
            Identifier = user.Identifier;
            Color = user.Color;
            Banned = user.Banned;

            Coins = user.Coins;
            LifetimeCoins = user.LifetimeCoins;
            CoinsSpentOnLottery = user.CoinsSpentOnLottery;

            EndlessScore = user.EndlessScore;
            LifetimeEndlessScore = user.LifetimeEndlessScore;
            EndlessScoreLastUpdated = user.EndlessScoreLastUpdated;

            DailyChallengesWon = user.DailyChallengesWon;

            FollowerCount = user.FollowerCount;
            Followed = user.Following.Select(x => new FollowingDTO{ UserId = x.UserBeingFollowedId, CreatedOn = x.CreatedOn }).ToList();

            Levels = user.Levels.Select(x => x.LevelId).ToList();
            UserStats = LevelUserStats;
            UserSaveData = user.SaveData;

            Transactions = transactions;

            Notification_FollowedPlayedLevel = user.Notification_FollowedPlayedLevel;
            Notification_FollowedLikedLevel = user.Notification_FollowedLikedLevel;
            Notification_MyLevelIsLevelOfTheWeek = user.Notification_MyLevelIsLevelOfTheWeek;
            Notification_NewFollower = user.Notification_NewFollower;
            Notification_WorldRecordBeaten = user.Notification_WorldRecordBeaten;
            Notification_NewDailyChallenge = user.Notification_NewDailyChallenge;
            Notification_DailyChallengeResults = user.Notification_DailyChallengeResults;
            Notification_FollowedPublishedLevel = user.Notification_FollowedPublishedLevel;

            TermsOfServiceStatus = tOSStatus;
            PrivacyPolicyStatus = pPStatus;

            UpdatedOn = user.UpdatedOn;
            CreatedOn = user.CreatedOn;
            LastActive = user.LastActive;
        }

        public struct FollowingDTO
        {
            public string UserId { get; set; }
            public DateTimeOffset CreatedOn { get; set; }
        }
    }
}