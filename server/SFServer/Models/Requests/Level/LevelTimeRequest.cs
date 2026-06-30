using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.LevelRequests
{
    public class LevelTimeRequest
    {
        [Required]
        public string Time { get; set; }
    }
}
