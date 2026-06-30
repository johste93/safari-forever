using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.DB
{
    public class UserCampaignLevelData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string UserCampaignLevelDataId { get; set; }

        [ForeignKey("UserSaveData")]
        public string UserSaveDataId { get; set; }

        public int World { get; set; }
        public int Index { get; set; }

        public bool Beaten { get; set; }
        
        public int Seconds { get; set; } = -1;
        public int Milliseconds { get; set; } = -1;

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

        [Obsolete] public double PersonalHighscore { get; set; } = -1;
    }
}
