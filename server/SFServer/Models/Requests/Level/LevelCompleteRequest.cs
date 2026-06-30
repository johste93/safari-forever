using System.ComponentModel.DataAnnotations;

namespace SFServer.Models.Requests.LevelRequests
{
    public class LevelCompleteRequest
    {
        [Required]
        public int? Deaths { get; set; }
        public int? Jumps { get; set; }
        public int? Seconds { get; set; }
        public int? Milliseconds { get; set; }
    }
}