using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using DG.Tweening;

public class GManager : MonoBehaviour
{
    public static GManager instance;
    public Rigidbody2D airplaneRigidbody2D;

    [Header("Cài đặt máy bay")]
    public float launchForce = 20f; // Lực đẩy ban đầu
    [Tooltip("Góc bay lên ban đầu khi nhấn Launch (độ). Có thể thay đổi trong Inspector.")]
    public float climbAngle = 30f;  // Góc bay lên ban đầu
    public float gravityScale = 1f; // Thang trọng lực chung
    public float boostedAltitude;

    [Header("Giới hạn độ cao")]
    public float maxAltitude = 1200f; // Độ cao tối đa
    public float maxPowerPercent = 300f; // Power tối đa (300% = 3x lực ban đầu)
    public float altitudeDragMultiplier = 2f; // Hệ số lực kéo xuống theo độ cao
    public float altitudeDragStart = 600f; // Độ cao bắt đầu có lực kéo mạnh
    public float velocityDampingFactor = 0.98f; // Hệ số giảm vận tốc theo độ cao
    public bool isBoosted = false;

    [Header("Button UI")]
    public Button playButton;
    public bool isPlay = false;

    [Header("Slider UI")]
    public Slider sliderFuel;
    public Slider sliderBooster;

    [Header("Trạng thái chơi")]
    public Vector2 startPosition;
    public float distanceTraveled;
    public float currentAltitude;
    private float rotationZ;
    public bool isBoosterActive = false;
    public float boostDecreaseRate = 100f;
    private bool isBoostDecreasing = false;
    public bool stopDisplayDistance = false;
    private Coroutine boostCoroutine;
    public int money = 0; 
    public int diamonds = 0; 
    public float totalMoney = 0;
    public int totalDiamond = 0;
    public float targetValue = 0f;
    public float duration = 1f;
    public bool isPlaying = true;

    [Header("Nâng cấp")]
    public float durationFuel = 5;
    public float totalBoost = 100;
    public int levelPower;
    public int levelFuel;
    public int levelBoost;
    
    [Header("Fuel Control Sync")]
    public float currentControlTimer = 0f; // Timer được chia sẻ giữa control loop và slider
    public float currentControlDuration = 0f; // Duration được chia sẻ

    [Header("Tính toán nâng cấp")]
    public float baseDurationFuel = 2f; // Thời gian cơ bản không thay đổi
    public float baseLaunchForce = 20f; // Lực cơ bản không thay đổi
    public float baseTotalBoost = 100f; // Boost cơ bản không thay đổi

    [Header("RotationZ settings")]

    public float rotationSmooth = 8f; 
    public float minMoveThreshold = 0.5f; 

    public GameObject arrowAngleZ;
    public GameObject rotationXObject;
    public ParticleSystem coinEffect;

    [Header("RotationX settings")]
    public float amplitude = 15.0f; // Góc nghiêng tối đa
    public float frequency = 1.0f;  // Tốc độ lắc lư
    private Quaternion originalRotation;
    
    [Header("RotationX Airplane Oscillation")]
    public bool isAirplaneRotationXActive = true; // Bật/tắt xoay X cho máy bay
    public float airplaneRotationXAmplitude = 10f; // Biên độ xoay (-10 đến +10)
    public float airplaneRotationXSpeed = 1f; // Tốc độ xoay
    public AnimationCurve airplaneRotationXCurve = AnimationCurve.EaseInOut(0f, -1f, 1f, 1f); // Curve để làm mượt
    private float currentAirplaneRotationX = 0f;
    private float targetAirplaneRotationX = 0f;
    
    [Header("Ground Collision Detection")]
    public bool isPlaneOnGround = false; // Flag để theo dõi máy bay chạm đất
    public bool isGroundCollisionDetected = false; // Flag collision từ Plane.cs
    public bool isCheckErrorAngleZ = false; //

    public bool isNoPlayingGame = true;

    [Header("UI Texts")]
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI altitudeText;
    public TextMeshProUGUI velocityText;
    public TextMeshProUGUI rotationZText;
    public TextMeshProUGUI levelPowerText;
    public TextMeshProUGUI levelFuelText;
    public TextMeshProUGUI levelBoostText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI totalMoneyText;
    public TextMeshProUGUI totalDiamondText;
    public TextMeshProUGUI powerMoneyText;
    public TextMeshProUGUI fuelMoneyText;
    public TextMeshProUGUI boostMoneyText;
    public TextMeshProUGUI newMapText;
    public TextMeshProUGUI rotationTrailText;

    [Header("Thành tích")]
    public Image sliderAchievement;

    [Header("Người chơi")]
    public bool isControllable = false;
    public float controlForce = 8f; // Lực điều khiển A/D
    public float maxVerticalSpeed = 15f; // Giới hạn tốc độ dọc

    [Header("Image UI")]
    public GameObject homeImage;
    public GameObject playImage;
    public Image leaderBoardImage;
    public Image buttonUpImage;
    public Image buttonDownImage;
    public Image buttonBoosterPlaneImage;

    [Header("Wheel Settings")]
    public float rotationForce = 100f;        // tốc độ xoay ban đầus
    public float angularFriction = 5f;        // hệ số ma sát góc
    public float currentRotationSpeed = 0f;  // tốc độ xoay hiện tại
    public float targetAltitude = 0f;

    [Header("Camera Management")]
    public CameraManager cameraManager; // Reference tới CameraManager

    void Awake()
    {

        instance = this;
        currentRotationSpeed = rotationForce;
        airplaneRigidbody2D.rotation = -10f;

    }

    void Start()
    {
        Debug.Log("Airplane GManager Start called " + airplaneRigidbody2D.name);
        
        // totalBoostMaxPower = totalBoost;
        // Debug.Log("totalBoost " + totalBoost + " totalBoostMaxPower " + totalBoostMaxPower);
        buttonDownImage.color = Color.gray;
        buttonUpImage.color = Color.gray;
        buttonBoosterPlaneImage.color = Color.gray;

        sliderAchievement.fillAmount = 0f;
        startZ = airplaneRigidbody2D.transform.rotation.eulerAngles.z;

        // THÊM: Load dữ liệu nâng cấp TRƯỚC khi tính toán
        LoadUpgradeData();
        // Tính toán lại các giá trị nâng cấp dựa trên rate đã load
        CalculateUpgradeValues();
        LoadMoneyUpgrade();
        
        // Cập nhật UI
        Debug.Log("money , totalMoney at Start: " + money + ", " + totalMoney);
        if (airplaneRigidbody2D != null)
        {
            startPosition = airplaneRigidbody2D.transform.position;
            airplaneRigidbody2D.velocity = Vector2.zero;
            airplaneRigidbody2D.angularVelocity = 0f;
            airplaneRigidbody2D.gravityScale = gravityScale;

            airplaneRigidbody2D.freezeRotation = true;

            airplaneRigidbody2D.drag = 0.2f; // Drag tự nhiên
            airplaneRigidbody2D.angularDrag = 0.1f; // Angular drag
    
        }
        Physics2D.gravity = new Vector2(0f, -9.81f * gravityScale);
        totalMoney = PlayerPrefs.GetFloat("TotalMoney", 0);
        totalMoneyText.text = totalMoney.ToString("F0");
        if (LuckyWheel.instance != null)
        {
            LuckyWheel.instance.checkValueLuckyWheel();
        }
        else
        {
            // Delay check nếu LuckyWheel chưa sẵn sàng
            StartCoroutine(DelayedLuckyWheelCheck());
        }
        SaveTotalMoney();
        SaveTotalDiamond();
        totalBoostMaxPower = totalBoost;
        
        // Kiểm tra LuckyWheel sau khi tất cả đã được khởi tạo
        StartCoroutine(DelayedLuckyWheelCheck());

       


    }
    
    // Coroutine để kiểm tra LuckyWheel sau một chút
    IEnumerator DelayedLuckyWheelCheck()
    {
        yield return new WaitForSeconds(0.1f);
        
        if (LuckyWheel.instance != null)
        {
            LuckyWheel.instance.checkValueLuckyWheel();
            Debug.Log("Delayed LuckyWheel check completed in Start()");
        }
        else
        {
            Debug.LogWarning("LuckyWheel.instance still null after delay in Start()");
        }
    }

    
    public void LaunchAirplane()
    {
        if (PositionX.instance != null)
        {
            PositionX.instance.checkPlay();
            if(PositionX.instance.isMaxPower)
            {
                Settings.instance.imageFuelPlay.color = Color.yellow;
                Settings.instance.imageFill.color = Color.yellow;
            }
        }
        // if (BirdGuide.instance != null && BirdGuide.instance.isShowGuide)
        // {
        //     BirdGuide.instance.pannelGuide.gameObject.SetActive(true);
        //     BirdGuide.instance.StartCoroutine(BirdGuide.instance.DelaytoGuide());
        //     newMapText.text = "How to Play";
        // }
        if (isPlaying)
        {
            if (airplaneRigidbody2D == null)
            {
                Debug.LogError("Rigidbody2D chưa được gán!");
                return;
            }
            
            // Start flight timer for mission achievement 4
            if (MissionAchievements.instance != null)
            {
                MissionAchievements.instance.StartFlightTimer();
            }
            else
            {
                Debug.LogWarning("[LAUNCH] MissionAchievements.instance is null - cannot start flight timer");
            }
            
            AudioManager.instance.StopSoundBackground();
            Settings.instance.imageHighScore.enabled = false;
            if (PositionX.instance.isMaxPower){
                Settings.instance.imagex2Fuel.gameObject.SetActive(true);
                // Settings.instance.imagex2Power.gameObject.SetActive(true);
            }
            
            // homeImage.gameObject.SetActive(false);
            playImage.gameObject.SetActive(true);
            Plane.instance.isStopSmokeEffect = false;
            // Âm thanh play
            
            StartCoroutine(LaunchSequence());
            isPlaying = false;
            isPlay = true;
            isHomeDown = true;
            isHomeUp = true;
            ChangePlane currentChangePlane = airplaneRigidbody2D.GetComponent<ChangePlane>();
           
            if (currentChangePlane != null)
            {
                Debug.Log("Setting isRotationChangePlane to false in LaunchAirplane");
                currentChangePlane.isRotationChangePlane = false;
            }
            else
            {
                Debug.LogWarning($"ChangePlane component not found on {airplaneRigidbody2D.name}");
            }
            // THÊM: Reset trạng thái chạm đất khi bắt đầu game mới
            ResetGroundCollision();
            
            // THÊM: Reset flag falling sequence
            isFallingInSequence = false;
            
            // THÊM: Kích hoạt camera delay 3s
            if (CameraManager.instance != null)
            {
                CameraManager.instance.StartGameWithDelay();
            }
            
            homeGameLeft();
            homeGameRight();

            isRotationOscillating = false;
            GuideManager.instance.guideText.gameObject.SetActive(false);
        }
    }

