using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogWindow : MonoBehaviour
{
	public RectTransform myRect;
    public RectTransform title;
	public RectTransform message;
	public RectTransform buttons;

	public void UpdateSize()
	{
		buttons.anchoredPosition = new Vector2(buttons.anchoredPosition.x, -(title.rect.height + message.rect.height));

		float totalHeight = title.rect.height + message.rect.height + buttons.rect.height;
		myRect.sizeDelta = new Vector2(myRect.sizeDelta.x, totalHeight);
        ((RectTransform)message.parent).sizeDelta = new Vector2(message.sizeDelta.x, message.rect.height);

    }
}
