using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFServer.Models.DB
{
    public class TrendingLevel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TrendingLevelId { get; set; }

        public string LevelId { get; set; }
        public int Score { get; set; }

        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

        public TrendingLevel()
        {
        }
    }
}
