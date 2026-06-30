using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLevelEditorButton : MonoBehaviour
{
    public void OnClick()
    {
        if(GlobalSingleton.mode == GameMode.Create)
		{
			if(!LevelBuilder.instance.IsLevelEmpty())
			{
				GameMaster.instance.ResetLevel(true);
				Garage.SaveWorkInProgressLevel();
			}
		}

        SceneLoader.Load(SafariScene.Menu);
    }
}
