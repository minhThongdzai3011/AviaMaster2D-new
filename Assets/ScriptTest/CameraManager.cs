using UnityEngine;
using Cinemachine; // Thêm thư viện Cinemachine

public class CameraManager : MonoBehaviour
{
    // Gán Virtual Camera vào trường này trong Inspector
    public CinemachineVirtualCamera virtualCamera;

    // Các thiết lập cho hiệu ứng zoom
    public float zoomSpeed = 2f;
    public float minOrthoSize = 3f;
    public float maxOrthoSize = 20f;
    public float smoothSpeed = 5f;

    [Header("Camera tự động theo máy bay")]
    public float baseOrthoSize = 9f; // Ortho size ban đầu khi máy bay sát đất
    public float altitudeZoomFactor = 0.3f; // Hệ số zoom theo độ cao
    public float followThreshold = 15f; // Ngưỡng orthoSize để bắt đầu follow máy bay
    public float cameraFollowSpeed = 2f; // Tốc độ di chuyển camera theo máy bay
    
    [Header("Ground và Aircraft references")]
    public Transform groundTransform; // Reference đến Ground
    public Transform aircraftTransform; // Reference đến máy bay
    
    private float targetOrthoSize;
    private bool isFollowingAircraft = false;
    private Vector3 originalCameraPosition;

    void Start()
    {
        // Kiểm tra xem đã gán Virtual Camera chưa
        if (virtualCamera == null)
        {
            Debug.LogError("Virtual Camera chưa được gán!");
            return;
        }

        // Thiết lập kích thước ban đầu
        targetOrthoSize = baseOrthoSize;
        virtualCamera.m_Lens.OrthographicSize = baseOrthoSize;
        
        // Lưu vị trí camera ban đầu
        originalCameraPosition = virtualCamera.transform.position;
        
        // Tự động tìm máy bay nếu chưa gán
        if (aircraftTransform == null && GManager.instance != null && GManager.instance.airplaneRigidbody2D != null)
        {
            aircraftTransform = GManager.instance.airplaneRigidbody2D.transform;
        }
    }

    void Update()
    {
        // Xử lý zoom thủ công bằng chuột
        HandleManualZoom();
        
        // Xử lý zoom tự động theo độ cao máy bay
        HandleAltitudeBasedZoom();
        
        // Xử lý camera follow máy bay
        HandleCameraFollow();
        
        // Áp dụng zoom mượt mà
        ApplySmoothZoom();
    }
    
    void HandleManualZoom()
    {
        // Lấy giá trị cuộn chuột
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0.0f)
        {
            // Thay đổi kích thước mục tiêu
            targetOrthoSize -= scroll * zoomSpeed;

            // Giới hạn kích thước camera trong một khoảng nhất định
            targetOrthoSize = Mathf.Clamp(targetOrthoSize, minOrthoSize, maxOrthoSize);
        }
    }
void HandleAltitudeBasedZoom()
{
    // Chỉ zoom tự động nếu không có input chuột gần đây
    if (GManager.instance != null && Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) < 0.01f)
    {
        float altitude = GManager.instance.currentAltitude;
        
        // Tính toán orthoSize dựa trên độ cao - LOGIC SỬA LẠI
        float calculatedOrthoSize;
        
        if (altitude <= 0f)
        {
            // Máy bay sát đất → orthoSize = 9
            calculatedOrthoSize = 9f;
        }
        else if (altitude <= 10f)  // THAY ĐỔI: <= 10f thay vì >= 10f
        {
            // Nội suy tuyến tính giữa 0-10m: orthoSize từ 9-14
            calculatedOrthoSize = Mathf.Lerp(9f, 14f, altitude / 10f);
        }
        else
        {
            // Máy bay cao hơn 10m, tiếp tục tăng orthoSize
            // Từ altitude 10m trở lên, mỗi 5m tăng thêm 1 orthoSize
            float extraAltitude = altitude - 10f;
            calculatedOrthoSize = 14f + (extraAltitude / 5f);
        }
        
        // Giới hạn trong khoảng min-max
        calculatedOrthoSize = Mathf.Clamp(calculatedOrthoSize, minOrthoSize, maxOrthoSize);
        
        // Cập nhật target
        targetOrthoSize = calculatedOrthoSize;
        
    }
}

