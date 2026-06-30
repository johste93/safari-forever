using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Services.iAP.Apple
{
    public class AppStoreResponseStatus
    {
        public static string GetExplaination(int status)
        {
            switch(status)
            {
                default:
                case 0: return "No explaination";
                case 21000: return "The request to the App Store was not made using the HTTP POST request method.";
                case 21001: return "This status code is no longer sent by the App Store.";
                case 21002: return "The data in the receipt-data property was malformed or the service experienced a temporary issue. Try again.";
                case 21003: return "The receipt could not be authenticated.";
                case 21004: return "The shared secret you provided does not match the shared secret on file for your account.";
                case 21005: return "The receipt server was temporarily unable to provide the receipt. Try again.";
                case 21006: return "This receipt is valid but the subscription has expired. When this status code is returned to your server, the receipt data is also decoded and returned as part of the response. Only returned for iOS 6-style transaction receipts for auto-renewable subscriptions.";
                case 21007: return "This receipt is from the test environment, but it was sent to the production environment for verification.";
                case 21008: return "This receipt is from the production environment, but it was sent to the test environment for verification. ";
                case 21009: return "Internal data access error. Try again later.";
                case 21010: return "The user account cannot be found or has been deleted.";
            }
        }
    }
}
