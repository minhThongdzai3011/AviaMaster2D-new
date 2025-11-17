using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Plane : MonoBehaviour
{
    public static Plane instance;
    public ParticleSystem smokeEffect;
    public TrailRenderer trailEffect;


    public int moneyDistance = 0;
    public int moneyCollect = 0;
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
            // GManager.instance.isControllable = false;
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
            AnimCoin anim = other.GetComponent<AnimCoin>();
            if (anim != null)
            {
                // tăng tiền ngay (hoặc bạn có thể tăng tiền khi animation hoàn tất trong AnimCoin)
                moneyCollect += 1;

                anim.collected = true;
                anim.Collect();
            }
            else
            {
                moneyCollect += 1;
                Destroy(other.gameObject);
            }
        }
        if (other.CompareTag("bird"))
        {
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Bonus"))
        {
            Destroy(other.gameObject);
            GManager.instance.durationFuel += (GManager.instance.durationFuel * 0.2f);
        }
        if (other.CompareTag("Bonus2"))
        {
            Destroy(other.gameObject);
            GManager.instance.totalBoost += (GManager.instance.totalBoost * 0.2f);
        }
        if (other.CompareTag("Bonus3"))
        {
            Destroy(other.gameObject);
            GManager.instance.totalDiamond += 10;
            
            // THÊM: Kiểm tra null safety trước khi gọi AnimDiamond
            if (AnimDiamond.instance != null)
            {
                AnimDiamond.instance.MoveUp();
            }
            else
            {
                Debug.LogWarning("AnimDiamond.instance is null! Make sure AnimDiamond component exists in scene.");
            }
        }
    }

    IEnumerator UpMass()
    {
        if (GManager.instance == null) { Debug.Log("GManager instance is null!"); yield break; }
        Rigidbody2D rb = GManager.instance.airplaneRigidbody2D;
        if (rb == null) { Debug.Log("AirplaneRigidbody2D chưa được khởi tạo!"); yield break; }
        
        // THÊM: Lấy rotation.z khi chạm đất
        float landingRotationZ = rb.transform.eulerAngles.z;
        if (landingRotationZ > 180f) landingRotationZ -= 360f; // Chuyển về [-180, 180]
        

        // Tham số trượt tự nhiên - SỬA để tránh giật
        float groundDrag = 0.5f; // Drag nhẹ hơn để tránh giật
        float minimumSlideSpeed = 0.1f; // Giảm threshold để smooth hơn
        float rotationSmoothSpeed = 1.5f; // Tốc độ giảm rotation

        // Lưu giá trị ban đầu
        float originalDrag = rb.drag;
        float originalMass = rb.mass;
        Vector2 landingVelocity = rb.velocity;
        
        Debug.Log($"Máy bay chạm đất - Velocity: {landingVelocity.magnitude:F1}, Landing speed: {landingVelocity.x:F1}, Rotation Z: {landingRotationZ:F1}°");

        // Áp dụng drag nhẹ để trượt tự nhiên
        rb.drag = groundDrag;

        // Đảm bảo máy bay không bounce trên mặt đất
        if (rb.velocity.y < 0) rb.velocity = new Vector2(rb.velocity.x, 0f);

        WaitForSeconds wait = new WaitForSeconds(0.02f); // 50 FPS thay vì 10 FPS

        // Giai đoạn 1: Trượt với ma sát dần dần và giảm rotation
        float slideTimer = 0f;
        float maxSlideTime = 5f; // Giảm thời gian để tránh kéo dài
        
        while (slideTimer < maxSlideTime && Mathf.Abs(rb.velocity.x) > minimumSlideSpeed)
        {
            slideTimer += 0.02f; // Update mỗi frame
            
            // SỬA: Ma sát mượt mà hơn
            Vector2 currentVelocity = rb.velocity;
            
            // Ma sát nhẹ và đều - tránh giật
            float frictionProgress = slideTimer / maxSlideTime;
            float currentFriction = Mathf.Lerp(0.998f, 0.995f, frictionProgress); // Ma sát rất nhẹ
            currentVelocity.x *= currentFriction;
            
            // Đảm bảo không có chuyển động dọc
            currentVelocity.y = 0f;
            
            // Áp dụng velocity mới
            rb.velocity = currentVelocity;
            
            // THÊM: Giảm rotation.z từ từ về 0
            Vector3 currentRotation = rb.transform.eulerAngles;
            float currentZ = currentRotation.z;
            if (currentZ > 180f) currentZ -= 360f;
            
            if (Mathf.Abs(currentZ) > 0.1f)
            {
                float targetZ = Mathf.LerpAngle(currentZ, 0f, Time.deltaTime * rotationSmoothSpeed);
                rb.transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, targetZ);
            }
            if ( rb.velocity.x > minimumSlideSpeed )
            {
                rb.velocity -= new Vector2(-0.001f, 0f);
                Debug.Log("Giảm velocity.x dần đều trong trượt" + rb.velocity.x);
                break;
            }
            
            
            yield return wait;
        }
        
        // Giai đoạn 2: Dừng hoàn toàn
        rb.velocity = Vector2.zero;
        
        // THÊM: Đảm bảo rotation.z = 0 khi dừng
        Vector3 finalRotation = rb.transform.eulerAngles;
        rb.transform.rotation = Quaternion.Euler(finalRotation.x, finalRotation.y, 0f);
        
        Debug.Log("Máy bay đã dừng hoàn toàn và rotation.z đã reset về 0°");
        
        
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
            GManager.instance.coinEffect.Stop();
            isAddMoneyDone = true;
            moneyDistance = (int)(GManager.instance.distanceTraveled / 1.34f);
            moneyTotal = moneyDistance + moneyCollect;
            Debug.Log("Money from Distance: " + moneyDistance + " | Current Money: " + moneyCollect + " | Total Money: " + moneyTotal);
            GManager.instance.totalMoney += moneyTotal;
            Debug.Log("Updated Total Money: " + GManager.instance.totalMoney);
            // Settings.instance.totalMoneyPlayText.text = "" + moneyTotal;

            // Cập nhật UI
            GManager.instance.totalMoneyText.text = "" + GManager.instance.totalMoney;
            GManager.instance.moneyText.text = "" + moneyCollect;


            // Lưu TotalMoney và đảm bảo lưu ngay
            PlayerPrefs.SetFloat("TotalMoney", GManager.instance.totalMoney);
            PlayerPrefs.Save();
            Settings.instance.OpenWinImage();
        }
    }
    

}
