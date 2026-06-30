using System;
using UnityEngine;
using UnityEngine.Analytics;

public class DataPrivacyOptInFlow
{
#if ENABLE_CLOUD_SERVICES_ANALYTICS
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitStepOne()
    {
        Analytics.initializeOnStartup = false;
    }

    // Call this when the user has given permission to collect data
    public static void UserHasOptedIntoDataCollection_LetsResumeAnalyticsInitialization()
    {
        Analytics.ResumeInitialization();
    }

    // Call this when the user has given limited data collection permission
    public static void UserHasOptedIntoLimitedDataCollection_LetsResumeAnalyticsInitialization()
    {
        Analytics.limitUserTracking = true;
        Analytics.ResumeInitialization();
    }

    // Call this when the user doesn't want any data collection.
    public static void UserHasOptedOutOfAllDataCollection()
    {
        // Don't call ResumeInitialization
        // But disable Analytics so the code knows to shutdown
        DisableAnalyticsCompletely();
    }

    // If you want to disable Analytics completely during runtime
    private static void DisableAnalyticsCompletely()
    {
        Analytics.enabled = false;
        Analytics.deviceStatsEnabled = false;
        PerformanceReporting.enabled = false;
    }
#endif
}