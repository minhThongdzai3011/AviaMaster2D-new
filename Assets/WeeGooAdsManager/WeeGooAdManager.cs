using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.UI;



[System.Serializable]
public class GameOptions
{
    public int frequency;
    public int timer;
    public string game;
    public string key;
    public string docDomain;
    public string baseName;
    public int gameVolume;
}

public class WeeGooAdManager : MonoBehaviour
{
    private static WeeGooAdManager _instance;

    public static WeeGooAdManager Instance { get { return _instance; } }

    [DllImport("__Internal")] private static extern void Ping(string instanceName);
    [DllImport("__Internal")] private static extern void RegisterGameControls();
    [DllImport("__Internal")] private static extern void FetchAd();
    [DllImport("__Internal")] private static extern void RegisterRewardCallbacks();
    [DllImport("__Internal")] private static extern void ShowRewardAdCallback();
    [DllImport("__Internal")] private static extern void RefetchReward();
    
    [DllImport("__Internal")] private static extern void GameEvent( string gameEvent );

    public delegate void FunctionWithContextDelegate();
    private Dictionary<string, FunctionWithContextDelegate> functionDictionary;

    private int adsCounter = 0;
    public int AdInterval = 0;
    public int ShowAdEveryNEvent = 1;
 
    private bool isShowing = false;
    private float initialVolume = 0f;
    private float timeSinceStart = 0f;
    private int pingCount = 0;
    private string homeBase;
    private bool locked = true;
    private bool canIncrement = true;
    private bool soundHasBeenPausedByAd = false;
    private int nextUpdate = 1;

    public UnityEvent OnReady;
    public UnityEvent OnSuccess;
    public UnityEvent OnFail;
    public UnityEvent OnPause;
    public UnityEvent OnResume;

    private bool rewardIsReadyToDisplay = false;

