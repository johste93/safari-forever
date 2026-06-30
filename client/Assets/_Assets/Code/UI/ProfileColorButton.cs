using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ProfileColorButton : MonoBehaviour
{
    public RectTransform graphic;

    public void OnClick()
    {
        graphic.DOComplete();
        graphic.DOPunchScale(Vector3.one * 0.1f, 0.3f);

        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            if(profile == null)
                return;

            ColorPicker.Constraints constraints = new ColorPicker.Constraints(){
                vibrance = new Vector2(0.2f, 0.9f)
            };
            
            ColorUtility.TryParseHtmlString(profile.color, out Color previousColor);
            DialogCanvas.instance.ShowColorPicker(true, true, true, previousColor, constraints, (confirmed, pickedColor)=>
            {
                if(!confirmed)
                    return;

                string hexColor = $"#{ColorUtility.ToHtmlStringRGB(pickedColor)}";

                UserAPI.UpdateColor(hexColor, (success, colorResponse)=>
                {
                    if(!success)
                        return;

                    profile.color = colorResponse.Color;
                    SaveManager.Save();

                    ColorUtility.TryParseHtmlString(profile.color, out Color newColor);

                    PopupCanvas.instance.background.DOKill();
                    PopupCanvas.instance.background.DOColor(newColor, 0.3f);
                });
            });
        });
    }
}
