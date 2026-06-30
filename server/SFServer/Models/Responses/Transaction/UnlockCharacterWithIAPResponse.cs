using SFServer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.TransactionResponses
{
    public class UnlockCharacterWithIAPResponse
    {
        public bool PurchaseSuccessful { get; set; }
        public Animal UnlockedAnimal { get; set; }
        public PurchaseError PurchaseError { get; set; }

        public UnlockCharacterWithIAPResponse(bool purchaseSuccessful, Animal animal, PurchaseError error = PurchaseError.None)
        {
            this.PurchaseSuccessful = purchaseSuccessful;
            this.UnlockedAnimal = animal;
            this.PurchaseError = error;
        }
    }
}
