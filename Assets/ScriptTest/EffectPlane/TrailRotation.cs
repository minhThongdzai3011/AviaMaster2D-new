using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrailRotation : MonoBehaviour
{
    [Header("Trail Time")]
    [Tooltip("Thời gian tồn tại của trail (giống TrailRenderer Time)")]
    public float trailTime = 0.15f;

    [Header("Trail Shape")]
    public float minDistanceBetweenPoints = 0.05f;
    public float startWidth = 0.4f;
    public float endWidth = 0.05f;
    public Gradient trailGradient;

    [Header("Performance")]
    public int maxPoints = 60;

    // ===== INTERNAL =====
    private LineRenderer lineRenderer;
    private Vector3 lastRecordedPosition;

    private struct TrailPoint
    {
        public Vector3 position;
        public float time;

        public TrailPoint(Vector3 pos, float t)
        {
            position = pos;
            time = t;
        }
    }

    private readonly List<TrailPoint> trailPoints = new List<TrailPoint>();

    // ====================

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Setup LineRenderer
        lineRenderer.useWorldSpace = true; // LineRenderer luôn world-space (chuẩn)
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.colorGradient = trailGradient;
        lineRenderer.numCornerVertices = 5;
        lineRenderer.numCapVertices = 5;
        lineRenderer.textureMode = LineTextureMode.Stretch;
        lineRenderer.alignment = LineAlignment.TransformZ;

        lastRecordedPosition = transform.position;
    }

    void Update()
    {
        float now = Time.time;
        Vector3 currentPos = transform.position;

        // 1️⃣ Thêm điểm mới khi đủ khoảng cách
        if (Vector3.Distance(currentPos, lastRecordedPosition) >= minDistanceBetweenPoints)
        {
            trailPoints.Add(new TrailPoint(currentPos, now));
            lastRecordedPosition = currentPos;

            // Giới hạn số điểm (an toàn hiệu năng)
            if (trailPoints.Count > maxPoints)
            {
                trailPoints.RemoveAt(0);
            }
        }

        // 2️⃣ Xóa điểm quá thời gian sống
        for (int i = trailPoints.Count - 1; i >= 0; i--)
        {
            if (now - trailPoints[i].time > trailTime)
            {
                trailPoints.RemoveAt(i);
            }
        }

        // 3️⃣ Update LineRenderer
        int count = trailPoints.Count;
        lineRenderer.positionCount = count;

        for (int i = 0; i < count; i++)
        {
            lineRenderer.SetPosition(i, trailPoints[i].position);
        }
    }

    // ================= PUBLIC =================

    public void ClearTrail()
    {
        trailPoints.Clear();
        lineRenderer.positionCount = 0;
        lastRecordedPosition = transform.position;
    }
}
