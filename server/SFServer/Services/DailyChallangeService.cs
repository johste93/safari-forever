using DSharpPlus;
using SFServer.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.DailyChallenge;
using SFServer.Models;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using SFServer.Utility;
using SFServer.Models.Notifications;
using SFServer.Models.DB;
using SFServer.Models.Transaction;
using SFServer.Models.Configs;
using Microsoft.Extensions.Options;

namespace SFServer.Services
{
    public class DailyChallengeService
    {
        private PostgreSqlContext _db;
        private NotificationService _notificationService;
        private DiscordClient _discordClient;
        private DiscordChannel channel;
        
        public const ulong discordChannelId = 626722495214518283;

        private DailyChallenge currentChallenge;
        private VersionConfig _versionConfig;
        private TransactionService _transactionService;

        private RewardConfig _rewardConfig;

        public DailyChallengeService(PostgreSqlContext db, DiscordClient discordClient, TransactionService transactionService, NotificationService notificationService, IOptions<VersionConfig> versionConfig, IOptions<RewardConfig> rewardConfig)
        {
            _db = db;
            _discordClient = discordClient;
            _notificationService = notificationService;
            _transactionService = transactionService;

            _rewardConfig = rewardConfig.Value;
            _versionConfig = versionConfig.Value;
        }

        private async Task Init()
        {
            if (channel == null)
                channel = await _discordClient.GetChannelAsync(discordChannelId);

            currentChallenge = await GetCurrentChallenge();
        }

        public async Task GenerateNewDailyChallenge()
        {
            await Init();
            //await _discordClient.SendMessageAsync(channel, $"Im on Vacation! I'll be back in a few days! ;)");

            if (await ChallengeAlreadyGenerated())
            {
                await EndDailyChallenge();
            }

            Console.WriteLine("Generating New Daily Challange");

            Level level = GetRandomDailyChallengeLevel();

            if (level == null)
            {
                Console.WriteLine("Error: No daily challange level was found!");
                return;
            }

            level.DailyChallengeOn = DateTimeOffset.Now;
            _db.UpdateLevel(level);

            var challenge = new DailyChallenge()
            {
                LevelId = level.LevelId
            };

            DiscordMessage msg = await PostChallangeToDiscord(challenge);

            challenge.DiscordMessageId = msg.Id;

            _db.DailyChallenges.Add(challenge);

            currentChallenge = challenge;

            await _db.SaveChangesAsync();
        }

        public async Task EndDailyChallenge()
        {
            await Init();

            var currentChallenge = await GetCurrentChallenge();
            if (currentChallenge == null)
                return;

            Console.WriteLine("Ending Daily Challange...");

            List<DailyChallengeParticipant> participants = await GetParticipants(currentChallenge);

            DailyChallengeParticipant winningParticipant = await GetTodaysChampion(currentChallenge);

            if (winningParticipant != null)
            {
                User winner = await _db.Users.Where(x => x.UserId == winningParticipant.UserId).FirstOrDefaultAsync();
                if (winner != null)
                {
                    winner.DailyChallengesWon++;
                    _db.UpdateUser(winner);
                    await _db.SaveChangesAsync();
                }
            }

            await PostResultToDiscord(currentChallenge, participants);

            await SendNotifications(participants);

            await RewardParticipants(currentChallenge, participants);
        }

        /*
        public async Task PostResults(string challengeId)
        {
            await Init();

            var challenge = await _db.DailyChallenges.FindAsync(challengeId);

            if(challenge == null)
                return;

            List<DailyChallengeParticipant> participants = await GetParticipants(challenge);

            await PostResultToDiscord(challenge, participants);

            await SendNotifications(participants);

            await RewardParticipants(participants);

            await RewardParticipants(participants);
        }
        */

        private async Task<DiscordMessage> PostChallangeToDiscord(DailyChallenge challenge)
        {
            await Init();

            var microphone2 =       DiscordEmoji.FromName(_discordClient, ":microphone2:");
            var star =              DiscordEmoji.FromName(_discordClient, ":star:");
            var checkered_flag =    DiscordEmoji.FromName(_discordClient, ":checkered_flag:");

            string message =    $"I'm your host {star} **{_discordClient.CurrentUser.Mention}!** {microphone2}" +
                                $"\nand here is your __**DAILY CHALLENGE!**__ {checkered_flag}" +
                                $"\n" +
                                $"\n{GetShareUrl(challenge.LevelId)}";

            return await _discordClient.SendMessageAsync(channel, message);
        }

        public async Task<DailyChallenge> GetChallengeById(string id)
        {
            return await _db.DailyChallenges.FindAsync(id);
        }

        public async Task<DailyChallengeParticipant> GetTodaysChampion(DailyChallenge challenge)
        {
            return await _db.DailyChallengeParticipants
                .Where(x => x.DailyChallengeId == challenge.DailyChallengeId)
                .OrderBy(x => x.Seconds)
                .ThenBy(x => x.Milliseconds)
                .ThenBy(x => x.UpdatedOn)
                .FirstOrDefaultAsync();
        }

