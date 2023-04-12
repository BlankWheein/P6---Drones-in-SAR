#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Unity.Services.Mediation.Platform
{
    class MockInterstitial : MonoBehaviour, IPlatformInterstitialAd
    {
        [Header("Properties")]
        public float AdTime = 5f;
        public bool DisplayConsoleMessages = false;
        public bool DisplayMockAd = true;
        public bool DisplaySlideAnimation = true;
        [Range(0f, 10f)]
        public float MockLoadTimeInSeconds = 0f;
        [Range(0f, 1f)]
        public float MockNoFillChance = 0f;

        [Header("References")]
        public Button CloseButton;
        public Button SkipButton;
        public Image ProgressBar;
        public Canvas Canvas;
        public RectTransform CanvasPanel;
        public EventSystem EventSystem;

        Coroutine m_AdCoroutine;

        public AdState AdState { get; private set; }
        public string AdUnitId { get; internal set; }

        public event EventHandler OnLoaded;
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;
        public event EventHandler OnShowed;
        public event EventHandler OnClicked;
        public event EventHandler OnClosed;
        public event EventHandler<ShowErrorEventArgs> OnFailedShow;

        const string k_UnityMediationNotInitiliazed = "Unity Mediation not Initialized.";
        const string k_AdUnitIdEmpty = "Ad Unit Id is Empty.";
        const string k_AdUnitFailedToFill = "Ad Unit failed to fill.";
        const string k_AdNotLoaded = "Ad Not Loaded.";
        const string k_InterstitialLoad = "Interstitial Loaded";
        const string k_InterstitialFailToLoad = "Interstitial Fail to Load:";
        const string k_InterstitialShowed = "Interstitial Showed";
        const string k_InterstitialFailToShow = "Interstitial Fail to Show:";
        const string k_InterstitialClicked = "Interstitial Clicked";
        const string k_InterstitialClosed = "Interstitial Closed";

        void Awake()
        {
            Canvas.enabled = false;

            if (EventSystem.current == null)
            {
                EventSystem.gameObject.SetActive(true);
                EventSystem.current = EventSystem;
            }

            if (DisplayConsoleMessages)
            {
                InitializeConsoleCallbacks();
            }
        }

        void InitializeConsoleCallbacks()
        {
            OnLoaded += (sender, args) => MediationLogger.Log(k_InterstitialLoad);
            OnFailedLoad += (sender, args) => MediationLogger.LogError(k_InterstitialFailToLoad + args.Message);

            OnShowed += (sender, args) => MediationLogger.Log(k_InterstitialShowed);
            OnFailedShow += (sender, args) => MediationLogger.LogError(k_InterstitialFailToShow + args.Message);

            OnClicked += (sender, args) => MediationLogger.Log(k_InterstitialClicked);
            OnClosed += (sender, args) => MediationLogger.Log(k_InterstitialClosed);
        }

        public void Load()
        {
            if (MediationService.InitializationState != InitializationState.Initialized)
            {
                OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(LoadError.SdkNotInitialized, k_UnityMediationNotInitiliazed));
            }
            else if (string.IsNullOrEmpty(AdUnitId))
            {
                OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(LoadError.Unknown, k_AdUnitIdEmpty));
            }
            else
            {
                StartCoroutine(AttemptLoad());
            }
        }

        IEnumerator AttemptLoad()
        {
            yield return new WaitForSeconds(MockLoadTimeInSeconds);

            if (Random.value < MockNoFillChance)
            {
                OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(LoadError.NoFill, k_AdUnitFailedToFill));
            }
            else
            {
                AdState = AdState.Loaded;
                OnLoaded?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Show()
        {
            if (AdState == AdState.Loaded)
            {
                if (DisplayMockAd)
                {
                    ShowAd();
                }

                OnShowed?.Invoke(this, EventArgs.Empty);
                AdState = AdState.Showing;
            }
            else
            {
                OnFailedShow?.Invoke(this, new ShowErrorEventArgs(ShowError.AdNotLoaded, k_AdNotLoaded));
            }
        }

        void ShowAd()
        {
            Canvas.enabled = true;
            m_AdCoroutine = StartCoroutine(StartAd());
        }

        IEnumerator StartAd()
        {
            ProgressBar.fillAmount = 0;
            SkipButton.gameObject.SetActive(true);
            CloseButton.gameObject.SetActive(false);

            if (DisplaySlideAnimation)
            {
                CanvasPanel.offsetMax = new Vector2(CanvasPanel.offsetMax.x, -Screen.height);
                CanvasPanel.offsetMin = new Vector2(CanvasPanel.offsetMin.x, -Screen.height);
            }

            float timer = 0;
            while (timer < AdTime)
            {
                var interpolatedTopValue = Mathf.Lerp(CanvasPanel.offsetMax.y, 0, 0.075f);
                CanvasPanel.offsetMax = new Vector2(CanvasPanel.offsetMax.x, interpolatedTopValue);
                CanvasPanel.offsetMin = new Vector2(CanvasPanel.offsetMin.x, interpolatedTopValue);

                ProgressBar.fillAmount = timer / AdTime;

                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            EndAd();
        }

        public void EndAd()
        {
            StopCoroutine(m_AdCoroutine);
            ProgressBar.fillAmount = 1;
            SkipButton.gameObject.SetActive(false);
            CloseButton.gameObject.SetActive(true);
        }

        public void CloseAd()
        {
            EndAd();
            OnClosed?.Invoke(this, EventArgs.Empty);
            InvokeImpressionEvent(this, new ImpressionEventArgs(AdUnitId, new ImpressionData()));
            Canvas.enabled = false;
        }

        public void ClickedAd()
        {
            OnClicked?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            if (this != null && gameObject != null)
                Destroy(gameObject);
        }

        void InvokeImpressionEvent(object sender, ImpressionEventArgs args)
        {
            ((EditorImpressionEventPublisher)MediationService.Instance.ImpressionEventPublisher).InvokeOnImpressionEvent(sender, args);
        }
    }
}
#endif
