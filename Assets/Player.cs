using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float launchForce = 20f; // Lực đẩy ban đầu
    public float climbAngle = 15f;  // Góc bay lên (độ)
    public float gravityScale = 1f; // Thang trọng lực chung
    private bool hasLaunched = false;
    public int index = 2;
    public bool isBonus = false;
    public bool isRocket = false;

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
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"other.tag: {other.tag}, hasLaunched: {hasLaunched}, isBonus: {isBonus}, isRocket: {isRocket}");
        if (other.CompareTag("Ship"))
        {
            if (hasLaunched)
            {
                Debug.Log("Máy bay đã chạm tàu!");
                hasLaunched = false;
            }
        }

        if (other.CompareTag("Bonus"))
        {
            Debug.Log("Máy bay nhận được vật phẩm Bonus!");
            Bonus(index);
            Destroy(other.gameObject);
            isBonus = true;
        }

        if (other.CompareTag("Rocket"))
        {
            Debug.Log("Máy bay va chạm với tên lửa!");
            Rocket(index);
            Destroy(other.gameObject);
            isRocket = true;
        }

        if (other.CompareTag("Sea"))
        {
            Debug.Log("Chạm Biển");
            float ftemp = GameManager.instance.distanceTraveled;
            int itemp = Mathf.FloorToInt(ftemp);
            GameManager.instance.money += itemp;
            GameManager.instance.moneyText.text = GameManager.instance.money.ToString("");
            PlayerPrefs.SetInt("Money", GameManager.instance.money);
            StartCoroutine(delayLoadGame());
        }
    }

    public void Bonus(int index)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on Player!");
            return;
        }

        // Reset velocity
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Reset gravity chung
        Physics2D.gravity = new Vector2(0, -9.81f * gravityScale);

        float bonusForce;
        if (GameManager.instance != null)
        {
            bonusForce = GameManager.instance.launchForce + (index * 2f);
        }
        else
        {
            bonusForce = launchForce + (index * 2f);
        }

        // Tính hướng bay lên trong 2D: dùng góc so với trục x (sang phải là 0 độ)
        float angleRad = Mathf.Deg2Rad * climbAngle;
        Vector2 launchDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;

        // Nếu máy bay hướng trái trong game, bạn có thể đảo x: launchDirection.x *= -1;

        rb.AddForce(launchDirection * bonusForce, ForceMode2D.Impulse);

        rb.gravityScale = gravityScale;

        Debug.Log($"Bonus activated! Force used: {bonusForce} (Base: {launchForce}, Index: {index})");

        hasLaunched = true;
    }

    public void Rocket(int index)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on Player!");
            return;
        }

        if (isRocket)
        {
            Debug.Log("Rocket effect already applied.");
            return;
        }

        float downwardForce = 10f + (index * 3f);

        rb.AddForce(Vector2.down * downwardForce, ForceMode2D.Impulse);

        StartCoroutine(IncreaseGravityTemporarily2D(2f, 3f));

        Debug.Log($"Rocket hit! Airplane falling faster with downward force: {downwardForce}");

        isRocket = true;
    }

    private IEnumerator IncreaseGravityTemporarily2D(float multiplier, float duration)
    {
        Physics2D.gravity = new Vector2(0, -9.81f * gravityScale * multiplier);

        yield return new WaitForSeconds(duration);

        Physics2D.gravity = new Vector2(0, -9.81f * gravityScale);
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
        }

        hasLaunched = false;
        isBonus = false;
        isRocket = false;

        Physics2D.gravity = new Vector2(0, -9.81f * gravityScale);

        gameObject.SetActive(true);

        if (BonusSpawner.instance != null) BonusSpawner.instance.DeleteSpawnedItems();
        if (RocketSpawner.instance != null) RocketSpawner.instance.DeleteSpawnedItems();
        // if (ShipSpawner.instance != null) ShipSpawner.instance.DeleteSpawnedItems();
    }

    IEnumerator delayLoadGame()
    {
        yield return new WaitForSeconds(2f);
        LoadGame();
        if (GameManager.instance.isLoop)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("Loop Game");
            GameManager.instance.LaunchAirplane();
        }
    }
}