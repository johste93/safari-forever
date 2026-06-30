using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.DTO
{
    public class ProfileInfoDTO
    {
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public int Identifier { get; set; }
        public string Color { get; set; }
        public DateTimeOffset LastActive { get; set; }
    }
}
