using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.TransactionRequests
{
    public class UnlockCharacterWithAppleIAPRequest
    {
        public string base64Receipt { get; set; }
    }
}
