using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugManager : ImmortalSingleton<DebugManager> {

	public KeyCode debugKey = KeyCode.F5;
	public GameObject logMessagePrefab;
	public Image containerImg;
	public RectTransform scrollRectRect;
	public TextMeshProUGUI fpsCounterText;
	public Toggle verboseToggle;

	public LayoutElement listLayoutElement;
	
	private ScrollRect scrollRect;
	private int secretKnocks = 0;
	private int numberOfFingersOnTheScreenLastFrame;
	private RectTransform logMessageParent;

	private GameObject debugCanvas;

	public delegate void ToggleLogTypeEvent(LogType type, bool show);
	public static ToggleLogTypeEvent On_ToggleLogType;

	public delegate void ToggleTransparentEvent(bool transparent);
	public static ToggleTransparentEvent On_ToggleTransparent;

	private bool logsEnabled = true;
	private bool warningsEnabled = true;
	private bool errorsEnabled = true;

	private bool scrollToBottom = true;

	private bool blockRaycasts = true;
	private CanvasGroup[] canvasGroups;

	public static float fps;
	private float count;

	private bool isOpen;

	private List<Log> logCache = new List<Log>();

	private struct Log
	{
		public string message;
		public string stackTrace; 
		public LogType type; 
	}

	protected override void Awake()
	{
		base.Awake();
		debugCanvas = transform.GetChild(0).GetChild(0).gameObject;
		logMessageParent = GetComponentInChildren<VerticalLayoutGroup>(true).GetComponent<RectTransform>();
		scrollRect = scrollRectRect.GetComponent<ScrollRect>();
	}

	private IEnumerator Start()
	{	
		yield return 0;
		listLayoutElement.minHeight = scrollRect.GetComponent<RectTransform>().rect.height;
		
		StartCoroutine(DetectInput());

		yield return new WaitForSeconds (0.1f);
		while (true) 
		{
			count = ((1f/Time.timeScale) / Time.deltaTime);
			fps = Mathf.RoundToInt (count);
			fpsCounterText.text = fps.ToString();
			yield return new WaitForSeconds (0.5f);
		}
	}
	

	private void Update()
	{
		if(!Application.isEditor)
			return;
			
		if(Input.GetKeyDown(debugKey))
		{
			Toggle();
		}

		if(Input.GetKey(KeyCode.F6))
		{
			Debug.Log("Test");
			Debug.LogWarning("Test");
			Debug.LogError("Test");
		}
	}

	public bool ToggleRaycastBlocking()
	{
		blockRaycasts = !blockRaycasts;
		if(canvasGroups == null)
		{
			canvasGroups = GetComponentsInChildren<CanvasGroup>(true);
		}
	
		foreach(CanvasGroup cG in canvasGroups)
		{
			cG.blocksRaycasts = blockRaycasts;
			cG.alpha = blockRaycasts ? 1f : 0.5f;
		}

		containerImg.raycastTarget = blockRaycasts;
		containerImg.color = containerImg.color.SetAlpha(blockRaycasts ? 0.5f : 0f);
		
		fpsCounterText.color = new Color(fpsCounterText.color.r, fpsCounterText.color.g, fpsCounterText.color.b, blockRaycasts ? 1f : 0.75f);

		On_ToggleTransparent?.Invoke(!blockRaycasts);

		return blockRaycasts;
	}

	private void Toggle()
	{
		ShowDebugger(!debugCanvas.activeInHierarchy);
	}

	public bool IsTransparent()
	{
		return !blockRaycasts;
	}

	private IEnumerator DetectInput()
	{
		while(true)
		{
			if(Time.time > 1f)
			{
				if(secretKnocks > 12)
				{
					ShowDebugger(true);
				}

				if(TouchInput.GetTouchCount() >= 2 && TouchInput.GetTouchCount() <= 3 )
				{
					if(numberOfFingersOnTheScreenLastFrame - TouchInput.GetTouchCount() == -1)
					{
						secretKnocks++;
					}

					numberOfFingersOnTheScreenLastFrame = TouchInput.GetTouchCount();
				}
				else
				{
					secretKnocks = 0;
				}
			}
			
			yield return 0;
		}
	}

	public void ShowDebugger(bool show)
	{
		debugCanvas.SetActive(show);
		secretKnocks = 0;

		if(show)
		{
			SpawnLogs();
			verboseToggle.isOn = Globals.debugConstants.verboseLogging;
		}

		isOpen = show;
	}

	public void OnVerboseLoggingToggled(bool value)
	{
		Globals.debugConstants.verboseLogging = value;
		verboseToggle.isOn = Globals.debugConstants.verboseLogging;
	}

	public bool IsOpen()
	{
		return isOpen;
	}

	private void SpawnLogs()
	{
		List<Log> logsToSpawn = new List<Log>(logCache);
		foreach(Log log in logsToSpawn)
		{
			SpawnLog(log);
		}

		logCache = new List<Log>();
	}

	private void SpawnLog(Log log)
	{
		GameObject lastSpawn = Instantiate(logMessagePrefab, logMessageParent) as GameObject;

		bool show = false;
		switch(log.type)
		{
			case LogType.Log:
				show = logsEnabled;
			break;
			case LogType.Warning:
				show = warningsEnabled;
			break;
			case LogType.Error:
			case LogType.Exception:
			case LogType.Assert:
				show = errorsEnabled;
			break;
		}

		lastSpawn.GetComponent<LogMessage>().Initalize(new LogEntry(log.message, log.stackTrace, log.type), show);

		if(scrollToBottom)
		{
			if(scrollToBottomRoutine == null)
				scrollToBottomRoutine = StartCoroutine(ScrollToBottom());
		}
	}

	public void Clear()
	{
		logMessageParent.DestroyChildren();
	}

	public void ToggleLogs()
	{
		if(On_ToggleLogType != null)
			On_ToggleLogType(LogType.Log, logsEnabled = !logsEnabled);
	}

	public void ToggleWarnings()
	{
		if(On_ToggleLogType != null)
			On_ToggleLogType(LogType.Warning, warningsEnabled = !warningsEnabled);
	}

	public void ToggleErrors()
	{
		if(On_ToggleLogType != null)
		{
			On_ToggleLogType(LogType.Error, errorsEnabled = !errorsEnabled);
			On_ToggleLogType(LogType.Assert, errorsEnabled = !errorsEnabled);
			On_ToggleLogType(LogType.Exception, errorsEnabled = !errorsEnabled);
		}
			
	}

	public void ToggleScrollToBottom(bool scrollToBottom)
	{
		this.scrollToBottom = scrollToBottom;
	}

	private void HandleLog(string message, string stackTrace, LogType type )
    {
		Log log = new Log(){
			message = message,
			stackTrace = stackTrace,
			type = type
		};

		
		if(IsOpen())
			SpawnLog(log);		
		else
			logCache.Add(log);
    }

	private Coroutine scrollToBottomRoutine;

	private IEnumerator ScrollToBottom()
	{
		yield return new WaitForEndOfFrame();
		float scrollRectPosition = scrollRect.content.rect.height - scrollRectRect.rect.height;
		scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, scrollRectPosition);
		scrollToBottomRoutine = null;
	}

	private void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

	private void Unsubscribe()
	{
		Application.logMessageReceivedThreaded -= HandleLog;
	}

	private void OnDestroy()
	{
		Unsubscribe();
	}

    private void OnDisable()
    {
        Unsubscribe();
    }

	public static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
    {
        { LogType.Assert, Color.white },
        { LogType.Error, Color.red },
        { LogType.Exception, Color.red },
        { LogType.Log, Color.white },
        { LogType.Warning, Color.yellow },
    }; 
}
