using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFServer.Contexts;
using SFServer.Filters;
using SFServer.Models.Configs;
using SFServer.Models.DB;
using SFServer.Models.DTO;
using SFServer.Models.Responses.SearchResponses;
using SFServer.Services;
using SFServer.Models.Enums;
using SFServer.Configs;
using SFServer.Models.Requests.CardsRequests;
using SFServer.Models.Responses.CardsResponses;

namespace SFServer.Controllers
{
    [Route("[controller]")]
    public class CardsController : Controller
    {
        private CardsService _cardsService;

        public CardsController(CardsService cardsService)
        {
            this._cardsService = cardsService;
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("selection")]
        public async Task<IActionResult> GetCardsSelection()
        {
            User user = (User)Request.HttpContext.Items["user"];

            CardsSelectionResponse response = await _cardsService.GetCardsSelection(user);

            return Ok(response);
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("pick")]
        public async Task<IActionResult> PickCard([FromBody] PickCardRequest pickCardRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            PickCardResponse response = await _cardsService.PickCard(user, pickCardRequest.CardPickedIndex);

            return Ok(response);
        }
    }
}
