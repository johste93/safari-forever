using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RankedLevels : MonoBehaviour
{
    public GameObject levelPrefab;
    public Image scrollBarHandle;
    public CustomScrollRect scrollRect;
    public RectTransform scrollRectTransform;
	public ListLoadAnimation listLoadAnimation;

    public Transform newContainer;
    public Transform trendingContainer;
    public Transform popularContainer;

    private Color scrollBarHandleColor;
    private bool loadLock;

	private float overDrag;

	private List<GameObject> listElements = new List<GameObject>();

    public System.Action OnReload;

    private List<Tuple<bool, bool>> isLoading;

    private Coroutine coroutine;

    public void LoadLevels(System.Action<bool> onComplete)
    {
        DialogCanvas.instance.ShowLoading();
        isLoading = new List<Tuple<bool, bool>>{ Tuple.Create(true, false), Tuple.Create(true, false), Tuple.Create(true, false)};

        BrowserAPI.FetchNewLevels(0, 5, (success, response)=>{
            
            isLoading[0] = Tuple.Create(false, success);
            CheckIfDoneLoading(onComplete);

            if(!success)
                return;
            
            SpawnLevels(response.Levels, newContainer, MenuLocation.Browser);
        });

        BrowserAPI.FetchTrendingLevels(0, 5, (success, response)=>{
            
            isLoading[1] = Tuple.Create(false, success);
            CheckIfDoneLoading(onComplete);

            if(!success)
                return;
            
            SpawnLevels(response.Levels, trendingContainer, MenuLocation.Browser);
        });

        BrowserAPI.FetchPopularLevels(0, 5, (success, response)=>{
            
            isLoading[2] = Tuple.Create(false, success);
            CheckIfDoneLoading(onComplete);

            if(!success)
                return;
            
            SpawnLevels(response.Levels, popularContainer, MenuLocation.Browser);
        });
    }

    private void CheckIfDoneLoading(System.Action<bool> onComplete)
    {
        if(IsLoading())
            return;

        coroutine = this.Delay1Frame(()=>{
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) scrollRect.transform);

            DialogCanvas.instance.HideLoading();

            onComplete?.Invoke(LoadingSuccessful());
        });
    }

    private bool IsLoading()
    {
        foreach(Tuple<bool, bool> stillLoading in isLoading)
        {
            if(stillLoading.Item1)
                return true;
        }
        return false;
    }

    private bool LoadingSuccessful()
    {
        foreach(Tuple<bool, bool> stillLoading in isLoading)
        {
            if(!stillLoading.Item2)
                return false;
        }
        return true;
    }

    public void SpawnLevels(List<LevelInfoDTO> levels, Transform container, MenuLocation exitLocation = MenuLocation.MainMenu, bool showBoostButton = false)
	{
        int currentIndex = 0;
		foreach(LevelInfoDTO levelInfo in levels)
		{
			SpawnLevel(currentIndex+1, container, levelInfo, showBoostButton, exitLocation);
			currentIndex++;
		}
		listLoadAnimation.rectTransform.SetAsLastSibling();
	}

    private void SpawnLevel(int index, Transform container, LevelInfoDTO levelInfo, bool showBoostButton, MenuLocation exitLocation = MenuLocation.MainMenu )
    {
        GameObject last = Instantiate(levelPrefab, container);
		listElements.Add(last);
        LevelElement element = last.GetComponent<LevelElement>();
        element.Initalize(index, levelInfo, exitLocation, true, showBoostButton);
    }

    private void Update()
    {
		float windowHeight = scrollRectTransform.rect.height;
		float contentHeight = scrollRect.content.sizeDelta.y;
		
		float position = (scrollRect.content.anchoredPosition.y + windowHeight);
		
		overDrag = Mathf.Max(0f, position - contentHeight);

		//listLoadAnimation?.SetValue(contentHeight < windowHeight ? 0f : Mathf.Clamp01(overDrag/75f));

        if(scrollRect.m_Dragging)
            scrollBarHandleColor.a = Mathf.Clamp(scrollBarHandleColor.a + (Time.deltaTime*2f), 0f, 0.5f);
        else
            scrollBarHandleColor.a = Mathf.Clamp01(scrollBarHandleColor.a - Time.deltaTime);

        scrollBarHandle.color = scrollBarHandleColor;

        if (loadLock)
			if (!scrollRect.m_Dragging && scrollRect.normalizedPosition.y > -0.15f)
			{
				loadLock = false;
			}
    }

    /*
    public void OnValueChanged()
	{
		if(loadLock)
			return;

		if(!scrollRect.m_Dragging)
			return;

		//if contentlist is shorter than window height
		if(scrollRect.content.sizeDelta.y < scrollRectTransform.rect.height)
			return;

		if (overDrag > 75f)
			Reload();
	}

    private void Reload()
    {
        if (loadLock)
			return;

		loadLock = true;
		scrollRect.enabled = false;
		scrollRect.EndDrag();

		listLoadAnimation?.SetValue(0.2f);

        ClearLevels();

		//Debug.Log("Load More!");
		OnReload?.Invoke();

        LoadLevels((success)=>{
            scrollRect.enabled = true;
        });
    }
    */

    public void ClearLevels()
	{
		foreach(GameObject go in listElements.ToArray())
			Destroy(go);

		listElements = new List<GameObject>();
	}

    private void OnDestroy()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}
