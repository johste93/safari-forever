using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.Mod
{
    public class BlacklistResponse
    {
        public string LevelId { get; set; }
        public bool Blacklisted { get; set; }
    }
}
