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
        currentBullets = maxBullets;
        
        if (firePoint == null)
        {
            firePoint = transform;
        }
        
        Debug.Log($"Plane 13 khởi tạo với {currentBullets} viên đạn");
    }

    void Update()
    {
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
        
        currentBullets--;
        Debug.Log($"Bắn! Còn lại {currentBullets} viên đạn");
        
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
        Debug.Log($"Đã reload! Có {currentBullets} viên đạn");
    }
}
