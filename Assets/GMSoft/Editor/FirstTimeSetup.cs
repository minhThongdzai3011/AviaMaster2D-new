using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace GMSoft.Editor
{
    [InitializeOnLoad]
    public class FirstTimeSetup
    {
        private const string FIRST_TIME_KEY_PREFIX = "GMSoft_SDK_FirstTime_v";
        private const string VERSION_KEY_PREFIX = "GMSoft_SDK_Version_";

        private static string CURRENT_VERSION => GMSDKConfig.Instance.SDKVersion;

        static FirstTimeSetup()
        {
            EditorApplication.delayCall += CheckFirstTimeSetup;
        }

        private static string GetProjectSpecificKey(string baseKey)
        {
            // Use project path to create unique key for each project
            string projectPath = Application.dataPath;
            string projectHash = projectPath.GetHashCode().ToString();
            return baseKey + projectHash;
        }

        private static void CheckFirstTimeSetup()
        {
            // Get project-specific keys
            string firstTimeKey = GetProjectSpecificKey(FIRST_TIME_KEY_PREFIX + CURRENT_VERSION + "_");
            string versionKey = GetProjectSpecificKey(VERSION_KEY_PREFIX);
            
            bool isFirstTime = !EditorPrefs.HasKey(firstTimeKey);
            string lastVersion = EditorPrefs.GetString(versionKey, "");
            
            // Check if this is first time import or version upgrade for this specific project
            if (isFirstTime || lastVersion != CURRENT_VERSION)
            {
                Debug.Log($"<b><color=green>[{GMSDKConfig.Instance.SDKName}]</color></b> First time setup detected for project: {Application.productName}");
                
                // Clear all previous data
                ClearPreviousData();
                
                // Mark as initialized for this specific project
                EditorPrefs.SetBool(firstTimeKey, true);
                EditorPrefs.SetString(versionKey, CURRENT_VERSION);
                
                // Show changelog window after a brief delay
                EditorApplication.delayCall += () => {
                    if (ShouldShowChangelogWindow())
                    {
                        ChangelogWindow.ShowWelcomeWindow();
                        Debug.Log($"<b><color=green>[{GMSDKConfig.Instance.SDKName}]</color></b> Welcome to {GMSDKConfig.Instance.DisplayVersionString}! Project: {Application.productName}");
                    }
                };
            }
        }

        private static bool ShouldShowChangelogWindow()
        {
            // Don't show in batch mode
            if (Application.isBatchMode)
                return false;
                
            // Don't show if Unity is still starting up
            if (EditorApplication.timeSinceStartup < 3f)
                return false;
                
            // Check if we're in play mode or compiling
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
                return false;
                
            return true;
        }

        private static void ClearPreviousData()
        {
            try
            {
                // Clear PublishWindow settings
                ClearPublishWindowData();
                
                // Clear BuildSizeChart history
                ClearBuildSizeHistory();
                
                // Clear any other SDK-related EditorPrefs
                ClearSDKEditorPrefs();
                
                Debug.Log($"<b><color=green>[{GMSDKConfig.Instance.SDKName}]</color></b> Previous data cleared successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"<b><color=green>[{GMSDKConfig.Instance.SDKName}]</color></b> Error clearing previous data: {ex.Message}");
            }
        }

        private static void ClearPublishWindowData()
        {
            // Use the PublishWindow's built-in clear method
            PublishWindow.ClearAllData();
        }

        private static void ClearBuildSizeHistory()
        {
            string historyKey = GetBuildSizeHistoryKey();
            if (EditorPrefs.HasKey(historyKey))
            {
                try
                {
                    EditorPrefs.DeleteKey(historyKey);
                    Debug.Log($"<b><color=green>[{GMSDKConfig.Instance.SDKName}]</color></b> Cleared build size history from EditorPrefs.");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"<b><color=green>[{GMSDKConfig.Instance.SDKName}]</color></b> Could not delete build history from EditorPrefs: {ex.Message}");
                }
            }
            
            // Also delete legacy JSON file if it exists
            string legacyFilePath = Path.Combine(Application.dataPath, "GMSoft/Editor/BuildSizeHistory.json");
            if (File.Exists(legacyFilePath))
            {
                try
                {
                    File.Delete(legacyFilePath);
                    Debug.Log($"<b><color=green>[{GMSDKConfig.Instance.SDKName}]</color></b> Cleared legacy build size history file.");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"<b><color=green>[{GMSDKConfig.Instance.SDKName}]</color></b> Could not delete legacy build history file: {ex.Message}");
                }
            }
        }

        private static void ClearSDKEditorPrefs()
        {
            // Clear any other GM Soft SDK related EditorPrefs
            string[] sdkKeys = {
                "GMSoft_LastChangelogVersion",
                "GMSoft_ChangelogShown",
                "BuildSizeChart_WindowPos",
                "BuildSizeChart_ShowDetails"
            };

            foreach (string key in sdkKeys)
            {
                if (EditorPrefs.HasKey(key))
                {
                    EditorPrefs.DeleteKey(key);
                }
            }
        }

        private static string GetBuildSizeHistoryKey()
        {
            // Generate a project-specific key based on project path
            string projectPath = Application.dataPath;
            string projectHash = projectPath.GetHashCode().ToString();
            return $"GMSoft_{PlayerSettings.productName}_{projectHash}_build_size_history";
        }

        // Menu item removed as requested
        private static void ManualResetSDKData()
        {
            if (EditorUtility.DisplayDialog("Reset GM Soft SDK Data", 
                "This will clear all GM Soft SDK data including:\n\n" +
                "• Build history\n" +
                "• Publisher settings\n" +
                "• Window preferences\n" +
                "• All cached data\n\n" +
                "Are you sure you want to continue?", 
                "Reset All Data", "Cancel"))
            {
                ClearPreviousData();
                
                // Also clear the first time flags to trigger setup again for this project
                string firstTimeKey = GetProjectSpecificKey(FIRST_TIME_KEY_PREFIX);
                string versionKey = GetProjectSpecificKey(VERSION_KEY_PREFIX);
                
                EditorPrefs.DeleteKey(firstTimeKey);
                EditorPrefs.DeleteKey(versionKey);
                
                Debug.Log($"<b><color=green>[{GMSDKConfig.Instance.SDKName}]</color></b> All SDK data has been reset for project: {Application.productName}");
                
                if (EditorUtility.DisplayDialog("Reset Complete", 
                    "All GM Soft SDK data has been reset.\n\nWould you like to restart Unity to complete the reset?", 
                    "Restart Unity", "Later"))
                {
                    EditorApplication.OpenProject(Directory.GetCurrentDirectory());
                }
            }
        }

        // Menu item removed as requested
        private static void ShowWelcomeWindow()
        {
            ChangelogWindow.ShowWelcomeWindow();
        }
    }
}
