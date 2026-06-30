using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.LevelRequests
{
    public class LevelsByUserRequest
    {
        public string UserId { get; set; }
        public int FromIndex { get; set; }
    }
}
