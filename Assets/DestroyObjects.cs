using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DestroyObjects : MonoBehaviour
{
    [Header("Destroy Settings")]
    public float destroyDistance = 20f; // Khoảng cách phía sau camera để xóa object

    public CinemachineVirtualCamera Camera;
    private Vector2 startPosition;

    void Start()
    {
        Camera = FindObjectOfType<CinemachineVirtualCamera>();
        startPosition = transform.position;
    }

    void Update()
    {

        // Lấy vị trí hiện tại của camera
        Vector2 cameraPosition = Camera.transform.position;

        // Kiểm tra nếu object ở phía sau camera theo trục X với khoảng cách destroyDistance
        if (transform.position.x < cameraPosition.x - destroyDistance && !gameObject.CompareTag("DontDestroy"))
        {
            Destroy(gameObject);
        }
        
    }
}