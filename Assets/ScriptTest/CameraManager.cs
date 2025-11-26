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
    
    [Header("Camera Delay Settings")]
    public float cameraDelayTime = 3f; // Thời gian delay trước khi camera bắt đầu theo máy bay
    public float cameraBlendTime = 2f; // Thời gian blend mượt mà khi bật Follow/LookAt
    
    private float targetOrthoSize;
    private bool isFollowingAircraft = false;
    private Vector3 originalCameraPosition;
    private bool isCameraDelayActive = false; // Flag để biết đang trong thời gian delay
    private float gameStartTime = 0f; // Thời điểm bắt đầu chơi
    private bool isBlending = false; // Flag để biết đang blend
    private float blendStartTime = 0f; // Thời điểm bắt đầu blend
    private Vector3 blendStartPosition; // Vị trí bắt đầu blend
    private bool wasFollowDisabled = false; // Follow có bị tắt không
    private float blendStartOrthoSize = 0f; // OrthoSize bắt đầu blend
    private float blendStartScreenY = 0f; // ScreenY bắt đầu blend

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
        // Kiểm tra delay và bật lại Follow/LookAt sau 3s
        if (isCameraDelayActive)
        {
            float timeSinceStart = Time.time - gameStartTime;
            if (timeSinceStart >= cameraDelayTime)
            {
                isCameraDelayActive = false;
                BeginBlend();
            }

            return; // Không xử lý gì khác trong thời gian delay
        }
        
        // Xử lý blend mượt mà: Di chuyển manual từ vị trí cũ đến vị trí Follow
        if (isBlending)
        {
            float blendProgress = (Time.time - blendStartTime) / cameraBlendTime;
            
            if (blendProgress >= 1f)
            {
                // Kết thúc blend - bật Follow/LookAt
                isBlending = false;
                wasFollowDisabled = false;
                virtualCamera.Follow = aircraftTransform;
                virtualCamera.LookAt = aircraftTransform;
                
                // Đặt screenY về giá trị target cuối cùng
                var composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
                if (composer != null)
                {
                    float altitude = GManager.instance != null ? GManager.instance.currentAltitude : 0f;
                    bool isFlying = altitude > 5f;
                    bool hasFuel = GManager.instance != null && GManager.instance.isPlay;
                    float targetScreenY = (isFlying && hasFuel) ? screenYFlying : screenYGround;
                    // composer.m_ScreenY = targetScreenY;
                    // currentScreenY = targetScreenY;
                    // currentScreenY = currentScreenY;
                }
                
                Debug.Log("Camera blend hoàn tất - Follow/LookAt enabled");
            }
            else
            {
                // Đang blend - tính toán vị trí target và lerp mượt mà
                if (aircraftTransform != null)
                {
                    Vector3 targetPosition = new Vector3(
                        aircraftTransform.position.x,
                        aircraftTransform.position.y,
                        originalCameraPosition.z
                    );
                    
                    // Sử dụng SmoothStep để blend mượt mà hơn
                    float smoothProgress = Mathf.SmoothStep(0f, 1f, blendProgress);
                    virtualCamera.transform.position = Vector3.Lerp(blendStartPosition, targetPosition, smoothProgress);
                    
                    // BLEND ORTHOSIZE MƯỢT MÀ - Tính orthoSize target dựa trên altitude hiện tại
                    float currentAltitude = GManager.instance != null ? GManager.instance.currentAltitude : 0f;
                    float targetOrthoSizeNow = CalculateOrthoSizeFromAltitude(currentAltitude);
                    
                    // Lerp từ orthoSize ban đầu đến orthoSize target
                    virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(blendStartOrthoSize, targetOrthoSizeNow, smoothProgress);
                    
                    // BLEND SCREEN Y MƯỢT MÀ - Tính screenY target
                    bool isFlying = currentAltitude > 5f;
                    bool hasFuel = GManager.instance != null && GManager.instance.isPlay;
                    float targetScreenY = (isFlying && hasFuel) ? screenYFlying : screenYGround;
                    
                    // Lerp screenY mượt mà
                    var composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
                    if (composer != null)
                    {
                        float blendedScreenY = Mathf.Lerp(blendStartScreenY, targetScreenY, smoothProgress);
                        composer.m_ScreenY = blendedScreenY;
                        currentScreenY = blendedScreenY;
                    }
                    
                    // Debug mỗi 0.2s
                    if (Mathf.FloorToInt(blendProgress * 5f) != Mathf.FloorToInt((blendProgress - Time.deltaTime / cameraBlendTime) * 5f))
                    {
                        Debug.Log($"Blending: {blendProgress * 100f:F0}% - Pos: {virtualCamera.transform.position}, OrthoSize: {virtualCamera.m_Lens.OrthographicSize:F1}, ScreenY: {currentScreenY:F2}");
                    }
                }
            }
            return; // Không xử lý zoom/screenY trong khi blend
        }
        
        // Xử lý zoom tự động theo độ cao máy bay
        HandleAltitudeBasedZoom();
        
        // Xử lý thay đổi ScreenY mượt mà
        HandleScreenYTransition();
        
        // Áp dụng zoom mượt mà
        ApplySmoothZoom();
    }
    
