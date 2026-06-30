using System;
using System.ComponentModel.DataAnnotations;

namespace SFServer.Models.Requests.SlotMachineRequests
{
    public class SpinnRequest
    {
        [Required]
        public int CoinsBet { get; set; }
    }
}
