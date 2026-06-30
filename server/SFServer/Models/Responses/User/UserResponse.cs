using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.DB;

namespace SFServer.Models.Responses.UserResponses
{
    public class UserResponse
    {
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public int Identifier { get; set; }
        public string Color { get; set; }

        public UserResponse(User user)
        {
            UserId = user.UserId;
            Nickname = user.Nickname;
            Identifier = user.Identifier;
            Color = user.Color;
        }
    }
}
