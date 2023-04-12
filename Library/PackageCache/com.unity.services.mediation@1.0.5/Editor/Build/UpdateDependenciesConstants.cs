#if UNITY_IOS || UNITY_ANDROID

using System;

namespace Unity.Services.Mediation.Build.Editor
{
    static class UpdateDependenciesConstants
    {
        internal const string taskExplanation = @"
Forced Update of Native Dependencies is currently activated.
This means that:
- When your project is built, lock files stored will be deleted.
- For iOS, pod update will be called.
- For Android, resolutionStrategy.cacheChangingModulesFor will be set to 0.

If you would like to disable this feature, uncheck the Force Future Builds to Include the Newest Valid Native Dependencies under: Services > Mediation > Configure > Build Settings.
";
    }
}

#endif
