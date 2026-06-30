using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProgressAnimation : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;

    private void OnEnable()
    {
        textMeshProUGUI.text = "0%";
    }

    public void SetValue(float value)
    {
        textMeshProUGUI.text = $"{Mathf.FloorToInt(Mathf.Clamp01(value) * 100f)}%";
    }   
}
