using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.Enums;

namespace SFServer.Models.DB
{
    public class LevelUserStats
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string LevelUserStatsId { get; set; }

        public string LevelId { get; set; }
        public string UserId { get; set; }
        public LevelOpinion Opinion { get; set; }
        public bool HasGivenLikeBefore { get; set; }
        public int Attempts { get; set; } = -1;
        public int Deaths { get; set; } = -1;
        public int Jumps { get; set; } = -1;
        public int Seconds { get; set; } = -1;
        public int Milliseconds { get; set; } = -1;
        public DateTimeOffset HighscoreUpdatedOn { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

        [Obsolete] public double bestTime { get; set; } = -1;

        [NotMapped]
        public bool Completed
        {
            get
            {
                return Seconds >= 0 && Milliseconds >= 0;
            }
        }

        public LevelUserStats()
        {}

        public LevelUserStats(string levelId, string UserId)
        {
            this.LevelId = levelId;
            this.UserId = UserId;
        }
    }
}
