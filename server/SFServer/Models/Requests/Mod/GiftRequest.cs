using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.Mod
{
    public class GiftRequest
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
