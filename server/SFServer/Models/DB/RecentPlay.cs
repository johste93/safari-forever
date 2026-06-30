using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFServer.Models.DB
{
    public class RecentPlay
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string RecentPlayId { get; set; }

        public string LevelId { get; set; }
        public string UserId { get; set; }

        public bool HasWon { get; set; }

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

        public RecentPlay()
        {
        }
    }
}
