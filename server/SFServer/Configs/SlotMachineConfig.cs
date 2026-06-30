using System;
namespace SFServer.Configs
{
    public class SlotMachineConfig
    {
        public double LossChance { get; set; }
        public int BaseCost { get; set; }
        public int CostIncrement { get; set; }
        public int MinimumBet { get; set; }
    }
}
