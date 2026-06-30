using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerList : MonoBehaviour
{
    public TextMeshProUGUI widgetHeader;
    public GameObject playerPrefab;
    public Transform container;
    public Image scrollBarHandle;
    public CustomScrollRect scrollRect;
    public RectTransform contentAnchor;
    public RectTransform scrollRectTransform;

    private Color scrollBarHandleColor;
    private bool loadLock;

    private int currentIndex;
    private int usersPrPage;
    private bool doneLoading;

    private string userId;

    private void Awake()
    {
        scrollBarHandleColor = scrollBarHandle.color;
    }

    public void Initalize(string userId)
    {
        this.userId = userId;
    }

    public void SpawnPlayer(FollowedUserDTO followedUserDTO)
    {
        GameObject last = Instantiate(playerPrefab, container);
        PlayerElement element = last.GetComponent<PlayerElement>();
        element.Initalize(followedUserDTO);
    }

    private void Update()
    {
        if(scrollRect.m_Dragging)
            scrollBarHandleColor.a = Mathf.Clamp(scrollBarHandleColor.a + (Time.deltaTime*2f), 0f, 0.5f);
        else
            scrollBarHandleColor.a = Mathf.Clamp01(scrollBarHandleColor.a - Time.deltaTime);

        scrollBarHandle.color = scrollBarHandleColor;

        if (loadLock)
            if (!scrollRect.m_Dragging && scrollRect.normalizedPosition.y > -0.15f)
                loadLock = false;
    }

    public void LoadFollows()
    {
        ClearList();
        FetchPlayers(userId, 0, (nickname)=>
        {
            bool usedLanguageIsRightToLeft = Localization.IsRightToLeftLanguage(Localization.KeyAvailable(TranslationKey.FollowingWidget_Header, SaveManager.currentSave.language) ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage);
            widgetHeader.TranslateFormat(TranslationKey.FollowingWidget_Header, SaveManager.currentSave.language, FontType.Stylized_Outlined, false, usedLanguageIsRightToLeft ? $"<ltr>{nickname}</ltr>" : nickname);
        });
    }

    private void FetchPlayers(string userId, int fromIndex, System.Action<string> onComplete = null)
    {
        doneLoading = false;
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {    
            if(profile == null)
            {
                doneLoading = true;
                return;
            }

            DialogCanvas.instance.ShowLoading();

            FollowerAPI.FetchFollowings(fromIndex, (success, response)=>
            {
                doneLoading = true;
    
                if(!success)
                {
                    Debug.LogError("Fetching follows failed.");
                    return;
                }

                usersPrPage = response.UsersPrPage;
                
                foreach(FollowedUserDTO followedUserDTO in response.followedUsers)
                {
                    SpawnPlayer(followedUserDTO);
                    currentIndex++;
                }

                onComplete?.Invoke(profile.nickname);
            });
        });
    }

    public void ClearList()
    {
        currentIndex = 0;
        container.DestroyChildren();
    }

    public void OnValueChanged()
    {
        if(!doneLoading)
            return;

        if(!scrollRect.m_Dragging)
            return;

        float windowHeight = scrollRectTransform.rect.height;
        float contentHeight = scrollRect.content.sizeDelta.y;

        if(contentHeight < windowHeight)
            return;

        float position = (scrollRect.content.anchoredPosition.y + windowHeight);

        if (Mathf.Max(0f, position - contentHeight) > 75f)
            LoadMore();
    }

    public void LoadMore()
    {
        if (loadLock)
            return;

        loadLock = true;

        if (currentIndex % usersPrPage != 0)
        {
            Debug.Log("No More notifications to load!");
            return;
        }

        Debug.Log("Load More!");
        FetchPlayers(userId, currentIndex);
    }
}
