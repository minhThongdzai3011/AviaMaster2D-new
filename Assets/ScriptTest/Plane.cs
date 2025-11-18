using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Plane : MonoBehaviour
{
    public static Plane instance;
    public ParticleSystem smokeEffect;
    public TrailRenderer trailEffect;

    public bool isGrounded = false;

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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && !isGrounded)
        {
            isGrounded = true;
            Debug.Log("*** MÁY BAY CHẠM ĐẤT - BẮT ĐẦU HỆ THỐNG DỪNG ***");
            
            // THÊM: Thông báo cho GManager biết máy bay đã chạm đất
            if (GManager.instance != null)
            {
                GManager.instance.OnPlaneGroundCollision();
                // Tắt các hệ thống có thể can thiệp velocity
                GManager.instance.isControllable = false;
                GManager.instance.isBoosterActive = false;
            }

            // Dừng các hiệu ứng
            if (RotaryFront.instance != null)
                RotaryFront.instance.StopWithDeceleration(3.0f);
            
            smokeEffect.Stop();
            
            // *** QUAN TRỌNG: Gọi UpMass() để giảm tốc độ ***
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
        
        // Lưu góc máy bay khi chạm đất
        float landingRotationZ = rb.transform.eulerAngles.z;
        if (landingRotationZ > 180f) landingRotationZ -= 360f; // Chuyển về [-180, 180]
        
        Debug.Log($"*** BẮT ĐẦU UpMass() - Góc: {landingRotationZ:F1}°, Velocity: {rb.velocity.magnitude:F2}, VelocityX: {rb.velocity.x:F2} ***");

        // Tham số điều khiển - ĐIỀU CHỈNH để trượt mượt mà
        float rotationDecaySpeed = 1.5f; // Tốc độ giảm góc (giảm xuống để chậm hơn)
        float velocityDecayRate = 0.98f; // Tỷ lệ giảm tốc độ mỗi frame (0.98 = giảm 2%/frame - cao hơn để trượt lâu hơn)
        float frictionMultiplier = 0.3f; // Lực ma sát (giảm xuống để ít ma sát hơn)
        float minimumSpeed = 0.1f; // Tốc độ tối thiểu trước khi dừng hẳn
        
        // Đảm bảo không bounce và giữ velocity.x
        Vector2 currentVel = rb.velocity;
        if (currentVel.y != 0f) 
        {
            rb.velocity = new Vector2(currentVel.x, 0f);
            Debug.Log($"Reset velocity.y = 0, giữ velocity.x = {currentVel.x:F2}");
        }
        
        // Tắt gravity để không bị ảnh hưởng
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        
        // Giảm drag để trượt xa hơn
        float originalDrag = rb.drag;
        rb.drag = 0.2f; // Drag rất nhẹ

        WaitForFixedUpdate waitFixed = new WaitForFixedUpdate();
        
        float elapsedTime = 0f;
        int frameCount = 0;
        
        // Vòng lặp giảm dần rotation và velocity
        while (Mathf.Abs(rb.velocity.x) > minimumSpeed || Mathf.Abs(GetCurrentRotationZ(rb)) > 0.5f)
        {
            elapsedTime += Time.fixedDeltaTime;
            frameCount++;
            
            // === XỬ LÝ ROTATION ===
            float currentZ = GetCurrentRotationZ(rb);
            
            // Giảm rotation từ từ về 0 bằng Lerp
            float targetZ = Mathf.LerpAngle(currentZ, 0f, Time.fixedDeltaTime * rotationDecaySpeed);
            rb.transform.rotation = Quaternion.Euler(0f, 0f, targetZ);
            
            // === XỬ LÝ VELOCITY - TRƯỢT MƯỢT MÀ ===
            Vector2 currentVelocity = rb.velocity;
            
            // Giảm velocity theo tỷ lệ phần trăm (mượt mà)
            currentVelocity.x *= velocityDecayRate;
            
            // Thêm lực ma sát nhỏ dựa trên tốc độ hiện tại
            float frictionForce = Mathf.Abs(currentVelocity.x) * frictionMultiplier * Time.fixedDeltaTime;
            if (currentVelocity.x > 0)
            {
                currentVelocity.x -= frictionForce;
            }
            else if (currentVelocity.x < 0)
            {
                currentVelocity.x += frictionForce;
            }
            
            // Đảm bảo không có chuyển động dọc
            currentVelocity.y = 0f;
            
            // Áp dụng velocity mới
            rb.velocity = currentVelocity;
            
            // Debug mỗi 30 frame (0.5 giây)
            if (frameCount % 30 == 0)
            {
                Debug.Log($"[Frame {frameCount}, {elapsedTime:F1}s] Rotation: {currentZ:F2}° → {targetZ:F2}°, VelocityX: {currentVelocity.x:F2}, DecayRate: {velocityDecayRate}");
            }
            
            // Kiểm tra nếu quá lâu (10 giây) thì dừng cưỡng bức
            if (elapsedTime > 10f)
            {
                Debug.Log($"TIMEOUT - Dừng cưỡng bức sau {elapsedTime:F1}s");
                break;
            }
            
            yield return waitFixed;
        }
        
        // Dừng hoàn toàn
        rb.velocity = Vector2.zero;
        rb.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        rb.drag = originalDrag;
        
        Debug.Log($"*** MÁY BAY DỪNG HOÀN TOÀN - Thời gian: {elapsedTime:F2}s, Frames: {frameCount} ***");
        
        // Bắt đầu tính tiền sau khi trượt xong
        yield return new WaitForSeconds(1f);
        StartCoroutine(OpenImageWIn());
    }
    
    // Hàm helper lấy rotation Z chuẩn hóa về [-180, 180]
    float GetCurrentRotationZ(Rigidbody2D rb)
    {
        float z = rb.transform.eulerAngles.z;
        if (z > 180f) z -= 360f;
        return z;
    }
    private bool isAddMoneyDone = false;
    IEnumerator OpenImageWIn()
    {
        yield return new WaitForSeconds(2f);
        if (!isAddMoneyDone)
        {
            // THÊM: Lưu GameObject máy bay hiện tại trước khi kết thúc game
            SaveCurrentAirplane();
            
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
    
    // THÊM: Hàm lưu tên GameObject máy bay hiện tại
    void SaveCurrentAirplane()
    {
        if (GManager.instance != null && GManager.instance.airplaneRigidbody2D != null)
        {
            string currentAirplaneName = GManager.instance.airplaneRigidbody2D.gameObject.name;
            PlayerPrefs.SetString("LastUsedAirplane", currentAirplaneName);
            PlayerPrefs.Save();
            Debug.Log($"Lưu máy bay hiện tại: {currentAirplaneName}");
        }
    }

    
    

}
