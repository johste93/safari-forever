using SFServer.Models.Enums;
using SFServer.Models.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.DB
{
    public class Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TransactionId { get; set; }

        public string UserId { get; set; }

        public int ChangeInBalance { get; set; }
        public int BalanceBefore { get; set; }
        public int BalanceAfter { get; set; }

        public Hat HatUnlocked { get; set; } = Hat.None;

        public string Description { get; set; }
        public TransactionType TransactionType { get; set; }
        public string LevelName { get; set; }

        public bool RecivedByPlayer { get; set; }

        public bool Refunded { get; set; }

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    }
}
