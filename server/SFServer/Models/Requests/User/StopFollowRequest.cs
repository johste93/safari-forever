using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.UserRequests
{
    public class UnfollowRequest
    {
        public string UserToUnfollow { get; set; }
    }
}
