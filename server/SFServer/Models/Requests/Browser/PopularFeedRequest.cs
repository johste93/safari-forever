using System;
namespace SFServer.Models.Requests.Browser
{
    public class TrendingFeedRequest
    {
        public int FromIndex { get; set; }
        public int Amount { get; set; }
    }
}
