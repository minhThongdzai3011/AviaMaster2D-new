using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Leaderboard
{
    public class CountryItemControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Image border;

        public Color hoverColor;

        public Color selectedColor;

        public Image flag;

        private Country country;

        private bool selected = false;

        private bool isHover = false;

        public UnityAction<CountryItemControl> OnSelectCountry = null;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (selected) return;
            Select();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHover = true;
            if (selected) return;
            border.gameObject.SetActive(true);
            border.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHover = false;
            if (selected) return;
            border.gameObject.SetActive(false);
        }

        public void SetData(Country country)
        {
            this.country = country;
            Refresh();
        }

        private void Refresh() 
        {
            flag.sprite = CountryData.Instance.GetFlagByISOCode(country.countryISOCode);
        }

        public void Select() 
        {
            selected = true;
            isHover = false;
            OnSelectCountry?.Invoke(this);
            PlayerInfoManager.Instance.SetCountry(country.countryISOCode);
            border.gameObject.SetActive(true);
            border.color = selectedColor;
        }

        public void Deselect() 
        {
            selected = false;
            border.gameObject.SetActive(false);
        }
    }
}
