using CrazyGames;
using GMSoft.Game;
using GMSoft.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GMSDK : MonoBehaviour
{
    public static GMSDK Instance { get; private set; }

    public GameModuleBase Game;
    public UserModuleBase User;

    private const int FAIL_REQUEST_COUNTER = 12;
    private int currentFailRequestCounter = FAIL_REQUEST_COUNTER;

    public GameObject gameLockedCanvas;
    public Text gameLockedLabel;

    private const string INFO_URL_BACKUP = "https://api.cdnwave.com/sdkdom/gamesdk";
    private const string INFO_URL = "https://api.cdndom.com/sdkdom/gamesdk";
    private const string DEFAULT_MORE_GAMES_URL = "https://azgames.io/";

    private readonly string publicXml = "<RSAKeyValue><Modulus>uDVZau+jEmEglKGYbFppkFmF1GNvoAM2zD9aY6CpnZOfB/VYKRsNyEUggzp5f49ggaVirL8ExuaznHvuZ4p2LU89edESf6bE6v+OuOP7+yLq8M8qDdKxUfyAD7PTItoIMWdCEhfrVIDUC1/IQZVYjX4IRG66BId1gXz9DwuIygG0JsDBU/l8lbo4b4ynKh0++P8ZoLUlhUwRCRW+w3RA/HobbOy05F7PWl3FZMObqYvoUVoqiDRULBTbsNHrUnzhnOqlNr0ismnq9a800o+WIVTXQQe2aZVXKLK/5++h3FT9EsunNEKzAFgwQdeeuFXGBVvDwK3iO7WEz6H5JS/ZyQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    private readonly string publicGameKeyXml = "<RSAKeyValue><Modulus>rrrvW6r2wD+hC869wEF0sBQwen6IqEiLedyQH+7KSbEtkwJ1XqxVYgmTr6XaCWkYx0FP5Wit6XlFxAT3zTQP51is0/STGurBAfntLOiJgU2MTX3FGznedO0tev/ibCdyR6ApnUYemAQzZYYWcxw05ho/FMq3jCu5a0D4AxraX+gv+IY2rjpQ4IZld6CyBlXPzm0bwkgAdP40AlgwwzW2hVo8m3WPSi8PyT1JWpv7D28M3zVj4PB8r//ktIEAzCqTO9rD2gNTZNk66K77bRzslc4G9L5V0a9ejofsur1uQWzD0xUVYQmYrc2OggWKjaaUtfs/Sn5D26JiEJmwkUgTRQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    private string currentTimeSpanRequest = string.Empty;

    private string sdkType = string.Empty;

    private bool onGetVeryfiedSignatureCalled = false;

    [DllImport("__Internal")]
    private static extern void GM_SDK_InitParam();

    [DllImport("__Internal")]
    private static extern void GM_SDK_GMEvent(string eventName, string msg = null);

    [DllImport("__Internal")]
    private static extern void GM_SDK_StartGame();

    [DllImport("__Internal")]
    private static extern void GM_SDK_GetVeryfiedSignature();

    private GMGameInfo gameInfo;
    private GameInfoRespone gameInfoResponse;

    public string targetScene;

    public event Action<GMGameInfo> OnLoadedGameInfo;
    public event Action<GameInfoRespone> OnLoadedGameInfoResponse;


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

#if !UNITY_EDITOR
        InitializeSDK();
#endif
    }

    private void Start()
    {
        LogSDKVersion();
        SendEvent("Start", "");
        LoadTargetScene();
    }

    private void OnGetVeryfiedSignature(string data)
    {
        try
        {
            onGetVeryfiedSignatureCalled = true;
            string json = Base64Decode(data);

            JObject jObject = JObject.Parse(json);
            string signed = jObject["signed"]?.ToString();
            string allowPlay = jObject["ap"]?.ToString();
            string domain = jObject["domain"]?.ToString();
            int stype = jObject["stype"]?.ToObject<int>() ?? 0;
            if (allowPlay != "yes")
            {
                LockGame("not allow play by veryfied signature");
                return;
            }

            string url = Application.absoluteURL;
            Uri uri = new Uri(url);
            string host = uri.Host;
            Debug.Log($"Current Domain: {url} ========> signature in localstorage: {signed}");
            bool checkSign = RsaHelper.Verify(domain, signed, publicXml);
            if (!checkSign)
            {
                LockGame("Domain signature not veryfied by sign");
                return;
            }
            if (stype == 1)
            {
                Debug.Log($"Domain veryfied: url: {url} ====> host: {host}");
                if (host.Equals(domain, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"Domain in white list: {domain}");
                    return;
                }
                // return;
            }
            else
            {
                Debug.Log($"Domain veryfied: url: {url} ====> host: {host}");
                if (host.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"Domain in white list: {domain}");
                    return;
                }
                // return;
            }

            LockGame("Domain signature not veryfied by domain");
        }
        catch
        {
            Debug.Log("Deserialize Veryfied Signature failure!");
            LockGame("Deserialize Veryfied Signature failure!");
        }
    }

    private void CheckGetVeryfiedSignatureCalled()
    {
        if (!onGetVeryfiedSignatureCalled)
        {
            LockGame("Get Veryfied Signature is not called");
        }
    }

    private void SetUnityHostName(string value)
    {
        Debug.Log($"Set Unity Host Name: {value}");
    }

    private void SetSDKType(string sdkType)
    {
        Debug.Log($"Set SDK Type: {sdkType}");
        this.sdkType = sdkType;
        switch (sdkType)
        {
            case "gm":
                Game = new GMGameModule();
                User = new GMUserModule();
                User.Initialize();
                break;
            case "crazy":
                CrazySDK.Init(() =>
                {
                    Game = new CrazyGameModule();
                    User = new CrazyUserModule();
                    User.Initialize();
                });
                break;
            case "gd":
                Game = new GDGameModule();
                User = new GDUserModule();
                User.Initialize();
                break;
        }
    }

    private void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(targetScene)) return;
        SceneManager.LoadScene(targetScene);
    }

    private void SetGameInfoParam(string data)
    {
#if !CRAZY_GAMES
        if (string.IsNullOrEmpty(data))
        {
            LockGame("Game Info is null before deserialization!");
            return;
        }
        try
        {
            gameInfo = JsonConvert.DeserializeObject<GMGameInfo>(data);
            OnLoadedGameInfo?.Invoke(gameInfo);
            if (gameInfo == null)
            {
                Debug.Log("Game Info is null after deserialization!");
                return;
            }
            if (gameInfo.allowPlay == "no")
            {
                LockGame("not allow play by game info");
            }
            gameInfo.promotion?.SendRequest();
            Debug.Log($"{JsonConvert.SerializeObject(gameInfo)}");
        }
        catch
        {
            Debug.Log("Deserialize GM Game Info failure!");
        }
#endif
    }

    public GMGameInfo GetGameInfo()
    {
#if UNITY_EDITOR
        return null;
#endif
        if (gameInfo == null)
        {
            Debug.LogWarning("Game Info is null!");
            return null;
        }
        return gameInfo;
    }

    public bool EnableMoreGames()
    {
#if UNITY_EDITOR
        return true;
#endif
        return gameInfo != null && !string.IsNullOrEmpty(gameInfo.moreGamesUrl);
    }

    public string GetMoreGamesUrl()
    {
#if UNITY_EDITOR
        return DEFAULT_MORE_GAMES_URL;
#endif
        if (gameInfo != null && !string.IsNullOrEmpty(gameInfo.moreGamesUrl))
        {
            return gameInfo.moreGamesUrl;
        }
        return DEFAULT_MORE_GAMES_URL;
    }

    public GMPromotion GetPromotion()
    {
#if UNITY_EDITOR
        return null;
#endif
        if (gameInfo != null && gameInfo.promotion != null)
        {
            return gameInfo.promotion;
        }
        return null;
    }

    public void InitializeSDK()
    {
        SendInfoUrlRequest();

        GM_SDK_InitParam();
        GM_SDK_StartGame();

#if !CRAZY_GAMES
        GM_SDK_GetVeryfiedSignature();
        Invoke(nameof(CheckGetVeryfiedSignatureCalled), 3); //3s neu ko call se lock game
#endif
    }

    private void SendInfoUrlRequest()
    {
        StartCoroutine(SendInfoUrlRequestCoroutine());
    }

    private IEnumerator SendInfoUrlRequestCoroutine()
    {
        string url = INFO_URL;
        if (currentFailRequestCounter % 2 == 0)
        {
            url = INFO_URL_BACKUP;
        }
        Dictionary<string, string> queries = new Dictionary<string, string>();
        double currentTimeSpan = GetCurrentTimeSpan();
        if (currentTimeSpan > 1_0000)
        {
            currentTimeSpan /= (double)1_0000;
        }
        currentTimeSpanRequest = Math.Round(currentTimeSpan).ToString();
        queries.Add("domain", Application.absoluteURL);
        queries.Add("gameid", GetGameId());
        queries.Add("timespan", currentTimeSpanRequest);
        string queriesString = JsonConvert.SerializeObject(queries);
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(queriesString);
        string base64Data = Convert.ToBase64String(plainTextBytes);
        if (url.Contains("http://"))
        {
            url = url.Replace("http://", "https://");
        }

        string gameInfoUrl = $"{url}?params={base64Data}";
        Debug.Log($"Create Request: {url}");
        Debug.Log("Wait Request");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(gameInfoUrl))
        {
            // Set timeout to 30 seconds
            webRequest.timeout = 30;

            yield return webRequest.SendWebRequest();

            Debug.Log("End Request");
            Debug.Log("Response");
            Debug.Log($"is success: {webRequest.result == UnityWebRequest.Result.Success}");

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var data = webRequest.downloadHandler.text;

                    string gameInfoString = Base64Decode(data);
                    gameInfoResponse = JsonConvert.DeserializeObject<GameInfoRespone>(gameInfoString);
                    OnLoadedGameInfoResponse?.Invoke(gameInfoResponse);
                    string hasCode = gameInfoResponse.hashcode;
                    if (gameInfoResponse == null || gameInfoResponse.allowPlay != 1)
                    {
                        //CheckMustBanPlayer(unityHostName, LockReason.Time_Span_Diff);
                        LockGame($"not allow play by game info response");
                        yield break;
                    }

                    bool checkSign = RsaHelper.Verify(currentTimeSpanRequest, hasCode, publicGameKeyXml);
                    Debug.Log($"check sign: {checkSign}");
                    if (!checkSign)
                    {
                        LockGame($"time span response diff");
                        yield break;
                    }

                    Debug.Log(data);
                    Debug.Log(gameInfoString);
                    //FIXME: gmsoft.Analytics.level = gameInfoResponse.analyticLevel;
                    yield break;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error processing response: {ex.Message}");
                }
            }
            else
            {
                Debug.LogError($"Request failed: {webRequest.error}");
            }
        }

        Invoke(nameof(RetryRequetLock), 30);
    }

    private void RetryRequetLock()
    {
        try
        {
            Debug.Log($"retryRequetLock: {currentFailRequestCounter}");
            SendEvent("Lock_Retry", "" + currentFailRequestCounter);
            if (currentFailRequestCounter-- > 0)
            {
                //moi 30s request 1 lan
                SendInfoUrlRequest();
            }
            else
            {
                LockGame("send game info request failure");
            }
        }
        catch
        {
            LockGame("EH358");
            Debug.LogError("ERROR CODE EH358");
        }
    }

    private void LockGame(string message = null)
    {
        Debug.Log($"[GMSDK] ===================> Game is locked: {message}");
        if (gameLockedCanvas != null)
        {
            gameLockedCanvas.SetActive(true);
            gameLockedLabel.text = $"{Application.productName} is locked.";
        }
        DisableGameInteract();
        SendEvent("Lock", message + " |" + currentFailRequestCounter);
        Firebase.Analytics.LogEvent($"game_locked__{message}");
    }

    private void DisableGameInteract()
    {
        if (EventSystem.current != null) EventSystem.current.enabled = false;
    }

    private void SendEvent(string eventName, string msg = null)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            GM_SDK_GMEvent(eventName, msg);
