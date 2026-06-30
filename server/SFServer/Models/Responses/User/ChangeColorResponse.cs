using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.UserResponses
{
    public class ChangeColorResponse
    {
        public ChangeColorResponse(string color)
        {
            this.Color = color;
        }

        public string Color { get; set; }
    }
}
