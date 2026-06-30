using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestorePurchasesButton : MonoBehaviour
{
    public Shop shop;

    public void RestorePurchases()
    {
        shop?.Exit();

        //IAPManager.instance?.RestorePurchases();
    }
}
