using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace SFServer.Services.iAP.Google
{
    public class GooglePlayStoreService
    {
        private HttpClient client;
        private ILogger<GooglePlayStoreService> _logger;

        private const string verifyReceiptEndpoint = "https://www.googleapis.com/androidpublisher/v3/applications/{0}/purchases/products/{1}/tokens/{2}";

        public GooglePlayStoreService(ILoggerFactory loggerFactory)
        {
            client = new HttpClient();
            _logger = loggerFactory.CreateLogger<GooglePlayStoreService>();
        }

        private async Task<string> RequestAccessToken()
        {
            var credential = GoogleCredential.FromJson(Startup.Configuration["GOOGLE_SERVICE_ACCOUNT"]).CreateScoped("https://www.googleapis.com/auth/androidpublisher");
            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

            return token;
        }

        public async Task<GoogleReceiptResponse> VerifyReceipt(string productId, string purchaseToken)
        {
            string accessToken = await RequestAccessToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string endpoint = string.Format(verifyReceiptEndpoint, "com.chumpware.safariforever", productId, purchaseToken);

            var response = await client.GetAsync(endpoint);
            if (response == null)
                return HandleError(500, "Could not understand Google's response.");

            string content = await response.Content.ReadAsStringAsync();

            GoogleReceiptResponse googlesResponse = JsonConvert.DeserializeObject<GoogleReceiptResponse>(content);

            if (googlesResponse == null)
                return HandleError(500, "Could not understand Google's response.");

            return googlesResponse;
        }

        private GoogleReceiptResponse HandleError(int statusCode, string msg)
        {
            _logger.LogError($"statusCode: {statusCode} - {msg}");
            return null;
        }
    }
}
