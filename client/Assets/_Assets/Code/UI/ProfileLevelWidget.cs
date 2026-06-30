using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProfileLevelWidget : MonoBehaviour
{
    public TextMeshProUGUI widgetHeader;
    public GameObject levelWidgetElementPrefab;
    public Transform grid;
    public LevelsButton levelsButton;

    //private string userId;

    public void Initalize(string userId, List<LevelInfoDTO> Levels)
    {
        grid.DestroyChildren();

        levelsButton.SetUserId(userId);

        for(int i = 0; i < Mathf.Min(4, Levels.Count); i++)
        {
            SpawnButton(userId, Levels[i]);
        }
    }

    private void SpawnButton(string userId, LevelInfoDTO level)
    {
        GameObject go = Instantiate(levelWidgetElementPrefab, grid);
        go.GetComponent<LevelWidgetElement>().Initalize(level, userId);
    }

    /*
    public void Initalize(string userId)
    {
        this.userId = userId;

        for(int i = 0; i < 4; i++)
        {
            widgets[i].gameObject.SetActive(true);
            widgets[i].loading.SetActive(true);
        }

        FetchLevels(userId, 4, 0);
    }
    
    /*
    private void FetchLevels(string userId, int count, int fromIndex)
    {
        widgetHeader.text = string.Empty;
        LevelFetcher.FetchUserLevels(userId, count, fromIndex, (success, result)=>
        {
            if(!success)
            {
                Debug.LogError("Fetching levels failed.");
                return;
            }

            widgetHeader.text = Localization.GetTranslationFormat("LevelsWidget.Header", SaveManager.currentSave.language, result.Nickname);

            this.userId = userId;
            levelsButton.SetUserId(userId);

            for(int i = 0; i < 4; i++)
            {
                if(result.Levels.Count > i)
                    widgets[i].Initalize(result.Levels[i], userId);
                else
                    widgets[i].gameObject.SetActive(false);
            }

        }, true);
    }
    */
}
