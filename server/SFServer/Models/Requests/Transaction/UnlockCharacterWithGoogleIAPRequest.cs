using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.TransactionRequests
{
    public class UnlockCharacterWithGoogleIAPRequest
    {
        public string productId { get; set; }
        public string purchaseToken { get; set; }
    }
}
