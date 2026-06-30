using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.DailyChallenge
{
    public class DailyChallengeParticipant
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string DailyChallengeParticipantId { get; set; }

        public string DailyChallengeId { get; set; }
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public int Seconds { get; set; } = -1;
        public int Milliseconds { get; set; } = -1;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;

        [Obsolete] public double Time { get; set; }
    }
}
