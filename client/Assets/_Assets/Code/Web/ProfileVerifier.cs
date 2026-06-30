using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileVerifier
{
    private static bool hasAgreedToTermsOfService;
    private static bool hasAgreedToPrivacyPolicy;

    public static void VerifyProfile(Profile profile, System.Action<bool> OnComplete, bool failSilently = false)
    {
        if(!failSilently)
            DialogCanvas.instance.ShowLoading();

        VerifyTermsOfServiceAgreement(profile, failSilently, (agreesToTermsOfService)=>{

            if(!agreesToTermsOfService)
            {
                if(!failSilently)
                    DialogCanvas.instance.HideLoading();

                OnComplete(false);
                return;
            }

            VerifyPrivacyPolicyAgreement(profile, failSilently, (agreesToPrivacyPolicy)=>{
               
                
                if(!agreesToPrivacyPolicy)
                {
                    if(!failSilently)
                        DialogCanvas.instance.HideLoading();

                    OnComplete(false);
                    return;
                }

                OnComplete(true);
            });
        });
    }

    private static void VerifyTermsOfServiceAgreement(Profile profile, bool failSilently, System.Action<bool> OnComplete)
    {
        if(!hasAgreedToTermsOfService)
        {
            UserAPI.FetchTermsOfServiceStatus(profile, failSilently, (termsSuccess, hasAgreed)=>
            {
                if(!termsSuccess)
                {
                    OnComplete(false);
                    return;
                }

                hasAgreedToTermsOfService = hasAgreed;
                if(hasAgreedToTermsOfService)
                {
                    OnComplete(true);
                    return;
                }
                else
                {
                    if(failSilently)
                    {
                        Debug.LogWarning("Failed Silently: User has not agreed to ToS");
                        OnComplete(false);
                        return;
                    }
                    
                    DialogCanvas.instance.ShowTermsOfService((accepts)=>
                    {
                        if(!accepts)
                        {
                            Debug.LogError("User did not accept");
                            return;
                        }

                        UserAPI.UpdateTermsOfServiceStatus(profile, accepts, (success)=>
                        {
                            if(!success)
                            {
                                Debug.LogError("Unable to update terms of service agreement");
                                return;
                            }

                            hasAgreedToTermsOfService = accepts;
                            OnComplete(accepts);
                            return;
                        });
                    });
                }
            });
        }
        else
        {
            OnComplete(true);
        }
    }

    private static void VerifyPrivacyPolicyAgreement(Profile profile, bool failSilently, System.Action<bool> OnComplete)
    {
        if(!hasAgreedToPrivacyPolicy)
        {
            UserAPI.FetchPrivacyPolicyStatus(profile, failSilently, (termsSuccess, hasAgreed)=>
            {
                if(!termsSuccess)
                {
                    OnComplete(false);
                    return;
                }

                hasAgreedToPrivacyPolicy = hasAgreed;
                if(hasAgreedToPrivacyPolicy)
                {
                    OnComplete(true);
                    return;
                }
                else
                {
                    if(failSilently)
                    {
                        Debug.LogWarning("Failed Silently: User has not agreed to Privacy Policy");
                        OnComplete(false);
                        return;
                    }
                    
                    DialogCanvas.instance.ShowPrivacyPolicy((accepts)=>
                    {
                        if(!accepts)
                        {
                            Debug.LogError("User did not accept");
                            return;
                        }

                        UserAPI.UpdatePrivacyPolicyStatus(profile, accepts, (success)=>
                        {
                            if(!success)
                            {
                                Debug.LogError("Unable to update privacy policy agreement");
                                return;
                            }

                            hasAgreedToPrivacyPolicy = accepts;
                            OnComplete(accepts);
                            return;
                        });
                    });
                }
            });
        }
        else
        {
            OnComplete(true);
        }
    }
}