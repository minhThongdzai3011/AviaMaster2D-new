using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GMAnalytics : MonoBehaviour
{
    public static GMAnalytics Instance { get; private set; }
    public bool debugAnalytics = true;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void LogCurrentHour()
    {
        int currentHour = System.DateTime.Now.Hour;
        LogEvent($"OpenGameTime:_{currentHour.ToString()}");
    }

    public static int gameCountPerSession = 0;
    public static void PlayGame()
    {
		LogEvent("PlayGame");

		gameCountPerSession++;

		LogEvent("GameCount_PerSession_" + gameCountPerSession);
	}
    public static void EndGame()
    {
		LogEvent("EndGame");
	}

	public static void LogEvent(string eventName, int level = 2)
    {
        if (Instance != null && Instance.debugAnalytics)
        {
            Debug.Log($"GMAnalytics LogEvent: {eventName}");
        }
        Firebase.Analytics.LogEvent($"GM_{eventName}", level);
    }
    public static void LogEventParameter(string eventName, string eventParam, int level = 2)
    {
        if (Instance != null && Instance.debugAnalytics)
        {
            Debug.Log($"GMAnalytics LogEventParameter: {eventName}, {eventParam}");
        }
        Firebase.Analytics.LogEventParameter(eventName, eventParam, level);
    }
    public static void LogEventParameter(string eventName, Dictionary<string, object> eventParamDict, int level = 2)
    {
        if (Instance != null && Instance.debugAnalytics)
        {
            Debug.Log($"GMAnalytics LogEventParameter: {eventName}, {JsonConvert.SerializeObject(eventParamDict)}");
        }
        Firebase.Analytics.LogEventParameter(eventName, eventParamDict, level);
    }
    // Start is called before the first frame update
    void Start()
    {
        LogTime();
        LogCurrentHour();
    }
    
    private int count;
    private void LogTime()
    {
        LogEvent($"SessionTime:_{(count / 2f).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)}");
        Invoke(nameof(LogTime), 30f);
        count++;
    }

    public void OnInteractGame()
    {
        // GmSoft.Instance.OnInteractGame();
    }
    public void MoreGamesButton()
    {
        LogEvent("MoreGames");
        // Application.OpenURL(GmSoft.Instance.GetMoreGameUrl());
        //Application.OpenURL(GmSoft.Instance.GetMoreGameUrl());

    }
    
}
