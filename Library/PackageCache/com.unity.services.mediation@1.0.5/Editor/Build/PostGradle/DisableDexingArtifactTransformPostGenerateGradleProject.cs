#if UNITY_ANDROID
using System;
using System.IO;
using MobileDependencyResolver.Utils.Editor;
using Unity.Services.Mediation;
using Unity.Services.Mediation.Settings.Editor;
using UnityEditor.Android;
using UnityEngine;

namespace Unity.Mediation.Build.Editor
{
    class DisableDexingArtifactTransformPostGenerateGradleProject : IPostGenerateGradleAndroidProject
    {
        const string k_GradlePropertiesFilename = "gradle.properties";
        const string k_EnableDexingArtifactTransform = "\nandroid.enableDexingArtifactTransform=false\n";
        const string k_DexingRegex = @"android.enableDexingArtifactTransform";
        const string k_AmendedLog = @" The file {0} has been amended to include {1}
to avoid exoplayer crashes. If this is not the desired result, disable this feature by unchecking
the build setting under Services > Mediation > Configure > Build Settings";

        public int callbackOrder { get; }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var disableDexingArtifactTransform = new ReloadableUserSetting<bool>(MediationSettingsProvider.instance,
                MediationUserSettingsKeys.disableDexingArtifactTransform, true);
            if (!disableDexingArtifactTransform)
            {
                // The user has turned off this feature in the settings.
                return;
            }

            // Make sure the mainTemplate.gradle is not included
            if (MobileDependencyResolverUtils.MainTemplateEnabled)
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
            var gradlePropertiesPath = Path.Combine(gradleProject, k_GradlePropertiesFilename);
            var success = FileContentAppender.AppendContentToFile(gradlePropertiesPath,
                k_EnableDexingArtifactTransform, k_DexingRegex);
            if (success)
            {
                MediationLogger.Log(string.Format(k_AmendedLog, gradlePropertiesPath, k_EnableDexingArtifactTransform));
            }
        }
    }
}
#endif
