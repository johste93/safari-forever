using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.Mod
{
    public class BlacklistRequest
    {
        [Required]
        public string LevelId { get; set; }
        [Required]
        public bool Blacklisted { get; set; }
    }
}
