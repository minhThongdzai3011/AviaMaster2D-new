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

    [Header("Button UI")]
    public Button playButton;

    [Header("Slider UI")]
    public Slider sliderFuel;
    public Slider sliderBooster;

    [Header("Trạng thái chơi")]
    public Vector2 startPosition;
    public float distanceTraveled;
    private float currentAltitude;
    private float rotationZ;
    public bool isBoosterActive = false;
    public float boostDecreaseRate = 100f; 
    private bool isBoostDecreasing = false;
    private Coroutine boostCoroutine;
    public int money = 0;

    [Header("Nâng cấp")]
    public int durationFuel = 2;
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

    void Awake()
    {


    }

    void Start()
    {
        instance = this;
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
        float r = 5f; // khoảng cách ngang trước khi bay lên
        float horizontalSpeed = 10f;
        float targetX = airplaneRigidbody2D.position.x + r;

        airplaneRigidbody2D.gravityScale = 0f; // tạm tắt trọng lực
        airplaneRigidbody2D.velocity = new Vector2(horizontalSpeed, 0f);

        // Chờ đến khi máy bay đi hết đoạn r
        while (airplaneRigidbody2D.position.x < targetX)
        {
            yield return null;
        }

        // Bước 2: Bay lên
        float climbForce = launchForce;
        float angleRad = Mathf.Deg2Rad * climbAngle;
        Vector2 launchDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;

        airplaneRigidbody2D.velocity = Vector2.zero;
        airplaneRigidbody2D.AddForce(launchDirection * climbForce, ForceMode2D.Impulse);

        // Bước 3: Chờ đến khi đạt độ cao thích hợp
        float targetAltitude = startPosition.y + 10f;
        while (airplaneRigidbody2D.position.y < targetAltitude)
        {
            yield return null;
        }

        // Bước 4: Giữ độ cao trong 2 giây
        airplaneRigidbody2D.gravityScale = 0f;
        airplaneRigidbody2D.velocity = new Vector2(airplaneRigidbody2D.velocity.x, 0f);
        StartCoroutine(DecreaseSliderFuel(durationFuel));
        yield return new WaitForSeconds(durationFuel);

        // Bước 5: Bắt đầu rơi xuống từ từ
        airplaneRigidbody2D.gravityScale = gravityScale;
        Physics2D.gravity = new Vector2(0f, -9.81f * gravityScale);

        Debug.Log("Máy bay đã hoàn thành chuỗi bay: ngang → bay lên → giữ → rơi");
    }

    void Update()
    {
        if (airplaneRigidbody2D == null) return;

        Vector2 currentPos = airplaneRigidbody2D.transform.position;
        distanceTraveled = Vector2.Distance(startPosition, currentPos);
        currentAltitude = currentPos.y - startPosition.y;
        if (currentAltitude < 0f) currentAltitude = 0f;
        rotationZ = airplaneRigidbody2D.transform.eulerAngles.z;


        UpdateRotationFromVelocity();

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
        if (Input.GetKey(KeyCode.E))
        {
            PlainUp();
        }
        if (Input.GetKey(KeyCode.D))
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

        if (GManager.instance.airplaneRigidbody2D.velocity.x == 0)
        {
            money = Mathf.FloorToInt(distanceTraveled / 1.67f);
            moneyText.text = money.ToString() + " $";
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
                airplaneRigidbody2D.AddForce(new Vector2(0f, 10f), ForceMode2D.Force);
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void PlainUp()
    {
        rotationZ = airplaneRigidbody2D.transform.eulerAngles.z + 45f;
    }

    public void PlainDown()
    {
        rotationZ = airplaneRigidbody2D.transform.eulerAngles.z - 45f;
    }

// Giảm fuel theo thời gian
    private IEnumerator DecreaseSliderFuel(int durationFuel)
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
        durationFuel += 1;
        levelFuelText.text = "Lv" + levelFuel + " | " + durationFuel + "s";
    }

    public void UpgradeBoost()
    {
        levelBoost += 1;
        totalBoost += 20;
        levelBoostText.text = "Lv" + levelBoost + " | " + totalBoost;
    }
    
    public void UpgradePower()
    {
        levelPower += 1;
        launchForce += 5;
        levelPowerText.text = "Lv" + levelPower + " | " + launchForce;
    }
}