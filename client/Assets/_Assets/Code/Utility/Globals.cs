using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using System.Linq;

public class Globals : ScriptableObject
{
	private const string RESOURCE_PATH = "";
	private const string FILENAME = "Globals";

	private static string path { get { return RESOURCE_PATH + FILENAME; } }

	public static Globals instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Resources.Load<Globals>(path);

				if (!_instance)
				{
					_instance = CreateInstance<Globals>();

					Debug.LogWarning("Globals ScriptableObject not found in Resources! See: " + path);
				}
			}

			return _instance;
		}
	}
	private static Globals _instance;

	public static GameConstants gameConstants { get { return instance._gameConstants; } }
	public GameConstants _gameConstants = new GameConstants();

	[System.Serializable]
	public class GameConstants
	{
		public int targetFramerate = 60;
		public float standardTapDuration = 0.3f;

		public int blockBudget = 600;

		public int levelWidth = 15;
		public int levelHeight = 20;
		public int maxNumberOfRooms = 4;
		
		public int maxNumberOfBullets = 50;

		public ColorSpace colorSpace = ColorSpace.HSV;

		public Vector2 tileOffset = new Vector2(0.5f, 0.5f);

		public LayerMask whatIsPlayer;
		public LayerMask whatIsEnemy;

		public float transitionInDuration;
		public float transitionOutDuration;

		public int levelIdLenght = 5;
		public string latestCompatibleVersion = "0.73";

		public GameObject GetCharacterPrefab(Animal animal)
		{
			//return Resources.Load<GameObject>("Characters/Crocodile"); //temp fix for loading new controller.
			return Resources.Load<GameObject>("Animals/" + animal.ToString());
		}
	}

	public static MusicConstants musicConstants { get { return instance._musicConstants; } }
	public MusicConstants _musicConstants = new MusicConstants();

	[System.Serializable]
	public class MusicConstants
	{
		public float defaultVolume = 1f;
	}


	public static LocalizationConstants localizationConstants { get { return instance._localizationConstants; } }
	public LocalizationConstants _localizationConstants = new LocalizationConstants();

	[System.Serializable]
	public class LocalizationConstants
	{
		public Language defaultLanguage = Language.English;

		public LanguageState[] enabledLanguages;

		public bool LanguageIsEnabled(Language language)
		{
			if(language == defaultLanguage)
				return true;

			return enabledLanguages.First(x => x.language == language).enabled;
		}
	}

	public static AccessibilityConstants accessibilityConstants { get { return instance._accessibilityConstants; } }
	public AccessibilityConstants _accessibilityConstants = new AccessibilityConstants();

	[System.Serializable]
	public class AccessibilityConstants
	{
		public FontCollection standard;
		public FontCollection dyslexic;

		public FontCollection rubik;
		public FontCollection fira;

		public TMP_FontAsset GetFont(bool useDyslexic, FontType type, TMP_FontAsset previousFont, Language language)
		{
			if(type == FontType.FixedFont)
				Debug.LogError("FixedFont!");

			switch(language)
			{
				default:
				case Language.English:
					if(useDyslexic)
					{
						switch(type)
						{
							default:
							case FontType.Stylized:
							case FontType.Regular:
								return dyslexic.regular;
							case FontType.Stylized_Outlined:
							case FontType.Regular_Outlined:
								return dyslexic.regularOutlined;
						}
					}
					
					switch(type)
					{
						default:
						case FontType.Stylized:
							return standard.stylized;
						case FontType.Stylized_Outlined:
							return standard.stylizedOutlined;	
						case FontType.Regular:
							return standard.regular;
						case FontType.Regular_Outlined:
							return standard.regularOutlined;
					}
				case Language.Hebrew:
					switch(type)
					{
						default:
						case FontType.Stylized:
							return rubik.stylized;
						case FontType.Stylized_Outlined:
							return rubik.stylizedOutlined;
						case FontType.Regular:
							return rubik.regular;
						case FontType.Regular_Outlined:
							return rubik.regularOutlined;
					}
				case Language.Greek:
				case Language.Russian:
				case Language.Vietnamese:
				case Language.Bulgarian:
					switch(type)
					{
						default:
						case FontType.Stylized:
							return fira.stylized;
						case FontType.Stylized_Outlined:
							return fira.stylizedOutlined;
						case FontType.Regular:
							return fira.regular;
						case FontType.Regular_Outlined:
							return fira.regularOutlined;
					}
			}
			
		}
	}
	
	public static ClientSecret clientSecret { get { return instance._clientSecret; } }
	public ClientSecret _clientSecret = new ClientSecret();

	[System.Serializable]
	public class ClientSecret
	{
		public string salt = "";
	}

	public static WebConstants webConstants { get { return instance._webConstants; } }
	public WebConstants _webConstants = new WebConstants();

	[System.Serializable]
	public class WebConstants
	{
		public bool useLocalHost;
		public string localHost = "http://localhost:5000/";
		public string serverUrl = "https://play.safariforever.com/";

		public string GetHost()
		{
			if(useLocalHost && Application.isEditor)
				return localHost;
			else
				return serverUrl;
		}
	}

	public static GifConstants gifConstants { get { return instance._gifConstants; } }
	public GifConstants _gifConstants = new GifConstants();

	[System.Serializable]
	public class GifConstants
	{
		public bool repeats = true;
		public int minimumSystemMemorySize = 2500;
		public int minimumNumberOfProcessorCores = 4;
		
		public List<GifQualitySettings> qualitySettings;

		public GifQualitySettings GetQualitySettings(GifQuality quality)
		{
			GifQualitySettings settings = qualitySettings.Where(x => x.quality == quality).First();

			//Debug.Log(settings.ToString());

			return settings;
		}
	}

	public static IAPConstants iAPConstants { get { return instance._iAPConstants; } }
	public IAPConstants _iAPConstants = new IAPConstants();

	[System.Serializable]
	public class IAPConstants
	{
		public string[] characterIds;
	}

	public static DebugConstants debugConstants { get { return instance._debugConstants; } }
	public DebugConstants _debugConstants = new DebugConstants();

	[System.Serializable]
	public class DebugConstants
	{
		public bool verboseLogging = false;
		public bool saveLevelsPlayedAsWIP = false;
	}
}