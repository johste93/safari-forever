using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSpotlight : MonoBehaviour
{
    public SpriteRenderer spotlight;
    
	public void Initalize(ShopCanvas shopCanvas)
	{
		float alpha = shopCanvas.thisGroup.alpha;
        spotlight.color = spotlight.color.SetAlpha(alpha);
        ShopCanvas.UpdateSpotlightAlpha += UpdateSpotlightAlpha;
	}

    private void UpdateSpotlightAlpha(float alpha)
    {
        spotlight.color = spotlight.color.SetAlpha(alpha);
    }

    private void Unsubscribe()
    {
        ShopCanvas.UpdateSpotlightAlpha -= UpdateSpotlightAlpha;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}
