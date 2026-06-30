using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.DB
{
    public class Client
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ClientId { get; set; }

        public string UserId { get; set; }
        public string FCMToken { get; set; }

        public bool Banned { get; set; }

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

        public Client(string clientId, string userId)
        {
            this.ClientId = clientId;
            this.UserId = userId;
        }
    }
}
