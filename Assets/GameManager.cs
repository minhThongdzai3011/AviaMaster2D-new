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
    public float climbAngle = 30f;  // Góc bay lên (độ)
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

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        Time.timeScale = 1f;

    }

    void Start()
    {
        moneyPlayer = PlayerPrefs.GetInt("Money", 0);
        if (airplaneRigidbody2D != null)
        {
            startPosition = airplaneRigidbody2D.transform.position;
            airplaneRigidbody2D.velocity = Vector2.zero;
            airplaneRigidbody2D.angularVelocity = 0f;
            airplaneRigidbody2D.gravityScale = gravityScale;
        }
        money = PlayerPrefs.GetInt("Money", 0);
        moneyText.text = money.ToString("");
        // Thiết lập gravity toàn cục nếu bạn dùng Physics2D.gravity
        Physics2D.gravity = new Vector2(0f, -9.81f * gravityScale);

    }

    public void LaunchAirplane()
    {

        if (airplaneRigidbody2D == null)
        {
            Debug.LogError("Rigidbody2D chưa được gán!");
            return;
        }
        anim.SetBool("isUp", true);
        anim.SetBool("isStand", false);

        // Tính hướng bay lên theo góc so với trục +X (sang phải là 0 độ)
        float angleRad = Mathf.Deg2Rad * climbAngle;
        Vector2 launchDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;

        // Áp dụng lực đẩy ban đầu (ForceMode2D.Impulse)
        airplaneRigidbody2D.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);

        // Đảm bảo gravityScale đúng (nếu trước đó bị tắt)
        airplaneRigidbody2D.gravityScale = gravityScale;

        Physics2D.gravity = new Vector2(0f, -9.81f * gravityScale);
    }

    void Update()
    {
        if (airplaneRigidbody2D == null) return;

        Vector2 currentPos = airplaneRigidbody2D.transform.position;
        distanceTraveled = Vector2.Distance(startPosition, currentPos);
        currentAltitude = currentPos.y - startPosition.y;
        if (currentAltitude < 0f) currentAltitude = 0f;
        money = Mathf.FloorToInt(distanceTraveled * 2f);
        distanceText.text = distanceTraveled.ToString("F2") + " m";
        altitudeText.text = currentAltitude.ToString("F2") + " m";
        moneyText.text = money.ToString("F0");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchAirplane();
        }
        if (airplaneRigidbody2D.velocity.y > 0.1f)
        {
            isUp = true;
            isStand = false;
            anim.SetBool("isUp", true);
            anim.SetBool("isStand", false);
        }
        else if (airplaneRigidbody2D.velocity.y < -0.1f)
        {
            isUp = false;
            isStand = false;
            anim.SetBool("isUp", false);
            anim.SetBool("isStand", false);
        }
        else
        {
            isUp = false;
            isStand = true;
            anim.SetBool("isUp", false);
            anim.SetBool("isStand", true);
        }

    }

    public void UpgradeFlightAngle()
    {
        if (moneyPlayer >= flightAngleMoney)
        {
            moneyPlayer -= flightAngleMoney;
            flightAngleLevel++;
            // climbAngle += 5f; // Tăng góc bay lên
            flightAngleMoney = Mathf.FloorToInt(flightAngleMoney * 1.5f); // Tăng giá tiền
            moneyTextPlayer.text = moneyPlayer.ToString("");
            PlayerPrefs.SetInt("Money", moneyPlayer);
            flightAngleTextLevel.text = "Level " + flightAngleLevel;
            flightAngleTextMoney.text = flightAngleMoney.ToString();
        }
        else
        {
            Debug.Log("Không đủ tiền để nâng cấp Góc bay!" + moneyPlayer + " < " + flightAngleMoney);
        }
    }
    public void UpgradeFlyingPower()
    {
        if (moneyPlayer >= flyingPowerMoney)
        {
            moneyPlayer -= flyingPowerMoney;
            flyingPowerLevel++;
            // launchForce += 2f; // Tăng lực bay
            flyingPowerMoney = Mathf.FloorToInt(flyingPowerMoney * 1.5f); // Tăng giá tiền
            moneyTextPlayer.text = moneyPlayer.ToString("");
            PlayerPrefs.SetInt("Money", moneyPlayer);
            flyingPowerTextLevel.text = "Level " + flyingPowerLevel;
            flyingPowerTextMoney.text = flyingPowerMoney.ToString();
        }
    }
    public void UpgradeTrainDistance()
    {
        if (moneyPlayer >= trainDistanceMoney)
        {
            moneyPlayer -= trainDistanceMoney;
            trainDistanceLevel++;
            // gravityScale = Mathf.Max(0.1f, gravityScale - 0.5f); // Giảm trọng lực
            if (airplaneRigidbody2D != null)
            {
                airplaneRigidbody2D.gravityScale = gravityScale;
            }
            Physics2D.gravity = new Vector2(0f, -9.81f * gravityScale);
            trainDistanceMoney = Mathf.FloorToInt(trainDistanceMoney * 1.5f); // Tăng giá tiền
            moneyTextPlayer.text = moneyPlayer.ToString("");
            PlayerPrefs.SetInt("Money", moneyPlayer);
            trainDistanceTextLevel.text = "Level " + trainDistanceLevel;
            trainDistanceTextMoney.text = trainDistanceMoney.ToString();
        }
    }

    public void LoopGame()
    {
        if (isLoop)
        {
            isLoop = false;
            loopImage.sprite = noLoopSprites;
        }
        else
        {
            isLoop = true;
            loopImage.sprite = autoLoopSprites;
        }

    }


}