using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Text.RegularExpressions;

public class NicknameWindow : MonoBehaviour
{
    public RectTransform window;
    public TMP_InputField inputField;
	private System.Action<bool, string> onCallback;

	public void OnClickConfirm()
	{
        if(inputField.text.Length < 2 || inputField.text.Length > 12)
        {
            Debug.LogError("Length not between 2 and 12");
            return;
        }

        if(!Regex.Match(inputField.text, "^[A-Za-z0-9]+(?:[ _-][A-Za-z0-9]+)*$").Success)
        {
            Debug.LogError("Regex Fail");
            return;
        }

		onCallback?.Invoke(true, inputField.text);

		Close();
	}

	public void OnClickCancel()
	{
		onCallback?.Invoke(false, "");

		Close();
	}


	public void Show(string currentNickname, System.Action<bool, string> onCallback)
    {
        inputField.text = currentNickname;
		this.onCallback = onCallback;
		window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        window.gameObject.SetActive(true);
        window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);
    }

	public void Close()
	{
		gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();
	}
}
