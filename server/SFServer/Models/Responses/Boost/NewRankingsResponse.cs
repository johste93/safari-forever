using SFServer.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.Boost
{
    public class NewRankingsResponse
    {
        public List<Tuple<int, long>> Rankings { get; set; }
    }
}
