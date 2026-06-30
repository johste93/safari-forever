using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Discord
{
    public class EmojiCollection
    {
        public Dictionary<string, DiscordEmoji> emojieDictionary { get; set; }

        public EmojiCollection(DiscordClient client, List<string> emojies)
        {
            emojieDictionary = new Dictionary<string, DiscordEmoji>();

            foreach(string emoji in emojies)
            {
                emojieDictionary.Add(emoji, DiscordEmoji.FromName(client, emoji));
            }
        }

    }
}