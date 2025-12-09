using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Leaderboard
{
    public class TabGroup : MonoBehaviour
    {
        public List<TabButton> tabs = new List<TabButton>();
        public TabButton defaultTab;
        private TabButton selectedTab;
        public UnityAction<TabButton> OnNewTabSelected = null;

        private void Start()
        {
            foreach (TabButton tab in tabs)
            {
                tab.OnSelectTab += OnSelectTab;
            }
            if (defaultTab != null)
            {
                defaultTab.Select();
                selectedTab = defaultTab;
            }
        }

        private void OnDestroy()
        {
            foreach (TabButton tab in tabs)
            {
                tab.OnSelectTab -= OnSelectTab;
            }
        }

        private void OnSelectTab(TabButton tab)
        {
            selectedTab?.Deselect();
            tab.Select();
            selectedTab = tab;
            OnNewTabSelected?.Invoke(tab);
        }
    }
}
