using UnityEngine;
using UnityEditor;
using GMSoft;

namespace GMSoft.Editor
{
    [CustomEditor(typeof(GMAdvertisementManager))]
    public class GMAdvertisementManagerEditor : UnityEditor.Editor
    {
        private GMAdvertisementManager manager;
        private bool showDebugInfo = false;
        private bool showAdSettings = true;
        private bool showCallbacks = true;

        private GUIStyle headerStyle;
        private GUIStyle subHeaderStyle;
        private GUIStyle labelStyle;
        private GUIStyle boxStyle;

        private void OnEnable()
        {
            manager = (GMAdvertisementManager)target;
        }

        private void InitializeStyles()
        {
            if (headerStyle == null)
            {
                // Header style for SDK title
                headerStyle = new GUIStyle();
                headerStyle.fontSize = 20;
                headerStyle.fontStyle = FontStyle.Bold;
                headerStyle.normal.textColor = new Color(0.2f, 0.7f, 1f); // Bright blue
                headerStyle.alignment = TextAnchor.MiddleCenter;
                headerStyle.margin = new RectOffset(0, 0, 10, 15);

                // Sub header style
                subHeaderStyle = new GUIStyle();
                subHeaderStyle.fontSize = 14;
                subHeaderStyle.fontStyle = FontStyle.Bold;
                subHeaderStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
                subHeaderStyle.alignment = TextAnchor.MiddleCenter;
                subHeaderStyle.margin = new RectOffset(0, 0, 0, 10);

                // Label style for section headers
                labelStyle = new GUIStyle();
                labelStyle.fontSize = 13;
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                labelStyle.margin = new RectOffset(5, 0, 5, 5);

                // Box style for grouping
                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.padding = new RectOffset(15, 15, 10, 10);
                boxStyle.margin = new RectOffset(0, 0, 5, 5);
            }
        }

