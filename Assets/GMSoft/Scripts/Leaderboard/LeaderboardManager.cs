using Assets.GMDev.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;

namespace Leaderboard
{
    public enum TOP_TIME
    {
        Day, Week, Month, AllTime
    }

    [Serializable]
    public class LeaderboardData
    {
        public int offset;
        public int limit;
        public int total;
        public RankData[] results;
    }

    [Serializable]
    public class RankData
    {
        public string playerId;
        public string playerName;
        public int rank;
        public float score;
        public string metadata;
    }

    public class LeaderboardManager : MonoBehaviour
    {
        public static LeaderboardManager Instance;

        public string leaderboardAlltimeId = "demo-leaderboard-alltime";
        public string leaderboardMonthId = "demo-leaderboard-month";
        public string leaderboardWeekId = "demo-leaderboard-week";
        public string leaderboardDayId = "demo-leaderboard-day";

        public Action<string> OnLeaderboardResponse = null;

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public void PostScore(int score)
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            options.Add("name", PlayerInfoManager.Instance.GetName());
            options.Add("country", PlayerInfoManager.Instance.GetCountry());
            options.Add("s", OpenSSL.EncodeWithPass(score.ToString()));
            AddScore(score, options);
        }

        private async void AddScore(int score, Dictionary<string, string> options = null)
        {
            AddPlayerScoreOptions scoreOptions = new AddPlayerScoreOptions()
            {
                Metadata = options
            };
            Debug.Log($"id: {leaderboardAlltimeId}");
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardAlltimeId, score, scoreOptions);
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardMonthId, score, scoreOptions);
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardWeekId, score, scoreOptions);
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardDayId, score, scoreOptions);
        }

        public async void GetScores(TOP_TIME top, int offset, int limit)
        {
            string leaderboardId = "";
            switch (top)
            {
                case TOP_TIME.AllTime:
                    leaderboardId = leaderboardAlltimeId;
                    break;
                case TOP_TIME.Month:
                    leaderboardId = leaderboardMonthId;
                    break;
                case TOP_TIME.Week:
                    leaderboardId = leaderboardWeekId;
                    break;
                case TOP_TIME.Day:
                    leaderboardId = leaderboardDayId;
                    break;
            }
            GetScoresOptions options = new GetScoresOptions()
            {
                Offset = offset,
                Limit = limit,
                IncludeMetadata = true
            };
            var scoresRespone = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, options);
            OnLeaderboardResponse?.Invoke(JsonConvert.SerializeObject(scoresRespone, Formatting.Indented));
        }
    }
}
