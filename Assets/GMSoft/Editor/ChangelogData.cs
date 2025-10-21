#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace GMSoft.Editor
{
    [Serializable]
    public class ChangelogEntry
    {
        public string version;
        public string date;
        public string title;
        public string description;
    }

    [Serializable]
    public class ChangelogData
    {
        public List<ChangelogEntry> entries = new List<ChangelogEntry>();
    }

    [Serializable]
    public class ChangelogSettings
    {
        public string gitUrl = "http://123.24.143.6:8080/scm/git/gmsoft-sdk";
        public string branch = "gmsoft-sdk-v7";
        public bool hasShownWelcome = false;
        public string lastViewedVersion = "";
        public string changelogAssetPath = "Assets/GMSoft/Editor/GMSoft_Changelog.asset";
    }
}
#endif
