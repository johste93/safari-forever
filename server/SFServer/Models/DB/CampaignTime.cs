using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.DB
{
    public class CampaignTime
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CampaignTimeId { get; set; }

        public int World { get; set; }
        public int LevelIndex { get; set; }
        public string UserId { get; set; }
        public int Seconds { get; set; } = -1;
        public int Milliseconds { get; set; } = -1;

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

        [Obsolete] public double Time { get; set; }
    }
}
