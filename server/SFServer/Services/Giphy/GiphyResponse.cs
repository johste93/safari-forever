

namespace SFServer.Services.Giphy
{
    public class GiphyResponse
    {
        public GiphyGifObject Data { get; set; }
        public GiphyMeta Meta { get; set; }

        public override string ToString()
        {
            return Data?.ToString() + "\n" + Meta?.ToString();
        }
    }
}