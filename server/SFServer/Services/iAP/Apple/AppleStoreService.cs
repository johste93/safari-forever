using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace SFServer.Services.iAP.Apple
{
    public class AppleAppStoreService
    {
        private HttpClient client;
        private ILogger<AppleAppStoreService> _logger;

        private const string verifyReceiptEndpoint = "https://buy.itunes.apple.com/verifyReceipt";
        private const string verifyReceiptSandboxEndpoint = "https://sandbox.itunes.apple.com/verifyReceipt";

        public AppleAppStoreService(ILoggerFactory loggerFactory)
        {
            client = new HttpClient();
            _logger = loggerFactory.CreateLogger<AppleAppStoreService>();
        }

        public async Task<AppleReceiptResponse> VerifyReceipt(string base64Receipt)
        {
            StringContent body = new StringContent($"{{ \"receipt-data\": \"{ base64Receipt }\" }}");

            //Verify your receipt first with the production URL; then verify with the sandbox URL if you receive a 21007 status code.
            //This approach ensures you do not have to switch between URLs while your application is tested, reviewed by App Review, or live in the App Store.

            var response = await client.PostAsync(verifyReceiptEndpoint, body);
            if (response == null)
                return HandleError(500, "Could not understand Apple's response.");

            string content = await response.Content.ReadAsStringAsync();

            AppleReceiptResponse verifyReceiptResponse = JsonConvert.DeserializeObject<AppleReceiptResponse>(content);

            if (verifyReceiptResponse == null)
                return HandleError(500, "Could not understand Apple's response.");

            if (verifyReceiptResponse.status == 21007)
            {
                //Is sandbox receipt. 
                var sandboxResponse = await client.PostAsync(verifyReceiptSandboxEndpoint, body);
                if (sandboxResponse == null)
                    return HandleError(500, "Could not understand Apple's response.");

                string sandboxContent = await sandboxResponse.Content.ReadAsStringAsync();

                AppleReceiptResponse sandboxVerifyReceiptResponse = JsonConvert.DeserializeObject<AppleReceiptResponse>(sandboxContent);

                if (sandboxVerifyReceiptResponse == null)
                    return HandleError(500, "Could not understand Apple's response.");

                return sandboxVerifyReceiptResponse;
            }

            return verifyReceiptResponse;
        }

        private AppleReceiptResponse HandleError(int statusCode, string msg)
        {
            _logger.LogError($"statusCode: {statusCode} - {msg}");
            return new AppleReceiptResponse() { status = statusCode };
        }
    }
}
