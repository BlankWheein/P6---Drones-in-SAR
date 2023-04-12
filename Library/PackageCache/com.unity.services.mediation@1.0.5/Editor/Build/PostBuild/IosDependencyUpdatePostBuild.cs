#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using MobileDependencyResolver.Utils.Editor;
using Unity.Services.Mediation.Settings.Editor;

namespace Unity.Services.Mediation.Build.Editor
{
    static class IosDependencyUpdatePostBuild
    {
        //IOSResolver.BUILD_ORDER_INSTALL_PODS = 50 (private), so this step is 51
        [PostProcessBuild(51)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            var forceUpdateDependenciesSetting = new ReloadableUserSetting<bool>(MediationSettingsProvider.instance,
                MediationUserSettingsKeys.forceDependencyUpdateKey, false);
            if (!forceUpdateDependenciesSetting)
            {
                //The user has turned off this feature in the settings.
                return;
            }
            MediationLogger.Log(UpdateDependenciesConstants.taskExplanation);
            MobileDependencyResolverUtils.PodUpdate(pathToBuiltProject);
            IosLockFilePostBuild.CopyPodfileToLockFolderIfRequired(pathToBuiltProject);
        }
    }
}

#endif
