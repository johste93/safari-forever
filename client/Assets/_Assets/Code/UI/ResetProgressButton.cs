using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetProgressButton : MonoBehaviour
{
    public void OnClick()
    {
		//new Dialog("","", false).Show();
		new Dialog(TranslationKey.Menu_Options_ResetProgress_Title, TranslationKey.Menu_Options_ResetProgress_Body)
        .AddPositiveButton(TranslationKey.Generic_Cancel, null)
        .AddDestructiveButton(TranslationKey.Generic_Yes, ()=>{
            PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
            Garage.DeleteWorkInProgressLevel();
            

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif              
        }).Show();
    }
}
