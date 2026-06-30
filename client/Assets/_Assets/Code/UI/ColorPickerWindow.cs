using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ColorPickerWindow : MonoBehaviour
{
    public GameObject colorPaletteButtonPrefab;
    public RectTransform paletteGroup;

    public ColorPicker colorPicker;

    public RectTransform window;

    private System.Action<bool, Color> onCallback;
    private System.Action<bool, Color[]> onCallbackArray;

    private List<ColorPaletteButton> paletteButtons;
    
    public void OnClickConfirm()
    {
        onCallback?.Invoke(true, colorPicker.color);
        onCallback = null;

        if(onCallbackArray != null)
        {
            List<Color> colors = new List<Color>();
            foreach(ColorPaletteButton btn in paletteButtons)
                colors.Add(btn.GetColor());

            onCallbackArray.Invoke(true, colors.ToArray());
            onCallbackArray = null;
        }

        Close();
    }

    public void OnClickCancel()
	{
		onCallback?.Invoke(false, Color.black);
        onCallback = null;

        onCallbackArray?.Invoke(false, null);
        onCallbackArray = null;

		Close();
	}

    public ColorPicker Show(bool hue, bool saturation, bool vibrance, Color currentColor, System.Action<bool, Color> onCallback)
    {
        paletteGroup.gameObject.SetActive(false);

        colorPicker.constraints = new ColorPicker.Constraints();
        colorPicker.hueObject.SetActive(hue);
        colorPicker.saturationObject.SetActive(saturation);
        colorPicker.vibranceObject.SetActive(vibrance);

        colorPicker.color = currentColor;
		this.onCallback = onCallback;
		window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        window.gameObject.SetActive(true);

        paletteGroup.gameObject.SetActive(false);

        this.Delay1Frame(() =>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(window);
            LayoutRebuilder.ForceRebuildLayoutImmediate(window);
            window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);
        });

        return colorPicker;
    }

    public ColorPicker Show(bool hue, bool saturation, bool vibrance, Color[] colors, ColorPicker.Constraints[] constraints, System.Action<bool, Color[]> onCallback)
    {
        colorPicker.constraints = constraints[0];
        colorPicker.hueObject.SetActive(hue);
        colorPicker.saturationObject.SetActive(saturation);
        colorPicker.vibranceObject.SetActive(vibrance);

        colorPicker.color = colors[0];
		this.onCallbackArray = onCallback;
		window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        window.gameObject.SetActive(true);
        this.Delay1Frame(() =>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(window);
            LayoutRebuilder.ForceRebuildLayoutImmediate(window);
            window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);
        });

        paletteGroup.gameObject.SetActive(colors.Length > 1);

        paletteGroup.DestroyChildren();

        paletteButtons = new List<ColorPaletteButton>();
        for(int i = 0; i < colors.Length; i++)
        {
            GameObject go = Instantiate(colorPaletteButtonPrefab, paletteGroup);
            ColorPaletteButton btn = go.GetComponent<ColorPaletteButton>();
            paletteButtons.Add(btn);

            btn.Initalize(colorPicker, colors[i], constraints[i]);

            if(i == 0)
                btn.SetActiveButton();
        }

        return colorPicker;
    }

	public void Close()
	{
		gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();
	}
}
