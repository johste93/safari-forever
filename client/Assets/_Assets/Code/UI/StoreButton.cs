using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreButton : MonoBehaviour
{
    public MenuCanvas menuCanvas;
    public ShopCanvas shopCanvas;
    public Shop shop;

    public void OnClick()
    {
        shop.Initalize((success)=>
        {
            if(!success)
                return;

            menuCanvas.Hide(false);
            shopCanvas.Show(false);
        });
    }
}
