using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Services.Giphy
{
    public class GiphyUserObject
    {
        public string avatar_url { get; set; }
        public string banner_image { get; set; }
        public string profile_url { get; set; }
        public string username { get; set; }
        public string display_name { get; set; }
        public string is_verified { get; set; }
    }
}
