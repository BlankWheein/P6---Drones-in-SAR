#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidMediationService : IPlatformMediationService, IDisposable, IAndroidInitializationListener
    {
        AndroidJavaClass m_UnityMediationClass;
        AndroidJavaClass m_InitializationConfiguration;
        AndroidInitializationListener m_InitializationListener;
        volatile bool m_Disposed;

        const string k_ErrorWhileLoadingMediationSDK = "Error while loading Mediation SDK. Mediation SDK will not initialize. " +
                                                               "Please check your build settings, and make sure Mediation SDK is integrated properly.";
        const string k_ErrorFailToInitializeNativeSdk = "Failed to initialize native SDK - ";
        const string k_InstanceDisposed = "Unity Mediation SDK: {0}: Instance of type {1} is disposed. Please create a new instance in order to call any method.";
        const string k_CannotCalInitializationState = "Cannot call InitializationState";
        const string k_CannotRetrieveSdkVersion = "Cannot retrieve Sdk Version";
        const string k_CannotCallInitiliaze = "Cannot call Initialize()";

        public AndroidMediationService()
        {
            ThreadUtil.Send(state =>
            {
                try
                {
                    m_UnityMediationClass         = new AndroidJavaClass(NativeAndroid.Class.UnityMediation);
                    m_InitializationConfiguration = new AndroidJavaClass(NativeAndroid.Class.InitializationConfiguration);
                    m_InitializationListener      = new AndroidInitializationListener(this);
                }
                catch (Exception e)
                {
                    MediationLogger.LogError(k_ErrorWhileLoadingMediationSDK);
                    MediationLogger.LogException(e);
                }
            });
        }

        void Dispose(bool disposing)
        {
            if (m_Disposed) return;
            m_Disposed = true;
            if (disposing)
            {
                //AndroidJavaObjects are created and destroyed with JNI's NewGlobalRef and DeleteGlobalRef,
                //Therefore must be used on the same attached thread. In this case, it's Unity thread.
                ThreadUtil.Post(state =>
                {
                    m_UnityMediationClass?.Dispose();
                    m_UnityMediationClass = null;
                    m_InitializationListener = null;
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AndroidMediationService()
        {
            Dispose(false);
        }

        bool CheckDisposedAndLogError(string message)
        {
            if (!m_Disposed) return false;
            MediationLogger.LogError(string.Format(k_InstanceDisposed, message, GetType().FullName));
            return true;
        }

        public event EventHandler OnInitializationComplete;
        public event EventHandler<InitializationErrorEventArgs> OnInitializationFailed;

        public InitializationState InitializationState
        {
            get
            {
                if (CheckDisposedAndLogError(k_CannotCalInitializationState)) return InitializationState.Uninitialized;
                try
                {
                    return m_UnityMediationClass
                        .CallStatic<AndroidJavaObject>(NativeAndroid.Method.GetInitializationState)
                        .ToEnum<InitializationState>();
                }
                catch (Exception e)
                {
                    MediationLogger.LogException(e);
                    return InitializationState.Uninitialized;
                }
            }
        }

        public string SdkVersion
        {
            get
            {
                if (CheckDisposedAndLogError(k_CannotRetrieveSdkVersion)) return string.Empty;
                try
                {
                    return m_UnityMediationClass.CallStatic<string>(NativeAndroid.Method.GetSdkVersion);
                }
                catch (Exception e)
                {
                    MediationLogger.LogException(e);
                    return string.Empty;
                }
            }
        }

        public void Initialize(string gameId, string installId)
        {
            if (CheckDisposedAndLogError(k_CannotCallInitiliaze)) return;
            ThreadUtil.Post(state =>
            {
                try
                {
                    var config = m_InitializationConfiguration.CallStatic<AndroidJavaObject>(NativeAndroid.Method.Builder)
                        .Call<AndroidJavaObject>(NativeAndroid.Method.SetGameId, gameId)
                        .Call<AndroidJavaObject>(NativeAndroid.Method.SetInitializationListener, m_InitializationListener)
                        .Call<AndroidJavaObject>(NativeAndroid.Method.SetOption, NativeAndroid.Parameters.InstallationId, installId)
                        .Call<AndroidJavaObject>(NativeAndroid.Method.Build);

                    m_UnityMediationClass.CallStatic(NativeAndroid.Method.Initiliaze, config);
                }
                catch (Exception e)
                {
                    MediationLogger.LogException(e);
                    var args = new InitializationErrorEventArgs(SdkInitializationError.Unknown,
                        k_ErrorFailToInitializeNativeSdk + e.Message);
                    OnInitializationFailed?.Invoke(null, args);
                }
            });
        }

        public void onInitializationComplete()
        {
            OnInitializationComplete?.Invoke(null, EventArgs.Empty);
        }

        public void onInitializationFailed(AndroidJavaObject error, string msg)
        {
            var sdkError = error.ToEnum<SdkInitializationError>();
            OnInitializationFailed?.Invoke(null, new InitializationErrorEventArgs(sdkError, msg));
        }
    }

    static class NativeAndroid
    {
        public static class Method
        {
            public const string Initiliaze = "initialize";
            public const string SetGameId = "setGameId";
            public const string SetInitializationListener = "setInitializationListener";
            public const string SetOption = "setOption";
            public const string Build = "build";
            public const string Builder = "builder";
            public const string GetSdkVersion = "getSdkVersion";
            public const string GetInitializationState = "getInitializationState";
        }

        public static class Class
        {
            public const string UnityMediation = "com.unity3d.mediation.UnityMediation";
            public const string InitializationConfiguration = "com.unity3d.mediation.InitializationConfiguration";
        }

        public static class Parameters
        {
            public const string InstallationId = "installation_id";
        }
    }
}
#endif
