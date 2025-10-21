using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
public partial class UIEventTracker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public enum PointerState
        {
            None = 0,
            Enter = 1 << 0,
            Exit = 1 << 1,
            Down = 1 << 2,
            Up = 1 << 3,
            Click = 1 << 4
        }
       
        public List<UIEvent> events;

        private UIEvent GetEvent(PointerState pointerState)
        {
            if (events == null) return null;
            return events.FirstOrDefault(e => e.pointerState == pointerState);
        }

        private void AnalyticsUIEvent(PointerState pointerState)
        {
            UIEvent @event = GetEvent(pointerState);
            if (@event == null) return;
            string eventName = @event.name;
            if (!@event.hasParams)
            {
                Firebase.Analytics.LogEvent($"{eventName}", @event.analyticsLevel);
            }
            else
            {
                string @params = JsonConvert.SerializeObject(@event.@params);
                Firebase.Analytics.LogEventParameter($"{eventName}", @params, @event.analyticsLevel);
            }
        }

        public void OnPointerClick(PointerEventData eventData) => AnalyticsUIEvent(PointerState.Click);
        public void OnPointerDown(PointerEventData eventData) => AnalyticsUIEvent(PointerState.Down);
        public void OnPointerEnter(PointerEventData eventData) => AnalyticsUIEvent(PointerState.Enter);
        public void OnPointerExit(PointerEventData eventData) => AnalyticsUIEvent(PointerState.Exit);
        public void OnPointerUp(PointerEventData eventData) => AnalyticsUIEvent(PointerState.Up);
    }
