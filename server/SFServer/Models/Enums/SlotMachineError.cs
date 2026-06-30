using System;
namespace SFServer.Models.Enums
{
    public enum SlotMachineError
    {
        None,
        CantAfford,
        NoMoreRewards,
        LessThanMinimumBet,
        MoreThanMaximumBet
    }
}
