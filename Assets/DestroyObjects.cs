using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DestroyObjects : MonoBehaviour
{
    [Header("Destroy Settings")]
    public float destroyDistance = 20f; 

    public CinemachineVirtualCamera Camera;
    private Vector2 startPosition;

    void Start()
    {
        Camera = FindObjectOfType<CinemachineVirtualCamera>();
        startPosition = transform.position;
    }

    void Update()
    {

        Vector2 cameraPosition = Camera.transform.position;

        if (transform.position.x < cameraPosition.x - destroyDistance && !gameObject.CompareTag("DontDestroy"))
        {
            Destroy(gameObject);
        }
        
    }
}