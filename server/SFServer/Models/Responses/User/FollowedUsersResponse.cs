using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFServer.Models.DTO;

namespace SFServer.Models.Responses.UserResponses
{
    public class FollowedUsersResponse
    {
        public int UsersPrPage { get; set; }
        public List<FollowedUserDTO> followedUsers { get; set; }

        public FollowedUsersResponse()
        {
            followedUsers = new List<FollowedUserDTO>();
        }
    }
}
