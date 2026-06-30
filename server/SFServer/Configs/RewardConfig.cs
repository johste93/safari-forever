using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Configs
{
    public class RewardConfig
    {
        //public int CompletedOldUserLevel { get; set; }
        //public int CompletedNewUserLevel { get; set; }
        public int RecivedLike { get; set; }
        public int MaxLikeRewards { get; set; }
        //public int CompletedPromotedLevel { get; set; }
        //public int CompletedCampaignLevel { get; set; }
        //public int DailyChallengeParticipation { get; set; }  
        public int DailyChallengeThirdPlace { get; set; }
        public int DailyChallengeSecondPlace { get; set; }
        public int DailyChallengeFirstPlace { get; set; }

        public int CompletedLevelBaseReward { get; set; }
        public int CompletedNewUserLevelMultipler { get; set; }
        public int DailyChallengeParticipationMultiplier { get; set; }
    }
}
