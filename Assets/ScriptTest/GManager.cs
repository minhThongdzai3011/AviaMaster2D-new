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

    [Header("Tính toán nâng cấp")]
    public float baseDurationFuel = 2f; // Thời gian cơ bản không thay đổi
    public float baseLaunchForce = 20f; // Lực cơ bản không thay đổi
    public float baseTotalBoost = 100f; // Boost cơ bản không thay đổi

    [Header("RotationZ settings")]
    public float maxUpAngle = 45f;    // giới hạn góc lên
    public float maxDownAngle = -45f; // giới hạn góc xuống
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

    [Header("Thành tích")]
    public Slider sliderAchievement;

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
    public float rotationForce = 100f;        // tốc độ xoay ban đầu
    public float angularFriction = 5f;        // hệ số ma sát góc
    public float currentRotationSpeed = 0f;  // tốc độ xoay hiện tại

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
        
        buttonDownImage.color = Color.gray;
        buttonUpImage.color = Color.gray;
        buttonBoosterPlaneImage.color = Color.gray;

        sliderAchievement.value = 0f;
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
        
        SaveTotalMoney();
        SaveTotalDiamond();
        totalBoostMaxPower = totalBoost;
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

    IEnumerator LaunchSequence()
    {
        yield return new WaitForSeconds(0.5f);
        arrowAngleZ.SetActive(false);
        rotationXObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        
        // THÊM: Set flag bay ngang
        isHorizontalFlying = true;
        
        // Bước 1: Di chuyển ngang một đoạn r
        float r = 10f;
        float horizontalSpeed = 10f;
        float targetX = airplaneRigidbody2D.position.x + r;

        // Thay thế đoạn code trong LaunchSequence():
        Debug.Log("Preparing to start horizontal flight..." + airplaneRigidbody2D.name);
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
                
                if (propellers.Length > 0)
                {
                    AudioManager.instance.PlayPlayerSound(AudioManager.instance.takeOffSoundClip);
                }
            }
        }
        

        airplaneRigidbody2D.gravityScale = 0f;
        airplaneRigidbody2D.velocity = new Vector2(horizontalSpeed, 0f);

        // Giữ góc 0 khi bay ngang
        airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Debug.Log($"Bắt đầu bay ngang - Target: {targetX}, Current: {airplaneRigidbody2D.position.x}");

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

            // --- 3. Giữ vận tốc ngang ---
            airplaneRigidbody2D.velocity = new Vector2(horizontalSpeed, 0f);

            yield return null;
        }

        
        Debug.Log($"Hoàn thành bay ngang - Distance: {airplaneRigidbody2D.position.x - (targetX - r):F1}f");
        
        // SỬA: Set false SAU KHI hoàn thành bay ngang
        isHorizontalFlying = false;

        // Bước 2: Bay lên với rotation dần dần
        AudioManager.instance.PlayPlayerSound(AudioManager.instance.gameplaySoundClip);
        float climbForce = launchForce;
        float angleRad = Mathf.Deg2Rad * climbAngle;
        Vector2 launchDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;

        // ✅ Không reset velocity để giữ momentum từ Bước 1 (bay ngang)
        // airplaneRigidbody2D.velocity = Vector2.zero;
        airplaneRigidbody2D.AddForce(launchDirection * climbForce, ForceMode2D.Impulse);

        // KHÔNG xoay ngay lập tức - sẽ xoay dần dần trong bước 3
        // airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, climbAngle);

        boostedAltitude = launchForce / 2.5f;

        // Bước 3: Chờ đến khi đạt độ cao và xoay dần dần
        float targetAltitude = startPosition.y + boostedAltitude;
        float startAltitude = airplaneRigidbody2D.position.y;
        
        // ✅ Lấy góc HIỆN TẠI từ Bước 1 (thay vì giả định 0°)
        float currentZ = airplaneRigidbody2D.transform.eulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f; // Chuyển từ 0-360 về -180 đến 180
        float startRotation = currentZ; // Góc hiện tại (~20° từ Bước 1)
        float targetRotation = climbAngle; // Mục tiêu là climbAngle (30°)

        Debug.Log($"Bắt đầu bay lên - Start: {startAltitude:F1}, Target: {targetAltitude:F1}, Rotation: {startRotation:F1}° → {targetRotation}°");

        while (airplaneRigidbody2D.position.y < targetAltitude)
        {
            // Tính toán tiến độ bay lên (0.0 đến 1.0)
            float currentAltitude = airplaneRigidbody2D.position.y;
            float altitudeProgress = (currentAltitude - startAltitude) / (targetAltitude - startAltitude);
            altitudeProgress = Mathf.Clamp01(altitudeProgress);

            // Xoay dần dần theo tiến độ bay lên

            float easedProgress = Mathf.SmoothStep(0f, 2f, altitudeProgress);
            float currentRotation = Mathf.Lerp(startRotation, targetRotation, easedProgress);
            // float currentRotation = Mathf.Lerp(startRotation, targetRotation, altitudeProgress);
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);

            

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
        Debug.Log($"Máy bay bắt đầu điều khiển với durationFuel = {durationFuel}s (rateFuel = {rateFuel}%)");
        if (PositionX.instance.isMaxPower)
        {
            durationFuel += 5f;
            Debug.Log($"Max Power active! durationFuel increased by 5s to {durationFuel}s");
        }
        StartCoroutine(DecreaseSliderFuel(durationFuel));
        buttonDownImage.color = Color.white;
        buttonUpImage.color = Color.white;
        // Trong giai đoạn này, rotation được điều khiển bởi HandleAircraftControl()
        float timer = 0f;
        while (timer < durationFuel)
        {
            buttonDownImage.color = Color.white;
            buttonUpImage.color = Color.white;
            timer += Time.deltaTime;
            if (Mathf.FloorToInt(timer) != Mathf.FloorToInt(timer - Time.deltaTime))
            {
                Debug.Log($"Thời gian điều khiển: {Mathf.FloorToInt(timer)}s / {durationFuel}s");
            }

            if (Input.GetKey(KeyCode.A))
            {
                PlainUp();
                buttonDownImage.color = Color.white;
                buttonUpImage.color = Color.gray;

            }
            else if (Input.GetKey(KeyCode.D))
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
            AudioManager.instance.PlaySound(AudioManager.instance.fallingSoundClip);
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
                Debug.Log($"Normal Power! minVelocityX set to {minVelocityX} m/s");
            }
            if (minVelocityX < 15f && !Plane.instance.isGrounded) minVelocityX = 15f; // Đảm bảo tối thiểu 15 m/s
            if (currentVel.x < minVelocityX && !Plane.instance.isGrounded)
            {
                currentVel.x = minVelocityX;
                airplaneRigidbody2D.velocity = currentVel;
                if (debugCounter % 60 == 0)
                {
                    Debug.Log($"Duy trì velocity.x tối thiểu 8 m/s: {currentVel}");
                }
            }
            
            // Kiểm tra input A/D để điều khiển góc rơi
            bool hasPlayerInput = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
            
            if (hasPlayerInput)
            {
                float targetAngle = currentZ0;
                float controlSpeed = 80f; // Tốc độ điều khiển (độ/giây)
                
                if (Input.GetKey(KeyCode.A))
                {
                    // A: Xoay lên (ít nghiêng xuống) - hướng về -10°
                    targetAngle = Mathf.MoveTowards(currentZ0, -10f, controlSpeed * Time.deltaTime);
                    buttonDownImage.color = Color.white;
                    buttonUpImage.color = Color.gray;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    // D: Xoay xuống (nghiêng xuống nhiều) - hướng về -35°
                    targetAngle = Mathf.MoveTowards(currentZ0, -35f, controlSpeed * Time.deltaTime);
                    buttonDownImage.color = Color.gray;
                    buttonUpImage.color = Color.white;
                }
                
                // Giới hạn góc trong khoảng cho phép
                targetAngle = Mathf.Clamp(targetAngle, -35f, -10f);
                
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
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !isHoldingButtonUp && !isHoldingButtonDown && isControllable && !isHorizontalFlying)
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
        if (Input.GetKey(KeyCode.Space) && totalBoost > 0) // ✅ THÊM KIỂM TRA totalBoost > 0
        {
            Debug.Log("Space Key Down Detected - Boost available: " + totalBoost);
            StartBoostDecrease();
            
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
        else if (Input.GetKey(KeyCode.Space) && totalBoost <= 0)
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
            totalMoney += 1000;
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


        if (distanceText != null) distanceText.text = distanceTraveled.ToString("F0") + " ft";
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
        bool isUpPressed = Input.GetKey(KeyCode.A) || isHoldingButtonUp;
        bool isDownPressed = Input.GetKey(KeyCode.D) || isHoldingButtonDown;

        // Điều khiển lên (A key HOẶC button Up) - CHỈ ĐIỀU CHỈNH, KHÔNG GHI ĐÈ auto-rotation
        if (isUpPressed)
        {
            // Điều chỉnh góc lên từ góc hiện tại
            float adjustment = Time.deltaTime * 360f;
            float targetRotation = Mathf.Min(currentZ + adjustment, maxUpAngle);
            
            // GIỮ NGUYÊN rotation.x hiện tại khi set rotation.z
            Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, targetRotation);

            // Nếu đang boost thì thêm lực theo hướng mới
            if (isBoosterActive)
            {
                float angleRad = targetRotation * Mathf.Deg2Rad;
                Vector2 forceDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
                airplaneRigidbody2D.AddForce(forceDirection * controlForce * 0.4f, ForceMode2D.Force);
            }
        }

        // Điều khiển xuống (D key HOẶC button Down) - CHỈ ĐIỀU CHỈNH, KHÔNG GHI ĐÈ auto-rotation
        else if (isDownPressed)
        {
            // Điều chỉnh góc xuống từ góc hiện tại
            float adjustment = Time.deltaTime * 180f;
            float targetRotation = Mathf.Max(currentZ - adjustment, maxDownAngle);
            
            // GIỮ NGUYÊN rotation.x hiện tại khi set rotation.z
            Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, targetRotation);

            // Nếu đang boost thì thêm lực theo hướng mới
            if (isBoosterActive)
            {
                float angleRad = targetRotation * Mathf.Deg2Rad;
                Vector2 forceDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
                airplaneRigidbody2D.AddForce(forceDirection * controlForce * 0.3f, ForceMode2D.Force);
            }
        }
    }
    
    float ComputeUpTargetAngle(Vector2 velocity)
    {
        // Tính góc từ vector vận tốc, nhưng ưu tiên cảm giác "bay lên"
        // Bạn có thể điều chỉnh upBias để làm góc lên nhạy hơn so với Atan2 thuần túy
        float upBias = 0.1f; // >1 làm tăng ảnh hưởng của vy
        float velAngle = Mathf.Atan2(velocity.y * upBias, velocity.x) * Mathf.Rad2Deg;
        return Mathf.Clamp(velAngle, 0f, maxUpAngle);
    }

    float ComputeDownTargetAngle(Vector2 velocity)
    {
        // Tính góc từ vector vận tốc, nhưng ưu tiên cảm giác "bay xuống"
        // downBias có thể >1 để nghiêng xuống mạnh hơn
        float downBias = 1f; // >1 làm tăng ảnh hưởng của vy (âm)
        float velAngle = Mathf.Atan2(velocity.y * downBias, velocity.x) * Mathf.Rad2Deg;
        return Mathf.Clamp(velAngle, maxDownAngle, 0f);
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
                Debug.Log($"BoosterUp - AngleZ: {currentAngleZ:F2}°, PowerMult: {actualPowerMultiplier:F2}, AltEff: {altitudeEfficiency:F2}, BoostForce: {boostForce:F2}");
                airplaneRigidbody2D.AddForce(boostDirection * boostForce, ForceMode2D.Force);
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
        PlayerPrefs.Save();
        isVelocity = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        
            SaveTotalMoney();
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
        float newTargetRotation = Mathf.Min(currentZ + Time.deltaTime * 1f, maxUpAngle);
        
        Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
        airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, newTargetRotation);

        Vector2 currentVelocity = airplaneRigidbody2D.velocity;
        float currentSpeed = currentVelocity.magnitude;
        float targetVerticalSpeed = Mathf.Sin(newTargetRotation * Mathf.Deg2Rad) * maxVerticalSpeed;

        currentVelocity.y = Mathf.Lerp(currentVelocity.y, targetVerticalSpeed, Time.deltaTime * 3f);

        if (currentSpeed > maxBoostSpeed)
        {
            float targetSpeed = Mathf.Lerp(currentSpeed, maxBoostSpeed, Time.deltaTime * 2f);
            currentVelocity = currentVelocity.normalized * targetSpeed;
        }
        else if (currentVelocity.magnitude > maxVerticalSpeed * 2f)
        {
            currentVelocity = currentVelocity.normalized * maxVerticalSpeed * 2f;
        }

        airplaneRigidbody2D.velocity = currentVelocity;
    }

    public void PlainDown()
    {
        float currentZ = airplaneRigidbody2D.transform.eulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;
        float newTargetRotation = Mathf.Max(currentZ - Time.deltaTime * 1f, maxDownAngle);
        
        Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
        airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, newTargetRotation);

        Vector2 currentVelocity = airplaneRigidbody2D.velocity;
        float currentSpeed = currentVelocity.magnitude;
        float targetVerticalSpeed = Mathf.Sin(newTargetRotation * Mathf.Deg2Rad) * maxVerticalSpeed;

        currentVelocity.y = Mathf.Lerp(currentVelocity.y, targetVerticalSpeed, Time.deltaTime * 3f);

        if (currentSpeed > maxBoostSpeed)
        {
            float targetSpeed = Mathf.Lerp(currentSpeed, maxBoostSpeed, Time.deltaTime * 2f);
            currentVelocity = currentVelocity.normalized * targetSpeed;
        }
        else if (currentVelocity.magnitude > maxVerticalSpeed * 2f)
        {
            currentVelocity = currentVelocity.normalized * maxVerticalSpeed * 2f;
        }

        airplaneRigidbody2D.velocity = currentVelocity;
    }

    public void ResetGame()
    {
        Debug.Log("Reset Game");
    }

    public IEnumerator DecreaseSliderFuel(float durationFuel)
    {
        float timer = 0f;
        float startValue = sliderFuel.value;
        float endValue = 0f;
        if (Plane.instance.isAddFuel){
            durationFuel += 5f;
        }
        Plane.instance.isAddFuel = false;

        while (timer < durationFuel)
        {
            timer += Time.deltaTime;

            sliderFuel.value = Mathf.Lerp(startValue, endValue, timer / durationFuel);

            yield return null;
        }


        sliderFuel.value = endValue;
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
    public float moneyPower = 20f;
    public float moneyFuel = 20f;
    public float moneyBoost = 20f;
    public bool isFuelMax = false;
    public bool isPowerMax = false;
    public bool isBoostMax = false;

    public void UpgradeFuel()
    {
        if (!isFuelMax && totalMoney >= moneyFuel)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.upgradeSoundClip);
            levelFuel += 1;
            rateFuel += 5;
            totalMoney -= moneyFuel;
            moneyFuel = moneyFuel * 1.25f;
            totalMoney = (int)Math.Round(totalMoney, 0);
            totalMoneyText.text = totalMoney.ToString("F0");
            PlayerPrefs.SetFloat("TotalMoney", totalMoney);
            PlayerPrefs.Save();
            moneyFuel = (int)Math.Round(moneyFuel, 1);
            if (moneyFuel > 999)
            {
                if (moneyFuel >= 1000 && moneyFuel < 1000000)
                {
                    fuelMoneyText.text = (moneyFuel / 1000f).ToString("F1") + "K";
                }
                else if (moneyFuel >= 1000000 && moneyFuel < 1000000000)
                {
                    fuelMoneyText.fontSize = 28;
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
            moneyBoost = moneyBoost * 1.25f;
            totalMoney = (int)Math.Round(totalMoney, 0);
            totalMoneyText.text = totalMoney.ToString("F0");
            PlayerPrefs.SetFloat("TotalMoney", totalMoney);
            PlayerPrefs.Save();
            moneyBoost = (int)Math.Round(moneyBoost, 1);
            if (moneyBoost > 999)
            {

                if (moneyBoost >= 1000 && moneyBoost < 1000000)
                {
                    boostMoneyText.text = (moneyBoost / 1000f).ToString("F1") + "K";
                }
                else if (moneyBoost >= 1000000 && moneyBoost < 1000000000)
                {
                    boostMoneyText.fontSize = 28;
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
            moneyPower = moneyPower * 1.25f;
            totalMoney = (int)Math.Round(totalMoney, 0);
            totalMoneyText.text = totalMoney.ToString("F0");
            PlayerPrefs.SetFloat("TotalMoney", totalMoney);
            PlayerPrefs.Save();
            moneyPower = (int)Math.Round(moneyPower, 1);
            if (moneyPower > 999)
            {

                if (moneyPower >= 1000 && moneyPower < 1000000)
                {
                    powerMoneyText.text = (moneyPower / 1000f).ToString("F1") + "K";
                }
                else if (moneyPower >= 1000000 && moneyPower < 1000000000)
                {
                    powerMoneyText.fontSize = 28;
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
        }
    }

    public void LoadMoneyUpgrade()
    {
        if (moneyPower > 999)
        {

            if (moneyPower >= 1000 && moneyPower < 1000000)
            {
                powerMoneyText.text = (moneyPower / 1000f).ToString("F1") + "K";
            }
            else if (moneyPower >= 1000000 && moneyPower < 1000000000)
            {
                powerMoneyText.fontSize = 28;
                powerMoneyText.text = (moneyPower / 1000000f).ToString("F1") + "M";
            }
        }
        if (moneyBoost > 999)
        {

            if (moneyBoost >= 1000 && moneyBoost < 1000000)
            {
                boostMoneyText.text = (moneyBoost / 1000f).ToString("F1") + "K";
            }
            else if (moneyBoost >= 1000000 && moneyBoost < 1000000000)
            {
                boostMoneyText.fontSize = 28;
                boostMoneyText.text = (moneyBoost / 1000000f).ToString("F1") + "M";
            }
        }
        if (moneyFuel > 999)
        {
            if (moneyFuel >= 1000 && moneyFuel < 1000000)
            {
                fuelMoneyText.text = (moneyFuel / 1000f).ToString("F1") + "K";
            }
            else if (moneyFuel >= 1000000 && moneyFuel < 1000000000)
            {
                fuelMoneyText.fontSize = 28;
                fuelMoneyText.text = (moneyFuel / 1000000f).ToString("F1") + "M";
            }
        }
        else
        {
            powerMoneyText.text = "" + moneyPower;
            boostMoneyText.text = "" + moneyBoost;
            fuelMoneyText.text = "" + moneyFuel;
        }
        
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
            sliderAchievement.value = Mathf.Lerp(sliderAchievement.value, targetValue, smoothSpeed * Time.deltaTime);
            if(sliderAchievement.value >= 0.75f)
            {
                sliderAchievement.value = 0.75f;
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
        sliderAchievement.value = 0f; 
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

        moneyPower = PlayerPrefs.GetFloat("MoneyPower", 20f);
        moneyFuel = PlayerPrefs.GetFloat("MoneyFuel", 20f);
        moneyBoost = PlayerPrefs.GetFloat("MoneyBoost", 20f);

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
                totalMoneyText.fontSize = 28;
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
                totalDiamondText.fontSize = 28;
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
                });
        }
    }
    public IEnumerator AircraftPlayUp()
    {
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

    



    
}