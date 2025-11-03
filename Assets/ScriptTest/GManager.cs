using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

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
    public int totalMoney = 0;
    public float targetValue = 0f;
    public float duration = 1f;
    public bool isPlaying = true;

    [Header("Nâng cấp")]
    public float durationFuel = 2;
    public float totalBoost = 100;
    public int levelPower;
    public int levelFuel;
    public int levelBoost;

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

    [Header("UI Texts")]
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI altitudeText;
    public TextMeshProUGUI rotationZText;
    public TextMeshProUGUI levelPowerText;
    public TextMeshProUGUI levelFuelText;
    public TextMeshProUGUI levelBoostText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI totalMoneyText;
    public TextMeshProUGUI powerMoneyText;
    public TextMeshProUGUI fuelMoneyText;
    public TextMeshProUGUI boostMoneyText;

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
    public Image exitLeaderBoardImage;

    void Awake()
    {

        instance = this;
    }

    void Start()
    {
        sliderAchievement.value = 0f;
        startZ = airplaneRigidbody2D.transform.rotation.eulerAngles.z;

        // Cập nhật UI
        SaveTotalMoney();

        if (airplaneRigidbody2D != null)
        {
            startPosition = airplaneRigidbody2D.transform.position;
            airplaneRigidbody2D.velocity = Vector2.zero;
            airplaneRigidbody2D.angularVelocity = 0f;
            airplaneRigidbody2D.gravityScale = gravityScale;

            airplaneRigidbody2D.freezeRotation = true;
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
            homeImage.gameObject.SetActive(false);
            playImage.gameObject.SetActive(true);

            StartCoroutine(LaunchSequence());
            isPlaying = false;
            isPlay = true;
            
            isRotationOscillating = false;
        }
    }


    IEnumerator LaunchSequence()
    {
        yield return new WaitForSeconds(0.5f);
        arrowAngleZ.SetActive(false);
        rotationXObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        // Bước 1: Di chuyển ngang một đoạn r
        float r = 5f;
        float horizontalSpeed = 10f;
        float targetX = airplaneRigidbody2D.position.x + r;

        airplaneRigidbody2D.gravityScale = 0f;
        airplaneRigidbody2D.velocity = new Vector2(horizontalSpeed, 0f);

        // Giữ góc 0 khi bay ngang
        airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        while (airplaneRigidbody2D.position.x < targetX)
        {
            yield return null;
        }

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
            float currentRotation = Mathf.Lerp(startRotation, targetRotation, altitudeProgress);
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);

            yield return null;
        }

        // Đảm bảo rotation đạt đúng climbAngle cuối cùng
        airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, climbAngle);
        Debug.Log($"Hoàn thành bay lên - Final rotation: {climbAngle}°");

        // Bước 4: Giữ độ cao và cho phép điều khiển
        airplaneRigidbody2D.gravityScale = 0.2f;
        airplaneRigidbody2D.velocity = new Vector2(airplaneRigidbody2D.velocity.x, 0f);

        isControllable = true;
        Debug.Log("Máy bay đã vào trạng thái có thể điều khiển." + durationFuel);
        StartCoroutine(DecreaseSliderFuel(durationFuel));

        // Trong giai đoạn này, rotation được điều khiển bởi HandleAircraftControl()
        float timer = 0f;
        while (timer < durationFuel)
        {
            timer += Time.deltaTime;

            // Nếu không điều khiển thủ công, tự động xoay theo velocity
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                Vector2 vel = airplaneRigidbody2D.velocity;
                if (vel.magnitude > 1f)
                {
                    float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
                    angle = Mathf.Clamp(angle, -45f, 45f);

                    float currentZ = airplaneRigidbody2D.transform.eulerAngles.z;
                    if (currentZ > 180f) currentZ -= 360f;

                    float targetAngle = Mathf.LerpAngle(currentZ, angle, Time.deltaTime * 2f);
                    airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
                }
            }

            yield return null;
        }

        // Bước 5: Kết thúc điều khiển và rơi
        isControllable = false;
        airplaneRigidbody2D.gravityScale = gravityScale;
        Physics2D.gravity = new Vector2(0f, -9.81f * gravityScale);

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
                airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
            }
            yield return null;
        }

        Debug.Log("Máy bay đã hoàn thành chuỗi bay: ngang → bay lên (xoay dần) → giữ (có điều khiển) → rơi");
    }

    public bool isVelocity = true;
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


        if (isVelocity)
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
        }
        else
        {
            UpdateRotationFromVelocity();
        }

        if (isPlay)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartBoostDecrease();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                StopBoostDecrease();
            }
            if (Input.GetKey(KeyCode.Space))
            {
                BoosterUp();
            }
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            PlainUp();
        }
        if (Input.GetKey(KeyCode.D) && !isControllable && !isBoosterActive)
        {
            PlainDown();
        }
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

    void HandleAircraftControl()
    {
        if (airplaneRigidbody2D == null) return;

        // Lấy góc hiện tại và chuyển về dạng -180 đến +180
        float currentZ = airplaneRigidbody2D.transform.eulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;

        // Điều khiển lên (A key)
        if (Input.GetKey(KeyCode.A))
        {
            // Xoay máy bay lên nhưng giới hạn tối đa 45 độ
            float targetRotation = Mathf.Min(currentZ + Time.deltaTime * 60f, maxUpAngle);
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);

            // Nếu đang boost thì thêm lực theo hướng mới
            if (isBoosterActive)
            {
                float angleRad = targetRotation * Mathf.Deg2Rad;
                Vector2 forceDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
                airplaneRigidbody2D.AddForce(forceDirection * controlForce * 0.3f, ForceMode2D.Force);
            }

        }

        // Điều khiển xuống (D key)
        if (Input.GetKey(KeyCode.D))
        {
            // Xoay máy bay xuống nhưng giới hạn tối đa -45 độ
            float targetRotation = Mathf.Max(currentZ - Time.deltaTime * 60f, maxDownAngle);
            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);

            // Nếu đang boost thì thêm lực theo hướng mới
            if (isBoosterActive)
            {
                float angleRad = targetRotation * Mathf.Deg2Rad;
                Vector2 forceDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
                airplaneRigidbody2D.AddForce(forceDirection * controlForce * 0.3f, ForceMode2D.Force);
            }

        }

        // Khi không nhấn A hoặc D, từ từ trở về góc 0
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            float targetRotation;

            // Tính toán để về 0 một cách mượt mà
            if (currentZ > 0)
            {
                targetRotation = Mathf.Max(currentZ - Time.deltaTime * 90f, 0f);
            }
            else if (currentZ < 0)
            {
                targetRotation = Mathf.Min(currentZ + Time.deltaTime * 90f, 0f);
            }
            else
            {
                targetRotation = 0f;
            }

            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);
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

            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, newZ);
        }
        else
        {
            airplaneRigidbody2D.transform.rotation = Quaternion.Lerp(
                airplaneRigidbody2D.transform.rotation,
                Quaternion.identity,
                Time.deltaTime * 2f
            );
        }
    }

    public void BoosterUp()
    {
        if (airplaneRigidbody2D != null && totalBoost > 0)
        {
            isBoosterActive = true;
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
                
                Debug.Log($"Boost: Power={actualPowerMultiplier:F2}x, Altitude Eff={altitudeEfficiency:F2}, Force={boostForce:F1}");
            }
        }
        else
        {
            // Nếu hết boost thì tắt booster
            isBoosterActive = false;
        }
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

    public void PlainUp()
    {
        rotationZ = airplaneRigidbody2D.transform.eulerAngles.z + 0f;
    }

    public void PlainDown()
    {
        rotationZ = airplaneRigidbody2D.transform.eulerAngles.z - 0f;
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
            durationFuel = durationFuel * (1 + (rateFuel / 100f));
            durationFuel = (float)Math.Round(durationFuel, 1);
            levelFuelText.text = "Character is " + rateFuel + " % fuel";
        }
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
            totalBoost = totalBoost * (1 + (rateBoost / 100f));
            totalBoost = (float)Math.Round(totalBoost, 1);
            levelBoostText.text = "Character is " + rateBoost + " % boost";
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
            launchForce = launchForce * (1 + (ratePower / 100f));
            launchForce = (float)Math.Round(launchForce, 1);
            levelPowerText.text = "Character is " + ratePower + " % power";
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


}