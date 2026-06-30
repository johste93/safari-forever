using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SFServer.Contexts;
using SFServer.Utility;
using SFServer.Models.Configs;
using Microsoft.Extensions.Options;
using SFServer.Services;

namespace SFServer.Discord
{
    //https://discordapp.com/api/oauth2/authorize?client_id=606270792577056779&scope=bot&permissions=67497025
    public class PepeBot
    {
        private Task Initialization { get; set; }
        public DiscordClient _discordClient { get; set; }
        private CommandsNextModule Commands { get; set; }
        private VersionConfig _versionConfig { get; set; }
        private DailyChallengeService _dailyChallengeService { get; set; }

        private static EmojiList cheerfulEmojies { get; set; }
        private string appstoreUrl = "https://apps.apple.com/app/apple-store/id1477660498";
        private string playStoreUrl = "https://play.google.com/store/apps/details?id=com.chumpware.safariforever";

        public PepeBot(PostgreSqlContext db, DiscordClient discordClient, VersionConfig versionConfig, DailyChallengeService dailyChallengeService, LevelStatsService levelStatsService)
        {
            _discordClient = discordClient;
            _versionConfig = versionConfig;
            _dailyChallengeService = dailyChallengeService;
            Initialization = InitAsync(db, levelStatsService);
        }

        private async Task InitAsync(PostgreSqlContext db, LevelStatsService levelStatsService)
        {
            cheerfulEmojies = new EmojiList(_discordClient, new List<string>()
            {
                ":100:",
                ":tada:",
                ":fire:",
                ":heart:",
                ":muscle:",
                ":rocket:",
                ":ok_hand:",
                ":thumbsup:",
                ":checkered_flag:"
            });


            _discordClient.MessageCreated += OnMessageCreated;
            _discordClient.GuildMemberAdded += OnGuildMemberAdded;

            var ccfg = new CommandsNextConfiguration
            {
                // enable responding in direct messages
                EnableDms = true,

                // enable mentioning the bot as a command prefix
                EnableMentionPrefix = true,

                Dependencies = BuildDependencyCollection(db, _versionConfig, levelStatsService)
            };

            this.Commands = _discordClient.UseCommandsNext(ccfg);

            this.Commands.RegisterCommands<BasicCommands>();
            this.Commands.RegisterCommands<LevelCommands>();
            this.Commands.RegisterCommands<HighscoreCommands>();
            this.Commands.SetHelpFormatter<SimpleHelpFormatter>();

            await _discordClient.ConnectAsync();
            await Task.Delay(-1);
        }

        private async Task OnGuildMemberAdded(GuildMemberAddEventArgs e)
        {
            if (e.Member.IsBot)
                return;

            var star = DiscordEmoji.FromName(_discordClient, ":star:");

            //var androidChannel = await _discordClient.GetChannelAsync(620609625057787904);
            //var iOSChannel = await _discordClient.GetChannelAsync(620610229427634186);

            string welcome = $"👋 Welcome to the Safari Forever Community, **{e.Member.Username}**!" +
                $"\nI'm {star} **Pepe Bot!** {star}" +
                $"\n" +
                $"\n__**Have you installed the game yet?**__" +
                $"\niPhone/iPad download: {appstoreUrl}" +
                $"\nAndroid download: {playStoreUrl}";

            //var channel = await discordClient.GetChannelAsync(449014097175117856);
            var channel = await _discordClient.CreateDmAsync(e.Member);
            await _discordClient.SendMessageAsync(channel, welcome);
        }

        private async Task OnMessageCreated(MessageCreateEventArgs e)
        {
            if (e.Author.IsBot)
                return;

            if(e.Message.Content.Contains("play.safariforever.com", StringComparison.OrdinalIgnoreCase))
            {
                int randomIndex = new Random().Next(0, cheerfulEmojies.emojieList.Count);
                var emoji = cheerfulEmojies.emojieList[randomIndex];
                await e.Message.CreateReactionAsync(emoji);
            }
        }

        private DependencyCollection BuildDependencyCollection( PostgreSqlContext db, VersionConfig versionConfig, LevelStatsService levelStatsService)
        {
            using (var deps = new DependencyCollectionBuilder())
            {
                deps.AddInstance(_discordClient) // Add the discord client
                    .AddInstance(db)
                    .AddInstance(levelStatsService)
                    .AddInstance(versionConfig)
                    .AddInstance(_dailyChallengeService)
                    .AddInstance(new EmojiCollection(_discordClient, new List<string>
                    {
                        ":star:",
                        ":skull:",
                        ":rocket:",
                        ":trophy:",
                        ":ok_hand:",
                        ":thumbsup:",
                        ":arrow_forward:",
                        ":checkered_flag:",
                        ":triangular_flag_on_post:",
                        ":bar_chart:"
                    }));

                return deps.Build();
            }
        }

    }
}

