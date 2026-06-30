using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SFServer.Contexts;
using SFServer.Models.DB;
using SFServer.Utility;
using SFServer.Models.Responses.LevelResponses;
using SFServer.Models.Configs;
using SFServer.Services;
using SFServer.Models.DailyChallenge;
using Microsoft.EntityFrameworkCore;
using SFServer.Models.Enums;

namespace SFServer.Discord
{
    public class LevelCommands
    {
        [Command("level"), Description("Tells you the current stats for the specified level")]
        public async Task Stats(CommandContext ctx, [Description("The level sharecode")] string shareCode) // this command takes a member as an argument; you can pass one by username, nickname, id, or mention
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            var db = ctx.Dependencies.GetDependency<PostgreSqlContext>();
            var levelStatService = ctx.Dependencies.GetDependency<LevelStatsService>();

            Level level = await db.Levels.FindAsync(shareCode);

            if (level == null)
            {
                await ctx.RespondAsync($"I cant find that level!");
                return;
            }

            if(!level.VerifiedUpload)
            {
                await ctx.RespondAsync($"I cant find that level!");
                return;
            }

            DailyChallengeService dailyChallengeService = ctx.Dependencies.GetDependency<DailyChallengeService>();
            DailyChallenge dailyChallenge = await dailyChallengeService.GetCurrentChallenge();

            bool thisIsTheDailyChallenge = dailyChallenge != null && level.LevelId == dailyChallenge.LevelId;

            if(thisIsTheDailyChallenge)
            {
                //Hide World Record
                level.Record_Seconds = -1;
                level.Record_Milliseconds = -1;
                level.RecordHolder = string.Empty;
            }

            List<int> jumps = await db.LevelUserStats.Where(x => x.LevelId == level.LevelId && x.Jumps >= 0).Select(x => x.Jumps).ToListAsync();
            int averageNumberOfJumps = -1;
            int totalJumps = -1;
            if (jumps.Count > 0)
            {
                totalJumps = jumps.Sum();
                averageNumberOfJumps = (int)((float)totalJumps / (float)jumps.Count);
            }

            List<int> deaths = await db.LevelUserStats.Where(x => x.LevelId == level.LevelId && x.Deaths >= 0).Select(x => x.Deaths).ToListAsync();
            int averageNumberOfDeaths = -1;
            if (deaths.Count > 0)
                averageNumberOfDeaths = (int)((float)deaths.Sum() / (float)deaths.Count);

            Difficulty difficulty = await levelStatService.CalculateDifficulty(level.LevelId);

            string recordHolderNickname = "?";
            if (!string.IsNullOrWhiteSpace(level.RecordHolder))
            {
                User recordHolderUser = db.Users.Where(x => x.UserId == level.RecordHolder).FirstOrDefault();
                if (recordHolderUser != null)
                    recordHolderNickname = $"{recordHolderUser.Nickname}#{recordHolderUser.Identifier.ToString("0000")}";
            }

            var stats = new LevelStatsResponse(level.Plays, level.Wins, averageNumberOfDeaths, averageNumberOfJumps, level.Deaths, totalJumps, level.Likes, level.Record_Seconds, level.Record_Milliseconds, level.RecordHolder, recordHolderNickname, difficulty);

            if (thisIsTheDailyChallenge)
            {
                stats.Record = "Secret";
                stats.RecordHolderNicknameAndIdentifier = "Secret";
            }

            User user = await db.Users.FindAsync(level.CreatorUserId);

            if (user == null)
            {
                await ctx.RespondAsync($"oh snap! I dont feel so good...");
                return;
            }

            User recordHolder = await db.Users.FindAsync(level.RecordHolder);

            var emojies = ctx.Dependencies.GetDependency<EmojiCollection>().emojieDictionary;

            string about = $"**__{level.Name} - By: {user.Nickname}__**" +
                $"\n{emojies[":bar_chart:"]} Difficulty: {difficulty}" +
                $"\n{emojies[":arrow_forward:"]} Plays: {level.Plays}" +
                $"\n{emojies[":thumbsup:"]} Likes: {level.Likes}" +
                $"\n{emojies[":triangular_flag_on_post:"]} Clear Rate: {stats.GetClearPercentage()}" +
                $"\n{emojies[":trophy:"]} Best Time: {stats.Record}{(thisIsTheDailyChallenge ? "" : (recordHolder != null ? $" by **{recordHolder.Nickname}**" : ""))}" +
                $"\n{emojies[":skull:"]} Deaths: {level.Deaths}";
                //$"\n{emojies[":boot:"]} Jumps: ~ {jumps}";
                //$"\n{emojies[":hot_face:"]} difficulty: {difficulty}"; 

            // and finally, let's respond and greet the user.
            await ctx.RespondAsync($"{about}");
        }

        [Command("challenge-me"), Description("Ask Pepe to Challenge you with a random level!")]
        public async Task Challenge(CommandContext ctx) // this command takes a member as an argument; you can pass one by username, nickname, id, or mention
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            var db = ctx.Dependencies.GetDependency<PostgreSqlContext>();
            var versionConfig = ctx.Dependencies.GetDependency<VersionConfig>();

            if(versionConfig == null)
            {
                await ctx.RespondAsync($"oh snap! I'm unable to get minimum version!");
                return;
            }

            Random rand = new Random();
            var levels = db.Levels.Where(x => x.Blacklisted == false && x.VerifiedUpload && ((x.MajorGameVersion > versionConfig.levelCompabilityVersion.Major) || (x.MajorGameVersion == versionConfig.levelCompabilityVersion.Major && x.MinorGameVersion >= versionConfig.levelCompabilityVersion.Minor)));
            int toSkip = rand.Next(1, levels.Count());
            Level level = levels.Skip(toSkip).Take(1).First();

            if (level == null)
            {
                await ctx.RespondAsync($"oh snap! I dont feel so good...");
                return;
            }

            string shareUrl = GetShareUrl(level.LevelId);

            var emojies = ctx.Dependencies.GetDependency<EmojiCollection>().emojieDictionary;

            string challenge = $"__I challenge you **{ctx.User.Username}**!__\n" +
                               $"{emojies[":checkered_flag:"]} Beat this level: ** {shareUrl} **";

            // and finally, let's respond and greet the user.
            await ctx.RespondAsync($"{challenge}");
        }

        private string GetShareUrl(string LevelId)
        {
            string Scheme = "https";
            return $"{Scheme}://play.safariforever.com/{LevelId}";
        }
    }
}