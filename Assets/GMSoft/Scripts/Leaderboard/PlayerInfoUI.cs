using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Leaderboard
{
    public class PlayerInfoUI : MonoBehaviour
    {
        public TMP_InputField playerNameInput;
        public Button country;
        public GameObject countrySelectPanelPrefab;

        private string playerName;

        private void Start()
        {
            playerNameInput.text = PlayerInfoManager.Instance.GetName();
            playerNameInput.onEndEdit.AddListener(UpdatePlayerName);
            if (PlayerInfoManager.Instance != null)
            {
                PlayerInfoManager.Instance.OnCountryChange += UpdateCountry;
            }
            UpdateCountry(PlayerInfoManager.Instance.GetCountry());
            country.onClick.AddListener(ShowCountrySelectPanel);
        }

        private void ShowCountrySelectPanel() 
        {
            Instantiate(countrySelectPanelPrefab, transform.GetComponentInParent<Canvas>().transform);   
        }

        private void OnDestroy()
        {
            if (PlayerInfoManager.Instance != null)
            {
                PlayerInfoManager.Instance.OnCountryChange -= UpdateCountry;
            }
        }

        private void UpdateCountry(string countryCode)
        {
            country.image.sprite = CountryData.Instance.GetFlagByISOCode(countryCode);
        }

        private void UpdatePlayerName(string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                playerNameInput.text = playerName;
                return;
            }
            playerName = newName;
            PlayerInfoManager.Instance.SetName(newName);
        }
    }
}
