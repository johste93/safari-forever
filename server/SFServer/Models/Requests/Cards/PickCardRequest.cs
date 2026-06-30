using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.CardsRequests
{
    public class PickCardRequest
    {
        public int CardPickedIndex { get; set; }
    }
}
