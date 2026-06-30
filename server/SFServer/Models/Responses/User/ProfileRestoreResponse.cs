using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.UserResponses
{
    public class ProfileRestoreResponse
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string Nickname { get; set; }
        public int Identifier { get; set; }
        public string Color { get; set; }
        public int Coins { get; set; }

        public ProfileRestoreResponse(string userId, string tokenComputation, string nickname, int identifier, string color, int coins)
        {
            this.UserId = userId;
            this.Token = tokenComputation;
            this.Nickname = nickname;
            this.Identifier = identifier;
            this.Color = color;
            this.Coins = coins;
        }
    }
}