        public async Task<List<DailyChallengeParticipant>> GetParticipants(DailyChallenge challenge)
        {
            return await _db.DailyChallengeParticipants
                .Where(x => x.DailyChallengeId == challenge.DailyChallengeId)
                .OrderBy(x => x.Seconds)
                .ThenBy(x => x.Milliseconds)
                .ThenBy(x => x.UpdatedOn)
                .ToListAsync<DailyChallengeParticipant>();
        }

        private async Task PostResultToDiscord(DailyChallenge challenge, List<DailyChallengeParticipant> participants)
        {
            DiscordMessage previousAnouncement = await channel.GetMessageAsync(challenge.DiscordMessageId);

            if (previousAnouncement == null)
            {
                Console.WriteLine("Previous Announcement not found!");
                return;
            }

            var drum =              DiscordEmoji.FromUnicode(_discordClient, "🥁");
            var microphone2 = DiscordEmoji.FromName(_discordClient, ":microphone2:");
               
            Discord​Embed​Builder builder = new Discord​Embed​Builder()
                .WithAuthor(_discordClient.CurrentUser.Username, null, _discordClient.CurrentUser.AvatarUrl)
                .WithTitle($"{microphone2} __**The Results!**__ {drum}")
                .WithUrl(new Uri($"https://discord.safariforever.com/daily/{challenge.DailyChallengeId}"))
                .WithThumbnailUrl($"https://play.safariforever.com/level/{challenge.LevelId}/img");


            if (participants.Count == 0)
            {
                var snail =         DiscordEmoji.FromName(_discordClient, ":snail:");
                builder.AddField("Winner:", $"There was no winner yesterday! {snail}");

                await previousAnouncement.ModifyAsync(default, builder);
                return;
            }

            //Send Notifications.
            var star = DiscordEmoji.FromName(_discordClient, ":star:");
            var first_place = DiscordEmoji.FromUnicode(_discordClient, "🥇");
            var second_place = DiscordEmoji.FromUnicode(_discordClient, "🥈");
            var third_place = DiscordEmoji.FromUnicode(_discordClient, "🥉");


            StringBuilder rankings = new StringBuilder();
            StringBuilder winners = new StringBuilder();

            Highscore firstPlaceScore = new Highscore(participants[0].Seconds, participants[0].Milliseconds);
            winners.Append($"{star} **{participants[0].Nickname}** - {firstPlaceScore.ToString()}");

            if(participants.Count >= 2)
            {
                Highscore secondPlaceScore = new Highscore(participants[1].Seconds, participants[1].Milliseconds);
                winners.Append($"\n{second_place} {participants[1].Nickname} - {secondPlaceScore.ToString()}");
            }

            if (participants.Count >= 3)
            {
                Highscore thirdPlaceScore = new Highscore(participants[2].Seconds, participants[2].Milliseconds);
                winners.Append($"\n{third_place} {participants[2].Nickname} - {thirdPlaceScore.ToString()}");
            }

            int participantsInGroup = Math.Min(participants.Count, 10);

            int rank = 1;
            for (int participantIndex = 0; participantIndex < participantsInGroup; participantIndex++)
            {
                Highscore score = new Highscore(participants[participantIndex].Seconds, participants[participantIndex].Milliseconds);
                rankings.Append($"{rank}. {participants[participantIndex].Nickname} - {score.ToString()}\n");
                rank++;
            }

            builder.AddField("**Winner:**", winners.ToString());
            builder.AddField($"**Top {Math.Min(10, participants.Count)}:**", rankings.ToString());

            await previousAnouncement.ModifyAsync(default, builder);
        }

        public async Task SendNotifications(List<DailyChallengeParticipant> participants)
        {
            if (participants.Count == 0)
                return;

            //First place
            string winnerTitle = $"Daily Challenge Result: {DateTimeOffset.Now.Date.ToShortDateString()}";
            string winnerBody = $"YOU WON!\nCongratulations!";
            await _notificationService.CreateNotification(participants[0].UserId, winnerTitle, winnerBody, NotificationType.DailyChallengeResults);
             
            //Second to second last place
            for (int i = 1; i < participants.Count - 1; i++)
            {
                string title = $"Daily Challenge Result: {DateTimeOffset.Now.Date.ToShortDateString()}";
                string body = $"You ranked number {(i + 1)}.\nRight before {participants[i + 1].Nickname} And just behind {participants[i - 1].Nickname}";
                await _notificationService.CreateNotification(participants[i].UserId, title, body, NotificationType.DailyChallengeResults);
            }

            //Last place
            if (participants.Count >= 2)
            {
                string title = $"Daily Challenge Result: {DateTimeOffset.Now.Date.ToShortDateString()}";
                string body = $"You ranked number {(participants.Count)}.\nJust behind {participants[participants.Count - 2].Nickname}";
                await _notificationService.CreateNotification(participants[participants.Count - 1].UserId, title, body, NotificationType.DailyChallengeResults);
            }

            await _db.SaveChangesAsync();
        }

