#if UNITY_IOS || UNITY_ANDROID

using System;
using Unity.Services.Mediation.Settings.Editor;

namespace Unity.Services.Mediation.Build.Editor
{
    static class LockFileConstants
    {
        internal const string taskExplanation = @"
Deterministic Builds are currently activated.
This means that:
- When your project is built for the first time, lock files will be stored inside of your project from that build.
- On subsequent builds (where the lockfiles are still available), lock files from the previous build will be used.
This will ensure that your builds will use the same dependencies each time, making your builds deterministic.

If you would like to disable this feature, uncheck the Deterministic Builds under: Services > Mediation > Configure > Build Settings.

";
        internal const string editorFolder = "Editor";
        internal const string platformDependenciesFolder = "Platform-Dependencies";
        internal const string checksumFile = "Checksum.txt";

        internal static bool DeterministicBuildSettingEnabled()
        {
            return new ReloadableUserSetting<bool>(MediationSettingsProvider.instance,
                MediationUserSettingsKeys.deterministicBuildsKey, true);
        }
    }
}

#endif
