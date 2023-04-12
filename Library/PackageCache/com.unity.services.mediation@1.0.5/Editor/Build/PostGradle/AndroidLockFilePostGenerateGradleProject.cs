#if UNITY_ANDROID

using System;
using System.IO;
using System.Linq;
using System.Threading;
using MobileDependencyResolver.Utils.Editor;
using Unity.Services.Mediation;
using Unity.Services.Mediation.Build.Editor;
using Unity.Services.Mediation.Settings.Editor;
using UnityEngine;
using UnityEditor.Android;

namespace Unity.Mediation.Build.Editor
{
    class AndroidLockFilePostGenerateGradleProject : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder { get; }

        internal const string androidFolder = "Android";
        const string k_LauncherFolder = "launcher";
        const string k_GradleFolder = "gradle";
        const string k_LibFolder = "lib";
        const string k_BinFolder = "bin";
        const string k_Java = "java";
        const string k_GradleGenerateLocksCommand = "dependencies --write-locks";
        const string k_JarCommand = "-jar";
        const string k_DependencyLocksFolder = "dependency-locks";
        const string k_LauncherBuildGradleFilename = "build.gradle";
        const string k_DependencyLockingPattern = @"dependencyLocking\s*?{";
        internal const string lockFilePattern = "*.lockfile";
        const string k_GradleJarPattern = "gradle-launcher-*.jar";
        const string k_MainTemplateGradleFile = "mainTemplate.gradle";
        const string k_PluginsFolder = "Plugins";
        const string k_CopyExistingLockfilesLog = "Only copying gradle lock files, no lockfile generation";
        const string k_GenerateLockfilesLog = "Generating lock files, copying them to the project to location: {0}";
        const string k_NoGradleTemplate = @"No Custom Main Gradle Template, no lock files will be generated.

Usage of the Custom Main Gradle Template file is recommended. Include it by ticking the checkbox here:
Edit > Project Settings > Player > Android > Publishing Settings > Custom Main Gradle Template
";
        const string k_DependencyLockAddition = @"

dependencyLocking {
    lockAllConfigurations()
}
";

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var deterministicBuildsSetting = new ReloadableUserSetting<bool>(MediationSettingsProvider.instance,
                MediationUserSettingsKeys.deterministicBuildsKey, true);
            if (!deterministicBuildsSetting)
            {
                //The user has turned off this feature in the settings.
                return;
            }

            MediationLogger.Log(LockFileConstants.taskExplanation);

            //We support v5.1.1 and above (2019), which supports dependency locking
            //Make sure the mainTemplate.gradle is actually included
            if (!MobileDependencyResolverUtils.MainTemplateEnabled)
            {
                //In this case, developers should store their aars however they wish,
                //since having the aars sourced locally + using locks fails resolution.
                MediationLogger.Log(k_NoGradleTemplate);
                return;
            }

            var assetsAndroidLockFolder = GenerateAssetsAndroidLockFolder();
            var gradleLauncherProject = GenerateGradleLauncherProjectPath(path);
            var gradleLauncherProjectLockFilesLocation = GenerateGradleLauncherProjectLockfilesPath(gradleLauncherProject);

            AddLockSettingsToBuildGradleFile(gradleLauncherProject);
            var mainTemplateGradleHash = LockFileChecksumUtils.GetHashFromFile(Path.Combine(Application.dataPath, k_PluginsFolder, androidFolder, k_MainTemplateGradleFile));
            var checksumFile = Path.Combine(assetsAndroidLockFolder, LockFileConstants.checksumFile);
            var hasValidLockfiles = LockFileChecksumUtils.HasValidChecksum(mainTemplateGradleHash, checksumFile);

