using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    public Slider slider;

    [Header("Trạng thái chơi")]
    public Vector2 startPosition;
    public float distanceTraveled;
    private float currentAltitude;
    public int money = 0;
    public int moneyPlayer = 0;
    public int score = 0;
    public bool isLoop = false;
    public Animator anim;
    public bool isUp;
    public bool isStand;

    [Header("Nâng cấp")]
    public int flightAngleLevel = 1;
    public int flyingPowerLevel = 1;
    public int bonusPowerLevel = 1;
    public int flightAngleMoney = 200;
    public int flyingPowerMoney = 200;
    public int bonusPowerMoney = 200;

    [Header("Rotation settings")]
    public float maxUpAngle = 45f;    // giới hạn góc lên
    public float maxDownAngle = -45f; // giới hạn góc xuống
    public float rotationSmooth = 8f; // độ mượt khi quay
    public float minMoveThreshold = 0.5f; // ngưỡng vận tốc để bắt đầu quay theo velocity

    void Awake()
    {
        

    }

    void Start()
    {
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
        yield return new WaitForSeconds(2f);

        // Bước 5: Bắt đầu rơi xuống từ từ
        airplaneRigidbody2D.gravityScale = gravityScale;
        Physics2D.gravity = new Vector2(0f, -9.81f * gravityScale);

        Debug.Log("Máy bay đã hoàn thành chuỗi bay: ngang → bay lên → giữ → rơi");
    }


    float lastDistance = 0f;
    void Update()
    {
        if (airplaneRigidbody2D == null) return;

        Vector2 currentPos = airplaneRigidbody2D.transform.position;
        distanceTraveled = Vector2.Distance(startPosition, currentPos);
        currentAltitude = currentPos.y - startPosition.y;
        if (currentAltitude < 0f) currentAltitude = 0f;

        // Tính money từ khoảng cách
    

        UpdateRotationFromVelocity();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchAirplane();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            airplaneRigidbody2D.velocity = new Vector2(airplaneRigidbody2D.velocity.x, airplaneRigidbody2D.velocity.y + 5f);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            airplaneRigidbody2D.velocity = new Vector2(airplaneRigidbody2D.velocity.x, airplaneRigidbody2D.velocity.y - 5f);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            AgainGame();
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
        if (airplaneRigidbody2D != null)
        {
            airplaneRigidbody2D.velocity = new Vector2(airplaneRigidbody2D.velocity.x, airplaneRigidbody2D.velocity.y + 10f);
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Xử lý khi va chạm với mặt đất
            Debug.Log("Máy bay đã va chạm với mặt đất!");
            // Có thể thêm logic kết thúc trò chơi hoặc giảm máu ở đây
            StartCoroutine(UpMass());
        }

    }
    IEnumerator UpMass()
    {
        airplaneRigidbody2D.mass = 1.1f;
        yield return new WaitForSeconds(0.1f);
        airplaneRigidbody2D.mass = 1.3f;
        yield return new WaitForSeconds(0.1f);
        airplaneRigidbody2D.mass = 1.5f;
        yield return new WaitForSeconds(0.1f);
        airplaneRigidbody2D.mass = 1.7f;
        yield return new WaitForSeconds(0.1f);
        airplaneRigidbody2D.mass = 1.9f;
        yield return new WaitForSeconds(0.1f);
        airplaneRigidbody2D.mass = 2.1f;
        yield return new WaitForSeconds(0.1f);
        airplaneRigidbody2D.mass = 2.3f;
        Debug.Log("New mass: " + airplaneRigidbody2D.mass);

    }

}