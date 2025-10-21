using System;
using System.Collections.Generic;
using UnityEngine;

namespace GMSoft.Editor
{
    [CreateAssetMenu(fileName = "GMSoft_Changelog", menuName = "GMSoft/Changelog Data", order = 1)]
    public class ChangelogScriptableObject : ScriptableObject
    {
        [Header("Repository Information")]
        public string repositoryUrl = "http://123.24.143.6:8080/scm/git/gmsoft-sdk";
        public string branchName = "gmsoft-sdk-v7";
        
        [Header("Changelog Entries")]
        [SerializeField]
        public List<ChangelogEntryData> entries = new List<ChangelogEntryData>();
        
        [System.Serializable]
        public class ChangelogEntryData
        {
            [Header("Version Information")]
            public string version = "1.0.0";
            public string date = "2025-08-22";
            
            [Header("Description")]
            [TextArea(3, 8)]
            public string description = "Brief description of this release";
        }
        
        // Helper methods
        public ChangelogEntryData GetLatestEntry()
        {
            if (entries.Count == 0) return null;
            
            ChangelogEntryData latest = entries[0];
            foreach (var entry in entries)
            {
                if (CompareVersions(entry.version, latest.version) > 0)
                {
                    latest = entry;
                }
            }
            return latest;
        }
        
        public List<ChangelogEntryData> GetSortedEntries()
        {
            var sorted = new List<ChangelogEntryData>(entries);
            sorted.Sort((a, b) => CompareVersions(b.version, a.version)); // Descending order
            return sorted;
        }
        
        private int CompareVersions(string version1, string version2)
        {
            try
            {
                var v1 = new Version(version1);
                var v2 = new Version(version2);
                return v1.CompareTo(v2);
            }
            catch
            {
                return string.Compare(version1, version2, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
    
    // Helper class to convert between ScriptableObject and display format
    public static class ChangelogConverter
    {
        public static ChangelogData ConvertFromScriptableObject(ChangelogScriptableObject scriptableObject)
        {
            if (scriptableObject == null) return new ChangelogData();
            
            var data = new ChangelogData();
            
            foreach (var entry in scriptableObject.entries)
            {
                var changelogEntry = new ChangelogEntry
                {
                    version = entry.version,
                    date = entry.date,
                    title = "",
                    description = entry.description
                };
                
                data.entries.Add(changelogEntry);
            }
            
            return data;
        }
    }
}
