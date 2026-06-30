using SFServer.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.SearchResponses
{
    public class PlayerSearchResponse
    {
        public List<ProfileInfoDTO> profiles { get; set; }

        public PlayerSearchResponse(ProfileInfoDTO profile)
        {
            this.profiles = new List<ProfileInfoDTO>() { profile };
        }
    }
}
