#if UNITY_ANDROID
using System;
using System.IO;
using MobileDependencyResolver.Utils.Editor;
using Unity.Services.Mediation;
using Unity.Services.Mediation.Build.Editor;
using Unity.Services.Mediation.Settings.Editor;
using UnityEditor.Android;
using UnityEngine;

namespace Unity.Mediation.Build.Editor
{
    class AndroidUpdateDependenciesPostGenerateGradleProject : IPostGenerateGradleAndroidProject
    {
        const string k_BuildGradleFilename = "build.gradle";
        const string k_SettingsToBeInserted = @"
configurations.all {
    resolutionStrategy.cacheChangingModulesFor 0, 'seconds'
}
";
        const string k_Regex = @"resolutionStrategy.cacheChangingModulesFor";
        const string k_AmendedLog = @" The file {0} has been amended to include {1}
to update the grade dependencies to the latest versions available online. If this is not the desired result, disable this feature by unchecking
the build setting under Services > Mediation > Configure > Build Settings";

        public int callbackOrder { get; }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var enabledSetting = new ReloadableUserSetting<bool>(MediationSettingsProvider.instance,
                MediationUserSettingsKeys.forceDependencyUpdateKey, true);
            if (!enabledSetting)
            {
                // The user has turned off this feature in the settings.
                return;
            }

            // Make sure the mainTemplate.gradle is not included
            if (!MobileDependencyResolverUtils.GradleTemplateEnabled)
            {
                // In this case, builds will work fine.
                return;
            }

            var gradleProject = Path.GetDirectoryName(path);
            if (gradleProject is null)
            {
                // Something went wrong, abort.
                return;
            }
            MediationLogger.Log(UpdateDependenciesConstants.taskExplanation);

            var buildGradlePath = Path.Combine(gradleProject, k_BuildGradleFilename);
            var success = FileContentAppender.AppendContentToFile(buildGradlePath,
                k_SettingsToBeInserted, k_Regex);
            if (success)
            {
                MediationLogger.Log(string.Format(k_AmendedLog, buildGradlePath, k_SettingsToBeInserted));
            }

            var lockFilePostGenerateGradleProject = new AndroidLockFilePostGenerateGradleProject();
            lockFilePostGenerateGradleProject.GenerateAndCopyLockFilesIfRequired(path);
        }
    }
}
#endif
