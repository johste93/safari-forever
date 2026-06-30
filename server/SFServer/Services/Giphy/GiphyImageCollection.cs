using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Services.Giphy
{
    public class GiphyImageCollection
    {
        public GiphyImageObject downsized_large { get; set; }
        public GiphyImageObject fixed_height_small_still { get; set; }
        public GiphyImageObject orginal { get; set; }
        public GiphyImageObject fixed_height_downsampled { get; set; }
        public GiphyImageObject downsized_still { get; set; }
        public GiphyImageObject fixed_height_still { get; set; }
        public GiphyImageObject downsized_medium { get; set; }
        public GiphyImageObject downsized { get; set; }
        public GiphyImageObject preview_webp { get; set; }

        public GiphyImageObject original_mp4 { get; set; }
        public GiphyImageObject fixed_height_small { get; set; }
        public GiphyImageObject fixed_height { get; set; }
        public GiphyImageObject downsized_small { get; set; }
        public GiphyImageObject preview { get; set; }
        public GiphyImageObject fixed_width_downsampled { get; set; }

        public GiphyImageObject fixed_width_small_still { get; set; }
        public GiphyImageObject fixed_width_small { get; set; }
        public GiphyImageObject original_still { get; set; }
        public GiphyImageObject fixed_width_still { get; set; }
        public GiphyImageObject looping { get; set; }
        public GiphyImageObject fixed_width { get; set; }
        public GiphyImageObject preview_gif { get; set; }
    }
}
