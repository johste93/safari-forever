using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Security
{
    public class Token
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TokenId { get; set; }

        public string UserId { get; set; }
        public string ClientId { get; set; }
        public string TokenString { get; set; }

        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

        public Token()
        {
        }

        public Token(string UserId, string ClientId, string tokenString)
        {
            this.UserId = UserId;
            this.ClientId = ClientId;
            this.TokenString = tokenString;
        }
    }
}
