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
    public float climbAngle = 30f;  // Góc bay lên (độ) - có thể chỉnh trong Inspector
    public float gravityScale = 1f; // Thang trọng lực chung
    public float boostedAltitude;

    [Header("Giới hạn độ cao")]
    public float maxAltitude = 1200f; // Độ cao tối đa
    public float maxPowerPercent = 300f; // Power tối đa (300% = 3x lực ban đầu)
    public float altitudeDragMultiplier = 2f; // Hệ số lực kéo xuống theo độ cao
    public float altitudeDragStart = 600f; // Độ cao bắt đầu có lực kéo mạnh
    public float velocityDampingFactor = 0.98f; // Hệ số giảm vận tốc theo độ cao


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
    public int totalMoney = 0;
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
    public float rotationSmooth = 8f; // độ mượt khi quay
    public float minMoveThreshold = 0.5f; // ngưỡng vận tốc để bắt đầu quay theo velocity

    public GameObject arrowAngleZ;
    public GameObject rotationXObject;

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

    [Header("UI Texts")]
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI altitudeText;
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

    void Awake()
    {

        instance = this;
        currentRotationSpeed = rotationForce;

    }

    void Start()
    {
        
        sliderAchievement.value = 0f;
        startZ = airplaneRigidbody2D.transform.rotation.eulerAngles.z;

        // THÊM: Load dữ liệu nâng cấp TRƯỚC khi tính toán
        LoadUpgradeData();

        // Tính toán lại các giá trị nâng cấp dựa trên rate đã load
        CalculateUpgradeValues();

        // Cập nhật UI
        SaveTotalMoney();
        SaveTotalDiamond();
        Debug.Log("money , totalMoney at Start: " + money + ", " + totalMoney);
        if (airplaneRigidbody2D != null)
        {
            startPosition = airplaneRigidbody2D.transform.position;
            airplaneRigidbody2D.velocity = Vector2.zero;
            airplaneRigidbody2D.angularVelocity = 0f;
            airplaneRigidbody2D.gravityScale = gravityScale;

            airplaneRigidbody2D.freezeRotation = true;

        
        
            // THÊM: Tăng drag tự nhiên của Rigidbody2D thay vì dùng LimitAirspeed()
            airplaneRigidbody2D.drag = 0.2f; // Drag tự nhiên
            airplaneRigidbody2D.angularDrag = 0.1f; // Angular drag
    
        }
        Physics2D.gravity = new Vector2(0f, -9.81f * gravityScale);
    }

    public void LaunchAirplane()
    {
        if (isPlaying)
        {
            if (airplaneRigidbody2D == null)
            {
                Debug.LogError("Rigidbody2D chưa được gán!");
                return;
            }
            // homeImage.gameObject.SetActive(false);
            playImage.gameObject.SetActive(true);

            StartCoroutine(LaunchSequence());
            isPlaying = false;
            isPlay = true;
            isHomeDown = true;
            isHomeUp = true;
            homeGameDown();
            homeGameUp();

            isRotationOscillating = false;
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
        RotaryFront.instance.StartRotation();

        airplaneRigidbody2D.gravityScale = 0f;
        airplaneRigidbody2D.velocity = new Vector2(horizontalSpeed, 0f);

        // Giữ góc 0 khi bay ngang
        airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Debug.Log($"Bắt đầu bay ngang - Target: {targetX}, Current: {airplaneRigidbody2D.position.x}");

        while (airplaneRigidbody2D.position.x < targetX)
        {
            // THÊM: Đảm bảo máy bay bay ngang
            airplaneRigidbody2D.velocity = new Vector2(horizontalSpeed, 0f);
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            yield return null;
        }
        
        Debug.Log($"Hoàn thành bay ngang - Distance: {airplaneRigidbody2D.position.x - (targetX - r):F1}f");
        
        // SỬA: Set false SAU KHI hoàn thành bay ngang
        isHorizontalFlying = false;

        // Bước 2: Bay lên với rotation dần dần
        float climbForce = launchForce;
        float angleRad = Mathf.Deg2Rad * climbAngle;
        Vector2 launchDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;

        airplaneRigidbody2D.velocity = Vector2.zero;
        airplaneRigidbody2D.AddForce(launchDirection * climbForce, ForceMode2D.Impulse);

        // KHÔNG xoay ngay lập tức - sẽ xoay dần dần trong bước 3
        // airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, climbAngle);

        boostedAltitude = launchForce / 2.5f;

        // Bước 3: Chờ đến khi đạt độ cao và xoay dần dần
        float targetAltitude = startPosition.y + boostedAltitude;
        float startAltitude = airplaneRigidbody2D.position.y;
        float startRotation = 0f; // Bắt đầu từ góc 0
        float targetRotation = climbAngle; // Mục tiêu là climbAngle

        Debug.Log($"Bắt đầu bay lên - Start: {startAltitude:F1}, Target: {targetAltitude:F1}, Rotation: {startRotation}° → {targetRotation}°");

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
        Debug.Log($"Hoàn thành xoay về 0° - Máy bay đã ổn định");

        // Bước 4: Giữ độ cao và cho phép điều khiển

        // Sửa lại: Trả lại drag tự nhiên của Rigidbody2D
        airplaneRigidbody2D.drag = 0f; // Drag ban đầu
        airplaneRigidbody2D.angularDrag = 0.05f; // Angular drag ban đầu
            
        airplaneRigidbody2D.gravityScale = 0.2f;
        airplaneRigidbody2D.velocity = new Vector2(airplaneRigidbody2D.velocity.x, 0f);

        isControllable = true;
        Debug.Log($"Máy bay bắt đầu điều khiển với durationFuel = {durationFuel}s (rateFuel = {rateFuel}%)");
        StartCoroutine(DecreaseSliderFuel(durationFuel));

        // Trong giai đoạn này, rotation được điều khiển bởi HandleAircraftControl()
        float timer = 0f;
        while (timer < durationFuel)
        {
            timer += Time.deltaTime;

            // Log mỗi giây để không spam console
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

        // Bước 5: Kết thúc điều khiển và rơi
        Plane.instance.trailEffect.enabled = false; // Dừng vệt khói khi rơi
        isControllable = false;
        airplaneRigidbody2D.gravityScale = gravityScale;
        Physics2D.gravity = new Vector2(0f, -9.81f * gravityScale);

        Plane.instance.smokeEffect.Play();

        // Trong giai đoạn rơi, tự động xoay theo velocity
        while (airplaneRigidbody2D.velocity.x > 0.1f)
        {
            Vector2 vel = airplaneRigidbody2D.velocity;
            if (vel.magnitude > 1f)
            {
                float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
                angle = Mathf.Clamp(angle, -45f, 45f);

                float currentZ = airplaneRigidbody2D.transform.eulerAngles.z;
                if (currentZ > 180f) currentZ -= 360f;

                float targetAngle = Mathf.LerpAngle(currentZ, angle, Time.deltaTime * 2f);
                
                // GIỮ NGUYÊN rotation.x hiện tại khi set rotation.z
                Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
                airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, targetAngle);
            }
            yield return null;
        }

        Debug.Log("Máy bay đã hoàn thành chuỗi bay: ngang → bay lên (xoay dần) → giữ (có điều khiển) → rơi");
    }

    public bool isVelocity = true;
    public bool isHorizontalFlying = false;
    public bool isSlidingOnGround = false; // THÊM: Kiểm tra trượt trên đất
    void Update()
    {
        
        if (airplaneRigidbody2D == null) return;
        Vector2 currentPos = airplaneRigidbody2D.transform.position;
        distanceTraveled = Vector2.Distance(startPosition, currentPos);
        currentAltitude = currentPos.y - startPosition.y;
        if (currentAltitude < 0f) currentAltitude = 0f;
        rotationZ = airplaneRigidbody2D.transform.eulerAngles.z;
        if (rotationZ > 180f) rotationZ -= 360f;

        rotationAngleZ();
        UpdateSliderAchievement();

        // Áp dụng lực kéo xuống theo độ cao
        ApplyAltitudeDrag();

        // THÊM: Cập nhật rotation X cho máy bay
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
            HandleAircraftControl(); // Đã bao gồm auto-rotation theo velocity
            if (isHoldingButtonUp)
            {
                PlainUp();
            }
            if (isHoldingButtonDown)
            {
                PlainDown();
            }
        }
        else
        {
            // Kiểm tra xem có đang bay không
            bool isAirborne = isPlay && (airplaneRigidbody2D.velocity.magnitude > 1f || currentAltitude > 5f);
            
            // Kiểm tra xem có đang trượt trên đất không
            isSlidingOnGround = isPlay && !isAirborne && airplaneRigidbody2D.velocity.magnitude > 0.2f;
            
            if (isAirborne)
            {
                ApplyAutoRotationFromVelocity(); // Tự động xoay khi bay nhưng không controllable
            }
            else if (isSlidingOnGround)
            {
                // Khi đang trượt trên đất, từ từ xoay về góc 0 (nằm phẳng)
                Vector3 currentRotation = airplaneRigidbody2D.transform.eulerAngles;
                float currentZ = currentRotation.z;
                if (currentZ > 180f) currentZ -= 360f;
                
                if (Mathf.Abs(currentZ) > 0.1f)
                {
                    float targetZ = Mathf.Lerp(currentZ, 0f, Time.deltaTime * 1.5f); // Chậm hơn để tự nhiên
                    airplaneRigidbody2D.transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, targetZ);
                }
            }
            else
            {
                // Hoàn toàn dừng - reset rotation về 0
                Vector3 currentRotation = airplaneRigidbody2D.transform.eulerAngles;
                float currentZ = currentRotation.z;
                if (currentZ > 180f) currentZ -= 360f;
                
                if (Mathf.Abs(currentZ) > 0.1f)
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

        if (isPlay)
        {
            bool isBoostPressed = Input.GetKey(KeyCode.Space) || isUseClickerBooster;
            
            // Xử lý keyboard Space - chỉ cho start/stop decrease
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartBoostDecrease();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                StopBoostDecrease();
            }
            
            // Xử lý boost và màu button
            if (isBoostPressed)
            {
                BoosterUp();
                buttonBoosterPlaneImage.color = Color.gray;
            }
            else
            {
                // Dừng boost khi thả button/key
                isBoosterActive = false;
                if (isBoostDecreasing && !Input.GetKey(KeyCode.Space) && !isUseClickerBooster)
                {
                    StopBoostDecrease();
                }
                buttonBoosterPlaneImage.color = Color.white;
            }
        }


        

        // if (!isHorizontalFlying && isPlay)
        // {
        //     if (Input.GetKey(KeyCode.A))
        //     {
        //         PlainUp();
        //     }
        //     if (Input.GetKey(KeyCode.D) && !isControllable && !isBoosterActive)
        //     {
        //         PlainDown();
        //     }
        // }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            PauseGame();
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            AgainGame();
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


        moneyText.text = money.ToString() + " $";

    }


    // void FixedUpdate()
    // {

    //     if(isTurnWheel){
    //         if (Mathf.Abs(currentRotationSpeed) > 0.1f)
    //         {
    //             currentRotationSpeed -= Mathf.Sign(currentRotationSpeed) * angularFriction * Time.fixedDeltaTime;
    //         }
    //         else
    //         {
    //             currentRotationSpeed = 0f;
    //         }

    //         // Tính góc xoay mới
    //         float newRotation = wheelRigidbody2D.rotation + currentRotationSpeed * Time.fixedDeltaTime;
    //         wheelRigidbody2D.MoveRotation(newRotation);
    //     }
    // }
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
            float adjustment = Time.deltaTime * 120f;
            float targetRotation = Mathf.Min(currentZ + adjustment, maxUpAngle);
            
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

        // Điều khiển xuống (D key HOẶC button Down) - CHỈ ĐIỀU CHỈNH, KHÔNG GHI ĐÈ auto-rotation
        else if (isDownPressed)
        {
            // Điều chỉnh góc xuống từ góc hiện tại
            float adjustment = Time.deltaTime * 120f;
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
        
        // KHÔNG CÓN logic "về góc 0" - để auto-rotation xử lý
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
        
        // CẢI THIỆN điều kiện để nhận biết máy bay chạm đất
        bool isOnGround = false;
        
        // Kiểm tra máy bay có đang chạm đất không - dựa vào altitude và velocity
        if (currentAltitude <= 3f && airplaneRigidbody2D.velocity.magnitude < 8f)
        {
            isOnGround = true;
        }
        
        // Điều kiện máy bay đang bay (không chạm đất)
        bool isAirborne = isPlay && !isOnGround && (isControllable || 
                                   (!isControllable && airplaneRigidbody2D.velocity.magnitude > 1f) ||
                                   currentAltitude > 5f);
        
        if (!isAirborne || isOnGround) 
        {
            // Khi trên đất hoặc chạm đất, từ từ reset rotation X về 0
            if (Mathf.Abs(currentAirplaneRotationX) > 0.1f)
            {
                // Tăng tốc độ reset khi chạm đất để có cảm giác tự nhiên
                float resetSpeed = isOnGround ? 4f : 2.5f;
                currentAirplaneRotationX = Mathf.Lerp(currentAirplaneRotationX, 0f, Time.deltaTime * resetSpeed);
                Vector3 groundRotation = airplaneRigidbody2D.transform.eulerAngles;
                airplaneRigidbody2D.transform.rotation = Quaternion.Euler(currentAirplaneRotationX, groundRotation.y, groundRotation.z);
                
            }
            else
            {
                // Đảm bảo rotation.x = 0 khi đã reset xong
                currentAirplaneRotationX = 0f;
                Vector3 groundRotation = airplaneRigidbody2D.transform.eulerAngles;
                airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, groundRotation.y, groundRotation.z);
            }
            return;
        }
        
        // Tính toán target rotation dựa trên sin wave
        float time = Time.time * airplaneRotationXSpeed;
        targetAirplaneRotationX = Mathf.Sin(time) * airplaneRotationXAmplitude;
        
        // ĐƠN GIẢN HÓA: Bỏ curve, dùng sin wave trực tiếp
        // Lerp từ current đến target để có chuyển động mượt mà hơn
        currentAirplaneRotationX = Mathf.Lerp(currentAirplaneRotationX, targetAirplaneRotationX, Time.deltaTime * 5f);
        
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

                float boostForce = 15f * actualPowerMultiplier * altitudeEfficiency;
                airplaneRigidbody2D.AddForce(boostDirection * boostForce, ForceMode2D.Force);
            }
        }
        else
        {
            // Nếu hết boost thì tắt booster
            isBoosterActive = false;
            StopBoostDecrease();
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

    public void AgainGame()
    {
        money = 0;
        isVelocity = true;
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

    public void PlainUp()
    {
        float currentZ = airplaneRigidbody2D.transform.eulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;
        float newTargetRotation = Mathf.Min(currentZ + Time.deltaTime * 1f, maxUpAngle);
        
        // GIỮ NGUYÊN rotation.x hiện tại khi set rotation.z
        Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
        airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, newTargetRotation);

        // ĐIỀU CHỈNH VELOCITY thay vì cộng dồn lực
        Vector2 currentVelocity = airplaneRigidbody2D.velocity;
        float targetVerticalSpeed = Mathf.Sin(newTargetRotation * Mathf.Deg2Rad) * maxVerticalSpeed;

        // Lerp velocity để có chuyển động mượt mà
        currentVelocity.y = Mathf.Lerp(currentVelocity.y, targetVerticalSpeed, Time.deltaTime * 3f);

        // Giới hạn tốc độ tổng thể
        if (currentVelocity.magnitude > maxVerticalSpeed * 2f)
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
        
        // GIỮ NGUYÊN rotation.x hiện tại khi set rotation.z
        Vector3 existingRotation = airplaneRigidbody2D.transform.eulerAngles;
        airplaneRigidbody2D.transform.rotation = Quaternion.Euler(existingRotation.x, existingRotation.y, newTargetRotation);

        // ĐIỀU CHỈNH VELOCITY thay vì cộng dồn lực
        Vector2 currentVelocity = airplaneRigidbody2D.velocity;
        float targetVerticalSpeed = Mathf.Sin(newTargetRotation * Mathf.Deg2Rad) * maxVerticalSpeed;

        // Lerp velocity để có chuyển động mượt mà
        currentVelocity.y = Mathf.Lerp(currentVelocity.y, targetVerticalSpeed, Time.deltaTime * 3f);

        // Giới hạn tốc độ tổng thể
        if (currentVelocity.magnitude > maxVerticalSpeed * 2f)
        {
            currentVelocity = currentVelocity.normalized * maxVerticalSpeed * 2f;
        }

        airplaneRigidbody2D.velocity = currentVelocity;
    }

    public void ResetGame()
    {
        Debug.Log("Reset Game");
    }

    // public void SettingGame()
    // {
    //     Settings.instance.settingsImage.gameObject.SetActive(true);
    // }

    // Giảm fuel theo thời gian
    private IEnumerator DecreaseSliderFuel(float durationFuel)
    {
        float timer = 0f;
        float startValue = sliderFuel.value;
        float endValue = 0f;

        while (timer < durationFuel)
        {
            timer += Time.deltaTime;

            // Tính toán giá trị mới cho slider một cách mượt mà
            sliderFuel.value = Mathf.Lerp(startValue, endValue, timer / durationFuel);

            // Đợi đến khung hình tiếp theo
            yield return null;
        }


        sliderFuel.value = endValue;
    }


    // Giảm boost theo thời gian
    private IEnumerator DecreaseSliderBoost()
    {
        while (isBoostDecreasing && totalBoost > 0)
        {
            // Giảm boost theo thời gian thực
            float decreaseAmount = boostDecreaseRate * Time.deltaTime;
            totalBoost -= decreaseAmount;

            // Đảm bảo không giảm xuống dưới 0
            if (totalBoost < 0)
            {
                totalBoost = 0;
            }

            // Cập nhật slider (giả sử slider có maxValue = 100)
            sliderBooster.value = totalBoost / 100f;

            // Cập nhật UI text nếu có
            if (levelBoostText != null)
            {
                levelBoostText.text = "Lv" + levelBoost + " | " + totalBoost.ToString("F0");
            }

            // Nếu boost = 0 thì dừng
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

    public void UpgradeFuel()
    {
        if (!isFuelMax)
        {
            levelFuel += 1;
            rateFuel += 5;

            if (levelFuel >= 60)
            {
                fuelMoneyText.text = "MAX";
                fuelMoneyText.color = Color.yellow;
                isFuelMax = true;
            }
            moneyFuel = moneyFuel * 1.34f;
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

            // CÔNG THỨC MỚI: Tăng durationFuel theo rateFuel
            // Mỗi lần nâng cấp thêm 5% -> rateFuel tăng 5%
            // durationFuel = baseDuration * (1 + rateFuel/100)
            CalculateUpgradeValues(); // Tính toán lại tất cả giá trị

            // THÊM: Lưu ngay sau khi nâng cấp
            SaveUpgradeData();

            levelFuelText.text = "Character is " + rateFuel + " % fuel";
            Debug.Log($"UpgradeFuel: Level {levelFuel}, Rate {rateFuel}%, Duration {durationFuel}s");
        }
    }

    // Hàm tính toán lại tất cả giá trị nâng cấp
    void CalculateUpgradeValues()
    {
        // Tính durationFuel dựa trên rateFuel
        durationFuel = baseDurationFuel * (1 + (rateFuel / 100f));
        durationFuel = (float)Math.Round(durationFuel, 1);

        // Tính launchForce dựa trên ratePower (cho consistency)
        launchForce = baseLaunchForce * (1 + (ratePower / 100f));
        launchForce = (float)Math.Round(launchForce, 1);

        // Tính totalBoost dựa trên rateBoost (cho consistency)
        totalBoost = baseTotalBoost * (1 + (rateBoost / 100f));
        totalBoost = (float)Math.Round(totalBoost, 1);

        Debug.Log($"Calculated values - Fuel: {durationFuel}s, Power: {launchForce}, Boost: {totalBoost}");
    }

    public void UpgradeBoost()
    {
        if (!isBoostMax)
        {
            levelBoost += 1;
            rateBoost += 5;
            if (levelBoost >= 60)
            {
                boostMoneyText.text = "MAX";
                boostMoneyText.color = Color.yellow;
                isBoostMax = true;
            }
            moneyBoost = moneyBoost * 1.34f;
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

            // Sử dụng hàm tính toán chung
            CalculateUpgradeValues();

            // THÊM: Lưu ngay sau khi nâng cấp
            SaveUpgradeData();

            levelBoostText.text = "Character is " + rateBoost + " % boost";
            Debug.Log($"UpgradeBoost: Level {levelBoost}, Rate {rateBoost}%, TotalBoost {totalBoost}");
        }

    }


    public void UpgradePower()
    {
        if (!isPowerMax)
        {
            levelPower += 1;
            ratePower += 5;
            if (levelPower >= 60)
            {
                powerMoneyText.text = "MAX";
                powerMoneyText.color = Color.yellow;
                isPowerMax = true;
            }
            moneyPower = moneyPower * 1.34f;
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

            // Sử dụng hàm tính toán chung
            CalculateUpgradeValues();

            // THÊM: Lưu ngay sau khi nâng cấp
            SaveUpgradeData();

            levelPowerText.text = "Character is " + ratePower + " % power";
            Debug.Log($"UpgradePower: Level {levelPower}, Rate {ratePower}%, Force {launchForce}");
        }
    }

    void UpdateSliderAchievement()
    {
        if (sliderAchievement != null)
        {
            // Tính toán giá trị mục tiêu dựa trên khoảng cách bay được
            float maxDistance = 600f; // Khoảng cách tối đa để đạt 100%
            float targetValue = Mathf.Clamp01(distanceTraveled / maxDistance);

            // Cập nhật slider với hiệu ứng mượt mà
            float smoothSpeed = 2f; // Tốc độ cập nhật (càng cao càng nhanh)
            sliderAchievement.value = Mathf.Lerp(sliderAchievement.value, targetValue, smoothSpeed * Time.deltaTime);
        }

        // Debug thông tin độ cao và hiệu quả boost
        if (currentAltitude > altitudeDragStart)
        {
            float altitudeRatio = (currentAltitude - altitudeDragStart) / (maxAltitude - altitudeDragStart);
            float efficiency = CalculateAltitudeEfficiency();

            if (currentAltitude % 50f < 1f) // Log mỗi 50m để tránh spam
            {
                Debug.Log($"Altitude: {currentAltitude:F0}m/{maxAltitude}m - Efficiency: {efficiency:F1}% - Drag: {altitudeRatio:F2}");
            }
        }
    }

    public void leaderBoard()
    {
        leaderBoardImage.gameObject.SetActive(true);
    }

    public void exitLeaderBoard()
    {
        leaderBoardImage.gameObject.SetActive(false);
    }
    [Header("RotationZ Oscillation")]

    public float angleRangeMax = 22f;
    public float angleRangeMin = -30f;        // Phạm vi góc dao động ±20 độ
    public float speed = 1f;            // Tốc độ dao động
    public bool isRotationOscillating = true;
    private float startZ;
    private float startWheelZ;
    public void rotationAngleZ()
    {
        if (isRotationOscillating)
        {
            if (arrowAngleZ == null) return;
            float angleAverange = (angleRangeMax + angleRangeMin) / 2;
            // Debug.Log("angleAverange: " + angleAverange);


            float angle = Mathf.Sin(Time.time * speed) * (angleRangeMax - angleRangeMin) / 2 + angleAverange;

            // if (angle < -3f && angle > -8f)
            // {
            //     speed = 1.5f;
            // }
            // else if (angle <= -8f && angle >= -25f || angle >= -3f && angle <= 12f)
            // {
            //     speed = 1.2f;
            // }
            // else
            // {
            //     speed = 1f;
            // }
            arrowAngleZ.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            float angleAverange = (angleRangeMax + angleRangeMin) / 2;
            // Debug.Log("angleAverange: " + angleAverange);
            float angle = Mathf.Sin(Time.time * speed) * (angleRangeMax - angleRangeMin) / 2 + angleAverange;
            if (angle < -3f && angle > -8f)
            {
                newMapText.text = "MAX POWER!";
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
        levelPowerText.text = "Character is " + ratePower + " % power";
        levelFuelText.text = "Character is " + rateFuel + " % fuel";
        levelBoostText.text = "Character is " + rateBoost + " % boost";

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
        totalMoney = PlayerPrefs.GetInt("TotalMoney", 0);
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
    
    public RectTransform upHomeImage;
    public RectTransform downHomeImage;
    public RectTransform downFuelImage;
    public RectTransform upAircraftImage;
    public bool isHomeUp = false;
    public bool isHomeDown = false;
    public bool isFuelDown = false;
    public bool isAircraftUp = false;

    public void homeGameUp()
    {
        if (isHomeUp)
        {
            upHomeImage.DOAnchorPosY(upHomeImage.anchoredPosition.y + 1200f, duration)
                .SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    isHomeUp = false;
                    isFuelDown = true;
                    fuelPlayDown();
                });
        }
    }

    public void homeGameDown()
    {
        if (isHomeDown)
        {
            downHomeImage.DOAnchorPosY(downHomeImage.anchoredPosition.y - 300f, duration)
                .SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    isHomeDown = false;
                    isAircraftUp = true;
                    StartCoroutine(AircraftPlayUp());
                });
        }
    }

    public void fuelPlayDown()
    {
        if (isFuelDown)
        {
            downFuelImage.DOAnchorPosY(downFuelImage.anchoredPosition.y - 250f, duration)
                .SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    isFuelDown = false;
                });
        }
    }
    IEnumerator AircraftPlayUp()
    {
        if (isAircraftUp)
            yield return new WaitForSeconds(1f);
        {
            upAircraftImage.DOAnchorPosY(upAircraftImage.anchoredPosition.y + 250f, duration)
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
        PlayerPrefs.SetInt("TotalMoney", totalMoney);
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