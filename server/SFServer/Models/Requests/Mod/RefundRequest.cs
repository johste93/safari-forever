using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.Mod
{
    public class RefundRequest
    {
        [Required]
        public string TransactionId { get; set; }
    }
}
