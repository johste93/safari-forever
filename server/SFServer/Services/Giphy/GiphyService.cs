using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RestSharp;
using Newtonsoft.Json;

namespace SFServer.Services.Giphy
{
    public class GiphyService
    {
        private RestClient uploadClient;
        private RestClient getByIdClient;
        private ILogger<GiphyService> _logger;
        private const string giphyUploadUrl = "https://upload.giphy.com/v1/gifs";
        private const string giphyGetGifByIdUrl = "https://api.giphy.com/v1/gifs/";

        public GiphyService(ILoggerFactory loggerFactory)
        {
            uploadClient = new RestClient(giphyUploadUrl);
            getByIdClient = new RestClient(giphyGetGifByIdUrl);
            _logger = loggerFactory.CreateLogger<GiphyService>();
        }

        public async Task<GiphyResponse> Upload(byte[] gifBytes)
        {
            var request = new RestRequest(Method.POST);


            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddParameter("api_key", Startup.Configuration["GIPHY_API_KEY"], ParameterType.GetOrPost);
            request.AddParameter("tags", "safariforever,safari forever", ParameterType.GetOrPost);
            request.AddParameter("username", "SafariForever", ParameterType.GetOrPost);
            request.AddFile("file", gifBytes, "player_recording");

            var restResponse = await uploadClient.ExecuteTaskAsync(request);

            if(!restResponse.IsSuccessful)
            {
                return HandleError((int) restResponse.StatusCode, restResponse.ErrorMessage);
            }

            GiphyResponse giphyResponse = JsonConvert.DeserializeObject<GiphyResponse>(restResponse.Content);

            if(giphyResponse == null)
            {
                return HandleError(500, "Could not understand Giphy's response.");
            }

            return giphyResponse;
        }

        public async Task<GiphyResponse> GetGifById(string gif_Id)
        {
            var request = new RestRequest(gif_Id, Method.GET);

            _logger.LogInformation(getByIdClient.BuildUri(request).ToString());

            request.AddParameter("api_key", Startup.Configuration["GIPHY_API_KEY"], ParameterType.GetOrPost);

            var restResponse = await getByIdClient.ExecuteTaskAsync(request);

            if (!restResponse.IsSuccessful)
            {
                return HandleError((int)restResponse.StatusCode, restResponse.ErrorMessage);
            }

            GiphyResponse giphyResponse = JsonConvert.DeserializeObject<GiphyResponse>(restResponse.Content);

            if (giphyResponse == null)
            {
                return HandleError(500, "Could not understand Giphy's response.");
            }

            return giphyResponse;
        }

        private GiphyResponse HandleError(int statusCode, string msg)
        {
            GiphyResponse errorResponse = new GiphyResponse();
            errorResponse.Meta = new GiphyMeta();
            errorResponse.Meta.Status = statusCode;
            errorResponse.Meta.Msg = msg;
            return errorResponse;
        }
    }
}