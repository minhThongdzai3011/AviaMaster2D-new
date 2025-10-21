#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace GMSoft.Editor
{
    public class ChangelogWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private ChangelogScriptableObject changelogAsset;
        private bool isWelcomeMode = false;

        [MenuItem("GMSoft/Changelog")]
        public static ChangelogWindow ShowWindow()
        {
            var window = GetWindow<ChangelogWindow>("GMSoft Changelog");
            window.minSize = new Vector2(500, 600);
            window.isWelcomeMode = false;
            window.LoadChangelogAsset();
            return window;
        }

        public static ChangelogWindow ShowWelcomeWindow()
        {
            // Modified to show the regular changelog window but with modal behavior
            var window = GetWindow<ChangelogWindow>(true, "GMSoft SDK Changelog", true);
            window.minSize = new Vector2(550, 650);
            window.maxSize = new Vector2(750, 850);
            window.isWelcomeMode = true; // Keep this to show the Get Started button
            
            // Center the window
            var main = EditorGUIUtility.GetMainWindowPosition();
            var pos = window.position;
            float centerX = main.x + (main.width - pos.width) * 0.5f;
            float centerY = main.y + (main.height - pos.height) * 0.5f;
            window.position = new Rect(centerX, centerY, pos.width, pos.height);
            
            window.LoadChangelogAsset();
            return window;
        }

        private void LoadChangelogAsset()
        {
            // Try to find any ChangelogScriptableObject in the project
            string[] guids = AssetDatabase.FindAssets("t:ChangelogScriptableObject");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                changelogAsset = AssetDatabase.LoadAssetAtPath<ChangelogScriptableObject>(path);
            }

            // If not found, try the default path
            if (changelogAsset == null)
            {
                changelogAsset = AssetDatabase.LoadAssetAtPath<ChangelogScriptableObject>("Assets/GMSoft/Editor/GMSoft_Changelog.asset");
            }
        }

        private void OnGUI()
        {
            // Load asset if not loaded yet
            if (changelogAsset == null)
            {
                LoadChangelogAsset();
            }
            
            if (isWelcomeMode)
            {
                DrawWelcomeMode();
            }
            else
            {
                DrawNormalMode();
            }
        }

        // Modified to show regular changelog window without special welcome mode
        private void DrawWelcomeMode()
        {
            // Use the regular changelog display mode instead of welcome content
            DrawHeader();
            
            if (changelogAsset == null)
            {
                DrawNoAssetMessage();
                return;
            }

            DrawChangelogContent();
            
            // Add a simple "Get Started" button at the bottom
            EditorGUILayout.Space(10);
            var buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 35
            };
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Get Started", buttonStyle, GUILayout.Width(150)))
            {
                Close();
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawNormalMode()
        {
            DrawHeader();
            
            if (changelogAsset == null)
            {
                DrawNoAssetMessage();
                return;
            }

            DrawChangelogContent();
        }

        private void DrawHeader()
        {
            GUILayout.Space(15);
            
            // Create gradient background for header
            Rect headerRect = GUILayoutUtility.GetRect(0, 60, GUILayout.ExpandWidth(true));
            DrawGradientRect(headerRect, new Color(0.1f, 0.3f, 0.6f, 0.8f), new Color(0.2f, 0.5f, 0.8f, 0.8f));
            
            // Add border
            EditorGUI.DrawRect(new Rect(headerRect.x, headerRect.y, headerRect.width, 2), new Color(0.2f, 0.7f, 1f));
            EditorGUI.DrawRect(new Rect(headerRect.x, headerRect.yMax - 2, headerRect.width, 2), new Color(0.2f, 0.7f, 1f));
            
            // Main title
            GUIStyle titleStyle = new GUIStyle();
            titleStyle.fontSize = 20;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.textColor = new Color(0.2f, 0.7f, 1f);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            
            Rect titleRect = new Rect(headerRect.x, headerRect.y + 8, headerRect.width, 25);
            GUI.Label(titleRect, "GMSoft SDK Changelog", titleStyle);
            
            // Subtitle
            GUIStyle subtitleStyle = new GUIStyle();
            subtitleStyle.fontSize = 12;
            subtitleStyle.fontStyle = FontStyle.Bold;
            subtitleStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
            subtitleStyle.alignment = TextAnchor.MiddleCenter;
            
            string gitUrl = changelogAsset?.repositoryUrl ?? "http://123.24.143.6:8080/scm/git/gmsoft-sdk";
            string branch = changelogAsset?.branchName ?? "gmsoft-sdk-v7";
            
            Rect subtitleRect = new Rect(headerRect.x, headerRect.y + 35, headerRect.width, 20);
            GUI.Label(subtitleRect, $"{gitUrl} | {branch}", subtitleStyle);
            
            GUILayout.Space(15);
        }

        private void DrawNoAssetMessage()
        {
            GUILayout.Space(30);
            
            EditorGUILayout.HelpBox("⚠️ No changelog asset found!\n\nCreate a changelog asset to display version history.", MessageType.Warning);
            
            GUILayout.Space(20);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Create Changelog Asset", GUILayout.Width(200), GUILayout.Height(30)))
            {
                EditorUtility.DisplayDialog("Create Changelog Asset", 
                    "Right-click in Project window and select:\nCreate → GMSoft → Changelog Data", 
                    "OK");
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawChangelogContent()
        {
            if (changelogAsset?.entries == null || changelogAsset.entries.Count == 0)
            {
                GUILayout.Space(50);
                
                EditorGUILayout.HelpBox("📝 No changelog entries found\n\nEdit the asset to add version history", MessageType.Info);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            GUILayout.Space(10);
            
            for (int i = 0; i < changelogAsset.entries.Count; i++)
            {
                DrawChangelogEntry(changelogAsset.entries[i], i == 0);
                GUILayout.Space(15);
            }
            
            GUILayout.Space(20);
            EditorGUILayout.EndScrollView();
        }

        private void DrawChangelogEntry(ChangelogScriptableObject.ChangelogEntryData entry, bool isLatest)
        {
            // Use EditorStyles.helpBox similar to PublishWindow for consistent styling
            var boxStyle = new GUIStyle(EditorStyles.helpBox);
            boxStyle.padding = new RectOffset(20, 20, 15, 15);
            boxStyle.margin = new RectOffset(7, 7, 7, 7); // Increased margin for border space
            
            // Add bright border by modifying the box style
            if (isLatest)
            {
                // Bright cyan border for latest version
                boxStyle.border = new RectOffset(3, 3, 3, 3);
                var originalBG = GUI.backgroundColor;
                GUI.backgroundColor = EditorGUIUtility.isProSkin ? 
                    new Color(0.3f, 0.5f, 0.8f, 0.3f) : 
                    new Color(0.7f, 0.9f, 1f, 0.5f);
                
                EditorGUILayout.BeginVertical(boxStyle);
                GUI.backgroundColor = originalBG;
            }
            else
            {
                // Bright blue border for other versions
                boxStyle.border = new RectOffset(2, 2, 2, 2);
                EditorGUILayout.BeginVertical(boxStyle);
            }
            
            // Version header
            EditorGUILayout.BeginHorizontal();
            
            var versionStyle = new GUIStyle(EditorStyles.boldLabel);
            versionStyle.fontSize = 16;
            
            if (isLatest)
            {
                // Latest version styling
                versionStyle.normal.textColor = EditorGUIUtility.isProSkin ? 
                    new Color(1f, 0.8f, 0.2f) : 
                    new Color(0.8f, 0.4f, 0f);
                GUILayout.Label($"🌟 {entry.version} (Latest)", versionStyle);
            }
            else
            {
                // Regular version styling
                versionStyle.normal.textColor = EditorGUIUtility.isProSkin ? 
                    new Color(0.4f, 0.8f, 1f) : 
                    new Color(0.2f, 0.4f, 0.8f);
                GUILayout.Label($"🚀 {entry.version}", versionStyle);
            }
            
            GUILayout.FlexibleSpace();
            
            // Date with good contrast
            var dateStyle = new GUIStyle(EditorStyles.miniLabel);
            dateStyle.normal.textColor = EditorGUIUtility.isProSkin ? 
                new Color(0.8f, 0.8f, 0.8f) : 
                new Color(0.3f, 0.3f, 0.3f);
            GUILayout.Label($"📅 {entry.date}", dateStyle);
            
            EditorGUILayout.EndHorizontal();
            
            // Description with proper text contrast
            if (!string.IsNullOrEmpty(entry.description))
            {
                GUILayout.Space(10);
                
                var descStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
                descStyle.fontSize = 12;
                descStyle.normal.textColor = EditorGUIUtility.isProSkin ? 
                    new Color(0.9f, 0.9f, 0.9f) : 
                    new Color(0.1f, 0.1f, 0.1f);
                descStyle.padding = new RectOffset(0, 0, 0, 0);
                
                EditorGUILayout.LabelField(entry.description, descStyle);
            }
            
            EditorGUILayout.EndVertical(); // Close the helpBox vertical group
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

        // This method is no longer needed as we're using the regular header
        // It's kept for compatibility but not used
        private void DrawWelcomeHeader()
        {
            // Not used anymore, regular header display is used instead
        }

        // This method is no longer needed as we're using the regular changelog display
        // It's kept for compatibility but not used
        private void DrawWelcomeContent()
        {
            // Not used anymore, regular changelog display is used instead
        }

        // This method is no longer needed as we're using a simpler button in DrawWelcomeMode
        // It's kept for compatibility but not used
        private void DrawWelcomeButtons()
        {
            // Not used anymore, a simpler "Get Started" button is shown in DrawWelcomeMode
        }
    }
}
#endif
