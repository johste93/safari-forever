using DSharpPlus;
using DSharpPlus.Entities;
using SFServer.Contexts;
using SFServer.Models.DB;
using SFServer.Models.Discord;
using SFServer.Scheduler.Scheduling;
using SFServer.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFServer.Scheduler.Tasks
{
    public class DiscordInviteTask : IScheduledTask
    {
        public string Schedule => "*/15 * * * *";
        private const ulong channelId = 449014097175117856;

        private DiscordChannel channel;

        public async Task Invoke(PostgreSqlContext db, DiscordClient discordClient, CancellationToken cancellationToken)
        {
            channel = await discordClient.GetChannelAsync(channelId);
            List<PersonalDiscordInvite> invites = await db.DiscordInvites.Where(x => x.Joined == false).ToListAsync();

            foreach(var i in invites)
            {
                DiscordInvite invite = await discordClient.GetInviteByCodeAsync(i.PersonalDiscordInviteId);
                if(invite.Uses > 0)
                {
                    //User has joined
                    i.Joined = true;
                    db.UpdateDiscordInvite(i);

                    await UnlockPingo(db, discordClient, i.UserId);
                }
            }

            await db.SaveChangesAsync();
        }

        private async Task UnlockPingo(PostgreSqlContext db, DiscordClient discordClient, string userId)
        {
            //If user has not unlocked pingo. Give them pingo
            UserSaveData save = await db.UserSaveData.Where(x => x.UserId == userId).FirstOrDefaultAsync();

            if(!save.PingoUnlocked)
            {
                User user = await db.Users.Where(x => x.UserId == userId).FirstOrDefaultAsync();
                user.HasJoinedDiscord = true;
                db.UpdateUser(user);

                save.UnlockAnimal(Models.Enums.Animal.Pingo);
                db.UpdateSaveData(user, save);

                string msg = $"{user.Nickname} has adopted Pingo!👏";     
                await discordClient.SendMessageAsync(channel, msg);
            }
        }
    }
}
