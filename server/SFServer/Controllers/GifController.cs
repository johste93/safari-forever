using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFServer.Filters;
using SFServer.Models.Requests.Gif;
using SFServer.Models.Responses.Gif;
using SFServer.Services.Giphy;

namespace SFServer.Controllers
{
    [Route("[controller]")]
    public class GifController : Controller
    {
        private GiphyService _giphyService;
        private ILogger<GifController> _logger;

        public GifController(GiphyService giphyService, ILoggerFactory logger)
        {
            _giphyService = giphyService;
            _logger = logger.CreateLogger<GifController>();
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("uploadToGiphy")]
        public async Task<IActionResult> UploadToGiphy([FromBody] UploadToGiphyRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Base64))
                return BadRequest("File missing.");

            byte[] gifBytes = System.Convert.FromBase64String(request.Base64);

            if (gifBytes.Length == 0)
                return BadRequest("Zero length file.");

            GiphyResponse uploadResponse = await _giphyService.Upload(gifBytes);

            if (uploadResponse == null || uploadResponse.Meta == null || uploadResponse.Data == null)
                return StatusCode(500, "Could not understand Giphy's response.");

            if (uploadResponse.Meta.Status < 200 || uploadResponse.Meta.Status >= 300)
                return StatusCode(uploadResponse.Meta.Status, uploadResponse.Meta.Msg);

            if (string.IsNullOrWhiteSpace(uploadResponse.Data.id))
                return StatusCode(uploadResponse.Meta.Status, uploadResponse.Meta.Msg);
                
            //Fetch the gifs short url.
            GiphyResponse getGifByIdResponse = await _giphyService.GetGifById(uploadResponse.Data.id);

            if (getGifByIdResponse == null || getGifByIdResponse.Meta == null || getGifByIdResponse.Data == null)
                return StatusCode(500, "Could not understand Giphy's response.");

            if (getGifByIdResponse.Meta.Status < 200 || getGifByIdResponse.Meta.Status >= 300)
                return StatusCode(getGifByIdResponse.Meta.Status, getGifByIdResponse.Meta.Msg);

            if (string.IsNullOrWhiteSpace(getGifByIdResponse.Data.bitly_gif_url))
                return StatusCode(getGifByIdResponse.Meta.Status, getGifByIdResponse.Meta.Msg);

            GifUploadResponse result = new GifUploadResponse()
            {
                Url = getGifByIdResponse.Data.url
            };

            return Ok(result);
        }

        private string GetShortUrl(string giphyId)
        {
            return $"https://sf4.life/g/{giphyId}";
        }
    }
}