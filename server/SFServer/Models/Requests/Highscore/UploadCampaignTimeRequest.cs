using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.Highscore
{
    public class UploadCampaignTimeRequest
    {
        public int? Seconds { get; set; }
        public int? Milliseconds { get; set; }
    }
}
