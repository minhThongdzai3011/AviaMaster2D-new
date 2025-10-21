using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public partial class LifeCycleEventTracker : MonoBehaviour
    {
        public enum LifeCycle
        {
            None = 0,
            Awake = 1 << 0,
            Start = 1 << 1,
            Enable = 1 << 2,
            Disable = 1 << 3,
            Pause = 1 << 4,
            Unpause = 1 << 5,
            Destroy = 1 << 6
        }

        public List<LifeCycleEvent> events;

        private LifeCycleEvent GetEvent(LifeCycle lifeCycle)
        {
            if (events == null) return null;
            return events.FirstOrDefault(e => e.lifeCycle == lifeCycle);
        }

        private void AnalyticLifeCycle(LifeCycle lifeCycle)
        {
            LifeCycleEvent @event = GetEvent(lifeCycle);
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

        private void Awake() => AnalyticLifeCycle(LifeCycle.Awake);
        private void Start() => AnalyticLifeCycle(LifeCycle.Start);
        private void OnEnable() => AnalyticLifeCycle(LifeCycle.Enable);
        private void OnDisable() => AnalyticLifeCycle(LifeCycle.Disable);
        private void OnApplicationPause(bool pause) => AnalyticLifeCycle(pause ? LifeCycle.Pause : LifeCycle.Unpause);
        private void OnDestroy() => AnalyticLifeCycle(LifeCycle.Destroy);
    }
