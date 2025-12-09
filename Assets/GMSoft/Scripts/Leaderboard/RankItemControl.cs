using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

namespace Leaderboard
{
    public class RankItemControl : MonoBehaviour
    {
        private RankData data;
        public TMP_Text index;
        public Image flag;
        public TMP_Text userName;
        public TMP_Text score;

        public void SetData(RankData data)
        {
            this.data = data;
            Refresh();
        }

        private void Refresh()
        {
            index.text = $"{data.rank + 1}";
            Dictionary<string, string> metadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.metadata);
            Sprite flagSpr = CountryData.Instance.GetFlagByISOCode(metadata["country"]);
            if (flagSpr == null)
            {
                flag.color = Color.black;
            }
            else
            {
                flag.sprite = flagSpr;
            }
            userName.text = $"{metadata["name"]}";
            score.text = $"{data.score}";
        }

        /// <summary>
        /// Check if current ranking is player or not
        /// </summary>
        /// <returns></returns>
        public bool IsMine()
        {
            if (data == null) return false;
            if (!AuthenticationService.Instance.IsSignedIn) return false;
            return data.playerId == AuthenticationService.Instance.PlayerId;
        }
    }
}
