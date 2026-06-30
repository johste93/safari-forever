using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Services.iAP.Google
{
    public class GoogleReceiptResponse
    {
        public AcknowledgementState acknowledgementState    { get; set; }
        public ConsumptionState consumptionState            { get; set; }
        public string developerPayload                      { get; set; }
        public string kind                                  { get; set; }
        public string orderId                               { get; set; }
        public PurchaseState purchaseState                  { get; set; }
        public long purchaseTimeMillis                      { get; set; }
        public PurchaseType purchaseType                    { get; set; }
        public ReceiptError error                           { get; set; }
    }
}