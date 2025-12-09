using Assets.GMDev.Utilities;
using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Random = System.Random;

namespace Leaderboard
{
    public class LeaderboardDemo : MonoBehaviour
    {
        public TMP_InputField dataResponse;

        public TMP_InputField nameInput;
        public TMP_InputField scoreInput;
        public TMP_InputField countryInput;
        public TMP_InputField scoreEncryptInput;

        public Button post1;
        public Button post2;

        public TMP_InputField offsetInput;
        public TMP_InputField limitInput;

        public Button get;

        private string playerName;
        private string country;

        private int offset;
        private int limit;
        private int score;

        private void Start()
        {
            nameInput.onValueChanged.AddListener(OnNameChange);
            scoreInput.onValueChanged.AddListener(OnScoreChange);
            countryInput.onValueChanged.AddListener(OnCountryChange);
            offsetInput.onValueChanged.AddListener(OnOffsetChange);
            limitInput.onValueChanged.AddListener(OnLimitChange);

            offset = 0;
            limit = 10;

            //post1.onClick.AddListener(() => 
            //{
            //    LeaderboardManager.Instance.PostScore(playerName, country, score);
            //});

            //post2.onClick.AddListener(() =>
            //{
            //    LeaderboardManager.Instance.PostScore2(playerName, country, score, scoreEncryptInput.text);
            //});

            get.onClick.AddListener(() => 
            {
                LeaderboardManager.Instance.GetScores(TOP_TIME.AllTime, offset, limit);
            });

            LeaderboardManager.Instance.OnLeaderboardResponse += (string data) =>
            {
                dataResponse.text = data;
            };
        }

        private void OnNameChange(string value)
        {
            playerName = value;
        }

        private void OnScoreChange(string value)
        {
            score = int.Parse(value);
            scoreEncryptInput.text = OpenSSL.EncodeWithPass(value);
        }

        private void OnCountryChange(string value)
        {
            country = value;
        }

        private void OnOffsetChange(string value)
        {
            offset = int.Parse(value);
        }

        private void OnLimitChange(string value)
        {
            limit = int.Parse(value);
        }
    }
}
