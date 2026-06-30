using DSharpPlus;
using DSharpPlus.Entities;
using SFServer.Contexts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.Discord;

namespace SFServer.Services
{
    public class DiscordService
    {
        private DiscordClient _discordClient;
        private PostgreSqlContext _db;

        public DiscordService(DiscordClient discordClient, PostgreSqlContext db)
        {
            _discordClient = discordClient;
            _db = db;
        }

        public async Task<DiscordMessage> PostToDiscord(ulong channelId, string content = null, Discord​Embed​Builder embed = null)
        {
            DiscordChannel channel = await _discordClient.GetChannelAsync(channelId);
            return await _discordClient.SendMessageAsync(channel, content, false, embed);
        }

        public async Task<DiscordMessage> EditDiscordMessage(ulong channelId, ulong messageId, string content = default, Discord​Embed​Builder embed = default)
        {
            DiscordChannel channel = await _discordClient.GetChannelAsync(channelId);
            DiscordMessage message = await channel.GetMessageAsync(messageId);
            return await message.ModifyAsync(content, embed);
        }

        public async Task<string> GenerateUniqueInviteId(ulong channelId, string userId)
        {
            PersonalDiscordInvite personalInvite = _db.DiscordInvites.Where(x => x.UserId == userId).FirstOrDefault();

            if (personalInvite == null)
            {
                DiscordChannel channel = await _discordClient.GetChannelAsync(channelId);
                DiscordInvite invite = await channel.CreateInviteAsync(default, default, default, true, "Personal invite created by server");

                personalInvite = new PersonalDiscordInvite()
                {
                    PersonalDiscordInviteId = invite.Code,
                    UserId = userId
                };

                _db.DiscordInvites.Add(personalInvite);
                await _db.SaveChangesAsync();
            }
            

            return personalInvite.PersonalDiscordInviteId;
        }
    }
}