    public bool controlPlane = false;
    public bool isPlayBolide = false;
    IEnumerator LaunchSequence()
    {
        //xoay canh quat
        
        if(airplaneRigidbody2D.name == "Forest" || airplaneRigidbody2D.name == "Avocado" || airplaneRigidbody2D.name == "BeeGee" || airplaneRigidbody2D.name == "Pancake" || airplaneRigidbody2D.name == "Scruffy" || airplaneRigidbody2D.name == "Jungle") 
        {
            Debug.Log("Start Rotation Front" + airplaneRigidbody2D.name +" isRotary " + Shop.instance.isRotaryFrontZDone);

            // THAY ĐỔI: Tìm TẤT CẢ RotaryFront trên máy bay để hỗ trợ nhiều cánh quạt
            if (Shop.instance.isRotaryFrontZDone)
            {
                RotaryFrontZ[] propellers = airplaneRigidbody2D.GetComponentsInChildren<RotaryFrontZ>();
                Debug.Log($"[DEBUG] Found {propellers.Length} RotaryFrontZ propellers on {airplaneRigidbody2D.name}");
                
                foreach (var propeller in propellers)
                {
                    if (propeller != null && propeller.gameObject.activeInHierarchy)
                    {
                        propeller.StartRotation();
                        Debug.Log($"Started rotation on RotaryFrontZ: {propeller.gameObject.name}");
                    }
                }
                
                if (propellers.Length == 0)
                {
                    Debug.LogWarning($"No RotaryFrontZ component found on {airplaneRigidbody2D.name}");
                }
            }
            else
            {
                RotaryFront[] propellers = airplaneRigidbody2D.GetComponentsInChildren<RotaryFront>();
                Debug.Log($"[DEBUG] Found {propellers.Length} RotaryFront propellers on {airplaneRigidbody2D.name}");
                
                foreach (var propeller in propellers)
                {
                    if (propeller != null && propeller.gameObject.activeInHierarchy)
                    {
                        propeller.StartRotation();
                        Debug.Log($"Started rotation on RotaryFront: {propeller.gameObject.name}");
                    }
                }
            }

        }
        AudioManager.instance.PlayPlayerSound(AudioManager.instance.takeOffSoundClip);
        

        if(SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane5) SuperPlaneManager.instance.imageSkillSuperPlane5.gameObject.SetActive(true);
        if(SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane2)  SuperPlaneManager.instance.imageSkillSuperPlane2.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        arrowAngleZ.SetActive(false);
        rotationXObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        
        // ==================== BƯỚC 0: DI CHUYỂN NGANG 5PX VỚI VẬN TỐC TĂNG DẦN ====================
        Debug.Log("=== BẮT ĐẦU BƯỚC 0: Di chuyển ngang 5px với vận tốc tăng dần ===");
        
        float step0Distance = 5f; // Khoảng cách di chuyển trong Bước 0
        float step0MaxSpeed = 8f; // Tốc độ tối đa trong Bước 0
        float step0Acceleration = 15f; // Gia tốc trong Bước 0 (m/s²)
        
        float step0TargetX = airplaneRigidbody2D.position.x + step0Distance;
        float step0StartX = airplaneRigidbody2D.position.x;
        
        // Giữ máy bay ở mặt đất (rotation = 0)
        airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        airplaneRigidbody2D.gravityScale = 0f;
        
        // Bắt đầu với vận tốc = 0, tăng dần
        float currentSpeed0 = 0f;
        
        Debug.Log($"Bước 0 - Start: {step0StartX:F2}, Target: {step0TargetX:F2}, Distance: {step0Distance}px");
        
        while (airplaneRigidbody2D.position.x < step0TargetX)
        {
            // Tăng tốc độ dần dần
            currentSpeed0 += step0Acceleration * Time.deltaTime;
            currentSpeed0 = Mathf.Min(currentSpeed0, step0MaxSpeed);
            
            // Áp dụng vận tốc (chỉ di chuyển ngang, không bay lên)
            airplaneRigidbody2D.velocity = new Vector2(currentSpeed0, 0f);
            
            // Giữ rotation = 0
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            
            yield return null;
        }
        
        Debug.Log($"Hoàn thành Bước 0 - Final Speed: {currentSpeed0:F2} m/s, Distance: {airplaneRigidbody2D.position.x - step0StartX:F2}px");
        Debug.Log("=== KẾT THÚC BƯỚC 0 ===\n");
        
        // ==================== BƯỚC 1: DI CHUYỂN NGANG VỪA BAY LÊN ====================
        Debug.Log("=== BẮT ĐẦU BƯỚC 1: Di chuyển ngang vừa bay lên ===");
        
        // THÊM: Set flag bay ngang
        isHorizontalFlying = true;
        isPlayBolide = true;
        
        float r = 10f;
        float horizontalSpeed = 20f;
        float targetX = airplaneRigidbody2D.position.x + r;

        // Bắt đầu từ vận tốc hiện tại của Bước 0
        float initialSpeed = airplaneRigidbody2D.velocity.x; // Kế thừa vận tốc từ Bước 0 (~8 m/s)
        Debug.Log($"Bước 1 - Kế thừa vận tốc từ Bước 0: {initialSpeed:F2} m/s");
        

        airplaneRigidbody2D.gravityScale = 0f;
        
        // Kế thừa vận tốc từ Bước 0 và tiếp tục tăng tốc
        float currentSpeed = initialSpeed; // Bắt đầu từ vận tốc cuối Bước 0
        float accelerationRate = 15f; // Tốc độ tăng tốc (m/s²)

        // Giữ góc 0 khi bay ngang
        airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Debug.Log($"Bắt đầu Bước 1 - Target: {targetX:F2}, Current: {airplaneRigidbody2D.position.x:F2}, Initial Speed: {currentSpeed:F2}");

        float riseHeight = 1.0f;
        float maxTiltAngle = 20f;
        float horizontalDistance = r;

        float startX = airplaneRigidbody2D.position.x;
        float startY = airplaneRigidbody2D.position.y;

        float tiltVel = 0f;   // dùng cho SmoothDamp
        float heightVel = 0f;

        while (airplaneRigidbody2D.position.x < targetX)
        {
            float traveled = airplaneRigidbody2D.position.x - startX;
            float progress = Mathf.Clamp01(traveled / horizontalDistance);

            // --- 1. Xoay đầu bằng SmoothDamp ---
            float targetTilt = Mathf.Lerp(0f, maxTiltAngle, progress);
            float currentTilt = airplaneRigidbody2D.transform.eulerAngles.z;

            float smoothTilt = Mathf.SmoothDampAngle(currentTilt, targetTilt, ref tiltVel, 0.2f);
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, smoothTilt);

            // --- 2. Nâng nhẹ độ cao bằng SmoothDamp ---
            float targetHeight = Mathf.Lerp(0f, riseHeight, progress);
            float smoothHeight = Mathf.SmoothDamp(
                airplaneRigidbody2D.position.y - startY,
                targetHeight,
                ref heightVel,
                0.25f
            );

            airplaneRigidbody2D.position = new Vector2(
                airplaneRigidbody2D.position.x,
                startY + smoothHeight
            );

            // --- 3. THAY ĐỔI: Tăng tốc độ từ từ thay vì fix cứng ---
            currentSpeed += accelerationRate * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, horizontalSpeed); // Giới hạn tốc độ tối đa
            
            // Áp dụng velocity mới với tốc độ tăng dần
            airplaneRigidbody2D.velocity = new Vector2(currentSpeed, 0f);
            
            // Debug tốc độ mỗi 10 frame
            if (Time.frameCount % 10 == 0)
            {
                Debug.Log($"Bước 1 - Current Speed: {currentSpeed:F2} m/s, Target: {horizontalSpeed} m/s, Progress: {progress:F2}");
            }

