using System;
using System.Collections.Generic;

namespace SFServer.Services.Giphy
{
    public class GiphyGifObject
    {
        public string type { get; set; }
        public string id { get; set; }
        public string url { get; set; }
        public string slug { get; set; }
        public string bitly_gif_url { get; set; }
        public string bitly_url { get; set; }
        public string embed_url { get; set; }
        public string username { get; set; }
        public string source { get; set; }
        public string title { get; set; }
        public string rating { get; set; }
        public string content_url { get; set; }
        public string source_tld { get; set; }
        public string source_post_url { get; set; }
        public string is_sticker { get; set; }
        public string import_datetime { get; set; }
        public string trending_datetime { get; set; }
        public Dictionary<string, GiphyImageObject> images { get; set; }
        public GiphyUserObject user { get; set; }

        public override string ToString()
        {
            return $"id: {id}";
        }
    }
}