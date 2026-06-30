using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SFServer.Security;
using System.ComponentModel.DataAnnotations.Schema;
using SFServer.Models.Enums;


namespace SFServer.Models.DB
{
    public class PrivacyPolicyAgreement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string PrivacyPolicyAgreementId { get; set; }

        public string UserId { get; set; }
        public bool Agreed { get; set; }

        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    }
}
