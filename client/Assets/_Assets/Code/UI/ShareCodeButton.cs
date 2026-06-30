using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MandarinDuck.NativeShareDialog;

public class ShareCodeButton : MonoBehaviour
{
    public string shareUrl;
    
    public void OnClick()
    {
        SocialManager.Share( shareUrl );

        if(Application.isEditor)
            shareUrl.CopyToClipboard();

    }
}
