using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public static Shop instance;
    public Image imageShop;
    public Image imagePlane1;
    public Image imagePlane2;
    public Image imagePlane3;
    public Sprite[] spritePlanes;
    
    private int currentIndex = 1; // Bắt đầu từ index 1
    
    void Start()
    {
        instance = this;
        
        // Khởi tạo ban đầu với spritePlanes[1,2,3]
        InitializePlanes();
    }

    void Update()
    {

    }
    
    void InitializePlanes()
    {
        if (spritePlanes != null && spritePlanes.Length >= 4)
        {
            imagePlane1.sprite = spritePlanes[1];
            imagePlane2.sprite = spritePlanes[2];
            imagePlane3.sprite = spritePlanes[3];
            currentIndex = 1;
            Debug.Log("Khởi tạo máy bay: Plane1=1, Plane2=2, Plane3=3");
        }
    }

    public void OpenShop()
    {
        Debug.Log("Mở cửa hàng!");
        imageShop.gameObject.SetActive(true);
        
        // Đảm bảo hiển thị đúng khi mở shop
        InitializePlanes();
    }

    public void CloseShop()
    {
        Debug.Log("Đóng cửa hàng!");
        imageShop.gameObject.SetActive(false);
    }

    public void buttonRight()
    {
        if (spritePlanes == null || spritePlanes.Length == 0) return;
        
        // Tăng index lên 1
        currentIndex++;
        
        // Nếu vượt quá độ dài mảng, quay về 0
        if (currentIndex >= spritePlanes.Length - 2) // -2 vì cần 3 sprite liên tiếp
        {
            currentIndex = 0;
        }
        
        // Cập nhật 3 sprite liên tiếp
        imagePlane1.sprite = spritePlanes[currentIndex];
        imagePlane2.sprite = spritePlanes[currentIndex + 1];
        imagePlane3.sprite = spritePlanes[currentIndex + 2];
        
    }
    
    public void buttonLeft()
    {
        if (spritePlanes == null || spritePlanes.Length == 0) return;
        
        // Giảm index xuống 1
        currentIndex--;
        
        // Nếu nhỏ hơn 0, quay về cuối mảng
        if (currentIndex < 0)
        {
            currentIndex = spritePlanes.Length - 3; // -3 vì cần 3 sprite liên tiếp
        }
        
        // Cập nhật 3 sprite liên tiếp
        imagePlane1.sprite = spritePlanes[currentIndex];
        imagePlane2.sprite = spritePlanes[currentIndex + 1];
        imagePlane3.sprite = spritePlanes[currentIndex + 2];
        
    }
}
