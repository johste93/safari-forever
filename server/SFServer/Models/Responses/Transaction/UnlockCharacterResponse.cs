using SFServer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.TransactionResponses
{
    public class UnlockCharacterResponse
    {
        public bool PurchaseSuccessful { get; set; }
        public int RemainingCoins { get; set; }
        public PurchaseError PurchaseError { get; set; }

        public UnlockCharacterResponse(bool purchaseSuccessful, int remainingCoins, PurchaseError error = PurchaseError.None)
        {
            this.PurchaseSuccessful = purchaseSuccessful;
            this.RemainingCoins = remainingCoins;
            this.PurchaseError = error;
        }
    }
}
