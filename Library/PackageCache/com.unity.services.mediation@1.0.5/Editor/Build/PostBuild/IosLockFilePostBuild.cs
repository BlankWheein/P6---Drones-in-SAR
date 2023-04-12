#if UNITY_IOS

using System.IO;
using MobileDependencyResolver.Utils.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Unity.Services.Mediation.Build.Editor
{
    static class IosLockFilePostBuild
    {
        internal const string iosFolder = "IOS";
        internal const string podfileLockFilename = "Podfile.lock";
        internal const string podfileFilename = "Podfile";
        const string k_CopyExistingLockfileLog = "Only copying Podfile.lock from {0}";
        const string k_GenerateLockfileLog = "Generating Podfile.lock, copying it to the project to location: {0}";

        static string GenerateAssetsIosLockFolderPath()
        {
            return Path.Combine(Application.dataPath, LockFileConstants.editorFolder, LockFileConstants.platformDependenciesFolder, iosFolder);
        }

        static string GenerateAssetsIosLockFilePath()
        {
            return Path.Combine(GenerateAssetsIosLockFolderPath(), podfileLockFilename);
        }

        //IOSResolver.BUILD_ORDER_INSTALL_PODS = 50 (private), so this step is 49
        [PostProcessBuild(49)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (!LockFileConstants.DeterministicBuildSettingEnabled())
            {
                //The user has turned off this feature in the settings.
                return;
            }

            MediationLogger.Log(LockFileConstants.taskExplanation);

            var assetsIosLockFolder = GenerateAssetsIosLockFolderPath();
            var assetsIosLockFile = GenerateAssetsIosLockFilePath();
            var podfileFile = Path.Combine(pathToBuiltProject, podfileFilename);

            var mainTemplateGradleHash = LockFileChecksumUtils.GetHashFromFile(podfileFile);
            var checksumFile = Path.Combine(assetsIosLockFolder, LockFileConstants.checksumFile);
            var hasValidLockfiles = LockFileChecksumUtils.HasValidChecksum(mainTemplateGradleHash, checksumFile);
            if (hasValidLockfiles && File.Exists(assetsIosLockFile))
            {
                CopyPodfileFromLockFolder(assetsIosLockFolder, pathToBuiltProject, assetsIosLockFile);
            }
            else
            {
                GeneratePodfile(assetsIosLockFolder, pathToBuiltProject, assetsIosLockFile);
                LockFileChecksumUtils.GenerateChecksumFile(mainTemplateGradleHash, checksumFile);
            }
        }

        static void CopyPodfileFromLockFolder(string assetsIosLockFolder, string pathToBuiltProject, string assetsIosLockFile)
        {
            MediationLogger.Log(string.Format(k_CopyExistingLockfileLog, assetsIosLockFolder));
            var destinationFilePath = Path.Combine(pathToBuiltProject, podfileLockFilename);
            File.Copy(assetsIosLockFile, destinationFilePath, true);
        }

        static void GeneratePodfile(string assetsIosLockFolder, string pathToBuiltProject, string assetsIosLockFile)
        {
            MediationLogger.Log(string.Format(k_GenerateLockfileLog, assetsIosLockFolder));
            try
            {
                var success = MobileDependencyResolverUtils.PodInstall(pathToBuiltProject);
                if (!success)
                {
                    MobileDependencyResolverUtils.PodRepoUpdate(pathToBuiltProject);
                    MobileDependencyResolverUtils.PodInstall(pathToBuiltProject);
                }
            }
            catch
            {
                //Failed to install, abort
                return;
            }
            CopyPodfileToLockFolder(pathToBuiltProject, assetsIosLockFolder, assetsIosLockFile);
        }

        internal static void CopyPodfileToLockFolderIfRequired(string pathToBuiltProject)
        {
            if (!LockFileConstants.DeterministicBuildSettingEnabled())
            {
                //The user has turned off this feature in the settings.
                return;
            }
            CopyPodfileToLockFolder(pathToBuiltProject, GenerateAssetsIosLockFolderPath(), GenerateAssetsIosLockFilePath());
        }

        static void CopyPodfileToLockFolder(string pathToBuiltProject, string assetsIosLockFolder, string assetsIosLockFile)
        {
            var sourceFilePath = Path.Combine(pathToBuiltProject, podfileLockFilename);
            var lockLocationDirectoryInfo = new DirectoryInfo(assetsIosLockFolder);
            if (!Directory.Exists(assetsIosLockFolder))
            {
                lockLocationDirectoryInfo.Create();
            }
            File.Copy(sourceFilePath, assetsIosLockFile, true);
        }
    }
}

#endif
