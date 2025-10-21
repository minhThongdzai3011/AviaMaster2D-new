using System.Collections.Generic;
using System.Linq;
using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

public class AnalyticsHelperWindow : EditorWindow
{
    private class ButtonInfo
    {
        public Button button;
        public GameObject gameObject;
        public string path;
        public UIEventTracker tracker;
        public string eventName = "click-button-";
        public bool hasTracker => tracker != null;
        public UIEvent clickEvent => tracker?.events?.FirstOrDefault(e => e.pointerState == UIEventTracker.PointerState.Click);
    }

    private List<ButtonInfo> buttonInfos = new List<ButtonInfo>();
    private Vector2 scrollPosition;
    private string searchFilter = "";
    private float lastRefreshTime;
    private const float REFRESH_INTERVAL = 2f;
    private bool pendingRefresh = false; // Flag to indicate refresh is needed

    [MenuItem("GMSoft/Analytics Helper")]
    public static void ShowWindow()
    {
        var window = GetWindow<AnalyticsHelperWindow>("Analytics Helper");
        window.minSize = new Vector2(600, 400);
        window.Show();
    }

    private void OnEnable()
    {
        RefreshButtonList();
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        UnityEditor.SceneManagement.PrefabStage.prefabStageOpened += OnPrefabStageChanged;
        UnityEditor.SceneManagement.PrefabStage.prefabStageClosing += OnPrefabStageChanged;
    }

    private void OnDisable()
    {
        EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        UnityEditor.SceneManagement.PrefabStage.prefabStageOpened -= OnPrefabStageChanged;
        UnityEditor.SceneManagement.PrefabStage.prefabStageClosing -= OnPrefabStageChanged;
    }

    private void OnPrefabStageChanged(UnityEditor.SceneManagement.PrefabStage stage)
    {
        // Refresh when entering or exiting prefab mode
        pendingRefresh = true;
    }

    private void OnHierarchyChanged()
    {
        // Don't refresh immediately, just flag it for next GUI update
        pendingRefresh = true;
    }

    private void OnGUI()
    {
        // Handle pending refresh before drawing GUI
        if (pendingRefresh)
        {
            RefreshButtonList();
            pendingRefresh = false;
        }
        
        DrawToolbar();
        DrawButtonList();
        
        // Auto refresh periodically
        if (EditorApplication.timeSinceStartup - lastRefreshTime > REFRESH_INTERVAL)
        {
            RefreshButtonList();
            lastRefreshTime = (float)EditorApplication.timeSinceStartup;
        }
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        EditorGUILayout.LabelField("Search:", GUILayout.Width(50));
        searchFilter = EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField, GUILayout.Width(200));

        GUILayout.Space(10);

        // Add All / Remove All buttons
        if (GUILayout.Button("Add All", EditorStyles.toolbarButton, GUILayout.Width(60)))
        {
            AddAllUIEventTrackers();
        }

        if (GUILayout.Button("Remove All", EditorStyles.toolbarButton, GUILayout.Width(80)))
        {
            RemoveAllUIEventTrackers();
        }

        GUILayout.FlexibleSpace();

        EditorGUILayout.LabelField($"Total Buttons: {buttonInfos.Count}", GUILayout.Width(100));

