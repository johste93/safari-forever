using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.UserRequests
{
    public class RestoreUserRequest
    {
        public string RestoreToken { get; set; }
        public string ClientId { get; set; }
    }
}
