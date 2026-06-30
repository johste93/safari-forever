using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConsentManager
{
    public static void SetConsent(PrivacyConsentPurpose purpose, bool consent)
    {
        switch(purpose)
        {
            case PrivacyConsentPurpose.Analytics:

                if(consent && SaveManager.age.HasValue && SaveManager.age > 18)
                    ApproveAnalytics();
                else
                    RefuseAnalytics();

            break;
            case PrivacyConsentPurpose.Marketing:

                if(consent && SaveManager.age.HasValue && SaveManager.age > 18)
                    ApproveMarketing();
                else
                    RefuseMarketing();

            break;
        }

        if(Globals.debugConstants.verboseLogging)
            Debug.Log($"User has consented to {purpose} equals: {consent && SaveManager.age.HasValue && SaveManager.age > 18}");
    }

    public static bool HasConsent(PrivacyConsentPurpose purpose)
    {
        switch(purpose)
        {
            case PrivacyConsentPurpose.Analytics:
                return SaveManager.analyticsConsentGiven;

            case PrivacyConsentPurpose.Marketing:
                return SaveManager.marketingConsentGiven;
        }

        Debug.LogError("Purpose not found!");
        return false;
    }

    private static void ApproveAnalytics()
    {
#if ENABLE_CLOUD_SERVICES_ANALYTICS
        DataPrivacyOptInFlow.UserHasOptedIntoLimitedDataCollection_LetsResumeAnalyticsInitialization();
        SaveManager.analyticsConsentRequested = true;
        SaveManager.analyticsConsentGiven = true;
#endif
    }

    private static void RefuseAnalytics()
    {
#if ENABLE_CLOUD_SERVICES_ANALYTICS
        DataPrivacyOptInFlow.UserHasOptedOutOfAllDataCollection();
        SaveManager.analyticsConsentRequested = true;
        SaveManager.analyticsConsentGiven = false;
#endif
    }

    private static void ApproveMarketing()
    {
        SaveManager.marketingConsentRequested = true;
        SaveManager.marketingConsentGiven = true;

        //FacebookManager.EnableFacebookTracking();
    }

    private static void RefuseMarketing()
    {
        SaveManager.marketingConsentRequested = true;
        SaveManager.marketingConsentGiven = false;

        //FacebookManager.DisableFacebookTracking();
    }
}
