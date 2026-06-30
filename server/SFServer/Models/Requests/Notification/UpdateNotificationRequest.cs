using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.NotificationRequests
{
    public class UpdateNotificationRequest
    {
        [Required]
        public List<string> notificationIds { get; set; }
    }
}
