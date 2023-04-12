#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidDataPrivacy : IDataPrivacy, IDisposable
    {
        AndroidJavaClass m_DataPrivacyClass;
        volatile bool m_Disposed;

        const string k_ErrorLoadingMediationDataPrivacySDK = "Error while loading Mediation Data Privacy SDK. Mediation Data Privacy SDK will not initialize. " +
                                                             "Please check your build settings, and make sure Mediation Data Privacy SDK is integrated properly.";
        const string k_ErrorRetrievingConsentStatus = "Error while retrieving consent status.";
        const string k_ErrorSubmittingConsentStatus = "Error while submitting consent status.";
        const string k_ClassNameDataPrivacy = "com.unity3d.mediation.DataPrivacy";
        const string k_MethodNameUserGaveConsent = "userGaveConsent";
        const string k_MethodNameGetConsentStatusForLaw = "getConsentStatusForLaw";
        const string k_EnumNameConsentStatus = "com.unity3d.mediation.ConsentStatus";
        const string k_EnumNameDataPrivacyLaw =  "com.unity3d.mediation.DataPrivacyLaw";

        public AndroidDataPrivacy()
        {
            ThreadUtil.Send(state =>
            {
                try
                {
                    m_DataPrivacyClass = new AndroidJavaClass(k_ClassNameDataPrivacy);
                }
                catch (Exception e)
                {
                    MediationLogger.LogError(k_ErrorLoadingMediationDataPrivacySDK);
                    MediationLogger.LogException(e);
                }
            });
        }

        void Dispose(bool disposing)
        {
            if (m_Disposed) return;
            m_Disposed = true;
            if (disposing)
            {
                //AndroidJavaObjects are created and destroyed with JNI's NewGlobalRef and DeleteGlobalRef,
                //Therefore must be used on the same attached thread. In this case, it's Unity thread.
                ThreadUtil.Post(state =>
                {
                    m_DataPrivacyClass?.Dispose();
                    m_DataPrivacyClass = null;
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AndroidDataPrivacy()
        {
            Dispose(false);
        }

        public void UserGaveConsent(ConsentStatus consent, DataPrivacyLaw dataPrivacyLaw)
        {
            ThreadUtil.Send(state =>
            {
                try
                {
                    using (var activity = ActivityUtil.GetUnityActivity())
                    {
                        var consentJava = AndroidJavaObjectExtensions.ToAndroidEnum(k_EnumNameConsentStatus, (int)consent);
                        var lawJava = AndroidJavaObjectExtensions.ToAndroidEnum(k_EnumNameDataPrivacyLaw, (int)dataPrivacyLaw);

                        m_DataPrivacyClass.CallStatic(k_MethodNameUserGaveConsent, consentJava, lawJava, activity);
                    }
                }
                catch (Exception e)
                {
                    MediationLogger.LogError(k_ErrorSubmittingConsentStatus);
                    MediationLogger.LogException(e);
                }
            });
        }

        public ConsentStatus GetConsentStatusForLaw(DataPrivacyLaw dataPrivacyLaw)
        {
            try
            {
                using (var activity = ActivityUtil.GetUnityActivity())
                {
                    var lawJava =  AndroidJavaObjectExtensions.ToAndroidEnum(k_EnumNameDataPrivacyLaw, (int)dataPrivacyLaw);
                    return m_DataPrivacyClass.CallStatic<AndroidJavaObject>(
                        k_MethodNameGetConsentStatusForLaw,
                        lawJava,
                        activity).ToEnum<ConsentStatus>();
                }
            }
            catch (Exception e)
            {
                MediationLogger.LogError(k_ErrorRetrievingConsentStatus);
                MediationLogger.LogException(e);
                return ConsentStatus.NotDetermined;
            }
        }
    }
}
#endif
