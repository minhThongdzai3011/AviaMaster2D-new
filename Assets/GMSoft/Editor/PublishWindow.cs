#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.Callbacks;
using UnityEditor.Build.Reporting;
using Newtonsoft.Json;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace GMSoft.Editor
{
    /// <summary>
    /// Fast Build Tool - A Unity editor window for quick WebGL builds with publisher-specific configurations
    /// </summary>
    public class PublishWindow : EditorWindow
    {
        #region Enums and Data Classes

        public enum Publisher
        {
            None,
            AzGames,
            OneGames,
            CrazyGames
        }

        // Custom display names for Publisher enum
        private static readonly Dictionary<Publisher, string> PublisherDisplayNames = new Dictionary<Publisher, string>
        {
            { Publisher.None, "None" },
            { Publisher.AzGames, "azgames.io" },
            { Publisher.OneGames, "1games.io" },
            { Publisher.CrazyGames, "crazygames.com" }
        };

        [Flags]
        public enum BuildNameOption
        {
            HasProductName = 1,
            HasBuildVersion = 2
        }

        [Serializable]
        public class BuildStats
        {
            public string totalSize = "...";
            public string totalTime = "...";
            public string totalWarnings = "...";
            public string totalErrors = "...";

            public BuildStats() { }

            public BuildStats(string totalSize, string totalTime, string totalWarnings, string totalErrors)
            {
                this.totalSize = totalSize;
                this.totalTime = totalTime;
                this.totalWarnings = totalWarnings;
                this.totalErrors = totalErrors;
            }
        }

        [Serializable]
        public class PublishWindowSettings
        {
            public int publisher = (int)Publisher.None;
            public string localHost = "http://gamelocal.com";
            public string webglTemplate = "Default";
            public bool showBuildOutput = true;
            public bool previewWithDebug = false;
            public BuildStats buildStats = new BuildStats();
            public string lastBuildFileName = "";
            public string selectedPath = "";
            public string lastBuildTime = "";
            public int dayBuildNumber = 1;

            public PublishWindowSettings() { }
        }

        #endregion

        #region Fields

        // Configuration
        public Publisher publisher;
        public string companyName;
        public string productName;
        public BuildNameOption buildNameOption = BuildNameOption.HasBuildVersion;
        public string separator = "_";
        public string webglTemplate = "Default";
        public string localHost = "http://gamelocal.com";

        // Build Stats
        public BuildStats buildStats = new BuildStats();

        // Private fields
        private Vector2 scrollPosition;
        private bool showConfiguration = true;
        private bool showBuildInfo = true;
        private bool showBuildOutput = true;
        private bool previewWithDebug = false;

        // GUI Styles
        private GUIStyle headerStyle;
        private GUIStyle sectionHeaderStyle;
        private GUIStyle buttonStyle;
        private GUIStyle infoBoxStyle;
        private bool stylesInitialized = false;

        // Build state
        private static bool isCurrentlyBuilding = false;
        private string buildStatusMessage = "";

        // File-based storage
        private PublishWindowSettings settings = new PublishWindowSettings();

        // Build Analysis Results
        [Serializable]
        public class BuildAnalysisResult
        {
            public bool hasGMSoftInScenes;
            public List<string> scenesWithGMSoft = new List<string>();
            public List<string> scenesWithoutGMSoft = new List<string>();
            
            public bool hasGamePlayStart;
            public bool hasGamePlayStop;
            public bool hasGamePlayPause;
            public bool hasGamePlayResume;
            public bool hasGamePlayAgain;
            public List<string> gamePlayMethodUsages = new List<string>();
            
            public string compressionFormat;
            public string decompressionFallback;
            
            public bool hasCopyrightText;
            public List<string> scenesWithCopyright = new List<string>();
            public List<string> scenesWithoutCopyright = new List<string>();
            
            public bool hasMoreGamesButton;
            public List<string> scenesWithMoreGamesButton = new List<string>();
            public List<string> scenesWithoutMoreGamesButton = new List<string>();
            
            // Asset breakdown data for circular chart
            public Dictionary<string, long> assetSizes = new Dictionary<string, long>();
            public long totalBuildSize = 0;
            
            // Build report status
            public string buildReportStatus = "No build data available";
            public bool hasBuildReportData = false;
            public string lastBuildReportSource = "None";
        }
        
        private BuildAnalysisResult buildAnalysisResult = new BuildAnalysisResult();

        #endregion

        #region File Storage Methods

        /// <summary>
        /// Get project-specific key for EditorPrefs
        /// </summary>
        private static string GetProjectKey(string baseKey)
        {
            // Use project path to create unique key for each project
            string projectPath = Application.dataPath;
            string projectHash = projectPath.GetHashCode().ToString();
            string productName = GetProductName();
            return $"{productName}_{baseKey}_{projectHash}";
        }

        /// <summary>
        /// Clear all PublishWindow data - used by FirstTimeSetup
        /// </summary>
        public static void ClearAllData()
        {
            try
            {
                // Delete any JSON files if they exist (legacy)
                string folderPath = Application.dataPath + "/GMSoft/Editor";
                if (Directory.Exists(folderPath))
                {
                    string[] filesToDelete = {
                        Path.Combine(folderPath, "PublishWindowSettings.json"),
                        Path.Combine(folderPath, "BuildSizeHistory.json")
                    };
                    
                    foreach (string file in filesToDelete)
                    {
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                            Debug.Log($"<b><color=green>[GMSoft SDK]</color></b> Deleted legacy file: {file}");
                        }
                    }
                }
                
                // Clear EditorPrefs for this project
                string productName = GetProductName();
                if (!string.IsNullOrEmpty(productName))
                {
                    string projectPath = Application.dataPath;
                    string projectHash = projectPath.GetHashCode().ToString();
                    
                    // Find and delete all EditorPrefs containing the project hash
                    string[] baseKeys = {
                        "publisher",
                        "build_local_host", 
                        "webgl_template",
                        "show_log",
                        "build_stats",
                        "last_build_file_name",
                        "selected_path",
                        "last_build_time",
                        "day_build_number",
                        "build_size_history",
                        "window_pos"
                    };
                    
                    foreach (string baseKey in baseKeys)
                    {
                        string key = GetProjectKey(baseKey);
                        if (EditorPrefs.HasKey(key))
                        {
                            EditorPrefs.DeleteKey(key);
                        }
                    }
                }
                
                Debug.Log("[PublishWindow] All data cleared successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[PublishWindow] Error clearing data: {ex.Message}");
            }
        }

        /// <summary>
        /// Lưu các cài đặt hiện tại vào EditorPrefs
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                // Update settings object with current values
                settings.publisher = (int)publisher;
                settings.localHost = localHost;
                settings.webglTemplate = webglTemplate;
                settings.showBuildOutput = showBuildOutput;
                settings.previewWithDebug = previewWithDebug;
                settings.buildStats = buildStats;
                settings.lastBuildFileName = GetLastBuildFileName();
                settings.selectedPath = GetSelectedPath();
                settings.lastBuildTime = HasBuildBefore() ? GetLastBuildTime().ToString() : "";
                settings.dayBuildNumber = GetDayBuildNumber();

                // Save settings to EditorPrefs
                EditorPrefs.SetInt(GetProjectKey("publisher"), (int)publisher);
                EditorPrefs.SetString(GetProjectKey("build_local_host"), localHost);
                EditorPrefs.SetString(GetProjectKey("webgl_template"), webglTemplate);
                EditorPrefs.SetBool(GetProjectKey("show_log"), showBuildOutput);
                EditorPrefs.SetBool(GetProjectKey("preview_with_debug"), previewWithDebug);
                EditorPrefs.SetString(GetProjectKey("last_build_file_name"), GetLastBuildFileName());
                EditorPrefs.SetString(GetProjectKey("selected_path"), GetSelectedPath());
                
                if (HasBuildBefore())
                {
                    EditorPrefs.SetString(GetProjectKey("last_build_time"), GetLastBuildTime().ToString());
                }
                
                EditorPrefs.SetInt(GetProjectKey("day_build_number"), GetDayBuildNumber());
                
                // Save build stats as JSON string
                string statsJson = JsonConvert.SerializeObject(buildStats);
                EditorPrefs.SetString(GetProjectKey("build_stats"), statsJson);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to save PublishWindow settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Tải cài đặt từ EditorPrefs hoặc migrate từ file nếu cần
        /// </summary>
        private void LoadSettings()
        {
            // Initialize basic product info
            companyName = GetCompanyName();
            productName = GetProductName();
            
            // First check for any legacy JSON file to migrate
            string folderPath = Application.dataPath + "/GMSoft/Editor";
            string oldFilePath = Path.Combine(folderPath, "PublishWindowSettings.json");
            
            // Try to migrate from JSON if exists
            if (System.IO.File.Exists(oldFilePath))
            {
                try
                {
                    string json = System.IO.File.ReadAllText(oldFilePath);
                    if (!string.IsNullOrEmpty(json))
                    {
                        settings = JsonConvert.DeserializeObject<PublishWindowSettings>(json) ?? new PublishWindowSettings();
                        
                        // Save to EditorPrefs
                        EditorPrefs.SetInt(GetProjectKey("publisher"), settings.publisher);
                        EditorPrefs.SetString(GetProjectKey("build_local_host"), settings.localHost);
                        EditorPrefs.SetString(GetProjectKey("webgl_template"), settings.webglTemplate);
                        EditorPrefs.SetBool(GetProjectKey("show_log"), settings.showBuildOutput);
                        EditorPrefs.SetBool(GetProjectKey("preview_with_debug"), settings.previewWithDebug);
                        EditorPrefs.SetString(GetProjectKey("last_build_file_name"), settings.lastBuildFileName);
                        EditorPrefs.SetString(GetProjectKey("selected_path"), settings.selectedPath);
                        EditorPrefs.SetString(GetProjectKey("last_build_time"), settings.lastBuildTime);
                        EditorPrefs.SetInt(GetProjectKey("day_build_number"), settings.dayBuildNumber);
                        
                        // Save build stats as JSON string
                        string statsJson = JsonConvert.SerializeObject(settings.buildStats);
                        EditorPrefs.SetString(GetProjectKey("build_stats"), statsJson);
                        
                        Debug.Log($"<b><color=green>[GMSoft SDK]</color></b> Migrated settings from JSON file to EditorPrefs");
                        
                        // Delete old JSON file
                        File.Delete(oldFilePath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to migrate settings from JSON: {ex.Message}");
                }
            }

            // Load from EditorPrefs
            settings = new PublishWindowSettings();
            
            // Apply settings to current fields
            if (EditorPrefs.HasKey(GetProjectKey("publisher")))
            {
                publisher = (Publisher)EditorPrefs.GetInt(GetProjectKey("publisher"), 0);
                settings.publisher = (int)publisher;
            }
            
            if (EditorPrefs.HasKey(GetProjectKey("build_local_host")))
            {
                localHost = EditorPrefs.GetString(GetProjectKey("build_local_host"), "http://gamelocal.com");
                settings.localHost = localHost;
            }
            
            if (EditorPrefs.HasKey(GetProjectKey("webgl_template")))
            {
                webglTemplate = EditorPrefs.GetString(GetProjectKey("webgl_template"), "Default");
                settings.webglTemplate = webglTemplate;
            }
            else
            {
                // First time setup - check for default template
                string defaultTemplate = GetDefaultWebGLTemplate();
                if (defaultTemplate != "Default")
                {
                    webglTemplate = defaultTemplate;
                    settings.webglTemplate = defaultTemplate;
                }
            }
            
            if (EditorPrefs.HasKey(GetProjectKey("show_log")))
            {
                showBuildOutput = EditorPrefs.GetBool(GetProjectKey("show_log"), true);
                settings.showBuildOutput = showBuildOutput;
            }
            
            if (EditorPrefs.HasKey(GetProjectKey("preview_with_debug")))
            {
                previewWithDebug = EditorPrefs.GetBool(GetProjectKey("preview_with_debug"), false);
                settings.previewWithDebug = previewWithDebug;
            }
            
            if (EditorPrefs.HasKey(GetProjectKey("build_stats")))
            {
                string statsJson = EditorPrefs.GetString(GetProjectKey("build_stats"), "");
                if (!string.IsNullOrEmpty(statsJson))
                {
                    try
                    {
                        buildStats = JsonConvert.DeserializeObject<BuildStats>(statsJson) ?? new BuildStats();
                        settings.buildStats = buildStats;
                    }
                    catch
                    {
                        buildStats = new BuildStats();
                        settings.buildStats = buildStats;
                    }
                }
            }
            
            // Load day build number
            if (EditorPrefs.HasKey(GetProjectKey("day_build_number")))
            {
                settings.dayBuildNumber = EditorPrefs.GetInt(GetProjectKey("day_build_number"), 1);
            }
            
            // Load other settings that are accessed through getters
            if (EditorPrefs.HasKey(GetProjectKey("last_build_file_name")))
            {
                settings.lastBuildFileName = EditorPrefs.GetString(GetProjectKey("last_build_file_name"), "");
            }
            
            if (EditorPrefs.HasKey(GetProjectKey("selected_path")))
            {
                settings.selectedPath = EditorPrefs.GetString(GetProjectKey("selected_path"), "");
            }
            
            if (EditorPrefs.HasKey(GetProjectKey("last_build_time")))
            {
                settings.lastBuildTime = EditorPrefs.GetString(GetProjectKey("last_build_time"), "");
            }
        }

        private void MigrateFromEditorPrefs()
        {
            try
            {
                string productName = GetProductName();

                // Check if any old EditorPrefs data exists
                if (EditorPrefs.HasKey($"{productName}_publisher") ||
                    EditorPrefs.HasKey($"{productName}_build_local_host") ||
                    EditorPrefs.HasKey($"{productName}_webgl_template") ||
                    EditorPrefs.HasKey($"{productName}_build_stats"))
                {
                    
                    // Migrate all the old data
                    settings.publisher = EditorPrefs.GetInt($"{productName}_publisher", (int)Publisher.None);
                    settings.localHost = EditorPrefs.GetString($"{productName}_build_local_host", "http://gamelocal.com");
                    settings.webglTemplate = EditorPrefs.GetString($"{productName}_webgl_template", "Default");
                    settings.showBuildOutput = EditorPrefs.GetBool($"{productName}_show_log", true);
                    settings.lastBuildFileName = EditorPrefs.GetString($"{productName}_last_build_file_name", "");
                    settings.selectedPath = EditorPrefs.GetString($"{productName}_selected_path", "");
                    
                    // Migrate build stats
                    string buildData = EditorPrefs.GetString($"{productName}_build_stats", "");
                    if (!string.IsNullOrEmpty(buildData))
                    {
                        try
                        {
                            settings.buildStats = JsonConvert.DeserializeObject<BuildStats>(buildData) ?? new BuildStats();
                        }
                        catch
                        {
                            settings.buildStats = new BuildStats();
                        }
                    }

                    // Migrate build time data
                    string lastBuildTimeKey = $"{productName}_last_build_time";
                    string dayBuildNumberKey = $"{productName}_day_build_number";
                    
                    if (EditorPrefs.HasKey(lastBuildTimeKey))
                    {
                        settings.lastBuildTime = EditorPrefs.GetString(lastBuildTimeKey, "");
                    }
                    
                    if (EditorPrefs.HasKey(dayBuildNumberKey))
                    {
                        settings.dayBuildNumber = EditorPrefs.GetInt(dayBuildNumberKey, 1);
                    }

                    // Save migrated data to EditorPrefs instead of file
                    EditorPrefs.SetInt(GetProjectKey("publisher"), settings.publisher);
                    EditorPrefs.SetString(GetProjectKey("build_local_host"), settings.localHost);
                    EditorPrefs.SetString(GetProjectKey("webgl_template"), settings.webglTemplate);
                    EditorPrefs.SetBool(GetProjectKey("show_log"), settings.showBuildOutput);
                    EditorPrefs.SetBool(GetProjectKey("preview_with_debug"), settings.previewWithDebug);
                    EditorPrefs.SetString(GetProjectKey("last_build_file_name"), settings.lastBuildFileName);
                    EditorPrefs.SetString(GetProjectKey("selected_path"), settings.selectedPath);
                    EditorPrefs.SetString(GetProjectKey("last_build_time"), settings.lastBuildTime);
                    EditorPrefs.SetInt(GetProjectKey("day_build_number"), settings.dayBuildNumber);
                    
                    // Save build stats as JSON string
                    string statsJson = JsonConvert.SerializeObject(settings.buildStats);
                    EditorPrefs.SetString(GetProjectKey("build_stats"), statsJson);

                    Debug.Log($"Migrated PublishWindow settings from old EditorPrefs to new project-specific EditorPrefs");

                    // Clear old EditorPrefs data
                    EditorPrefs.DeleteKey($"{productName}_publisher");
                    EditorPrefs.DeleteKey($"{productName}_build_local_host");
                    EditorPrefs.DeleteKey($"{productName}_webgl_template");
                    EditorPrefs.DeleteKey($"{productName}_show_log");
                    EditorPrefs.DeleteKey($"{productName}_build_stats");
                    EditorPrefs.DeleteKey($"{productName}_last_build_file_name");
                    EditorPrefs.DeleteKey($"{productName}_selected_path");
                    EditorPrefs.DeleteKey(lastBuildTimeKey);
                    EditorPrefs.DeleteKey(dayBuildNumberKey);
                    
                    Debug.Log("Cleared old PublishWindow EditorPrefs data");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to migrate PublishWindow settings from EditorPrefs: {ex.Message}");
            }
        }

        #endregion

        #region Requirement Checking Methods

        /// <summary>
        /// Performs all build analysis including asset breakdown
        /// </summary>
        private void PerformBuildAnalysis()
        {
            try
            {
                buildAnalysisResult = new BuildAnalysisResult();
                
                // Check scenes for GMSoft GameObject
                CheckScenesForGMSoft();
                
                // Check copyright text in scenes
                CheckCopyrightText();
                
                // Check MoreGamesButton in scenes
                CheckScenesForMoreGamesButton();
                
                // Check GamePlay method usage
                CheckGamePlayMethodUsage();
                
                // Check compression settings
                CheckCompressionSettings();
                
                // Analyze asset sizes for circular chart
                AnalyzeAssetSizes();
                
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error performing build analysis: {ex.Message}");
                // Initialize with default values to prevent null reference
                if (buildAnalysisResult == null)
                {
                    buildAnalysisResult = new BuildAnalysisResult();
                    // Ensure all lists are initialized
                    buildAnalysisResult.scenesWithGMSoft = new List<string>();
                    buildAnalysisResult.scenesWithoutGMSoft = new List<string>();
                    buildAnalysisResult.gamePlayMethodUsages = new List<string>();
                    buildAnalysisResult.scenesWithCopyright = new List<string>();
                    buildAnalysisResult.scenesWithoutCopyright = new List<string>();
                    buildAnalysisResult.scenesWithMoreGamesButton = new List<string>();
                    buildAnalysisResult.scenesWithoutMoreGamesButton = new List<string>();
                    buildAnalysisResult.assetSizes = new Dictionary<string, long>();
                }
            }
        }

        /// <summary>
        /// Check if all scenes have at least one GMSoft GameObject
        /// </summary>
        private void CheckScenesForGMSoft()
        {
            try
            {
                buildAnalysisResult.scenesWithGMSoft.Clear();
                buildAnalysisResult.scenesWithoutGMSoft.Clear();
                
                // Create a copy of the scenes array to avoid collection modification issues
                var scenesToCheck = EditorBuildSettings.scenes.ToArray();
                
                foreach (var scene in scenesToCheck)
                {
                    if (!scene.enabled) continue;
                    
                    try
                    {
                        string scenePath = scene.path;
                        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                        bool foundGMSoft = false;
                        
                        // Check if scene contains GMSoft GameObject by reading the scene file
                        if (System.IO.File.Exists(scenePath))
                        {
                            string sceneContent = System.IO.File.ReadAllText(scenePath);
                            // Check for GMSDK component or GMSoft named GameObject
                            if (sceneContent.Contains("GMSDK") || sceneContent.Contains("GMSoft") || sceneContent.Contains("GmSoft"))
                            {
                                foundGMSoft = true;
                            }
                        }
                        
                        if (foundGMSoft)
                        {
                            buildAnalysisResult.scenesWithGMSoft.Add(sceneName);
                        }
                        else
                        {
                            buildAnalysisResult.scenesWithoutGMSoft.Add(sceneName);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"Error checking scene {scene.path}: {ex.Message}");
                    }
                }
                
                buildAnalysisResult.hasGMSoftInScenes = buildAnalysisResult.scenesWithGMSoft.Count > 0;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error in CheckScenesForGMSoft: {ex.Message}");
            }
        }

        /// <summary>
        /// Check if GamePlay methods are being called in the project (not just declared)
        /// </summary>
        private void CheckGamePlayMethodUsage()
        {
            try
            {
                buildAnalysisResult.gamePlayMethodUsages.Clear();
                
                // Search for GamePlay method calls in all scripts (excluding GMSDK.cs itself)
                string[] allScripts = AssetDatabase.FindAssets("t:Script", new[] { "Assets" });
                
                foreach (string guid in allScripts)
                {
                    try
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        if (path.EndsWith(".cs") && !path.Contains("GMSDK.cs") && !path.Contains("GameModuleBase.cs"))
                        {
                            if (System.IO.File.Exists(path))
                            {
                                string content = System.IO.File.ReadAllText(path);
                                string fileName = System.IO.Path.GetFileName(path);
                                
                                // Use more specific regex patterns to find actual method calls
                                // Check for GamePlayStart calls
                                if (System.Text.RegularExpressions.Regex.IsMatch(content, @"\.GamePlayStart\s*\(\s*\)"))
                                {
                                    buildAnalysisResult.hasGamePlayStart = true;
                                    if (!buildAnalysisResult.gamePlayMethodUsages.Any(x => x.Contains("GamePlayStart")))
                                        buildAnalysisResult.gamePlayMethodUsages.Add($"GamePlayStart called in {fileName}");
                                }
                                
                                // Check for GamePlayStop calls
                                if (System.Text.RegularExpressions.Regex.IsMatch(content, @"\.GamePlayStop\s*\(\s*\)"))
                                {
                                    buildAnalysisResult.hasGamePlayStop = true;
                                    if (!buildAnalysisResult.gamePlayMethodUsages.Any(x => x.Contains("GamePlayStop")))
                                        buildAnalysisResult.gamePlayMethodUsages.Add($"GamePlayStop called in {fileName}");
                                }
                                
                                // Check for GamePlayPause calls
                                if (System.Text.RegularExpressions.Regex.IsMatch(content, @"\.GamePlayPause\s*\(\s*\)"))
                                {
                                    buildAnalysisResult.hasGamePlayPause = true;
                                    if (!buildAnalysisResult.gamePlayMethodUsages.Any(x => x.Contains("GamePlayPause")))
                                        buildAnalysisResult.gamePlayMethodUsages.Add($"GamePlayPause called in {fileName}");
                                }
                                
                                // Check for GamePlayResume calls
                                if (System.Text.RegularExpressions.Regex.IsMatch(content, @"\.GamePlayResume\s*\(\s*\)"))
                                {
                                    buildAnalysisResult.hasGamePlayResume = true;
                                    if (!buildAnalysisResult.gamePlayMethodUsages.Any(x => x.Contains("GamePlayResume")))
                                        buildAnalysisResult.gamePlayMethodUsages.Add($"GamePlayResume called in {fileName}");
                                }
                                
                                // Check for GamePlayAgain calls
                                if (System.Text.RegularExpressions.Regex.IsMatch(content, @"\.GamePlayAgain\s*\(\s*\)"))
                                {
                                    buildAnalysisResult.hasGamePlayAgain = true;
                                    if (!buildAnalysisResult.gamePlayMethodUsages.Any(x => x.Contains("GamePlayAgain")))
                                        buildAnalysisResult.gamePlayMethodUsages.Add($"GamePlayAgain called in {fileName}");
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"Error checking file {guid}: {ex.Message}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error in CheckGamePlayMethodUsage: {ex.Message}");
            }
        }

        /// <summary>
        /// Check WebGL compression settings
        /// </summary>
        private void CheckCompressionSettings()
        {
            try
            {
                buildAnalysisResult.compressionFormat = PlayerSettings.WebGL.compressionFormat.ToString();
                buildAnalysisResult.decompressionFallback = PlayerSettings.WebGL.decompressionFallback.ToString();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Error checking compression settings: {ex.Message}");
                buildAnalysisResult.compressionFormat = "Unknown";
                buildAnalysisResult.decompressionFallback = "Unknown";
            }
        }

        /// <summary>
        /// Check if all scenes contain copyright text "\xA9 2025 Azgames.io"
        /// </summary>
        private void CheckCopyrightText()
        {
            try
            {
                // Ensure lists are initialized
                if (buildAnalysisResult.scenesWithCopyright == null)
                    buildAnalysisResult.scenesWithCopyright = new List<string>();
                if (buildAnalysisResult.scenesWithoutCopyright == null)
                    buildAnalysisResult.scenesWithoutCopyright = new List<string>();
                    
                buildAnalysisResult.scenesWithCopyright.Clear();
                buildAnalysisResult.scenesWithoutCopyright.Clear();
                
                string copyrightText = "xA9 2025 Azgames.io";
                
                // Create a copy of the scenes array to avoid collection modification issues
                var scenesToCheck = EditorBuildSettings.scenes.ToArray();
                
                foreach (var scene in scenesToCheck)
                {
                    if (!scene.enabled) continue;
                    
                    try
                    {
                        string scenePath = scene.path;
                        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                        bool foundCopyright = false;
                        
                        // Check if scene contains copyright text by reading the scene file
                        if (System.IO.File.Exists(scenePath))
                        {
                            string sceneContent = System.IO.File.ReadAllText(scenePath);
                            
                            // Check for copyright text in various text components
                            // Look for Text components (Legacy UI)
                            if (sceneContent.Contains("m_Text:") && sceneContent.Contains(copyrightText))
                            {
                                foundCopyright = true;
                            }
                            // Look for TextMeshPro components
                            else if (sceneContent.Contains("m_text:") && sceneContent.Contains(copyrightText))
                            {
                                foundCopyright = true;
                            }
                            // Look for TextMeshProUGUI components
                            else if (sceneContent.Contains("TextMeshProUGUI") && sceneContent.Contains(copyrightText))
                            {
                                foundCopyright = true;
                            }
                            // General search for the copyright text
                            else if (sceneContent.Contains(copyrightText))
                            {
                                foundCopyright = true;
                            }
                        }
                        
                        if (foundCopyright)
                        {
                            buildAnalysisResult.scenesWithCopyright.Add(sceneName);
                        }
                        else
                        {
                            buildAnalysisResult.scenesWithoutCopyright.Add(sceneName);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"Error checking copyright in scene {scene.path}: {ex.Message}");
                        // Add to scenes without copyright if there's an error reading
                        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
                        buildAnalysisResult.scenesWithoutCopyright.Add(sceneName);
                    }
                }
                
                // Chỉ cần tìm thấy ít nhất 1 scene có copyright là đủ
                buildAnalysisResult.hasCopyrightText = buildAnalysisResult.scenesWithCopyright.Count > 0;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error in CheckCopyrightText: {ex.Message}");
                buildAnalysisResult.hasCopyrightText = false;
            }
        }
        
        /// <summary>
        /// Check if all scenes have at least one MoreGamesButton script
        /// </summary>
        private void CheckScenesForMoreGamesButton()
        {
            try
            {
                // Ensure lists are initialized
                if (buildAnalysisResult.scenesWithMoreGamesButton == null)
                    buildAnalysisResult.scenesWithMoreGamesButton = new List<string>();
                if (buildAnalysisResult.scenesWithoutMoreGamesButton == null)
                    buildAnalysisResult.scenesWithoutMoreGamesButton = new List<string>();
                    
                buildAnalysisResult.scenesWithMoreGamesButton.Clear();
                buildAnalysisResult.scenesWithoutMoreGamesButton.Clear();
                
                // Create a copy of the scenes array to avoid collection modification issues
                var scenesToCheck = EditorBuildSettings.scenes.ToArray();
                
                foreach (var scene in scenesToCheck)
                {
                    if (!scene.enabled) continue;
                    
                    try
                    {
                        string scenePath = scene.path;
                        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                        bool foundMoreGamesButton = false;
                        
                        // Check if scene contains MoreGamesButton script by reading the scene file
                        if (System.IO.File.Exists(scenePath))
                        {
                            string sceneContent = System.IO.File.ReadAllText(scenePath);
                            
                            // Check for MoreGamesButton script reference using GUID
                            // The GUID of MoreGamesButton.cs is: 78794efce45e0b240a21864589118799
                            string moreGamesButtonGuid = "78794efce45e0b240a21864589118799";
                            
                            // Look for the script component reference in the scene file
                            if (sceneContent.Contains(moreGamesButtonGuid))
                            {
                                foundMoreGamesButton = true;
                            }
                            
                            // Fallback: check for class name reference
                            if (!foundMoreGamesButton && sceneContent.Contains("MoreGamesButton"))
                            {
                                foundMoreGamesButton = true;
                            }
                        }
                        
                        if (foundMoreGamesButton)
                        {
                            buildAnalysisResult.scenesWithMoreGamesButton.Add(sceneName);
                        }
                        else
                        {
                            buildAnalysisResult.scenesWithoutMoreGamesButton.Add(sceneName);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"Error checking MoreGamesButton in scene {scene.path}: {ex.Message}");
                        // Add to scenes without MoreGamesButton if there's an error reading
                        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
                        buildAnalysisResult.scenesWithoutMoreGamesButton.Add(sceneName);
                    }
                }
                
                // At least one scene should have MoreGamesButton
                buildAnalysisResult.hasMoreGamesButton = buildAnalysisResult.scenesWithMoreGamesButton.Count > 0;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error in CheckScenesForMoreGamesButton: {ex.Message}");
                buildAnalysisResult.hasMoreGamesButton = false;
            }
        }
        
        /// <summary>
        /// Analyze asset sizes for the circular chart (fallback method)
        /// </summary>
        private void AnalyzeAssetSizes()
        {
            try
            {
                buildAnalysisResult.assetSizes.Clear();
                buildAnalysisResult.totalBuildSize = 0;
                
                // Get all assets in the project
                string[] allAssets = AssetDatabase.GetAllAssetPaths();
                
                // Categories for asset breakdown (using Unity format)
                Dictionary<string, long> categories = new Dictionary<string, long>
                {
                    {"Textures", 0},
                    {"Audio", 0},
                    {"Meshes", 0},
                    {"Scripts", 0},
                    {"Materials", 0},
                    {"Animation", 0},
                    {"Levels", 0},
                    {"Resources", 0},
                    {"StreamingAssets", 0},
                    {"Miscellaneous", 0}
                };
                
                int analyzedAssets = 0;
                
                foreach (string assetPath in allAssets)
                {
                    if (assetPath.StartsWith("Assets/"))
                    {
                        try
                        {
                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(assetPath);
                            if (fileInfo.Exists)
                            {
                                long fileSize = fileInfo.Length;
                                buildAnalysisResult.totalBuildSize += fileSize;
                                
                                string extension = System.IO.Path.GetExtension(assetPath).ToLower();
                                string category = GetUnityBuildReportCategory(assetPath, extension);
                                
                                categories[category] += fileSize;
                                analyzedAssets++;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogWarning($"Error analyzing asset {assetPath}: {ex.Message}");
                        }
                    }
                }
                
                buildAnalysisResult.assetSizes = categories;
                buildAnalysisResult.hasBuildReportData = false;
                
                if (analyzedAssets > 0)
                {
                    buildAnalysisResult.buildReportStatus = $"Estimated from {analyzedAssets} project assets. Build for accurate data.";
                }
                else
                {
                    buildAnalysisResult.buildReportStatus = "No assets found in project to analyze.";
                }
                
                Debug.Log($"[BuildAnalysis] Analyzed {analyzedAssets} project assets as fallback. Total estimated size: {BytesToString(buildAnalysisResult.totalBuildSize)}");
            }
            catch (System.Exception ex)
            {
                buildAnalysisResult.buildReportStatus = $"Error analyzing project assets: {ex.Message}";
                Debug.LogError($"Error in AnalyzeAssetSizes: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Initialize build analysis result with default values when no data is available
        /// </summary>
        private void InitializeEmptyBuildAnalysis()
        {
            buildAnalysisResult.assetSizes.Clear();
            buildAnalysisResult.totalBuildSize = 0;
            buildAnalysisResult.hasBuildReportData = false;
            buildAnalysisResult.lastBuildReportSource = "None";
            buildAnalysisResult.buildReportStatus = "No build has been performed yet. Run a WebGL build to see detailed asset breakdown.";
        }
        
        /// <summary>
        /// Get asset category based on file extension
        /// </summary>
        private string GetAssetCategory(string extension)
        {
            switch (extension)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".tga":
                case ".psd":
                case ".exr":
                case ".hdr":
                    return "Textures";
                    
                case ".wav":
                case ".mp3":
                case ".ogg":
                case ".aiff":
                case ".aac":
                    return "Audio";
                    
                case ".fbx":
                case ".obj":
                case ".dae":
                case ".3ds":
                case ".blend":
                case ".mesh":
                    return "Meshes";
                    
                case ".cs":
                case ".js":
                case ".dll":
                    return "Scripts";
                    
                case ".mat":
                case ".shader":
                case ".shadergraph":
                    return "Materials";
                    
                case ".anim":
                case ".controller":
                case ".overrideController":
                    return "Animations";
                    
                case ".unity":
                    return "Scenes";
                    
                default:
                    return "Other";
            }
        }


        #endregion

        #region Window Management

        [MenuItem("GMSoft/Fast Build Tool &#b")]
        private static void OpenWindow()
        {
            ShowWindow();
        }

        public static void ShowWindow()
        {
            var window = GetWindow<PublishWindow>();
            window.titleContent = new GUIContent()
            {
                text = GMSDKConfig.Instance.DisplayVersionString,
                image = EditorGUIUtility.IconContent("BuildSettings.WebGL").image
            };
            
            // Set default window size to 900x800
            Vector2 defaultSize = new Vector2(900, 800);
            window.position = new Rect(
                (Screen.currentResolution.width - defaultSize.x) / 2,
                (Screen.currentResolution.height - defaultSize.y) / 2,
                defaultSize.x,
                defaultSize.y
            );
            window.minSize = new Vector2(800, 600);
            
            // Check for day reset after window is created (window will load settings first)
            EditorApplication.delayCall += () =>
            {
                if (window != null && DateTime.Now.Date > window.GetLastBuildTime().Date)
                {
                    window.ResetDayBuildNumber();
                }
            };
        }

        // Đã bỏ không sử dụng biến static, thay vào đó sử dụng GetProjectKey() để đảm bảo nhất quán
        private static void InitializeKeys()
        {
            // Phương thức này đã không còn cần thiết, nhưng được giữ lại để tương thích
            // với mã hiện tại. Tất cả các tham chiếu đến các biến static đã được thay thế
            // bằng calls đến GetProjectKey()
        }

        private void OnEnable()
        {
            LoadSettings();
            isCurrentlyBuilding = false;
            
            // Initialize build analysis result to prevent null reference
            if (buildAnalysisResult == null)
            {
                buildAnalysisResult = new BuildAnalysisResult();
                // Ensure all lists are initialized
                buildAnalysisResult.scenesWithGMSoft = new List<string>();
                buildAnalysisResult.scenesWithoutGMSoft = new List<string>();
                buildAnalysisResult.gamePlayMethodUsages = new List<string>();
                buildAnalysisResult.scenesWithCopyright = new List<string>();
                buildAnalysisResult.scenesWithoutCopyright = new List<string>();
                buildAnalysisResult.scenesWithMoreGamesButton = new List<string>();
                buildAnalysisResult.scenesWithoutMoreGamesButton = new List<string>();
                buildAnalysisResult.assetSizes = new Dictionary<string, long>();
                
                // Initialize empty build analysis state
                InitializeEmptyBuildAnalysis();
            }
            
            // Perform initial build analysis with delay to avoid GUI conflicts
            EditorApplication.delayCall += () =>
            {
                if (this != null)
                {
                    try
                    {
                        PerformBuildAnalysis();
                        EditorApplication.delayCall += () =>
                        {
                            if (this != null)
                                Repaint();
                        };
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"Error performing initial build analysis: {ex.Message}");
                    }
                }
            };
        }

        private void OnDisable()
        {
            SaveSettings();
        }

        private void OnDestroy()
        {
            SaveSettings();
            isCurrentlyBuilding = false;
            
            // Cleanup compile progress tracking
            EditorApplication.update -= UpdateCompileProgress;
            EditorUtility.ClearProgressBar();
        }

        private void LoadSettingsFromOldMethods()
        {
            // This method is kept for compatibility but will be replaced by LoadSettings
            companyName = GetCompanyName();
            productName = GetProductName();
        }

        private void InitializeStyles()
        {
            if (stylesInitialized) return;

            headerStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 40, // increased 1.5x from 20
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black },
                padding = new RectOffset(0, 0, 5, 5),
                margin = new RectOffset(0, 0, 0, 0)
            };

            sectionHeaderStyle = new GUIStyle(EditorStyles.foldoutHeader)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 35
            };

            infoBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 11,
                padding = new RectOffset(10, 10, 10, 10)
            };

            stylesInitialized = true;
        }

        #endregion

        #region Custom GUI Controls

        /// <summary>
        /// Draw a requirement status line with status indicator (similar to Analytics Helper)
        /// </summary>
        private void DrawRequirementStatus(string label, bool isValid, string status)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Status indicator
            string statusIcon = isValid ? "✓" : "✗";
            Color statusColor = isValid ? Color.green : Color.red;
            
            var statusStyle = new GUIStyle(EditorStyles.boldLabel) 
            { 
                normal = { textColor = statusColor },
                fontSize = 14
            };
            EditorGUILayout.LabelField(statusIcon, statusStyle, GUILayout.Width(20));
            
            // Label
            EditorGUILayout.LabelField(label, GUILayout.Width(130));
            
            // Status text
            Color previousColor = GUI.color;
            GUI.color = isValid ? Color.green : Color.red;
            EditorGUILayout.LabelField(status, EditorStyles.boldLabel);
            GUI.color = previousColor;
            
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw a requirement status line with custom color coding (for warnings)
        /// </summary>
        private void DrawRequirementStatusWithColor(string label, Color statusColor, string status)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Status indicator with warning icon
            string statusIcon = statusColor == Color.yellow ? "⚠" : statusColor == Color.green ? "✓" : "✗";
            
            var statusStyle = new GUIStyle(EditorStyles.boldLabel) 
            { 
                normal = { textColor = statusColor },
                fontSize = 14
            };
            EditorGUILayout.LabelField(statusIcon, statusStyle, GUILayout.Width(20));
            
            // Label
            EditorGUILayout.LabelField(label, GUILayout.Width(130));
            
            // Status text
            Color previousColor = GUI.color;
            GUI.color = statusColor;
            EditorGUILayout.LabelField(status, EditorStyles.boldLabel);
            GUI.color = previousColor;
            
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws enum toggle buttons similar to Odin's EnumToggleButtons
        /// </summary>
        private T DrawEnumToggleButtons<T>(string label, T currentValue, Dictionary<T, string> displayNames = null) where T : Enum
        {
            var enumValues = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
            var currentIndex = Array.IndexOf(enumValues, currentValue);
            
            EditorGUILayout.BeginHorizontal();
            
            // Draw label on the left
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Width(140));
            
            // Add some space to match the indentation of other fields
            GUILayout.Space(10);
            
            for (int i = 0; i < enumValues.Length; i++)
            {
                var enumValue = enumValues[i];
                string displayName = displayNames?.ContainsKey(enumValue) == true ? displayNames[enumValue] : enumValue.ToString();
                
                // Create toggle button style with better visual feedback
                var buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontSize = 11;
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.fixedHeight = 28;
                
                if (i == currentIndex)
                {
                    // Selected state - lighter background for better appearance
                    GUI.backgroundColor = EditorGUIUtility.isProSkin ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.85f, 0.85f, 0.85f);
                    buttonStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                    buttonStyle.hover.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                }
                else
                {
                    // Unselected state - default background
                    GUI.backgroundColor = Color.white;
                    buttonStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                }
                
                // Add some padding between buttons
                if (i > 0) GUILayout.Space(2);
                
                if (GUILayout.Button(displayName, buttonStyle, GUILayout.ExpandWidth(true)))
                {
                    currentIndex = i;
                }
            }
            
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            
            return enumValues[currentIndex];
        }

        /// <summary>
        /// Draw build size breakdown as list only
        /// </summary>
        private void DrawBuildSizeChartHorizontal()
        {
            if (buildAnalysisResult?.assetSizes == null || buildAnalysisResult.totalBuildSize == 0)
            {
                DrawNoBuildDataMessage();
                return;
            }
            
            // Show build report status
            if (!string.IsNullOrEmpty(buildAnalysisResult.buildReportStatus))
            {
                GUIStyle statusStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = buildAnalysisResult.hasBuildReportData ? Color.green : Color.yellow }
                };
                EditorGUILayout.LabelField($"Data source: {buildAnalysisResult.lastBuildReportSource}", statusStyle);
                EditorGUILayout.Space(5);
            }
            
            // Asset Breakdown List
            DrawAssetBreakdownList();
        }
        
        /// <summary>
        /// Draw message when no build data is available
        /// </summary>
        private void DrawNoBuildDataMessage()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14
            };
            
            GUIStyle messageStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { textColor = Color.gray }
            };
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("📊 No Build Data Available", titleStyle);
            EditorGUILayout.Space(10);
            
            string message = "";
            if (!string.IsNullOrEmpty(buildAnalysisResult?.buildReportStatus))
            {
                message = buildAnalysisResult.buildReportStatus;
            }
            else
            {
                message = "Run a WebGL build to see detailed asset breakdown and size analysis.";
            }
            
            EditorGUILayout.LabelField(message, messageStyle);
            
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("💡 Tip: The chart will show accurate data from Unity's build report after your first build.", messageStyle);
            EditorGUILayout.Space(10);
            
            EditorGUILayout.EndVertical();
        }
        
        
        
        /// <summary>
        /// Draw interactive compression settings controls
        /// </summary>
        private void DrawCompressionSettings()
        {
            try
            {
                // Get current compression format
                var currentFormat = PlayerSettings.WebGL.compressionFormat;
                var formatOptions = new string[] { "Gzip", "Brotli", "Disabled" };
                var formatValues = new WebGLCompressionFormat[] 
                {
                    WebGLCompressionFormat.Gzip,
                    WebGLCompressionFormat.Brotli,
                    WebGLCompressionFormat.Disabled
                };
                
                int currentFormatIndex = System.Array.IndexOf(formatValues, currentFormat);
                if (currentFormatIndex == -1) currentFormatIndex = 0;
                
                // Compression Format Dropdown - using standard Unity layout
                EditorGUI.BeginChangeCheck();
                int newFormatIndex = EditorGUILayout.Popup("Format", currentFormatIndex, formatOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    PlayerSettings.WebGL.compressionFormat = formatValues[newFormatIndex];
                    // Update build analysis result
                    if (buildAnalysisResult != null)
                    {
                        buildAnalysisResult.compressionFormat = formatValues[newFormatIndex].ToString();
                    }
                    Debug.Log($"[PublishWindow] Changed WebGL compression format to: {formatValues[newFormatIndex]}");
                }
                
                // Decompression Fallback Toggle (only show if compression is enabled)
                if (formatValues[newFormatIndex] != WebGLCompressionFormat.Disabled)
                {
                    // Get current decompression fallback (true = JavaScript fallback enabled)
                    bool currentFallback = PlayerSettings.WebGL.decompressionFallback;
                    
                    EditorGUI.BeginChangeCheck();
                    bool newFallback = EditorGUILayout.Toggle("Decompress fallback", currentFallback);
                    if (EditorGUI.EndChangeCheck())
                    {
                        PlayerSettings.WebGL.decompressionFallback = newFallback;
                        // Update build analysis result
                        if (buildAnalysisResult != null)
                        {
                            buildAnalysisResult.decompressionFallback = newFallback ? "JavaScript" : "Native";
                        }
                        Debug.Log($"[PublishWindow] Changed WebGL decompression fallback to: {(newFallback ? "JavaScript" : "Native")}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                EditorGUILayout.LabelField($"Error loading compression settings: {ex.Message}", EditorStyles.miniLabel);
                Debug.LogWarning($"[PublishWindow] Error in DrawCompressionSettings: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Draw asset breakdown as a simple list
        /// </summary>
        private void DrawAssetBreakdownList()
        {
            if (buildAnalysisResult?.assetSizes == null || buildAnalysisResult.totalBuildSize == 0)
            {
                EditorGUILayout.LabelField("No asset data available.", EditorStyles.centeredGreyMiniLabel);
                return;
            }
            
            EditorGUILayout.LabelField("Asset Breakdown:", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            // Sort by size (largest first)
            var sortedAssets = buildAnalysisResult.assetSizes
                .Where(kvp => kvp.Value > 0)
                .OrderByDescending(kvp => kvp.Value)
                .ToList();
            
            foreach (var kvp in sortedAssets)
            {
                float percentage = (float)kvp.Value / buildAnalysisResult.totalBuildSize * 100f;
                
                EditorGUILayout.BeginHorizontal();
                
                // Category name
                EditorGUILayout.LabelField($"• {kvp.Key}:", GUILayout.Width(120));
                
                // Size and percentage
                EditorGUILayout.LabelField($"{BytesToString(kvp.Value)} ({percentage:F1}%)", EditorStyles.boldLabel);
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space(10);
            
            // Total size with emphasis
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Total Build Size:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            GUIStyle totalStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = EditorGUIUtility.isProSkin ? Color.cyan : Color.blue }
            };
            EditorGUILayout.LabelField(BytesToString(buildAnalysisResult.totalBuildSize), totalStyle);
            EditorGUILayout.EndHorizontal();
        }
        
        
        /// <summary>
        /// Draws enum flag toggle buttons for flags enums
        /// </summary>
        private T DrawEnumFlagToggleButtons<T>(string label, T currentValue) where T : Enum
        {
            var enumValues = Enum.GetValues(typeof(T)).Cast<T>().Where(v => Convert.ToInt32(v) != 0).ToArray();
            int currentFlags = Convert.ToInt32(currentValue);
            
            EditorGUILayout.BeginHorizontal();
            
            // Draw label on the left with proper vertical alignment
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Width(140));
            
            // Add some space to shift buttons to the right
            GUILayout.Space(10);
            
            for (int i = 0; i < enumValues.Length; i++)
            {
                var enumValue = enumValues[i];
                int flagValue = Convert.ToInt32(enumValue);
                bool isSelected = (currentFlags & flagValue) == flagValue;
                
                // Create toggle button style with better visual feedback
                var buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontSize = 10;
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.fixedHeight = 26;
                
                if (isSelected)
                {
                    // Selected state - lighter background for better appearance
                    GUI.backgroundColor = EditorGUIUtility.isProSkin ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.85f, 0.85f, 0.85f);
                    buttonStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                    buttonStyle.hover.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                }
                else
                {
                    // Unselected state - default background
                    GUI.backgroundColor = Color.white;
                    buttonStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                }
                
                // Add some padding between buttons
                if (i > 0) GUILayout.Space(2);
                
                // Format enum name for display (split camelCase)
                string displayName = System.Text.RegularExpressions.Regex.Replace(enumValue.ToString(), "([A-Z])", " $1").Trim();
                
                if (GUILayout.Button(displayName, buttonStyle, GUILayout.ExpandWidth(true)))
                {
                    if (isSelected)
                        currentFlags &= ~flagValue; // Remove flag
                    else
                        currentFlags |= flagValue;  // Add flag
                }
            }
            
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            
            return (T)Enum.ToObject(typeof(T), currentFlags);
        }

        #endregion

        #region GUI Drawing

        private void OnGUI()
        {
            // Don't initialize styles or do complex operations during build
            if (isCurrentlyBuilding)
            {
                InitializeStyles();
                
                EditorGUILayout.Space(20);
                
                // Tiêu đề build
                var titleStyle = new GUIStyle(EditorStyles.boldLabel) {
                    fontSize = 18,
                    alignment = TextAnchor.MiddleCenter
                };
                GUILayout.Label("Build in progress, please wait...", titleStyle);
                EditorGUILayout.Space(10);
                
                // Build info box
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                // Project info section
                EditorGUILayout.LabelField("Project Information", EditorStyles.boldLabel);
                EditorGUILayout.Space(5);
                
                // Company & Product
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Company Name:", EditorStyles.boldLabel, GUILayout.Width(140));
                EditorGUILayout.LabelField(GetCompanyName());
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Product Name:", EditorStyles.boldLabel, GUILayout.Width(140));
                EditorGUILayout.LabelField(GetProductName());
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("SDK Version:", EditorStyles.boldLabel, GUILayout.Width(140));
                EditorGUILayout.LabelField(GMSDKConfig.Instance.FullVersionString);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(10);
                
                // Compression Settings section
                EditorGUILayout.LabelField("Compression Settings", EditorStyles.boldLabel);
                EditorGUILayout.Space(5);
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Format:", EditorStyles.boldLabel, GUILayout.Width(140));
                string compressionFormat = buildAnalysisResult?.compressionFormat ?? PlayerSettings.WebGL.compressionFormat.ToString();
                bool isBrotli = compressionFormat == "Brotli";
                GUI.color = isBrotli ? Color.green : Color.yellow;
                EditorGUILayout.LabelField(compressionFormat, EditorStyles.boldLabel);
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Decompression:", EditorStyles.boldLabel, GUILayout.Width(140));
                EditorGUILayout.LabelField(buildAnalysisResult?.decompressionFallback ?? PlayerSettings.WebGL.decompressionFallback.ToString());
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(10);
                
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.Space(20);
                
                // SDK Requirements Check Header
                var headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 16,
                    alignment = TextAnchor.MiddleCenter
                };
                EditorGUILayout.LabelField("🔍 SDK Requirements Check", headerStyle);
                EditorGUILayout.Space(10);
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                // 1. Core SDK Components Section
                var sectionStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 12,
                    normal = { textColor = EditorGUIUtility.isProSkin ? Color.cyan : Color.blue }
                };
                EditorGUILayout.LabelField("Core SDK Components", sectionStyle);
                EditorGUILayout.Space(5);
                
                // GMSoft in Scenes
                if (buildAnalysisResult != null)
                {
                    int gmSoftSceneCount = buildAnalysisResult.scenesWithGMSoft?.Count ?? 0;
                    
                    if (gmSoftSceneCount == 0)
                    {
                        // No GMSoft found - Red
                        DrawRequirementStatus("GMSoft GameObject", false, "Not found in any scene");
                    }
                    else if (gmSoftSceneCount == 1)
                    {
                        // Exactly one GMSoft - Green (Perfect)
                        DrawRequirementStatus("GMSoft GameObject", true, 
                            $"Found in: {string.Join(", ", buildAnalysisResult.scenesWithGMSoft)}");
                    }
                    else
                    {
                        // Multiple GMSoft - Yellow (Warning)
                        DrawRequirementStatusWithColor("GMSoft GameObject", Color.yellow, 
                            $"Multiple found: {string.Join(", ", buildAnalysisResult.scenesWithGMSoft)}");
                    }
                    
                    EditorGUILayout.Space(10);
                    
                    // 2. GamePlay Methods Section
                    EditorGUILayout.LabelField("GamePlay Methods", sectionStyle);
                    EditorGUILayout.Space(5);
                    
                    DrawRequirementStatus("GamePlayStart", buildAnalysisResult.hasGamePlayStart, 
                        buildAnalysisResult.hasGamePlayStart ? "Implementation found" : "Not implemented");
                    DrawRequirementStatus("GamePlayStop", buildAnalysisResult.hasGamePlayStop,
                        buildAnalysisResult.hasGamePlayStop ? "Implementation found" : "Not implemented");
                    DrawRequirementStatus("GamePlayPause", buildAnalysisResult.hasGamePlayPause,
                        buildAnalysisResult.hasGamePlayPause ? "Implementation found" : "Not implemented");
                    DrawRequirementStatus("GamePlayResume", buildAnalysisResult.hasGamePlayResume,
                        buildAnalysisResult.hasGamePlayResume ? "Implementation found" : "Not implemented");
                    DrawRequirementStatus("GamePlayAgain", buildAnalysisResult.hasGamePlayAgain,
                        buildAnalysisResult.hasGamePlayAgain ? "Implementation found" : "Not implemented");
                    
                    EditorGUILayout.Space(10);
                    
                    // 3. UI Components Section
                    EditorGUILayout.LabelField("UI Components", sectionStyle);
                    EditorGUILayout.Space(5);
                    
                    // Safe null checks for MoreGamesButton
                    if (buildAnalysisResult != null && 
                        buildAnalysisResult.scenesWithMoreGamesButton != null && 
                        buildAnalysisResult.scenesWithoutMoreGamesButton != null)
                    {
                        int moreGamesButtonSceneCount = buildAnalysisResult.scenesWithMoreGamesButton.Count;
                        int totalScenesForMoreGames = buildAnalysisResult.scenesWithMoreGamesButton.Count + buildAnalysisResult.scenesWithoutMoreGamesButton.Count;
                        
                        if (buildAnalysisResult.hasMoreGamesButton && moreGamesButtonSceneCount > 0)
                        {
                            // At least one scene has MoreGamesButton - Green (requirement satisfied)
                            DrawRequirementStatus("MoreGamesButton", true, 
                                $"Found in {moreGamesButtonSceneCount}/{totalScenesForMoreGames} scenes");
                                
                            // Show which scenes have the MoreGamesButton with better formatting
                            if (buildAnalysisResult.scenesWithMoreGamesButton.Count > 0)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("", GUILayout.Width(20)); // Indent
                                EditorGUILayout.LabelField("Scenes:", GUILayout.Width(50));
                                GUI.color = Color.green;
                                EditorGUILayout.LabelField(string.Join(", ", buildAnalysisResult.scenesWithMoreGamesButton), EditorStyles.miniLabel);
                                GUI.color = Color.white;
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        else
                        {
                            // No MoreGamesButton found - Red
                            DrawRequirementStatus("MoreGamesButton", false, 
                                $"Not found (0/{totalScenesForMoreGames} scenes)");
                        }
                    }
                    else
                    {
                        DrawRequirementStatus("MoreGamesButton", false, "Checking...");
                    }
                    
                    EditorGUILayout.Space(10);
                    
                    // 4. Legal & Compliance Section
                    EditorGUILayout.LabelField("Legal & Compliance", sectionStyle);
                    EditorGUILayout.Space(5);
                    
                    // Safe null checks
                    if (buildAnalysisResult != null && 
                        buildAnalysisResult.scenesWithCopyright != null && 
                        buildAnalysisResult.scenesWithoutCopyright != null)
                    {
                        int copyrightSceneCount = buildAnalysisResult.scenesWithCopyright.Count;
                        int totalScenes = buildAnalysisResult.scenesWithCopyright.Count + buildAnalysisResult.scenesWithoutCopyright.Count;
                        
                        if (buildAnalysisResult.hasCopyrightText && copyrightSceneCount > 0)
                        {
                            // At least one scene has copyright - Green (requirement satisfied)
                            DrawRequirementStatus("Copyright Text", true, 
                                $"Found in {copyrightSceneCount}/{totalScenes} scenes");
                                
                            // Show which scenes have the copyright with better formatting
                            if (buildAnalysisResult.scenesWithCopyright.Count > 0)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("", GUILayout.Width(20)); // Indent
                                EditorGUILayout.LabelField("Scenes:", GUILayout.Width(50));
                                GUI.color = Color.green;
                                EditorGUILayout.LabelField(string.Join(", ", buildAnalysisResult.scenesWithCopyright), EditorStyles.miniLabel);
                                GUI.color = Color.white;
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        else
                        {
                            // No copyright found - Red
                            DrawRequirementStatus("Copyright Text", false, 
                                $"Not found (0/{totalScenes} scenes)");
                        }
                    }
                    else
                    {
                        DrawRequirementStatus("Copyright Text", false, "Checking...");
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Checking requirements...", EditorStyles.centeredGreyMiniLabel);
                }
                
                EditorGUILayout.EndVertical();
                
                return;
            }

            InitializeStyles();

            // Simple GUI without complex error handling during normal operation
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            DrawHeader();
            EditorGUILayout.Space(10);
            
            DrawConfigurationSection();
            EditorGUILayout.Space(10);
            
            DrawBuildInformationSection();
            EditorGUILayout.Space(10);
            
            DrawBuildOutputSection();
            
            DrawActionsSection();
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.EndScrollView();
            
            // Draw footer outside scroll view
            DrawFooter();
        }

        private void DrawFooter()
        {
            var footerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 9,
                normal = { textColor = Color.gray },
                alignment = TextAnchor.LowerRight
            };
            
            var copyrightStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                normal = { textColor = Color.gray },
                alignment = TextAnchor.LowerCenter
            };
            
            var shortcutStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 9,
                normal = { textColor = Color.gray },
                alignment = TextAnchor.LowerLeft
            };
            
            // Copyright footer in the center
            var copyrightRect = new Rect(0, position.height - 20, position.width, 15);
            GUI.Label(copyrightRect, "© 2025 Azgames.io", copyrightStyle);
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(15);
            
            // Reserve proper height for the large header text
            var headerRect = GUILayoutUtility.GetRect(0, 70); // Tăng chiều cao
            
            // Chuỗi text cần hiển thị
            string title = GMSDKConfig.Instance.DisplayVersionString;
            
            // Tạo style cơ bản cho text
            var baseStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 36,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            
            // Sử dụng màu trắng trong chế độ tối, đen trong chế độ sáng
            Color textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            
            // Đơn giản hóa: sử dụng một nhãn duy nhất với màu trắng
            baseStyle.normal.textColor = textColor;
            
            // Vẽ tiêu đề một lần với màu trắng
            GUI.Label(headerRect, title, baseStyle);
            
            EditorGUILayout.Space(10);
        }

        private void DrawConfigurationSection()
        {
            showConfiguration = EditorGUILayout.BeginFoldoutHeaderGroup(showConfiguration, "Project Configuration", sectionHeaderStyle);
            
            if (showConfiguration)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                // Publisher Settings
                EditorGUILayout.LabelField("Publisher Settings", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                
                Publisher newPublisher = DrawEnumToggleButtons("Publisher", publisher, PublisherDisplayNames);
                if (newPublisher != publisher)
                {
                    publisher = newPublisher;
                    SetPublisher(publisher);
                }
                
                if (publisher == Publisher.None)
                {
                    string newCompanyName = EditorGUILayout.TextField("Company Name", companyName);
                    if (newCompanyName != companyName)
                    {
                        companyName = newCompanyName;
                        SetCompanyName(companyName);
                    }
                }
                
                string newProductName = EditorGUILayout.TextField("Product Name", productName);
                if (newProductName != productName)
                {
                    productName = newProductName;
                    SetProductName(productName);
                }
                
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(10);
                
                // Build Settings
                EditorGUILayout.LabelField("Build Settings", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                
                buildNameOption = DrawEnumFlagToggleButtons("Build Name Format", buildNameOption);
                separator = EditorGUILayout.TextField("Name Separator", separator);
                
                string newWebglTemplate = DrawWebGLTemplateDropdown();
                if (newWebglTemplate != webglTemplate)
                {
                    webglTemplate = newWebglTemplate;
                    SetWebGLTemplate(webglTemplate);
                }
                
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(10);
                
                // Compression Settings
                EditorGUILayout.LabelField("Compression Settings", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                
                DrawCompressionSettings();
                
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(10);
                
                // Local Development
                EditorGUILayout.LabelField("Local Development", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                
                string newLocalHost = EditorGUILayout.TextField("Local Host URL", localHost);
                if (newLocalHost != localHost)
                {
                    localHost = newLocalHost;
                    SetLocalHost(localHost);
                }
                
                EditorGUI.indentLevel--;
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawBuildInformationSection()
        {
            showBuildInfo = EditorGUILayout.BeginFoldoutHeaderGroup(showBuildInfo, "Build Information", sectionHeaderStyle);
            
            if (showBuildInfo)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.BeginHorizontal();
                
                // Build Details (Left Column)
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Build Details", EditorStyles.boldLabel);
                
                EditorGUILayout.LabelField("File Name:", GetLastBuildFileName().Length == 0 ? "No build yet" : GetLastBuildFileName());
                EditorGUILayout.LabelField("Last Build:", HasBuildBefore() ? GetLastBuildTime().ToString("dd/MM/yyyy hh:mm:ss tt") : "No build yet");
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Build Count:", (GetDayBuildNumber() - 1).ToString());
                if (GUILayout.Button("+", GUILayout.Width(25))) IncrementDayBuildNumber();
                if (GUILayout.Button("-", GUILayout.Width(25))) DecrementDayBuildNumber();
                if (GUILayout.Button("Reset", GUILayout.Width(50))) ResetDayBuildNumber();
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.Space(20);
                
                // Statistics (Right Column)
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);
                
                EditorGUILayout.LabelField("Build Size:", buildStats.totalSize);
                EditorGUILayout.LabelField("Build Time:", buildStats.totalTime);
                
                // Warnings and errors
                EditorGUILayout.LabelField("Warnings:", buildStats.totalWarnings);
                EditorGUILayout.LabelField("Errors:", buildStats.totalErrors);
                
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawBuildOutputSection()
        {
            showBuildOutput = EditorGUILayout.BeginFoldoutHeaderGroup(showBuildOutput, "Build Output", sectionHeaderStyle);
            
            if (showBuildOutput)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.LabelField("Output Settings", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Save Path:", GetSelectedPath().Length == 0 ? "No path selected" : GetSelectedPath());
                if (GUILayout.Button("Browse", GUILayout.Width(60)))
                {
                    string path = EditorUtility.OpenFolderPanel("Choose Build Output Directory", GetSelectedPath(), "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        SetSelectedPath(path);
                    }
                }
                GUI.enabled = !string.IsNullOrEmpty(GetSelectedPath()) && System.IO.Directory.Exists(GetSelectedPath());
                if (GUILayout.Button("Open", GUILayout.Width(50)))
                {
                    OpenFolderInExplorer(GetSelectedPath());
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.LabelField("Build Name Preview:", GetPreviewBuildName());
                
                // Preview with debug checkbox (moved above URL)
                EditorGUI.BeginChangeCheck();
                previewWithDebug = EditorGUILayout.Toggle("Preview with debug", previewWithDebug);
                if (EditorGUI.EndChangeCheck())
                {
                    SaveSettings();
                }

                // Preview URL with debug option
                string previewUrl = HasBuildBefore() ? $"{localHost}/{GetLastBuildFileName()}" : "No build available";
                if (HasBuildBefore() && previewWithDebug)
                {
                    previewUrl += "?d=1";
                }
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Preview URL:", previewUrl);
                GUI.enabled = HasBuildBefore();
                if (GUILayout.Button("Preview", GUILayout.Width(60)))
                {
                    PreviewBuild();
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawActionsSection()
        {
            EditorGUILayout.Space(15);
            
            // Single row of action buttons
            EditorGUILayout.BeginHorizontal();
            
            // Build WebGL Button
            GUI.enabled = !isCurrentlyBuilding;
            if (GUILayout.Button("Build WebGL", buttonStyle, GUILayout.Height(40)))
            {
                ExecuteBuild();
            }
            GUI.enabled = true;
            
            EditorGUILayout.Space(5);
            
            // Build and Run Button
            GUI.enabled = !isCurrentlyBuilding;
            if (GUILayout.Button("Build and Run", buttonStyle, GUILayout.Height(40)))
            {
                BuildAndRun();
            }
            GUI.enabled = true;
            
            EditorGUILayout.Space(5);
            
            // Player Settings Button
            GUI.enabled = !isCurrentlyBuilding;
            if (GUILayout.Button("Player Settings", buttonStyle, GUILayout.Height(40)))
            {
                OpenPlayerSettings();
            }
            GUI.enabled = true;
            
            EditorGUILayout.Space(5);
            
            // Clear PlayerPrefs Button
            GUI.enabled = !isCurrentlyBuilding;
            if (GUILayout.Button("Clear PlayerPrefs", buttonStyle, GUILayout.Height(40)))
            {
                ClearPlayerPrefs();
            }
            GUI.enabled = true;
            
            EditorGUILayout.Space(5);
            
            // Build Size Chart Button
            GUI.enabled = !isCurrentlyBuilding;
            if (GUILayout.Button("Build Size Chart", buttonStyle, GUILayout.Height(40)))
            {
                OpenBuildSizeChart();
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
        }

        private string DrawWebGLTemplateDropdown()
        {
            var templates = GetAvailableTemplates().ToArray();
            int currentIndex = Array.IndexOf(templates, webglTemplate);
            if (currentIndex < 0) currentIndex = 0;
            
            EditorGUILayout.BeginHorizontal();
            int newIndex = EditorGUILayout.Popup("WebGL Template", currentIndex, templates);
            EditorGUILayout.EndHorizontal();
            
            return templates[newIndex];
        }

        #endregion

        #region Build Counter Methods

        /// <summary>
        /// Tăng số build lên 1 và lưu lại
        /// </summary>
        private void IncrementDayBuildNumber()
        {
            int newValue = GetDayBuildNumber() + 1;
            settings.dayBuildNumber = newValue;
            EditorPrefs.SetInt(GetProjectKey("day_build_number"), newValue);
            SaveSettings();
        }

        /// <summary>
        /// Giảm số build đi 1 và lưu lại
        /// </summary>
        private void DecrementDayBuildNumber()
        {
            int newValue = Mathf.Max(GetDayBuildNumber() - 1, 1);
            settings.dayBuildNumber = newValue;
            EditorPrefs.SetInt(GetProjectKey("day_build_number"), newValue);
            SaveSettings();
        }

        #endregion

        #region Configuration Methods

        public string GetLocalHost()
        {
            // First check settings, then EditorPrefs
            if (!string.IsNullOrEmpty(settings.localHost))
                return settings.localHost;
                
            return EditorPrefs.GetString(GetProjectKey("build_local_host"), "http://gamelocal.com");
        }

        private void SetLocalHost(string value)
        {
            settings.localHost = value;
            EditorPrefs.SetString(GetProjectKey("build_local_host"), value);
            SaveSettings();
        }

        public string GetWebGLTemplate()
        {
            // First check settings, then EditorPrefs
            if (!string.IsNullOrEmpty(settings.webglTemplate))
                return settings.webglTemplate;
                
            string template = EditorPrefs.GetString(GetProjectKey("webgl_template"), "");
            if (string.IsNullOrEmpty(template))
            {
                template = GetDefaultWebGLTemplate();
                EditorPrefs.SetString(GetProjectKey("webgl_template"), template);
            }
            return template;
        }

        private string GetDefaultWebGLTemplate()
        {
            // Check if gmsoft-v7 template exists and use it as default
            var availableTemplates = GetAvailableTemplates();
            
            // First priority: gmsoft-v7
            if (availableTemplates.Contains("gmsoft-v7"))
                return "gmsoft-v7";
            
            // Second priority: 2020.3-gmsoft-v4 (legacy)
            if (availableTemplates.Contains("2020.3-gmsoft-v4"))
                return "2020.3-gmsoft-v4";
            
            // Fallback to Default
            return "Default";
        }

        private void SetWebGLTemplate(string value)
        {
            settings.webglTemplate = value;
            EditorPrefs.SetString(GetProjectKey("webgl_template"), value);
            SaveSettings();
        }

        private IEnumerable<string> GetAvailableTemplates()
        {
            var templates = new List<string>();
            var allTemplates = new List<string> { "Default", "Minimal", "PWA" };

            // Unity built-in templates
            string templatesPath = Path.Combine(EditorApplication.applicationPath, "..", "PlaybackEngines", "WebGLSupport", "BuildTools", "WebGLTemplates");
            if (Directory.Exists(templatesPath))
            {
                var builtInTemplates = Directory.GetDirectories(templatesPath)
                    .Select(dir => Path.GetFileName(dir))
                    .Where(name => !string.IsNullOrEmpty(name) && !allTemplates.Contains(name));
                allTemplates.AddRange(builtInTemplates);
            }

            // Project templates
            string projectTemplatesPath = Path.Combine(Application.dataPath, "WebGLTemplates");
            if (Directory.Exists(projectTemplatesPath))
            {
                var projectTemplates = Directory.GetDirectories(projectTemplatesPath)
                    .Select(dir => Path.GetFileName(dir))
                    .Where(name => !string.IsNullOrEmpty(name));
                allTemplates.AddRange(projectTemplates);
            }

            // Prioritize GM Soft templates at the top
            if (allTemplates.Contains("gmsoft-v7"))
                templates.Add("gmsoft-v7");
            
            if (allTemplates.Contains("2020.3-gmsoft-v4"))
                templates.Add("2020.3-gmsoft-v4");

            // Add Default first if not already added
            if (!templates.Contains("Default"))
                templates.Add("Default");

            // Add remaining templates
            templates.AddRange(allTemplates.Where(t => !templates.Contains(t)));

            return templates.Count > 0 ? templates : new[] { "Default", "gmsoft-v7" };
        }

        private void SetCompanyName(string value)
        {
            PlayerSettings.companyName = value;
        }

        private void SetPublisher(Publisher publisher)
        {
            Publisher previousPublisher = this.publisher;
            this.publisher = publisher;
            settings.publisher = (int)publisher;
            SaveSettings();
            
            // Check if we need to show recompile progress
            bool needsRecompile = WillTriggerRecompile(previousPublisher, publisher);
            
            // Handle scripting define symbols
            RemoveScriptingDefineSymbol("CRAZY_GAMES"); // Remove first to avoid duplicates
            
            if (publisher != Publisher.None)
            {
                string companyNameToSet = "";
                switch (publisher)
                {
                    case Publisher.AzGames:
                        companyNameToSet = "azgames.io";
                        PlayerSettings.companyName = companyNameToSet;
                        companyName = PlayerSettings.companyName;
                        break;
                    case Publisher.OneGames:
                        companyNameToSet = "1games.io";
                        PlayerSettings.companyName = companyNameToSet;
                        companyName = PlayerSettings.companyName;
                        break;
                    case Publisher.CrazyGames:
                        // For CrazyGames, only add the define symbol, don't change company name
                        AddScriptingDefineSymbol("CRAZY_GAMES");
                        break;
                }
            }
            
            // Show compile progress if needed
            if (needsRecompile)
            {
                ShowCompileProgress(previousPublisher, publisher);
            }
        }

        private Publisher GetPublisher()
        {
            return (Publisher)settings.publisher;
        }

        public void SetProductName(string value)
        {
            PlayerSettings.productName = value;
        }

        private string GetCompanyName()
        {
            return PlayerSettings.companyName;
        }

        private static string GetProductName()
        {
            return PlayerSettings.productName;
        }

        #endregion

        #region Scripting Define Symbols Methods

        private void AddScriptingDefineSymbol(string symbol)
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            
            if (!currentDefines.Contains(symbol))
            {
                if (string.IsNullOrEmpty(currentDefines))
                {
                    currentDefines = symbol;
                }
                else
                {
                    currentDefines += ";" + symbol;
                }
                
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, currentDefines);
            }
        }

        private void RemoveScriptingDefineSymbol(string symbol)
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            
            if (currentDefines.Contains(symbol))
            {
                var definesList = currentDefines.Split(';').ToList();
                definesList.Remove(symbol);
                var newDefines = string.Join(";", definesList);
                
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, newDefines);
            }
        }

        private bool WillTriggerRecompile(Publisher previousPublisher, Publisher newPublisher)
        {
            // Check if transition involves CrazyGames (which uses CRAZY_GAMES define symbol)
            bool previousWasCrazy = previousPublisher == Publisher.CrazyGames;
            bool newIsCrazy = newPublisher == Publisher.CrazyGames;
            
            // Recompile is needed if we're switching to/from CrazyGames
            return previousWasCrazy != newIsCrazy;
        }

        private void ShowCompileProgress(Publisher previousPublisher, Publisher newPublisher)
        {
            string fromPublisher = PublisherDisplayNames.ContainsKey(previousPublisher) ? 
                PublisherDisplayNames[previousPublisher] : previousPublisher.ToString();
            string toPublisher = PublisherDisplayNames.ContainsKey(newPublisher) ? 
                PublisherDisplayNames[newPublisher] : newPublisher.ToString();
            
            // Show progress dialog immediately
            EditorUtility.DisplayProgressBar(
                "Changing Publisher", 
                $"Switching from {fromPublisher} to {toPublisher}...\nRecompiling scripts...", 
                0.3f
            );
            
            // Start coroutine to monitor compilation and hide progress
            EditorApplication.update += UpdateCompileProgress;
            compileStartTime = EditorApplication.timeSinceStartup;
            
            Debug.Log($"[PublishWindow] Changing publisher from {fromPublisher} to {toPublisher}. Scripts will recompile...");
        }
        
        private double compileStartTime;
        private void UpdateCompileProgress()
        {
            // Update progress bar
            double elapsed = EditorApplication.timeSinceStartup - compileStartTime;
            float progress = Mathf.Clamp01((float)(elapsed / 3.0)); // Assume max 3 seconds
            
            EditorUtility.DisplayProgressBar(
                "Changing Publisher", 
                $"Recompiling scripts... ({elapsed:F1}s)", 
                progress
            );
            
            // Check if compilation is done or timeout after 10 seconds
            if (!EditorApplication.isCompiling || elapsed > 10.0)
            {
                EditorApplication.update -= UpdateCompileProgress;
                EditorUtility.ClearProgressBar();
                
                if (!EditorApplication.isCompiling)
                {
                    Debug.Log($"[PublishWindow] Publisher change completed. Scripts recompiled in {elapsed:F1}s");
                }
                else
                {
                    Debug.Log($"[PublishWindow] Progress bar timed out after {elapsed:F1}s");
                }
            }
        }

        #endregion

        #region Build Methods

        private void ExecuteBuild()
        {
            if (isCurrentlyBuilding)
            {
                Debug.LogWarning("Build already in progress!");
                return;
            }
            
            string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", GetSelectedPath(), "");
            if (string.IsNullOrEmpty(path)) return;
            
            // Perform build analysis before build
            PerformBuildAnalysis();
            
            // Mark build as started BEFORE any build operations
            isCurrentlyBuilding = true;
            
            // Force window to repaint to show build status
            Repaint();
            
                // Use delayed call to ensure GUI updates before build starts
            EditorApplication.delayCall += () =>
            {
                // Thêm các bước thông báo chi tiết
                StartCoroutine(BuildWithProgress(path));
            };
        }
        
        // Mô phỏng coroutine cho quá trình build
        private System.Collections.IEnumerator BuildWithProgress(string path)
        {
            // Chờ một khoảng thời gian ngắn để giao diện cập nhật
            yield return null;
            
            // Thực hiện build thực tế ngay lập tức
            PerformBuild(path);
        }
        
        // Build with progress and auto-run after completion
        private System.Collections.IEnumerator BuildWithProgressAndRun(string path)
        {
            // Chờ một khoảng thời gian ngắn để giao diện cập nhật
            yield return null;
            
            // Thực hiện build thực tế với auto-run
            PerformBuildAndRun(path);
        }
        
        // Helper class để đợi trong editor
        private class EditorWaitForSeconds
        {
            private double _targetTime;
            
            public EditorWaitForSeconds(float seconds)
            {
                _targetTime = EditorApplication.timeSinceStartup + seconds;
            }
            
            public bool keepWaiting
            {
                get { return EditorApplication.timeSinceStartup < _targetTime; }
            }
        }
        
        // Mô phỏng StartCoroutine cho EditorWindow
        private void StartCoroutine(System.Collections.IEnumerator routine)
        {
            EditorApplication.update += () => 
            {
                if (routine.MoveNext())
                {
                    if (routine.Current is EditorWaitForSeconds wait)
                    {
                        if (wait.keepWaiting)
                            return;
                    }
                    EditorApplication.update -= () => {};
                }
            };
        }
        
        private void PerformBuildAndRun(string path)
        {
            try
            {
                SetSelectedPath(path);
                string savePath = Path.Combine(path, GetPreviewBuildName());
                
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                PlayerSettings.bundleVersion = GetBundleVersion();

                // Setup WebGL template
                string templateToUse = SetupWebGLTemplate();
                PlayerSettings.WebGL.template = templateToUse;

                buildStatusMessage = "Building WebGL...";
                
                // Execute build
                BuildReport buildReport = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, savePath, BuildTarget.WebGL, BuildOptions.None);

                ProcessBuildResultAndRun(buildReport, savePath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Build failed with exception: {ex.Message}");
            }
            finally
            {
                // Reset build state
                isCurrentlyBuilding = false;
                
                // Force repaint after build completes
                EditorApplication.delayCall += () =>
                {
                    if (this != null)
                    {
                        Repaint();
                    }
                };
            }
        }
        
        private void PerformBuild(string path)
        {
            try
            {
                SetSelectedPath(path);
                string savePath = Path.Combine(path, GetPreviewBuildName());
                
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                PlayerSettings.bundleVersion = GetBundleVersion();

                // Setup WebGL template
                string templateToUse = SetupWebGLTemplate();
                PlayerSettings.WebGL.template = templateToUse;

                buildStatusMessage = "Building WebGL...";
                
                // Execute build
                BuildReport buildReport = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, savePath, BuildTarget.WebGL, BuildOptions.None);

                ProcessBuildResult(buildReport, savePath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Build failed with exception: {ex.Message}");
            }
            finally
            {
                // Reset build state
                isCurrentlyBuilding = false;
                
                // Force repaint after build completes
                EditorApplication.delayCall += () =>
                {
                    if (this != null)
                    {
                        Repaint();
                    }
                };
            }
        }

        private string SetupWebGLTemplate()
        {
            string templateToUse = webglTemplate;
            string projectTemplatesPath = Path.Combine(Application.dataPath, "WebGLTemplates");
            
            if (Directory.Exists(projectTemplatesPath))
            {
                var projectTemplateNames = Directory.GetDirectories(projectTemplatesPath)
                    .Select(dir => Path.GetFileName(dir));
                if (projectTemplateNames.Contains(webglTemplate))
                {
                    templateToUse = "PROJECT:" + webglTemplate;
                }
            }
            
            return templateToUse;
        }

        private void ProcessBuildResultAndRun(BuildReport buildReport, string savePath)
        {
            if (buildReport.summary.result == BuildResult.Succeeded)
            {
                // Log chi tiết thông tin build
                Debug.Log($"Build completed successfully in {(int)buildReport.summary.totalTime.TotalSeconds} seconds");
                Debug.Log($"Build size: {EditorUtility.FormatBytes((long)buildReport.summary.totalSize)}");
                Debug.Log($"Warnings: {buildReport.summary.totalWarnings}, Errors: {buildReport.summary.totalErrors}");
                
                // Tạo báo cáo chi tiết về build
                StringBuilder buildSummary = new StringBuilder();
                buildSummary.AppendLine($"===== BUILD REPORT =====");
                buildSummary.AppendLine($"Product Name: {GetProductName()}");
                buildSummary.AppendLine($"Company: {GetCompanyName()}");
                buildSummary.AppendLine($"Version: {GMSDKConfig.Instance.FullVersionString}");
                buildSummary.AppendLine($"Build Path: {savePath}");
                buildSummary.AppendLine($"Build Size: {EditorUtility.FormatBytes((long)buildReport.summary.totalSize)}");
                buildSummary.AppendLine($"Build Time: {buildReport.summary.totalTime:mm\\:ss\\.ff}");
                buildSummary.AppendLine($"Warnings: {buildReport.summary.totalWarnings}");
                buildSummary.AppendLine($"Errors: {buildReport.summary.totalErrors}");
                buildSummary.AppendLine($"Platform: WebGL");
                buildSummary.AppendLine($"WebGL Template: {webglTemplate}");
                buildSummary.AppendLine($"Build Number: {GetDayBuildNumber() - 1}");
                buildSummary.AppendLine($"Build Timestamp: {DateTime.Now}");
                buildSummary.AppendLine($"======================");
                
                // Log báo cáo chi tiết
                Debug.Log(buildSummary.ToString());
                
                // Cập nhật thông tin build stats
                UpdateBuildStats(buildReport);
                
                // Auto-open URL after successful build
                string buildName = GetPreviewBuildName();
                string url = $"{localHost}/{buildName}";
                if (previewWithDebug)
                {
                    url += "?d=1";
                }
                
                Debug.Log($"Build and Run: Opening {url}");
                Application.OpenURL(url);
                
                // Also open build folder
                OpenBuildFolder(savePath);
            }
            else
            {
                Debug.LogError($"Build failed: {buildReport.summary.result}");
                
                if (buildReport.summary.totalErrors > 0)
                {
                    Debug.LogError("Build errors found. Check the console for details.");
                }
                
                buildStatusMessage = $"Build Failed: {buildReport.summary.result}";
                
                // Show build folder even on failure for debugging
                OpenBuildFolder(savePath);
            }
        }

        private void ProcessBuildResult(BuildReport buildReport, string savePath)
        {
            if (buildReport.summary.result == BuildResult.Succeeded)
            {
                // Log chi tiết thông tin build
                Debug.Log($"Build completed successfully in {(int)buildReport.summary.totalTime.TotalSeconds} seconds");
                Debug.Log($"Build size: {EditorUtility.FormatBytes((long)buildReport.summary.totalSize)}");
                Debug.Log($"Warnings: {buildReport.summary.totalWarnings}, Errors: {buildReport.summary.totalErrors}");
                
                // Tạo báo cáo chi tiết về build
                StringBuilder buildSummary = new StringBuilder();
                buildSummary.AppendLine($"===== BUILD REPORT =====");
                buildSummary.AppendLine($"Product Name: {GetProductName()}");
                buildSummary.AppendLine($"Company: {GetCompanyName()}");
                buildSummary.AppendLine($"Version: {GMSDKConfig.Instance.FullVersionString}");
                buildSummary.AppendLine($"Build Path: {savePath}");
                buildSummary.AppendLine($"Build Size: {EditorUtility.FormatBytes((long)buildReport.summary.totalSize)}");
                buildSummary.AppendLine($"Build Time: {buildReport.summary.totalTime:mm\\:ss\\.ff}");
                buildSummary.AppendLine($"Warnings: {buildReport.summary.totalWarnings}");
                buildSummary.AppendLine($"Errors: {buildReport.summary.totalErrors}");
                buildSummary.AppendLine($"Platform: WebGL");
                buildSummary.AppendLine($"WebGL Template: {webglTemplate}");
                buildSummary.AppendLine($"Build Number: {GetDayBuildNumber() - 1}");
                buildSummary.AppendLine($"Build Timestamp: {DateTime.Now}");
                buildSummary.AppendLine($"======================");
                
                // Log báo cáo chi tiết
                Debug.Log(buildSummary.ToString());
                
                // Cập nhật thông tin build stats
                UpdateBuildStats(buildReport);
                OpenBuildFolder(savePath);
                OnBuildCompleted(savePath);
                
                // Hiển thị thông báo thành công trong console
                Debug.Log("✓ Build completed successfully!");
            }
            else
            {
                Debug.LogError($"Build failed with result: {buildReport.summary.result}");
            }
        }

        private void UpdateBuildStats(BuildReport buildReport)
        {
            string buildSize = BytesToString((long)buildReport.summary.totalSize);
            string buildTime = buildReport.summary.totalTime.ToString(@"mm\:ss");
            string totalErrors = buildReport.summary.totalErrors.ToString();
            string totalWarnings = buildReport.summary.totalWarnings.ToString();
            
            buildStats = new BuildStats(buildSize, buildTime, totalWarnings, totalErrors);
            settings.buildStats = buildStats;
            
            // Extract asset breakdown from build report
            ExtractAssetDataFromBuildReport(buildReport);
            
            SaveSettings();
        }
        
        /// <summary>
        /// Extract asset breakdown data from Unity's BuildReport using Editor.log parsing
        /// </summary>
        private void ExtractAssetDataFromBuildReport(BuildReport buildReport)
        {
            try
            {
                buildAnalysisResult.assetSizes.Clear();
                buildAnalysisResult.totalBuildSize = (long)buildReport.summary.totalSize;
                buildAnalysisResult.hasBuildReportData = false;
                buildAnalysisResult.lastBuildReportSource = "None";
                
                // Initialize categories based on Unity's build report format
                Dictionary<string, long> categories = new Dictionary<string, long>
                {
                    {"Textures", 0},
                    {"Meshes", 0},
                    {"Audio", 0},
                    {"Scripts", 0},
                    {"Levels", 0},
                    {"Animation", 0},
                    {"Materials", 0},
                    {"Resources", 0},
                    {"Miscellaneous", 0},
                    {"StreamingAssets", 0}
                };
                
                // Try to parse from Editor.log first (most accurate)
                if (TryParseFromEditorLog(categories))
                {
                    buildAnalysisResult.assetSizes = categories;
                    buildAnalysisResult.hasBuildReportData = true;
                    buildAnalysisResult.lastBuildReportSource = "Unity Editor.log";
                    buildAnalysisResult.buildReportStatus = "Asset data extracted from Unity build report";
                    Debug.Log($"[BuildAnalysis] Extracted asset breakdown from Editor.log: Total size {BytesToString(buildAnalysisResult.totalBuildSize)}");
                    return;
                }
                
                // Fallback: Use BuildReport.files if available
                if (buildReport.files != null && buildReport.files.Length > 0)
                {
                    foreach (var file in buildReport.files)
                    {
                        string extension = System.IO.Path.GetExtension(file.path).ToLower();
                        string category = GetUnityBuildReportCategory(file.path, extension);
                        categories[category] += (long)file.size;
                    }
                    
                    buildAnalysisResult.assetSizes = categories;
                    buildAnalysisResult.hasBuildReportData = true;
                    buildAnalysisResult.lastBuildReportSource = "Unity BuildReport API";
                    buildAnalysisResult.buildReportStatus = "Asset data extracted from Unity BuildReport API";
                    Debug.Log($"[BuildAnalysis] Extracted asset breakdown from BuildReport.files: Total size {BytesToString(buildAnalysisResult.totalBuildSize)}");
                    return;
                }
                
                // Final fallback: Use project analysis
                Debug.LogWarning("[BuildAnalysis] No build report data available, using project analysis as fallback.");
                buildAnalysisResult.lastBuildReportSource = "Project Analysis (Estimated)";
                buildAnalysisResult.buildReportStatus = "Build report not found. Using estimated project data. Run a build for accurate results.";
                AnalyzeAssetSizes();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[BuildAnalysis] Error extracting asset data from build report: {ex.Message}. Using project analysis as fallback.");
                buildAnalysisResult.lastBuildReportSource = "Project Analysis (Error Fallback)";
                buildAnalysisResult.buildReportStatus = $"Error reading build report: {ex.Message}. Using estimated project data.";
                AnalyzeAssetSizes();
            }
        }
        
        /// <summary>
        /// Try to parse asset breakdown from Unity's Editor.log file
        /// </summary>
        private bool TryParseFromEditorLog(Dictionary<string, long> categories)
        {
            try
            {
                string editorLogPath = GetEditorLogPath();
                if (!System.IO.File.Exists(editorLogPath))
                {
                    buildAnalysisResult.buildReportStatus = $"Editor.log file not found at: {editorLogPath}";
                    Debug.LogWarning($"[BuildAnalysis] Editor.log file not found at: {editorLogPath}");
                    return false;
                }
                
                // Check if file is accessible
                try
                {
                    using (var fileStream = System.IO.File.OpenRead(editorLogPath))
                    {
                        // File is accessible
                    }
                }
                catch (System.Exception ex)
                {
                    buildAnalysisResult.buildReportStatus = $"Cannot access Editor.log file: {ex.Message}";
                    Debug.LogWarning($"[BuildAnalysis] Cannot access Editor.log file: {ex.Message}");
                    return false;
                }
                
                // Read the log file
                string logContent;
                try
                {
                    logContent = System.IO.File.ReadAllText(editorLogPath);
                }
                catch (System.Exception ex)
                {
                    buildAnalysisResult.buildReportStatus = $"Error reading Editor.log file: {ex.Message}";
                    Debug.LogWarning($"[BuildAnalysis] Error reading Editor.log file: {ex.Message}");
                    return false;
                }
                
                if (string.IsNullOrEmpty(logContent))
                {
                    buildAnalysisResult.buildReportStatus = "Editor.log file is empty";
                    Debug.LogWarning("[BuildAnalysis] Editor.log file is empty.");
                    return false;
                }
                
                // Find the build report section
                string buildReportMarker = "Build Report";
                int buildReportIndex = logContent.LastIndexOf(buildReportMarker);
                if (buildReportIndex == -1)
                {
                    buildAnalysisResult.buildReportStatus = "Build Report section not found in Editor.log. Try building the project first.";
                    Debug.LogWarning("[BuildAnalysis] Build Report section not found in Editor.log.");
                    return false;
                }
                
                // Extract the build report section
                string buildReportSection = logContent.Substring(buildReportIndex);
                
                // Parse "Uncompressed usage by category" section
                bool parseSuccess = ParseUncompressedUsageByCategory(buildReportSection, categories);
                
                if (!parseSuccess)
                {
                    buildAnalysisResult.buildReportStatus = "Could not parse asset categories from build report. Data may be incomplete.";
                }
                
                return parseSuccess;
            }
            catch (System.Exception ex)
            {
                buildAnalysisResult.buildReportStatus = $"Unexpected error parsing Editor.log: {ex.Message}";
                Debug.LogWarning($"[BuildAnalysis] Error parsing Editor.log: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Parse the "Uncompressed usage by category" section from build report
        /// </summary>
        private bool ParseUncompressedUsageByCategory(string buildReportContent, Dictionary<string, long> categories)
        {
            try
            {
                string categoryMarker = "Uncompressed usage by category";
                int categoryIndex = buildReportContent.IndexOf(categoryMarker);
                if (categoryIndex == -1)
                {
                    // Try alternative marker
                    categoryMarker = "Used Assets and files from the Resources folder";
                    categoryIndex = buildReportContent.IndexOf(categoryMarker);
                    
                    if (categoryIndex == -1)
                    {
                        Debug.LogWarning("[BuildAnalysis] Asset category section not found in build report.");
                        return false;
                    }
                }
                
                // Extract lines after the category marker
                string[] lines = buildReportContent.Substring(categoryIndex).Split('\n');
                bool foundAnyData = false;
                int parsedLines = 0;
                
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Contains("----") || line.Contains(categoryMarker))
                        continue;
                        
                    // Stop if we hit another section
                    if (line.Contains("System memory usage") || line.Contains("Serialized files"))
                        break;
                        
                    // Look for lines like: "Textures      1.2 mb	 45.2%"
                    var parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3)
                    {
                        string categoryName = parts[0].Trim();
                        string sizeStr = parts[1].Trim();
                        string unitStr = parts[2].Trim().ToLower();
                        
                        // Parse size
                        if (float.TryParse(sizeStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float size))
                        {
                            long sizeInBytes = ConvertToBytes(size, unitStr);
                            
                            // Map Unity category names to our categories
                            string mappedCategory = MapUnityCategory(categoryName);
                            if (categories.ContainsKey(mappedCategory))
                            {
                                categories[mappedCategory] += sizeInBytes; // Use += in case of duplicates
                                foundAnyData = true;
                                parsedLines++;
                            }
                        }
                    }
                }
                
                if (!foundAnyData)
                {
                    Debug.LogWarning("[BuildAnalysis] No asset category data could be parsed from build report.");
                    return false;
                }
                
                Debug.Log($"[BuildAnalysis] Successfully parsed {parsedLines} asset categories from build report.");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[BuildAnalysis] Error parsing category usage: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Get Unity Editor.log file path
        /// </summary>
        private string GetEditorLogPath()
        {
            string localAppDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            return System.IO.Path.Combine(localAppDataPath, "Unity", "Editor", "Editor.log");
        }
        
        /// <summary>
        /// Convert size with unit to bytes
        /// </summary>
        private long ConvertToBytes(float size, string unit)
        {
            switch (unit.ToLower())
            {
                case "b":
                case "bytes":
                    return (long)size;
                case "kb":
                    return (long)(size * 1024);
                case "mb":
                    return (long)(size * 1024 * 1024);
                case "gb":
                    return (long)(size * 1024 * 1024 * 1024);
                default:
                    return (long)size; // Default to bytes
            }
        }
        
        /// <summary>
        /// Map Unity's category names to our display categories
        /// </summary>
        private string MapUnityCategory(string unityCategory)
        {
            switch (unityCategory.ToLower())
            {
                case "textures":
                    return "Textures";
                case "meshes":
                    return "Meshes";
                case "audio":
                case "sounds":
                    return "Audio";
                case "scripts":
                case "code":
                    return "Scripts";
                case "levels":
                case "scenes":
                    return "Levels";
                case "animation":
                case "animations":
                    return "Animation";
                case "materials":
                case "shaders":
                    return "Materials";
                case "resources":
                    return "Resources";
                case "streamingassets":
                    return "StreamingAssets";
                default:
                    return "Miscellaneous";
            }
        }
        
        /// <summary>
        /// Get Unity build report category for a file path (fallback method)
        /// </summary>
        private string GetUnityBuildReportCategory(string filePath, string extension)
        {
            // Check file path patterns first
            if (filePath.Contains("Resources/"))
                return "Resources";
            if (filePath.Contains("StreamingAssets/"))
                return "StreamingAssets";
            if (filePath.EndsWith(".unity"))
                return "Levels";
                
            // Then check by extension
            switch (extension)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".tga":
                case ".psd":
                case ".exr":
                case ".hdr":
                    return "Textures";
                    
                case ".wav":
                case ".mp3":
                case ".ogg":
                case ".aiff":
                case ".aac":
                    return "Audio";
                    
                case ".fbx":
                case ".obj":
                case ".dae":
                case ".3ds":
                case ".blend":
                case ".mesh":
                    return "Meshes";
                    
                case ".cs":
                case ".js":
                case ".dll":
                    return "Scripts";
                    
                case ".mat":
                case ".shader":
                case ".shadergraph":
                    return "Materials";
                    
                case ".anim":
                case ".controller":
                case ".overrideController":
                    return "Animation";
                    
                default:
                    return "Miscellaneous";
            }
        }

        private void OpenBuildFolder(string savePath)
        {
            savePath = savePath.Replace("\\", "/");
            string windir = Environment.GetEnvironmentVariable("windir") ?? "C:\\Windows\\";
            
            if (!windir.EndsWith("\\"))
                windir += "\\";

            var fileToLocate = new FileInfo(savePath);
            var processInfo = new ProcessStartInfo(windir + "explorer.exe")
            {
                Arguments = "/select, \"" + fileToLocate.FullName + "\"",
                WindowStyle = ProcessWindowStyle.Normal,
                WorkingDirectory = windir
            };

            Process.Start(processInfo);
        }

        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            // Post-build processing if needed
        }

        #endregion

        #region Action Methods

        private void BuildAndRun()
        {
            if (isCurrentlyBuilding)
            {
                Debug.LogWarning("Build already in progress!");
                return;
            }
            
            string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", GetSelectedPath(), "");
            if (string.IsNullOrEmpty(path)) return;
            
            // Perform build analysis before build
            PerformBuildAnalysis();
            
            // Mark build as started BEFORE any build operations
            isCurrentlyBuilding = true;
            
            // Force window to repaint to show build status
            Repaint();
            
            // Use delayed call to ensure GUI updates before build starts
            EditorApplication.delayCall += () =>
            {
                // Start build with auto-run after completion
                StartCoroutine(BuildWithProgressAndRun(path));
            };
        }

        private void Preview()
        {
            if (!HasBuildBefore())
            {
                Debug.LogWarning("No previous build found to preview.");
                return;
            }
            
            string url = $"{localHost}/{GetLastBuildFileName()}";
            if (previewWithDebug)
            {
                url += "?d=1";
            }
            
            Application.OpenURL(url);
        }

        private void OpenPlayerSettings()
        {
            SettingsService.OpenProjectSettings("Project/Player");
        }

        private void PreviewBuild()
        {
            if (!HasBuildBefore())
            {
                Debug.LogWarning("No previous build found to preview.");
                return;
            }
            
            string url = $"{localHost}/{GetLastBuildFileName()}";
            if (previewWithDebug)
            {
                url += "?d=1";
            }
            
            Debug.Log($"Opening preview URL: {url}");
            Application.OpenURL(url);
        }
        
        private void ClearPlayerPrefs()
        {
            if (EditorUtility.DisplayDialog("Clear PlayerPrefs",
                $"Are you sure you want to clear all PlayerPrefs for {GetProductName()}?\n\nThis will reset all game save data for testing.",
                "Clear", "Cancel"))
            {
                PlayerPrefs.DeleteAll();
                Debug.Log("✓ PlayerPrefs cleared successfully!");
            }
        }
        
        private void OpenFolderInExplorer(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || !System.IO.Directory.Exists(folderPath))
            {
                Debug.LogWarning("Folder path does not exist: " + folderPath);
                return;
            }
            
            try
            {
                folderPath = folderPath.Replace("/", "\\");
                string windir = System.Environment.GetEnvironmentVariable("windir") ?? "C:\\Windows\\";
                
                if (!windir.EndsWith("\\"))
                    windir += "\\";

                var processInfo = new System.Diagnostics.ProcessStartInfo(windir + "explorer.exe")
                {
                    Arguments = $"\"{folderPath}\"",
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal,
                    WorkingDirectory = windir
                };

                System.Diagnostics.Process.Start(processInfo);
                Debug.Log($"Opened folder in explorer: {folderPath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to open folder in explorer: {ex.Message}");
            }
        }

        private void OpenBuildSizeChart()
        {
            // Delay opening to avoid GUI conflicts
            EditorApplication.delayCall += () =>
            {
                BuildSizeChartWindow.ShowWindow();
            };
        }

        #endregion

        #region Utility Methods

        private static string BytesToString(long byteCount)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0) return "0" + suffixes[0];
            
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suffixes[place];
        }

        private string GetPreviewBuildName()
        {
            string previewBuildName = string.Empty;
            
            if (buildNameOption.HasFlag(BuildNameOption.HasProductName)) 
                previewBuildName += GetBuildProductName();
            
            if (buildNameOption.HasFlag(BuildNameOption.HasProductName) && buildNameOption.HasFlag(BuildNameOption.HasBuildVersion)) 
                previewBuildName += separator;
            
            if (buildNameOption.HasFlag(BuildNameOption.HasBuildVersion)) 
                previewBuildName += GetBundleVersion();
            
            if (string.IsNullOrEmpty(previewBuildName)) 
                previewBuildName = "build-test";
            
            return previewBuildName;
        }

        private string GetLastBuildFileName()
        {
            // First check settings, then EditorPrefs
            if (!string.IsNullOrEmpty(settings.lastBuildFileName))
                return settings.lastBuildFileName;
                
            return EditorPrefs.GetString(GetProjectKey("last_build_file_name"), "");
        }

        private void SetLastBuildFileName(string value)
        {
            settings.lastBuildFileName = value;
            EditorPrefs.SetString(GetProjectKey("last_build_file_name"), value);
            SaveSettings();
        }

        public string GetBundleVersion()
        {
            return DateTime.Now.ToString("yyMMdd") + GetDayBuildNumber().ToString("00");
        }

        private string GetSelectedPath()
        {
            // First check settings, then EditorPrefs
            if (!string.IsNullOrEmpty(settings.selectedPath))
                return settings.selectedPath;
                
            return EditorPrefs.GetString(GetProjectKey("selected_path"), "");
        }

        private void SetSelectedPath(string path)
        {
            settings.selectedPath = path;
            EditorPrefs.SetString(GetProjectKey("selected_path"), path);
            SaveSettings();
        }

        private static string GetBuildProductName()
        {
            return Application.productName.ToLower().Replace(" ", "-");
        }

        private bool HasBuildBefore()
        {
            // First check settings
            if (!string.IsNullOrEmpty(settings.lastBuildTime))
                return true;
                
            // Then check EditorPrefs
            return EditorPrefs.HasKey(GetProjectKey("last_build_time"));
        }

        private DateTime GetLastBuildTime()
        {
            // First check settings
            if (!string.IsNullOrEmpty(settings.lastBuildTime))
            {
                if (DateTime.TryParse(settings.lastBuildTime, out DateTime result))
                    return result;
            }
            
            // Then check EditorPrefs
            string timeStr = EditorPrefs.GetString(GetProjectKey("last_build_time"), "");
            if (!string.IsNullOrEmpty(timeStr))
            {
                if (DateTime.TryParse(timeStr, out DateTime result))
                    return result;
            }
            
            return DateTime.Now;
        }

        /// <summary>
        /// Lấy số build hiện tại từ settings hoặc EditorPrefs
        /// </summary>
        private int GetDayBuildNumber()
        {
            // First check EditorPrefs for the most up-to-date value
            int editorPrefsValue = EditorPrefs.GetInt(GetProjectKey("day_build_number"), 1);
            
            // Update settings to keep it synchronized
            settings.dayBuildNumber = editorPrefsValue;
            
            return editorPrefsValue;
        }

        /// <summary>
        /// Đặt lại số build về 1 (ví dụ: khi chuyển ngày)
        /// </summary>
        private void ResetDayBuildNumber()
        {
            settings.dayBuildNumber = 1;
            EditorPrefs.SetInt(GetProjectKey("day_build_number"), 1);
            SaveSettings();
        }

        private void OnBuildCompleted(string buildPath = null)
        {
            string buildName = GetPreviewBuildName();
            SetLastBuildFileName(buildName);
            settings.lastBuildTime = DateTime.Now.ToString();
            
            // Tăng số build lên 1 và lưu lại
            int newBuildNumber = GetDayBuildNumber() + 1;
            settings.dayBuildNumber = newBuildNumber;
            EditorPrefs.SetInt(GetProjectKey("day_build_number"), newBuildNumber);
            SaveSettings();
            
            // Add build to size history if we have a valid build path
            if (!string.IsNullOrEmpty(buildPath))
            {
                // Use delayed call to avoid any GUI conflicts
                EditorApplication.delayCall += () =>
                {
                    BuildSizeChartWindow.AddBuildToHistory(buildPath, buildName);
                    BuildSizeChartWindow.RefreshAllWindows();
                };
            }
        }

        private void DrawGradientRect(Rect position, Color colorStart, Color colorEnd, int borderSize = 1)
        {
            EditorGUI.DrawRect(position, colorEnd);
            
            for (int i = 0; i < (int)position.height; i++)
            {
                float t = i / position.height;
                Color currentColor = Color.Lerp(colorStart, colorEnd, t);
                Rect lineRect = new Rect(position.x, position.y + i, position.width, 1);
                EditorGUI.DrawRect(lineRect, currentColor);
            }
            
            // Draw border
            EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, borderSize), Color.black);
            EditorGUI.DrawRect(new Rect(position.x, position.y + position.height - borderSize, position.width, borderSize), Color.black);
            EditorGUI.DrawRect(new Rect(position.x, position.y, borderSize, position.height), Color.black);
            EditorGUI.DrawRect(new Rect(position.x + position.width - borderSize, position.y, borderSize, position.height), Color.black);
        }

        #endregion
    }
}
#endif