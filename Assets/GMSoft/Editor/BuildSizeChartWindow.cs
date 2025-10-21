#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GMSoft.Editor
{
    [Serializable]
    public class BuildSizeData
    {
        public DateTime buildTime;
        public long sizeInBytes;
        public string buildName;
        
        public BuildSizeData(DateTime time, long size, string name)
        {
            buildTime = time;
            sizeInBytes = size;
            buildName = name;
        }
        
        public string GetFormattedSize()
        {
            if (sizeInBytes < 1024) return $"{sizeInBytes} B";
            if (sizeInBytes < 1024 * 1024) return $"{sizeInBytes / 1024f:F1} KB";
            if (sizeInBytes < 1024 * 1024 * 1024) return $"{sizeInBytes / (1024f * 1024f):F1} MB";
            return $"{sizeInBytes / (1024f * 1024f * 1024f):F2} GB";
        }
    }

    public class BuildSizeChartWindow : EditorWindow
    {
        private List<BuildSizeData> buildHistory = new List<BuildSizeData>();
        private int selectedCount = 10;
        private readonly int[] countOptions = { 5, 10, 15, 20, 25, 30 };
        private readonly string[] countLabels = { "5", "10", "15", "20", "25", "30" };
        
        private Vector2 scrollPosition;
        private GUIStyle chartStyle;
        private GUIStyle headerStyle;
        private GUIStyle tooltipStyle;
        private bool stylesInitialized = false;
        
        // Chart drawing variables
        private Rect chartRect;
        private Vector2 hoveredPoint = Vector2.zero;
        private bool isHovering = false;
        private BuildSizeData hoveredBuild;
        
        // Animation variables
        private float animationTime = 0f;
        private bool isAnimating = false;
        private float lastUpdateTime = 0f;
        private const float POINT_ANIMATION_DURATION = 0.4f; // Faster point animation
        private const float DELAY_PER_POINT = 0.03f; // Faster delay between points
        private const float LINE_ANIMATION_DURATION = 0.6f; // Duration for line drawing animation
        
        private float GetEasedProgress(float t)
        {
            // Use smooth ease-out cubic for better line quality
            t = Mathf.Clamp01(t);
            return 1 - Mathf.Pow(1 - t, 3);
        }

        public static void ShowWindow()
        {
            var window = GetWindow<BuildSizeChartWindow>();
            
            // Try to get a chart-related icon, fallback to null if not available
            Texture2D windowIcon = null;
            try
            {
                // Try common chart/graph related icons
                var iconNames = new string[] 
                {
                    "Profiler.NetworkMessages",
                    "Profiler.Memory", 
                    "UnityEditor.Graphs.AnimatorControllerTool",
                    "d_UnityEditor.AnimationWindow",
                    "UnityEditor.Timeline.TimelineWindow"
                };
                
                foreach (var iconName in iconNames)
                {
                    var iconContent = EditorGUIUtility.IconContent(iconName);
                    if (iconContent != null && iconContent.image != null)
                    {
                        windowIcon = iconContent.image as Texture2D;
                        break;
                    }
                }
            }
            catch
            {
                // If all icons fail, use null (no icon)
                windowIcon = null;
            }
            
            window.titleContent = new GUIContent("Build Size Chart");
            window.minSize = new Vector2(600, 400);
            window.LoadBuildHistory();
            window.Show();
        }
        
        public static void RefreshAllWindows()
        {
            // Find and refresh any open BuildSizeChartWindow
            var windows = Resources.FindObjectsOfTypeAll<BuildSizeChartWindow>();
            BuildSizeChartWindow windowToFocus = null;
            
            foreach (var window in windows)
            {
                if (window != null)
                {
                    window.RefreshData();
                    // Store reference to focus later (focus the first found window)
                    if (windowToFocus == null)
                    {
                        windowToFocus = window;
                    }
                }
            }
            
            // Auto focus to chart window after build completes - use longer delay to avoid GUI conflicts
            if (windowToFocus != null)
            {
                // Use longer delay and multiple frame wait to ensure GUI is stable
                EditorApplication.delayCall += () =>
                {
                    EditorApplication.delayCall += () =>
                    {
                        if (windowToFocus != null)
                        {
                            windowToFocus.Show(); // Ensure window is visible
                            windowToFocus.Focus(); // Bring to front and focus
                        }
                    };
                };
            }
        }

        private void OnEnable()
        {
            LoadBuildHistory();
            StartAnimation();
        }
        
        private void StartAnimation()
        {
            animationTime = 0f;
            isAnimating = true;
            lastUpdateTime = Time.realtimeSinceStartup;
        }
        
        public void RefreshData()
        {
            LoadBuildHistory();
            StartAnimation();
            Repaint();
            
            // Show notification that data was updated
            ShowNotification(new GUIContent("Build data updated!"));
        }

        private void InitializeStyles()
        {
            if (stylesInitialized) return;

            headerStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black }
            };

            chartStyle = new GUIStyle()
            {
                normal = { background = MakeTexture(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.2f)) }
            };

            tooltipStyle = new GUIStyle()
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { 
                    textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black,
                    background = MakeTexture(16, 16, EditorGUIUtility.isProSkin ? new Color(0.05f, 0.05f, 0.05f, 1.0f) : new Color(1.0f, 1.0f, 1.0f, 1.0f))
                },
                border = new RectOffset(8, 8, 8, 8),
                padding = new RectOffset(12, 12, 10, 10)
            };

            stylesInitialized = true;
        }

        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = color;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void OnGUI()
        {
            InitializeStyles();

            // Get recent builds count for consistent animation timing
            var recentBuilds = buildHistory.OrderByDescending(b => b.buildTime).Take(selectedCount).OrderBy(b => b.buildTime).ToList();
            int animationBuildCount = recentBuilds.Count;

            // Update animation with more precise timing
            if (isAnimating)
            {
                float deltaTime = Time.realtimeSinceStartup - lastUpdateTime;
                animationTime += deltaTime;
                
                // Calculate total animation time including delays and two phases - use actual displayed build count
                float pointPhaseTime = POINT_ANIMATION_DURATION + (animationBuildCount > 0 ? (animationBuildCount - 1) * DELAY_PER_POINT : 0);
                float totalAnimationTime = pointPhaseTime + LINE_ANIMATION_DURATION;
                
                if (animationTime >= totalAnimationTime)
                {
                    animationTime = totalAnimationTime;
                    isAnimating = false;
                }
                
                // Force continuous repaint during animation
                if (Event.current.type == EventType.Repaint)
                {
                    EditorApplication.QueuePlayerLoopUpdate();
                }
                Repaint();
            }
            lastUpdateTime = Time.realtimeSinceStartup;

            EditorGUILayout.Space(15);

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
            GUI.Label(titleRect, "Build Size History Chart", titleStyle);
            
            // Subtitle
            GUIStyle subtitleStyle = new GUIStyle();
            subtitleStyle.fontSize = 12;
            subtitleStyle.fontStyle = FontStyle.Bold;
            subtitleStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
            subtitleStyle.alignment = TextAnchor.MiddleCenter;
            
            Rect subtitleRect = new Rect(headerRect.x, headerRect.y + 35, headerRect.width, 20);
            GUI.Label(subtitleRect, "Track your WebGL build sizes over time", subtitleStyle);
            
            EditorGUILayout.Space(15);
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField($"Total builds in history: {buildHistory.Count}");
            
            // Controls with toolbar style (like enum toggle)
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Show recent builds:", GUILayout.Width(120));
            
            // Create toolbar (enum-style toggle buttons)
            int currentIndex = Array.IndexOf(countOptions, selectedCount);
            int newSelectedIndex = GUILayout.Toolbar(currentIndex, countLabels, GUILayout.Width(160));
            
            if (newSelectedIndex >= 0 && newSelectedIndex < countOptions.Length && newSelectedIndex != currentIndex)
            {
                selectedCount = countOptions[newSelectedIndex];
                StartAnimation(); // Restart animation when selection changes
            }
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Chart area
            if (buildHistory.Count > 0)
            {
                DrawChart();
            }
            else
            {
                EditorGUILayout.HelpBox("No build history found. Build your project to see size data.", MessageType.Info);
            }
        }

        private void DrawChart()
        {
            var recentBuilds = buildHistory.OrderByDescending(b => b.buildTime).Take(selectedCount).OrderBy(b => b.buildTime).ToList();
            
            if (recentBuilds.Count == 0)
            {
                EditorGUILayout.HelpBox("No builds to display", MessageType.Info);
                return;
            }
            
            if (recentBuilds.Count == 1)
            {
                EditorGUILayout.HelpBox($"Only 1 build found. Add more builds to see chart trend. Current build: {recentBuilds[0].buildName} ({recentBuilds[0].GetFormattedSize()})", MessageType.Info);
                return;
            }

            // Chart area
            GUILayout.BeginVertical(chartStyle);
            chartRect = GUILayoutUtility.GetRect(0, 300, GUILayout.ExpandWidth(true));
            chartRect = new Rect(chartRect.x + 60, chartRect.y + 20, chartRect.width - 80, chartRect.height - 60);
            
            // Draw axes and labels - pass full build history for correct numbering
            DrawAxes(chartRect, recentBuilds, buildHistory);
            
            // Calculate animation progress for two-phase animation - use consistent timing
            float pointPhaseTime = POINT_ANIMATION_DURATION + (recentBuilds.Count > 0 ? (recentBuilds.Count - 1) * DELAY_PER_POINT : 0);
            float totalAnimationTime = pointPhaseTime + LINE_ANIMATION_DURATION;
            float rawProgress = isAnimating ? Mathf.Clamp01(animationTime / totalAnimationTime) : 1f;
            
            // Handle mouse hover BEFORE drawing chart
            HandleMouseHover(chartRect, recentBuilds, rawProgress);
            
            // Draw chart line and points with two-phase animation
            DrawChartLine(chartRect, recentBuilds, rawProgress, pointPhaseTime, totalAnimationTime);
            
            GUILayout.EndVertical();

            // Show tooltip if hovering
            if (isHovering && hoveredBuild != null)
            {
                ShowTooltip();
            }
        }

        private void DrawAxes(Rect rect, List<BuildSizeData> builds, List<BuildSizeData> fullHistory)
        {
            if (builds.Count == 0) return;

            long maxSize = builds.Max(b => b.sizeInBytes);
            long minSize = builds.Min(b => b.sizeInBytes);
            
            // Ensure we have a reasonable range for Y axis
            long sizeRange = maxSize - minSize;
            if (sizeRange < 1024 * 1024) // Less than 1MB difference
            {
                long avgSize = (maxSize + minSize) / 2;
                minSize = Math.Max(0, avgSize - 5 * 1024 * 1024); // -5MB from average
                maxSize = avgSize + 5 * 1024 * 1024; // +5MB from average
            }

            Handles.BeginGUI();
            
            // Set axis color
            Handles.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            
            // Draw Y-axis (Size)
            Handles.DrawLine(
                new Vector3(rect.x, rect.y), 
                new Vector3(rect.x, rect.y + rect.height)
            );
            
            // Draw X-axis (Build Number)
            Handles.DrawLine(
                new Vector3(rect.x, rect.y + rect.height), 
                new Vector3(rect.x + rect.width, rect.y + rect.height)
            );

            // Draw Y-axis labels (Size) - no animation for axis labels
            for (int i = 0; i <= 5; i++)
            {
                float y = rect.y + rect.height - (rect.height / 5) * i;
                long sizeAtY = minSize + (maxSize - minSize) * i / 5;
                
                // Draw tick mark
                Handles.DrawLine(
                    new Vector3(rect.x - 5, y), 
                    new Vector3(rect.x, y)
                );
                
                // Draw size label (static, no animation)
                string sizeLabel = FormatBytes(sizeAtY);
                Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(sizeLabel));
                GUI.Label(new Rect(rect.x - labelSize.x - 10, y - labelSize.y/2, labelSize.x, labelSize.y), sizeLabel);
            }

            // Draw X-axis labels (Build Time Timeline with even spacing)
            // Always show labels for all builds if <= 6, otherwise show evenly spaced labels
            if (builds.Count <= 6)
            {
                // Show label for each build with time at even spacing positions
                for (int i = 0; i < builds.Count; i++)
                {
                    float normalizedPosition = builds.Count > 1 ? (float)i / (builds.Count - 1) : 0.5f;
                    float x = rect.x + rect.width * normalizedPosition;
                    
                    // Draw tick mark
                    Handles.DrawLine(
                        new Vector3(x, rect.y + rect.height), 
                        new Vector3(x, rect.y + rect.height + 5)
                    );
                    
                    // Format time label based on time span
                    string timeLabel = GetFormattedTimeLabel(builds[i].buildTime, builds);
                    Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(timeLabel));
                    
                    // Use smaller font for time labels to fit better
                    GUIStyle timeStyle = new GUIStyle(GUI.skin.label);
                    timeStyle.fontSize = 10;
                    
                    GUI.Label(new Rect(x - labelSize.x/2, rect.y + rect.height + 10, labelSize.x, labelSize.y), timeLabel, timeStyle);
                }
            }
            else
            {
                // Show 6 evenly spaced labels when there are many builds
                int maxLabels = 6;
                for (int i = 0; i < maxLabels; i++)
                {
                    float normalizedPosition = (float)i / (maxLabels - 1);
                    float x = rect.x + rect.width * normalizedPosition;
                    int buildIndexInFiltered = (builds.Count - 1) * i / (maxLabels - 1);
                    
                    // Draw tick mark
                    Handles.DrawLine(
                        new Vector3(x, rect.y + rect.height), 
                        new Vector3(x, rect.y + rect.height + 5)
                    );
                    
                    // Format time label for selected build
                    string timeLabel = GetFormattedTimeLabel(builds[buildIndexInFiltered].buildTime, builds);
                    Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(timeLabel));
                    
                    // Use smaller font for time labels to fit better
                    GUIStyle timeStyle = new GUIStyle(GUI.skin.label);
                    timeStyle.fontSize = 10;
                    
                    GUI.Label(new Rect(x - labelSize.x/2, rect.y + rect.height + 10, labelSize.x, labelSize.y), timeLabel, timeStyle);
                }
            }

            // Draw grid lines (static, no animation) - align with chart points
            Handles.color = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.1f) : new Color(0, 0, 0, 0.1f);
            
            // Horizontal grid lines
            for (int i = 1; i < 5; i++)
            {
                float y = rect.y + (rect.height / 5) * i;
                Handles.DrawLine(new Vector3(rect.x, y), new Vector3(rect.x + rect.width, y));
            }
            
            // Vertical grid lines - align with actual chart points with even spacing
            int buildCount = builds.Count;
            if (buildCount > 2)
            {
                for (int i = 1; i < buildCount - 1; i++)
                {
                    float normalizedPosition = (float)i / (buildCount - 1);
                    float x = rect.x + rect.width * normalizedPosition;
                    Handles.DrawLine(new Vector3(x, rect.y), new Vector3(x, rect.y + rect.height));
                }
            }
            
            Handles.EndGUI();
        }

        private string FormatBytes(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024f:F0} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024f * 1024f):F1} MB";
            return $"{bytes / (1024f * 1024f * 1024f):F2} GB";
        }

        private string GetFormattedTimeLabel(DateTime buildTime, List<BuildSizeData> allBuilds)
        {
            if (allBuilds.Count <= 1)
                return buildTime.ToString("MM/dd HH:mm");

            // Calculate time span of all builds to determine appropriate format
            var orderedBuilds = allBuilds.OrderBy(b => b.buildTime).ToList();
            var timeSpan = orderedBuilds.Last().buildTime - orderedBuilds.First().buildTime;

            if (timeSpan.TotalDays > 30)
            {
                // More than 30 days: show month/day
                return buildTime.ToString("MM/dd");
            }
            else if (timeSpan.TotalDays > 7)
            {
                // More than 7 days: show month/day and hour
                return buildTime.ToString("MM/dd HH:mm");
            }
            else if (timeSpan.TotalDays > 1)
            {
                // More than 1 day: show day and time
                return buildTime.ToString("dd HH:mm");
            }
            else
            {
                // Same day or less: show time only
                return buildTime.ToString("HH:mm");
            }
        }

        private void DrawGrid(Rect rect)
        {
            Handles.BeginGUI();
            Handles.color = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.1f) : new Color(0, 0, 0, 0.1f);
            
            // Vertical lines
            for (int i = 1; i < 5; i++)
            {
                float x = rect.x + (rect.width / 5) * i;
                Handles.DrawLine(new Vector3(x, rect.y), new Vector3(x, rect.y + rect.height));
            }
            
            // Horizontal lines
            for (int i = 1; i < 5; i++)
            {
                float y = rect.y + (rect.height / 5) * i;
                Handles.DrawLine(new Vector3(rect.x, y), new Vector3(rect.x + rect.width, y));
            }
            
            Handles.EndGUI();
        }

        private void DrawChartLine(Rect rect, List<BuildSizeData> builds, float animProgress, float pointPhaseTime, float totalAnimationTime)
        {
            if (builds.Count < 2) return;

            long maxSize = builds.Max(b => b.sizeInBytes);
            long minSize = builds.Min(b => b.sizeInBytes);
            long sizeRange = maxSize - minSize;
            
            // Ensure we have a reasonable range
            if (sizeRange < 1024 * 1024) // Less than 1MB difference
            {
                long avgSize = (maxSize + minSize) / 2;
                minSize = Math.Max(0, avgSize - 5 * 1024 * 1024);
                maxSize = avgSize + 5 * 1024 * 1024;
                sizeRange = maxSize - minSize;
            }

            Handles.BeginGUI();
            
            // Phase 1: Draw points with cascade animation
            float pointPhaseProgress = Mathf.Clamp01(animationTime / pointPhaseTime);
            
            for (int i = 0; i < builds.Count; i++)
            {
                // Calculate delay for this specific point
                float delayPerPoint = DELAY_PER_POINT;
                float pointStartTime = i * delayPerPoint;
                float pointEndTime = pointStartTime + POINT_ANIMATION_DURATION;
                
                // Check if this point's animation should be active
                if (animationTime > pointStartTime)
                {
                    // Calculate individual point progress
                    float individualPointProgress = Mathf.Clamp01((animationTime - pointStartTime) / POINT_ANIMATION_DURATION);
                    
                    Vector2 point = GetAnimatedChartPoint(rect, builds[i], i, builds.Count, minSize, sizeRange, individualPointProgress);
                    
                    // Check if this point is being hovered - use more robust comparison
                    bool isHovered = hoveredBuild != null && 
                                   (hoveredBuild == builds[i] || 
                                    (hoveredBuild.buildTime == builds[i].buildTime && 
                                     hoveredBuild.sizeInBytes == builds[i].sizeInBytes &&
                                     hoveredBuild.buildName == builds[i].buildName));
                    
                    // Set color based on hover state with more visible colors
                    Handles.color = isHovered ? Color.red : Color.white; // Using red for better visibility
                    
                    // Draw larger point if hovered
                    float pointSize = isHovered ? 8f : 4f; // Larger size difference
                    Handles.DrawSolidDisc(point, Vector3.forward, pointSize);
                }
            }
            
            // Phase 2: Draw smooth lines after all points are complete
            if (animationTime > pointPhaseTime)
            {
                float lineProgress = Mathf.Clamp01((animationTime - pointPhaseTime) / LINE_ANIMATION_DURATION);
                
                // Draw smooth line using multiple segments for better quality
                DrawSmoothLine(rect, builds, lineProgress, minSize, sizeRange);
            }
            
            Handles.EndGUI();
        }
        
        private void DrawSmoothLine(Rect rect, List<BuildSizeData> builds, float lineProgress, long minSize, long sizeRange)
        {
            if (builds.Count < 2) return;
            
            // Set line color and thickness
            Handles.color = Color.cyan;
            
            // Calculate all final points first
            Vector2[] points = new Vector2[builds.Count];
            for (int i = 0; i < builds.Count; i++)
            {
                points[i] = GetFinalChartPoint(rect, builds[i], i, builds.Count, minSize, sizeRange);
            }
            
            // Draw line segments with smooth animation
            for (int i = 0; i < builds.Count - 1; i++)
            {
                Vector2 point1 = points[i];
                Vector2 point2 = points[i + 1];
                
                // Calculate progress for this line segment
                float segmentStart = (float)i / (builds.Count - 1);
                float segmentEnd = (float)(i + 1) / (builds.Count - 1);
                
                if (lineProgress > segmentStart)
                {
                    Vector2 drawEnd = point2;
                    
                    // If we're still animating this segment, interpolate the end point
                    if (lineProgress < segmentEnd)
                    {
                        float segmentProgress = (lineProgress - segmentStart) / (segmentEnd - segmentStart);
                        // Use linear interpolation instead of easing for smoother lines
                        drawEnd = Vector2.Lerp(point1, point2, segmentProgress);
                    }
                    
                    // Draw the line segment with anti-aliasing
                    DrawAntiAliasedLine(point1, drawEnd);
                }
            }
        }
        
        private void DrawAntiAliasedLine(Vector2 start, Vector2 end)
        {
            // Use Handles.DrawAAPolyLine for smoother lines if available, fallback to regular line
            try
            {
                // Try to use anti-aliased line drawing
                Vector3[] linePoints = { new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0) };
                Handles.DrawAAPolyLine(2.0f, linePoints);
            }
            catch
            {
                // Fallback to regular line if AA version fails
                Handles.DrawLine(start, end);
            }
        }
        
        private Vector2 GetFinalChartPoint(Rect rect, BuildSizeData build, int index, int totalCount, long minSize, long sizeRange)
        {
            // X position based on build index for even spacing
            float normalizedIndex = totalCount > 1 ? (float)index / (totalCount - 1) : 0.5f;
            float x = rect.x + rect.width * normalizedIndex;
            
            // Y position based on actual size (final position)
            float normalizedSize = sizeRange > 0 ? (float)(build.sizeInBytes - minSize) / sizeRange : 0.5f;
            float y = rect.y + rect.height - rect.height * normalizedSize;
            
            return new Vector2(x, y);
        }
        
        private Vector2 GetAnimatedChartPoint(Rect rect, BuildSizeData build, int index, int totalCount, long minSize, long sizeRange, float individualProgress)
        {
            // X position based on build index for even spacing
            float normalizedIndex = totalCount > 1 ? (float)index / (totalCount - 1) : 0.5f;
            float x = rect.x + rect.width * normalizedIndex;
            
            // Apply easing to individual point progress
            float easedPointProgress = individualProgress > 0 ? GetEasedProgress(individualProgress) : 0f;
            
            // Y position - animate from minSize (bottom) to actual build size with individual timing
            long animatedBuildSize = minSize + (long)((build.sizeInBytes - minSize) * easedPointProgress);
            float normalizedSize = sizeRange > 0 ? (float)(animatedBuildSize - minSize) / sizeRange : 0.5f;
            float y = rect.y + rect.height - rect.height * normalizedSize;
            
            return new Vector2(x, y);
        }

        private Vector2 GetChartPoint(Rect rect, BuildSizeData build, int index, int totalCount, long minSize, long sizeRange)
        {
            // X position based on build index for even spacing
            float normalizedIndex = totalCount > 1 ? (float)index / (totalCount - 1) : 0.5f;
            float x = rect.x + rect.width * normalizedIndex;
            
            // Y position based on actual size (no animation here, used for mouse hover)
            float normalizedSize = sizeRange > 0 ? (float)(build.sizeInBytes - minSize) / sizeRange : 0.5f;
            float y = rect.y + rect.height - rect.height * normalizedSize;
            
            return new Vector2(x, y);
        }

        private void HandleMouseHover(Rect rect, List<BuildSizeData> builds, float animProgress)
        {
            Event currentEvent = Event.current;
            Vector2 mousePos = currentEvent.mousePosition;
            isHovering = rect.Contains(mousePos);
            
            if (isHovering)
            {
                // Simple and fast distance calculation
                long maxSize = builds.Max(b => b.sizeInBytes);
                long minSize = builds.Min(b => b.sizeInBytes);
                long sizeRange = maxSize - minSize;
                
                // Use same range calculation as in other methods
                if (sizeRange < 1024 * 1024)
                {
                    long avgSize = (maxSize + minSize) / 2;
                    minSize = Math.Max(0, avgSize - 5 * 1024 * 1024);
                    maxSize = avgSize + 5 * 1024 * 1024;
                    sizeRange = maxSize - minSize;
                }

                float closestDistance = float.MaxValue;
                BuildSizeData closestBuild = null;
                Vector2 closestPoint = Vector2.zero;
                
                for (int i = 0; i < builds.Count; i++)
                {
                    // Use final point position for consistent hover (no animation dependency)
                    Vector2 point = GetFinalChartPoint(rect, builds[i], i, builds.Count, minSize, sizeRange);
                    float distance = Vector2.Distance(mousePos, point);
                    
                    if (distance < 25f && distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestBuild = builds[i];
                        closestPoint = point;
                    }
                }
                
                // Update hover state
                if (hoveredBuild != closestBuild)
                {
                    hoveredBuild = closestBuild;
                    hoveredPoint = closestPoint;
                    Repaint();
                }
            }
            else
            {
                // Clear hover state when mouse leaves chart area
                if (hoveredBuild != null)
                {
                    hoveredBuild = null;
                    Repaint();
                }
            }
        }

        private void ShowTooltip()
        {
            if (hoveredBuild == null) return;

            // Calculate time since build for more context
            TimeSpan timeSinceBuild = DateTime.Now - hoveredBuild.buildTime;
            string timeAgoText = GetTimeAgoText(timeSinceBuild);

            // Show actual build size in tooltip with enhanced time information
            string tooltip = $"Build: {hoveredBuild.buildName}\n" +
                           $"Size: {hoveredBuild.GetFormattedSize()}\n" +
                           $"Date: {hoveredBuild.buildTime:dd/MM/yyyy HH:mm:ss}\n" +
                           $"Time ago: {timeAgoText}";

            Vector2 tooltipSize = tooltipStyle.CalcSize(new GUIContent(tooltip));
            Vector2 tooltipPos = hoveredPoint + new Vector2(10, -tooltipSize.y/2);
            
            // Keep tooltip within window bounds
            if (tooltipPos.x + tooltipSize.x > position.width)
                tooltipPos.x = hoveredPoint.x - tooltipSize.x - 10;
            if (tooltipPos.y < 0)
                tooltipPos.y = 0;
            if (tooltipPos.y + tooltipSize.y > position.height)
                tooltipPos.y = position.height - tooltipSize.y;

            GUI.Label(new Rect(tooltipPos, tooltipSize), tooltip, tooltipStyle);
        }

        private string GetTimeAgoText(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 365)
                return $"{(int)(timeSpan.TotalDays / 365)} year(s) ago";
            else if (timeSpan.TotalDays >= 30)
                return $"{(int)(timeSpan.TotalDays / 30)} month(s) ago";
            else if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays} day(s) ago";
            else if (timeSpan.TotalHours >= 1)
                return $"{(int)timeSpan.TotalHours} hour(s) ago";
            else if (timeSpan.TotalMinutes >= 1)
                return $"{(int)timeSpan.TotalMinutes} minute(s) ago";
            else
                return "Just now";
        }

        private float GetNormalizedTimePosition(DateTime buildTime, List<BuildSizeData> allBuilds)
        {
            if (allBuilds.Count <= 1)
                return 0.5f;

            // Get the time range from the currently displayed builds (not all builds)
            var currentBuilds = buildHistory.OrderByDescending(b => b.buildTime).Take(selectedCount).OrderBy(b => b.buildTime).ToList();
            
            if (currentBuilds.Count <= 1)
                return 0.5f;

            DateTime minTime = currentBuilds.First().buildTime;
            DateTime maxTime = currentBuilds.Last().buildTime;
            
            // Handle case where all builds are at the same time
            if (minTime == maxTime)
                return 0.5f;
            
            // Calculate normalized position based on time
            double totalTimeSpan = (maxTime - minTime).TotalMilliseconds;
            double buildTimeSpan = (buildTime - minTime).TotalMilliseconds;
            
            return (float)(buildTimeSpan / totalTimeSpan);
        }

        private static string GetBuildHistoryKey()
        {
            // Generate a project-specific key based on project path
            string projectPath = Application.dataPath;
            string projectHash = projectPath.GetHashCode().ToString();
            return $"GMSoft_{PlayerSettings.productName}_{projectHash}_build_size_history";
        }

        private void MigrateFromFile()
        {
            // Try to migrate from old JSON file if it exists
            try
            {
                string jsonFilePath = System.IO.Path.Combine(Application.dataPath, "GMSoft/Editor/BuildSizeHistory.json");
                if (System.IO.File.Exists(jsonFilePath))
                {
                    string historyJson = System.IO.File.ReadAllText(jsonFilePath);
                    if (!string.IsNullOrEmpty(historyJson))
                    {
                        var existingHistory = JsonConvert.DeserializeObject<List<BuildSizeData>>(historyJson);
                        if (existingHistory != null && existingHistory.Count > 0)
                        {
                            // Save to EditorPrefs
                            string key = GetBuildHistoryKey();
                            string newJson = JsonConvert.SerializeObject(existingHistory);
                            EditorPrefs.SetString(key, newJson);
                            
                            // Update local buildHistory
                            buildHistory = existingHistory;
                            
                            Debug.Log($"Build history migrated from JSON file to EditorPrefs with key: {key}");
                            
                            // Delete the old file
                            try
                            {
                                System.IO.File.Delete(jsonFilePath);
                                Debug.Log($"Deleted legacy file: {jsonFilePath}");
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning($"Failed to delete legacy file: {ex.Message}");
                            }
                        }
                    }
                }
                
                // Also check for old EditorPrefs format
                MigrateFromOldEditorPrefs();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to migrate build history from file: {ex.Message}");
            }
        }
        
        private void MigrateFromOldEditorPrefs()
        {
            // Try to migrate from old EditorPrefs keys if they exist
            try
            {
                string productName = PlayerSettings.productName;
                if (!string.IsNullOrEmpty(productName))
                {
                    string oldPrefsKey = productName + "_build_size_history";
                    if (EditorPrefs.HasKey(oldPrefsKey))
                    {
                        string historyJson = EditorPrefs.GetString(oldPrefsKey);
                        if (!string.IsNullOrEmpty(historyJson))
                        {
                            var existingHistory = JsonConvert.DeserializeObject<List<BuildSizeData>>(historyJson);
                            if (existingHistory != null && existingHistory.Count > 0)
                            {
                                // Save to new EditorPrefs key
                                string newKey = GetBuildHistoryKey();
                                string newJson = JsonConvert.SerializeObject(existingHistory);
                                EditorPrefs.SetString(newKey, newJson);
                                
                                // Update local buildHistory
                                buildHistory = existingHistory;
                                
                                Debug.Log($"Build history migrated from old EditorPrefs key to new key: {newKey}");
                                
                                // Delete the old key
                                EditorPrefs.DeleteKey(oldPrefsKey);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to migrate build history from old EditorPrefs: {ex.Message}");
            }
        }

        private void LoadBuildHistory()
        {
            string key = GetBuildHistoryKey();
            
            if (EditorPrefs.HasKey(key))
            {
                try
                {
                    string historyJson = EditorPrefs.GetString(key);
                    if (!string.IsNullOrEmpty(historyJson))
                    {
                        buildHistory = JsonConvert.DeserializeObject<List<BuildSizeData>>(historyJson) ?? new List<BuildSizeData>();
                    }
                    else
                    {
                        buildHistory = new List<BuildSizeData>();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to load build history from EditorPrefs: {ex.Message}");
                    buildHistory = new List<BuildSizeData>();
                }
            }
            else
            {
                // Try to migrate from JSON file if exists
                MigrateFromFile();
                
                if (!EditorPrefs.HasKey(key))
                {
                    buildHistory = new List<BuildSizeData>();
                }
            }
        }

        public static void AddBuildToHistory(string buildPath, string buildName)
        {
            try
            {
                if (System.IO.Directory.Exists(buildPath))
                {
                    long totalSize = GetDirectorySize(buildPath);
                    string key = GetBuildHistoryKey();
                    
                    List<BuildSizeData> history = new List<BuildSizeData>();
                    
                    // Load existing history from EditorPrefs
                    if (EditorPrefs.HasKey(key))
                    {
                        try
                        {
                            string existingJson = EditorPrefs.GetString(key);
                            if (!string.IsNullOrEmpty(existingJson))
                            {
                                history = JsonConvert.DeserializeObject<List<BuildSizeData>>(existingJson) ?? new List<BuildSizeData>();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"Failed to load existing build history: {ex.Message}");
                            history = new List<BuildSizeData>();
                        }
                    }
                    
                    history.Add(new BuildSizeData(DateTime.Now, totalSize, buildName));
                    
                    // Keep only last 30 builds to prevent file bloat
                    if (history.Count > 30)
                    {
                        history = history.OrderByDescending(b => b.buildTime).Take(30).ToList();
                    }
                    
                    // Save to EditorPrefs
                    string newJson = JsonConvert.SerializeObject(history);
                    EditorPrefs.SetString(key, newJson);
                    
                    Debug.Log($"Build history saved to EditorPrefs with key: {key}");
                    
                    // Auto refresh any open windows
                    RefreshAllWindows();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to add build to history: {ex.Message}");
            }
        }
        
        private static string FormatBytesStatic(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024f:F0} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024f * 1024f):F1} MB";
            return $"{bytes / (1024f * 1024f * 1024f):F2} GB";
        }

        private static long GetDirectorySize(string path)
        {
            long size = 0;
            try
            {
                string[] files = System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
                    size += fileInfo.Length;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to calculate directory size: {ex.Message}");
            }
            return size;
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
    }
}
#endif
