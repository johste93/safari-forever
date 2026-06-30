using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.Boost
{
    public class BoostRequest
    {
        [Required]
        public string LevelId { get; set; }
        [Required]
        public int Amount { get; set; }
    }
}
