using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.Mod
{
    public class BanRequest
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public bool Banned { get; set; }
    }
}
