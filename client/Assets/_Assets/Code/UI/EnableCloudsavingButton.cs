using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableCloudsavingButton : MonoBehaviour
{
    public OptionsCanvas optionsCanvas;

    public void OnClick()
    {
        new Dialog(TranslationKey.Cloudsaving_Restore_Enable_Title, TranslationKey.Cloudsaving_Restore_Enable_Body)
                .AddNegativeButton(TranslationKey.Generic_Cancel, null)
                .AddPositiveButton(TranslationKey.Generic_Confirm,()=>{
                    EnableRestore();
                }, true).Show();
        
    }

    private void EnableRestore()
    {
        DialogCanvas.instance.ShowLoading();
#if UNITY_ANDROID// && !UNITY_EDITOR
        AndroidCloudSave.Authenticate((loginSuccess)=>
        {
            if(!loginSuccess)
			{
                DialogCanvas.instance.HideLoading();
				//Error not sign in. Ask player what to do:
				// Try again
				// Cancel
				new Dialog(TranslationKey.Cloudsaving_GooglePlayGames_Settings_Error_SignIn_Title, TranslationKey.Cloudsaving_GooglePlayGames_Settings_Error_SignIn_Body)
				.AddNegativeButton(TranslationKey.Generic_Cancel, null)
                .AddNeutralButton(TranslationKey.Generic_TryAgain, ()=>
				{
					EnableRestore();
				})
                .Show();
				return;
			}

            UserAPI.EnableRestore((restoreSuccess)=>
            {
                if(!restoreSuccess)
                {
                    DialogCanvas.instance.HideLoading();
                    return;
                }
                
                //Upload Key
                AndroidCloudSave.SaveRestoreTokenToCloud(SaveManager.RestoreToken, (saveSuccess)=>
                {
                    DialogCanvas.instance.HideLoading();
                    if(!saveSuccess)
                    {
                        //Error not sign in. Ask player what to do:
                        // Try again
                        // Cancel
                        new Dialog(TranslationKey.Cloudsaving_GooglePlayGames_Settings_Error_FailedUpload_Title, TranslationKey.Cloudsaving_GooglePlayGames_Settings_Error_FailedUpload_Body)
                        .AddNegativeButton(TranslationKey.Generic_Cancel, null)
                        .AddNeutralButton(TranslationKey.Generic_TryAgain, ()=>
                        {
                            EnableRestore();
                        })
                        .Show();
                        return;
                    }

                    new Dialog(TranslationKey.Cloudsaving_Restore_Enabled_Title, TranslationKey.Cloudsaving_Restore_Enabled_Body)
                        .AddPositiveButton(TranslationKey.Generic_Ok,null);

                    optionsCanvas.UpdateButtonVisiblity();
                });
            });
        });
#else
        UserAPI.EnableRestore((success)=>{
            if(success)
            {
                DialogCanvas.instance.Show(new Dialog(TranslationKey.Cloudsaving_Restore_Enabled_Title, TranslationKey.Cloudsaving_Restore_Enabled_Body)
                    .AddPositiveButton(TranslationKey.Generic_Ok,null));
            }
            optionsCanvas.UpdateButtonVisiblity();
        });
#endif
    }
}
