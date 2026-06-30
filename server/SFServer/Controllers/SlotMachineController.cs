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
using SFServer.Models.Responses.SlotMachineResponses;
using SFServer.Models.Requests.SlotMachineRequests;

namespace SFServer.Controllers
{
    [Route("[controller]")]
    public class SlotMachineController : Controller
    {
        SlotMachineService _slotMachineService;

        public SlotMachineController(SlotMachineService slotMachineService)
        {
            this._slotMachineService = slotMachineService;
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("bets")]
        public IActionResult GetBets()
        {   
            return Ok(new GetBetsResponse(_slotMachineService.GetMinBet(), _slotMachineService.GetMaxBet()));
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ModelValidation]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpPost("spinn")]
        public async Task<IActionResult> Spinn([FromBody] SpinnRequest spinnRequest)
        {
            User user = (User)Request.HttpContext.Items["user"];

            SlotMachineResult result = await _slotMachineService.Spinn(user, spinnRequest.CoinsBet);

            return Ok(result);
        }
    }
}
