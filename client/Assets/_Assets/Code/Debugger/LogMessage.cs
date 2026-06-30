using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class LogMessage : MonoBehaviour {

	private Image image;

	private RectTransform rectTransform;
	private LayoutElement layoutElement;
	private TextMeshProUGUI textMesh;
	
	private LogEntry logEntry;

	private const int colorTagLength = 7;

	private bool hasCopied;

	private float lastCopied;
	private float heldDuration;

	private float minimumSize = 52f;

	public void MatchChildSize()
	{
		if(textMesh.rectTransform.rect.height + 12 > minimumSize)
			StartCoroutine(SetHeight(textMesh.rectTransform.rect.height + 12));
	}	
	
	public void Shrink()
	{
		StartCoroutine(SetHeight(minimumSize));
		gameObject.SetActive(true);
	} 

	public void Hide()
	{
		StartCoroutine(SetHeight(0));
		gameObject.SetActive(false);
	}

	public void On_ToggleTransparent(bool transparent)
	{
		if(image == null)
			image = GetComponent<Image>();

		image.color = image.color.SetAlpha(transparent ? 1f/255f : 0.2f);
	}

	private IEnumerator SetHeight(float size)
	{
		yield return 0;

		if(layoutElement == null)
			layoutElement = GetComponent<LayoutElement>();

		layoutElement.minHeight = size;
		layoutElement.preferredHeight = size;

		if(rectTransform == null)
			rectTransform = GetComponent<RectTransform>();

		rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, size);
	}

	public void On_TouchStart(TouchInfo touch)
	{
		hasCopied = false;
	}

	public void On_TouchMaintained(TouchInfo touch)
	{
		if(!DebugManager.instance.containerImg.gameObject.activeInHierarchy)
			return;

		if(touch.pickedUIElement != gameObject)
			return;

		if(hasCopied)
			return;

		heldDuration += Time.deltaTime;

		if(heldDuration > 2f)
		{	
			Copy();
		}
	}

	public void On_TouchEnd(TouchInfo touch)
	{
		if(touch.pickedUIElement != gameObject)
			return;

		if(touch.duration < Globals.gameConstants.standardTapDuration)
		{
			if(layoutElement == null)
				layoutElement = GetComponent<LayoutElement>();
				
			if(layoutElement.minHeight == minimumSize)
				MatchChildSize();
			else
				Shrink();

			return;
		}
	}
	

	public void Copy()
	{
		if(Time.time - lastCopied < 1.05f)
			return;

		hasCopied = true;
		lastCopied = Time.time;
		heldDuration = 0f;

		string before = textMesh.text;
		
		string msg = logEntry.message + "\n" + logEntry.stackTrace;
		msg.CopyToClipboard();

		textMesh.text = "Copied to clipboard!";
		
		this.Delay(1f, ()=>{
			textMesh.text = before;
		});
	}

	public void Initalize(LogEntry logEntry, bool show )
	{
		this.logEntry = logEntry;
		textMesh = GetComponentInChildren<TextMeshProUGUI>();

		string message = logEntry.message;
		
		if(message.Substring(0, Mathf.Min(colorTagLength, message.Length)).Equals("<color=", StringComparison.OrdinalIgnoreCase))
		{
			int positionOfFirstCloseColorTag = message.IndexOf('>');

			string colorName = message.Substring(colorTagLength, positionOfFirstCloseColorTag - colorTagLength);

			if(colorName[0] != '#')
			{
				Color c = ColorParser.Parse(colorName);
				string hexColor = "#" + ColorUtility.ToHtmlStringRGB(c);

				message = message.Replace(colorName, hexColor);
			}
		}

		textMesh.text = message + "\n" + logEntry.stackTrace;
		textMesh.color = DebugManager.logTypeColors[logEntry.type];

		On_ToggleTransparent(DebugManager.instance.IsTransparent());

		if(!show)
			Hide();
	}

	private void On_ToggleLogType(LogType type, bool show)
	{
		if(logEntry.type == type)
		{
			if(show)
			{
				if(layoutElement.minHeight == 0)
					Shrink();
			}
			else
				Hide();
		}
	}

	private void OnEnable()
	{
		DebugManager.On_ToggleTransparent += On_ToggleTransparent;
		DebugManager.On_ToggleLogType += On_ToggleLogType;
		TouchInput.On_TouchStart += On_TouchStart;
		TouchInput.On_TouchMaintained += On_TouchMaintained;
		TouchInput.On_TouchEnd += On_TouchEnd;
	}

	private void Unsubscribe()
	{
		DebugManager.On_ToggleTransparent -= On_ToggleTransparent;
		DebugManager.On_ToggleLogType -= On_ToggleLogType;
		TouchInput.On_TouchStart -= On_TouchStart;
		TouchInput.On_TouchMaintained -= On_TouchMaintained;
		TouchInput.On_TouchEnd -= On_TouchEnd;
	}

	private void OnDestroy()
	{
		Unsubscribe();
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

}
