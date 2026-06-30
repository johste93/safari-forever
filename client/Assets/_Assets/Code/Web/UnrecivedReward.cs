using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnrecivedReward
{
    public int ChangeInBalance { get; set; }
    public int BalanceBefore { get; set; }
    public int BalanceAfter { get; set; }
    public TransactionType TransactionType { get; set; }
    public string LevelName { get; set; }
    public DateTimeOffset RecivedOn { get; set; }
    public string Description { get; set; }
    
    public Hat Hat { get; set; }
}
