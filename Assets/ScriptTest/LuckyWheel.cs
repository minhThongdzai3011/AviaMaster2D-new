using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckyWheel : MonoBehaviour
{
    public static LuckyWheel instance;
    public float RotatePower;
    public float StopPower;
    public float minRotatePower = 1000f;
    public float maxRotatePower = 1800f;
    public float minStopPower = 200f;
    public float maxStopPower = 400f;
    private Rigidbody2D rbody;
    int inRotate;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        rbody = GetComponent<Rigidbody2D>();
        
    }
    float t;

    // Update is called once per frame
    void Update()
    {
        if (rbody.angularVelocity > 0)
        {
            rbody.angularVelocity -= StopPower * Time.deltaTime;
            rbody.angularVelocity = Mathf.Clamp(rbody.angularVelocity, 0, 1440);
        }

        if (rbody.angularVelocity == 0 && inRotate == 1)
        {
            t += 1 * Time.deltaTime;
            if (t >= 0.5f)
            {
                GetReward();
                inRotate = 0;
                t = 0;
            }
        }
    }

    public void Rotate()
    {
        RotatePower = Random.Range(minRotatePower, maxRotatePower);
        StopPower = Random.Range(minStopPower, maxStopPower);
        if (inRotate == 0)
        {
            rbody.AddTorque(RotatePower);
            inRotate = 1;
        }
    }

    public void GetReward()
    {
        float rot = transform.eulerAngles.z;
        if (rot > 0 + 22 && rot < 45 + 22)
        {
            Win(8);
        }
        else if (rot >= 45 + 22 && rot < 90 + 22)
        {
            Win(7);
        }
        else if (rot >= 90 + 22 && rot < 135 + 22)
        {
            Win(6);
        }
        else if (rot >= 135 + 22 && rot < 180 + 22)
        {
            Win(5);
        }
        else if (rot >= 180 + 22 && rot < 225 + 22)
        {
            Win(4);
        }
        else if (rot >= 225 + 22 && rot < 270 + 22)
        {
            Win(3);
        }
        else if (rot >= 270 + 22 && rot < 315 + 22)
        {
            Win(2);
        }
        else if (rot >= 315 + 22 && rot < 360 + 22)
        {
            Win(1);
        }
        StartCoroutine(WaitAndPrint(1f, rot));
    }

    public void Win(int Score)
    {
        print("You Win " + Score);
    }
    
    IEnumerator WaitAndPrint(float waitTime, float rot)
    {
        yield return new WaitForSeconds(waitTime);
        rot = 0;
        rbody.rotation = Quaternion.Euler(0, 0, 0).eulerAngles.z;
    }

}
