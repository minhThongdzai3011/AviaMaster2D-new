using UnityEngine;

[CreateAssetMenu(fileName = "GMSDKConfig", menuName = "GMSoft/SDK Config", order = 1)]
public class GMSDKConfig : ScriptableObject
{
    [Header("Version Information")]
    [SerializeField] private string sdkVersion = "7.0";
    [SerializeField] private string sdkName = "GMSoft SDK";
    [SerializeField] private string targetPlatform = "WebGL Platform";

    [Header("Build Information")]
    [SerializeField] private int buildNumber = 1;
    [SerializeField] private string releaseType = "Release";

    private static GMSDKConfig _instance;

    public static GMSDKConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GMSDKConfig>("GMSDKConfig");
                if (_instance == null)
                {
                    Debug.LogWarning("[GMSDKConfig] Config not found in Resources folder. Creating default config.");
                    _instance = CreateInstance<GMSDKConfig>();
                }
            }
            return _instance;
        }
    }

    public string SDKVersion => sdkVersion;
    public string SDKName => sdkName;
    public string TargetPlatform => targetPlatform;
    public int BuildNumber => buildNumber;
    public string ReleaseType => releaseType;

    public string FullVersionString => $"{sdkVersion}.{buildNumber}";
    public string DisplayVersionString => $"{sdkName} v{FullVersionString} - {targetPlatform}";
    public string ShortDisplayVersion => $"v{sdkVersion}";

    public void IncrementBuildNumber()
    {
        buildNumber++;
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public void SetVersion(string version)
    {
        sdkVersion = version;
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public void SetReleaseType(string type)
    {
        releaseType = type;
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}