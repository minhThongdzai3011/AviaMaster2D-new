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
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenShop()
    {
        Debug.Log("Mở cửa hàng!");
        imageShop.gameObject.SetActive(true);
    }

    public void CloseShop()
    {
        Debug.Log("Đóng cửa hàng!");
        imageShop.gameObject.SetActive(false);
    }

    public void buttonRight()
    {
        for (int i = 0; i < spritePlanes.Length - 1; i++)
        {
            if (imagePlane1.sprite == spritePlanes[i])
            {
                imagePlane1.sprite = spritePlanes[i + 1];
                imagePlane2.sprite = spritePlanes[(i + 2) % spritePlanes.Length];
                imagePlane3.sprite = spritePlanes[(i + 3) % spritePlanes.Length];
                break;
            }
        }
    }
    
    public void buttonLeft()
    {
        for (int i = 1; i < spritePlanes.Length; i++)
        {
            if (imagePlane1.sprite == spritePlanes[i])
            {
                imagePlane1.sprite = spritePlanes[i - 1];
                imagePlane2.sprite = spritePlanes[(i) % spritePlanes.Length];
                imagePlane3.sprite = spritePlanes[(i + 1) % spritePlanes.Length];
                break;
            }
        }
    }
}
