using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockCharacterWithIAPResponse
{
    public bool PurchaseSuccessful { get; set; }
    public Animal UnlockedAnimal { get; set; }
    public PurchaseError PurchaseError { get; set; }
}
