using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.Mod
{
    public class GiftResponse
    {
        public string UserId { get; set; }
        public string TransactionId { get; set; }
        public int Amount { get; set; }
        public int BalanceBefore { get; set; }
        public int BalanceAfter { get; set; }
        public int ChangeInBalance { get; set; }
    }
}