    private void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        try
        {
            Debug.Log("Initiating WeeGooEvoads 2...");
            Ping(Instance.name.ToString());
            pingCount++;

            if (pingCount == 1)
            {
                RegisterRewardCallbacks();
                RegisterGameControls();
            }
        }
        catch (System.IO.IOException)
        {
            Debug.Log("Awake: Ping only available in live environment.");
        }
#else
        Debug.LogWarning("WGSDK: Ads will only work in the browser. Please build your game and upload it to a server.");
#endif
    }

 
    private void Update()
    {
        if (locked)
            return;

        if (!canIncrement)
            return;

        if (Time.realtimeSinceStartup >= nextUpdate)
        {
            nextUpdate = Mathf.FloorToInt(Time.realtimeSinceStartup) + 1;
            UpdateEverySecond();
        }
    }

    private void UpdateEverySecond()
    {
        timeSinceStart += Time.deltaTime;

        if (AdInterval > 0)
        {
            int seconds = Mathf.FloorToInt(timeSinceStart);

            if (!isShowing && seconds > 0 && seconds % AdInterval == 0 && (Mathf.Abs(seconds - adsCounter * AdInterval)) % AdInterval == 0)
            {
                canIncrement = false;
                adsCounter++;
                EnterAdState();
            }
        }
    }

    public void CheckIfAllowed()
    {
        string[] hosts = { homeBase, "bG9jYWxob3N0Og==", "d2dwbGF5ZXIuY29t" };
        string liveHost = GetLiveHost();

        bool allowed = false;
        for (int i = 0; i < hosts.Length; i++)
        {
            string host = GetDecodedHost(hosts[i]);
            if (IsHostMatch(liveHost, host))
            {
                allowed = true;
                break;
            }
        }

        if (!allowed)
        {
            Application.OpenURL("http://" + homeBase);
        }
    }

    private string GetLiveHost()
    {
        string liveHost = Application.absoluteURL;
        string[] splitString = liveHost.Split(new string[] { "//" }, StringSplitOptions.None);
        liveHost = splitString[1].Replace("www.", "");

        if (liveHost.Length > homeBase.Length)
            liveHost = liveHost.Substring(0, homeBase.Length);

        return liveHost;
    }

    private string GetDecodedHost(string host)
    {
        if (Regex.IsMatch(host, "^[a-zA-Z0-9/+]*={0,2}$"))
        {
            byte[] decodedBytes = Convert.FromBase64String(host);
            return Encoding.UTF8.GetString(decodedBytes);
        }

        return host;
    }

    private bool IsHostMatch(string liveHost, string host)
    {
        return host == liveHost;
    }

    public void FetchNewReward()
    {
        RefetchReward();
    }

    public void Unlock(string optionsFromFile)
    {
        string tempOptions = Uri.UnescapeDataString(optionsFromFile);
        GameOptions myOptions = JsonUtility.FromJson<GameOptions>(tempOptions);

        if (myOptions.key == "_246_")
        {
            locked = false;
            Instance.locked = false;

            if (myOptions.frequency > 0)
                ShowAdEveryNEvent = myOptions.frequency;

            if (myOptions.timer > -1)
                AdInterval = myOptions.timer;

            homeBase = myOptions.docDomain;

            Debug.Log($"Frequency: {ShowAdEveryNEvent}");
            Debug.Log($"AdInterval: {AdInterval}");
            Debug.Log($"HomeBase: {homeBase}");
            Debug.Log($"TempOptions: {tempOptions}");
            Debug.Log($"Key: {myOptions.key}");

            CheckIfAllowed();
        }
    }

    public void GetAd()
    {
        adsCounter++;

        if (adsCounter > 0 && adsCounter % ShowAdEveryNEvent == 0)
        {
            EnterAdState();
        }
    }

    public bool IsLocked => locked;

    public void Resume()
    {
        OnResume?.Invoke();
        ExitAdState();
    }

    public void Pause()
    {
        OnPause?.Invoke();
        isShowing = true;
        Time.timeScale = 0;

        if (AudioListener.volume > 0 && !soundHasBeenPausedByAd)
        {
            initialVolume = AudioListener.volume;
            AudioListener.pause = true;
            soundHasBeenPausedByAd = true;
        }
    }

    private void EnterAdState()
    {
        Pause();
        FetchAd();
    }

    private void ExitAdState()
    {
        isShowing = false;
        Time.timeScale = 1;

        if (soundHasBeenPausedByAd)
        {
            AudioListener.volume = initialVolume;
            AudioListener.pause = false;
            soundHasBeenPausedByAd = false;
        }

        canIncrement = true;
    }

    public void OnReadyMethod()
    {
        rewardIsReadyToDisplay = true;
        OnReady?.Invoke();
    }

    public void OnSuccessMethod()
    {
        ExitAdState();
        OnSuccess?.Invoke();
        Debug.Log("WG call success method!");
    }
 
    public void OnFailMethod()
    {
        ExitAdState();
        OnFail?.Invoke();
        Debug.Log("WG call fail method!");
    }

    public void ShowRewardAd()
    {
        if (rewardIsReadyToDisplay)
        {
            ShowRewardAdCallback();
            rewardIsReadyToDisplay = false;
        }
    }

    //game events
    public void GAME_START(){
        GameEvent( "game-event-start" );
    }
    public void GAME_LEVEL_START(){
        GameEvent( "game-event-start" );
    }
    public void GAME_LEVEL_FINISHED(){
        GameEvent( "game-event-start" );
    }
    public void GAME_LEVEL_FAILED(){
        GameEvent( "game-event-start" );
    }
    public void GAME_LIFE_LOST(){
        GameEvent( "game-event-start" );
    }
    public void GAME_PAUSE(){
        GameEvent( "game-event-start" );
    }
    public void GAME_RESUME(){
        GameEvent( "game-event-start" );
    }
    public void GAME_OVER(){
        GameEvent( "game-event-start" );
    }
    public void GAME_CHECKPOINT(){
        GameEvent( "game-event-start" );
    }
    public void GAME_TUTORIAL(){
        GameEvent( "game-event-start" );
    }
    public void GAME_IAP(){
        GameEvent( "game-event-start" );
    }
    public void GAME_ACHIEVEMENT_UBBLOCKED(){
        GameEvent( "game-event-start" );
    }
    public void GAME_COUNTDOWN(){
        GameEvent( "game-event-start" );
    }
    public void GAME_DAILY_REWARD(){
        GameEvent( "game-event-start" );
    }

}