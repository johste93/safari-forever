using SFServer.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.DTO
{
    public class UserViewDTO
    {
        public string Nickname { get; set; }
        public string NicknameAndIdentfier { get; set; }
        public string PlayButtonUrl { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public int FollowerCount { get; set; }

        public string[] Levels = new string[4];
        public string[] DisplayValues = new string[4]
        {
            "none", "none", "none", "none"
        };

        public UserViewDTO(User user, List<string> levels, string playButtonUrl)
        {
            this.Nickname = user.Nickname;
            this.NicknameAndIdentfier = $"{user.Nickname}#{user.Identifier.ToString().PadLeft(4, '0')}";
            this.PlayButtonUrl = playButtonUrl;
            this.Color = user.Color;
            this.Description = $"Check out {user.Nickname}'s profile!\nFollowers: {user.FollowerCount}";
            this.FollowerCount = user.FollowerCount;

            for(int i = 0; i < levels.Count; i++)
            {
                Levels[i] = levels[i];
                DisplayValues[i] = "inline-block";
            }
        }
    }
}
