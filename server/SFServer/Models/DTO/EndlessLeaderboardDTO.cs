using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.DTO
{
    public class EndlessLeaderboardDTO
    {
        public string Nickname { get; set; }
        public int Identifier { get; set; }
        public int Score { get; set; }
        public int Rank { get; set; }
    }
}