void HandleAltitudeBasedZoom()
{
    // Chỉ zoom tự động nếu không có input chuột gần đây
    if (GManager.instance != null && Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) < 0.01f)
    {
        float altitude = GManager.instance.currentAltitude;
        float calculatedOrthoSize = CalculateOrthoSizeFromAltitude(altitude);
        targetOrthoSize = calculatedOrthoSize;
    }
}

// Helper function để tính orthoSize từ altitude
float CalculateOrthoSizeFromAltitude(float altitude)
{
    float calculatedOrthoSize;
    
    if (altitude <= 0f)
    {
        calculatedOrthoSize = 7f;
    }
    else if (altitude <= 10f)
    {
        calculatedOrthoSize = Mathf.Lerp(7f, 14f, altitude / 10f);
    }
    else
    {
        float extraAltitude = altitude - 10f;
        calculatedOrthoSize = 14f + (extraAltitude / 5f);
    }
    
    calculatedOrthoSize = Mathf.Clamp(calculatedOrthoSize, minOrthoSize, maxOrthoSize);
    return calculatedOrthoSize;
}

void HandleScreenYTransition()
{
    if (GManager.instance == null || virtualCamera == null) return;
    
    var composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
    if (composer == null) return;
    
    // Xác định target screenY dựa trên trạng thái máy bay
    float altitude = GManager.instance.currentAltitude;
    bool isFlying = altitude > 5f;
    bool hasFuel = GManager.instance.isPlay;
    
    float targetScreenY = (isFlying && hasFuel) ? screenYFlying : screenYGround;
    
    // Chuyển đổi mượt mà
    currentScreenY = Mathf.Lerp(currentScreenY, targetScreenY, screenTransitionSpeed * Time.deltaTime);
    composer.m_ScreenY = currentScreenY;
}

void HandleCameraFollow()
{
    if (virtualCamera == null || aircraftTransform == null) return;
    
    // KIỂM TRA: Nếu đang trong thời gian delay, giữ nguyên camera tại vị trí ban đầu
    if (isCameraDelayActive)
    {
        float timeSinceStart = Time.time - gameStartTime;
        if (timeSinceStart < cameraDelayTime)
        {
            // Vẫn đang trong thời gian delay - giữ nguyên vị trí ban đầu
            virtualCamera.transform.position = originalCameraPosition;
            
            // Debug mỗi 0.5 giây
            if (Mathf.FloorToInt(timeSinceStart * 2f) != Mathf.FloorToInt((timeSinceStart - Time.deltaTime) * 2f))
            {
                Debug.Log($"Camera DELAY active: {timeSinceStart:F1}s / {cameraDelayTime}s - Position locked at {originalCameraPosition}");
            }
            return;
        }
        else
        {
            // Hết thời gian delay - cho phép camera di chuyển
            isCameraDelayActive = false;
            Debug.Log($"Camera delay kết thúc sau {cameraDelayTime}s - Bắt đầu follow logic");
        }
    }
    
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
        // KHÔNG áp dụng zoom trong thời gian delay
        if (isCameraDelayActive)
        {
            return;
        }
        
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
        isCameraDelayActive = false;
        
        // Reset screenY về giá trị đất
        currentScreenY = screenYGround;
        var composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
        if (composer != null)
        {
            composer.m_ScreenY = currentScreenY;
        }
    }
    
    // THÊM: Method để bắt đầu game và kích hoạt delay
    public void StartGameWithDelay()
    {
        isCameraDelayActive = true;
        gameStartTime = Time.time;
        isFollowingAircraft = false;
        isBlending = false;

        // Đặt camera về vị trí ban đầu
        virtualCamera.transform.position = originalCameraPosition;

        // KHÓA camera: tắt Follow và LookAt
        virtualCamera.Follow = null;
        virtualCamera.LookAt = null;

        targetOrthoSize = baseOrthoSize;
        virtualCamera.m_Lens.OrthographicSize = baseOrthoSize;

        Debug.Log($"*** CAMERA DELAY START ({cameraDelayTime}s) — Follow/LookAt DISABLED ***");
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

    void BeginBlend()
    {
        isBlending = true;
        blendStartTime = Time.time;

        blendStartPosition = virtualCamera.transform.position;
        blendStartOrthoSize = virtualCamera.m_Lens.OrthographicSize;

        var composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
        if (composer != null)
            blendStartScreenY = composer.m_ScreenY;

        // TẮT FOLLOW/LOOKAT DÙ ĐANG Ở TRẠNG THÁI NÀO
        virtualCamera.Follow = null;
        virtualCamera.LookAt = null;

        wasFollowDisabled = true;
    }

}