using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.NotificationRequests
{
    public class MarkAsReadRequest
    {
        public List<string> ReadNotificationsIds { get; set; }
    }
}
