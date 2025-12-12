using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public Sprite prizeSpriteImageCoin;
    public Sprite prizeSpriteCoin;
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
        AudioManager.instance.PlaySound(AudioManager.instance.luckyWheelSoundClip);
        if (Settings.instance.isSpinning)
        {

            RotatePower = Random.Range(minRotatePower, maxRotatePower);
            StopPower = Random.Range(minStopPower, maxStopPower);
            if (inRotate == 0)
            {
                rbody.AddTorque(RotatePower);
                inRotate = 1;
                
            }

        }
        // Settings.instance.AdsLuckyWheelButton.gameObject.SetActive(true);
    }
    public float rot;

    public void GetReward()
    {
        int a; string b;
        if (!Settings.instance.isSpinning) return;

        float rawRot = transform.eulerAngles.z;
        float adjustedRot = ((transform.eulerAngles.z + 360f) % 360f) % 360f;
        StartCoroutine(StopWheel(2f));
        if (adjustedRot >= 0 && adjustedRot < 45)
        {
            Win("12500Coin");
            a = 12500;
            b = "Coin";
            GManager.instance.AddCoinByLuckyWheel(12500);
            Debug.Log("Rot " + adjustedRot);
        }
        else if (adjustedRot >= 45 && adjustedRot < 90)
        {
            Win("250Diamond");
            a = 250;
            b = "Diamond";
            GManager.instance.AddDiamondByLuckyWheel(250);
            Debug.Log("Rot " + adjustedRot);
        }
        else if (adjustedRot >= 90 && adjustedRot < 135)
        {
            Win("17500Coin");
            a = 17500;
            b = "Coin";
            GManager.instance.AddCoinByLuckyWheel(17500);
            Debug.Log("Rot " + adjustedRot);
        }
        else if (adjustedRot >= 135 && adjustedRot < 180)
        {
            Win("500Diamond");
            a = 500;
            b = "Diamond";
            GManager.instance.AddDiamondByLuckyWheel(500);
            Debug.Log("Rot " + adjustedRot);
        }
        else if (adjustedRot >= 180 && adjustedRot < 225)
        {
            Win("5000Coin");
            a = 5000;
            b = "Coin";
            GManager.instance.AddCoinByLuckyWheel(5000);
            Debug.Log("Rot " + adjustedRot);
        }
        else if (adjustedRot >= 225 && adjustedRot < 270)
        {
            Win("1000Diamond");
            a = 1000;
            b = "Diamond";
            GManager.instance.AddDiamondByLuckyWheel(1000);
            Debug.Log("Rot " + adjustedRot);
        }
        else if (adjustedRot >= 270 && adjustedRot < 315)
        {
            Win("7500Coin");
            a = 7500;
            b = "Coin";
            GManager.instance.AddCoinByLuckyWheel(7500);
            Debug.Log("Rot " + adjustedRot);
        }
        else // 315 <= adjustedRot < 360
        {
            Win("750Diamond");
            a = 750;
            b = "Diamond";
            GManager.instance.AddDiamondByLuckyWheel(750);
            Debug.Log("Rot " + adjustedRot);
        }

        StartCoroutine(WaitAndPrint(1f, rawRot, a , b));
        Settings.instance.isSpinning = false;
    }

    public void Win(string Score)
    {
        print("You Win " + Score);
        Settings.instance.resultText.text = "Waiting...";
        Settings.instance.isCountingDown = true;
        Settings.instance.StartCountdown();
    }

    IEnumerator WaitAndPrint(float waitTime, float rot , int a , string b)
    {
        yield return new WaitForSeconds(waitTime);
        AudioManager.instance.PlaySound(AudioManager.instance.rewardLuckyWheelSoundClip);
        Settings.instance.pannel.gameObject.SetActive(true);
        if (b == "Coin")
        {
            Settings.instance.prizeImage.sprite = prizeSpriteImageCoin;
            Settings.instance.prizeText.text =  a + "";
        }
        else if (b == "Diamond")
        {
            Settings.instance.prizeImage.sprite = prizeSpriteCoin;
            Settings.instance.prizeText.text = a + "";
            PlayerPrefs.Save();
        }
    }


    IEnumerator StopWheel(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

}
