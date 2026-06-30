using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.Mod
{
    public class BanResponse
    {
        public string UserId { get; set; }
        public string NicknamePlusIdentifier { get; set; }
        public bool Banned { get; set; } 
    }
}
