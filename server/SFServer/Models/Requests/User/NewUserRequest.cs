using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.UserRequests
{
    public class NewUserRequest
    {
        /*
            Usernames can only consist of Ascii
            Usernames can consist of lowercase and capitals
            Usernames can consist of alphanumeric characters
            Usernames can consist of underscore and hyphens and spaces
            Cannot be two underscores, two hypens or two spaces in a row
            Cannot have a underscore, hypen or space at the start or end
        */

        [Required]
        [RegularExpression("^[A-Za-z0-9]+(?:[ _-][A-Za-z0-9]+)*$", ErrorMessage = "Please use only printable English characters")]
        public string Nickname { get; set; }
    }
}
