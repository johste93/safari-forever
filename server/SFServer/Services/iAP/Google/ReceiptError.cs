using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Services.iAP.Google
{
    public class ReceiptError
    {
        public int code { get; set; }
        public string message { get; set; }
        public Dictionary<string, string>[] errors { get; set; }
    }
}