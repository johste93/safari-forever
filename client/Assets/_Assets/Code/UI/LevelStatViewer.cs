using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DG.Tweening;

public class LevelStatViewer : MonoBehaviour
{
    public TextMeshProUGUI difficulityTextMesh;
    public TextMeshProUGUI clearRateTextMesh;
    public TextMeshProUGUI likesTextMesh;
 
    public TextMeshProUGUI recordTextMesh;
    public TextMeshProUGUI playsTextMesh;
    public TextMeshProUGUI deathsTextMesh;
    public TextMeshProUGUI jumpsTextMesh;

    private int serverLikes = 0;
    public int likeModifier = 0;
    public bool alreadyLiked = false;

    public StatPopup recordHolderPopup;
    public StatPopup totalWinsPopup;
    public StatPopup totalDeathsPopup;

    public void UpdateLikeModifier()
    {
        int totalLikes = serverLikes + likeModifier + (alreadyLiked ? -1 : 0);

        if (totalLikes > 999)
            likesTextMesh.text = (totalLikes / 1000f).ToString("0.0") + "K";
        else
            likesTextMesh.text = totalLikes.ToString();
    }

    public void ToggleRecordHolder()
    {
        totalWinsPopup.Close();
        totalDeathsPopup.Close();

        if(recordHolderPopup.IsShowing())
            recordHolderPopup.Close();
        else
            recordHolderPopup.Show();
    }

    public void ToggleTotalWins()
    {
        recordHolderPopup.Close();
        totalDeathsPopup.Close();

        if(totalWinsPopup.IsShowing())
            totalWinsPopup.Close();
        else
            totalWinsPopup.Show();
    }

    public void ToggleTotalDeaths()
    {
        totalWinsPopup.Close();
        recordHolderPopup.Close();

        if(totalDeathsPopup.IsShowing())
            totalDeathsPopup.Close();
        else
            totalDeathsPopup.Show();
    }

    public void PullStats(string levelId, double currentTime, System.Action<bool> onComplete)
    {
        LevelAPI.PullStats(levelId, (success, stats)=>
        {
            if(!success)
            {
                onComplete?.Invoke(false);
                return;
            }

            Debug.Log(JsonConvert.SerializeObject(stats));

            recordHolderPopup.textMesh.text = stats.RecordHolderNicknameAndIdentifier;

            if(stats.AvgJumps >= 0)
                jumpsTextMesh.text = $"~ {stats.AvgJumps.ToString()}";

            if(stats.AvgDeaths >= 0)
                deathsTextMesh.text = $"~ {stats.AvgDeaths.ToString()}";

            serverLikes = stats.Likes;

            int totalLikes = serverLikes + likeModifier + (alreadyLiked ? -1 : 0);

            if (serverLikes > 999)
                likesTextMesh.text = (totalLikes / 1000f).ToString("0.0") + "K";
            else
                likesTextMesh.text = totalLikes.ToString();


            if(stats.TotalDeaths >= 0)
            {
                if (stats.TotalDeaths > 999)
                    totalDeathsPopup.textMesh.TranslateFormat(TranslationKey.LevelStats_Total, SaveManager.currentSave.language, FontType.Regular, false, $"{(stats.TotalDeaths / 1000f).ToString("0.0")}K");
                else
                    totalDeathsPopup.textMesh.TranslateFormat(TranslationKey.LevelStats_Total, SaveManager.currentSave.language, FontType.Regular, false, $"{stats.TotalDeaths.ToString()}");
            }

            if(stats.Wins >= 0)
            {
                if (stats.Wins > 999)
                    totalWinsPopup.textMesh.TranslateFormat(TranslationKey.LevelStats_Total, SaveManager.currentSave.language, FontType.Regular, false, $"{(stats.Wins / 1000f).ToString("0.0")}K");
                else
                    totalWinsPopup.textMesh.TranslateFormat(TranslationKey.LevelStats_Total, SaveManager.currentSave.language, FontType.Regular, false, $"{stats.Wins.ToString()}");
            }

            clearRateTextMesh.text = Mathf.CeilToInt(stats.ClearRate * 100f) + "%";

            if (playsTextMesh != null)
            {
                if(stats.Plays >= 0)
                {
                    if (stats.Plays > 999)
                        playsTextMesh.text = $"{(stats.Plays / 1000f).ToString("0.0")}K";
                    else
                        playsTextMesh.text = $"{stats.Plays.ToString()}";
                }
            }

            recordTextMesh.color = Color.white;
            recordTextMesh.text = stats.Record;

            if(double.TryParse(stats.Record, out double doubleRecord))
            {
                Debug.Log($"stats.Record: {stats.Record} doubleRecord: {doubleRecord}");

                bool newRecordIsEqualToOldRecord = System.Math.Abs(doubleRecord - currentTime) < 0.001f;
                bool newRecordIsBiggerThan0 = currentTime > 0;
                bool newRecordIsShorterThanOldRecord = currentTime < doubleRecord;
                bool thereIsNoPreviousRecord = doubleRecord <= 0f;

                Debug.Log($"newRecordIsEqualToOldRecord: {newRecordIsEqualToOldRecord} newRecordIsBiggerThan0: {newRecordIsBiggerThan0} newRecordIsShorterThanOldRecord: {newRecordIsShorterThanOldRecord} thereIsNoPreviousRecord: {thereIsNoPreviousRecord}");

                if (newRecordIsBiggerThan0 && ((newRecordIsShorterThanOldRecord && !newRecordIsEqualToOldRecord) || thereIsNoPreviousRecord))
                {
                    Debug.Log("New Record!");
                    recordTextMesh.color = Color.yellow;
                    recordTextMesh.text = Stopwatch.ToString((float)currentTime).Substring(0, 5);
                }
            }

            SaveManager.currentSave.FetchOnlineProfile((profile)=>{
                if(profile == null)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                if(profile.userId == stats.RecordHolderId)
                {
                    Debug.Log("You are the record holder!");
                    recordTextMesh.color = Color.yellow;
                }

                onComplete?.Invoke(true);
            }, true);   

            LocalizeDifficulty(stats.Difficulty, difficulityTextMesh);
        });
    }

    private void LocalizeDifficulty(Difficulty difficulty, TextMeshProUGUI textMesh)
    {
        switch(difficulty)
        {
            case Difficulty.Unrated:
                textMesh.Translate(TranslationKey.Difficulty_Unrated, SaveManager.currentSave.language, FontType.Regular_Outlined, true);
                break;
            case Difficulty.Beginner:
                textMesh.Translate(TranslationKey.Difficulty_Beginner, SaveManager.currentSave.language, FontType.Regular_Outlined, true);
                break;
            case Difficulty.Intermediate:
                textMesh.Translate(TranslationKey.Difficulty_Intermediate, SaveManager.currentSave.language, FontType.Regular_Outlined, true);
                break;
            case Difficulty.Expert:
                textMesh.Translate(TranslationKey.Difficulty_Expert, SaveManager.currentSave.language, FontType.Regular_Outlined, true);
                break;
            case Difficulty.Savant:
                textMesh.Translate(TranslationKey.Difficulty_Savant, SaveManager.currentSave.language, FontType.Regular_Outlined, true);
                break;
        }
    }

    public void Reset()
    {
        LocalizeDifficulty(Difficulty.Unrated, difficulityTextMesh);
        clearRateTextMesh.text = "?";
        likesTextMesh.text = "?";
        recordTextMesh.text = "?";
        playsTextMesh.text = "?";
        deathsTextMesh.text = "?";
        jumpsTextMesh.text = "?";
        recordTextMesh.color = Color.white;
    }
}
