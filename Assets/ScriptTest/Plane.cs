using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Plane : MonoBehaviour
{
    public static Plane instance;
    public ParticleSystem smokeEffect;
    public TrailRenderer trailEffect;
    

    private int moneyDistance = 0;
    public int moneyTotal = 0;
    // Start is called before the first frame update
    void Start()
    {
        smokeEffect.Stop();
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // THÊM: Thông báo cho GManager biết máy bay đã chạm đất
            if (GManager.instance != null)
            {
                GManager.instance.OnPlaneGroundCollision();
            }
            
            RotaryFront.instance.StopWithDeceleration(3.0f);
            smokeEffect.Stop();
            StartCoroutine(UpMass());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            GManager.instance.money += 1;
        }
    }

    IEnumerator UpMass()
    {
        if (GManager.instance == null) { Debug.Log("GManager instance is null!"); yield break; }
        Rigidbody2D rb = GManager.instance.airplaneRigidbody2D;
        if (rb == null) { Debug.Log("AirplaneRigidbody2D chưa được khởi tạo!"); yield break; }

        // Tham số trượt tự nhiên
        float groundDrag = 1f; // Drag nhẹ hơn để không dừng đột ngột
        float minimumSlideSpeed = 0.2f; // Tốc độ tối thiểu để tiếp tục trượt
        float massIncreaseRate = 0.1f; // Tăng mass chậm hơn để kéo dài quá trình trượt

        // Lưu giá trị ban đầu
        float originalDrag = rb.drag;
        float originalMass = rb.mass;
        Vector2 landingVelocity = rb.velocity;
        
        Debug.Log($"Máy bay chạm đất - Velocity: {landingVelocity.magnitude:F1}, Landing speed: {landingVelocity.x:F1}");

        // Áp dụng drag nhẹ để trượt tự nhiên
        rb.drag = groundDrag;

        // Đảm bảo máy bay không bounce trên mặt đất
        if (rb.velocity.y < 0) rb.velocity = new Vector2(rb.velocity.x, 0f);

        WaitForSeconds wait = new WaitForSeconds(0.1f); // Update thường xuyên hơn để mượt mà

        // Giai đoạn 1: Trượt với ma sát dần dần
        float slideTimer = 0f;
        float maxSlideTime = 8f; // Thời gian trượt tối đa (giây)
        
        while (slideTimer < maxSlideTime && Mathf.Abs(rb.velocity.x) > minimumSlideSpeed)
        {
            slideTimer += 0.5f;
            
            // Áp dụng ma sát dần dần
            Vector2 currentVelocity = rb.velocity;
            
            // Ma sát tăng dần theo thời gian (trượt chậm lại dần)
            float frictionProgress = slideTimer / maxSlideTime;
            // float currentFriction = Mathf.Lerp(0.995f, 0.92f, frictionProgress); // Từ ma sát nhẹ đến nặng
            float currentFriction = Mathf.Lerp(0.998f, 0.995f, frictionProgress);
            currentVelocity.x *= currentFriction;
            
            // Đảm bảo không có chuyển động dọc
            currentVelocity.y = 0f;
            
            // Áp dụng velocity mới
            rb.velocity = currentVelocity;
            
            // Tăng mass từ từ để tạo cảm giác nặng dần
            rb.mass += massIncreaseRate;
            
            // Debug mỗi giây
            if (Mathf.FloorToInt(slideTimer) != Mathf.FloorToInt(slideTimer - 0.05f))
            {
                Debug.Log($"Trượt - Time: {slideTimer:F1}s, Speed: {currentVelocity.x:F1}, Friction: {currentFriction:F3}");
            }
            
            yield return wait;
        }
        
        // Giai đoạn 2: Dừng hoàn toàn
        rb.velocity = Vector2.zero;
        rb.mass = 100f; // Set mass cuối cùng
        
        Debug.Log($"Máy bay dừng sau {slideTimer:F1}s trượt");
        
        // Bắt đầu tính tiền sau khi trượt xong
        yield return new WaitForSeconds(1f);
        StartCoroutine(OpenImageWIn());

        // Khôi phục drag ban đầu (tùy chọn)
        // rb.drag = originalDrag;
    }
    private bool isAddMoneyDone = false;
    IEnumerator OpenImageWIn()
    {
        yield return new WaitForSeconds(2f);
        if (!isAddMoneyDone)
        {
            isAddMoneyDone = true;
            moneyDistance = (int)(GManager.instance.distanceTraveled / 1.34f);
            moneyTotal = moneyDistance + GManager.instance.money;
            Debug.Log("Money from Distance: " + moneyDistance + " | Current Money: " + GManager.instance.money + " | Total Money: " + moneyTotal);
            GManager.instance.totalMoney += moneyTotal;
            Debug.Log("Updated Total Money: " + GManager.instance.totalMoney);
            GManager.instance.money = 0;
            Settings.instance.totalMoneyPlayText.text = "" + moneyTotal;

            // Cập nhật UI
            GManager.instance.totalMoneyText.text = "" + GManager.instance.totalMoney;
            GManager.instance.moneyText.text = "" + GManager.instance.money;


            // Lưu TotalMoney và đảm bảo lưu ngay
            PlayerPrefs.SetInt("TotalMoney", GManager.instance.totalMoney);
            PlayerPrefs.Save();
            Settings.instance.OpenWinImage();
        }
    }
    

}
