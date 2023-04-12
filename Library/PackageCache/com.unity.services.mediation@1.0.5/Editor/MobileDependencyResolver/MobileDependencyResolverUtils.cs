using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MobileDependencyResolver.Utils.Editor
{
    static class MobileDependencyResolverUtils
    {
        static Type s_MobileDependencyResolverType;
        static Type s_IosResolverType;
        static Type s_CommandLineType;

        const string k_PodInstallCommand = "install";
        const string k_PodRepoUpdateCommand = "repo update";
        const string k_PodUpdateCommand = "update";
        const string k_MemberMessage = "message";
        const string k_MemberExitCode = "exitCode";
        const string k_MethodRun = "Run";
        const string k_CommandLineType = "GooglePlayServices.CommandLine, Google.JarResolver";

        public static bool IsPresent => MobileDependencyResolverType != null;

        public static void ResolveIfNeeded()
        {
            if (!AutomaticResolutionEnabled)
            {
                Resolve();
            }
        }

        public static bool AutomaticResolutionEnabled
        {
            get
            {
                var psrType = MobileDependencyResolverType;
                if (psrType == null) return false;
                var autoResolutionProperty = psrType.GetProperty("AutomaticResolutionEnabled");
                if (autoResolutionProperty == null) return false;
                return (bool)autoResolutionProperty.GetValue(null);
            }
        }

        public static bool GradleTemplateEnabled
        {
            get
            {
                var psrType = MobileDependencyResolverType;
                if (psrType == null) return false;
                var autoResolutionProperty = psrType.GetProperty("GradleTemplateEnabled");
                if (autoResolutionProperty == null) return false;
                return (bool)autoResolutionProperty.GetValue(null);
            }
        }

        public static bool MainTemplateEnabled
        {
            get
            {
                var psrType = Type.GetType("GooglePlayServices.SettingsDialog, Google.JarResolver");
                if (psrType == null) return false;
                var autoResolutionProperty = psrType.GetProperty("PatchMainTemplateGradle", BindingFlags.Static | BindingFlags.NonPublic);
                if (autoResolutionProperty == null) return false;
                return (bool)autoResolutionProperty.GetValue(null);
            }
            set
            {
                var psrType = Type.GetType("GooglePlayServices.SettingsDialog, Google.JarResolver");
                if (psrType == null) return;
                var autoResolutionProperty = psrType.GetProperty("PatchMainTemplateGradle", BindingFlags.Static | BindingFlags.NonPublic);
                if (autoResolutionProperty == null) return;
                autoResolutionProperty.SetValue(null, value);
            }
        }

        public static void Resolve()
        {
            var psrType = MobileDependencyResolverType;
            if (psrType == null) return;
            var resolveMethod = psrType.GetMethod("Resolve");
            if (resolveMethod == null) return;
            resolveMethod.Invoke(null, new object[] {Type.Missing, Type.Missing, Type.Missing});
        }

        public static void ResolveSync(bool forceResolution)
        {
            var psrType = MobileDependencyResolverType;
            if (psrType == null) return;
            var resolveMethod = psrType.GetMethod("ResolveSync");
            if (resolveMethod == null) return;
            resolveMethod.Invoke(null, new object[] {forceResolution});
        }

        public static void DeleteResolvedLibraries()
        {
            var psrType = MobileDependencyResolverType;
            if (psrType == null) return;
            var resolveMethod = psrType.GetMethod("DeleteResolvedLibrariesSync");
            if (resolveMethod == null) return;
            resolveMethod.Invoke(null, new object[] {});
        }

        public static IList<KeyValuePair<string, string>> GetPackageSpecs()
        {
            var psrType = MobileDependencyResolverType;
            if (psrType == null) return new List<KeyValuePair<string, string>>();
            var getPackageSpecsMethod = psrType.GetMethod("GetPackageSpecs");
            if (getPackageSpecsMethod == null) return new List<KeyValuePair<string, string>>();
            return (IList<KeyValuePair<string, string>>)getPackageSpecsMethod.Invoke(null, new object[] { null });
        }

        public static bool PodUpdate(string pathToBuiltProject)
        {
            return PodCommand(k_PodUpdateCommand, pathToBuiltProject);
        }

        public static bool PodInstall(string pathToBuiltProject)
        {
            return PodCommand(k_PodInstallCommand, pathToBuiltProject);
        }

        public static bool PodRepoUpdate(string pathToBuiltProject)
        {
            return PodCommand(k_PodRepoUpdateCommand, pathToBuiltProject);
        }

        static bool PodCommand(string command, string pathToBuiltProject)
        {
            var iosResolverType = IosResolverType;
            if (iosResolverType == null)
            {
                return false;
            }
            var method = IosResolverType.GetMethod("RunPodCommand", BindingFlags.Static | BindingFlags.NonPublic);
            var returnValue = method?.Invoke(obj: null, parameters: new object[] { command, pathToBuiltProject, false });
            if (returnValue != null)
            {
                var memberInfo = returnValue.GetType().GetMember(k_MemberMessage).First();
                if (memberInfo != null)
                {
                    Debug.Log(memberInfo.GetValue(returnValue));
                }
                var exitCodeMemberInfo = returnValue.GetType().GetMember(k_MemberExitCode).First();
                if (exitCodeMemberInfo != null)
                {
                    var exitCodeValue = (int)exitCodeMemberInfo.GetValue(returnValue);
                    return exitCodeValue == 0;
                }
            }

            return false;
        }

        public static void RunCommand(string tool, string command, string workingDirectory = null)
        {
            var commandLineType = CommandLineType;
            if (commandLineType == null)
            {
                return;
            }
            var method = commandLineType.GetMethod(k_MethodRun, BindingFlags.Static | BindingFlags.Public);
            var returnValue = method?.Invoke(obj: null, parameters: new object[] { tool, command, workingDirectory, null, null });
            if (returnValue != null)
            {
                var memberInfo = returnValue.GetType().GetMember(k_MemberMessage).First();
                if (memberInfo != null)
                {
                    Debug.Log(memberInfo.GetValue(returnValue));
                }
            }
        }

        static Type MobileDependencyResolverType
        {
            get
            {
                if (s_MobileDependencyResolverType != null)
                {
                    return s_MobileDependencyResolverType;
                }

                try
                {
                    s_MobileDependencyResolverType = Type.GetType("GooglePlayServices.PlayServicesResolver, Google.JarResolver");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return s_MobileDependencyResolverType;
            }
        }

        static Type IosResolverType
        {
            get
            {
                if (s_IosResolverType != null)
                {
                    return s_IosResolverType;
                }

                try
                {
                    s_IosResolverType = Type.GetType("Google.IOSResolver, Google.IOSResolver");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return s_IosResolverType;
            }
        }

        static Type CommandLineType
        {
            get
            {
                if (s_CommandLineType != null)
                {
                    return s_CommandLineType;
                }

                try
                {
                    //To check for the value below: Debug.Log(typeof(GooglePlayServices.CommandLine).AssemblyQualifiedName);
                    s_CommandLineType = Type.GetType(k_CommandLineType);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return s_CommandLineType;
            }
        }
        static object GetValue(this MemberInfo memberInfo, object forObject)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).GetValue(forObject);
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetValue(forObject);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
