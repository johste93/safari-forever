using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GoogleGameServices
{
    public static void SignIn(System.Action onComplete)
    {
        if (Social.localUser.authenticated)
        {
            onComplete();
            return;
        }

        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("Welcome " + Social.localUser.userName);
            }
            else
            {
                Debug.Log("Authentication failed.");
            }

            onComplete();
        });
    }
}
