using Unity.Services.Core.Configuration.Editor;
using UnityEditor;
using UnityEngine;
using UnityEditor.Advertisements;

namespace Unity.Services.Mediation.Build.Editor
{
    class MediationConfigurationProvider : IConfigurationProvider
    {
        public int callbackOrder { get; }
        const string k_CouldNotRetrieveGameId = "Could not retrieve gameId from Dashboard. " +
                            "Please make sure that you linked the project in the Project Settings (Window > General > Services) " +
                            "or provided it manually via {0}.";
        public void OnBuildingConfiguration(ConfigurationBuilder builder)
        {
#if UNITY_ANDROID || UNITY_IOS
            string gameId = null;
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    gameId = AdvertisementSettings.GetGameId(RuntimePlatform.Android);
                    break;

                case BuildTarget.iOS:
                    gameId = AdvertisementSettings.GetGameId(RuntimePlatform.IPhonePlayer);
                    break;
            }

            if (string.IsNullOrEmpty(gameId))
            {
                MediationLogger.LogWarning(string.Format(k_CouldNotRetrieveGameId, nameof(InitializationOptionsExtensions.SetGameId)));
            }
            else
            {
                builder.SetString(MediationServiceInitializer.keyGameId, gameId);
            }
#endif
        }
    }
}
