using UnityEngine;
using UnityEditor;

namespace GMSoft.Editor
{
    [CustomEditor(typeof(GMSDK))]
    public class GMSDKEditor : UnityEditor.Editor
    {
        private GUIStyle headerStyle;
        private GUIStyle subHeaderStyle;
        private GUIStyle labelStyle;
        private GUIStyle boxStyle;
        private GUIStyle buttonStyle;
        private Texture2D logoTexture;
        private bool showAdvancedSettings = false;

        void OnEnable()
        {
            // Remove style initialization from here to avoid GUI error
        }

        void InitializeStyles()
        {
            // Only initialize if styles are null to avoid recreating every frame
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

                // Button style
                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontSize = 12;
                buttonStyle.fontStyle = FontStyle.Bold;
            }
        }

        public override void OnInspectorGUI()
        {
            // Initialize styles here to avoid GUI errors
            InitializeStyles();
            
            GMSDK gmsdk = (GMSDK)target;

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
            GUI.Label(titleRect, GMSDKConfig.Instance.SDKName.ToUpper(), headerStyle);
            
            // Version subtitle
            Rect subtitleRect = new Rect(headerRect.x, headerRect.y + 30, headerRect.width, 20);
            GUI.Label(subtitleRect, GMSDKConfig.Instance.DisplayVersionString, subHeaderStyle);
            
            EditorGUILayout.Space(15);

            // Configuration Section
            DrawConfigurationSection();

            // UI References Section
            DrawUIReferencesSection();

            // Footer with company info
            DrawFooter();

            // Apply changes
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConfigurationSection()
        {
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField("Configuration", labelStyle);
            
            // Target Scene
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetScene"), new GUIContent("Target Scene", "Scene to load when SDK initialization is complete"));
            
            // Auto-fill button for target scene
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Auto-fill Scene Index 1", buttonStyle, GUILayout.Width(150)))
            {
                AutoFillTargetSceneFromBuildSettings();
            }
            EditorGUILayout.EndHorizontal();
            
            // Display read-only information about the fail request counter - Hidden
            // EditorGUILayout.BeginHorizontal();
            // EditorGUILayout.LabelField("Fail Request Counter:", GUILayout.Width(150));
            // EditorGUILayout.LabelField("12 (Constant)", EditorStyles.helpBox);
            // EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        private void DrawUIReferencesSection()
        {
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField("UI References", labelStyle);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gameLockedCanvas"), new GUIContent("Game Locked Canvas"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gameLockedLabel"), new GUIContent("Game Locked Label"));
            
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

        private void AutoFillTargetSceneFromBuildSettings()
        {
            // Get scenes from build settings
            var scenes = EditorBuildSettings.scenes;
            
            if (scenes != null && scenes.Length > 1)
            {
                // Get scene at index 1 (second scene in build settings)
                var sceneAtIndex1 = scenes[1];
                
                if (sceneAtIndex1.enabled && !string.IsNullOrEmpty(sceneAtIndex1.path))
                {
                    // Extract scene name from path (remove Assets/ and .unity extension)
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(sceneAtIndex1.path);
                    
                    // Set the target scene property
                    SerializedProperty targetSceneProperty = serializedObject.FindProperty("targetScene");
                    if (targetSceneProperty != null)
                    {
                        targetSceneProperty.stringValue = sceneName;
                        serializedObject.ApplyModifiedProperties();
                        
                        Debug.Log($"Target scene auto-filled with: {sceneName}");
                    }
                }
                else
                {
                    Debug.LogWarning("Scene at index 1 in build settings is disabled or has no path.");
                }
            }
            else
            {
                Debug.LogWarning("No scene found at index 1 in build settings. Please make sure you have at least 2 scenes in your build settings.");
            }
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