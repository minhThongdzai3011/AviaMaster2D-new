using System.Collections.Generic;
using UnityEngine;
using Utils.UI;

namespace Leaderboard
{
    public class CountrySelectPanel : CustomPopup
    {
        public CountryItemControl countryItemPrefab;
        public Transform countryContainer;
        private CountryItemControl currentCountry;
        private List<CountryItemControl> countryList = new List<CountryItemControl>();

        protected override void Start()
        {
            base.Start();
            CreateFlags();
        }

        private void CreateFlags()
        {
            foreach (var country in CountryData.Instance.countries)
            {
                CountryItemControl item = Instantiate(countryItemPrefab, countryContainer);
                item.SetData(country);
                if (PlayerInfoManager.Instance.GetCountry() == country.countryISOCode) 
                {
                    item.Select();
                    currentCountry = item;
                }
                item.OnSelectCountry += SelectNewCountry;
                countryList.Add(item);
            }
        }

        private void OnDestroy()
        {
            if (countryList == null || countryList.Count <= 0) return;
            foreach (CountryItemControl item in countryList) 
            {
                item.OnSelectCountry -= SelectNewCountry;
            }
        }

        private void SelectNewCountry(CountryItemControl newCountry) 
        {
            currentCountry?.Deselect();
            currentCountry = newCountry;
        }
    }
}
