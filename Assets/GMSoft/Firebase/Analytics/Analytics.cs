using FirebaseWebGL.Scripts.FirebaseAnalytics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Firebase
{
    public class Analytics
    {
        public static int level = 9999;
        public static bool enable = true;

        public static void SetUserProperties(string props)
        {

#if !UNITY_EDITOR
            FirebaseAnalytics.SetUserProperties(props);
#endif
        }

        public static void LogEvent(string eventName, int level = 0)
        {
            if (!enable) return;
#if !UNITY_EDITOR
            if (Analytics.level >= level)
            {
                FirebaseAnalytics.LogEvent(eventName);
            }
#endif
        }

        public static void LogEventParameter(string eventName, string eventParam, int level = 0)
        {
            if (!enable) return;
#if !UNITY_EDITOR
            if (Analytics.level >= level)
            {
                FirebaseAnalytics.LogEventParameter(eventName, eventParam);
            }
#endif
        }

        public static void LogEventParameter(string eventName, Dictionary<string, object> eventParamDict, int level = 0)
        {
            if (!enable) return;
#if !UNITY_EDITOR
            string eventParam = JsonConvert.SerializeObject(eventParamDict);
            if (Analytics.level >= level)
            {
                FirebaseAnalytics.LogEventParameter(eventName, eventParam);
            }
#endif
        }
    }
}
