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
using System.Text;

namespace SFServer.Discord
{
    public class HighscoreCommands
    {
        [Command("highscores"), Description("Tells you the current highscores for the specied level")]
        public async Task GetHighscores(CommandContext ctx, [Description("The level sharecode")] string shareCode)
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            var db = ctx.Dependencies.GetDependency<PostgreSqlContext>();
            var dailyChallengeService = ctx.Dependencies.GetDependency<DailyChallengeService>();
            var emojies = ctx.Dependencies.GetDependency<EmojiCollection>().emojieDictionary;

            Level level = await db.Levels.FindAsync(shareCode);

            if (level == null)
            {
                await ctx.RespondAsync($"I cant find that level!");
                return;
            }

            if (!level.VerifiedUpload)
            {
                await ctx.RespondAsync($"I cant find that level!");
                return;
            }

            var stats = await db.LevelUserStats
                .Join(db.Users,
                  stats => stats.UserId,
                  user => user.UserId,
                  (stats, user) => new
                  {
                      LevelUserStats = stats,
                      User = user
                  })
                .Where(x => x.LevelUserStats.LevelId == shareCode && x.LevelUserStats.Seconds >= 0 && x.LevelUserStats.Milliseconds >= 0 && x.User.Banned == false)
                .OrderBy(x => x.LevelUserStats.Seconds)
                .ThenBy(x => x.LevelUserStats.Milliseconds)
                .ThenBy(x => x.LevelUserStats.HighscoreUpdatedOn)
                .Take(10)
                .ToListAsync();

            string content = $"Looks like no one has set an highscore yet! Will you be the first? {emojies[":rocket:"]}";

            DailyChallenge dailyChallenge = await dailyChallengeService.GetCurrentChallenge();
            bool thisIsTheDailyChallenge = dailyChallenge != null && level.LevelId == dailyChallenge.LevelId;

            if(thisIsTheDailyChallenge)
            {
                content = $"Hey! Thats the daily challenge! I won't tell you.";
                await ctx.RespondAsync(content);
                return;
            }

            if (stats.Count > 0)
            {
                User creator = db.Users.Where(x => x.UserId == level.CreatorUserId).FirstOrDefault();

                StringBuilder builder = new StringBuilder();

                if (creator != null)
                    builder.Append($"**__Highscores for {level.Name} - By: {creator.Nickname}__**\n");
                else
                    builder.Append($"**__Highscores for {level.Name}__**");

                builder.Append("```apache\n");

                for (int i = 0; i < stats.Count; i++)
                {
                    string spaces = "";
                    for(int j = 0; j < 18 - stats[i].User.Nickname.Length; j++)
                        spaces += " ";

                    string placement = $"{(i + 1).ToString().PadLeft(2, ' ')}. ";
                    if (i == 0)
                        builder.Append($"{placement} {stats[i].User.Nickname}#{stats[i].User.Identifier.ToString("0000")}{spaces}{stats[i].LevelUserStats.Seconds.ToString().PadLeft(2, ' ')}.{stats[i].LevelUserStats.Milliseconds.ToString("00")} {emojies[":trophy:"]}\n");
                    else
                        builder.Append($"{placement} {stats[i].User.Nickname}#{stats[i].User.Identifier.ToString("0000")}{spaces}{stats[i].LevelUserStats.Seconds.ToString().PadLeft(2, ' ')}.{stats[i].LevelUserStats.Milliseconds.ToString("00")}\n");
                }

                builder.Append("```");

                content = builder.ToString();
            }

            await ctx.RespondAsync(content);
        }
    }
}