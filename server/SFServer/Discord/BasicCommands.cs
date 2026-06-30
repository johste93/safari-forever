using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;


namespace SFServer.Discord
{
    public class BasicCommands
    {
        [Command("hello"), Aliases("hi")]
        public async Task Hi(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"👋 Hello, {ctx.User.Mention}!");
        }

        [Command("introduce-yourself")]
        public async Task Introduce(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var emojies = ctx.Dependencies.GetDependency<EmojiCollection>().emojieDictionary;

            string introduction = $"👋 Hello, Everyone!" +
                $"\nI'm {emojies[":star:"]} **Pepe Bot!** {emojies[":star:"]}" +
                $"\n" +
                $"\nI can help you with fun things!" +
                $"\nMention me \"@Pepe Bot\" and I shall do your bidding!" +
                $"\nto see what I can do; try: \"@Pepe Bot help\"";

            await ctx.RespondAsync(introduction);
        }
    }
}
