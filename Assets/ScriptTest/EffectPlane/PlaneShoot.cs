using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    // Start is called before the first frame update
    void Start()
    {
        // Khởi tạo số đạn
        currentBullets = maxBullets;
        
        // Nếu không có firePoint, dùng vị trí của object
        if (firePoint == null)
        {
            firePoint = transform;
        }
        
        Debug.Log($"Plane 13 khởi tạo với {currentBullets} viên đạn");
    }

    // Update is called once per frame
    void Update()
    {
        // Kiểm tra nếu ấn phím O và còn đạn
        if (Input.GetKeyDown(KeyCode.O) && Time.time >= nextFireTime && currentBullets > 0)
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
        
        // Giảm số đạn
        currentBullets--;
        Debug.Log($"Bắn! Còn lại {currentBullets} viên đạn");
        
        // Tạo viên đạn tại vị trí firePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        // Lấy Rigidbody2D của đạn (nếu có)
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            // Tính hướng bắn dựa trên rotation của máy bay
            // Trong Unity 2D, hướng "up" là hướng mặc định
            Vector2 shootDirection = firePoint.up;
            
            // Hoặc nếu muốn bắn theo hướng right:
            // Vector2 shootDirection = firePoint.right;
            
            // Áp dụng vận tốc cho đạn
            rb.velocity = shootDirection * bulletSpeed;
        }
        else
        {
            Debug.LogWarning("Bullet không có Rigidbody2D component!");
        }
        
        // Thông báo khi hết đạn
        if (currentBullets <= 0)
        {
            Debug.Log("Hết đạn!");
        }
    }
    
    // Phương thức để reload đạn (nếu cần)
    public void ReloadBullets()
    {
        currentBullets = maxBullets;
        Debug.Log($"Đã reload! Có {currentBullets} viên đạn");
    }
}
