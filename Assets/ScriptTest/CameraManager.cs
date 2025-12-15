using UnityEngine;
using Cinemachine;
using UnityEngine.Animations; // Thêm thư viện Cinemachine
using System.Collections;

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
    public float maxOrthoSize = 30f;
    public float smoothSpeed = 5f;

    [Header("Camera tự động theo máy bay")]
    public float baseOrthoSize = 7f; // Ortho size ban đầu khi máy bay sát đất
    public float altitudeZoomFactor = 0.3f; // Hệ số zoom theo độ cao
    public float followThreshold = 15f; // Ngưỡng orthoSize để bắt đầu follow máy bay
    public float cameraFollowSpeed = 2f; // Tốc độ di chuyển camera theo máy bay
    
    [Header("Screen Position Settings")]
    public float screenYGround = 0.5f; // ScreenY khi máy bay ở đất
    public float screenYFlying = 0.5f; // ScreenY khi máy bay đang bay
    public float screenTransitionSpeed = 1f; // Tốc độ chuyển đổi screenY
    private float currentScreenY; // ScreenY hiện tại
    private float currentScreenX; // ScreenX hiện tại
    public float screenXDelay = 0.4f; // ScreenX ban đầu (lệch trái/phải)
    public float screenYDelay = 0.7f; // ScreenY ban đầu (cao hơn một chút)
    public float screenBlendSpeed = 2f; // Tốc độ blend về 0.5, 0.5
    
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
    private float disableUpdateUntil = 0f;
    
    // Chỉ chuyển ScreenX/Y về 0.5 sau khi người chơi ấn Chơi
    private bool hasPressedPlay = false;
    // Lưu ScreenX/Y tại thời điểm bắt đầu delay để blend ổn định mỗi lần
    private float delayStartScreenX = 0f;
    private float delayStartScreenY = 0f;
    // Flag để khóa screen position khi đã đạt target
    private bool isScreenPositionLocked = false;
    private const float SCREEN_LOCK_THRESHOLD = 0.001f;
    
    // Smoothing cho velocity detection
    private float smoothedVelocityY = 0f;
    public float velocitySmoothTime = 0.1f; // Thời gian smooth velocity
    private float velocityYDerivative = 0f;


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
        
        // Thiết lập screenY và screenX ban đầu (DELAY position)
        currentScreenY = screenYDelay; // Bắt đầu cao hơn
        currentScreenX = screenXDelay; // Bắt đầu lệch trái
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer != null)
        {
            transposer.m_ScreenX = currentScreenX;
            transposer.m_ScreenY = currentScreenY;
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
            
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (transposer != null)
            {
                // Tính progress của delay (0 → 1)
                float delayProgress = Mathf.Clamp01(timeSinceStart / cameraDelayTime);
                float t = Mathf.SmoothStep(0f, 1f, delayProgress);

                // Tính target screenY dựa trên altitude và velocity để tránh giật
                float currentAltitude = GManager.instance != null ? GManager.instance.currentAltitude : 0f;
                
                // Lấy velocity để xác định hướng bay
                float velocityY = 0f;
                if (aircraftTransform != null && GManager.instance.airplaneRigidbody2D != null)
                {
                    velocityY = GManager.instance.airplaneRigidbody2D.velocity.y;
                }
                bool isAscending = velocityY > 0.1f;
                
                float targetScreenY;
                if (currentAltitude < 15f)
                {
                    // Dưới 15m: bay lên = 0.4
                    targetScreenY = 0.5f;
                }
                else if (currentAltitude < 20f)
                {
                    float altT = (currentAltitude - 15f) / 5f;
                    targetScreenY = Mathf.Lerp(0.4f, 0.3f, altT);
                }
                else
                {
                    targetScreenY = 0.5f; // Altitude cao thì về 0.5
                }

                float blendedX = Mathf.Lerp(delayStartScreenX, 0.5f, t);
                float blendedY = Mathf.Lerp(delayStartScreenY, targetScreenY, t);

                transposer.m_ScreenX = blendedX;
                transposer.m_ScreenY = blendedY;
                currentScreenX = blendedX;
                currentScreenY = blendedY;

                // Debug mỗi 0.5s
                if (Mathf.FloorToInt(timeSinceStart * 2f) != Mathf.FloorToInt((timeSinceStart - Time.deltaTime) * 2f))
                {
                    Debug.Log($"🎥 Delay Blending: {timeSinceStart:F1}s/{cameraDelayTime}s - ScreenX: {currentScreenX:F2} → 0.5, ScreenY: {currentScreenY:F2} → {targetScreenY:F2} (Alt: {currentAltitude:F1}m)");
                }
            }

            // Khóa vị trí camera trong suốt thời gian delay để tránh bị dịch chuyển 5px
            virtualCamera.transform.position = originalCameraPosition;
            
            if (timeSinceStart >= cameraDelayTime)
            {
                isCameraDelayActive = false;
                
                // ✅ Đảm bảo ScreenX/Y đã được blend mượt mà đến target theo altitude
                if (transposer != null)
                {
                    // Tính target screenY cuối cùng dựa trên altitude
                    float currentAltitude = GManager.instance != null ? GManager.instance.currentAltitude : 0f;
                    float finalScreenY;
                    if (currentAltitude < 15f)
                    {
                        finalScreenY = 0.5f;
                    }
                    else if (currentAltitude < 20f)
                    {
                        float altT = (currentAltitude - 15f) / 5f;
                        finalScreenY = Mathf.Lerp(0.5f, 0.3f, altT);
                    }
                    else
                    {
                        finalScreenY = 0.5f;
                    }
                    
                    transposer.m_ScreenX = 0.5f;
                    transposer.m_ScreenY = finalScreenY;
                    currentScreenX = 0.5f;
                    currentScreenY = finalScreenY;
                    // KHÔNG khóa để có thể chuyển đổi theo altitude
                    isScreenPositionLocked = false;
                    Debug.Log($"✅ ScreenX/Y set to 0.5, {finalScreenY:F2} (Alt: {currentAltitude:F1}m) - Starting blend to Follow/LookAt");
                }
                
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
                // TÍNH VỊ TRÍ CINEMACHINE MUỐN CAMERA ĐỨNG
                var targetPos = new Vector3(
                    aircraftTransform.position.x,
                    aircraftTransform.position.y,
                    originalCameraPosition.z
                );

                // ĐẶT CAMERA VỀ ĐÚNG VỊ TRÍ CINEMACHINE MONG MUỐN
                virtualCamera.transform.position = targetPos;
                disableUpdateUntil = Time.time + 0.2f; 
                // NGĂN CINEMACHINE GIẬT DAMPING TRONG FRAME ĐẦU
                virtualCamera.PreviousStateIsValid = false;
                SetCinemachineActive(true);
                virtualCamera.m_Lens.OrthographicSize = CalculateOrthoSizeFromAltitude(GManager.instance.currentAltitude);

                // BẬT FOLLOW/LOOKAT
                virtualCamera.Follow = aircraftTransform;
                virtualCamera.LookAt = aircraftTransform;

                // KHÓA ZOOM & SCREENY 1 frame để tránh cập nhật song song gây giật
                StartCoroutine(FreezeCameraOneFrame());



                isBlending = false;
                wasFollowDisabled = false;

                
                // Đặt screenY theo altitude hiện tại - đã được blend mượt mà trong quá trình blend
                var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                if (transposer != null)
                {
                    // KHÔNG khóa - để HandleScreenYTransition có thể điều chỉnh theo altitude
                    float currentAltitude = GManager.instance != null ? GManager.instance.currentAltitude : 0f;
                    
                    // Tính target screenY dựa trên altitude
                    float finalScreenY;
                    if (currentAltitude < 15f)
                    {
                        finalScreenY = 0.5f;
                    }
                    else if (currentAltitude < 20f)
                    {
                        float t = (currentAltitude - 15f) / 5f;
                        finalScreenY = Mathf.Lerp(0.5f, 0.3f, t);
                    }
                    else if (currentAltitude <= 45f)
                    {
                        finalScreenY = 0.3f;
                    }
                    else
                    {
                        finalScreenY = 0.5f;
                    }
                    
                    transposer.m_ScreenY = finalScreenY;
                    currentScreenY = finalScreenY;
                    transposer.m_ScreenX = 0.5f;
                    currentScreenX = 0.5f;
                    isScreenPositionLocked = false;
                    
                    Debug.Log($"✅ Blend complete - ScreenY: {finalScreenY:F1} for altitude: {currentAltitude:F1}m");
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
                    
                    // BLEND SCREEN X/Y MƯỢT MÀ - Từ 0.5 sang giá trị target theo altitude
                    var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                    if (transposer != null)
                    {
                        // Tính target screenY dựa trên altitude hiện tại
                        float targetScreenYForAltitude;
                        if (currentAltitude < 15f)
                        {
                            targetScreenYForAltitude = 0.4f;
                        }
                        else if (currentAltitude < 20f)
                        {
                            float t = (currentAltitude - 15f) / 5f;
                            targetScreenYForAltitude = Mathf.Lerp(0.4f, 0.3f, t);
                        }
                        else if (currentAltitude <= 45f)
                        {
                            targetScreenYForAltitude = 0.3f;
                        }
                        else
                        {
                            targetScreenYForAltitude = 0.5f;
                        }
                        
                        // Blend mượt mà từ screenY ban đầu (0.5) sang target
                        transposer.m_ScreenX = 0.5f;
                        transposer.m_ScreenY = Mathf.Lerp(0.5f, targetScreenYForAltitude, smoothProgress);
                        currentScreenX = 0.5f;
                        currentScreenY = transposer.m_ScreenY;
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
        
        HandleCameraFollow();

        // Xử lý zoom tự động theo độ cao máy bay
        HandleAltitudeBasedZoom();
        
        // Xử lý thay đổi ScreenY mượt mà - CHỈ khi chưa khóa
        if (!isScreenPositionLocked)
        {
            HandleScreenYTransition();
        }
        
        // Áp dụng zoom mượt mà
        ApplySmoothZoom();
    }
    
    void HandleAltitudeBasedZoom()
    {
        if (freezeFrame) return;

        // Chỉ zoom tự động nếu không có input chuột gần đây
        if (GManager.instance != null && Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) < 0.01f)
        {
            float altitude = GManager.instance.currentAltitude;
            float calculatedOrthoSize = CalculateOrthoSizeFromAltitude(altitude);
            targetOrthoSize = calculatedOrthoSize;
        }
    }

    float CalculateOrthoSizeFromAltitude(float altitude)
    {
        float calculatedOrthoSize;
        
        if (altitude <= 0f)
        {
            calculatedOrthoSize = 7f;
        }
        else if (altitude <= 15f)
        {
            calculatedOrthoSize = Mathf.Lerp(7f, 20f, altitude / 15f);
        }
        else
        {
            float extraAltitude = altitude - 20f;
            calculatedOrthoSize = 20f + Mathf.Min(extraAltitude / 3f, 15f); 
        }
        
        calculatedOrthoSize = Mathf.Clamp(calculatedOrthoSize, minOrthoSize, maxOrthoSize);
        return calculatedOrthoSize;
    }

    void HandleScreenYTransition()
    {
        if (freezeFrame) return;

        if (GManager.instance == null || virtualCamera == null) return;
        // Không chuyển ScreenY trước khi người chơi ấn Chơi hoặc trong thời gian delay/blend
        if (!hasPressedPlay || isCameraDelayActive || isBlending) return;
        
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer == null) return;
        
        // Xác định target screenX/Y dựa trên độ cao và hướng bay của máy bay
        float altitude = GManager.instance.currentAltitude;
        bool hasFuel = GManager.instance.isPlay;
        
        // Lấy velocity của máy bay và smooth để tránh giật
        float targetVelocityY = 0f;
        if (aircraftTransform != null && GManager.instance.airplaneRigidbody2D != null)
        {
            targetVelocityY = GManager.instance.airplaneRigidbody2D.velocity.y;
        }
        
        // Smooth velocity để tránh nhảy target liên tục
        smoothedVelocityY = Mathf.SmoothDamp(smoothedVelocityY, targetVelocityY, ref velocityYDerivative, velocitySmoothTime);
        
        bool isAscending = smoothedVelocityY > 0.5f; // Bay lên nếu smoothed velocity Y > 0.5
        bool isDescending = smoothedVelocityY < -0.5f; // Bay xuống nếu smoothed velocity Y < -0.5
        
        float targetScreenX = 0.5f;
        float targetScreenY;
        
        if (!hasFuel)
        {
            // Không bay hoặc hết nhiên liệu: giữ vị trí mặc định
            targetScreenY = screenYGround;
        }
        else if (altitude < 15f)
        {
            // Dưới 15m: Phân biệt bay lên/xuống
            if (isAscending)
            {
                // Bay lên: screenY = 0.4 để người chơi cảm nhận được máy bay bay lên
                targetScreenY = 0.4f;
            }
            else if (isDescending)
            {
                // Bay xuống: screenY = 0.8 để nhìn thấy mặt đất tốt hơn
                targetScreenY = 0.8f;
            }
            else
            {
                // Hover hoặc bay ngang: giữ nguyên giá trị hiện tại
                targetScreenY = currentScreenY;
            }
        }
        else if (altitude < 20f)
        {
            // 15-20m: Vùng chuyển tiếp mượt mà
            float t = (altitude - 15f) / 5f;
            if (isDescending)
            {
                // Bay xuống: chuyển từ 0.8 → 0.3
                targetScreenY = Mathf.Lerp(0.8f, 0.3f, t);
            }
            else
            {
                // Bay lên hoặc hover: chuyển từ 0.4 → 0.3
                targetScreenY = Mathf.Lerp(0.4f, 0.3f, t);
            }
        }
        else if (altitude <= 45f)
        {
            // 20-45m: Giữ ở 0.3
            targetScreenY = 0.3f;
        }
        else if (altitude < 50f)
        {
            // 45-50m: Vùng chuyển tiếp mượt mà từ 0.3 → 0.5
            float t = (altitude - 45f) / 5f;
            targetScreenY = Mathf.Lerp(0.3f, 0.5f, t);
        }
        else
        {
            // Trên 50m: Giữ ở 0.5
            targetScreenY = 0.5f;
        }
        
        // Chuyển đổi mượt mà với tốc độ động - tăng tốc khi cần di chuyển nhiều
        float distanceToTarget = Mathf.Abs(targetScreenY - currentScreenY);
        // Tăng tốc độ transition khi khoảng cách lớn (bay xuống nhanh)
        float dynamicSpeed = screenTransitionSpeed;
        if (distanceToTarget > 0.2f)
        {
            // Khi cần di chuyển > 0.2, tăng tốc độ lên 3-4 lần
            dynamicSpeed = screenTransitionSpeed * Mathf.Lerp(3f, 4f, (distanceToTarget - 0.2f) / 0.4f);
        }
        
        currentScreenY = Mathf.Lerp(currentScreenY, targetScreenY, dynamicSpeed * Time.deltaTime);
        currentScreenX = Mathf.Lerp(currentScreenX, targetScreenX, screenTransitionSpeed * Time.deltaTime);
        
        transposer.m_ScreenY = currentScreenY;
        transposer.m_ScreenX = currentScreenX;
    }

void HandleCameraFollow()
{
    if (virtualCamera == null || aircraftTransform == null) return;
    // Khi Cinemachine đang Follow, không tự di chuyển transform thủ công để tránh giật
    if (virtualCamera.Follow != null) return;
    
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
        isBlending = false;
        hasPressedPlay = false;
        isScreenPositionLocked = false;

        // Trạng thái pre-game: ScreenX/Y về 0.3 / 0.86
        currentScreenX = screenXDelay;
        currentScreenY = screenYDelay;
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer != null)
        {
            transposer.m_ScreenX = currentScreenX;
            transposer.m_ScreenY = currentScreenY;
        }

        // Ngắt Follow/LookAt và tắt Cinemachine để không tự di chuyển khi ở menu/shop
        virtualCamera.Follow = null;
        virtualCamera.LookAt = null;
        SetCinemachineActive(false);
    }
    
    // THÊM: Method để bắt đầu game và kích hoạt delay
    public void StartGameWithDelay()
    {
        hasPressedPlay = true; // Đánh dấu đã ấn Chơi
        isCameraDelayActive = true;
        gameStartTime = Time.time;
        isFollowingAircraft = false;
        isBlending = false;
        isScreenPositionLocked = false;

        // Lấy vị trí hiện tại làm mốc, tránh nhảy 5px khi bắt đầu chơi
        originalCameraPosition = virtualCamera.transform.position;

        // KHÓA camera: tắt Follow và LookAt
        virtualCamera.Follow = null;
        virtualCamera.LookAt = null;

        targetOrthoSize = baseOrthoSize;
        virtualCamera.m_Lens.OrthographicSize = baseOrthoSize;

        // Giữ ScreenX/Y tại vị trí pre-game ngay khi bắt đầu delay
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer != null)
        {
            // Đặt lại state khởi điểm của blend về 0.3/0.86 để lần chơi sau không bị nhảy
            currentScreenX = screenXDelay;
            currentScreenY = screenYDelay;
            transposer.m_ScreenX = currentScreenX;
            transposer.m_ScreenY = currentScreenY;
            // Lưu điểm bắt đầu của blend để tiến trình mượt, lặp lại ổn định giữa các lần chơi
            delayStartScreenX = currentScreenX;
            delayStartScreenY = currentScreenY;
        }

        // Reset state của Cinemachine để tránh giật khung đầu
        virtualCamera.PreviousStateIsValid = false;
        // Tắt các component của Cinemachine trong thời gian delay để không tự dịch chuyển transform
        SetCinemachineActive(false);

        Debug.Log($"*** CAMERA DELAY START ({cameraDelayTime}s) — Follow/LookAt DISABLED ***");
    }

    
    // Method để force follow máy bay
    public void ForceFollowAircraft(bool follow)
    {
        isFollowingAircraft = follow;
        Debug.Log($"CameraManager: Force follow set to {follow}");
    }
    
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

        var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer != null)
            blendStartScreenY = transposer.m_ScreenY;

        SetCinemachineActive(false);
        // TẮT FOLLOW/LOOKAT DÙ ĐANG Ở TRẠNG THÁI NÀO
        virtualCamera.Follow = null;
        virtualCamera.LookAt = null;

        wasFollowDisabled = true;
    }

    private bool freezeFrame = false;

    IEnumerator FreezeCameraOneFrame()
    {
        freezeFrame = true;
        yield return null; // khóa 1 frame
        freezeFrame = false;
    }

    void SetCinemachineActive(bool enabled)
    {
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer)
        {
            if (!enabled)
            {
                // Tắt hoàn toàn ảnh hưởng của Cinemachine (KHÔNG dùng .enabled)
                transposer.m_XDamping = 0;
                transposer.m_YDamping = 0;
                transposer.m_ZDamping = 0;
                transposer.m_DeadZoneWidth = 0f;
                transposer.m_DeadZoneHeight = 0f;
                transposer.m_SoftZoneWidth = 0f;
                transposer.m_SoftZoneHeight = 0f;
            }
            else
            {
                // Bật lại damping mặc định
                transposer.m_XDamping = 1;
                transposer.m_YDamping = 1;
                transposer.m_ZDamping = 1;
            }
        }

        var followZoom = virtualCamera.GetComponent<CinemachineFollowZoom>();
        if (followZoom) followZoom.enabled = enabled;

        var lookAtCon = virtualCamera.GetComponent<LookAtConstraint>();
        if (lookAtCon) lookAtCon.enabled = enabled;
    }




}