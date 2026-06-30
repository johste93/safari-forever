using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Notifications
{
    public class NotificationFeedResponse
    {
        public int NotifcationsPrPage { get; set; }
        public List<Notification> Notifications { get; set; }
    }
}
