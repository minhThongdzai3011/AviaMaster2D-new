using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


public class MissionDaily : MonoBehaviour
{
    public static MissionDaily instance;
    [Header("Mission Daily Text")]
    public TextMeshProUGUI textDailyMission1;
    public TextMeshProUGUI textDailyMission2;
    public TextMeshProUGUI textDailyMission3;
    public TextMeshProUGUI textDailyMission4;
    public TextMeshProUGUI textDailyMission5;

    [Header("Mission Daily Image")]
    public Image imageDailyMission1;
    public Image imageDailyMission2;
    public Image imageDailyMission3;
    public Image imageDailyMission4;
    public Image imageDailyMission5;

    [Header("Mission Daily Image Fill")]
    public Image imageFillDailyMission1;
    public Image imageFillDailyMission2;
    public Image imageFillDailyMission3;
    public Image imageFillDailyMission4;
    public Image imageFillDailyMission5;

    [Header("Mission Daily Button")]
    public Button buttonDailyMission1;
    public Button buttonDailyMission2;
    public Button buttonDailyMission3;
    public Button buttonDailyMission4;
    public Button buttonDailyMission5;
    [Header("Mission Daily")]
    public bool isDailyMission1Completed = false;
    public bool isDailyMission2Completed = false;
    public bool isDailyMission3Completed = false;
    public bool isDailyMission4Completed = false;
    public bool isDailyMission5Completed = false;
    public int dailyMission1Progress = 0;
    public int dailyMission1Target = 4;
    public int dailyMission2Progress = 0;
    public int dailyMission2Target = 500;
    public int dailyMission3Progress = 0;
    public int dailyMission3Target = 3;
    public int dailyMission4Progress = 0;
    public int dailyMission4Target = 1;
    public int dailyMission5Progress = 0;
    public int dailyMission5Target = 1;
    private DateTime endOfDay;
    public TextMeshProUGUI countdownText;


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);    
        Load();
        UpdateDailyMission();

        DateTime now = DateTime.Now;
        endOfDay = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);


    }

    // Update is called once per frame
    void Update()
    {
        DateTime now = DateTime.Now;
        TimeSpan timeRemaining = endOfDay - now;

        if (timeRemaining.TotalSeconds <= 0)
        {
            countdownText.text = "00:00:00";
            ResetDailyMissions();
        }
        else
        {
            // Hiển thị theo định dạng HH:mm:ss
            countdownText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                timeRemaining.Hours, timeRemaining.Minutes, timeRemaining.Seconds);
        }

    }

    public void UpdateDailyMission()
    {
        if (!isDailyMission1Completed)
        {
            imageFillDailyMission1.fillAmount = (float)dailyMission1Progress / dailyMission1Target;
            Debug.Log("Daily Mission 1 Progress: " + imageFillDailyMission1.fillAmount);
            textDailyMission1.text =dailyMission1Progress + "/" + dailyMission1Target;

            if (dailyMission1Progress >= dailyMission1Target)
            {
                isDailyMission1Completed = true;
                buttonDailyMission1.interactable = true;
                textDailyMission1.text = "Mission Completed";
                imageDailyMission1.color = Color.yellow;
                buttonDailyMission1.image.color = Color.yellow;
            }
        }
        if (!isDailyMission2Completed)
        {
            imageFillDailyMission2.fillAmount = (float)dailyMission2Progress / dailyMission2Target;
            Debug.Log("Daily Mission 2 Progress: " + imageFillDailyMission2.fillAmount);
            textDailyMission2.text = dailyMission2Progress + "/" + dailyMission2Target;

            if (dailyMission2Progress >= dailyMission2Target)
            {
                isDailyMission2Completed = true;
                buttonDailyMission2.interactable = true;
                textDailyMission2.text = "Mission Completed";
                imageDailyMission2.color = Color.yellow;
                buttonDailyMission2.image.color = Color.yellow;
                dailyMission1Progress++;
                UpdateDailyMission();
            }
        }
        if (!isDailyMission3Completed)
        {
            imageFillDailyMission3.fillAmount = (float)dailyMission3Progress / dailyMission3Target;
           // Debug.Break(); tạm dừng bên Unity để kiểm tra giá trị
            Debug.Log("Daily Mission 3 Progress: " + imageFillDailyMission3.fillAmount);
            textDailyMission3.text = dailyMission3Progress + "/" + dailyMission3Target;

            if (dailyMission3Progress >= dailyMission3Target)
            {
                isDailyMission3Completed = true;
                buttonDailyMission3.interactable = true;
                textDailyMission3.text = "Mission Completed";
                imageDailyMission3.color = Color.yellow;
                buttonDailyMission3.image.color = Color.yellow;
                dailyMission1Progress++;
                UpdateDailyMission();
            }
        }
        if (!isDailyMission4Completed)
        {
            imageFillDailyMission4.fillAmount = (float)dailyMission4Progress / dailyMission4Target;
            Debug.Log("Daily Mission 4 Progress: " + imageFillDailyMission4.fillAmount);
            textDailyMission4.text = dailyMission4Progress + "/" + dailyMission4Target;

            if (dailyMission4Progress >= dailyMission4Target)
            {
                isDailyMission4Completed = true;
                buttonDailyMission4.interactable = true;
                textDailyMission4.text = "Mission Completed";
                imageDailyMission4.color = Color.yellow;
                buttonDailyMission4.image.color = Color.yellow;
                dailyMission1Progress++;
                UpdateDailyMission();
            }
        }
        if (!isDailyMission5Completed)
        {
            imageFillDailyMission5.fillAmount = (float)dailyMission5Progress / dailyMission5Target;
            Debug.Log("Daily Mission 5 Progress: " + imageFillDailyMission5.fillAmount);
            textDailyMission5.text = dailyMission5Progress + "/" + dailyMission5Target;

            if (dailyMission5Progress >= dailyMission5Target)
            {
                isDailyMission5Completed = true;
                buttonDailyMission5.interactable = true;
                textDailyMission5.text = "Mission Completed";
                imageDailyMission5.color = Color.yellow;
                buttonDailyMission5.image.color = Color.yellow;
                dailyMission1Progress++;
                UpdateDailyMission();
            }
        }
        Save();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("DailyMission1Progress", dailyMission1Progress);
        PlayerPrefs.SetInt("DailyMission2Progress", dailyMission2Progress);
        PlayerPrefs.SetInt("DailyMission3Progress", dailyMission3Progress);
        PlayerPrefs.SetInt("DailyMission4Progress", dailyMission4Progress);
        PlayerPrefs.SetInt("DailyMission5Progress", dailyMission5Progress);
    }

    public void Load()
    {
        dailyMission1Progress = PlayerPrefs.GetInt("DailyMission1Progress", 0);
        dailyMission2Progress = PlayerPrefs.GetInt("DailyMission2Progress", 0);
        dailyMission3Progress = PlayerPrefs.GetInt("DailyMission3Progress", 0);
        dailyMission4Progress = PlayerPrefs.GetInt("DailyMission4Progress", 0);
        dailyMission5Progress = PlayerPrefs.GetInt("DailyMission5Progress", 0);
    }

    public void ResetDailyMissions()
    {
        dailyMission1Progress = 0;
        dailyMission2Progress = 0;
        dailyMission3Progress = 0;
        dailyMission4Progress = 0;
        dailyMission5Progress = 0;

        isDailyMission1Completed = false;
        isDailyMission2Completed = false;
        isDailyMission3Completed = false;
        isDailyMission4Completed = false;
        isDailyMission5Completed = false;

        buttonDailyMission1.interactable = false;
        buttonDailyMission2.interactable = false;
        buttonDailyMission3.interactable = false;
        buttonDailyMission4.interactable = false;
        buttonDailyMission5.interactable = false;

        imageDailyMission1.color = Color.gray;
        imageDailyMission2.color = Color.gray;
        imageDailyMission3.color = Color.gray;
        imageDailyMission4.color = Color.gray;
        imageDailyMission5.color = Color.gray;

        buttonDailyMission1.image.color = Color.white;
        buttonDailyMission2.image.color = Color.white;
        buttonDailyMission3.image.color = Color.white;
        buttonDailyMission4.image.color = Color.white;
        buttonDailyMission5.image.color = Color.white;

        UpdateDailyMission();
    }

    public void buttonMission1ClaimReward()
    {
        if (isDailyMission1Completed)
        {
            Debug.Log("Daily Mission 1 Reward Claimed!");
            isDailyMission1Completed = false;
            dailyMission1Progress = 0;
            buttonDailyMission1.interactable = false;
            imageDailyMission1.color = Color.gray;
            buttonDailyMission1.image.color = Color.gray;
            UpdateDailyMission();
        }
    }
    public void buttonMission2ClaimReward()
    {
        if (isDailyMission2Completed)
        {
            Debug.Log("Daily Mission 2 Reward Claimed!");
            isDailyMission2Completed = false;
            dailyMission2Progress = 0;
            buttonDailyMission2.interactable = false;
            imageDailyMission2.color = Color.gray;
            buttonDailyMission2.image.color = Color.gray;
            UpdateDailyMission();
        }
    }
    public void buttonMission3ClaimReward()
    {
        if (isDailyMission3Completed)
        {
            Debug.Log("Daily Mission 3 Reward Claimed!");
            isDailyMission3Completed = false;
            dailyMission3Progress = 0;
            buttonDailyMission3.interactable = false;
            imageDailyMission3.color = Color.gray;
            buttonDailyMission3.image.color = Color.gray;
            UpdateDailyMission();
        }
    }
    public void buttonMission4ClaimReward()
    {
        if (isDailyMission4Completed)
        {
            Debug.Log("Daily Mission 4 Reward Claimed!");
            isDailyMission4Completed = false;
            dailyMission4Progress = 0;
            buttonDailyMission4.interactable = false;
            imageDailyMission4.color = Color.gray;
            buttonDailyMission4.image.color = Color.gray;
            UpdateDailyMission();
        }
    }
    public void buttonMission5ClaimReward()
    {
        if (isDailyMission5Completed)
        {
            Debug.Log("Daily Mission 5 Reward Claimed!");
            isDailyMission5Completed = false;
            dailyMission5Progress = 0;
            buttonDailyMission5.interactable = false;
            imageDailyMission5.color = Color.gray;
            buttonDailyMission5.image.color = Color.gray;
            UpdateDailyMission();
        }
    }

}
