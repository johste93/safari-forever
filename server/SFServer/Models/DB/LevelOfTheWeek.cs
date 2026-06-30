using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFServer.Models.DB
{
    public class LevelOfTheWeek
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string LevelOfTheWeekId { get; set; }

        public string LevelId { get; set; }

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
