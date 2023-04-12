#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidInterstitialAd : IPlatformInterstitialAd, IAndroidInterstitialLoadListener, IAndroidInterstitialShowListener
    {
        public event EventHandler OnLoaded;
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;
        public event EventHandler OnShowed;
        public event EventHandler OnClicked;
        public event EventHandler OnClosed;
        public event EventHandler<ShowErrorEventArgs> OnFailedShow;

        const string k_CannotCallAdState = "Cannot call AdState";
        const string k_CannotCallAdUnitId = "Cannot call AdUnitId";
        const string k_CannotCallLoad = "Cannot call Load()";
        const string k_CannotCallShow = "Cannot call Show()";
        const string k_FailToLoad = "Failed to load - ";
        const string k_FailToShow = "Failed to show - ";
        const string k_GetAdState = "getAdState";
        const string k_GetAdUnitId = "getAdUnitId";
        const string k_MethodLoadName = "load";
        const string k_ShowMethodName = "show";

        const string k_ErrorInterstitialLoad = "Error while creating Interstitial Ad. Interstitial Ad will not load. " +
                                               "Please check your build settings, and make sure Mediation SDK is integrated properly.";
        const string k_ErrorInstanceDisposed = "Unity Mediation SDK: {0}: Instance of type {1} is disposed. " +
                                               "Please create a new instance in order to call any method.";

        public AdState AdState
        {
            get
            {
                if (CheckDisposedAndLogError(k_CannotCallAdState)) return AdState.Unloaded;
                try
                {
                    using (var state = m_InterstitialAd.Call<AndroidJavaObject>(k_GetAdState))
                    {
                        return state.ToEnum<AdState>();
                    }
                }
                catch (Exception e)
                {
                    MediationLogger.LogException(e);
                    return AdState.Unloaded;
                }
            }
        }

        public string AdUnitId
        {
            get
            {
                if (CheckDisposedAndLogError(k_CannotCallAdUnitId)) return null;
                try
                {
                    return m_InterstitialAd.Call<string>(k_GetAdUnitId);
                }
                catch (Exception e)
                {
                    MediationLogger.LogException(e);
                    return null;
                }
            }
        }

        AndroidJavaObject m_InterstitialAd;
        AndroidInterstitialAdLoadListener m_InterstitialAdLoadListener;
        AndroidInterstitialAdShowListener m_InterstitialAdShowListener;
        volatile bool m_Disposed;

        public AndroidInterstitialAd(string adUnitId)
        {
            ThreadUtil.Send(state =>
            {
                try
                {
                    using (var activity = ActivityUtil.GetUnityActivity())
                    {
                        m_InterstitialAd = new AndroidJavaObject("com.unity3d.mediation.InterstitialAd",
                            activity, adUnitId);
                    }
                }
                catch (Exception e)
                {
                    MediationLogger.LogError(k_ErrorInterstitialLoad);
                    MediationLogger.LogException(e);
                }
            });
        }

        public void Load()
        {
            if (CheckDisposedAndLogError(k_CannotCallLoad)) return;

            ThreadUtil.Post(state =>
            {
                try
                {
                    if (m_InterstitialAdLoadListener == null)
                    {
                        m_InterstitialAdLoadListener = new AndroidInterstitialAdLoadListener(this);
                    }

                    m_InterstitialAd.Call(k_MethodLoadName, m_InterstitialAdLoadListener);
                }
                catch (Exception e)
                {
                    MediationLogger.LogException(e);
                    OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(LoadError.Unknown, k_FailToLoad + e.Message));
                }
            });
        }

        public void Show()
        {
            if (CheckDisposedAndLogError(k_CannotCallShow)) return;

            ThreadUtil.Post(state =>
            {
                try
                {
                    if (m_InterstitialAdShowListener == null)
                    {
                        m_InterstitialAdShowListener = new AndroidInterstitialAdShowListener(this);
                    }

                    m_InterstitialAd.Call(k_ShowMethodName, m_InterstitialAdShowListener);
                }
                catch (Exception e)
                {
                    MediationLogger.LogException(e);
                    OnFailedShow?.Invoke(this, new ShowErrorEventArgs(ShowError.Unknown, k_FailToShow + e.Message));
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
                    m_InterstitialAd?.Dispose();
                    m_InterstitialAdLoadListener = null;
                    m_InterstitialAdShowListener = null;
                    m_InterstitialAd = null;
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AndroidInterstitialAd()
        {
            Dispose(false);
        }

        bool CheckDisposedAndLogError(string message)
        {
            if (!m_Disposed) return false;
            MediationLogger.LogError(string.Format(k_ErrorInstanceDisposed, message, GetType().FullName));
            return true;
        }

        public void onInterstitialLoaded(AndroidJavaObject interstitialAd)
        {
            OnLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void onInterstitialFailedLoad(AndroidJavaObject interstitialAd, AndroidJavaObject error, string msg)
        {
            OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(error.ToEnum<LoadError>(), msg));
        }

        public void onInterstitialShowed(AndroidJavaObject interstitialAd)
        {
            OnShowed?.Invoke(this, EventArgs.Empty);
        }

        public void onInterstitialClicked(AndroidJavaObject interstitialAd)
        {
            OnClicked?.Invoke(this, EventArgs.Empty);
        }

        public void onInterstitialClosed(AndroidJavaObject interstitialAd)
        {
            OnClosed?.Invoke(this, EventArgs.Empty);
        }

        public void onInterstitialFailedShow(AndroidJavaObject interstitialAd, AndroidJavaObject error, string msg)
        {
            OnFailedShow?.Invoke(this, new ShowErrorEventArgs(error.ToEnum<ShowError>(), msg));
        }
    }
}
#endif
