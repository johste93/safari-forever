using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.DailyChallenge
{
    public class DailyChallenge
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string DailyChallengeId { get; set; }

        public string LevelId { get; set; }
        public ulong DiscordMessageId { get; set; }
        
        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
