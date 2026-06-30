using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFServer.Services.iAP.Apple
{
    public class AppleReceiptResponse
    {
        /// <summary>
        /// The environment for which the receipt was generated.
        /// </summary>
        public string environment { get; set; }

        /// <summary>
        /// An indicator that an error occurred during the request. A value of 1 indicates a temporary issue; retry validation for this receipt at a later time. 
        /// A value of 0 indicates an unresolvable issue; do not retry validation for this receipt. Only applicable to status codes 21100-21199.
        /// </summary>
        [JsonProperty("is-retryable")]
        public bool isRetryable { get; set; }

        /// <summary>
        /// A JSON representation of the receipt that was sent for verification. 
        /// </summary>
        public AppStoreReceipt receipt { get; set; }

        /// <summary>
        /// Either 0 if the receipt is valid, or a status code if there is an error. 
        /// The status code reflects the status of the app receipt as a whole. See status for possible status codes and descriptions.
        /// </summary>
        public int status { get; set; }
    }
}
