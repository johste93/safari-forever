using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.UserResponses
{
    public class ChangeNicknameResponse
    {
        public string Nickname { get; set; }
        public int Identifier { get; set; }

        public ChangeNicknameResponse(string nickname, int identifier)
        {
            this.Nickname = nickname;
            this.Identifier = identifier;
        }
    }
}
