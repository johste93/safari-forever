using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ColorPaletteButton : MonoBehaviour
{
    public Image image;
    public Transform child;

    private ColorPicker.Constraints constraints;
    private ColorPicker colorPicker;

    public void Initalize(ColorPicker colorPicker, Color color, ColorPicker.Constraints constraints)
    {
        image.color = color;
        this.constraints = constraints;
        this.colorPicker = colorPicker;
    }

    public void OnClick()
    {
        colorPicker.onColorUpdate = null;
        child.DOComplete();
        child.DOPunchScale(Vector3.one * 0.1f, 0.3f);
        SetActiveButton();
    }

    public void SetActiveButton()
    {
        colorPicker.color = image.color;
        colorPicker.constraints = constraints;
        colorPicker.onColorUpdate = OnValueChanged;

        colorPicker.selectedColor.transform.DOComplete();
        colorPicker.selectedColor.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
    }

    public void OnValueChanged(Color color)
    {
        image.color = color;
    }

    public Color GetColor()
    {
        return image.color;
    }
}
