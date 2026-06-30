using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Discord
{
    public class EmojiList
    {
        public List<DiscordEmoji> emojieList { get; set; }

        public EmojiList(DiscordClient client, List<string> emojies)
        {
            emojieList = new List<DiscordEmoji>();

            foreach (string emoji in emojies)
            {
                emojieList.Add(DiscordEmoji.FromName(client, emoji));
            }
        }

    }
}
