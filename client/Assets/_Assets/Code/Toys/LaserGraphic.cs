using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGraphic : MonoBehaviour
{
    public LaserCannon laserCannon;
    public SpriteRenderer[] spriteRenders;
    public Transform graphicParent;

    public void SetColor(Color color, float alpha)
    {
        foreach(SpriteRenderer sR in spriteRenders)
            sR.color = color.SetAlpha(alpha);
    }

    private void Update()
    {
		if(laserCannon.isSuspended)
			return;
			
        float fluctuation = 0f;

        if(laserCannon.isFiring)
        {
            fluctuation = Mathf.Abs(Mathf.Sin((Time.time - laserCannon.startFireTime) * 8f) * 0.2f);
        }

        graphicParent.localScale = new Vector3(1f, 1f - fluctuation, 1f);
    }
}
