#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

public class iOSBuildActions
{
    [PostProcessBuild]
	public static void OnPostProcess(BuildTarget buildTarget, string buildPath)
	{
		if(buildTarget != BuildTarget.iOS)
			return;

		AddXcodeFrameworks(buildPath);
	
		AddXcodeCapabilities(buildPath);

		ModifyPlist(buildPath);
		
	}

	private static void AddXcodeFrameworks(string buildPath)
	{
		Debug.Log("Adding XCode Frameworks!");

		//Setup!
		PBXProject pBXProject = new PBXProject();
		//string projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
		string pbxPath = PBXProject.GetPBXProjectPath(buildPath);
        var file = File.ReadAllText(pbxPath);
        pBXProject.ReadFromString(file);
		string target = pBXProject.GetUnityMainTargetGuid();

		pBXProject.AddFrameworkToProject(target, "UserNotifications.framework", false); //Notifications
		//pBXProject.AddFrameworkToProject(target, "StoreKit.framework", false); //Notifications

		//Starting with Xcode 14, bitcode is no longer required for watchOS and tvOS applications, and the App Store no longer accepts bitcode submissions from Xcode 14.
		SetBitcode(pBXProject, target, false);
        SetBitcode(pBXProject, pBXProject.TargetGuidByName(PBXProject.GetUnityTestTargetName()), false);
		SetBitcode(pBXProject, pBXProject.TargetGuidByName("GameAssembly"), false);
		SetBitcode(pBXProject, pBXProject.GetUnityFrameworkTargetGuid(), false);

		//Save Changes
		pBXProject.WriteToFile(pbxPath);
	}

	private static void AddXcodeCapabilities(string buildPath)
	{
		Debug.Log("Adding XCode Capabilities!");

		//Setup
        string pbxPath = PBXProject.GetPBXProjectPath(buildPath);
        var pCM = new ProjectCapabilityManager(pbxPath, "Unity-iPhone/Safari Forever.entitlements", "Unity-iPhone");

		//pCM.AddInAppPurchase();
		pCM.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
		pCM.AddPushNotifications(Debug.isDebugBuild);

		//Save Changes
		pCM.WriteToFile();
	}

	private static void SetBitcode(PBXProject project, string targetGUID, bool enabled) {
       project.SetBuildProperty(targetGUID, "ENABLE_BITCODE", enabled ? "YES" : "NO");
   }

	private static void ModifyPlist(string buildPath)
	{
		Debug.Log("Modifing XCode Plist!");

		//Setup
		string plistPath = buildPath + "/Info.plist";
		PlistDocument plist = new PlistDocument();
		plist.ReadFromString(File.ReadAllText(plistPath));

		PlistElementDict rootDict = plist.root;

		//Disable encryption question
		{
			rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);
		}

		//Disable Auto Init Firebase
		{
			rootDict.SetBoolean("FirebaseMessagingAutoInitEnabled", false);
		}

		//Enable filesharing.
		{
			rootDict.SetBoolean("UIFileSharingEnabled", EditorUserBuildSettings.development);
		}

        //Add languages
        {
            PlistElementArray localizations = rootDict.CreateArray("CFBundleLocalizations");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.English))
                localizations.AddString("en");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.Norwegian))
                localizations.AddString("no");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.Hebrew))
                localizations.AddString("he");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.Euro_Spanish))
                localizations.AddString("es");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.German))
                localizations.AddString("de");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.French) || Globals.localizationConstants.LanguageIsEnabled(Language.Canadian_French))
                localizations.AddString("fr");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.Greek))
                localizations.AddString("el");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.Danish))
                localizations.AddString("da");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.Dutch))
                localizations.AddString("nl");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.Finnish))
                localizations.AddString("fi");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.Italian))
                localizations.AddString("it");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.Polish))
                localizations.AddString("pl");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.Brazilian_Portuguese))
                localizations.AddString("pt-BR");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.Euro_Portuguese))
                localizations.AddString("pt");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.LatinAmerican_Spanish))
                localizations.AddString("es-MX");

            if (Globals.localizationConstants.LanguageIsEnabled(Language.Swedish))
                localizations.AddString("sv");

			if (Globals.localizationConstants.LanguageIsEnabled(Language.Russian))
				localizations.AddString("ru");

			if(Globals.localizationConstants.LanguageIsEnabled(Language.Vietnamese))
				localizations.AddString("vi");

			if(Globals.localizationConstants.LanguageIsEnabled(Language.Afrikaans))
				localizations.AddString("af");

			if(Globals.localizationConstants.LanguageIsEnabled(Language.Bulgarian))
				localizations.AddString("bg");

			if(Globals.localizationConstants.LanguageIsEnabled(Language.Estonian))
				localizations.AddString("et");

			if(Globals.localizationConstants.LanguageIsEnabled(Language.Hungarian))
				localizations.AddString("hu");

			if(Globals.localizationConstants.LanguageIsEnabled(Language.Latvian))
				localizations.AddString("lv");

			if(Globals.localizationConstants.LanguageIsEnabled(Language.Lithuanian))
				localizations.AddString("lt");

			if(Globals.localizationConstants.LanguageIsEnabled(Language.Romanian))
				localizations.AddString("ro");

			if(Globals.localizationConstants.LanguageIsEnabled(Language.Serbian))
				localizations.AddString("sr");

			if(Globals.localizationConstants.LanguageIsEnabled(Language.Turkish))
				localizations.AddString("tr");

			if(Globals.localizationConstants.LanguageIsEnabled(Language.Ukrainian))
				localizations.AddString("uk");
		}	

		//Save Changes
		File.WriteAllText(plistPath, plist.WriteToString());
	}
}
#endif