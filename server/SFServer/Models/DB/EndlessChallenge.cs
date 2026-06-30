using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.DB
{
    public class EndlessChallenge
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string endlessChallengeId { get; set; }
        public string UserId { get; set; }
        public string LevelId { get; set; }

        public bool Completed { get; set; }
        public bool Skipped { get; set; }

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
