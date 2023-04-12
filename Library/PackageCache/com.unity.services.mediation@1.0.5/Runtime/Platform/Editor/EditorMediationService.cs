#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class EditorMediationService : IPlatformMediationService
    {
#pragma warning disable 67
        public event EventHandler OnInitializationComplete;

        public event EventHandler<InitializationErrorEventArgs> OnInitializationFailed;
#pragma warning restore 67

        public InitializationState InitializationState { get; private set; } = InitializationState.Uninitialized;
        public string SdkVersion => "0.0.0";

        const string k_BuildPlatformNotSupported = "The selected build platform is not supported by Mediation. Build Target: {0}. Using Temporary GameId.";
        const string k_GameIdEmpty = "Game Id was Empty.";
        const string k_TemporaryGameId = "EDITOR";

        public EditorMediationService()
        {
        }

        public void Initialize(string gameId, string installId)
        {
            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            if (activeBuildTarget != BuildTarget.Android && activeBuildTarget != BuildTarget.iOS)
            {
                MediationLogger.LogWarning(string.Format(k_BuildPlatformNotSupported,activeBuildTarget.ToString()));
                gameId = k_TemporaryGameId;
            }

            if (!string.IsNullOrEmpty(gameId))
            {
                InitializationState = InitializationState.Initialized;
                OnInitializationComplete?.Invoke(null, EventArgs.Empty);
            }
            else
            {
                InitializationState = InitializationState.Uninitialized;
                OnInitializationFailed?.Invoke(null, new InitializationErrorEventArgs(SdkInitializationError.Unknown, k_GameIdEmpty));
            }
        }
    }
}
#endif
