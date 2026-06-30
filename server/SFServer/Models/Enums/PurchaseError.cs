using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Enums
{
    public enum PurchaseError
    {
        None,
        CantAfford,
        AlreadyPurchased,
        NotAvailable
    }
}
