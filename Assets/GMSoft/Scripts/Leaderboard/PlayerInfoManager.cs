using Best.HTTP;
using Newtonsoft.Json;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Leaderboard
{
    public class PlayerInfoManager : MonoBehaviour
    {
        public bool useGeoPlugin = false;

        private const string PLAYER_PREFIX_NAME = "player";
        private const int PLAYER_SUFFIX_LENGTH = 6;
        private const string PLAYER_NAME_KEY = "player-name";
        private const string COUNTRY_NAME_KEY = "country-name";
        private const string DEFAULT_COUNTRY = "US";
        private char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

        public UnityAction<string> OnPlayerNameChange = null;
        public UnityAction<string> OnCountryChange = null;

        public static PlayerInfoManager Instance;

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (!PlayerPrefs.HasKey(COUNTRY_NAME_KEY) && useGeoPlugin)
            {
                SendGeoRequest();
            }
        }

        private async void SendGeoRequest()
        {
            string url = "http://www.geoplugin.net/json.gp";
            try
            {
                var geoRequest = HTTPRequest.CreateGet(url);
                var response = await geoRequest.GetHTTPResponseAsync();
                if (response.IsSuccess)
                {
                    string geoJson = response.DataAsText;
                    GeoIpData geoIpData = JsonConvert.DeserializeObject<GeoIpData>(geoJson);
                    SetCountry(geoIpData.geoplugin_countryCode);
                }
            }
            catch
            {
                Debug.Log($"request geo data fail.");
            }
        }

        private string GenerateRandomString(int length)
        {
            StringBuilder result = new StringBuilder(length);
            System.Random random = new System.Random();
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }

        private void ChangePlayerName(string value)
        {
            PlayerPrefs.SetString(PLAYER_NAME_KEY, value);
        }

        public string GetName()
        {
            if (PlayerPrefs.HasKey(PLAYER_NAME_KEY))
            {
                return PlayerPrefs.GetString(PLAYER_NAME_KEY);
            }
            else
            {
                string playerName = $"{PLAYER_PREFIX_NAME}{GenerateRandomString(PLAYER_SUFFIX_LENGTH)}";
                PlayerPrefs.SetString(PLAYER_NAME_KEY, playerName);
                PlayerPrefs.Save();
                return playerName;
            }
        }

        public void SetName(string name)
        {
            OnPlayerNameChange?.Invoke(name);
            PlayerPrefs.SetString(PLAYER_NAME_KEY, name);
            PlayerPrefs.Save();
        }

        public string GetCountry()
        {
            if (PlayerPrefs.HasKey(COUNTRY_NAME_KEY))
            {
                return PlayerPrefs.GetString(COUNTRY_NAME_KEY);
            }
            else 
            {
                PlayerPrefs.SetString(COUNTRY_NAME_KEY, DEFAULT_COUNTRY);
                PlayerPrefs.Save();
                return DEFAULT_COUNTRY;
            }
        }

        public void SetCountry(string countryISOCode)
        {
            OnCountryChange?.Invoke(countryISOCode);
            PlayerPrefs.SetString(COUNTRY_NAME_KEY, countryISOCode);
            PlayerPrefs.Save();
        }
    }
}
