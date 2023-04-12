#if UNITY_ANDROID || UNITY_IOS
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Unity.Services.Mediation.Adapters.Editor;

namespace Unity.Services.Mediation.Build.Editor
{
    class NoAdapterPreBuildCheck : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        const string k_NoAdaptersDetected = "No Adapters Detected";
        const string k_DisplayDialogMessage = "Go to Project Settings -> Mediation to add an adapter\nDo you want to cancel the build?";
        const string k_CancelBuild = "Cancel build";
        const string k_Ignore = "Ignore";
        const string k_BuildCanceled = "Build canceled";

        public void OnPreprocessBuild(BuildReport report)
        {
            if (MediationSdkInfo.GetInstalledAdapters().Count == 0)
            {
                MediationLogger.LogWarning(k_NoAdaptersDetected);
                if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
                {
                    //This check is currently not ran in batch mode
                    return;
                }

                if (EditorUtility.DisplayDialog(k_NoAdaptersDetected, k_DisplayDialogMessage, k_CancelBuild, k_Ignore))
                {
                    throw new BuildFailedException(k_BuildCanceled);
                }
            }
        }
    }
}
#endif
