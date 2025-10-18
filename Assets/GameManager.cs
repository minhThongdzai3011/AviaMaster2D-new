using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Rigidbody2D airplaneRigidbody2D;

    [Header("Cài đặt máy bay")]
    public float launchForce = 20f; // Lực đẩy ban đầu
    [Tooltip("Góc bay lên ban đầu khi nhấn Launch (độ). Có thể thay đổi trong Inspector.")]
    public float climbAngle = 30f;  // Góc bay lên (độ) - có thể chỉnh trong Inspector
    public float gravityScale = 1f; // Thang trọng lực chung

    [Header("Sprite")]
    public Sprite autoLoopSprites;
    public Sprite noLoopSprites;

    [Header("Image UI")]
    public Image loopImage;

    [Header("Button UI")]
    public Button upgradeFlightAngleButton;
    public Button upgradeFlyingPowerButton;
    public Button upgradeTrainDistanceButton;

    [Header("Text UI")]
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI altitudeText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI flightAngleTextLevel;
    public TextMeshProUGUI flightAngleTextMoney;
    public TextMeshProUGUI flyingPowerTextLevel;
    public TextMeshProUGUI flyingPowerTextMoney;
    public TextMeshProUGUI trainDistanceTextLevel;
    public TextMeshProUGUI trainDistanceTextMoney;
    public TextMeshProUGUI moneyTextPlayer;

    [Header("Trạng thái chơi")]
    public Vector2 startPosition;
    public float distanceTraveled;
    private float currentAltitude;
    public int money = 0;
    public int moneyPlayer = 0;
    public bool isLoop = false;
    public Animator anim;
    public bool isUp;
    public bool isStand;

    [Header("Nâng cấp")]
    public int flightAngleLevel = 1;
    public int flyingPowerLevel = 1;
    public int trainDistanceLevel = 1;
    public int flightAngleMoney = 200;
    public int flyingPowerMoney = 200;
    public int trainDistanceMoney = 200;

    [Header("Rotation settings")]
    public float maxUpAngle = 45f;    // giới hạn góc lên
    public float maxDownAngle = -45f; // giới hạn góc xuống
    public float rotationSmooth = 8f; // độ mượt khi quay
    public float minMoveThreshold = 0.5f; // ngưỡng vận tốc để bắt đầu quay theo velocity

    void Awake()
    {
        if (instance == null) instance = this; else Destroy(gameObject);

        Time.timeScale = 1f;
        moneyPlayer = PlayerPrefs.GetInt("Money", 0);
        if (moneyTextPlayer != null) moneyTextPlayer.text = moneyPlayer.ToString("");
        flightAngleLevel = PlayerPrefs.GetInt("FlightAngleLevel", 1);
        flyingPowerLevel = PlayerPrefs.GetInt("FlyingPowerLevel", 1);
        trainDistanceLevel = PlayerPrefs.GetInt("TrainDistanceLevel", 1);
        flightAngleMoney = PlayerPrefs.GetInt("FlightAngleMoney", 200);
        flyingPowerMoney = PlayerPrefs.GetInt("FlyingPowerMoney", 200);
        trainDistanceMoney = PlayerPrefs.GetInt("TrainDistanceMoney", 200);
        if (flightAngleTextLevel != null) flightAngleTextLevel.text = "Level " + flightAngleLevel;
        if (flightAngleTextMoney != null) flightAngleTextMoney.text = flightAngleMoney.ToString();
        if (flyingPowerTextLevel != null) flyingPowerTextLevel.text = "Level " + flyingPowerLevel;
        if (flyingPowerTextMoney != null) flyingPowerTextMoney.text = flyingPowerMoney.ToString();
        if (trainDistanceTextLevel != null) trainDistanceTextLevel.text = "Level " + trainDistanceLevel;
        if (trainDistanceTextMoney != null) trainDistanceTextMoney.text = trainDistanceMoney.ToString();

        Time.timeScale = 0.5f;
        AudioBackgroundPlay();
    }

    void Start()
    {
        if (airplaneRigidbody2D != null)
        {
            startPosition = airplaneRigidbody2D.transform.position;
            airplaneRigidbody2D.velocity = Vector2.zero;
            airplaneRigidbody2D.angularVelocity = 0f;
            airplaneRigidbody2D.gravityScale = gravityScale;

            // Khóa rotation vật lý để script điều khiển rotation
            airplaneRigidbody2D.freezeRotation = true;
        }
        money = PlayerPrefs.GetInt("Money", 0);
        if (moneyText != null) moneyText.text = money.ToString("");
        Physics2D.gravity = new Vector2(0f, -9.81f * gravityScale);
    }

    public void LaunchAirplane()
    {
        if (airplaneRigidbody2D == null)
        {
            Debug.LogError("Rigidbody2D chưa được gán!");
            return;
        }
        if (anim != null)
        {
            anim.SetBool("isUp", true);
            anim.SetBool("isStand", false);
        }

        // Tính hướng bay lên theo góc climbAngle (độ)
        float angleRad = Mathf.Deg2Rad * climbAngle;
        Vector2 launchDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;

        // Áp dụng lực đẩy ban đầu
        airplaneRigidbody2D.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);

        // Đảm bảo gravityScale đúng
        airplaneRigidbody2D.gravityScale = gravityScale;
        Physics2D.gravity = new Vector2(0f, -9.81f * gravityScale);

        // Đặt rotation khởi tạo theo hướng launchDirection chính xác (sử dụng Atan2)
        float launchAngleDeg = Mathf.Atan2(launchDirection.y, launchDirection.x) * Mathf.Rad2Deg;
        airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, launchAngleDeg);

        Debug.Log("Máy bay đã được phóng với lực: " + launchForce + " và góc: " + climbAngle + " (launchAngleDeg = " + launchAngleDeg + ")");
    }

    void Update()
    {
        if (airplaneRigidbody2D == null) return;

        Vector2 currentPos = airplaneRigidbody2D.transform.position;
        distanceTraveled = Vector2.Distance(startPosition, currentPos);
        currentAltitude = currentPos.y - startPosition.y;
        if (currentAltitude < 0f) currentAltitude = 0f;
        money = Mathf.FloorToInt(distanceTraveled * 2f);
        if (distanceText != null) distanceText.text = distanceTraveled.ToString("F2") + " m";
        if (altitudeText != null) altitudeText.text = currentAltitude.ToString("F2") + " m";
        if (moneyText != null) moneyText.text = money.ToString("F0");

        // Tự động xoay máy bay theo hướng vận tốc khi có velocity
        UpdateRotationFromVelocity();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchAirplane();
        }
    }

    // Thêm hai hàm helper ở cùng class

    float ComputeUpTargetAngle(Vector2 velocity)
    {
        // Tính góc từ vector vận tốc, nhưng ưu tiên cảm giác "bay lên"
        // Bạn có thể điều chỉnh upBias để làm góc lên nhạy hơn so với Atan2 thuần túy
        float upBias = 0.5f; // >1 làm tăng ảnh hưởng của vy
        float velAngle = Mathf.Atan2(velocity.y * upBias, velocity.x) * Mathf.Rad2Deg;
        return Mathf.Clamp(velAngle, 0f, maxUpAngle);
    }

    float ComputeDownTargetAngle(Vector2 velocity)
    {
        // Tính góc từ vector vận tốc, nhưng ưu tiên cảm giác "bay xuống"
        // downBias có thể >1 để nghiêng xuống mạnh hơn
        float downBias = 1.5f; // >1 làm tăng ảnh hưởng của vy (âm)
        float velAngle = Mathf.Atan2(velocity.y * downBias, velocity.x) * Mathf.Rad2Deg;
        return Mathf.Clamp(velAngle, maxDownAngle, 0f);
    }

    // Thay thế UpdateRotationFromVelocity bằng phiên bản gọi hai hàm trên

    void UpdateRotationFromVelocity()
    {
        if (airplaneRigidbody2D == null) return;

        Vector2 velocity = airplaneRigidbody2D.velocity;
        float minMoveSqr = minMoveThreshold * minMoveThreshold;

        // Nếu đang đứng trên tàu, về thẳng
        if (Player.instance != null && Player.instance.isShipStand)
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

            // Mượt hóa góc hiện tại -> target
            float currentZ = airplaneRigidbody2D.transform.eulerAngles.z;
            if (currentZ > 180f) currentZ -= 360f;
            float newZ = Mathf.Lerp(currentZ, targetAngle, Time.deltaTime * rotationSmooth);

            airplaneRigidbody2D.transform.rotation = Quaternion.Euler(0f, 0f, newZ);
        }
        else
        {
            // Không di chuyển đáng kể, từ từ về ngang (0 độ)
            airplaneRigidbody2D.transform.rotation = Quaternion.Lerp(
                airplaneRigidbody2D.transform.rotation,
                Quaternion.identity,
                Time.deltaTime * 2f
            );
        }
    }

    void AudioBackgroundPlay()
    {
        // Placeholder nếu bạn có âm thanh nền
    }
}