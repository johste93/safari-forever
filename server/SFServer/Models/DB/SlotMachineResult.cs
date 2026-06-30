using System;
using SFServer.Models.Enums;

namespace SFServer.Models.DB
{
    public class SlotMachineResult
    {
        public SlotMachineError Error { get; set; }
        public bool DidWin { get; set; }
        public Hat Reward { get; set; }
    }
}
