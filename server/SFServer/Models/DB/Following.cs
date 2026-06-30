using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.DB
{
    public class Following
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string FollowingId { get; set; }

        public string UserBeingFollowedId { get; set; }

        [ForeignKey("Follower")]
        public string UserFollowingId { get; set; }
        public User Follower { get; set; }

        public bool Active { get; set; }

        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
    }
}
