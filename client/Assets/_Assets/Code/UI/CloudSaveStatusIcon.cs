using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudSaveStatusIcon : MonoBehaviour
{
    public GameObject child;
    public Image statusImg;
    public Sprite okSprite;
    public Sprite errorSprite;

    private bool hasReturnedSuccessOnce
    {
        get{
            return PlayerPrefs.GetString("cloudSavingEnabled", "false") == "true";
        }
        set{
            PlayerPrefs.SetString("cloudSavingEnabled", value.ToString().ToLower());
        }
    }

    private void OnEnable()
    {
        if(hasReturnedSuccessOnce)
        {
            statusImg.sprite = okSprite;
            return;
        }

#if UNITY_ANDROID

    if(Social.Active.localUser.authenticated)
    {
        AndroidCloudSave.LoadRestoreTokenFromCloud((loadSuccess, restoreToken)=>
        {
            if(!loadSuccess)
                return;

            statusImg.sprite = string.IsNullOrWhiteSpace(restoreToken) ? errorSprite : okSprite;
        });
        
        child.SetActive(true);
    }
    else
    {
        child.SetActive(false);
    }
    
#else
    
    //Keychain
    statusImg.sprite = string.IsNullOrWhiteSpace(SaveManager.RestoreToken) ? errorSprite : okSprite;
    child.SetActive(true);

#endif
    }
}
