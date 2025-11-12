using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlane : MonoBehaviour
{
    public static ChangePlane instance;
    public Rigidbody2D airplaneRigidbody2D;
    public bool isRotationChangePlane = true;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        airplaneRigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotationChangePlane)
        {
            airplaneRigidbody2D = GManager.instance.airplaneRigidbody2D;
            airplaneRigidbody2D.velocity = Vector2.zero;
            airplaneRigidbody2D.angularVelocity = 0f;
        }
    }
}
