using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.Boost
{
    public class RankRequest
    {
        [Required]
        public int AmountInvested { get; set; }
        [Required]
        public long CreatedOn { get; set; }
    }
}
