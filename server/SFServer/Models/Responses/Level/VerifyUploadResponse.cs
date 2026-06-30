using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.LevelResponses
{
    public class VerifyUploadResponse
    {
        public string ShareUrl { get; set; }

        public VerifyUploadResponse(string shareUrl)
        {
            this.ShareUrl = shareUrl;
        }
    }
}