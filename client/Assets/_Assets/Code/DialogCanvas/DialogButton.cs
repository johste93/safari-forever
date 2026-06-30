using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogButton : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public Color destructiveColor;

    private System.Action callback;
    private DialogButtonValues values;

    public void Initalize(DialogButtonValues dialogButtonValues)
    {
        values = dialogButtonValues;
        callback = dialogButtonValues.callback;

        string buttonText = "";
        if(!string.IsNullOrWhiteSpace(dialogButtonValues.text))
        {
            buttonText = dialogButtonValues.isPreferableButton ? $"<b>{dialogButtonValues.text}</b>" : dialogButtonValues.text;
            textMesh.text = buttonText;
            textMesh.isRightToLeftText = dialogButtonValues.isRightToLeftText;
            textMesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized, textMesh.font, dialogButtonValues.language);
            //Dont change alignment
        }
        else
        {
            textMesh.Translate(dialogButtonValues.key, dialogButtonValues.language, FontType.Regular, false);
        }

        //if(dialogButtonValues.isPreferableButton)
            //textMesh.fontStyle = FontStyles.Bold;        

        if(dialogButtonValues.type == DialogButtonType.Destructive)
            textMesh.color = destructiveColor;
    }

    public void OnClick()
    {
        DialogCanvas.instance.Complete();

        if(callback != null)
            callback();

        if(DialogCanvas.instance == null)
        {
            Debug.LogError("No Dialog Canvas exsists in the scene!");
            return;
        }
    }
}
