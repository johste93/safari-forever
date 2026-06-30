using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.Mod
{
    public class RefundResponse
    {
        public string TransactionId { get; set; }
        public string UserId { get; set; }
        public bool Refunded { get; set; }
        public int BalanceBefore { get; set; }
        public int BalanceAfter { get; set; }
        public int ChangeInBalance { get; set; }
    }
}
