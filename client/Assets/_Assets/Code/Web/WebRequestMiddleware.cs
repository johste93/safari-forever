using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebRequestMiddleware
{
    public static void FetchAndVerifyProfile(bool failSilently,  System.Action<bool, Profile> onComplete)
    {
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            if(profile == null)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            //Debug.Log(profile.token);

            ProfileVerifier.VerifyProfile(profile, (profileVerified)=>
            {
                onComplete?.Invoke(profileVerified, profile);
            }, failSilently);
        }, failSilently);
    }
}