        public async Task RewardParticipants(DailyChallenge challenge, List<DailyChallengeParticipant> participants)
        {
            if (participants.Count == 0)
                return;

            Level level = await _db.Levels.Where(x => x.LevelId == challenge.LevelId).FirstOrDefaultAsync();

            List<string> alreadyRewardedUsers = new List<string>();

            string levelName = level != null ? level.Name : "Unknown Level";

            if (participants.Count > 0)
            {
                User firstPlace = await _db.Users.FirstOrDefaultAsync(x => x.UserId == participants[0].UserId && !alreadyRewardedUsers.Contains(x.UserId));
                if(firstPlace != null)
                {
                    alreadyRewardedUsers.Add(firstPlace.UserId);
                    await _transactionService.AddCoins(firstPlace, _rewardConfig.DailyChallengeFirstPlace, "Daily challenge First Place", TransactionType.DailyChallengeFirstPlace, levelName);
                }
                    
            }

            if (participants.Count > 1)
            {
                User secondPlace = await _db.Users.FirstOrDefaultAsync(x => x.UserId == participants[1].UserId && !alreadyRewardedUsers.Contains(x.UserId));
                if (secondPlace != null)
                {
                    alreadyRewardedUsers.Add(secondPlace.UserId);
                    await _transactionService.AddCoins(secondPlace, _rewardConfig.DailyChallengeSecondPlace, "Daily challenge Second Place", TransactionType.DailyChallengeSecondPlace, levelName);
                }
                    
            }

            if (participants.Count > 2)
            {
                User thirdPlace = await _db.Users.FirstOrDefaultAsync(x => x.UserId == participants[2].UserId && !alreadyRewardedUsers.Contains(x.UserId));
                if (thirdPlace != null)
                {
                    alreadyRewardedUsers.Add(thirdPlace.UserId);
                    await _transactionService.AddCoins(thirdPlace, _rewardConfig.DailyChallengeThirdPlace, "Daily challenge Third Place", TransactionType.DailyChallengeThirdPlace, levelName);
                }
            }

            /*
            List<string> participantsIds = participants.Select(x => x.UserId).ToList();
            List<User> users = await _db.Users.Where(x => participantsIds.Contains(x.UserId) && !alreadyRewardedUsers.Contains(x.UserId)).ToListAsync();
            for (int i = 2; i < users.Count; i++)
            {
                await _transactionService.AddCoins(users[i], _rewardConfig.DailyChallengeParticipation, "Daily challenge Participation", RewardType.DailyChallengeParticipation, levelName);
            }
            */
        }

        public async Task SendNewDailyChallengeNotification()
        {
            DailyChallenge challenge = await GetCurrentChallenge();

            if (challenge == null)
                return;

            Level level = await _db.Levels.Where(x => x.LevelId == challenge.LevelId).FirstOrDefaultAsync();

            if (level == null)
                return;

            User creator = await _db.Users.FindAsync(level.CreatorUserId);

            if (creator == null)
                return;

            string title = "Todays daily challenge available!";
            string body = $"Todays daily challenge is {level.Name} by {creator.Nickname}";
            string deeplinkUrl = $"https://play.safariforever.com/{challenge.LevelId}";
            await _notificationService.SendPushNotification(level.CreatorUserId, title, body, NotificationType.NewDailyChallenge, deeplinkUrl);
        }

        public async Task<DailyChallenge> GetCurrentChallenge()
        {
            if(currentChallenge == null)
            {
                currentChallenge = await _db.DailyChallenges.OrderByDescending(p => p.CreatedOn).FirstOrDefaultAsync<DailyChallenge>();
            }

            return currentChallenge;
        }

        public async Task<bool> ChallengeAlreadyGenerated()
        {
            DateTime yesterday = DateTime.UtcNow.AddDays(-1);
            return await _db.DailyChallenges.AnyAsync(x => x.CreatedOn.Date == yesterday.Date);
        }

        private Level GetRandomDailyChallengeLevel()
        {
            Random rand = new Random();
            DateTimeOffset date = DateTimeOffset.Now.AddMonths(-1);
            //There has to be atleast 15 non blacklisted levels.

            var levels = _db.Levels.Where(x => x.Record_Seconds > 10 && x.Deaths > 0 && x.Blacklisted == false && x.VerifiedUpload && x.DailyChallengeOn < date && ((x.MajorGameVersion > _versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == _versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= _versionConfig.levelCompabilityVersion.Minor)));
            int toSkip = rand.Next(1, levels.Count());
            return levels.Skip(toSkip).Take(1).First();
        }

        private string GetShareUrl(string LevelId)
        {
            string Scheme = "https";
            return $"{Scheme}://play.safariforever.com/{LevelId}";
        }

        private string FormatTime(double input)
        {
            int seconds = (int)Math.Floor(input);
            int miliseconds = (int)Math.Floor((input - ((int)Math.Floor(input))) * 100);

            return $"{seconds.ToString("D2")}:{miliseconds.ToString("D2")}";
        }
    }
}