        public override void OnInspectorGUI()
        {
            InitializeStyles();
            
            if (serializedObject == null)
            {
                EditorGUILayout.HelpBox("SerializedObject is null", MessageType.Error);
                return;
            }
            
            serializedObject.Update();

            manager = (GMAdvertisementManager)target;
            
            if (manager == null)
            {
                EditorGUILayout.HelpBox("GMAdvertisementManager target is null", MessageType.Error);
                return;
            }

            // Main Header with gradient background
            DrawHeaderSection();
            
            EditorGUILayout.Space(15);

            // Ad Settings Section
            DrawAdSettingsSection();

            GUILayout.Space(5);

            // Callbacks Section
            DrawCallbacksSection();

            GUILayout.Space(5);

            // Status Information
            DrawStatusSection();

            GUILayout.Space(5);

            // Debug Information
            DrawDebugSection();

            // Footer
            DrawFooter();

            if (serializedObject != null)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawHeaderSection()
        {
            // Main SDK Header with gradient background
            EditorGUILayout.Space(15);
            
            // Create gradient background for header
            Rect headerRect = GUILayoutUtility.GetRect(0, 60, GUILayout.ExpandWidth(true));
            DrawGradientRect(headerRect, new Color(0.1f, 0.3f, 0.6f, 0.8f), new Color(0.2f, 0.5f, 0.8f, 0.8f));
            
            // Add border
            EditorGUI.DrawRect(new Rect(headerRect.x, headerRect.y, headerRect.width, 2), new Color(0.2f, 0.7f, 1f));
            EditorGUI.DrawRect(new Rect(headerRect.x, headerRect.yMax - 2, headerRect.width, 2), new Color(0.2f, 0.7f, 1f));
            
            // Main title
            Rect titleRect = new Rect(headerRect.x, headerRect.y + 8, headerRect.width, 25);
            GUI.Label(titleRect, "GM ADVERTISEMENT MANAGER", headerStyle);
            
            // Status subtitle
            Rect subtitleRect = new Rect(headerRect.x, headerRect.y + 30, headerRect.width, 20);
            string statusText = "Advertisement System";
            if (Application.isPlaying && manager != null)
            {
                statusText = manager.advertisement != null ? "Advertisement System - Active" : "Advertisement System - Inactive";
            }
            GUI.Label(subtitleRect, statusText, subHeaderStyle);
        }

        private new void DrawHeader()
        {
            EditorGUILayout.BeginVertical(boxStyle);
            GUILayout.Label("GM Advertisement Manager", headerStyle);
            
            if (manager != null && Application.isPlaying)
            {
                string status = manager.advertisement != null ? "Initialized" : "Not Initialized";
                GUILayout.Label(status, EditorStyles.centeredGreyMiniLabel);
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawAdSettingsSection()
        {
            EditorGUILayout.BeginVertical(boxStyle);
            
            showAdSettings = EditorGUILayout.Foldout(showAdSettings, "Ad Settings", true, labelStyle);
            
            if (showAdSettings)
            {
                EditorGUILayout.Space(5);
                
                // Game Behavior Settings
                EditorGUILayout.LabelField("Game Behavior", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                
                var pauseGameProp = serializedObject.FindProperty("pauseGameDuringAd");
                if (pauseGameProp != null)
                {
                    EditorGUILayout.PropertyField(pauseGameProp, 
                        new GUIContent("Pause Game During Ad", "Should the game be paused when showing ads?"));
                }
                else
                {
                    EditorGUILayout.LabelField("PauseGameDuringAd property not found", EditorStyles.helpBox);
                }
                
                var loadingCanvasProp = serializedObject.FindProperty("loadingAdOverlayCanvas");
                if (loadingCanvasProp != null)
                {
                    EditorGUILayout.PropertyField(loadingCanvasProp, 
                        new GUIContent("Loading Overlay Canvas", "Canvas to show while ad is loading"));
                }
                else
                {
                    EditorGUILayout.LabelField("LoadingAdOverlayCanvas property not found", EditorStyles.helpBox);
                }

                EditorGUILayout.Space(5);

                // Countdown UI Settings
                EditorGUILayout.LabelField("Countdown UI", EditorStyles.boldLabel);
                
                var countdownTextProp = serializedObject.FindProperty("countdownText");
                if (countdownTextProp != null)
                {
                    EditorGUILayout.PropertyField(countdownTextProp, 
                        new GUIContent("Countdown Text", "Text component to display countdown before ad shows (3 seconds)"));
                }
                else
                {
                    EditorGUILayout.LabelField("CountdownText property not found", EditorStyles.helpBox);
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawCallbacksSection()
        {
            EditorGUILayout.BeginVertical(boxStyle);
            
            showCallbacks = EditorGUILayout.Foldout(showCallbacks, "Event Callbacks", true, labelStyle);
            
            if (showCallbacks)
            {
                EditorGUILayout.Space(5);
                
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Runtime Actions (Set via Code)", EditorStyles.miniLabel);
                
                using (new EditorGUI.DisabledScope(true))
                {
                    if (manager != null)
                    {
                        EditorGUILayout.LabelField("Pause Action", manager.PauseAction != null ? "Set" : "Not Set", EditorStyles.miniLabel);
                        EditorGUILayout.LabelField("Resume Action", manager.ResumeAction != null ? "Set" : "Not Set", EditorStyles.miniLabel);
                        EditorGUILayout.LabelField("Reward Success Action", manager.RewardSuccessAction != null ? "Set" : "Not Set", EditorStyles.miniLabel);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Manager not available", EditorStyles.miniLabel);
                    }
                }
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawStatusSection()
        {
            if (!Application.isPlaying || manager == null) return;

            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField("Runtime Status", labelStyle);
            
            EditorGUILayout.Space(5);
            
            using (new EditorGUI.DisabledScope(true))
            {
                try
                {
                    // Ad Availability
                    bool canShowAd = manager.CanShowAd();
                    bool canShowRewardAd = manager.CanShowRewardedAd();
                    
                    EditorGUILayout.LabelField("Ad Status", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Can Show Ad", canShowAd ? "Available" : "Waiting");
                    if (!canShowAd)
                    {
                        float nextAdTime = manager.GetNextShowAdTime();
                        EditorGUILayout.LabelField("Next Ad Time", $"{nextAdTime:F1}s");
                    }
                    
                    EditorGUILayout.LabelField("Can Show Reward Ad", canShowRewardAd ? "Available" : "Waiting");
                    if (!canShowRewardAd)
                    {
                        float nextRewardTime = manager.GetNextShowRewardAdTime();
                        EditorGUILayout.LabelField("Next Reward Ad Time", $"{nextRewardTime:F1}s");
                    }

                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Countdown Status", EditorStyles.boldLabel);
                    
                    // Access private field using reflection for countdown status
                    var countdownActiveField = typeof(GMAdvertisementManager).GetField("isCountdownActive", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    bool isCountdownActive = countdownActiveField != null ? (bool)countdownActiveField.GetValue(manager) : false;
                    
                    EditorGUILayout.LabelField("Countdown Active", isCountdownActive ? "Yes" : "No");
                    if (manager.countdownText != null)
                    {
                        EditorGUILayout.LabelField("Countdown Text", "Assigned");
                        if (isCountdownActive && !string.IsNullOrEmpty(manager.countdownText.text))
                        {
                            EditorGUILayout.LabelField("Current Text", manager.countdownText.text);
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Countdown Text", "Not Assigned", EditorStyles.helpBox);
                    }
                    
                    EditorGUI.indentLevel--;
                }
                catch (System.Exception e)
                {
                    EditorGUILayout.LabelField("Error getting status: " + e.Message, EditorStyles.helpBox);
                }
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawDebugSection()
        {
            EditorGUILayout.BeginVertical(boxStyle);
            
            showDebugInfo = EditorGUILayout.Foldout(showDebugInfo, "Debug Information", true, labelStyle);
            
            if (showDebugInfo)
            {
                EditorGUILayout.Space(5);
                
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.LabelField("Component Info", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    
                    if (manager != null)
                    {
                        EditorGUILayout.LabelField("Instance ID", manager.GetInstanceID().ToString());
                        EditorGUILayout.LabelField("GameObject", manager.gameObject != null ? manager.gameObject.name : "None");
                        EditorGUILayout.LabelField("Is Singleton", GMAdvertisementManager.Instance == manager ? "Yes" : "No");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Manager not available");
                    }
                    
                    EditorGUI.indentLevel--;
                    
                    if (Application.isPlaying && manager != null)
                    {
                        EditorGUILayout.Space(5);
                        EditorGUILayout.LabelField("Runtime Info", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        
                        try
                        {
                            EditorGUILayout.LabelField("Advertisement Type", manager.advertisement?.GetType().Name ?? "None");
                            EditorGUILayout.LabelField("Time Scale", Time.timeScale.ToString("F2"));
                            EditorGUILayout.LabelField("Audio Volume", AudioListener.volume.ToString("F2"));
                        }
                        catch (System.Exception e)
                        {
                            EditorGUILayout.LabelField("Error getting runtime info: " + e.Message);
                        }
                        
                        EditorGUI.indentLevel--;
                    }
                }
                
                EditorGUILayout.Space(10);
                
                EditorGUILayout.HelpBox(
                    "Tips:\n" +
                    "• This manager should be placed on a persistent GameObject\n" +
                    "• Configure the advertisement module based on your platform\n" +
                    "• Check Console for detailed ad logs",
                    MessageType.Info
                );
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawFooter()
        {
            EditorGUILayout.Space(10);
            
            // Company footer with styling
            Rect footerRect = GUILayoutUtility.GetRect(0, 25, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(footerRect, new Color(0.1f, 0.1f, 0.1f, 0.2f));
            
            GUIStyle footerStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
            footerStyle.fontSize = 10;
            GUI.Label(footerRect, "© 2025 Azgames.io", footerStyle);
        }

        private void DrawGradientRect(Rect rect, Color color1, Color color2)
        {
            // Simple gradient effect using multiple colored rects
            int steps = 20;
            float stepHeight = rect.height / steps;
            
            for (int i = 0; i < steps; i++)
            {
                float t = (float)i / (steps - 1);
                Color currentColor = Color.Lerp(color1, color2, t);
                Rect stepRect = new Rect(rect.x, rect.y + i * stepHeight, rect.width, stepHeight + 1);
                EditorGUI.DrawRect(stepRect, currentColor);
            }
        }
    }
}
