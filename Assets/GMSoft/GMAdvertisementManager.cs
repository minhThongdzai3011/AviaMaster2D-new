using GMSoft.Advertisement;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GMAdvertisementManager : MonoBehaviour
{
    public static GMAdvertisementManager Instance;
    public AdvertisementModuleBase advertisement;

    public bool pauseGameDuringAd = true;

    public GameObject loadingAdOverlayCanvas;

    // UI Text components for countdown display
    public Text countdownText;

    public Action ResumeAction;
    public Action PauseAction;
    public Action RewardSuccessAction;

    private SdkAdInfo adInfo;

    private float originTimeScale;
    private bool originRunInBackground;
    private float originAudioListenerVolume;

    private float timeShowAd;
    private float showAdTimer;
    private float timeShowRewardAd;
    private float showRewardAdTimer;

    private bool enableAd = false;

    // Countdown variables
    private bool isCountdownActive = false;
    private Coroutine countdownCoroutine;
    private const float COUNTDOWN_TIME = 3f; // Fixed countdown time

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        showAdTimer += Time.deltaTime;
        showRewardAdTimer += Time.deltaTime;
    }

    private void SetAdType(string type)
    {
        if(enableAd == false)
        {
            Debug.Log("Ads are disabled.");
            return;
        }
        switch (type)
        {
            case "crazy":
                advertisement = new CrazyGameAdModule();
                break;
            case "gd":
                advertisement = new GameDistributionAdModule();
                (advertisement as GameDistributionAdModule).SetGameKey(adInfo.gdGameKey);
                break;
            case "h5":
                advertisement = new H5AdModule();
                break;
            case "wg":
                advertisement = new WeeGooAdModule();
                showAdTimer = 0; // Initialize to allow immediate ad display;
                break;
            default:
                advertisement = new GMAdModule();
                break;
        }
        advertisement?.Initialize();
    }

    private void Start()
    {
        if (advertisement == null || !enableAd) return;
        advertisement?.PreloadRewardedAd();
    }

    private void SetAdParam(string data)
    {
        Debug.Log($"Set Ad Param: {data}");
        if (string.IsNullOrEmpty(data))
        {
            Debug.Log("Null Ad Param data");
            return;
        }
        try
        {
            adInfo = JsonConvert.DeserializeObject<SdkAdInfo>(data);
            Debug.Log(JsonConvert.SerializeObject(adInfo));
            if (adInfo == null)
            {
                Debug.Log("Deserialize SdkAdInfo is null!");
                return;
            }
            enableAd = adInfo.enable.ToLower() == "yes";
            timeShowAd = adInfo.timeShowInter;
            timeShowRewardAd = adInfo.timeShowReward;
            showAdTimer = adInfo.timeShowReward; // Initialize to allow immediate ad display
            showRewardAdTimer = adInfo.timeShowReward; // Initialize to allow immediate rewarded ad display
            SetAdType(adInfo.sdkType);
            HandleAdEvent();
        }
        catch
        {
            Debug.Log("Deserialize SdkAdInfo failure!");
        }
    }

    private void HandleAdEvent()
    {
        if (advertisement == null || !enableAd)
        {
            Debug.Log("Advertisement module is not initialized or ads are disabled.");
            return;
        }
        advertisement.OnPauseGame += OnPause;
        advertisement.OnResumeGame += OnResume;
        advertisement.OnRewardGame += OnRewardGame;
        advertisement.OnRewardedVideoSuccess += OnRewardedSuccess;
        advertisement.OnRewardedVideoFailure += OnRewardedFailure;
        advertisement.OnPreloadRewardedVideo += OnReadyRewardAd;
    }

    public bool CanShowAd()
    {
        if (adInfo == null || !enableAd)
        {
            Debug.Log("Ad info is null or ads are disabled.");
            return false;
        }
        return GetNextShowAdTime() <= 0;
    }

    public bool CanShowRewardedAd()
    {
        if (adInfo == null || !enableAd)
        {
            Debug.Log("Ad info is null or ads are disabled.");
            return false;
        }
        if(advertisement.preladRewardedVideo == false)
        {
            Debug.Log("Rewarded video is not preloaded.");
            return false;
        }
        return GetNextShowRewardAdTime() <= 0;
    }

    public float GetNextShowAdTime()
    {
        if (adInfo == null || !enableAd)
        {
            Debug.Log("Ad info is null or ads are disabled.");
            return 0f;
        }
        return Mathf.Max(0, timeShowAd - showAdTimer);
    }

    public float GetNextShowRewardAdTime()
    {
        if (adInfo == null || !enableAd)
        {
            //Debug.Log("Ad info is null or ads are disabled.");
            return 0f;
        }
        return Mathf.Max(0, timeShowRewardAd - showRewardAdTimer);
    }

    private IEnumerator ShowCountdownAndThenAd(float countdownTime, Action showAdAction)
    {
        isCountdownActive = true;

        // Show loading overlay during countdown
        loadingAdOverlayCanvas?.SetActive(true);

        float remainingTime = countdownTime;

        while (remainingTime > 0)
        {
            // Update countdown text
            if (countdownText != null)
            {
                countdownText.text = $"ADS SHOWING IN {Mathf.Ceil(remainingTime)}S";
            }

            yield return new WaitForSecondsRealtime(1f);
            remainingTime -= 1f;
        }

        isCountdownActive = false;

        // Clear countdown text when countdown ends
        if (countdownText != null)
        {
            countdownText.text = "";
        }

        // Show the ad
        showAdAction?.Invoke();
    }

    public void CancelCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        isCountdownActive = false;

        // Clear countdown text when cancelling
        if (countdownText != null)
        {
            countdownText.text = "";
        }

        // Hide loading overlay when cancelling countdown
        loadingAdOverlayCanvas?.SetActive(false);
    }

    private void OnReadyRewardAd(int value)
    {
    }

    private void OnRewardedFailure()
    {
        Debug.Log("Rewarded video failed.");
        advertisement?.PreloadRewardedAd();
        Invoke("PreloadRewardedAd", 0.5f);
    }

    private void OnRewardedSuccess()
    {
        Debug.Log("Rewarded video success.");
        advertisement?.PreloadRewardedAd();
        Invoke("PreloadRewardedAd", 0.5f);
    }

    private void OnRewardGame()
    {
        RewardSuccessAction?.Invoke();
        RewardSuccessAction = null;
    }

    private void OnPause()
    {
        PauseAction?.Invoke();
        PauseAction = null;
        loadingAdOverlayCanvas?.SetActive(true);
        originTimeScale = Time.timeScale;
        originRunInBackground = Application.runInBackground;
        originAudioListenerVolume = AudioListener.volume;
        AudioListener.volume = 0;
        if (pauseGameDuringAd)
        {
            Time.timeScale = 0;
        }
    }

    private void OnResume()
    {
        CleanupAd();
    }

    private void CleanupAd()
    {
        ResumeAction?.Invoke();
        ResumeAction = null;
        loadingAdOverlayCanvas?.SetActive(false);
        AudioListener.volume = originAudioListenerVolume;
        Application.runInBackground = originRunInBackground;
        if (pauseGameDuringAd)
        {
            Time.timeScale = originTimeScale;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
            // Focus window for WebGL
            Application.ExternalCall("window.focus");
#endif
    }

    public void ShowAd(Action PauseAction = null, Action ResumeAction = null)
    {
#if UNITY_EDITOR
        ResumeAction?.Invoke();
        return;
#endif

        if (isCountdownActive)
        {
            Debug.Log("Countdown is already active.");
            return;
        }

        if (!gameObject.activeInHierarchy)
        {
            Debug.Log("Cannot start ad coroutine - GameObject is not active in hierarchy.");
            return;
        }

        this.PauseAction = PauseAction;
        this.ResumeAction = ResumeAction;

        if (!CanShowAd())
        {
            Debug.Log("Cannot show ad. Next show time: " + GetNextShowAdTime());
            this.ResumeAction?.Invoke();
            return;
        }

        // Start countdown before showing ad
        countdownCoroutine = StartCoroutine(ShowCountdownAndThenAd(COUNTDOWN_TIME, () =>
        {
            showAdTimer = 0f;
            advertisement?.ShowAd();
            timeShowAd = adInfo.timeShowInter;
        }));
    }

    public void ShowRewardedAd(Action PauseAction = null, Action ResumeAction = null, Action FinishAction = null)
    {
#if UNITY_EDITOR
        ResumeAction?.Invoke();
        return;
#endif

        if (isCountdownActive)
        {
            Debug.Log("Countdown is already active.");
            return;
        }

        if (!gameObject.activeInHierarchy)
        {
            Debug.Log("Cannot start rewarded ad coroutine - GameObject is not active in hierarchy.");
            return;
        }

        this.PauseAction = PauseAction;
        this.ResumeAction = ResumeAction;
        this.RewardSuccessAction = FinishAction;

        if (!CanShowRewardedAd())
        {
            Debug.Log("Cannot show rewarded ad. Next show time: " + GetNextShowRewardAdTime());
            this.ResumeAction?.Invoke();
            return;
        }

        // Start countdown before showing rewarded ad
        countdownCoroutine = StartCoroutine(ShowCountdownAndThenAd(COUNTDOWN_TIME, () =>
        {
            showRewardAdTimer = 0f;
            advertisement?.ShowRewardedAd();
            Debug.Log("Show rewarded ad called.");
            timeShowRewardAd = adInfo.timeShowReward;
        }));
    }

    private void PreloadRewardedAd()
    {
        advertisement?.PreloadRewardedAd();
    }

    public SdkAdInfo GetSdkAdInfo()
    {
        return adInfo;
    }
}

[Serializable]
public class SdkAdInfo
{
    public string enable;
    [JsonProperty("sdk_type")]
    public string sdkType;
    [JsonProperty("time_show_inter")]
    public int timeShowInter;
    [JsonProperty("time_show_reward")]
    public int timeShowReward;
    [JsonProperty("gd_game_key")]
    public string gdGameKey;
}