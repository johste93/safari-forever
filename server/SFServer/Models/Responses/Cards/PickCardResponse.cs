using Microsoft.AspNetCore.Mvc;
using SFServer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Responses.CardsResponses
{
    public class PickCardResponse
    {
        public CardError Error { get; set; }
        public bool DidWin { get; set; }
        public Hat Reward { get; set; }
    }
}
