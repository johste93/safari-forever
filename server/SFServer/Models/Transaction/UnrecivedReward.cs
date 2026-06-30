using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Transaction
{
    public class UnrecivedReward
    {
        public string TransactionId { get; set; }
        public int ChangeInBalance { get; set; }
        public int BalanceBefore { get; set; }
        public int BalanceAfter { get; set; }

        public TransactionType TransactionType { get; set; }

        public string LevelName { get; set; }
        public DateTimeOffset RecivedOn { get; set; }
        public string Description { get; set; }
    }
}
