using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlaneShoot : MonoBehaviour
{
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
        SuperPlaneManager.instance.textBulletPlane.text = "x" + currentBullets.ToString();
        Debug.Log($"Bắn! Còn lại {currentBullets} viên đạn");
        
        // Set active false cho image bullet tương ứng
        if (imageBullets != null && currentBullets >= 0 && currentBullets < imageBullets.Length)
        {
            imageBullets[currentBullets].gameObject.SetActive(false);
        }
        
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            Vector2 shootDirection = firePoint.right;
            
            rb.velocity = shootDirection * bulletSpeed;
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
