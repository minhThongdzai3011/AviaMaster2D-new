using UnityEngine;
using UnityEditor;
using System.IO;

namespace GMSoft.Editor
{
    [CustomEditor(typeof(GMSDKConfig))]
    public class GMSDKConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GMSDKConfig config = (GMSDKConfig)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Version Display", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Full Version", config.FullVersionString);
            EditorGUILayout.TextField("Display Version", config.DisplayVersionString);
            EditorGUILayout.TextField("Short Version", config.ShortDisplayVersion);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            if (GUILayout.Button("Increment Build Number"))
            {
                config.IncrementBuildNumber();
                EditorUtility.SetDirty(config);
            }
        }
    }

    [InitializeOnLoad]
    public class GMSDKConfigInitializer
    {
        static GMSDKConfigInitializer()
        {
            EditorApplication.delayCall += EnsureConfigExists;
        }

        private static void EnsureConfigExists()
        {
            string resourcesPath = "Assets/GMSoft/Resources";
            string configPath = Path.Combine(resourcesPath, "GMSDKConfig.asset");

            if (!File.Exists(configPath))
            {
                if (!Directory.Exists(resourcesPath))
                {
                    Directory.CreateDirectory(resourcesPath);
                }

                GMSDKConfig config = ScriptableObject.CreateInstance<GMSDKConfig>();
                AssetDatabase.CreateAsset(config, configPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"<b><color=green>[GMSDKConfig]</color></b> Created default config at: {configPath}");
            }
        }

        [MenuItem("GMSoft/Version/Open SDK Config")]
        private static void OpenSDKConfig()
        {
            string configPath = "Assets/GMSoft/Resources/GMSDKConfig.asset";
            GMSDKConfig config = AssetDatabase.LoadAssetAtPath<GMSDKConfig>(configPath);

            if (config == null)
            {
                EnsureConfigExists();
                config = AssetDatabase.LoadAssetAtPath<GMSDKConfig>(configPath);
            }

            if (config != null)
            {
                Selection.activeObject = config;
                EditorUtility.FocusProjectWindow();
            }
            else
            {
                Debug.LogError("[GMSDKConfig] Could not create or find SDK config asset.");
            }
        }

        [MenuItem("GMSoft/Version/Show Current Version")]
        private static void ShowCurrentVersion()
        {
            GMSDKConfig config = GMSDKConfig.Instance;
            EditorUtility.DisplayDialog("GMSoft SDK Version",
                $"SDK Version: {config.FullVersionString}\n" +
                $"Display: {config.DisplayVersionString}\n" +
                $"Build Number: {config.BuildNumber}\n" +
                $"Release Type: {config.ReleaseType}",
                "OK");
        }

    }
}