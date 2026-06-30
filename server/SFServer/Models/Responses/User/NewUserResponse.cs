using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.DB;
using SFServer.Models.DTO;

namespace SFServer.Models.Responses.UserResponses
{
    public class NewUserResponse
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string RestoreToken { get; set; }
        public SaveDataDTO saveData { get; set; }

        #region
        //These are here for compability with lower than v0.91. Can be removed once everyone has 0.91 or newer.
        public string Nickname { get; set; }
        public int Identifier { get; set; }
        public string Color { get; set; }
        public int Coins { get; set; }
        #endregion

        public NewUserResponse(User newUser, string tokenComputation)
        {
            this.UserId = newUser.UserId;
            this.Token = tokenComputation;
            this.RestoreToken = newUser.RestoreToken;
            this.saveData = new SaveDataDTO(newUser);

            this.Nickname = newUser.Nickname;
            this.Identifier = newUser.Identifier;
            this.Color = newUser.Color;
            this.Coins = newUser.Coins;
        }
    }
}
