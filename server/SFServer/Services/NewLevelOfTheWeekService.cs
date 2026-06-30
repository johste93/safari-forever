using DSharpPlus;
using DSharpPlus.Entities;
using SFServer.Contexts;
using SFServer.Models.DailyChallenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models;
using System.Globalization;
using SFServer.Models.DB;
using SFServer.Models.Notifications;

namespace SFServer.Services
{
    public class NewLevelOfTheWeekService
    {
        private PostgreSqlContext _db;
        private DiscordClient _discordClient;
        private NotificationService _notificationService;
        private DiscordChannel channel;
        private const ulong discordChannelId = 606250608722444318;

        public NewLevelOfTheWeekService(PostgreSqlContext db, DiscordClient discordClient, NotificationService notificationService)
        {
            _db = db;
            _discordClient = discordClient;
            _notificationService = notificationService;
        }

        private async Task Init()
        {
            if (channel == null)
                channel = await _discordClient.GetChannelAsync(discordChannelId);
        }

        public async Task PostNewLevelOfTheWeek()
        {
            var oneWeekAgo = DateTimeOffset.Now.AddDays(-8);
            Level newLevelOfTheWeek = _db.Levels.Where(x => x.Blacklisted == false && x.VerifiedUpload && x.CreatedOn > oneWeekAgo).OrderByDescending(x => x.Likes).ThenBy(x=> x.Plays).Take(1).First();

            if (newLevelOfTheWeek == null)
            {
                Console.WriteLine("Error: No level of the week was found!");
                return;
            }

            _db.LevelsOfTheWeek.Add(new LevelOfTheWeek()
            {
                LevelId = newLevelOfTheWeek.LevelId,
            });

            string title = "Top new level of the week";
            string body = $"Your level {newLevelOfTheWeek.Name} is top new level of the week. Congratulations!";
            string deeplinkUrl = $"https://play.safariforever.com/{newLevelOfTheWeek.LevelId}";
            await _notificationService.CreateNotification(newLevelOfTheWeek.CreatorUserId, title, body, Models.Notifications.NotificationType.MyLevelIsLevelOfTheWeek, deeplinkUrl, new List<NotificationLink>() { new NotificationLink("View Level", deeplinkUrl) });

            await _db.SaveChangesAsync();

            await PostLevelToDiscord(newLevelOfTheWeek);
        }

        private async Task<DiscordMessage> PostLevelToDiscord(Level newLevelOfTheWeek)
        {
            await Init();

            var drum = DiscordEmoji.FromUnicode(_discordClient, "🥁");
            var microphone2 = DiscordEmoji.FromName(_discordClient, ":microphone2:");

            var tada = DiscordEmoji.FromName(_discordClient, ":tada:");
            var star = DiscordEmoji.FromName(_discordClient, ":star:");

            User creator = await _db.Users.FindAsync(newLevelOfTheWeek.CreatorUserId);

            string message = $"{microphone2} __**TOP NEW LEVEL OF WEEK: {GetIso8601WeekOfYear(DateTime.UtcNow)}!**__ {drum}" +
                $"\n";

            if(creator != null)
                message += $"\n__**{newLevelOfTheWeek.Name}**__ By: {star} **{creator.Nickname}** {star}";

            message += $"\n{GetShareUrl(newLevelOfTheWeek.LevelId)} {tada}";

            return await _discordClient.SendMessageAsync(channel, message);
        }
        
        private string GetShareUrl(string LevelId)
        {
            string Scheme = "https";
            return $"{Scheme}://play.safariforever.com/{LevelId}";
        }

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
