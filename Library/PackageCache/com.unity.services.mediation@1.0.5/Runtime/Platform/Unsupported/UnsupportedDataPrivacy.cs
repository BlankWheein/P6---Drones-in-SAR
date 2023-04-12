#if !UNITY_ANDROID && !UNITY_IOS

namespace Unity.Services.Mediation.Platform
{
    class UnsupportedDataPrivacy : IDataPrivacy
    {
        const string k_UnsupportedWarning = "{0}: Unity Mediation is not supported on this platform";

        public void UserGaveConsent(ConsentStatus consent, DataPrivacyLaw dataPrivacyLaw)
        {
            MediationLogger.LogWarning(string.Format(k_UnsupportedWarning, System.Reflection.MethodBase.GetCurrentMethod().Name));
        }

        public ConsentStatus GetConsentStatusForLaw(DataPrivacyLaw dataPrivacyLaw)
        {
            MediationLogger.LogWarning(string.Format(k_UnsupportedWarning, System.Reflection.MethodBase.GetCurrentMethod().Name));
            return ConsentStatus.NotDetermined;
        }
    }
}
#endif
