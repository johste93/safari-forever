using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class AgeVerificationWindow : MonoBehaviour
{
    public RectTransform window;
    public TMP_InputField inputField;
	private System.Action<bool, int> onCallback;

    public GameObject cancelButton;

	public void OnClickConfirm()
	{
        if(inputField.text.Length == 0)
        {
            Debug.LogError("Input empty");
            return;
        }

        if(!int.TryParse(inputField.text, out int age))
        {
            Debug.LogError("Unable to parse input");
            return;
        }

        if(age < 0 || age > 120)
        {
            Debug.LogError("Age outside of human lifespan.");
            return;
        }

		onCallback?.Invoke(true, age);

		Close();
	}

	public void OnClickCancel()
	{
		onCallback?.Invoke(false, -1);

		Close();
	}


	public void Show(System.Action<bool, int> onCallback, bool showCancelButton)
    {
        cancelButton.SetActive(showCancelButton);

        inputField.text = string.Empty;
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
