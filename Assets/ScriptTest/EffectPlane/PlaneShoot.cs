using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlaneShoot : MonoBehaviour
{
    public static PlaneShoot instance;
    [Header("Shooting Settings")]
    public GameObject bulletPrefab; // Prefab của viên đạn - gắn vào Inspector
    public Transform firePoint; // Vị trí bắn đạn (nếu có)
    public float bulletSpeed = 20f; // Tốc độ đạn
    public float fireRate = 0.3f; // Tốc độ bắn (giây giữa mỗi lần bắn)
    
    [Header("Ammo Settings")]
    public int maxBullets = 3; // Số đạn tối đa
    
    private int currentBullets; // Số đạn hiện tại
    private float nextFireTime = 0f;

    [Header("Image Bullet")]
    public Image[] imageBullets; // Mảng hình ảnh đạn để hiển thị số đạn còn lại

     // Hiệu ứng bắn đạn (nếu có)
    
    // Start is called before the first frame update
    void Start()
    {
        currentBullets = maxBullets;
        
        if (firePoint == null)
        {
            firePoint = transform;
        }
        
        // Đảm bảo tất cả image bullets hiển thị ban đầu
        if (imageBullets != null)
        {
            for (int i = 0; i < imageBullets.Length; i++)
            {
                if (imageBullets[i] != null)
                {
                    imageBullets[i].gameObject.SetActive(true);
                }
            }
        }
        
        Debug.Log($"Plane 13 khởi tạo với {currentBullets} viên đạn");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextFireTime && currentBullets > 0 && GManager.instance.isBoosted)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }
    
    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet Prefab chưa được gán!");
            return;
        }
        
        currentBullets--;
        Debug.Log($"Bắn! Còn lại {currentBullets} viên đạn");
        
        // Set active false cho image bullet tương ứng
        if (imageBullets != null && currentBullets >= 0 && currentBullets < imageBullets.Length)
        {
            imageBullets[currentBullets].gameObject.SetActive(false);
        }
        
        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            float angleZ = firePoint.eulerAngles.z;

            // 1. Set rotation cho đạn (để sprite nhìn đúng)
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, angleZ);

            // 2. Tính hướng bay theo rotation
            Vector2 direction = new Vector2(
                Mathf.Cos(angleZ * Mathf.Deg2Rad),
                Mathf.Sin(angleZ * Mathf.Deg2Rad)
            );

            // 3. Set velocity
            rb.velocity = direction.normalized * bulletSpeed;

            // Gửi góc bay cho Bullet để smooth
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetTargetAngle(angleZ);
            }


            // 4. Khóa rotation – KHÔNG cho xoáy
            rb.freezeRotation = true;
            rb.angularVelocity = 0f;
        }


        else
        {
            Debug.LogWarning("Bullet không có Rigidbody2D component!");
        }
        
        if (currentBullets <= 0)
        {
            Debug.Log("Hết đạn!");
        }
    }
    
    public void ReloadBullets()
    {
        currentBullets = maxBullets;
        
        // Set active true cho tất cả image bullets khi reload
        if (imageBullets != null)
        {
            for (int i = 0; i < imageBullets.Length; i++)
            {
                if (imageBullets[i] != null)
                {
                    imageBullets[i].gameObject.SetActive(true);
                }
            }
        }
        
        Debug.Log($"Đã reload! Có {currentBullets} viên đạn");
    }
}
