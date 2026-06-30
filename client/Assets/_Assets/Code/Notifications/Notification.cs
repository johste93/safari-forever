using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SafariForever.Notifications
{
    public class Notification 
    {   
        public string NotificationId { get; set; }
        public string UserId { get; set; }
        public bool Read { get; set; }

        public NotificationType NotificationType { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public NotificationLink[] Links { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}