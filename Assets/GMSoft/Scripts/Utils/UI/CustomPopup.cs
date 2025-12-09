using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utils.UI
{
    public class CustomPopup : MonoBehaviour, IPointerClickHandler
    {
        public bool closeWhenClickOutside;
        public GameObject backgroundOverlay;
        public Button close;

        protected virtual void Start()
        {
            if (close != null)
            {
                close.onClick.AddListener(ClosePopup);
            }
        }

        public virtual void OpenPopup()
        {
            gameObject.SetActive(true);
        }

        public virtual void ClosePopup()
        {
            gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!closeWhenClickOutside) return;
            if (eventData.pointerCurrentRaycast.gameObject == backgroundOverlay)
            {
                ClosePopup();
            }
        }
    }
}
