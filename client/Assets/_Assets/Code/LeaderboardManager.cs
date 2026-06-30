using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform contentParent;
    public CustomScrollRect scrollRect;

    public ScreenScroller screenScroller;

    public UIButton previousPage;
    public UIButton nextPage;

    public ListLoadAnimation topLoadAnimation;
    public ListLoadAnimation bottomLoadAnimation;

    public Image scrollBarHandle;
    public RectTransform scrollRectTransform;

    public TextMeshProUGUI playerRank;

    private int currentPage = 0;
    private List<GameObject> elements = new List<GameObject>();

    private bool loadLock;
    private float underDrag;
    private float overDrag;
    private Color scrollBarHandleColor;
    private bool waitingForPlayerToEndTouch;

    private void Update()
    {
        float windowHeight = scrollRectTransform.rect.height;
        float contentHeight = scrollRect.content.sizeDelta.y;
        
        float topPosition = (scrollRect.content.anchoredPosition.y - (windowHeight/2));
        float bottomPosition = topPosition + windowHeight;

        underDrag = topPosition < 0 ? Mathf.Abs(topPosition) : 0;
        overDrag = bottomPosition > 0 ? bottomPosition - contentHeight : 0;

        topLoadAnimation.SetValue(currentPage == 0 || contentHeight < windowHeight ? 0f : Mathf.Clamp01(underDrag/100f));
        bottomLoadAnimation.SetValue(contentHeight < windowHeight ? 0f : Mathf.Clamp01(overDrag/100f));

        if(scrollRect.m_Dragging)
            scrollBarHandleColor.a = Mathf.Clamp(scrollBarHandleColor.a + (Time.deltaTime*2f), 0f, 0.5f);
        else
            scrollBarHandleColor.a = Mathf.Clamp01(scrollBarHandleColor.a - Time.deltaTime);

        scrollBarHandle.color = scrollBarHandleColor;

        if (loadLock)
            if (!scrollRect.m_Dragging && (scrollRect.normalizedPosition.y > -0.05f || scrollRect.normalizedPosition.y < 1.05f))
                loadLock = false;

        if(waitingForPlayerToEndTouch && TouchInput.GetTouchCount() == 0)
        {
            scrollRect.enabled = true;
        }
    }

    public void OnValueChanged()
    {
        if(loadLock)
            return;

        if(!scrollRect.m_Dragging)
            return;
        
        //if contentlist is shorter than window height
        if(scrollRect.content.sizeDelta.y < scrollRectTransform.rect.height)
            return;

        if (underDrag > 100f)
            PreviousPage();

        if (overDrag > 100f)
            NextPage();
    }

    public void ShowLeaderboard()
    {
        if(elements.Count == 0)
        {
            LoadPage(0, (success)=>
            {
                if(!success)
                    return;

                screenScroller.Next();
            });
        }
        else
            screenScroller.Next();
    }

    public void NextPage()
    {
        LoadPage(currentPage+1, null);
    }

    public void PreviousPage()
    {
        if(currentPage == 0)
            return;

        LoadPage(Mathf.Max(0, currentPage-1), null);
    }

    private void LoadPage(int page, System.Action<bool> onComplete)
    {
        loadLock = true;
        scrollRect.enabled = false;
		scrollRect.EndDrag();

        DialogCanvas.instance.ShowLoading();
        EndlessAPI.FetchLeaderboard(page, (success, response)=>
        {
            DialogCanvas.instance.HideLoading();
            if(!success)
            {
                scrollRect.enabled = true;
                onComplete?.Invoke(false);
                return;
            }

            SpawnLeaderboard(response.Leaderboard, currentPage > page);

            scrollRect.enabled = true;

            currentPage = page;

            string rank = "n/a";

            if(response.PersonalRank > 0)
                rank = response.PersonalRank.ToString();

            playerRank.TranslateFormat(TranslationKey.Leaderboard_Rank, SaveManager.currentSave.language, FontType.Regular_Outlined, false, rank);

            UpdateButtons();

            onComplete?.Invoke(true);
        });
    }

    private void UpdateButtons()
    {
        previousPage.SetInteractable(currentPage > 0);
        nextPage.SetInteractable(elements.Count > 0);
    }

    private void SpawnLeaderboard(List<EndlessLeaderboardDTO> contestants, bool scrollToBottom)
    {
        int count = elements.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            GameObject.Destroy(elements[i]);
        }

        elements = new List<GameObject>();

        foreach(EndlessLeaderboardDTO dto in contestants)
        {
            GameObject go = Instantiate(playerPrefab, contentParent);
            go.transform.SetSiblingIndex(contentParent.childCount-2);
            elements.Add(go);
            PlayerRank playerRank = go.GetComponent<PlayerRank>();
            playerRank.Initalize(dto.Nickname, dto.Identifier, dto.Score, dto.Rank);
        }

        //scrollRect.content.anchoredPosition = new Vector2(0,scrollRectTransform.rect.height/2f);
        scrollRect.normalizedPosition = new Vector2(0, scrollToBottom ? 0 : 1);
    }
}
