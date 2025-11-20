using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Plane : MonoBehaviour
{
    public static Plane instance;
    public ParticleSystem smokeEffect;
    public TrailRenderer trailEffect;
    public TrailRenderer trailRenderer;

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
        if (other.CompareTag("MapStartCity"))
        {
            MapSpawner.instance.isMapCityUnlocked = true;
            Debug.Log("Map City Unlocked");
            Settings.instance.NotificationNewMapText.text = "You have unlocked the City Map!";
            Settings.instance.NotificationNewMap();
        }
        if (other.CompareTag("MapStartBeach"))
        {
            MapSpawner.instance.isMapBeachUnlocked = true;
            Debug.Log("Map Beach Unlocked");
            Settings.instance.NotificationNewMapText.text = "You have unlocked the Beach Map!";
            Settings.instance.NotificationNewMap();
        }
        if (other.CompareTag("MapStartDesert"))
        {
            MapSpawner.instance.isMapDesertUnlocked = true;
            Debug.Log("Map Desert Unlocked");
            Settings.instance.NotificationNewMapText.text = "You have unlocked the Desert Map!";
            Settings.instance.NotificationNewMap();
        }
        if (other.CompareTag("MapStartField"))
        {
            MapSpawner.instance.isMapFieldUnlocked = true;
            Debug.Log("Map Field Unlocked");
            Settings.instance.NotificationNewMapText.text = "You have unlocked the Field Map!";
            Settings.instance.NotificationNewMap();
        }
        if (other.CompareTag("MapStartIce"))
        {
            MapSpawner.instance.isMapIceUnlocked = true;
            Debug.Log("Map Ice Unlocked");
            Settings.instance.NotificationNewMapText.text = "You have unlocked the Ice Map!";
            Settings.instance.NotificationNewMap();
        }
        if (other.CompareTag("MapStartLava"))
        {
            MapSpawner.instance.isMapLavaUnlocked = true;
            Debug.Log("Map Lava Unlocked");
            Settings.instance.NotificationNewMapText.text = "You have unlocked the Lava Map!";
            Settings.instance.NotificationNewMap();
        }
        
    }
    
    
    
    IEnumerator UpMass()
    {
        // Lấy Rigidbody2D từ GManager
        Rigidbody2D airplaneRb = GManager.instance.airplaneRigidbody2D;
        
        if (airplaneRb == null)
        {
            Debug.LogWarning("Airplane Rigidbody2D not found!");
            yield break;
        }
        
        // Lấy góc ban đầu khi máy bay chạm đất
        float initialAngleZ = GetCurrentRotationZ(airplaneRb);
        float initialAngleX = airplaneRb.transform.eulerAngles.x;
        if (initialAngleX > 180f) initialAngleX -= 360f; // Chuẩn hóa về [-180, 180]
        
        // Lấy velocity ban đầu
        Vector2 initialVelocity = airplaneRb.velocity;
        
        // Thời gian để giảm góc và velocity về 0 (có thể điều chỉnh)
        float duration = 3f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            // Tính tỷ lệ giảm dần (từ 1 về 0)
            float t = elapsedTime / duration;
            float smoothT = 1f - t; // Giảm từ 1 về 0
            
            // Giảm từ từ rotation về 0
            float targetAngleZ = initialAngleZ * smoothT;
            float targetAngleX = initialAngleX * smoothT;
            Vector3 currentRotation = airplaneRb.transform.eulerAngles;
            currentRotation.z = targetAngleZ;
            currentRotation.x = targetAngleX;
            airplaneRb.transform.eulerAngles = currentRotation;
            
            // Giảm từ từ velocity về 0
            Vector2 targetVelocity = initialVelocity * smoothT;
            airplaneRb.velocity = targetVelocity;
            
            yield return null;
        }
        
        // Đảm bảo rotation và velocity về 0 hoàn toàn
        Vector3 finalRotation = airplaneRb.transform.eulerAngles;
        finalRotation.z = 0f;
        finalRotation.x = 0f;
        airplaneRb.transform.eulerAngles = finalRotation;
        airplaneRb.velocity = Vector2.zero;
        
        // Có thể thêm logic khi máy bay đã dừng hoàn toàn
        Debug.Log("Airplane stopped sliding");
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
