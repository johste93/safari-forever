using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Notifications
{
    public class NotificationLink
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string NotificationLinkId { get; set; }

        [ForeignKey("Notification")]
        public string NotificationId { get; set; }

        public string ButtonText { get; set; }
        public string Url { get; set; }

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

        public NotificationLink()
        {
        }

        public NotificationLink(string buttonText, string url)
        {
            ButtonText = buttonText;
            Url = url;
        }
    }
}
