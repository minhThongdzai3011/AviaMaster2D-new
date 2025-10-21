using System.Collections.Generic;
public class AnalyticEvent
    {
        public int analyticsLevel = 2;
        public string name;
        public bool hasParams;
        public Dictionary<string, string> @params = new Dictionary<string, string>();
    }
