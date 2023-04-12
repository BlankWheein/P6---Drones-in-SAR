#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidRewardedAd : IPlatformRewardedAd, IAndroidRewardedLoadListener, IAndroidRewardedShowListener
    {
        public event EventHandler OnLoaded;
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;
        public event EventHandler OnShowed;
        public event EventHandler OnClicked;
        public event EventHandler OnClosed;
        public event EventHandler<ShowErrorEventArgs> OnFailedShow;
        public event EventHandler<RewardEventArgs> OnUserRewarded;

        const string k_CannotCallAdState = "Cannot call AdState";
        const string k_CannotCallAdUnitId = "Cannot call AdUnitId";
        const string k_CannotCallLoad = "Cannot call Load()";
        const string k_CannotCallShow = "Cannot call Show()";
        const string k_FailToLoad = "Failed to load - ";
        const string k_FailToShow = "Failed to show - ";
        const string k_ErrorLoadingRewardedAd = "Error while creating Rewarded Ad. Rewarded Ad will not load. " +
                                                "Please check your build settings, and make sure Mediation SDK is integrated properly.";
        const string k_ErrorInstanceDisposed = "Unity Mediation SDK: {0}: Instance of type {1} is disposed. " +
                                               "Please create a new instance in order to call any method.";

        /// <summary>
        /// Retrieves AdState from the Underlying Android SDK
        /// </summary>
        public AdState AdState
        {
            get
            {
                if (CheckDisposedAndLogError(k_CannotCallAdState)) return AdState.Unloaded;
                try
                {
                    using (var state = m_RewardedAd.Call<AndroidJavaObject>(NativeAndroid.Method.GetAdState))
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

        /// <summary>
        /// Retrieves Ad Unit Id from the Underlying Android SDK
        /// </summary>
        public string AdUnitId
        {
            get
            {
                if (CheckDisposedAndLogError(k_CannotCallAdUnitId)) return null;
                try
                {
                    return m_RewardedAd.Call<string>(NativeAndroid.Method.GetAdUnitId);
                }
                catch (Exception e)
                {
                    MediationLogger.LogException(e);
                    return null;
                }
            }
        }

        AndroidJavaObject m_RewardedAd;
        AndroidRewardedAdLoadListener m_RewardedAdLoadListener;
        AndroidRewardedAdShowListener m_RewardedAdShowListener;
        volatile bool m_Disposed;

        public AndroidRewardedAd(string adUnitId)
        {
            ThreadUtil.Send(state =>
            {
                try
                {
                    using (var activity = ActivityUtil.GetUnityActivity())
                    {
                        m_RewardedAd = new AndroidJavaObject(NativeAndroid.Class.RewardedAd,
                            activity, adUnitId);
                    }
                }
                catch (Exception e)
                {
                    MediationLogger.LogError(k_ErrorLoadingRewardedAd);
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
                    if (m_RewardedAdLoadListener == null)
                    {
                        m_RewardedAdLoadListener = new AndroidRewardedAdLoadListener(this);
                    }
                    m_RewardedAd.Call(NativeAndroid.Method.Load, m_RewardedAdLoadListener);
                }
                catch (Exception e)
                {
                    MediationLogger.LogException(e);
                    OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(LoadError.Unknown, $"{k_FailToLoad}{e.Message}"));
                }
            });
        }

        public void Show(RewardedAdShowOptions showOptions = null)
        {
            if (CheckDisposedAndLogError(k_CannotCallShow)) return;
            ThreadUtil.Post(state =>
            {
                try
                {
                    if (m_RewardedAdShowListener == null)
                    {
                        m_RewardedAdShowListener = new AndroidRewardedAdShowListener(this);
                    }

                    AndroidJavaObject showOptionsJava = null;
                    if (showOptions != null && !string.IsNullOrEmpty(showOptions.S2SData.UserId))
                    {
                        showOptionsJava = new AndroidJavaObject(NativeAndroid.Class.RewardedAdShowOptions);
                        var s2sData = new AndroidJavaObject(NativeAndroid.Class.RewardedAdShowOptionsS2SRedeemData,
                            showOptions.S2SData.UserId, showOptions.S2SData.CustomData);
                        showOptionsJava.Call(NativeAndroid.Method.SetS2RedeemData, s2sData);
                    }

                    m_RewardedAd.Call(NativeAndroid.Method.Show, m_RewardedAdShowListener, showOptionsJava);
                }
                catch (Exception e)
                {
                    MediationLogger.LogException(e);
                    OnFailedShow?.Invoke(this, new ShowErrorEventArgs(ShowError.Unknown, $"{k_FailToShow}{e.Message}"));
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
                    m_RewardedAd?.Dispose();
                    m_RewardedAdLoadListener = null;
                    m_RewardedAdShowListener = null;
                    m_RewardedAd = null;
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AndroidRewardedAd()
        {
            Dispose(false);
        }

        bool CheckDisposedAndLogError(string message)
        {
            if (!m_Disposed) return false;
            MediationLogger.LogError(string.Format(k_ErrorInstanceDisposed, message, GetType().FullName));
            return true;
        }

        public void onRewardedLoaded(AndroidJavaObject rewardedAd)
        {
            OnLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void onRewardedFailedLoad(AndroidJavaObject rewardedAd, AndroidJavaObject error, string msg)
        {
            OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(error.ToEnum<LoadError>(), msg));
        }

        public void onRewardedShowed(AndroidJavaObject rewardedAd)
        {
            OnShowed?.Invoke(this, EventArgs.Empty);
        }

        public void onRewardedClicked(AndroidJavaObject rewardedAd)
        {
            OnClicked?.Invoke(this, EventArgs.Empty);
        }

        public void onRewardedClosed(AndroidJavaObject rewardedAd)
        {
            OnClosed?.Invoke(this, EventArgs.Empty);
        }

        public void onRewardedFailedShow(AndroidJavaObject rewardedAd, AndroidJavaObject error, string msg)
        {
            OnFailedShow?.Invoke(this, new ShowErrorEventArgs(error.ToEnum<ShowError>(), msg));
        }

        public void onUserRewarded(AndroidJavaObject rewardedAd, AndroidJavaObject reward)
        {
            var type = reward.Call<string>(NativeAndroid.Method.GetType);
            var amount = reward.Call<string>(NativeAndroid.Method.GetAmount);
            OnUserRewarded?.Invoke(this, new RewardEventArgs(type, amount));
        }

        static class NativeAndroid
        {
            public static class Method
            {
                 public const string GetAdState = "getAdState";
                 public const string GetAdUnitId = "getAdUnitId";
                 public const string Load = "load";
                 public const string Show = "show";
                 public new const string GetType = "getType";
                 public const string GetAmount = "getAmount";
                 public const string SetS2RedeemData = "setS2SRedeemData";
            }

           public static class Class
           {
              public const string RewardedAdShowOptions = "com.unity3d.mediation.RewardedAdShowOptions";
              public const string RewardedAdShowOptionsS2SRedeemData = "com.unity3d.mediation.RewardedAdShowOptions$S2SRedeemData";
              public const string RewardedAd = "com.unity3d.mediation.RewardedAd";
           }
        }
    }
}
#endif
