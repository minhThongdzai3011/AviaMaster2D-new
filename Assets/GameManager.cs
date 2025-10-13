using TMPro;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Rigidbody2D airplaneRigidbody2D;

    [Header("Cài đặt máy bay")]
    public float launchForce = 20f; // Lực đẩy ban đầu
    public float climbAngle = 30f;  // Góc bay lên (độ)
    public float gravityScale = 1f; // Thang trọng lực chung

    [Header("Text UI")]
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI altitudeText;
    public TextMeshProUGUI moneyText;

    public Vector2 startPosition;
    public float distanceTraveled;
    private float currentAltitude;
    public int money = 0;
    public bool isLoop = false;
    public Animator anim;
    public bool isUp;
    public bool isStand;

    private float elapsedTime = 0f;
    private bool isCounting = false;

private float previousY;
    private float currentY;


    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        Time.timeScale = 1f;
    }

    void Start()
    {
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
        previousY = transform.position.y;

    }

    public void LaunchAirplane()
    {
        StartCoroutine(CountTime());

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
        StartCoroutine(delayOneSeconds());
    }

    void Update()
    {
        if (airplaneRigidbody2D == null) return;

        Vector2 currentPos = airplaneRigidbody2D.transform.position;
        distanceTraveled = Vector2.Distance(startPosition, currentPos);
        currentAltitude = currentPos.y - startPosition.y;
        if (currentAltitude < 0f) currentAltitude = 0f;

        distanceText.text = distanceTraveled.ToString("F2") + " m";
        altitudeText.text = currentAltitude.ToString("F2") + " m";

        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchAirplane();
        }
        // currentY = transform.position.y;

        // if (currentY > previousY)
        // {
        //     Debug.Log("Máy bay đang bay lên");
        // }
        // else if (currentY < previousY)
        // {
        //     Debug.Log("Máy bay đang hạ xuống");
        //     isUp = false;
        // }
        // else
        // {
        //     Debug.Log("Máy bay đứng yên theo trục Y");
        //     Debug.Log($"currentY: {currentY}, previousY: {previousY}");
        // }

        // previousY = currentY;

    }

    IEnumerator CountTime()
    {
        isCounting = true;
        while (isCounting)
        {
            elapsedTime += Time.deltaTime;
            Debug.Log("Thời gian trôi qua: " + elapsedTime.ToString("F2") + " giây");
            yield return null;
        }
    }

    IEnumerator delayOneSeconds()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("isUp",false);
    }



}