#endif
    }

    private double GetCurrentTimeSpan()
    {
        TimeSpan timeDiff = DateTime.UtcNow - new DateTime(1970, 1, 1);
        double totaltime = timeDiff.TotalMilliseconds;
        return totaltime;
    }

    public static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    private string GetGameId()
    {
        return Application.productName.ToLowerInvariant().Replace(" ", "-");
    }

    public void GamePlayStart()
    {
        Game?.GameplayStart();
    }

    public void GamePlayStop()
    {
        Game?.GameplayStop();
    }

    public void GamePlayPause()
    {
        Game?.GameplayPause();
    }

    public void GamePlayResume()
    {
        Game?.GameplayResume();
    }

    public void GamePlayAgain()
    {
        Game?.GamePlayAgain();
    }

    public bool CanSetUserName()
    {
        return User?.CanSetUserName() ?? false;
    }

    public string GetUserName()
    {
        return User?.GetUserName() ?? string.Empty;
    }

    public void SetUserName(string userName)
    {
        User?.SetUserName(userName);
    }

    public string GetSDKVersion()
    {
        return GMSDKConfig.Instance.SDKVersion;
    }

    public string GetFullSDKVersion()
    {
        return GMSDKConfig.Instance.FullVersionString;
    }

    public string GetDisplaySDKVersion()
    {
        return GMSDKConfig.Instance.DisplayVersionString;
    }

    public void LogSDKVersion()
    {
        Debug.Log($"<b><color=green>[{GMSDKConfig.Instance.SDKName}]</color></b> {GMSDKConfig.Instance.DisplayVersionString} - Build {GMSDKConfig.Instance.BuildNumber}");
    }
}

