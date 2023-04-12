#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidImpressionEventPublisher : IImpressionEventPublisher, IAndroidImpressionListener, IDisposable
    {
        public event EventHandler<ImpressionEventArgs> OnImpression;

        AndroidJavaClass m_ImpressionEventPublisher;
        AndroidImpressionListener m_ImpressionListener;
        volatile bool m_Disposed;
        const string k_ErrorLoadingImpressionEventPublished = "Error while loading ImpressionEventPublisher. ImpressionEventPublisher will not initialize. " +
                                                              "Please check your build settings, and make sure Mediation SDK is integrated properly.";

        public AndroidImpressionEventPublisher()
        {
            ThreadUtil.Send(state =>
            {
                try
                {
                    m_ImpressionEventPublisher = new AndroidJavaClass(NativeAndroid.Class.ImpressionEventPublisher);
                    m_ImpressionListener = new AndroidImpressionListener(this);
                    m_ImpressionEventPublisher.CallStatic(NativeAndroid.Method.Subscribe, m_ImpressionListener);
                }
                catch (Exception e)
                {
                    MediationLogger.LogError(k_ErrorLoadingImpressionEventPublished);
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
                    try
                    {
                        m_ImpressionEventPublisher?.CallStatic(NativeAndroid.Method.Unsubscribe, m_ImpressionListener);
                        m_ImpressionEventPublisher?.Dispose();
                        m_ImpressionEventPublisher = null;
                        m_ImpressionListener = null;
                    }
                    catch (Exception e)
                    {
                        MediationLogger.LogException(e);
                    }
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AndroidImpressionEventPublisher()
        {
            Dispose(false);
        }

        public void onImpression(string adUnitId, AndroidJavaObject impressionData)
        {
            ImpressionData impressData = null;
            if (impressionData != null)
            {
                impressData = new ImpressionData
                {
                    Timestamp = impressionData.Call<string>(NativeAndroid.Method.GetTimestamp),
                    AdUnitName = impressionData.Call<string>(NativeAndroid.Method.GetAdUnitName),
                    AdUnitId = impressionData.Call<string>(NativeAndroid.Method.GetAdUnitId),
                    AdUnitFormat = impressionData.Call<string>(NativeAndroid.Method.GetAdUnitFormat),
                    ImpressionId = impressionData.Call<string>(NativeAndroid.Method.GetImpressionId),
                    Currency = impressionData.Call<string>(NativeAndroid.Method.GetCurrency),
                    RevenueAccuracy = impressionData.Call<string>(NativeAndroid.Method.GetRevenueAccuracy),
                    PublisherRevenuePerImpression = impressionData.Call<double>(NativeAndroid.Method.GetPublisherRevenuePerImpression),
                    PublisherRevenuePerImpressionInMicros = impressionData.Call<Int64>(NativeAndroid.Method.GetPublishRevenuePerImpressionInMicros),
                    AdSourceName = impressionData.Call<string>(NativeAndroid.Method.GetAdSourceName),
                    AdSourceInstance = impressionData.Call<string>(NativeAndroid.Method.GetAdSourceInstance),
                    AppVersion = impressionData.Call<string>(NativeAndroid.Method.GetAppVersion),
                    LineItemId = impressionData.Call<string>(NativeAndroid.Method.GetALineItemId),
                    LineItemName = impressionData.Call<string>(NativeAndroid.Method.GetLineItemName),
                    LineItemPriority = impressionData.Call<string>(NativeAndroid.Method.GetLineItemPriority),
                    Country = impressionData.Call<string>(NativeAndroid.Method.GetCountry),
                };
            }

            OnImpression?.Invoke(null, new ImpressionEventArgs(adUnitId, impressData));
        }

        static class NativeAndroid
        {
           public static class Method
            {
                public const string GetTimestamp = "getTimestamp";
                public const string GetAdUnitName = "getAdUnitName";
                public const string GetAdUnitId = "getAdUnitId";
                public const string GetAdUnitFormat = "getAdUnitFormat";
                public const string GetImpressionId = "getImpressionId";
                public const string GetCurrency = "getCurrency";
                public const string GetRevenueAccuracy = "getRevenueAccuracy";
                public const string GetPublisherRevenuePerImpression = "getPublisherRevenuePerImpression";
                public const string GetPublishRevenuePerImpressionInMicros = "getPublisherRevenuePerImpressionInMicros";
                public const string GetAdSourceName = "getAdSourceName";
                public const string GetAdSourceInstance = "getAdSourceInstance";
                public const string GetAppVersion = "getAppVersion";
                public const string GetALineItemId = "getLineItemId";
                public const string GetLineItemName = "getLineItemName";
                public const string GetLineItemPriority = "getLineItemPriority";
                public const string GetCountry = "getCountry";
                public const string Subscribe = "subscribe";
                public const string Unsubscribe = "unsubscribe";
            }

           public static class Class
           {
               public const string ImpressionEventPublisher = "com.unity3d.mediation.ImpressionEventPublisher";
           }
        }
    }
}
#endif
