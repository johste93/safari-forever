using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelsList : MonoBehaviour
{
    public TextMeshProUGUI widgetHeader;
    public GameObject levelPrefab;
    public RectTransform container;
    public Image scrollBarHandle;
    public CustomScrollRect scrollRect;
    public RectTransform scrollRectTransform;
	public ListLoadAnimation listLoadAnimation;

	public int levelsPrPage = 0;

    private Color scrollBarHandleColor;
    private bool loadLock;

    private int currentIndex;
	private float overDrag;

	public System.Action OnLoadMore;

	public List<LevelElement> listElements = new List<LevelElement>();

    private void Awake()
    {
        scrollBarHandleColor = scrollBarHandle.color;
    }

	public void SpawnLevels(List<LevelInfoDTO> levels, MenuLocation exitLocation = MenuLocation.MainMenu, bool showBoostButton = false)
	{
		foreach(LevelInfoDTO levelInfo in levels)
		{
			SpawnLevel(currentIndex+1, levelInfo, showBoostButton, exitLocation);
			currentIndex++;
		}
		listLoadAnimation.rectTransform.SetAsLastSibling();
	}

    private void SpawnLevel(int index, LevelInfoDTO levelInfo, bool showBoostButton, MenuLocation exitLocation = MenuLocation.MainMenu )
    {
        GameObject last = Instantiate(levelPrefab, container);
        LevelElement element = last.GetComponent<LevelElement>();
        element.Initalize(index, levelInfo, exitLocation, true, showBoostButton);
		listElements.Add(element);
    }

    private void Update()
    {
		float windowHeight = scrollRectTransform.rect.height;
		float contentHeight = scrollRect.content.sizeDelta.y;
		
		float position = (scrollRect.content.anchoredPosition.y + windowHeight);
		
		overDrag = Mathf.Max(0f, position - contentHeight);

		listLoadAnimation?.SetValue(contentHeight < windowHeight ? 0f : Mathf.Clamp01(overDrag/75f));

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

	public void ClearLevels()
	{
		currentIndex = 0;

		foreach(LevelElement go in listElements.ToArray())
			Destroy(go?.gameObject);

		listElements = new List<LevelElement>();
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

		if (overDrag > 75f)
			LoadMore();
	}

	public void LoadMore()
	{
		if (loadLock)
			return;

		if (currentIndex % levelsPrPage != 0)
			return;

		loadLock = true;
		scrollRect.enabled = false;
		scrollRect.EndDrag();

		listLoadAnimation?.SetValue(0.2f);

		OnLoadMore?.Invoke();
	}

	public int GetCurrentIndex()
	{
		return currentIndex;
	}
}
