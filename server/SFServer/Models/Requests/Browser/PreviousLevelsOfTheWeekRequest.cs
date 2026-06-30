using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.Browser
{
    public class PreviousLevelsOfTheWeekRequest
    {
        public int FromIndex { get; set; }
        public int Amount { get; set; }
    }
}
