using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentAlphaLetter : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public RectTransform rectTransform;

    private AlfabeticalSeparator[] separators;

    public void FindSeparators()
    {
        separators = transform.parent.parent.GetComponentsInChildren<AlfabeticalSeparator>();
    }

    public void ClearSeparators()
    {
        separators = new AlfabeticalSeparator[0];
    }

    public void OnValueChanged(Vector2 value)
    {
        AlfabeticalSeparator closest = null;
        
        foreach(AlfabeticalSeparator separator in separators)
        {
            if(!separator.gameObject.activeInHierarchy)
                continue;

            if(separator.rectTransform.position.y < rectTransform.position.y)
                continue;

            if(closest == null || MathIsHard.DistanceBetweenFloats(separator.rectTransform.position.y, rectTransform.position.y) < MathIsHard.DistanceBetweenFloats(closest.rectTransform.position.y, rectTransform.position.y))
                closest = separator;
        }

        if(closest != null)
            textMeshProUGUI.text = closest.textMesh.text;
        else
            textMeshProUGUI.text = "";
    }
}
