using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.Endless
{
    public class FetchRankResponse
    {
        public int Score { get; set; } = -1;
        public int Rank { get; set; } = -1;
    }
}
