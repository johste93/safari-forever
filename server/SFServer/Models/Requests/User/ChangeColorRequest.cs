using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SFServer.Models.Requests.UserRequests
{
    public class ChangeColorRequest
    {
        [Required]
        [RegularExpression("^#([A-Fa-f0-9]{6})$", ErrorMessage = "Color has to be in this format #FFFFFF")]
        public string Color { get; set; }
    }
}
