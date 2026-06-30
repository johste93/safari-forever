

namespace SFServer.Services.Giphy
{
    public class GiphyMeta
    {
        public string Msg { get; set; }
        public int Status { get; set; }
        public string Response_id { get; set; }

        public override string ToString()
        {
            return $"{Status}: {Msg}\n{Response_id}";
        }
    }
}