            yield return null;
        }

        
        Debug.Log($"Hoàn thành Bước 1 - Final Speed: {currentSpeed:F2} m/s, Distance: {airplaneRigidbody2D.position.x - (targetX - r):F1}px");
        Debug.Log("=== KẾT THÚC BƯỚC 1 ===\n");
        
        // SỬA: Set false SAU KHI hoàn thành bay ngang
        isHorizontalFlying = false;

        // ==================== BƯỚC 2: BAY LÊN VỚI TĂNG TỐC MƯỢT MÀ ====================
        Debug.Log("=== BẮT ĐẦU BƯỚC 2: Bay lên với tăng tốc ===");
        AudioManager.instance.PlayPlayerSound(AudioManager.instance.gameplaySoundClip);
        float targetClimbSpeed = launchForce; // Tốc độ mục tiêu (60)
        float currentHorizontalSpeed = airplaneRigidbody2D.velocity.x; // Tốc độ hiện tại (~10)
        float climbAcceleration = 30f; // Gia tốc bay lên (m/s²)
        
        // Không dùng AddForce nữa - thay bằng tăng tốc từ từ
        // airplaneRigidbody2D.AddForce(launchDirection * climbForce, ForceMode2D.Impulse);

        boostedAltitude = launchForce / 2.5f;

        // Bước 3: Chờ đến khi đạt độ cao và xoay dần dần
        targetAltitude = startPosition.y + boostedAltitude;
        
        Debug.Log($"Chuẩn bị bay lên - Target Altitude: {targetAltitude:F1}");
        if(PositionX.instance != null && PositionX.instance.isMaxPower)
        {
            targetAltitude += boostedAltitude * 0.5f; // Tăng thêm 50% nếu max power
            if (SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane1)
            {
                targetAltitude += boostedAltitude * 0.2f; // Tăng thêm 20% nếu Super Plane active
                Debug.Log("Max Power and Super Plane 1 Active in Launch Sequence" );
            }
            if (SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane2)
            {
                targetAltitude += boostedAltitude * 0.05f; // Tăng thêm 5% nếu Super Plane 2 active
                Debug.Log("Max Power and Super Plane 2 Active in Launch Sequence" );
            }
            if (SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane3)
            {
                targetAltitude += boostedAltitude * 0.05f; // Tăng thêm 5% nếu Super Plane 3 active
                Debug.Log("Max Power and Super Plane 3 Active in Launch Sequence" );
            }
            if (SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane4)
            {
                targetAltitude += boostedAltitude * 0.05f; // Tăng thêm 5% nếu Super Plane 4 active
                Debug.Log("Max Power and Super Plane 4 Active in Launch Sequence" );
            }
            if (SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane5)
            {
                targetAltitude += boostedAltitude * 0.05f; // Tăng thêm 5% nếu Super Plane 5 active
                Debug.Log("Max Power and Super Plane 5 Active in Launch Sequence" );
            }
        }
        else
        {
            if (SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane1)
            {
                targetAltitude += boostedAltitude * 0.2f; // Tăng thêm 20% nếu Super Plane active
                Debug.Log("Max Power and Super Plane 1 Active in Launch Sequence" );
            }
            if (SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane2)
            {
                targetAltitude += boostedAltitude * 0.05f; // Tăng thêm 5% nếu Super Plane 2 active
                Debug.Log("Max Power and Super Plane 2 Active in Launch Sequence" );
            }
            if (SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane3)
            {
                targetAltitude += boostedAltitude * 0.05f; // Tăng thêm 5% nếu Super Plane 3 active
                Debug.Log("Max Power and Super Plane 3 Active in Launch Sequence" );
            }
            if (SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane4)
            {
                targetAltitude += boostedAltitude * 0.05f; // Tăng thêm 5% nếu Super Plane 4 active
                Debug.Log("Max Power and Super Plane 4 Active in Launch Sequence" );
            }
            if (SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane5)
            {
                targetAltitude += boostedAltitude * 0.05f; // Tăng thêm 5% nếu Super Plane 5 active
                Debug.Log("Max Power and Super Plane 5 Active in Launch Sequence" );
            }
        }
        if (SuperPlaneManager.instance != null)
        {
            SuperPlaneManager.instance.BuffBoostandFuelSuperPlane1();
            SuperPlaneManager.instance.BuffBoostandFuelSuperPlane2();
            SuperPlaneManager.instance.BuffBoostandFuelSuperPlane3();
            SuperPlaneManager.instance.BuffBoostandFuelSuperPlane4();
            SuperPlaneManager.instance.BuffBoostandFuelSuperPlane5();
        }
        float startAltitude = airplaneRigidbody2D.position.y;
        
        // ✅ Lấy góc HIỆN TẠI từ Bước 1 (thay vì giả định 0°)
        float currentZ = airplaneRigidbody2D.transform.eulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f; // Chuyển từ 0-360 về -180 đến 180
        float startRotation = currentZ; // Góc hiện tại (~20° từ Bước 1)
        float targetRotation = climbAngle; // Mục tiêu là climbAngle (30°)

        Debug.Log($"Bắt đầu bay lên - Start: {startAltitude:F1}, Target: {targetAltitude:F1}, Rotation: {startRotation:F1}° → {targetRotation}°");
        Debug.Log($"Tăng tốc từ {currentHorizontalSpeed:F1} m/s → {targetClimbSpeed:F1} m/s");

        while (airplaneRigidbody2D.position.y < targetAltitude)
        {
            // Tính toán tiến độ bay lên (0.0 đến 1.0)
            float currentAltitude = airplaneRigidbody2D.position.y;
            float altitudeProgress = (currentAltitude - startAltitude) / (targetAltitude - startAltitude);
            altitudeProgress = Mathf.Clamp01(altitudeProgress);

            // --- THAY ĐỔI: Tăng tốc mượt mà từ 10 → 60 ---
            Vector2 currentVel = airplaneRigidbody2D.velocity;
            
            // Tăng tốc ngang dần dần
            if (currentVel.x < targetClimbSpeed)
            {
                currentVel.x += climbAcceleration * Time.deltaTime;
                currentVel.x = Mathf.Min(currentVel.x, targetClimbSpeed); // Giới hạn tốc độ tối đa
            }
            
            // Thêm velocity.y để máy bay bay lên (dựa trên góc climbAngle)
            float targetVerticalSpeed = currentVel.x * Mathf.Tan(climbAngle * Mathf.Deg2Rad);
            currentVel.y = Mathf.Lerp(currentVel.y, targetVerticalSpeed, Time.deltaTime * 2f);
            
            // Áp dụng velocity mới
            airplaneRigidbody2D.velocity = currentVel;

            // Xoay dần dần theo tiến độ bay lên
            float easedProgress = Mathf.SmoothStep(0f, 2f, altitudeProgress);
            float currentRotation = Mathf.Lerp(startRotation, targetRotation, easedProgress);
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);

            // Debug tốc độ mỗi 20 frame
            if (Time.frameCount % 20 == 0)
            {
                Debug.Log($"Climb Phase - Speed: {currentVel.x:F1} m/s, Altitude: {currentAltitude:F1}, Progress: {altitudeProgress:F2}");
            }

            yield return null;
        }

        // Đảm bảo rotation đạt đúng climbAngle cuối cùng
        airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, climbAngle);
        Debug.Log($"Hoàn thành bay lên - Final rotation: {climbAngle}°");

        // Bước 3.5: THÊM - Xoay dần từ climbAngle về 0 một cách mượt mà
        float levelOffDuration = 1.0f; // Thời gian xoay về 0 (có thể điều chỉnh)
        float levelOffTimer = 0f;
        float startLevelOffRotation = climbAngle;
        float targetLevelOffRotation = 0f;

        Debug.Log($"Bắt đầu xoay dần từ {startLevelOffRotation}° về {targetLevelOffRotation}° trong {levelOffDuration}s");

        while (levelOffTimer < levelOffDuration)
        {
            levelOffTimer += Time.deltaTime;
            
            // Tính toán tiến độ xoay dần (0.0 đến 1.0)
            float levelOffProgress = levelOffTimer / levelOffDuration;
            levelOffProgress = Mathf.Clamp01(levelOffProgress);

            // Sử dụng SmoothStep để có chuyển động mượt mà hơn
            float easedProgress = Mathf.SmoothStep(0f, 1f, levelOffProgress);
            float currentLevelOffRotation = Mathf.Lerp(startLevelOffRotation, targetLevelOffRotation, easedProgress);
            
            // Áp dụng rotation
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, currentLevelOffRotation);

            // Duy trì vận tốc ngang, giảm dần vận tốc dọc để ổn định
            Vector2 currentVelocity = airplaneRigidbody2D.velocity;
            currentVelocity.y = Mathf.Lerp(currentVelocity.y, 0f, Time.deltaTime * 2f);
            airplaneRigidbody2D.velocity = currentVelocity;

            yield return null;
        }

        // Đảm bảo rotation = 0 chính xác
        airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        
        // THÊM: Reset currentAirplaneRotationX để tránh frame đầu bị gắn giá trị ngẫu nhiên
        currentAirplaneRotationX = 0f;

        
        Debug.Log("Hoàn thành xoay về 0° - Bắt đầu giai đoạn điều khiển" + GuideManager.instance.isShowGuide + " IsShowGuide " + GuideManager.instance.isShowGuide);
        if (GuideManager.instance != null && !GuideManager.instance.isShowGuide)
        {
            
            Debug.Log("Show Guide called from GManager");
            GuideManager.instance.ShowGuide();
        }
        else
        {
            upAircraftImage.gameObject.SetActive(true);
            GuideManager.instance.canvasGroupButton.DOFade(1f, 1f).SetUpdate(true);
        }

        // Bước 4: Giữ độ cao và cho phép điều khiển

        isCheckErrorAngleZ = true;  
        isBoosted = true;
        // RandomBonusStart.instance.SpawnRandomBonusAtLaunch();
        if(Shop.instance != null && Shop.instance.isCheckedPlaneIndex == 14)
        {
            SuperPlaneManager.instance.skillEffectSuperPlane2.SetActive(true);
        }
        if (Plane.instance != null)
        {
            if(TrailRendererLeft.instance != null && TrailRendererRight.instance != null && isBoost )
            {
                // TrailRendererLeft.instance.TrailEffect();
                // TrailRendererRight.instance.TrailEffect();
                Debug.Log("Trail Effects Activated" );
            }
            if (TrailRendererLeft.instance != null && TrailRendererLeft.instance.gameObject.activeInHierarchy && isBoost)
            {
                // TrailRendererLeft.instance.TrailEffect();
                Debug.Log("Left Trail Effect Activated" );
            }

            if (TrailRendererRight.instance != null && TrailRendererRight.instance.gameObject.activeInHierarchy && isBoost)
            {
                // TrailRendererRight.instance.TrailEffect();
                Debug.Log("Right Trail Effect Activated" );
            }

        }
        // Sửa lại: Trả lại drag tự nhiên của Rigidbody2D
        airplaneRigidbody2D.drag = 0f; // Drag ban đầu
        airplaneRigidbody2D.angularDrag = 0.05f; // Angular drag ban đầu
            
        airplaneRigidbody2D.gravityScale = 0.2f;
        airplaneRigidbody2D.velocity = new Vector2(airplaneRigidbody2D.velocity.x, 0f);

        isControllable = true;
        RocketSpawner.instance.StartSpawning();
        BonusSpawner.instance.StartSpawning();
        BonusHigherSpawn.instance.StartSpawning();
        DiamondHigherSpawn.instance.StartSpawning();
        // CatDiamondSpawner.instance.StartSpawning();
        PositionX.instance.timePerfect = 0.5f * durationFuel;
        Debug.Log($"Máy bay bắt đầu điều khiển với durationFuel = {durationFuel}s (rateFuel = {PositionX.instance.timePerfect})");
        if (PositionX.instance.isMaxPower)
        {
            int temp = (int)(durationFuel * 0.2f);
            if (temp > 5) temp = 5;
            durationFuel += temp;
            Debug.Log($"Max Power active! durationFuel increased by {temp}s to {durationFuel}s");
        }
        // Khởi tạo biến chia sẻ
        currentControlTimer = 0f;
        currentControlDuration = durationFuel;
        
        StartCoroutine(DecreaseSliderFuel());
        buttonDownImage.color = Color.white;
        buttonUpImage.color = Color.white;
        // Trong giai đoạn này, rotation được điều khiển bởi HandleAircraftControl()
        
        while (currentControlTimer < currentControlDuration)
        {
            buttonDownImage.color = Color.white;
            buttonUpImage.color = Color.white;
            currentControlTimer += Time.deltaTime;
            
            // ✅ SỬA: Xử lý thêm fuel AN TOÀN - giữ tỷ lệ progress
            if (Plane.instance != null && Plane.instance.isAddFuel)
            {
                float addedFuel = Plane.instance.addedFuelAmount;
                float oldDuration = currentControlDuration;
                
                // Cập nhật duration mới từ durationFuel (đã được Plane.cs cập nhật)
                currentControlDuration = this.durationFuel;
                
                // ✅ QUAN TRỌNG: Giữ tỷ lệ tiến độ, KHÔNG giảm timer trực tiếp
                // Công thức: newTimer = (oldTimer / oldDuration) * newDuration
                float progressRatio = currentControlTimer / oldDuration;
                currentControlTimer = progressRatio * currentControlDuration;
                
                // ✅ ĐẢM BẢO timer không bị âm hoặc vượt quá duration
                currentControlTimer = Mathf.Clamp(currentControlTimer, 0f, currentControlDuration * 0.99f);
                
                Debug.Log($"[CONTROL] Fuel added! OldDuration: {oldDuration:F1}s → NewDuration: {currentControlDuration:F1}s");
                Debug.Log($"[CONTROL] Timer: {currentControlTimer:F1}s (Progress: {progressRatio:P0})");
                
                // RESET FLAG NGAY LẬP TỨC
                Plane.instance.isAddFuel = false;
                Plane.instance.addedFuelAmount = 0f;
            }
            
            if (Mathf.FloorToInt(currentControlTimer) != Mathf.FloorToInt(currentControlTimer - Time.deltaTime))
            {
                Debug.Log($"Thời gian điều khiển: {Mathf.FloorToInt(currentControlTimer)}s / {currentControlDuration}s");
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                PlainUp();
                buttonDownImage.color = Color.white;
                buttonUpImage.color = Color.gray;

            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                PlainDown();
                buttonDownImage.color = Color.gray;
                buttonUpImage.color = Color.white;
            }

            // XỬ LÝ ĐIỀU KHIỂN A/D TRONG GIAI ĐOẠN durationFuel
            else
            {
                if (!isUseClicker)
                {
                    buttonDownImage.color = Color.white;
                    buttonUpImage.color = Color.white;
                }

                // Giảm dần velocity.y về 0 để ổn định độ cao
                Vector2 vel = airplaneRigidbody2D.velocity;
                vel.y = Mathf.Lerp(vel.y, 0f, Time.deltaTime * 2f);
                airplaneRigidbody2D.velocity = vel;

                // KHÔNG cần xử lý rotation.z ở đây nữa - HandleAircraftControl() sẽ lo
                // ApplyAutoRotationFromVelocity() sẽ được gọi trong HandleAircraftControl()
            }

            yield return null;
        }

        // Bước 5: Kết thúc fuel và tính toán vật lý rơi chân thực
        if (Plane.instance != null)
        {
            Plane.instance.trailEffect.enabled = false; 
        }
        if (Plane.instance != null && !Plane.instance.isGrounded){
            AudioManager.instance.StopPlayerSound();
            AudioManager.instance.PlayFallingSound();
        }
        
        isControllable = false; // Tắt controllable bình thường
        isFallingInSequence = true; // THÊM: Báo hiệu đang trong giai đoạn rơi
        airplaneRigidbody2D.gravityScale = gravityScale;
        Physics2D.gravity = new Vector2(0f, -9.81f * gravityScale);

        if(!Plane.instance.isGrounded){
            Plane.instance.smokeEffect.Play();
        }
        // Plane.instance.trailRenderer.enabled = false;
        Debug.Log("Bắt đầu giai đoạn rơi - Tính toán vật lý chân thực được kích hoạt");

        Vector2 initialVelocity = airplaneRigidbody2D.velocity;
        Debug.Log($"Initial Velocity trước khi tính toán vật lý thực tế: {initialVelocity}");
        if (initialVelocity.x < 10f && !Plane.instance.isGrounded)
        {
            initialVelocity.x = 10f;
            airplaneRigidbody2D.velocity = initialVelocity;
            Debug.Log($"Tăng velocity.x lên 10: {initialVelocity}");
        }

        // Các thông số vật lý máy bay
        float aircraftMass = airplaneRigidbody2D.mass; // Khối lượng máy bay
        float wingArea = 2.5f; // Diện tích cánh (m²)
        float airDensity = 1.225f; // Mật độ không khí (kg/m³)
        
        // THÊM: Debug velocity sau khi áp dụng lực khí động
        int debugCounter = 0;
        
        // Trong giai đoạn rơi với tính toán vật lý thực tế
        while (airplaneRigidbody2D.velocity.x > 0.1f)
        {
            Vector2 velocity = airplaneRigidbody2D.velocity;
            float speed = velocity.magnitude;
            
            // Debug velocity mỗi 60 frame (1 giây)
            if (debugCounter % 60 == 0)
            {
                Debug.Log($"Frame {debugCounter}: Velocity thực tế = {velocity}, Speed = {speed:F2}");
            }
            debugCounter++;
            
            // Tính toán dynamic pressure
            float dynamicPressure = 0.5f * airDensity * speed * speed;
            
            // Lấy góc máy bay hiện tại
            float currentZ0 = airplaneRigidbody2D.transform.eulerAngles.z;
            if (currentZ0 > 180f) currentZ0 -= 360f;
            
            // Tính angle of attack (góc tấn công) - góc giữa máy bay và hướng bay
            float angleOfAttack = currentZ0 - Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            angleOfAttack = Mathf.DeltaAngle(0f, angleOfAttack); // Normalize về [-180, 180]
            
            // Tính hệ số nâng và lực nâng
            float liftCoeff = Mathf.Sin(angleOfAttack * Mathf.Deg2Rad) * 0.8f; // Hệ số nâng đơn giản
            float liftForce = liftCoeff * dynamicPressure * wingArea;
            
            // Tính hệ số cản và lực cản (GIẢM để velocity không bị kéo xuống quá nhanh)
            float dragCoeff = 0.02f + 0.1f * Mathf.Abs(liftCoeff * liftCoeff); // Cản cơ bản + cản cảm ứng (giảm từ 0.05 + 0.3)
            float dragForce = dragCoeff * dynamicPressure * wingArea;
            
            // Tính hướng lực nâng và cản (vuông góc và song song với velocity)
            Vector2 velocityNorm = velocity.normalized;
            Vector2 liftDirection = new Vector2(-velocityNorm.y, velocityNorm.x); // Vuông góc với velocity
            Vector2 dragDirection = -velocityNorm; // Ngược lại velocity
            
            // Áp dụng lực nâng và cản
            Vector2 liftForceVec = liftDirection * liftForce;
            Vector2 dragForceVec = dragDirection * dragForce;
            
            // THÊM: Debug lực khí động mỗi giây
            if (debugCounter % 60 == 0)
            {
                Debug.Log($"Aerodynamic Forces - Lift: {liftForceVec}, Drag: {dragForceVec}, DragCoeff: {dragCoeff:F3}");
            }
            
            airplaneRigidbody2D.AddForce(liftForceVec, ForceMode2D.Force);
            airplaneRigidbody2D.AddForce(dragForceVec, ForceMode2D.Force);
            
            // THÊM: Duy trì velocity.x tối thiểu 15 m/s để không bị lực cản kéo xuống quá thấp
            Vector2 currentVel = airplaneRigidbody2D.velocity;
            float minVelocityX;
            if (PositionX.instance != null && PositionX.instance.isMaxPower)
            {
                minVelocityX = launchForce / 2.67f;
                Debug.Log($"Max Power active! minVelocityX set to {minVelocityX} m/s");
            }
            else{
                minVelocityX = launchForce / 1.34f;
            }
            if (minVelocityX < 20f && !Plane.instance.isGrounded) minVelocityX = 20f; // Đảm bảo tối thiểu 20 m/s
            if (minVelocityX > 40f && !Plane.instance.isGrounded) minVelocityX = 35f; // Giới hạn tối đa 40 m/s
            if (currentVel.x < minVelocityX && !Plane.instance.isGrounded)
            {
                currentVel.x = minVelocityX;
                airplaneRigidbody2D.velocity = currentVel;
                if (debugCounter % 60 == 0)
                {
                    Debug.Log($"Duy trì velocity.x tối thiểu 20 m/s: {currentVel}");
                }
            }
            
            // Kiểm tra input A/D để điều khiển góc rơi
            bool hasPlayerInput = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
            
            if (hasPlayerInput)
            {
                float targetAngle = currentZ0;
                float controlSpeed = 80f; // Tốc độ điều khiển (độ/giây)
                
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    // A: Xoay lên (ít nghiêng xuống) - hướng về -10°
                    targetAngle = Mathf.MoveTowards(currentZ0, -10f, controlSpeed * Time.deltaTime);
                    buttonDownImage.color = Color.white;
                    buttonUpImage.color = Color.gray;
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    // D: Xoay xuống (nghiêng xuống nhiều) - hướng về -50°
                    targetAngle = Mathf.MoveTowards(currentZ0, -50f, controlSpeed * Time.deltaTime);
                    buttonDownImage.color = Color.gray;
                    buttonUpImage.color = Color.white;
                }
                
                // Giới hạn góc trong khoảng cho phép
                targetAngle = Mathf.Clamp(targetAngle, -50f, -10f);
                
                // Áp dụng góc với tính đến lực khí động
                float smoothAngle = Mathf.LerpAngle(currentZ0, targetAngle, Time.deltaTime * 6f);
                
                Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
                airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, smoothAngle);
                
                // Tính toán tỷ lệ lượn (glide ratio)
                float glideRatio = Mathf.Abs(velocity.x / velocity.y);
                
                // Tính khoảng cách đáp đất ước tính
                float currentAltitude = airplaneRigidbody2D.transform.position.y;
                float estimatedDistance = currentAltitude * glideRatio;
                
            }
            else
            {
                // Không có input - tự động ổn định theo tỷ lệ lượn tối ưu
                Vector2 vel = airplaneRigidbody2D.velocity;
                if (vel.magnitude > 1f)
                {
                    // Tính góc lượn tối ưu (best glide angle)
                    float optimalGlideAngle = -20f; // Góc lượn tối ưu cho hiệu suất tốt nhất
                    
                    float targetAngle = Mathf.LerpAngle(currentZ0, optimalGlideAngle, Time.deltaTime * 1.5f);
                    
                    Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
                    airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, targetAngle);
                    
                    // Tính toán tỷ lệ lượn hiện tại
                    float currentGlideRatio = Mathf.Abs(vel.x / vel.y);
                    float currentAltitude = airplaneRigidbody2D.transform.position.y;
                    float estimatedDistance = currentAltitude * currentGlideRatio;
                    
                    
                }
            }
            
            yield return null;
        }

        isFallingInSequence = false;
        Debug.Log("Kết thúc giai đoạn rơi - Tắt A/D điều khiển");

        Debug.Log("Máy bay đã hoàn thành chuỗi bay: ngang → bay lên (xoay dần) → giữ (có điều khiển) → rơi");
    }


    public bool isVelocity = true;
    public bool isHorizontalFlying = false;
    public bool isSlidingOnGround = false;
    public bool checkControlPlane = false;
    public bool isFallingInSequence = false; 
    void Update()
    {
        
        if (airplaneRigidbody2D == null) return;

        if(airplaneRigidbody2D.velocity.x < 28.5f & isControllable & !isHorizontalFlying & isPlay)
        {
            airplaneRigidbody2D.velocity = 29.0f * Vector2.right;
        }

        Vector2 currentPos = airplaneRigidbody2D.transform.position;
        distanceTraveled = Vector2.Distance(startPosition, currentPos);
        currentAltitude = currentPos.y - startPosition.y;
        if (currentAltitude < 0f) currentAltitude = 0f;
        rotationZ = airplaneRigidbody2D.transform.eulerAngles.z;
        if (rotationZ > 180f) rotationZ -= 360f;
        velocityText.text = airplaneRigidbody2D.velocity.magnitude.ToString("F2") + " m/s";
        rotationAngleZ();
        UpdateSliderAchievement();
        
        if (Plane.instance != null && Plane.instance.isGrounded)
        {
            return; 
        }

        ApplyAltitudeDrag();

        UpdateAirplaneRotationX();
        
        CountPerfectTime();

        if (isVelocity && !isHorizontalFlying && !isPlay)
        {
            airplaneRigidbody2D.velocity = 0.5f * airplaneRigidbody2D.velocity;
            isVelocity = false;

        }
        if (airplaneRigidbody2D.velocity.x > 100.0f)
        {
            airplaneRigidbody2D.velocity = new Vector2(100.0f, airplaneRigidbody2D.velocity.y);
        }

        if (isControllable)
        {
            HandleAircraftControl();
            if (isHoldingButtonUp)
            {
                
                PlainUp();
            }
            if (isHoldingButtonDown)
            {
                PlainDown();
            }
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !isHoldingButtonUp && !isHoldingButtonDown && isControllable && !isHorizontalFlying &&  !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            checkControlPlane = true;
        }
        else if (!isFallingInSequence) 
        {
            bool isAirborne = isPlay && (airplaneRigidbody2D.velocity.magnitude > 1f || currentAltitude > 5f);
            
            isSlidingOnGround = isPlay && !isAirborne && airplaneRigidbody2D.velocity.magnitude > 0.2f;
            
            if (isAirborne)
            {
                ApplyAutoRotationFromVelocity(); 
            }
            else if (isSlidingOnGround)
            {
                Vector3 currentRotation = airplaneRigidbody2D.transform.eulerAngles;
                float currentZ1 = currentRotation.z;
                if (currentZ1 > 180f) currentZ1 -= 360f;
                
                if (Mathf.Abs(currentZ1) > 0.1f)
                {
                    float targetZ = Mathf.Lerp(currentZ1, 0f, Time.deltaTime * 1.5f); 
                    airplaneRigidbody2D.transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, targetZ);
                }
            }
            else
            {
                Vector3 currentRotation = airplaneRigidbody2D.transform.eulerAngles;
                float currentZ1 = currentRotation.z;
                if (currentZ1 > 180f) currentZ1 -= 360f;
                
                if (Mathf.Abs(currentZ1) > 0.1f)
                {
                    airplaneRigidbody2D.transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
                }
            }
            
            if (isHoldingButtonUp)
            {
                PlainUp();
            }
            if (isHoldingButtonDown)
            {
                PlainDown();
            }
        }

        if (isPlay && isBoosted)
    {
        bool isBoostPressed = Input.GetKey(KeyCode.Space) || isUseClickerBooster;
        
        // ✅ CHỈ XỬ LÝ NẾU CÒN BOOST
        if (Input.GetKey(KeyCode.Space) && totalBoost > 0) 
        {
            Debug.Log("Space Key Down Detected - Boost available: " + totalBoost);
            StartBoostDecrease();
            
            // THÊM: Gọi BoosterUp để tăng tốc
            BoosterUp();
            
            if (Plane.instance != null)
            {
                Debug.Log("Trail Renderer Instances: " + 
                        "Left - " + (TrailRendererLeft.instance != null ? "Exists" : "Null") + ", " +
                        "Right - " + (TrailRendererRight.instance != null ? "Exists" : "Null") +
                        " TrailRendererRight ActiveInHierarchy: " + (TrailRendererRight.instance != null) + " " + (TrailRendererRight.instance.gameObject.activeInHierarchy.ToString()));
                
                if(TrailRendererLeft.instance != null && TrailRendererRight.instance != null)
                {
                    TrailRendererLeft.instance.isBoosterActive = true;
                    TrailRendererLeft.instance.PlayTrail();
                    TrailRendererRight.instance.isBoosterActive = true;
                    TrailRendererRight.instance.PlayTrail();
                    EffectFuel.instance.StartBlink();
                    Debug.Log("Both Trail Effects Activated");
                }
                else if(TrailRendererLeft.instance != null && TrailRendererLeft.instance.gameObject.activeInHierarchy)
                {
                    TrailRendererLeft.instance.isBoosterActive = true;
                    TrailRendererLeft.instance.PlayTrail();
                    EffectFuel.instance.StartBlink();
                    Debug.Log("Left Trail Effect Activated");
                }
                else if(TrailRendererRight.instance != null && TrailRendererRight.instance.gameObject.activeInHierarchy)
                {
                    TrailRendererRight.instance.isBoosterActive = true;
                    TrailRendererRight.instance.PlayTrail();
                    EffectFuel.instance.StartBlink();
                    Debug.Log("Right Trail Effect Activated");
                }
                else
                {
                    Debug.LogWarning("Right Trail Effect Activated");
                }
            }
        }
        else
        {
            // THÊM: Logic giảm tốc độ khi không boost HOẶC hết boost (dù có giữ Space)
            ApplyNaturalSpeedReduction();
            
            isBoosterActive = false;
            if (isBoostDecreasing)
            {
                StopBoostDecrease();
            }
            
            // SỬA: Xử lý trail renderer khi hết boost (kể cả khi giữ Space)
            if (Input.GetKey(KeyCode.Space) && totalBoost <= 0)
            {
                if (TrailRendererLeft.instance != null)
                {
                    TrailRendererLeft.instance.isBoosterActive = false;
                    TrailRendererLeft.instance.PlayTrail();
                }
                if (TrailRendererRight.instance != null)
                {
                    TrailRendererRight.instance.isBoosterActive = false;
                    TrailRendererRight.instance.PlayTrail();
                }
                if (EffectFuel.instance != null)
                {
                    EffectFuel.instance.StopBlink();
                }
                Debug.Log("Space pressed but no boost remaining - applying speed reduction");
            }
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopBoostDecrease();
            
            if (Plane.instance != null)
            {
                if(TrailRendererLeft.instance != null && TrailRendererRight.instance != null)
                {
                    TrailRendererLeft.instance.isBoosterActive = false;
                    TrailRendererLeft.instance.PlayTrail();
                    TrailRendererRight.instance.isBoosterActive = false;
                    TrailRendererRight.instance.PlayTrail();
                    EffectFuel.instance.StopBlink();
                    Debug.Log("Both Trail Effects Deactivated");
                }
                else if(TrailRendererLeft.instance != null && TrailRendererLeft.instance.gameObject.activeInHierarchy)
                {
                    TrailRendererLeft.instance.isBoosterActive = false;
                    TrailRendererLeft.instance.PlayTrail();
                    EffectFuel.instance.StopBlink();
                    Debug.Log("Left Trail Effect Deactivated");
                }
                else if(TrailRendererRight.instance != null && TrailRendererRight.instance.gameObject.activeInHierarchy)
                {
                    TrailRendererRight.instance.isBoosterActive = false;
                    TrailRendererRight.instance.PlayTrail();
                    EffectFuel.instance.StopBlink();
                    Debug.Log("Right Trail Effect Deactivated");
                }
            }
        }
        
        if (isBoostPressed && totalBoost > 0)
        {
            BoosterUp();
            buttonBoosterPlaneImage.color = Color.gray;
        }
        else
        {
            // isBoosterActive = false;
            if (isBoostDecreasing && !Input.GetKey(KeyCode.Space) && !isUseClickerBooster)
            {
                StopBoostDecrease();
            }
            buttonBoosterPlaneImage.color = Color.white;
        }
    }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            PauseGame();
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            AgainGame();
        }
        if (Input.GetKey(KeyCode.Keypad2))
        {
            totalMoney += 100000;
            PlayerPrefs.SetFloat("TotalMoney", totalMoney);
            PlayerPrefs.Save();
            SaveTotalMoney();

        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            MapSpawner.instance.isMapCityUnlocked = true;
            PlayerPrefs.SetInt("IsMapCityUnlocked", 1);
            MapSpawner.instance.isMapDesertUnlocked = true;
            PlayerPrefs.SetInt("IsMapDesertUnlocked", 1);
            MapSpawner.instance.isMapBeachUnlocked = true;
            PlayerPrefs.SetInt("IsMapBeachUnlocked", 1);
            MapSpawner.instance.isMapFieldUnlocked = true;
            PlayerPrefs.SetInt("IsMapFieldUnlocked", 1);
            MapSpawner.instance.isMapIceUnlocked = true;
            PlayerPrefs.SetInt("IsMapIceUnlocked", 1);
            MapSpawner.instance.isMapLavaUnlocked = true;
            PlayerPrefs.SetInt("IsMapLavaUnlocked", 1);
            PlayerPrefs.Save();
            AgainGame();
        }
        if (Input.GetKeyDown(KeyCode.Keypad4)) 
        {
            totalDiamond += 1000;
            totalDiamondText.text = totalDiamond.ToString();
            PlayerPrefs.SetInt("TotalDiamond", totalDiamond);
            PlayerPrefs.Save();
            SaveTotalDiamond();
        }

        if(Plane.instance != null && Input.GetKeyDown(KeyCode.Keypad5))
        {
            Plane.instance.isAirPortBeach = true;
            PlayerPrefs.SetInt("LastSafeLandingAirport", 1);
            PlayerPrefs.Save();
            return; 
        }
        if(Plane.instance != null && Input.GetKeyDown(KeyCode.Keypad6))
        {
            Plane.instance.isAirPortDesert = true;
            PlayerPrefs.SetInt("LastSafeLandingAirport", 2);
            PlayerPrefs.Save();
            Debug.Log("[CHEAT] Set safe landing to Desert (2), use Keypad0 to restart");
            return; 
        }
        if(Plane.instance != null && Input.GetKeyDown(KeyCode.Keypad7))
        {
            Plane.instance.isAirPortField = true;
            PlayerPrefs.SetInt("LastSafeLandingAirport", 3);
            PlayerPrefs.Save();
            Debug.Log("[CHEAT] Set safe landing to Field (3), use Keypad0 to restart");
            return; 
        }
        if(Plane.instance != null && Input.GetKeyDown(KeyCode.Keypad8))
        {
            Plane.instance.isAirPortIce = true;
            PlayerPrefs.SetInt("LastSafeLandingAirport", 4);
            PlayerPrefs.Save();
            Debug.Log("[CHEAT] Set safe landing to Ice (4), use Keypad0 to restart");
            return; 
        }
        if(Plane.instance != null && Input.GetKeyDown(KeyCode.Keypad9))
        {
            Plane.instance.isAirPortLava = true;
            PlayerPrefs.SetInt("LastSafeLandingAirport", 5);
            PlayerPrefs.Save();
            Debug.Log("[CHEAT] Set safe landing to Lava (5), use Keypad0 to restart");
            return; 
        }
        
        // Keypad0 để restart game với delay
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            Debug.Log("[CHEAT] Manual restart requested via Keypad0");
            AgainGame();
            return;
        }

        
        if (distanceText != null && !stopDisplayDistance) distanceText.text = distanceTraveled.ToString("F0") + " ft";
        if (altitudeText != null)
        {
            altitudeText.text = currentAltitude.ToString("F2") + " m";

            // Thêm warning khi gần giới hạn độ cao
            if (currentAltitude > maxAltitude * 0.8f)
            {
                altitudeText.color = Color.red; // Màu đỏ cảnh báo
            }
            else if (currentAltitude > maxAltitude * 0.6f)
            {
                altitudeText.color = Color.yellow; // Màu vàng cảnh báo
            }
            else
            {
                altitudeText.color = Color.white; // Màu bình thường
            }
        }
        if (rotationZText != null) rotationZText.text = rotationZ.ToString("F2") + " °";

        moneyText.text = Plane.instance.moneyCollect.ToString() + " $";
        totalDiamondText.text = totalDiamond.ToString("F0");

        if (sliderFuel.value <= 0f )
        {
            if(TrailRendererLeft.instance != null)
            {
                TrailRendererLeft.instance.StopTrail();
            }
            if(TrailRendererRight.instance != null)
            {
                TrailRendererRight.instance.StopTrail();
            }
        }
    }

    void HandleAircraftControl()
    {
        if (airplaneRigidbody2D == null) return;

        // LUÔN áp dụng auto-rotation theo velocity khi đang bay
        bool isAirborne = isPlay && (isControllable || 
                                   (!isControllable && airplaneRigidbody2D.velocity.magnitude > 1f) ||
                                   currentAltitude > 5f);
        
        if (isAirborne)
        {
            ApplyAutoRotationFromVelocity(); // Luôn xoay theo velocity khi bay
        }

        // Lấy góc hiện tại và chuyển về dạng -180 đến +180
        float currentZ = airplaneRigidbody2D.transform.eulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;

        // THÊM: Kiểm tra cả keyboard VÀ button
        bool isUpPressed = Input.GetKey(KeyCode.A) || isHoldingButtonUp || Input.GetKey(KeyCode.LeftArrow);
        bool isDownPressed = Input.GetKey(KeyCode.D) || isHoldingButtonDown || Input.GetKey(KeyCode.RightArrow);

        // Điều khiển lên (A key HOẶC button Up) - CHỈ ĐIỀU CHỈNH, KHÔNG GHI ĐÈ auto-rotation
        if (isUpPressed)
        {
            // Điều chỉnh góc lên từ góc hiện tại
            float adjustment = Time.deltaTime * 360f;
            float targetRotation = Mathf.Min(currentZ + adjustment, Plane.instance.maxUpAngle);
            
            // GIỮ NGUYÊN rotation.x hiện tại khi set rotation.z
            Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, targetRotation);

            // Nếu đang boost thì thêm lực theo hướng mới
            if (isBoosterActive)
            {
                float angleRad = targetRotation * Mathf.Deg2Rad;
                if (isBoosterActive)
                {
                    float angleRad1 = targetRotation * Mathf.Deg2Rad;
                    Vector2 dir = new Vector2(Mathf.Cos(angleRad1), Mathf.Sin(angleRad1));

                    // Ép thành phần X luôn >= 1
                    dir.x = Mathf.Max(dir.x, 1f);
                    dir.Normalize();

                    airplaneRigidbody2D.AddForce(dir * controlForce * 0.4f, ForceMode2D.Force);
                }

            }
        }

        // Điều khiển xuống (D key HOẶC button Down) - CHỈ ĐIỀU CHỈNH, KHÔNG GHI ĐÈ auto-rotation
        else if (isDownPressed)
        {
            // Điều chỉnh góc xuống từ góc hiện tại
            float adjustment = Time.deltaTime * 180f;
            float targetRotation = Mathf.Max(currentZ - adjustment, Plane.instance.maxDownAngle);
            
            // GIỮ NGUYÊN rotation.x hiện tại khi set rotation.z
            Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, targetRotation);

            // Nếu đang boost thì thêm lực theo hướng mới
            if (isBoosterActive)
            {
                float angleRad = targetRotation * Mathf.Deg2Rad;
                if (isBoosterActive)
                {
                    float angleRad1 = targetRotation * Mathf.Deg2Rad;
                    Vector2 dir = new Vector2(Mathf.Cos(angleRad1), Mathf.Sin(angleRad1));

                    // Ép thành phần X luôn >= 1
                    dir.x = Mathf.Max(dir.x, 1f);
                    dir.Normalize();

                    airplaneRigidbody2D.AddForce(dir * controlForce * 0.4f, ForceMode2D.Force);
                }

            }
        }
    }
    
    float ComputeUpTargetAngle(Vector2 velocity)
    {
        // Tính góc từ vector vận tốc, nhưng ưu tiên cảm giác "bay lên"
        // Bạn có thể điều chỉnh upBias để làm góc lên nhạy hơn so với Atan2 thuần túy
        float upBias = 0.1f; // >1 làm tăng ảnh hưởng của vy
        float velAngle = Mathf.Atan2(velocity.y * upBias, velocity.x) * Mathf.Rad2Deg;
        return Mathf.Clamp(velAngle, 0f, Plane.instance.maxUpAngle);
    }

    float ComputeDownTargetAngle(Vector2 velocity)
    {
        // Tính góc từ vector vận tốc, nhưng ưu tiên cảm giác "bay xuống"
        // downBias có thể >1 để nghiêng xuống mạnh hơn
        float downBias = 1f; // >1 làm tăng ảnh hưởng của vy (âm)
        float velAngle = Mathf.Atan2(velocity.y * downBias, velocity.x) * Mathf.Rad2Deg;
        return Mathf.Clamp(velAngle, Plane.instance.maxDownAngle, 0f);
    }

    // Hàm tự động xoay theo velocity - LUÔN hoạt động khi bay
    void ApplyAutoRotationFromVelocity()
    {
        if (airplaneRigidbody2D == null) return;
        
        Vector2 velocity = airplaneRigidbody2D.velocity;
        float minMoveSqr = minMoveThreshold * minMoveThreshold;

        if (velocity.sqrMagnitude > minMoveSqr)
        {
            float targetAngle;

            // Chọn phần tính góc theo hướng (vy dương => lên, vy âm => xuống)
            if (velocity.y > 0.0f)
            {
                targetAngle = ComputeUpTargetAngle(velocity);
            }
            else if (velocity.y < 0.0f)
            {
                targetAngle = ComputeDownTargetAngle(velocity);
            }
            else
            {
                targetAngle = 0f;
            }

            float currentZ = airplaneRigidbody2D.transform.eulerAngles.z;
            if (currentZ > 180f) currentZ -= 360f;
            float newZ = Mathf.Lerp(currentZ, targetAngle, Time.deltaTime * rotationSmooth);

            // GIỮ NGUYÊN rotation.x hiện tại khi set rotation.z
            Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, newZ);
        }
    }



    void UpdateRotationFromVelocity()
    {
        if (airplaneRigidbody2D == null) return;
        if (isControllable) return;
        Vector2 velocity = airplaneRigidbody2D.velocity;
        float minMoveSqr = minMoveThreshold * minMoveThreshold;

        if (Player.instance != null)
        {
            airplaneRigidbody2D.transform.rotation = Quaternion.Lerp(
                airplaneRigidbody2D.transform.rotation,
                Quaternion.identity,
                Time.deltaTime * 3f
            );
            return;
        }

        if (velocity.sqrMagnitude > minMoveSqr)
        {
            float targetAngle;

            // Chọn phần tính góc theo hướng (vy dương => lên, vy âm => xuống)
            if (velocity.y > 0.0f)
            {
                targetAngle = ComputeUpTargetAngle(velocity);
            }
            else if (velocity.y < 0.0f)
            {
                targetAngle = ComputeDownTargetAngle(velocity);
            }
            else
            {
                targetAngle = 0f;
            }

            float currentZ = airplaneRigidbody2D.transform.eulerAngles.z;
            if (currentZ > 180f) currentZ -= 360f;
            float newZ = Mathf.Lerp(currentZ, targetAngle, Time.deltaTime * rotationSmooth);

            // GIỮ NGUYÊN rotation.x hiện tại khi set rotation.z
            Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, newZ);
        }
        else
        {
            // GIỮ NGUYÊN rotation.x hiện tại khi lerp về identity
            Vector3 currentRotation = airplaneRigidbody2D.transform.eulerAngles;
            Vector3 targetRotation = new Vector3(currentRotation.x, 0f, 0f); // Giữ X, reset Y và Z
            airplaneRigidbody2D.transform.rotation = Quaternion.Lerp(
                airplaneRigidbody2D.transform.rotation,
                Quaternion.Euler(targetRotation),
                Time.deltaTime * 2f
            );
        }
    }

    // Hàm xoay rotation.x cho máy bay mượt mà
    void UpdateAirplaneRotationX()
    {
        if (!isAirplaneRotationXActive || airplaneRigidbody2D == null) 
        {
            return;
        }
        
        // SỬ DỤNG collision detection từ Plane.cs kết hợp với altitude/velocity
        bool isOnGround = isGroundCollisionDetected || 
                         (currentAltitude <= 2f && airplaneRigidbody2D.velocity.magnitude < 5f);
        
        // Điều kiện máy bay đang bay (CHÍNH XÁC HÓA điều kiện)
        bool isAirborne = isPlay && !isOnGround && 
                         (currentAltitude > 2f || // Ở trên 2m
                          airplaneRigidbody2D.velocity.magnitude > 5f || // Hoặc đang di chuyển nhanh
                          isControllable); // Hoặc đang trong giai đoạn điều khiển
        
        if (isOnGround) 
        {
            // Khi chạm đất, reset rotation X về 0
            if (Mathf.Abs(currentAirplaneRotationX) > 0.1f)
            {
                float resetSpeed = isGroundCollisionDetected ? 6f : 4f;
                currentAirplaneRotationX = Mathf.Lerp(currentAirplaneRotationX, 0f, Time.deltaTime * resetSpeed);
                Vector3 groundRotation = airplaneRigidbody2D.transform.eulerAngles;
                airplaneRigidbody2D.transform.rotation = Quaternion.Euler(currentAirplaneRotationX, groundRotation.y, groundRotation.z);
                
            }
            else
            {
                currentAirplaneRotationX = 0f;
                Vector3 groundRotation = airplaneRigidbody2D.transform.eulerAngles;
                airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, groundRotation.y, groundRotation.z);
            }
            return;
        }
        
        // KHI ĐANG BAY - DAO ĐỘNG ROTATION.X
        if (isAirborne) 
        {
            // Tính toán target rotation dựa trên sin wave
            float time = Time.time * airplaneRotationXSpeed;
            targetAirplaneRotationX = Mathf.Sin(time) * airplaneRotationXAmplitude;
            
            // THÊM: Nếu vừa bắt đầu bay (currentAirplaneRotationX gần 0), lerp chậm hơn để tránh nhảy
            float lerpSpeed = 5f;
            if (Mathf.Abs(currentAirplaneRotationX) < 0.5f && Mathf.Abs(targetAirplaneRotationX) > 3f)
            {
                lerpSpeed = 1.5f; // Chậm hơn để chuyển tiếp mượt từ 0
            }
            
            // Lerp từ current đến target với tốc độ điều chỉnh
            currentAirplaneRotationX = Mathf.Lerp(currentAirplaneRotationX, targetAirplaneRotationX, Time.deltaTime * lerpSpeed);
            
            
        }
        else 
        {
            // Khi không bay và không chạm đất (trạng thái trung gian), từ từ về 0
            if (Mathf.Abs(currentAirplaneRotationX) > 0.1f)
            {
                currentAirplaneRotationX = Mathf.Lerp(currentAirplaneRotationX, 0f, Time.deltaTime * 2.5f);
            }
            else
            {
                currentAirplaneRotationX = 0f;
            }
        }
        
        // Áp dụng rotation X cho máy bay - CHỈ THAY ĐỔI X, GIỮ NGUYÊN Y và Z
        Vector3 currentRotation = airplaneRigidbody2D.transform.eulerAngles;
        Vector3 newRotation = new Vector3(currentAirplaneRotationX, currentRotation.y, currentRotation.z);
        airplaneRigidbody2D.transform.rotation = Quaternion.Euler(newRotation);
    }

    // Hàm để dừng rotation X (có thể gọi khi cần)
    public void StopAirplaneRotationX()
    {
        isAirplaneRotationXActive = false;
        
        // Từ từ trả về rotation X = 0
        if (airplaneRigidbody2D != null)
        {
            Vector3 currentRotation = airplaneRigidbody2D.transform.eulerAngles;
            float targetX = Mathf.LerpAngle(currentRotation.x, 0f, Time.deltaTime * 3f);
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(targetX, currentRotation.y, currentRotation.z);
        }
    }

    // Hàm để bắt đầu rotation X
    public void StartAirplaneRotationX()
    {
        isAirplaneRotationXActive = true;
        Debug.Log("Started airplane rotation X oscillation");
    }

    // THÊM: Hàm để Plane.cs gọi khi chạm đất
    public void OnPlaneGroundCollision()
    {
        isGroundCollisionDetected = true;
        isPlaneOnGround = true;
        Debug.Log("GManager: Nhận thông báo máy bay chạm đất từ Plane.cs");
    }

    // THÊM: Hàm để reset trạng thái chạm đất
    public void ResetGroundCollision()
    {
        isGroundCollisionDetected = false;
        isPlaneOnGround = false;
        Debug.Log("GManager: Reset trạng thái chạm đất");
    }

    public bool isBoost = true;
    // THÊM: Hàm giảm tốc độ tự nhiên khi không boost
    public void ApplyNaturalSpeedReduction()
    {
        if (airplaneRigidbody2D == null) return;
        
        Vector2 currentVelocity = airplaneRigidbody2D.velocity;
        float currentSpeed = currentVelocity.magnitude;
        
        // Tính tốc độ cơ bản (không boost)
        float basePowerMultiplier = 1f + (ratePower / 100f);
        float baseSpeed = launchForce * 0.75f * basePowerMultiplier;
        
        // Giới hạn tốc độ cơ bản
        if (baseSpeed < 20f) baseSpeed = 20f;
        if (baseSpeed > 30f) baseSpeed = 30f;
        
        // Nếu tốc độ hiện tại > tốc độ cơ bản, giảm dần
        if (currentSpeed > baseSpeed)
        {
            float newSpeed = Mathf.Lerp(currentSpeed, baseSpeed, Time.deltaTime * 2f);
            airplaneRigidbody2D.velocity = currentVelocity.normalized * newSpeed;
            
            // Debug mỗi 30 frame
            if (Time.frameCount % 30 == 0)
            {
                Debug.Log($"Speed reduction: {currentSpeed:F1} → {newSpeed:F1} (target: {baseSpeed:F1})");
            }
        }
    }

    // THÊM: Hàm giảm tốc độ tự nhiên khi không boost


    public void BoosterUp()
    {
        if (airplaneRigidbody2D != null && totalBoost > 0)
        {
            isBoosterActive = true;
            
            // Bắt đầu giảm boost nếu chưa bắt đầu
            if (!isBoostDecreasing)
            {
                StartBoostDecrease();
            }
            
            if (isBoosterActive)
            {
                // Lấy góc hiện tại của máy bay (chuyển về radian)
                float currentAngleZ = airplaneRigidbody2D.transform.eulerAngles.z;
                if (currentAngleZ > 180f) currentAngleZ -= 360f; // Chuyển về -180 đến +180
                float angleRad = currentAngleZ * Mathf.Deg2Rad;

                // Tính vector lực theo góc máy bay
                Vector2 boostDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

                // Tính lực boost với giới hạn power và độ cao
                float basePowerMultiplier = 1f + (ratePower / 100f); // 1.0 + (power% / 100)
                float maxPowerMultiplier = maxPowerPercent / 100f; // 3.0 cho 300%
                float actualPowerMultiplier = Mathf.Min(basePowerMultiplier, maxPowerMultiplier);

                // Giảm hiệu quả boost theo độ cao
                float altitudeEfficiency = CalculateAltitudeEfficiency();

                float boostForce = 18f * actualPowerMultiplier * altitudeEfficiency;
                if (boostForce > 18f)
                {
                    boostForce = 18f; // Giới hạn lực boost tối đa
                }
                
                // THÊM: Kiểm tra vận tốc hiện tại TRƯỚC KHI áp dụng lực
                Vector2 currentVelocity = airplaneRigidbody2D.velocity;
                float currentSpeed = currentVelocity.magnitude;
                
                // GIỚI HẠN VẬN TỐC TỐI ĐA KHI BOOST
                float maxBoostVelocity = 60f; // Hoặc giá trị bạn muốn
                
                if (currentSpeed < maxBoostVelocity)
                {
                    // Chỉ áp dụng lực nếu chưa đạt tốc độ tối đa
                    airplaneRigidbody2D.AddForce(boostDirection * boostForce, ForceMode2D.Force);
                    
                    // THÊM: Kiểm tra sau khi áp dụng lực
                    Vector2 newVelocity = airplaneRigidbody2D.velocity;
                    if (newVelocity.magnitude > maxBoostVelocity)
                    {
                        // Clamp velocity về giới hạn tối đa
                        airplaneRigidbody2D.velocity = newVelocity.normalized * maxBoostVelocity;
                        Debug.Log($"Velocity clamped to {maxBoostVelocity} m/s");
                    }
                }
                else
                {
                    // Nếu đã đạt tốc độ tối đa, chỉ duy trì mà không tăng thêm
                    Debug.Log($"Max boost velocity reached: {currentSpeed:F1} m/s - No additional force applied");
                }

                Debug.Log($"BoosterUp - AngleZ: {currentAngleZ:F2}°, PowerMult: {actualPowerMultiplier:F2}, AltEff: {altitudeEfficiency:F2}, BoostForce: {boostForce:F2}, Speed: {currentSpeed:F1}");
            }
        }
        // Dòng 946-955
        else
        {
            // Nếu hết boost thì tắt booster
            isBoosterActive = false;
            StopBoostDecrease();
            buttonBoosterPlaneImage.color = Color.gray;
            isBoost = false;

            if (airplaneRigidbody2D != null)
            {
                Vector2 currentVel = airplaneRigidbody2D.velocity;
                float currentSpeed = currentVel.magnitude;
                
                // Tính tốc độ mục tiêu (không boost)
                float basePowerMultiplier = 1f + (ratePower / 100f);
                float targetSpeed = launchForce * 0.75f * basePowerMultiplier;
                if (targetSpeed < 20f) targetSpeed = 20f;
                if (targetSpeed > 40f) targetSpeed = 40f;
                
                // Giảm tốc độ dần về mục tiêu
                if (currentSpeed > targetSpeed)
                {
                    float newSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 2f);
                    airplaneRigidbody2D.velocity = currentVel.normalized * newSpeed;
                    Debug.Log($"Post-boost speed reduction: {currentSpeed:F1} → {newSpeed:F1}");
                }
            }
            
            if (Plane.instance != null)
            {
                if(TrailRendererLeft.instance != null && TrailRendererRight.instance != null)
                {
                    TrailRendererLeft.instance.isBoosterActive = false;
                    // TrailRendererLeft.instance.TrailEffect();
                    TrailRendererRight.instance.isBoosterActive = false;
                    // TrailRendererRight.instance.TrailEffect();
                    Debug.Log("Both Trail Effects Deactivated (Out of Boost)");
                }
                else if(TrailRendererLeft.instance != null && TrailRendererLeft.instance.gameObject.activeInHierarchy)
                {
                    TrailRendererLeft.instance.isBoosterActive = false;
                    // TrailRendererLeft.instance.TrailEffect();
                    Debug.Log("Left Trail Effect Deactivated (Out of Boost)");
                }
                else if(TrailRendererRight.instance != null && TrailRendererRight.instance.gameObject.activeInHierarchy)
                {
                    TrailRendererRight.instance.isBoosterActive = false;
                    // TrailRendererRight.instance.TrailEffect();
                    Debug.Log("Right Trail Effect Deactivated (Out of Boost)");
                }
            }
        }
    }

    public bool isBoosterUp = false;
    public bool isUseClickerBooster = false;
    
    // Thay thế PointerDownBooster() và PointerUpBooster() bằng:
    public void OnPointerDownButtonBooster()
    {
        isUseClickerBooster = true;
        buttonBoosterPlaneImage.color = Color.gray;
        Debug.Log("OnPointerDownButtonBooster - isUseClickerBooster = true");
    }

    public void OnPointerUpButtonBooster()
    {
        isUseClickerBooster = false;
        buttonBoosterPlaneImage.color = Color.white;
        Debug.Log("OnPointerUpButtonBooster - isUseClickerBooster = false");
    }

    void ApplyAltitudeDrag()
    {
        if (airplaneRigidbody2D == null) return;

        // Tính lực kéo xuống dựa trên độ cao
        if (currentAltitude > altitudeDragStart)
        {
            // Tính hệ số lực kéo (tăng dần theo độ cao)
            float altitudeRatio = (currentAltitude - altitudeDragStart) / (maxAltitude - altitudeDragStart);
            altitudeRatio = Mathf.Clamp01(altitudeRatio); // Giới hạn 0-1

            // Lực kéo xuống tăng theo bình phương của độ cao (mạnh hơn khi cao hơn)
            float dragForce = altitudeDragMultiplier * altitudeRatio * altitudeRatio;

            // Áp dụng lực kéo xuống
            Vector2 downwardForce = new Vector2(0f, -dragForce);
            airplaneRigidbody2D.AddForce(downwardForce, ForceMode2D.Force);

            // Giảm vận tốc tổng thể khi ở độ cao lớn
            if (currentAltitude > maxAltitude * 0.8f) // Từ 80% độ cao tối đa
            {
                Vector2 velocity = airplaneRigidbody2D.velocity;
                float dampingFactor = Mathf.Lerp(1f, velocityDampingFactor, altitudeRatio);
                airplaneRigidbody2D.velocity = velocity * dampingFactor;
            }

            // Giới hạn cứng tại độ cao tối đa
            if (currentAltitude >= maxAltitude)
            {
                Vector2 velocity = airplaneRigidbody2D.velocity;
                if (velocity.y > 0f) // Nếu đang bay lên
                {
                    velocity.y = Mathf.Min(velocity.y, 0f); // Chặn không cho bay lên nữa
                    airplaneRigidbody2D.velocity = velocity;
                }

                // Áp dụng lực kéo xuống mạnh
                Vector2 hardLimit = new Vector2(0f, -altitudeDragMultiplier * 2f);
                airplaneRigidbody2D.AddForce(hardLimit, ForceMode2D.Force);
            }
        }
    }

    float CalculateAltitudeEfficiency()
    {
        // Hiệu quả giảm dần theo độ cao
        if (currentAltitude <= altitudeDragStart)
        {
            return 1f; // Hiệu quả 100% ở độ cao thấp
        }

        float altitudeRatio = (currentAltitude - altitudeDragStart) / (maxAltitude - altitudeDragStart);
        altitudeRatio = Mathf.Clamp01(altitudeRatio);

        // Hiệu quả giảm từ 100% xuống 30% ở độ cao tối đa
        float efficiency = Mathf.Lerp(1f, 0.3f, altitudeRatio);
        return efficiency;
    }
    public bool isGamePaused = false;
    public void PauseGame()
    {
        if (!isGamePaused)
        {
            Time.timeScale = 0f;
            isGamePaused = true;
        }
        else
        {
            Time.timeScale = 1f;
            isGamePaused = false;
        }
    }
    public int saveTime = 0;
    public void AgainGame()
    {
        money = 0;
        saveTime = Settings.instance.currentTime;
        PlayerPrefs.SetInt("SaveTime", saveTime);
        
        // Đảm bảo tất cả PlayerPrefs được save
        PlayerPrefs.Save();
        Debug.Log("[AGAIN_GAME] PlayerPrefs saved, starting delayed scene reload...");
        
        isVelocity = true;
        
        // Thêm delay nhỏ để đảm bảo PlayerPrefs được flush to disk
        StartCoroutine(DelayedSceneReload());
        
        SaveTotalMoney();
    }
    
    // Coroutine để delay reload scene
    IEnumerator DelayedSceneReload()
    {
        // Chờ 0.1 giây để đảm bảo PlayerPrefs được flush
        yield return new WaitForSecondsRealtime(0.1f);
        
        int lastSafeLanding = PlayerPrefs.GetInt("LastSafeLandingAirport", -1);
        Debug.Log($"[AGAIN_GAME] About to reload scene. LastSafeLanding: {lastSafeLanding}");
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }



    public bool isHoldingButtonUp = false;
    public bool isHoldingButtonDown = false;
    public bool isUseClicker = false;
    public void OnPointerDownButtonUp()
    {
        isHoldingButtonUp = true;
        buttonUpImage.color = Color.gray;
        isUseClicker = true;
    }
    public void OnPointerUpButtonUp()
    {
        isHoldingButtonUp = false;
        buttonUpImage.color = Color.white;
        isUseClicker = false;
    }
    public void OnPointerDownButtonDown()
    {
        isHoldingButtonDown = true;
        buttonDownImage.color = Color.gray;
        isUseClicker = true;
    }
    public void OnPointerUpButtonDown()
    {
        isHoldingButtonDown = false;
        buttonDownImage.color = Color.white;
        isUseClicker = false;
    }

    public float maxBoostSpeed = 50f; 
