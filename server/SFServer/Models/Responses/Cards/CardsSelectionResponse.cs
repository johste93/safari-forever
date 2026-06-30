using SFServer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.CardsResponses
{
    public class CardsSelectionResponse
    {
        public CardError Error { get; set; }
        public bool[] Cards { get; set; }
        public int CardPrice { get; set; }
    }
}
