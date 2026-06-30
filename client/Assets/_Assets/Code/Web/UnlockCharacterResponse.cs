using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockCharacterResponse
{
    public bool PurchaseSuccessful { get; set; }
    public int RemainingCoins { get; set; }
    public PurchaseError PurchaseError { get; set; }
}
