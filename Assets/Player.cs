using UnityEngine;
using System.Collections;
using System.Data.SqlTypes;
using UnityEngine.SocialPlatforms.Impl;

public class Player : MonoBehaviour
{
    public static Player instance;
    [Header("Cài đặt máy bay")]
    public float launchForce = 20f; // Lực đẩy ban đầu
    public float gravityScale = 1f; // Thang trọng lực chung
    private bool hasLaunched = false;
    public bool isBonus = false;
    public bool isRocket = false;
    
    [Header("Hiệu ứng Bonus & Rocket")]
    public float bonusForceMultiplier = 1.05f; // Hệ số nhân lực khi ăn bonus
    public float bonusAngleIncrease = 10f; // Góc tăng thêm khi ăn bonus
    public float rocketAngleDecrease = 15f; // Góc giảm khi ăn rocket
    public float effectDuration = 2f; // Thời gian hiệu ứng kéo dài
    
    [Header("Rotation mượt mà")]
    public float rotationSpeed = 2f; // Tốc độ xoay mượt mà
    private float targetAngle = 0f; // Góc đích cần đạt tới
    private bool isRotating = false; // Đang trong quá trình xoay mượt mà

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Disable gravity ban đầu nếu muốn (tuỳ game)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = gravityScale;
            rb.velocity = Vector2.zero;
        }

        // Thiết lập gravity chung
        Physics2D.gravity = new Vector2(0, -9.81f * gravityScale);
    }

    void Update()
    {
        // Xử lý rotation mượt mà
        if (isRotating && !isOnGround)
        {
            float currentAngle = transform.eulerAngles.z;
            if (currentAngle > 180f) currentAngle -= 360f; // Convert to signed angle
            
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
            
            // Kiểm tra xem đã đạt tới góc đích chưa
            if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) < 1f)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
                isRotating = false;
            }
        }
        
        // Xử lý ma sát khi máy bay trên mặt đất
        if (isOnGround)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // rb.mass = 8f; // Tăng khối lượng để tăng ma sát

                // Áp dụng ma sát cho velocity theo trục x (ngang)
                Vector2 velocity = rb.velocity;

                // Ma sát chỉ ảnh hưởng đến chuyển động ngang
                velocity.x *= groundFriction;

                // Nếu tốc độ quá nhỏ thì dừng hẳn
                if (Mathf.Abs(velocity.x) < 0.1f)
                {
                    velocity.x = 0f;
                }

                // Xử lý rotation - nếu góc <= -1 thì set về 0
                float z = transform.eulerAngles.z;
                if (z > 180f) z -= 360f; // convert to signed angle in -180..180

                if (z <= -1f)
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }

                // Giữ nguyên velocity.y = 0 để không bị rơi xuống nữa
                velocity.y = 0f;

                rb.velocity = velocity;

                // Nếu máy bay gần như dừng hẳn, kết thúc game
                if (Mathf.Abs(velocity.x) < 0.05f && velocity.y == 0f)
                {
                    Debug.Log("Máy bay đã dừng hoàn toàn trên mặt đất!");
                    StartCoroutine(EndGameSequence());
                }
            }
        }
    }

    public bool isShipStand = false;
    public bool isOnGround = false;
    [Header("Ma sát đất")]
    public float groundFriction = 0.85f; // Hệ số ma sát (0-1, càng nhỏ càng trượt nhiều)
    public float groundDrag = 2f; // Lực cản không khí khi trên mặt đất

    public bool isBonus2 = false;
    public bool isBonus3 = false;
    public bool isBonus4 = false;
    public bool isBonus5 = false;
    public bool isBonus10 = false;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bonus2")
        {
            GameManager.instance.score *= 2;
            isBonus2 = true;
            ApplyBonusEffect();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Bonus3")
        {
            GameManager.instance.score *= 3;
            isBonus3 = true;
            ApplyBonusEffect();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Bonus4")
        {
            GameManager.instance.score *= 4;
            isBonus4 = true;
            ApplyBonusEffect();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Bonus5")
        {
            GameManager.instance.score *= 5;
            isBonus5 = true;
            ApplyBonusEffect();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Bonus10")
        {
            GameManager.instance.score *= 10;
            isBonus10 = true;
            ApplyBonusEffect();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Rocket")
        {
            GameManager.instance.score /= 2;
            isRocket = true;
            Debug.Log("Ăn Rocket!");
            ApplyRocketEffect();
            Destroy(collision.gameObject);
        }
    }

    void ApplyBonusEffect()
    {
        isBonus = true;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        if (rb != null && !isOnGround)
        {
            // Tăng lực đẩy lên
            Vector2 currentVelocity = rb.velocity;
            Vector2 bonusForce = new Vector2(
                currentVelocity.x * bonusForceMultiplier,
                currentVelocity.y + launchForce * 0.5f
            );
            rb.velocity = bonusForce;
            
            // Thiết lập góc đích để xoay mượt mà
            float currentAngle = transform.eulerAngles.z;
            if (currentAngle > 180f) currentAngle -= 360f; // Convert to signed angle
            
            targetAngle = Mathf.Clamp(currentAngle + bonusAngleIncrease, -45f, 45f);
            isRotating = true;
            
            Debug.Log($"Bonus Effect: Góc từ {currentAngle} sẽ tăng mượt mà lên {targetAngle}");
        }
        
        // Tự động tắt effect sau một thời gian
        StartCoroutine(DisableBonusEffect());
    }

    void ApplyRocketEffect()
    {
        isRocket = true;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        if (rb != null && !isOnGround)
        {
            // Thiết lập góc đích để xoay mượt mà hướng xuống
            float currentAngle = transform.eulerAngles.z;
            if (currentAngle > 180f) currentAngle -= 360f; // Convert to signed angle
            
            targetAngle = Mathf.Clamp(currentAngle - rocketAngleDecrease, -45f, 45f);
            isRotating = true;
            
            // Thêm lực hướng xuống
            Vector2 downwardForce = new Vector2(rb.velocity.x, rb.velocity.y - launchForce * 0.3f);
            rb.velocity = downwardForce;
            
            Debug.Log($"Rocket Effect: Góc từ {currentAngle} sẽ giảm mượt mà xuống {targetAngle}");
        }
        
        // Tự động tắt effect sau một thời gian
        StartCoroutine(DisableRocketEffect());
    }

    IEnumerator DisableBonusEffect()
    {
        yield return new WaitForSeconds(effectDuration);
        isBonus = false;
        Debug.Log("Bonus effect ended");
    }

    IEnumerator DisableRocketEffect()
    {
        yield return new WaitForSeconds(effectDuration);
        isRocket = false;
        Debug.Log("Rocket effect ended");
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isOnGround = true;

            // Tắt gravity khi trên mặt đất để tránh rung lắc
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.drag = groundDrag;
            }
            if (rb.velocity.y <= 0f)
            {
                Vector2 velocity = rb.velocity;
                velocity.y = 0f;
                rb.velocity = velocity;
                rb.rotation = 0f;
                Debug.Log("Đã chạm đất, dừng rơi và đặt góc về 0");
                StartCoroutine(StopRunning());
            }
        }
    }

    public IEnumerator StopRunning()
    {
        yield return new WaitForSeconds(1f);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Khi rời khỏi mặt đất
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isOnGround = false;

            // Khôi phục gravity và drag bình thường
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = gravityScale;
                rb.drag = 0f; // Reset drag về 0 khi bay
            }
        }
    }

    public void LoadGame()
    {
        transform.position = GameManager.instance.startPosition;
        transform.rotation = Quaternion.identity;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f; // tắt gravity cho tới khi launch lại nếu cần
            rb.drag = 0f; // Reset drag về 0
            rb.mass = 1f; // Reset mass về 1
            rb.isKinematic = false;
        }

        hasLaunched = false;
        isBonus = false;
        isRocket = false;
        isOnGround = false; // Reset trạng thái đất
        isShipStand = false; // Reset trạng thái tàu

        // Reset rotation mượt mà
        isRotating = false;
        targetAngle = 0f;
        GameManager.instance.score = 0;
        GameManager.instance.slider.value = 0f;
        targetValue = 0f;
        GameManager.instance.playButton.gameObject.SetActive(true);
        GameManager gameManager = GameManager.instance;
        System.Reflection.FieldInfo lastDistanceField = typeof(GameManager).GetField("lastDistance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (lastDistanceField != null)
            lastDistanceField.SetValue(gameManager, 0f);

        GameManager.instance.moneyPlayer += GameManager.instance.money;
        Debug.Log("moneyPlayer: " + GameManager.instance.moneyPlayer);
        PlayerPrefs.SetInt("Money", GameManager.instance.moneyPlayer);
        GameManager.instance.money = 0;
        GameManager.instance.score = 0; 
        GameManager.instance.moneyTextPlayer.text = GameManager.instance.moneyPlayer.ToString();

        // Dừng tất cả coroutines hiệu ứng
        StopAllCoroutines();

        Physics2D.gravity = new Vector2(0, -9.81f * gravityScale);

        gameObject.SetActive(true);

        if (BonusSpawner.instance != null) BonusSpawner.instance.DeleteSpawnedItems();
        if (RocketSpawner.instance != null) RocketSpawner.instance.DeleteSpawnedItems();
        if (MapSpawner.instance != null) MapSpawner.instance.DeleteSpawnedItems();
        GameManager.instance.imageUpgradePlay.gameObject.SetActive(true);
    }

    // IEnumerator delayLoadGame()
    // {
    //     yield return new WaitForSeconds(1f);
    //     Rigidbody2D rb = GetComponent<Rigidbody2D>();

    //     // gameObject.SetActive(true);
    //     LoadGame();

    //     if (GameManager.instance.isLoop)
    //     {
    //         yield return new WaitForSeconds(1f);
    //         Debug.Log("Loop Game");
    //         GameManager.instance.LaunchAirplane();
    //     }
    // }

    public void NextPlay()
    {
        GameManager.instance.imageWin.gameObject.SetActive(false);
        
        LoadGame();
    }

    public float duration = 2f;


    IEnumerator CountMoney()
    {
        float elapsed = 0f;
        int currentMoney = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            currentMoney = Mathf.RoundToInt(progress * GameManager.instance.money);
            GameManager.instance.moneyText.text = "$" + currentMoney.ToString();
            yield return null;
        }

        GameManager.instance.moneyText.text = "$" + GameManager.instance.money.ToString();
    }

    IEnumerator CountScore()
    {
        float elapsed = 0f;
        int currentScore = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            currentScore = Mathf.RoundToInt(progress * GameManager.instance.score);
            GameManager.instance.scoreText.text = "$" + currentScore.ToString();
            yield return null;
        }

        GameManager.instance.scoreText.text = "$" + GameManager.instance.score.ToString();
    }
    
    public float targetValue = 0f; 

    IEnumerator EndGameSequence()
    {
        // Đợi một frame để đảm bảo UI được setup
        yield return null;
        
        Debug.Log("Starting end game sequence...");
        
        // Setup UI
        GameManager.instance.imageWin.gameObject.SetActive(true);
        GameManager.instance.money = Mathf.RoundToInt(GameManager.instance.score / 3.5f);
        
        // Chạy các animation cùng lúc
        StartCoroutine(CountMoney());
        StartCoroutine(CountScore());
        StartCoroutine(AnimateSlider());
        
        // Đợi animation hoàn thành trước khi làm gì khác
        yield return new WaitForSeconds(duration + 0.5f);
        
        Debug.Log("End game sequence completed");
    }

    IEnumerator AnimateSlider()
    {
        targetValue = Mathf.Clamp01(GameManager.instance.distanceTraveled / 600f);

        // Kiểm tra null safety
        if (GameManager.instance?.slider == null || GameManager.instance?.completeText == null)
        {
            Debug.LogError("GameManager instance, slider hoặc completeText bị null!");
            yield break;
        }

        float elapsed = 0f;
        float startValue = GameManager.instance.slider.value;
        Debug.Log($"Starting slider animation from {startValue} to {targetValue}");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float currentValue = Mathf.Lerp(startValue, targetValue, progress);

            // Cập nhật slider
            GameManager.instance.slider.value = currentValue;

            // Cập nhật text theo phần trăm
            int percent = Mathf.RoundToInt(currentValue * 100f);
            GameManager.instance.completeText.text = percent.ToString() + "%";
            
            Debug.Log($"Slider progress: {percent}%"); // Debug để theo dõi

            yield return null;
        }

        // Đảm bảo kết thúc chính xác
        GameManager.instance.slider.value = targetValue;
        GameManager.instance.completeText.text = Mathf.RoundToInt(targetValue * 100f).ToString() + "% ";
        
        Debug.Log("Slider animation completed!");
    }
    

}