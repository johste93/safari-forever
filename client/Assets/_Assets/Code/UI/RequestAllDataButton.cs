using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MandarinDuck.NativeShareDialog;

public class RequestAllDataButton : MonoBehaviour
{

    public void OnClick()
	{
		UserAPI.RequestAllUserData((success, result)=>
		{
			if(!success)
				return;

			if(Application.isEditor)
				result.CopyToClipboard();

			SocialManager.Share(result);
		});
	}

	
}
