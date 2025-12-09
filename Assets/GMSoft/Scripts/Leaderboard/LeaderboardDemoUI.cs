using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.UI;

namespace Leaderboard
{
    public class LeaderboardDemoUI : CustomPopup
    {
        public TabGroup tabGroup;

        public int rankPerPage = 10;

        public int currentPage = 1;

        public Button previousPage;

        public Button nextPage;

        public TMP_Text page;

        public RankItemControl rankPrefab;

        public Transform rankContainer;

        [HideInInspector]
        public LeaderboardData leaderboardData;

        private List<RankItemControl> items = new List<RankItemControl>();

        private TOP_TIME topTime;

        protected override void Start()
        {
            base.Start();
            LeaderboardManager.Instance.OnLeaderboardResponse += LeaderboardResponse;
            tabGroup.OnNewTabSelected += SelectNewTab;
            nextPage.onClick.AddListener(OnNextPage);
            previousPage.onClick.AddListener(OnPreviousPage);
            InitRankItems();
        }

        private void InitRankItems()
        {
            for (int i = 0; i < rankPerPage; i++)
            {
                RankItemControl item = Instantiate(rankPrefab, rankContainer);
                item.gameObject.SetActive(false);
                items.Add(item);
            }
        }

        private void OnDestroy()
        {
            tabGroup.OnNewTabSelected -= SelectNewTab;
            if (LeaderboardManager.Instance != null)
            {
                LeaderboardManager.Instance.OnLeaderboardResponse -= LeaderboardResponse;
            }
        }

        private void SelectNewTab(TabButton tab)
        {
            currentPage = 1;
            switch (tab.tabName)
            {
                case "alltime":
                    topTime = TOP_TIME.AllTime;
                    break;
                case "month":
                    topTime = TOP_TIME.Month;
                    break;
                case "week":
                    topTime = TOP_TIME.Week;
                    break;
                case "day":
                    topTime = TOP_TIME.Day;
                    break;
            }
            GetScores();
        }

        private void LeaderboardResponse(string data)
        {
            try
            {
                leaderboardData = JsonConvert.DeserializeObject<LeaderboardData>(data);
                UpdateRankingUI();
            }
            catch
            {
            }
        }

        private void UpdateRankingUI()
        {
            page.text = $"{currentPage}";
            previousPage.interactable = true;
            nextPage.interactable = true;
            if (currentPage <= 1)
            {
                previousPage.interactable = false;
            }
            nextPage.interactable = leaderboardData.total > rankPerPage * currentPage;
            for (int i = 0; i < leaderboardData.results.Length; i++)
            {
                RankData rankData = leaderboardData.results[i];
                items[i].gameObject.SetActive(true);
                items[i].SetData(rankData);
            }
        }

        private void DeactivePaging()
        {
            previousPage.interactable = false;
            nextPage.interactable = false;
        }

        private void ClearContent()
        {
            foreach (RankItemControl item in items)
            {
                item.gameObject.SetActive(false);
            }
        }

        private void OnNextPage()
        {
            DeactivePaging();
            currentPage++;
            page.text = $"{currentPage}";
            GetScores();
        }

        private void OnPreviousPage()
        {
            DeactivePaging();
            currentPage--;
            page.text = $"{currentPage}";
            GetScores();
        }

        private void GetScores()
        {
            ClearContent();
            LeaderboardManager.Instance.GetScores(topTime, (currentPage - 1) * rankPerPage, rankPerPage);
        }
    }
}
