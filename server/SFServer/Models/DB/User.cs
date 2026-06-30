using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SFServer.Security;
using System.ComponentModel.DataAnnotations.Schema;
using SFServer.Models.Enums;

namespace SFServer.Models.DB
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserId { get; set; }

        public List<Token> Tokens { get; set; }
        public string RestoreToken { get; set; }

        public UserRole Role { get; set; }

        public string Nickname { get; set; }
        public int Identifier { get; set; }
        public string Color { get; set; }
        public bool Banned { get; set; }
        public bool HasJoinedDiscord { get; set; }
        public List<Level> Levels { get; set; }
        public UserSaveData SaveData { get; set; }

        public int Coins { get; set; }
        public int LifetimeCoins { get; set; }
        public int CoinsSpentOnLottery { get; set; }
        public int CoinsSpentOnCards { get; set; }

        public int CardTable { get; set; }

        public int EndlessScore { get; set; }
        public int LifetimeEndlessScore { get; set; }
        public DateTimeOffset EndlessScoreLastUpdated { get; set; }

        public int DailyChallengesWon { get; set; }

        public int FollowerCount { get; set; }
        public List<Following> Following { get; set; }

        public bool Notification_FollowedPlayedLevel { get; set; } = false;
        public bool Notification_FollowedLikedLevel { get; set; } = false;
        public bool Notification_MyLevelIsLevelOfTheWeek { get; set; } = false;
        public bool Notification_NewFollower { get; set; } = false;
        public bool Notification_WorldRecordBeaten { get; set; } = false;
        public bool Notification_NewDailyChallenge { get; set; } = false;
        public bool Notification_DailyChallengeResults { get; set; } = false;
        public bool Notification_FollowedPublishedLevel { get; set; } = false;

        [Obsolete ("Use TermsOfServiceAgreement table instead")] public DateTimeOffset? AgreedToTermsOfService { get; set; }

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset LastActive { get; set; } = DateTimeOffset.Now;

        public User()
        {
            Tokens = new List<Token>();
            Levels = new List<Level>();
            Following = new List<Following>();
        }
    }
}
