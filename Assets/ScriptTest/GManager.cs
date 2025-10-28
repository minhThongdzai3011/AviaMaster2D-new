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


    [Header("Button UI")]
    public Button playButton;

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

    [Header("Người chơi")]
    public bool isControllable = false;
    public float controlForce = 8f; // Lực điều khiển A/D
    public float maxVerticalSpeed = 15f; // Giới hạn tốc độ dọc

    void Awake()
    {


    }

    void Start()
    {
        instance = this;
        totalMoney = PlayerPrefs.GetInt("TotalMoney", 0);

        // Cập nhật UI
        if (totalMoneyText != null)
            totalMoneyText.text = "" + totalMoney;
        
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
        if (airplaneRigidbody2D == null)
        {
            Debug.LogError("Rigidbody2D chưa được gán!");
            return;
        }

        StartCoroutine(LaunchSequence());
    }


    IEnumerator LaunchSequence()
    {
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
            
            // Debug để theo dõi
            if (Time.frameCount % 30 == 0) // Mỗi 30 frame log một lần
            {
                Debug.Log($"Bay lên - Altitude: {currentAltitude:F1}/{targetAltitude:F1} ({altitudeProgress*100:F0}%) - Rotation: {currentRotation:F1}°");
            }
            
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

        
        if (isVelocity)
        {
            airplaneRigidbody2D.velocity = 0.5f * airplaneRigidbody2D.velocity;
            isVelocity = false;
        }

        if (isControllable)
        {
            HandleAircraftControl();
        }
        else
        {
            UpdateRotationFromVelocity();
        }


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
        if (Input.GetKey(KeyCode.A))
        {
            PlainUp();
        }
        if (Input.GetKey(KeyCode.D) && !isControllable && !isBoosterActive)
        {
            PlainDown();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            AgainGame();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            LaunchAirplane();
        }

        if (distanceText != null) distanceText.text = distanceTraveled.ToString("F2") + " m";
        if (altitudeText != null) altitudeText.text = currentAltitude.ToString("F2") + " m";
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
            
            Debug.Log("Máy bay nghiêng xuống - Angle: " + targetRotation.ToString("F1") + "°");
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
                
                // Áp dụng lực theo hướng máy bay đang hướng
                float boostForce = 15f; // Có thể tùy chỉnh
                airplaneRigidbody2D.AddForce(boostDirection * boostForce, ForceMode2D.Force);
                
            }
        }
        else
        {
            // Nếu hết boost thì tắt booster
            isBoosterActive = false;
        }
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
    public void UpgradeFuel()
    {
        levelFuel += 1;
        durationFuel += 0.1f;
        durationFuel = (float)Math.Round(durationFuel, 1);
        levelFuelText.text = "Lv" + levelFuel + " | " + durationFuel + "s";
    }

    public void UpgradeBoost()
    {
        levelBoost += 1;
        totalBoost += 5;
        levelBoostText.text = "Lv" + levelBoost + " | " + totalBoost;
    }
    
    public void UpgradePower()
    {
        levelPower += 1;
        launchForce += 2;
        levelPowerText.text = "Lv" + levelPower + " | " + launchForce;
    }
}