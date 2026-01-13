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
    public int[] normalLuckValues = { 10000, 7500, 3000, 5000 };
    public int[] superLuckValues = { 25000, 35000, 15000, 20000 };
    public int[] ultraLuckValues = { 50000, 75000, 30000, 40000 };
    private Rigidbody2D rbody;
    public Sprite prizeSpriteImageCoin;
    public Sprite prizeSpriteCoin;
    
    int inRotate;
    public void Awake()
    {
        instance = this;
        rbody = GetComponent<Rigidbody2D>();
          ValidateArrays();
    }
    void Start()
    {
          ValidateArrays();
        // Gọi sau khi Settings.instance đã được khởi tạo
        if (Settings.instance != null)
        {
            int a = PlayerPrefs.GetInt("isLuckyWheel", 0);
            if (a == 1)
            {
                normalLuckValues = superLuckValues;
            }
            else if (a == 2)
            {
                normalLuckValues = ultraLuckValues;
            }
            else
            {
                normalLuckValues = new int[] { 12500, 17500, 5000, 7500 };
            }
        }
    }
    float t;

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

        if(Settings.instance.isSpinning)
        {
           CheckPlane.instance.ResetActiveLuckyWheel();
        }
        else
        {
           CheckPlane.instance.SetActiveLuckyWheel();
        }
        
    }
    public void checkValueLuckyWheel()
    {
         ValidateArrays();
        if (superLuckValues == null || superLuckValues.Length < 4)
        {
            superLuckValues = new int[] { 25000, 35000, 15000, 20000 };
            // Debug.LogWarning("superLuckValues was null or invalid, reinitializing...");
        }
        
        if (ultraLuckValues == null || ultraLuckValues.Length < 4)
        {
            ultraLuckValues = new int[] { 50000, 75000, 30000, 40000 };
            // Debug.LogWarning("ultraLuckValues was null or invalid, reinitializing...");
        }
        
        if (GManager.instance.moneyFuel > 50000 || GManager.instance.moneyBoost > 50000 || GManager.instance.moneyPower > 50000)
        {
            normalLuckValues = new int[] { ultraLuckValues[0], ultraLuckValues[1], ultraLuckValues[2], ultraLuckValues[3] };
            PlayerPrefs.SetInt("isLuckyWheel", 2);
            PlayerPrefs.Save();
            // Debug.Log("Lucky Wheel set to ULTRA LUCK: [" + string.Join(", ", normalLuckValues) + "]");
            ChangeTextValueLuckyWheel();
        }
        else if (GManager.instance.moneyFuel > 20000 || GManager.instance.moneyBoost > 20000 || GManager.instance.moneyPower > 20000)
        {
            normalLuckValues = new int[] { superLuckValues[0], superLuckValues[1], superLuckValues[2], superLuckValues[3] };
            PlayerPrefs.SetInt("isLuckyWheel", 1);
            PlayerPrefs.Save();
            Debug.Log("Lucky Wheel set to SUPER LUCK: [" + string.Join(", ", normalLuckValues) + "]");
            ChangeTextValueLuckyWheel();
        }
        else
        {
            normalLuckValues = new int[] { 12500, 17500, 5000, 7500 };
            PlayerPrefs.SetInt("isLuckyWheel", 0);
            PlayerPrefs.Save();
            // Debug.Log("Lucky Wheel set to NORMAL LUCK: [12500, 17500, 5000, 7500]");
            ChangeTextValueLuckyWheel();
        }
    }

    public void ChangeTextValueLuckyWheel()
    {
        if (Settings.instance == null)
        {
            Debug.LogWarning("Settings.instance is null, skipping ChangeTextValueLuckyWheel");
            return;
        }
        
        if (normalLuckValues == null || normalLuckValues.Length < 4)
        {
            Debug.LogError("normalLuckValues is null or has insufficient elements!");
            return;
        }
        
        Settings.instance.value1Text.text = normalLuckValues[0] + "";
        Settings.instance.value2Text.text = normalLuckValues[1] + "";
        Settings.instance.value3Text.text = normalLuckValues[2] + "";
        Settings.instance.value4Text.text = normalLuckValues[3] + "";
    }


    public void Rotate()
    {
        Debug.Log("Rotate called. isSpinning: " + Settings.instance.isSpinning + ", resultText: " + Settings.instance.resultText.text);
        
        if (Settings.instance.isSpinning && Settings.instance.resultText.text == "Spin")
        {
            AudioManager.instance.PlaySound(AudioManager.instance.luckyWheelSoundClip);
            // RotatePower = Random.Range(minRotatePower, maxRotatePower);

            // StopPower = Random.Range(minStopPower, maxStopPower);
            
            if (inRotate == 0)
            {
                rbody.AddTorque(RotatePower);
                inRotate = 1;
                Settings.instance.exitLeaderboardButton.gameObject.SetActive(false);

                GMAnalytics.LogEvent("spin_lucky_wheel", 2);
            }
        }
        else if (!Settings.instance.isSpinning)
        {
            Settings.instance.pannel.gameObject.SetActive(false);
        }
    }
    public float rot;

    public void GetReward()
    {
        int a; string b;
        if (!Settings.instance.isSpinning) return;

            if (normalLuckValues == null || normalLuckValues.Length < 4)
    {
        Debug.LogError("normalLuckValues is invalid in GetReward! Auto-fixing...");
        ValidateArrays();
        
        // Nếu vẫn lỗi, dừng function
        if (normalLuckValues == null || normalLuckValues.Length < 4)
        {
            Debug.LogError("Failed to fix normalLuckValues! Stopping GetReward.");
            return;
        }
    }

        float rawRot = transform.eulerAngles.z;
        float adjustedRot = ((transform.eulerAngles.z + 360f) % 360f) % 360f;
        StartCoroutine(StopWheel(2f));
        if (adjustedRot >= 0 && adjustedRot < 45)
        {
            Win("12500Coin");
            a = normalLuckValues[0];
            b = "Coin";
            GManager.instance.AddCoinByLuckyWheel(normalLuckValues[0]);
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
            a = normalLuckValues[1];
            b = "Coin";
            GManager.instance.AddCoinByLuckyWheel(normalLuckValues[1]);
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
            a = normalLuckValues[2];
            b = "Coin";
            GManager.instance.AddCoinByLuckyWheel(normalLuckValues[2]);
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
            a = normalLuckValues[3];
            b = "Coin";
            GManager.instance.AddCoinByLuckyWheel(normalLuckValues[3]);
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

        StartCoroutine(WaitAndPrint(0f, rawRot, a , b));
        Settings.instance.isSpinning = false;

    }

    public void Win(string Score)
    {
        print("You Win " + Score);
        
        Settings.instance.currentTime = 600;
        PlayerPrefs.SetInt("SaveTime", Settings.instance.currentTime);
        PlayerPrefs.Save();
        
        Settings.instance.isSpinning = false;
        Settings.instance.StartCountdown(); 
        StartCoroutine(DelayTwoSeconds(2f));
        Debug.Log("Win - Countdown started with 600 seconds");
        Settings.instance.exitLeaderboardButton.gameObject.SetActive(true);
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

    IEnumerator DelayTwoSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        Settings.instance.pannel.gameObject.SetActive(false);
    }

    void ValidateArrays()
    {
        if (superLuckValues == null || superLuckValues.Length < 4)
        {
            superLuckValues = new int[] { 25000, 35000, 15000, 20000 };
            Debug.LogWarning("superLuckValues was invalid, reinitialized");
        }
        
        if (ultraLuckValues == null || ultraLuckValues.Length < 4)
        {
            ultraLuckValues = new int[] { 50000, 75000, 30000, 40000 };
            Debug.LogWarning("ultraLuckValues was invalid, reinitialized");
        }
        
        if (normalLuckValues == null || normalLuckValues.Length < 4)
        {
            normalLuckValues = new int[] { 12500, 17500, 5000, 7500 };
            Debug.LogWarning("normalLuckValues was invalid, reinitialized");
        }
    }

}
