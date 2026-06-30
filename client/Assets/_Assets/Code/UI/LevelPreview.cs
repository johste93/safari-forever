using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class LevelPreview : MonoBehaviour
{
    public Image img;
    public TextMeshProUGUI nameTextMesh;
	public UIButton uIButton;
	public Button button;

	public VideoPlayer player;

	public void OnClick()
	{
		
#if UNITY_WEBGL
		Application.OpenURL("https://safariforever.com/");
		return;
#else
		player.gameObject.SetActive(true);
		player.time = 0;
		player.SetDirectAudioMute(0, !SaveManager.currentSave.sfx);
		player.Play();
#endif
	}

	private void Awake()
	{
		//uIButton.enabled = button.enabled = Application.isEditor;
		player.loopPointReached += (videoPlayer)=>{
			player.gameObject.SetActive(false);
		};
	}

    public void SetName(string name)
    {
        nameTextMesh.text = name;
		nameTextMesh.isRightToLeftText = false;
		nameTextMesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized_Outlined, nameTextMesh.font, Globals.localizationConstants.defaultLanguage);
		//Dont change alignment
        nameTextMesh.enabled = !string.IsNullOrEmpty(nameTextMesh.text);
    }

    public void SetImage(Sprite sprite)
    {
        img.sprite = sprite;
        img.enabled = img.sprite != null;
    }

	public void FromBytes(byte[] bytes)
	{
		Sprite preview = bytes.ToSpriteFromGIF();

		if(preview == null)
		{
			Debug.LogError("Unable to Parse Image");
			return;
		}

		SetImage(preview);
	}

	public void LoadImage(string levelId, System.Action callback)
	{
		LevelAPI.GetImage(levelId, (success, bytes)=>
		{
			if(!success)
			{
				callback?.Invoke();
				return;
			}
				
			FromBytes(bytes);
			callback?.Invoke();
		});
	}

	public void Reset()
	{
		SetName("");
		SetImage(null);
	}

	private void OnDisable()
	{
		player.Stop();
		player.gameObject.SetActive(false);
	}
}
