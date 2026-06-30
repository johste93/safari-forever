using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFServer.Filters;
using SFServer.Models.DB;
using SFServer.Models.Responses.Discord;
using SFServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Controllers
{
    [Route("[controller]")]
    public class DiscordController : Controller
    {
        DiscordService _discordService;
        private const ulong channelId = 449014097175117856;

        public DiscordController(DiscordService discordService)
        {
            _discordService = discordService;
        }

        [Authorize(Roles = "Player,Admin,Moderator")]
        [ServiceFilter(typeof(LegalVerificationAttribute))]
        [ServiceFilter(typeof(ClientVersionValidationAttribute))]
        [HttpGet("invite")]
        public async Task<IActionResult> GetPersonalInvite()
        {
            User user = (User)Request.HttpContext.Items["user"];

            string code = await _discordService.GenerateUniqueInviteId(channelId, user.UserId);

            return Ok(new PersonalInviteResponse()
            {
                InviteUrl = $"https://discord.com/invite/{code}"
            });
        }
    }
}