            if (hasValidLockfiles && LockfilesExist(assetsAndroidLockFolder))
            {
                CopyExistingLockFiles(gradleLauncherProjectLockFilesLocation, assetsAndroidLockFolder);
            }
            else
            {
                GenerateAndCopyLockFiles(gradleLauncherProjectLockFilesLocation, assetsAndroidLockFolder, gradleLauncherProject);
                LockFileChecksumUtils.GenerateChecksumFile(mainTemplateGradleHash, checksumFile);
            }
        }

        string GenerateGradleLauncherProjectPath(string path)
        {
            var gradleProject = Path.GetDirectoryName(path);
            if (gradleProject is null)
            {
                //Something went wrong, abort.
                return "";
            }

            return Path.Combine(Path.GetDirectoryName(path) ?? "", k_LauncherFolder);
        }

        string GenerateGradleLauncherProjectLockfilesPath(string gradleLauncherProject)
        {
            return Path.Combine(gradleLauncherProject, k_GradleFolder, k_DependencyLocksFolder);
        }

        string GenerateAssetsAndroidLockFolder()
        {
            return Path.Combine(Application.dataPath, LockFileConstants.editorFolder, LockFileConstants.platformDependenciesFolder, androidFolder);
        }

        bool LockfilesExist(string assetsAndroidLockFolder)
        {
            if (Directory.Exists(assetsAndroidLockFolder))
            {
                try
                {
                    var lockfiles = Directory.GetFiles(assetsAndroidLockFolder, lockFilePattern);
                    return lockfiles.Length > 0;
                }
                catch (Exception)
                {
                    //Unable to fetch lockfiles, abort
                }
            }

            return false;
        }

        void AddLockSettingsToBuildGradleFile(string gradleLauncherProject)
        {
            //Add lock info into launcher gradle file
            var launcherBuildGradleFile = Path.Combine(gradleLauncherProject, k_LauncherBuildGradleFilename);
            FileContentAppender.AppendContentToFile(launcherBuildGradleFile, k_DependencyLockAddition, k_DependencyLockingPattern);
        }

        void CopyExistingLockFiles(string gradleLauncherProjectLockFilesLocation, string assetsAndroidLockFolder)
        {
            MediationLogger.Log(k_CopyExistingLockfilesLog);

            var lockLocationDirectoryInfo = new DirectoryInfo(gradleLauncherProjectLockFilesLocation);
            DeleteDirectoryPath(gradleLauncherProjectLockFilesLocation);
            lockLocationDirectoryInfo.Create();
            CopyLockFiles(assetsAndroidLockFolder, gradleLauncherProjectLockFilesLocation);
        }

        void GenerateAndCopyLockFiles(string gradleLauncherProjectLockFilesLocation, string assetsAndroidLockFolder, string gradleLauncherProject)
        {
            MediationLogger.Log(string.Format(k_GenerateLockfilesLog, assetsAndroidLockFolder));
            RunGradleLockfileCommand(gradleLauncherProject);
            var storedLockLocationDirectoryInfo = new DirectoryInfo(assetsAndroidLockFolder);
            DeleteDirectoryPath(assetsAndroidLockFolder);
            storedLockLocationDirectoryInfo.Create();
            CopyLockFiles(gradleLauncherProjectLockFilesLocation, assetsAndroidLockFolder);
        }

        internal void GenerateAndCopyLockFilesIfRequired(string path)
        {
            var assetsAndroidLockFolder = GenerateAssetsAndroidLockFolder();
            var gradleLauncherProject = GenerateGradleLauncherProjectPath(path);
            var gradleLauncherProjectLockFilesLocation = GenerateGradleLauncherProjectLockfilesPath(gradleLauncherProject);
            if (!LockFileConstants.DeterministicBuildSettingEnabled())
            {
                //The user has turned off this feature in the settings.
                return;
            }
            GenerateAndCopyLockFiles(gradleLauncherProjectLockFilesLocation, assetsAndroidLockFolder, gradleLauncherProject);
        }

        void DeleteDirectoryPath(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                var directoryInfo = new DirectoryInfo(directoryPath);
                try
                {
                    directoryInfo.Delete(true);
                }
                catch (Exception)
                {
                    //Failed to delete, abort
                    return;
                }

                //On Windows machines the Delete function could be asynchronous, the following code is to confirm it has surely been deleted
                var retryCount = 0;
                var retryLimit = 10;
                while (Directory.Exists(directoryPath))
                {
                    Thread.Sleep(100);
                    retryCount++;
                    if (retryCount >= retryLimit)
                    {
                        return;
                    }
                }
            }
        }

        void RunGradleLockfileCommand(string gradleLauncherProject)
        {
            var potentialGradleJarLocations = Directory.GetFiles(Path.Combine(AndroidExternalToolsSettings.gradlePath, k_LibFolder), k_GradleJarPattern);
            if (potentialGradleJarLocations.Length == 0)
            {
                //Gradle is not here, abort
                return;
            }
            var gradleJarLocation = potentialGradleJarLocations.First();
            var javaLocation = Path.Combine(AndroidExternalToolsSettings.jdkRootPath, k_BinFolder, k_Java);
            try
            {
                MobileDependencyResolverUtils.RunCommand(javaLocation, $"{k_JarCommand} \"{gradleJarLocation}\" {k_GradleGenerateLocksCommand}", gradleLauncherProject);
            }
            catch
            {
                //The command failed, abort
            }

        }

        void CopyLockFiles(string source, string destination)
        {
            var lockfiles = Directory.GetFiles(source, lockFilePattern);
            foreach (var lockfile in lockfiles)
            {
                var fileName = Path.GetFileName(lockfile);
                var destinationFilePath = Path.Combine(destination, fileName);
                File.Copy(lockfile, destinationFilePath, true);
            }
        }
    }
}

#endif