[Serializable]
public class GameInfoRespone
{
    [JsonProperty("p")]
    public int allowPlay;

    [JsonProperty("c")]
    public string hashcode;
}

[Serializable]
public class GMGameInfo
{
    [JsonProperty("allow_play")]
    public string allowPlay;
    [JsonProperty("more_games_url")]
    public string moreGamesUrl;
    public GMPromotion promotion;
}

[Serializable]
public class GMPromotion
{
    public string enable;
    [JsonProperty("call_to_action")]
    public string callToAction;
    [JsonProperty("promotion_list")]
    public GMPromotionData[] data;

    public GMPromotionData GetRandomLoadedPromotionData()
    {
        if (data == null || data.Length <= 0) return null;
        List<GMPromotionData> loadedPromotions = new List<GMPromotionData>();
        foreach (GMPromotionData promotion in data)
        {
            if (promotion.isLoaded)
            {
                loadedPromotions.Add(promotion);
            }
        }
        if (loadedPromotions.Count <= 0) return null;
        int randomIndex = UnityEngine.Random.Range(0, loadedPromotions.Count);
        return loadedPromotions[randomIndex];
    }

    public bool IsLoadedAllPromotions()
    {
        if (data == null || data.Length <= 0) return false;
        foreach (GMPromotionData promotion in data)
        {
            if (!promotion.isLoaded) return false;
        }
        return true;
    }

    public void SendRequest()
    {
        if (data == null) return;
        if (data.Length <= 0) return;
        foreach (GMPromotionData promotion in data)
        {
            try
            {
                GMSDK.Instance.StartCoroutine(LoadPromotionImageCoroutine(promotion));
            }
            catch
            {
                Debug.Log("send request promotion image failure");
            }
        }
    }

    private static IEnumerator LoadPromotionImageCoroutine(GMPromotionData promotion)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(promotion.image))
        {
            webRequest.timeout = 30;
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var texture = DownloadHandlerTexture.GetContent(webRequest);
                promotion.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                promotion.isLoaded = true;
            }
            else
            {
                Debug.LogError($"Failed to load promotion image: {webRequest.error}");
            }
        }
    }
}

[Serializable]
public class GMPromotionData
{
    public string name;
    public string image;
    public string url;

    [NonSerialized]
    public bool isLoaded;
    [NonSerialized]
    public Sprite sprite;
}

[Serializable]
public struct VeryfiedDomainData
{
    public string[] whiteListDomain;
    public string[] veryfiedSignature;
}
