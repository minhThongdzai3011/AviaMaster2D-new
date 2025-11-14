using UnityEngine;
using Cinemachine;
using UnityEngine.Animations; // Thêm thư viện Cinemachine

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    // Gán Virtual Camera vào trường này trong Inspector
    public CinemachineVirtualCamera virtualCamera;

    public CinemachineFollowZoom followZoom;
    public LookAtConstraint lookAtConstraint;

    // Các thiết lập cho hiệu ứng zoom
    public float zoomSpeed = 2f;
    public float minOrthoSize = 3f;
    public float maxOrthoSize = 20f;
    public float smoothSpeed = 5f;

    [Header("Camera tự động theo máy bay")]
    public float baseOrthoSize = 7f; // Ortho size ban đầu khi máy bay sát đất
    public float altitudeZoomFactor = 0.3f; // Hệ số zoom theo độ cao
    public float followThreshold = 15f; // Ngưỡng orthoSize để bắt đầu follow máy bay
    public float cameraFollowSpeed = 2f; // Tốc độ di chuyển camera theo máy bay
    
    [Header("Screen Position Settings")]
    public float screenYGround = 0.7f; // ScreenY khi máy bay ở đất
    public float screenYFlying = 0.5f; // ScreenY khi máy bay đang bay
    public float screenTransitionSpeed = 1f; // Tốc độ chuyển đổi screenY
    private float currentScreenY; // ScreenY hiện tại
    
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
        instance = this;

        // Thiết lập kích thước ban đầu
        targetOrthoSize = baseOrthoSize;
        virtualCamera.m_Lens.OrthographicSize = baseOrthoSize;
        
        // Thiết lập screenY ban đầu
        currentScreenY = screenYGround;
        var composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
        if (composer != null)
        {
            composer.m_ScreenX = 0.5f;
            composer.m_ScreenY = currentScreenY;
        }
        
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
        
        // Xử lý thay đổi ScreenY mượt mà
        HandleScreenYTransition();
        
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
            calculatedOrthoSize = 7f;
        }
        else if (altitude <= 10f)  // THAY ĐỔI: <= 10f thay vì >= 10f
        {
            // Nội suy tuyến tính giữa 0-10m: orthoSize từ 9-14
            calculatedOrthoSize = Mathf.Lerp(7f, 14f, altitude / 10f);
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

void HandleScreenYTransition()
{
    if (GManager.instance == null || virtualCamera == null) return;
    
    var composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
    if (composer == null) return;
    
    // Xác định target screenY dựa trên trạng thái máy bay
    float targetScreenY;
    float altitude = GManager.instance.currentAltitude;
    bool isFlying = altitude > 5f; // Coi như đang bay nếu độ cao > 5m
    bool hasFuel = GManager.instance.isPlay; // Đang có fuel và đang chơi
    
    if (isFlying && hasFuel)
    {
        // Đang bay và có fuel → screenY = 0.5
        targetScreenY = screenYFlying;
    }
    else
    {
        // Máy bay ở đất hoặc hết fuel → screenY = 0.7
        targetScreenY = screenYGround;
    }
    
    // Chuyển đổi mượt mà
    currentScreenY = Mathf.Lerp(currentScreenY, targetScreenY, screenTransitionSpeed * Time.deltaTime);
    
    // Áp dụng vào composer
    composer.m_ScreenY = currentScreenY;
    
    // Debug log để theo dõi (chỉ khi có thay đổi đáng kể)
    if (Mathf.Abs(currentScreenY - targetScreenY) > 0.01f)
    {
        Debug.Log($"ScreenY transition: {currentScreenY:F2} → {targetScreenY:F2} (Flying: {isFlying}, Fuel: {hasFuel})");
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
        
        // Reset screenY về giá trị đất
        currentScreenY = screenYGround;
        var composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
        if (composer != null)
        {
            composer.m_ScreenY = currentScreenY;
        }
    }
    
    // Method để force follow máy bay
    public void ForceFollowAircraft(bool follow)
    {
        isFollowingAircraft = follow;
        Debug.Log($"CameraManager: Force follow set to {follow}");
    }
    
    // THÊM: Method để force follow ngay lập tức với aircraft hiện tại
    public void ForceFollowCurrentAircraft()
    {
        if (aircraftTransform != null)
        {
            isFollowingAircraft = true;
            Vector3 aircraftPos = aircraftTransform.position;
            Vector3 targetCameraPos = new Vector3(
                aircraftPos.x, 
                aircraftPos.y, 
                originalCameraPosition.z
            );
            virtualCamera.transform.position = targetCameraPos;
            Debug.Log($"CameraManager: Force follow current aircraft {aircraftTransform.name} at {targetCameraPos}");
        }
        else
        {
            Debug.LogError("CameraManager: Cannot force follow - aircraftTransform is null");
        }
    }
    
    // THÊM: Method để cập nhật target aircraft khi đổi máy bay
    public void UpdateAircraftTarget(Transform newAircraftTransform)
    {
        aircraftTransform = newAircraftTransform;
        Debug.Log($"CameraManager: Aircraft target updated to {newAircraftTransform.name} at position {newAircraftTransform.position}");
        
        // Nếu đang follow thì cập nhật ngay vị trí camera
        if (isFollowingAircraft)
        {
            Vector3 aircraftPos = aircraftTransform.position;
            Vector3 targetCameraPos = new Vector3(
                aircraftPos.x, 
                aircraftPos.y, 
                originalCameraPosition.z
            );
            virtualCamera.transform.position = targetCameraPos;
            Debug.Log($"CameraManager: Camera position updated immediately to {targetCameraPos}");
        }
    }
    
    // THÊM: Method để cập nhật Virtual Camera follow target (nếu sử dụng Cinemachine Follow)
    public void UpdateCinemachineFollow(Transform newTarget)
    {
        if (virtualCamera != null && newTarget != null)
        {
            virtualCamera.Follow = newTarget;
            virtualCamera.LookAt = newTarget;
            Debug.Log($"CameraManager: Cinemachine Follow/LookAt updated to {newTarget.name}");
            
            // FORCE camera follow ngay lập tức nếu đang ở độ cao thích hợp
            if (targetOrthoSize > 15f)
            {
                isFollowingAircraft = true;
                Debug.Log("CameraManager: Force following aircraft due to high ortho size");
            }
        }
        else
        {
            Debug.LogError($"CameraManager: UpdateCinemachineFollow failed - VirtualCamera: {virtualCamera != null}, NewTarget: {newTarget != null}");
        }
    }
}