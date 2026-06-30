using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.DB;

namespace SFServer.Models.DTO
{
    public class UserCampaignLevelDataDTO
    {
        public int World { get; set; }
        public int Index { get; set; }

        public bool Beaten { get; set; }
        public int Seconds { get; set; }
        public int Milliseconds { get; set; }
        
        [Obsolete] public double PersonalHighscore { get; set; }

        public UserCampaignLevelDataDTO()
        {
        }

        public UserCampaignLevelDataDTO(UserCampaignLevelData userCampaignLevelData)
        {
            World = userCampaignLevelData.World;
            Index = userCampaignLevelData.Index;

            Beaten = userCampaignLevelData.Beaten;
            Seconds = userCampaignLevelData.Seconds;
            Milliseconds = userCampaignLevelData.Milliseconds;
        }
    }
}