public void PlainUp()
{
    float currentZ = airplaneRigidbody2D.transform.eulerAngles.z;
    if (currentZ > 180f) currentZ -= 360f;
    float newTargetRotation = Mathf.Min(currentZ + Time.deltaTime * Plane.instance.rotationOnPlane, Plane.instance.maxUpAngle);
    
    Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
    airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, newTargetRotation);

    Vector2 currentVelocity = airplaneRigidbody2D.velocity;
    float currentSpeed = currentVelocity.magnitude; // Lưu tốc độ hiện tại
    
    
    Vector2 newDirection = airplaneRigidbody2D.transform.right; // Transform.right luôn có magnitude = 1
    
    // Áp dụng velocity mới với tốc độ gốc
    airplaneRigidbody2D.velocity = newDirection * currentSpeed;
    // Debug.Log("PlainUp - New Direction: " + newDirection + ", Current Speed: " + currentSpeed );
    
    // Giới hạn tốc độ tối đa
    if (airplaneRigidbody2D.velocity.magnitude > maxBoostSpeed)
    {
        airplaneRigidbody2D.velocity = airplaneRigidbody2D.velocity.normalized * maxBoostSpeed;
    }
}

public void PlainDown()
{
    float currentZ = airplaneRigidbody2D.transform.eulerAngles.z;
    if (currentZ > 180f) currentZ -= 360f;
    float newTargetRotation = Mathf.Max(currentZ - Time.deltaTime * Plane.instance.rotationOnPlane, Plane.instance.maxDownAngle);
    
    Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
    airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, newTargetRotation);

    Vector2 currentVelocity = airplaneRigidbody2D.velocity;
    float currentSpeed = currentVelocity.magnitude; // Lưu tốc độ hiện tại
    
    Vector2 newDirection = airplaneRigidbody2D.transform.right; // Transform.right luôn có magnitude = 1
    
    // Áp dụng velocity mới với tốc độ gốc
    airplaneRigidbody2D.velocity = newDirection * currentSpeed;
    // Debug.Log("PlainDown - New Direction: " + newDirection + ", Current Speed: " + currentSpeed );
    
    // Giới hạn tốc độ tối đa
    if (airplaneRigidbody2D.velocity.magnitude > maxBoostSpeed)
    {
        airplaneRigidbody2D.velocity = airplaneRigidbody2D.velocity.normalized * maxBoostSpeed;
    }
}






   
    public void ResetGame()
    {
        Debug.Log("Reset Game");
    }

    public IEnumerator DecreaseSliderFuel()
    {
        float startValue = sliderFuel.value;
        float endValue = 0f;
        
        while (currentControlTimer < currentControlDuration)
        {
            // ✅ CHỈ ĐỌC giá trị từ biến chia sẻ với kiểm tra an toàn
            if (currentControlDuration > 0)
            {
                // ✅ Clamp01 đảm bảo progress LUÔN trong [0, 1]
                float currentProgress = Mathf.Clamp01(currentControlTimer / currentControlDuration);
                sliderFuel.value = Mathf.Lerp(startValue, endValue, currentProgress);
                
                // Debug mỗi giây
                if (Time.frameCount % 60 == 0)
                {
                    Debug.Log($"[SLIDER] Progress: {currentProgress:P0} ({currentControlTimer:F1}s / {currentControlDuration:F1}s), SliderValue: {sliderFuel.value:F2}");
                }
            }

            yield return null;
        }

        sliderFuel.value = endValue;
        Debug.Log("[SLIDER] Fuel depleted - slider = 0");
    }
    public float totalBoostMaxPower ;
    // Giảm boost theo thời gian
    private IEnumerator DecreaseSliderBoost()
    {
        Debug.Log("Started DecreaseSliderBoost Coroutine");
        while (isBoostDecreasing && totalBoost > 0)
        {
            Debug.Log("Decreasing Boost... Current Boost: " + totalBoost);
            
            float decreaseAmount = boostDecreaseRate * Time.deltaTime;
            
            totalBoost -= decreaseAmount;

            if (totalBoost < 0)
            {
                totalBoost = 0;
            }

            sliderBooster.value = totalBoost / totalBoostMaxPower;

            if (levelBoostText != null)
            {
                levelBoostText.text = "Lv" + levelBoost + " | " + totalBoost.ToString("F0");
            }

            if (totalBoost <= 0)
            {
                isBoostDecreasing = false;
                isBoosterActive = false;
                break;
            }

            yield return null;
        }
        boostCoroutine = null;
    }

    public void StartBoostDecrease()
    {
        if (!isBoostDecreasing && totalBoost > 0)
        {
            isBoostDecreasing = true;
            if (boostCoroutine != null)
            {
                StopCoroutine(boostCoroutine);
            }
            boostCoroutine = StartCoroutine(DecreaseSliderBoost());
        }
    }

    public void StopBoostDecrease()
    {
        isBoostDecreasing = false;
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
            boostCoroutine = null;
        }
    }

    // Nâng cấp các chỉ số
    public int ratePower = 0;
    public int rateFuel = 0;
    public int rateBoost = 0;
    public float moneyPower = 50f;
    public float moneyFuel = 50f;
    public float moneyBoost = 50f;
    public bool isFuelMax = false;
    public bool isPowerMax = false;
    public bool isBoostMax = false;
    public float upgradeMultiplierPower = 1.25f;
    public float upgradeMultiplierFuel = 1.25f;
    public float upgradeMultiplierBoost = 1.25f;

    public void UpgradeFuel()
    {
        if (!isFuelMax && totalMoney >= moneyFuel)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.upgradeSoundClip);
            levelFuel += 1;
            rateFuel += 5;
            totalMoney -= moneyFuel;
            MissionAchievements.instance.achievementMission1Progress += ((int)moneyFuel);
            MissionAchievements.instance.UpdateAchievementMission();
            moneyFuel = moneyFuel * upgradeMultiplierFuel;
            totalMoney = (int)Math.Round(totalMoney, 0);
            totalMoneyText.text = totalMoney.ToString("F0");
            PlayerPrefs.SetFloat("TotalMoney", totalMoney);
            PlayerPrefs.Save();

            if(moneyFuel > 100000 && moneyFuel <= 1000000) upgradeMultiplierFuel = 1.15f;
             else if(moneyFuel > 1000000) upgradeMultiplierFuel = 1.05f;

            moneyFuel = (int)Math.Round(moneyFuel, 1);
            if (moneyFuel > 999)
            {
                if (moneyFuel >= 1000 && moneyFuel < 1000000)
                {
                    fuelMoneyText.text = (moneyFuel / 1000f).ToString("F1") + "K";
                }
                else if (moneyFuel >= 1000000 && moneyFuel < 1000000000)
                {
                    fuelMoneyText.text = (moneyFuel / 1000000f).ToString("F1") + "M";
                }
            }
            else
            {
                fuelMoneyText.text = "" + moneyFuel;
            }
            CalculateUpgradeValues(); 
            SaveUpgradeData();
            SaveTotalMoney();
            CheckPlane.instance.CheckMoneyForUpgradeButtonNext();
            levelFuelText.text = "Fuel capacity increased by " + rateFuel + "%";
            Debug.Log($"UpgradeFuel: Level {levelFuel}, Rate {rateFuel}%, Duration {durationFuel}s");
            PositionX.instance.timePerfect = durationFuel * 0.5f;
            
            // Thêm null check cho LuckyWheel.instance
            if (LuckyWheel.instance != null)
            {
                LuckyWheel.instance.checkValueLuckyWheel();
            }
            else
            {
                Debug.LogWarning("LuckyWheel.instance is null in UpgradeFuel");
            }
        }
    }

    void CalculateUpgradeValues()
    {
        durationFuel = baseDurationFuel * (1 + (rateFuel / 100f));
        durationFuel = (float)Math.Round(durationFuel, 1);

        launchForce = baseLaunchForce * (1 + (ratePower / 100f));
        launchForce = (float)Math.Round(launchForce, 1);

        totalBoost = baseTotalBoost * (1 + (rateBoost / 100f));
        totalBoost = (float)Math.Round(totalBoost, 1);

        Debug.Log($"Calculated values - Fuel: {durationFuel}s, Power: {launchForce}, Boost: {totalBoost}");
    }

    public void UpgradeBoost()
    {
        if (!isBoostMax && totalMoney >= moneyBoost)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.upgradeSoundClip);
            levelBoost += 1;
            rateBoost += 5;
            totalMoney -= moneyBoost;
            MissionAchievements.instance.achievementMission1Progress += ((int)moneyBoost);
            MissionAchievements.instance.UpdateAchievementMission();
            moneyBoost = moneyBoost * upgradeMultiplierBoost;
            totalMoney = (int)Math.Round(totalMoney, 0);
            totalMoneyText.text = totalMoney.ToString("F0");
            totalBoostMaxPower = totalBoost;
            PlayerPrefs.SetFloat("TotalMoney", totalMoney);
            PlayerPrefs.Save();

            if(moneyBoost > 100000 && moneyBoost <= 1000000) upgradeMultiplierBoost = 1.15f;
             else if(moneyBoost > 1000000) upgradeMultiplierBoost = 1.05f;

            moneyBoost = (int)Math.Round(moneyBoost, 1);
            if (moneyBoost > 999)
            {

                if (moneyBoost >= 1000 && moneyBoost < 1000000)
                {
                    boostMoneyText.text = (moneyBoost / 1000f).ToString("F1") + "K";
                }
                else if (moneyBoost >= 1000000 && moneyBoost < 1000000000)
                {
                    boostMoneyText.text = (moneyBoost / 1000000f).ToString("F1") + "M";
                }
            }
            else
            {
                boostMoneyText.text = "" + moneyBoost;
            }

            CalculateUpgradeValues();

            SaveUpgradeData();
            SaveTotalMoney();
            CheckPlane.instance.CheckMoneyForUpgradeButtonNext();
            levelBoostText.text = "Flight stability increased " + rateBoost + "%";
            Debug.Log($"UpgradeBoost: Level {levelBoost}, Rate {rateBoost}%, TotalBoost {totalBoost}");
            
            // Thêm null check cho LuckyWheel.instance
            if (LuckyWheel.instance != null)
            {
                LuckyWheel.instance.checkValueLuckyWheel();
            }
            else
            {
                Debug.LogWarning("LuckyWheel.instance is null in UpgradeBoost");
            }
        }

    }


    public void UpgradePower()
    {
        if (!isPowerMax && totalMoney >= moneyPower)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.upgradeSoundClip);
            levelPower += 1;
            ratePower += 5;
            totalMoney -= moneyPower;
            MissionAchievements.instance.achievementMission1Progress += ((int)moneyPower);
            MissionAchievements.instance.UpdateAchievementMission();
            moneyPower = moneyPower * upgradeMultiplierPower;
            totalMoney = (int)Math.Round(totalMoney, 0);
            totalMoneyText.text = totalMoney.ToString("F0");
            PlayerPrefs.SetFloat("TotalMoney", totalMoney);
            PlayerPrefs.Save();

            if(moneyPower > 100000 && moneyPower <= 1000000) upgradeMultiplierPower = 1.15f;
             else if(moneyPower > 1000000) upgradeMultiplierPower = 1.05f;

            moneyPower = (int)Math.Round(moneyPower, 1);
            if (moneyPower > 999)
            {

                if (moneyPower >= 1000 && moneyPower < 1000000)
                {
                    powerMoneyText.text = (moneyPower / 1000f).ToString("F1") + "K";
                }
                else if (moneyPower >= 1000000 && moneyPower < 1000000000)
                {
                    powerMoneyText.text = (moneyPower / 1000000f).ToString("F1") + "M";
                }
            }
            else
            {
                powerMoneyText.text = "" + moneyPower;
            }
            CalculateUpgradeValues();
            SaveUpgradeData();
            SaveTotalMoney();
            CheckPlane.instance.CheckMoneyForUpgradeButtonNext();

            levelPowerText.text = "Initial thrust increased by " + ratePower + "%";
            Debug.Log($"UpgradePower: Level {levelPower}, Rate {ratePower}%, Force {launchForce}");
            
            // Thêm null check cho LuckyWheel.instance
            if (LuckyWheel.instance != null)
            {
                LuckyWheel.instance.checkValueLuckyWheel();
            }
            else
            {
                Debug.LogWarning("LuckyWheel.instance is null in UpgradePower");
            }
        }
    }

    public void LoadMoneyUpgrade()
    {
        Debug.Log("LoadMoneyUpgrade called");
        
        // Xử lý Power Money
        if (moneyPower > 999)
        {
            if (moneyPower >= 1000 && moneyPower < 1000000)
            {
                powerMoneyText.text = (moneyPower / 1000f).ToString("F1") + "K";
                Debug.Log("Power money text set for K value");
            }
            else if (moneyPower >= 1000000 && moneyPower < 1000000000)
            {
                powerMoneyText.text = (moneyPower / 1000000f).ToString("F1") + "M";
            }
        }
        else
        {
            powerMoneyText.text = "" + moneyPower;
        }
        
        // Xử lý Boost Money
        if (moneyBoost > 999)
        {
            if (moneyBoost >= 1000 && moneyBoost < 1000000)
            {
                boostMoneyText.text = (moneyBoost / 1000f).ToString("F1") + "K";
            }
            else if (moneyBoost >= 1000000 && moneyBoost < 1000000000)
            {
                boostMoneyText.text = (moneyBoost / 1000000f).ToString("F1") + "M";
            }
        }
        else
        {
            boostMoneyText.text = "" + moneyBoost;
        }
        
        // Xử lý Fuel Money
        if (moneyFuel > 999)
        {
            if (moneyFuel >= 1000 && moneyFuel < 1000000)
            {
                fuelMoneyText.text = (moneyFuel / 1000f).ToString("F1") + "K";
            }
            else if (moneyFuel >= 1000000 && moneyFuel < 1000000000)
            {
                fuelMoneyText.text = (moneyFuel / 1000000f).ToString("F1") + "M";
            }
        }
        else
        {
            fuelMoneyText.text = "" + moneyFuel;
        }
        
        Debug.Log("LoadMoneyUpgrade completed");
    }
    public float tempDistanceTraveled = 0f;

    [Header("Achievement Milestone System")]
    public float achievementStartDistance = 0f; 
    public float milestoneDistance = 380f; 
    public float milestoneDistance2 = 360f;
    void UpdateSliderAchievement()
    {
        if (sliderAchievement != null)
        {
            float distanceInCurrentMilestone = distanceTraveled - achievementStartDistance;
            
            float targetValue = Mathf.Clamp01(distanceInCurrentMilestone / milestoneDistance);

            float smoothSpeed = 2f;
            sliderAchievement.fillAmount = Mathf.Lerp(sliderAchievement.fillAmount, targetValue, smoothSpeed * Time.deltaTime);
            if(sliderAchievement.fillAmount >= 0.75f)
            {
                sliderAchievement.fillAmount = 0.75f;
            }
        }

        if (currentAltitude > altitudeDragStart)
        {
            float altitudeRatio = (currentAltitude - altitudeDragStart) / (maxAltitude - altitudeDragStart);
            float efficiency = CalculateAltitudeEfficiency();

            if (currentAltitude % 50f < 1f)
            {
                Debug.Log($"Altitude: {currentAltitude:F0}m/{maxAltitude}m - Efficiency: {efficiency:F1}% - Drag: {altitudeRatio:F2}");
            }
        }
    }

    public void ResetAchievementSlider()
    {
        achievementStartDistance = distanceTraveled; 
        sliderAchievement.fillAmount = 0f; 
        Debug.Log($"Achievement slider reset at distance: {achievementStartDistance:F0}m");
    }

    [Header("RotationZ Oscillation")]

    public float angleRangeMax = 22f;
    public float angleRangeMin = -30f;       
    public float speed = 1f;           
    public bool isRotationOscillating = true;
    public bool isCheckBonus = true;
    private float startZ;
    private float startWheelZ;
    public bool isBonus = false;
    public bool isSuperBonus = false;
    public void rotationAngleZ()
    {
        if (isRotationOscillating)
        {
            if (arrowAngleZ == null) return;
            float angleAverange = (angleRangeMax + angleRangeMin) / 2;

            float angle = Mathf.Sin(Time.time * speed) * (angleRangeMax - angleRangeMin) / 2 + angleAverange;

            arrowAngleZ.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            if(angle > -3f && angle < -8f)
            {
                speed = 5f;
            }
            else{
                speed = 3f;
            }
        }
        else
        {
            if (isCheckBonus)
            {
                float angleNew = arrowAngleZ.transform.eulerAngles.z;
                float angle = angleNew > 180f ? angleNew - 360f : angleNew;
                if (angle < -3f && angle > -8f)
                {
                    Debug.Log("isBonus: " + isBonus);
                    isCheckBonus = false;
                }
                else{
                    Debug.Log("isBonus: " + isBonus);
                    isCheckBonus = false;
                }
            }
        }

    }

    void LoadUpgradeData()
    {
        levelPower = PlayerPrefs.GetInt("LevelPower", 0);
        levelFuel = PlayerPrefs.GetInt("LevelFuel", 0);
        levelBoost = PlayerPrefs.GetInt("LevelBoost", 0);

        ratePower = PlayerPrefs.GetInt("RatePower", 0);
        rateFuel = PlayerPrefs.GetInt("RateFuel", 0);
        rateBoost = PlayerPrefs.GetInt("RateBoost", 0);

        moneyPower = PlayerPrefs.GetFloat("MoneyPower", 50f);
        moneyFuel = PlayerPrefs.GetFloat("MoneyFuel", 50f);
        moneyBoost = PlayerPrefs.GetFloat("MoneyBoost", 50f);

        isPowerMax = PlayerPrefs.GetInt("IsPowerMax", 0) == 1;
        isFuelMax = PlayerPrefs.GetInt("IsFuelMax", 0) == 1;
        isBoostMax = PlayerPrefs.GetInt("IsBoostMax", 0) == 1;

        Debug.Log($"Loaded upgrade data - rateFuel: {rateFuel}%, ratePower: {ratePower}%, rateBoost: {rateBoost}%");
        levelPowerText.text = "Initial thrust increased by " + ratePower + "%";
        levelFuelText.text = "Flight stability increased " + rateFuel + "%";
        levelBoostText.text = "Fuel capacity increased by " + rateBoost + "%";

        powerMoneyText.text = isPowerMax ? "MAX" : moneyPower.ToString("F0");
        fuelMoneyText.text = isFuelMax ? "MAX" : moneyFuel.ToString("F0");
        boostMoneyText.text = isBoostMax ? "MAX" : moneyBoost.ToString("F0");
    }

    void SaveUpgradeData()
    {
        PlayerPrefs.SetInt("LevelPower", levelPower);
        PlayerPrefs.SetInt("LevelFuel", levelFuel);
        PlayerPrefs.SetInt("LevelBoost", levelBoost);

        PlayerPrefs.SetInt("RatePower", ratePower);
        PlayerPrefs.SetInt("RateFuel", rateFuel);
        PlayerPrefs.SetInt("RateBoost", rateBoost);

        PlayerPrefs.SetFloat("MoneyPower", moneyPower);
        PlayerPrefs.SetFloat("MoneyFuel", moneyFuel);
        PlayerPrefs.SetFloat("MoneyBoost", moneyBoost);

        PlayerPrefs.SetInt("IsPowerMax", isPowerMax ? 1 : 0);
        PlayerPrefs.SetInt("IsFuelMax", isFuelMax ? 1 : 0);
        PlayerPrefs.SetInt("IsBoostMax", isBoostMax ? 1 : 0);

        PlayerPrefs.Save();
        Debug.Log($"Saved upgrade data - rateFuel: {rateFuel}%, durationFuel: {durationFuel}s");
    }

    public void SaveTotalMoney()
    {
        
        
        totalMoney = PlayerPrefs.GetFloat("TotalMoney", 0);
        if (totalMoney > 999)
        {

            if (totalMoney >= 1000 && totalMoney < 1000000)
            {
                totalMoneyText.text = (totalMoney / 1000f).ToString("F1") + "k";
            }
            else if (totalMoney >= 1000000 && totalMoney < 1000000000)
            {
                totalMoneyText.text = (totalMoney / 1000000f).ToString("F1") + "m";
            }
        }
        else
        {
            totalMoneyText.text = "" + totalMoney;
        }
    }
    public void SaveTotalDiamond()
    {
        totalDiamond = PlayerPrefs.GetInt("TotalDiamond", 0);
        if (totalDiamond > 999)
        {

            if (totalDiamond >= 1000 && totalDiamond < 1000000)
            {
                totalDiamondText.text = (totalDiamond / 1000f).ToString("F1") + "k";
            }
            else if (totalDiamond >= 1000000 && totalDiamond < 1000000000)
            {
                totalDiamondText.text = (totalDiamond / 1000000f).ToString("F1") + "m";
            }
        }
        else
        {
            totalDiamondText.text = "" + totalDiamond;
        }
    }


    public bool isTurnWheel = false;
    public void turnWheel()
    {
        isTurnWheel = true;
        Debug.Log("isTurnWheel: " + isTurnWheel);
    }
    
    public RectTransform leftHomeImage;
    public RectTransform rightHomeImage;
    public RectTransform downFuelImage;
    public RectTransform upAircraftImage;
    public RectTransform leftCoinImage;
    public RectTransform leftDiamondImage;
    public bool isHomeUp = false;
    public bool isHomeDown = false;
    public bool isFuelDown = false;
    public bool isAircraftUp = false;

    public void homeGameLeft()
    {
        if (isHomeUp)
        {
            leftHomeImage.DOAnchorPosX(leftHomeImage.anchoredPosition.x - 600f, duration)
                .SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    isHomeUp = false;
                    isFuelDown = true;
                    fuelPlayDown();
                    leftHomeImage.gameObject.SetActive(false);
                });
            leftCoinImage.DOAnchorPosX(leftCoinImage.anchoredPosition.x - 110f, duration)
                .SetEase(Ease.OutCubic);
            leftDiamondImage.DOAnchorPosX(leftDiamondImage.anchoredPosition.x - 110f, duration)
                .SetEase(Ease.OutCubic);
        }
    }

    public void homeGameRight()
    {
        if (isHomeDown)
        {
            rightHomeImage.DOAnchorPosX(rightHomeImage.anchoredPosition.x + 600f, duration)
                .SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    isHomeDown = false;
                    isAircraftUp = true;
                    StartCoroutine(AircraftPlayUp());
                    rightHomeImage.gameObject.SetActive(false);
                });
        }
    }

    public void fuelPlayDown()
    {
        if (isFuelDown)
        {
            downFuelImage.DOAnchorPosY(downFuelImage.anchoredPosition.y - 390f, duration)
                .SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    isFuelDown = false;
                    if(Shop.instance.isCheckedPlaneIndex == 5)
                    {
                        Shop.instance.bullets.gameObject.SetActive(true);
                    }
                    else
                    {
                        Shop.instance.bullets.gameObject.SetActive(false);
                    }
                });
        }
    }
    public IEnumerator AircraftPlayUp()
    {
        
            yield return new WaitForSeconds(2f);
        if (isAircraftUp)
            yield return new WaitForSeconds(1f);
        {
            upAircraftImage.DOAnchorPosY(upAircraftImage.anchoredPosition.y + 280f, duration)
                .SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    isAircraftUp = false;
                });
        }
    }

    public void AddCoinByLuckyWheel(int coinAmount)
    {
        totalMoney += coinAmount;
        totalMoneyText.text = totalMoney.ToString();
        PlayerPrefs.SetFloat("TotalMoney", totalMoney);
        PlayerPrefs.Save();
        SaveTotalMoney();
    }
    
    public void AddDiamondByLuckyWheel(int diamondAmount)
    {
        totalDiamond += diamondAmount;
        totalDiamondText.text = totalDiamond.ToString();
        PlayerPrefs.SetInt("TotalDiamond", totalDiamond);
        PlayerPrefs.Save();
        SaveTotalDiamond();
    }

    // Đếm thời gian perfect
    public float perfectTime = 0f;
    private bool isEndingMaxPower = false;
    
    public void CountPerfectTime()
    {
        if (PositionX.instance.isMaxPower && !isEndingMaxPower && isBoosted)
        {
            perfectTime += Time.deltaTime;
            if (perfectTime >= PositionX.instance.timePerfect)
            {
                StartCoroutine(EndMaxPowerEffect());
            }
        }
        else if (!PositionX.instance.isMaxPower)
        {
            perfectTime = 0f;
        }
    }

    IEnumerator EndMaxPowerEffect()
    {
        isEndingMaxPower = true;
        Debug.Log("Starting Max Power End Effect - Blinking for 2 seconds");
        
        // Nhấp nháy giữa màu gốc và gold trong 2 giây
        float blinkDuration = 2f;
        float blinkInterval = 0.15f; // Tốc độ nhấp nháy
        float elapsed = 0f;
        bool isGold = true;
        
        while (elapsed < blinkDuration)
        {
            if (EffectAirplane.instance != null)
            {
                if (isGold)
                {
                    EffectAirplane.instance.MakePlaneGold();
                    // Đổi propellers và wheels sang gold
                    foreach (var propeller in EffectRotaryFront.instances)
                    {
                        propeller.gameObject.GetComponent<Renderer>().material = Plane.instance.GoldMaterial;
                    }
                    if (DestroyWheels.instance != null)
                    {
                        DestroyWheels.instance.Golden();
                    }
                }
                else
                {
                    EffectAirplane.instance.RestoreOriginalMaterial();
                    // Restore propellers và wheels về màu gốc
                    EffectRotaryFront.RestoreAllOriginalMaterials();
                    if (DestroyWheels.instance != null)
                    {
                        DestroyWheels.instance.RestoreOriginalMaterial();
                    }
                }
                isGold = !isGold;
            }
            
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }
        
        // Restore về màu gốc cho tất cả
        if (EffectAirplane.instance != null)
        {
            EffectAirplane.instance.RestoreOriginalMaterial();
        }
        
        // Restore propellers về màu gốc
        EffectRotaryFront.RestoreAllOriginalMaterials();
        
        // Restore wheels về màu gốc
        if (DestroyWheels.instance != null)
        {
            DestroyWheels.instance.RestoreOriginalMaterial();
        }
        
        // Tắt Max Power
        PositionX.instance.isMaxPower = false;
        isEndingMaxPower = false;
        perfectTime = 0f;
        if(TrailRendererRight.instance != null)
        {
            TrailRendererRight.instance.ChangeColor();
        }
        if(TrailRendererLeft.instance != null)
        {
            TrailRendererLeft.instance.ChangeColor();
        }
        
        Debug.Log("Max Power Ended - Restored all materials to original");
    }



    
}