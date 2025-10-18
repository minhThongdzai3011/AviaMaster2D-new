using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public static Player instance;
    [Header("Cài đặt máy bay")]
    public float launchForce = 20f; // Lực đẩy ban đầu
    private float climbAngle = 15f;  // Góc bay lên (độ)
    public float gravityScale = 1f; // Thang trọng lực chung
    private bool hasLaunched = false;
    public int index = 2;
    public bool isBonus = false;
    public bool isRocket = false;

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
        // Xử lý ma sát khi máy bay trên mặt đất
        if (isOnGround)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.mass = 8f; // Tăng khối lượng để tăng ma sát
                
                // Áp dụng ma sát cho velocity theo trục x (ngang)
                Vector2 velocity = rb.velocity;

                // Ma sát chỉ ảnh hưởng đến chuyển động ngang
                // velocity.x *= groundFriction;

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
                
                // Debug để theo dõi
                Debug.Log($"Ground Friction - Velocity.x: {velocity.x}, Angle: {z}");
                
                // Nếu máy bay gần như dừng hẳn, kết thúc game
                if (Mathf.Abs(velocity.x) < 0.05f && velocity.y == 0f)
                {
                    Debug.Log("Máy bay đã dừng hoàn toàn trên mặt đất!");
                    StartCoroutine(delayLoadGame());
                }
            }
        }
    }

    public bool isShipStand = false;
    public bool isOnGround = false;
    [Header("Ma sát đất")]
    public float groundFriction = 0.95f; // Hệ số ma sát (0-1, càng nhỏ càng trượt nhiều)
    public float groundDrag = 2f; // Lực cản không khí khi trên mặt đất
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Máy bay chạm đất!");
            isOnGround = true;
            GameManager.instance.anim.SetBool("isUp", false);
            GameManager.instance.anim.SetBool("isStand", true);
            
            // Tắt gravity khi trên mặt đất để tránh rung lắc
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                // Thêm drag để máy bay chậm dần
                rb.drag = groundDrag;
            }
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        // Khi rời khỏi mặt đất
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Máy bay rời khỏi mặt đất!");
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
        }

        hasLaunched = false;
        isBonus = false;
        isRocket = false;
        isOnGround = false; // Reset trạng thái đất
        isShipStand = false; // Reset trạng thái tàu

        Physics2D.gravity = new Vector2(0, -9.81f * gravityScale);

        gameObject.SetActive(true);

        if (BonusSpawner.instance != null) BonusSpawner.instance.DeleteSpawnedItems();
        if (RocketSpawner.instance != null) RocketSpawner.instance.DeleteSpawnedItems();
        if (ShipSpawner.instance != null) ShipSpawner.instance.DeleteSpawnedItems();
    }

    IEnumerator delayLoadGame()
    {
        yield return new WaitForSeconds(1f);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.mass = 1f;
        // gameObject.SetActive(true);
        LoadGame();
        if (GameManager.instance.isLoop)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("Loop Game");
            GameManager.instance.LaunchAirplane();
        }
    }
}