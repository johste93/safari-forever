using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Notifications
{
    public class Notification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string NotificationId { get; set; }

        public string UserId { get; set; }
        public bool Read { get; set; }

        public NotificationType NotificationType { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public string DeeplinkUrl { get; set; }
        public List<NotificationLink> Links { get; set; }

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

        public Notification()
        {
        }

        public Notification(string userId, string title, string body, NotificationType type, string deeplinkUrl, List<NotificationLink> links)
        {
            this.UserId = userId;
            this.Title = title;
            this.Body = body;
            this.Links = links == null ? new List<NotificationLink>() : links;
            this.NotificationType = type;
            this.DeeplinkUrl = deeplinkUrl;
        }
    }
}