        EditorGUILayout.EndHorizontal();
    }

    private void DrawButtonList()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        var filteredButtons = string.IsNullOrEmpty(searchFilter) 
            ? buttonInfos 
            : buttonInfos.Where(b => b.path.ToLower().Contains(searchFilter.ToLower())).ToList();

        if (filteredButtons.Count == 0)
        {
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            string contextMessage = prefabStage != null
                ? $"No buttons found in the current prefab: {prefabStage.prefabContentsRoot.name}"
                : "No buttons found in the current scene.";
            EditorGUILayout.HelpBox(contextMessage, MessageType.Info);
        }
        else
        {
            // Check for null buttons before drawing to avoid collection modification during iteration
            bool needsRefresh = false;
            foreach (var buttonInfo in filteredButtons.ToList()) // Create a copy to avoid modification during iteration
            {
                if (buttonInfo.button == null)
                {
                    needsRefresh = true;
                    continue;
                }
                DrawButtonItem(buttonInfo);
            }
            
            // Flag for refresh after GUI is done instead of doing it immediately
            if (needsRefresh)
            {
                pendingRefresh = true;
            }
        }
        
        EditorGUILayout.EndScrollView();
    }

    private void DrawButtonItem(ButtonInfo buttonInfo)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // Button path with status indicator
        EditorGUILayout.BeginHorizontal();
        
        // Status indicator
        string statusIcon = buttonInfo.hasTracker && buttonInfo.clickEvent != null ? "✓" : 
                           buttonInfo.hasTracker ? "⚠" : "○";
        Color statusColor = buttonInfo.hasTracker && buttonInfo.clickEvent != null ? Color.green :
                           buttonInfo.hasTracker ? Color.yellow : Color.gray;
        
        var statusStyle = new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = statusColor } };
        EditorGUILayout.LabelField(statusIcon, statusStyle, GUILayout.Width(15));
        
        // Button path - clickable to select
        var pathContent = new GUIContent(buttonInfo.path, "Click to select this button in the hierarchy");
        var buttonStyle = new GUIStyle(GUI.skin.label)
        {
            normal = { textColor = Color.white },
            hover = { textColor = new Color(0.8f, 0.8f, 0.8f) },
            alignment = TextAnchor.MiddleLeft
        };
        
        // Get the button rect before drawing
        Rect buttonRect = GUILayoutUtility.GetRect(pathContent, buttonStyle);
        
        // Always draw the button (visual only)
        GUI.Label(buttonRect, pathContent, buttonStyle);
        
        // Handle click manually to ensure it always works
        if (Event.current.type == EventType.MouseDown && 
            Event.current.button == 0 && 
            buttonRect.Contains(Event.current.mousePosition))
        {
            // Consume the event to prevent Unity from handling it
            Event.current.Use();
            
            // Luôn ping và select, kể cả khi đã được select rồi
            EditorGUIUtility.PingObject(buttonInfo.gameObject);
            Selection.activeGameObject = buttonInfo.gameObject;
            
            // Di chuyển camera đến object mà không thay đổi zoom
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.Focus();
                
                // Lấy vị trí của object
                Vector3 objectPosition = buttonInfo.gameObject.transform.position;
                
                // Chỉ di chuyển camera đến vị trí object, giữ nguyên zoom và rotation
                SceneView.lastActiveSceneView.LookAt(objectPosition);
                
                SceneView.lastActiveSceneView.Repaint();
            }
            
            // Force repaint this window too
            Repaint();
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Event configuration row
        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField("Event:", GUILayout.Width(45));
        
        EditorGUI.BeginChangeCheck();
        string newEventName = EditorGUILayout.TextField(buttonInfo.eventName);
        if (EditorGUI.EndChangeCheck())
        {
            buttonInfo.eventName = newEventName;
            
            // Update existing tracker's event name if it exists
            if (buttonInfo.hasTracker && buttonInfo.clickEvent != null)
            {
                Undo.RecordObject(buttonInfo.tracker, "Update Event Name");
                buttonInfo.clickEvent.name = newEventName;
                EditorUtility.SetDirty(buttonInfo.tracker);
            }
        }
        
        // Single action button
        if (buttonInfo.hasTracker)
        {
            if (GUILayout.Button("−", GUILayout.Width(25)))
            {
                RemoveUIEventTracker(buttonInfo);
            }
        }
        else
        {
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                AddUIEventTracker(buttonInfo);
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(1);
    }

    private void AddUIEventTracker(ButtonInfo buttonInfo)
    {
        if (buttonInfo.button == null) return;
        
        GameObject go = buttonInfo.gameObject;
        
        // Add UIEventTracker component if it doesn't exist
        UIEventTracker tracker = go.GetComponent<UIEventTracker>();
        if (tracker == null)
        {
            Undo.AddComponent<UIEventTracker>(go);
            tracker = go.GetComponent<UIEventTracker>();
        }
        
        // Initialize events list if null
        if (tracker.events == null)
        {
            tracker.events = new List<UIEvent>();
        }
        
        // Check if click event already exists
        var existingClickEvent = tracker.events.FirstOrDefault(e => e.pointerState == UIEventTracker.PointerState.Click);
        if (existingClickEvent == null)
        {
            // Create new UIEvent for Click
            var uiEvent = new UIEvent
            {
                pointerState = UIEventTracker.PointerState.Click,
                name = buttonInfo.eventName,
                analyticsLevel = 2,
                hasParams = false,
                @params = new Dictionary<string, string>()
            };
            
            Undo.RecordObject(tracker, "Add Click Event");
            tracker.events.Add(uiEvent);
        }
        else
        {
            // Update existing event name
            Undo.RecordObject(tracker, "Update Click Event");
            existingClickEvent.name = buttonInfo.eventName;
        }
        
        EditorUtility.SetDirty(tracker);
        buttonInfo.tracker = tracker;
    }

    private void RemoveUIEventTracker(ButtonInfo buttonInfo)
    {
        if (buttonInfo.tracker == null) return;
        
        Undo.DestroyObjectImmediate(buttonInfo.tracker);
        buttonInfo.tracker = null;
        pendingRefresh = true; // Flag for refresh instead of immediate refresh
    }

    private void AddAllUIEventTrackers()
    {
        // Get filtered buttons based on current search
        var filteredButtons = string.IsNullOrEmpty(searchFilter) 
            ? buttonInfos 
            : buttonInfos.Where(b => b.path.ToLower().Contains(searchFilter.ToLower())).ToList();
        
        int addedCount = 0;
        
        // Group all operations under a single undo operation
        Undo.SetCurrentGroupName("Add All UI Event Trackers");
        int undoGroup = Undo.GetCurrentGroup();
        
        foreach (var buttonInfo in filteredButtons)
        {
            if (!buttonInfo.hasTracker && buttonInfo.button != null)
            {
                AddUIEventTracker(buttonInfo);
                addedCount++;
            }
        }
        
        Undo.CollapseUndoOperations(undoGroup);
        
        if (addedCount > 0)
        {
            Debug.Log($"Added UIEventTracker to {addedCount} buttons.");
        }
        else
        {
            Debug.Log("No buttons needed UIEventTracker components.");
        }
        
        pendingRefresh = true;
    }

    private void RemoveAllUIEventTrackers()
    {
        // Get filtered buttons based on current search
        var filteredButtons = string.IsNullOrEmpty(searchFilter) 
            ? buttonInfos 
            : buttonInfos.Where(b => b.path.ToLower().Contains(searchFilter.ToLower())).ToList();
        
        int removedCount = 0;
        
        // Group all operations under a single undo operation
        Undo.SetCurrentGroupName("Remove All UI Event Trackers");
        int undoGroup = Undo.GetCurrentGroup();
        
        foreach (var buttonInfo in filteredButtons)
        {
            if (buttonInfo.hasTracker)
            {
                Undo.DestroyObjectImmediate(buttonInfo.tracker);
                buttonInfo.tracker = null;
                removedCount++;
            }
        }
        
        Undo.CollapseUndoOperations(undoGroup);
        
        if (removedCount > 0)
        {
            Debug.Log($"Removed UIEventTracker from {removedCount} buttons.");
        }
        else
        {
            Debug.Log("No buttons had UIEventTracker components to remove.");
        }
        
        pendingRefresh = true;
    }

    private void RefreshButtonList()
    {
        buttonInfos.Clear();

        // Find all buttons in the current scene or prefab being edited
        Button[] allButtons = null;

        // Check if we're in prefab mode
        var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            // We're in prefab preview mode - search within the prefab
            allButtons = prefabStage.prefabContentsRoot.GetComponentsInChildren<Button>(true);
        }
        else
        {
            // Normal scene mode
            allButtons = FindObjectsOfType<Button>(true);
        }

        foreach (var button in allButtons)
        {
            var info = new ButtonInfo
            {
                button = button,
                gameObject = button.gameObject,
                path = GetGameObjectPath(button.gameObject),
                tracker = button.GetComponent<UIEventTracker>()
            };

            // If tracker exists and has click event, get its name
            if (info.clickEvent != null && !string.IsNullOrEmpty(info.clickEvent.name))
            {
                info.eventName = info.clickEvent.name;
            }
            else
            {
                // Generate default name based on button name
                info.eventName = $"click-button-{button.name.ToLower().Replace(" ", "-")}";
            }

            buttonInfos.Add(info);
        }

        // Sort by path for better organization
        buttonInfos = buttonInfos.OrderBy(b => b.path).ToList();

        Repaint();
    }

    private string GetGameObjectPath(GameObject go)
    {
        string path = go.name;
        Transform parent = go.transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }
}

