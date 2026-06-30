using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Discord
{
    public class PersonalDiscordInvite
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string PersonalDiscordInviteId { get; set; } //Same as discord Invite id
        public string UserId { get; set; }
        public bool Joined { get; set; }

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    }
}
