using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SafariForever.Notifications
{
    public class NotificationFeedResponse
    {
        public int NotifcationsPrPage { get; set; }
        public List<Notification> Notifications { get; set; }
    }
}
