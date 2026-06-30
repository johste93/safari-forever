using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Services.Giphy
{
    public class GiphyImageObject
    {
        public int height { get; set; }
        public int width { get; set; }

        public string url { get; set; }
        public int size { get; set; }
        
        public int frames { get; set; }
        public string hash { get; set; }

        public string mp4 { get; set; }
        public string mp4_size { get; set; }

        public string webp { get; set; }
        public int webp_size { get; set; }

    }
}