void HandleCameraFollow()
{
    if (virtualCamera == null || aircraftTransform == null) return;
    
    // LOGIC MỚI: Chỉ follow khi orthoSize > 15
    bool shouldFollow = targetOrthoSize > 15f;
    
    if (shouldFollow && !isFollowingAircraft)
    {
        // Bắt đầu follow máy bay
        isFollowingAircraft = true;
        Debug.Log($"Bắt đầu follow máy bay - OrthoSize: {targetOrthoSize:F1}");
    }
    else if (!shouldFollow && isFollowingAircraft)
    {
        // Dừng follow, quay về vị trí để nhìn thấy Ground
        isFollowingAircraft = false;
        Debug.Log($"Dừng follow, nhìn Ground - OrthoSize: {targetOrthoSize:F1}");
    }
    
    // Di chuyển camera
    if (isFollowingAircraft)
    {
        // Follow máy bay - không cần nhìn Ground nữa
        Vector3 aircraftPos = aircraftTransform.position;
        Vector3 targetCameraPos = new Vector3(
            aircraftPos.x, 
            aircraftPos.y, 
            originalCameraPosition.z
        );
        
        virtualCamera.transform.position = Vector3.Lerp(
            virtualCamera.transform.position,
            targetCameraPos,
            cameraFollowSpeed * Time.deltaTime
        );
    }
    else
    {
        // Không follow - LUÔN PHẢI NHÌN THẤY GROUND
        Vector3 targetPos = CalculateGroundVisiblePosition();
        
        virtualCamera.transform.position = Vector3.Lerp(
            virtualCamera.transform.position,
            targetPos,
            cameraFollowSpeed * Time.deltaTime
        );
    }
}

Vector3 CalculateGroundVisiblePosition()
{
    if (aircraftTransform == null || groundTransform == null)
    {
        return originalCameraPosition;
    }
    
    Vector3 aircraftPos = aircraftTransform.position;
    Vector3 groundPos = groundTransform.position;
    
    // Tính vị trí camera để LUÔN nhìn thấy Ground
    // Camera Y phải đảm bảo Ground luôn trong tầm nhìn
    float groundY = groundPos.y;
    float aircraftY = aircraftPos.y;
    
    // Tính camera Y để nhìn thấy cả ground và máy bay
    float cameraY = groundY + (targetOrthoSize * 0.4f); // 40% orthoSize phía trên ground
    
    // Đảm bảo máy bay vẫn trong tầm nhìn
    if (aircraftY > cameraY + targetOrthoSize * 0.5f)
    {
        cameraY = aircraftY - targetOrthoSize * 0.3f;
    }
    
    // Camera X có thể di chuyển nhẹ theo máy bay (20%)
    float cameraX = Mathf.Lerp(originalCameraPosition.x, aircraftPos.x, 0.2f);
    
    return new Vector3(
        cameraX,
        cameraY,
        originalCameraPosition.z
    );
}
    void ApplySmoothZoom()
    {
        // Di chuyển kích thước hiện tại đến kích thước mục tiêu một cách mượt mà
        virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
            virtualCamera.m_Lens.OrthographicSize, 
            targetOrthoSize, 
            smoothSpeed * Time.deltaTime
        );
    }
    
    // Method để reset camera về vị trí ban đầu
    public void ResetCamera()
    {
        targetOrthoSize = baseOrthoSize;
        isFollowingAircraft = false;
        virtualCamera.transform.position = originalCameraPosition;
    }
    
    // Method để force follow máy bay
    public void ForceFollowAircraft(bool follow)
    {
        isFollowingAircraft = follow;
    }
}