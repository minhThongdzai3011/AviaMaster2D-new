using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


public class MissionAchievement : MonoBehaviour
{
    public static MissionAchievement instance;
    [Header("Mission Achievement Text")]
    public TextMeshProUGUI textAchievementMission1;
    public TextMeshProUGUI textAchievementMission2;
    public TextMeshProUGUI textAchievementMission3;
    public TextMeshProUGUI textAchievementMission4;
    public TextMeshProUGUI textAchievementMission5;

    [Header("Mission Achievement Image")]
    public Image imageAchievementMission1;
    public Image imageAchievementMission2;
    public Image imageAchievementMission3;
    public Image imageAchievementMission4;
    public Image imageAchievementMission5;

    [Header("Mission Achievement Image Fill")]
    public Image imageFillAchievementMission1;
    public Image imageFillAchievementMission2;
    public Image imageFillAchievementMission3;
    public Image imageFillAchievementMission4;
    public Image imageFillAchievementMission5;

    [Header("Mission Achievement Button")]
    public Button buttonAchievementMission1;
    public Button buttonAchievementMission2;
    public Button buttonAchievementMission3;
    public Button buttonAchievementMission4;
    public Button buttonAchievementMission5;
    [Header("Mission Achievement")]
    public bool isAchievementMission1Completed = false;
    public bool isAchievementMission2Completed = false;
    public bool isAchievementMission3Completed = false;
    public bool isAchievementMission4Completed = false;
    public bool isAchievementMission5Completed = false;
    
    [Header("Current Levels (Array Index)")]
    public int achievementMission1CurrentLevel = 0;
    public int achievementMission2CurrentLevel = 0;
    public int achievementMission3CurrentLevel = 0;
    public int achievementMission4CurrentLevel = 0;
    public int achievementMission5CurrentLevel = 0;
    
    public int achievementMission1Progress = 0;
    public int[] achievementMission1Target = {100000, 500000, 1000000, 5000000, 10000000};
    public int achievementMission2Progress = 0;
    public int[] achievementMission2Target = {3, 5 , 7, 9 , 15};
    public int achievementMission3Progress = 0;
    public int[] achievementMission3Target = {10, 50, 120, 300, 500};
    public int achievementMission4Progress = 0;
    public int[] achievementMission4Target = {10, 30, 120, 360, 1000};
    public int achievementMission5Progress = 0;
    public int[] achievementMission5Target = {2, 3, 4, 5, 6};
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
        UpdateAchievementMission();

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
            ResetAchievementMissions();
        }
        else
        {
            // Hiển thị theo định dạng HH:mm:ss
            countdownText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                timeRemaining.Hours, timeRemaining.Minutes, timeRemaining.Seconds);
        }

    }

    public void UpdateAchievementMission()
    {
        // Achievement Mission 1
        if (achievementMission1CurrentLevel < achievementMission1Target.Length)
        {
            if (!isAchievementMission1Completed)
            {
                imageFillAchievementMission1.fillAmount = (float)achievementMission1Progress / achievementMission1Target[achievementMission1CurrentLevel];
                Debug.Log("Achievement Mission 1 Progress: " + imageFillAchievementMission1.fillAmount);
                textAchievementMission1.text = achievementMission1Progress + "/" + achievementMission1Target[achievementMission1CurrentLevel];

                if (achievementMission1Progress >= achievementMission1Target[achievementMission1CurrentLevel])
                {
                    isAchievementMission1Completed = true;
                    buttonAchievementMission1.interactable = true;
                    textAchievementMission1.text = "Mission Completed";
                    imageAchievementMission1.color = Color.yellow;
                    buttonAchievementMission1.image.color = Color.yellow;
                }
            }
        }
        else
        {
            // Max level reached - set gray
            textAchievementMission1.text = "MAX LEVEL";
            imageAchievementMission1.color = Color.gray;
            buttonAchievementMission1.image.color = Color.gray;
            buttonAchievementMission1.interactable = false;
            imageFillAchievementMission1.fillAmount = 1f;
        }

        // Achievement Mission 2
        if (achievementMission2CurrentLevel < achievementMission2Target.Length)
        {
            if (!isAchievementMission2Completed)
            {
                imageFillAchievementMission2.fillAmount = (float)achievementMission2Progress / achievementMission2Target[achievementMission2CurrentLevel];
                Debug.Log("Achievement Mission 2 Progress: " + imageFillAchievementMission2.fillAmount);
                textAchievementMission2.text = achievementMission2Progress + "/" + achievementMission2Target[achievementMission2CurrentLevel];

                if (achievementMission2Progress >= achievementMission2Target[achievementMission2CurrentLevel])
                {
                    isAchievementMission2Completed = true;
                    buttonAchievementMission2.interactable = true;
                    textAchievementMission2.text = "Mission Completed";
                    imageAchievementMission2.color = Color.yellow;
                    buttonAchievementMission2.image.color = Color.yellow;
                }
            }
        }
        else
        {
            textAchievementMission2.text = "MAX LEVEL";
            imageAchievementMission2.color = Color.gray;
            buttonAchievementMission2.image.color = Color.gray;
            buttonAchievementMission2.interactable = false;
            imageFillAchievementMission2.fillAmount = 1f;
        }

        // Achievement Mission 3
        if (achievementMission3CurrentLevel < achievementMission3Target.Length)
        {
            if (!isAchievementMission3Completed)
            {
                imageFillAchievementMission3.fillAmount = (float)achievementMission3Progress / achievementMission3Target[achievementMission3CurrentLevel];
                Debug.Log("Achievement Mission 3 Progress: " + imageFillAchievementMission3.fillAmount);
                textAchievementMission3.text = achievementMission3Progress + "/" + achievementMission3Target[achievementMission3CurrentLevel];

                if (achievementMission3Progress >= achievementMission3Target[achievementMission3CurrentLevel])
                {
                    isAchievementMission3Completed = true;
                    buttonAchievementMission3.interactable = true;
                    textAchievementMission3.text = "Mission Completed";
                    imageAchievementMission3.color = Color.yellow;
                    buttonAchievementMission3.image.color = Color.yellow;
                }
            }
        }
        else
        {
            textAchievementMission3.text = "MAX LEVEL";
            imageAchievementMission3.color = Color.gray;
            buttonAchievementMission3.image.color = Color.gray;
            buttonAchievementMission3.interactable = false;
            imageFillAchievementMission3.fillAmount = 1f;
        }

        // Achievement Mission 4
        if (achievementMission4CurrentLevel < achievementMission4Target.Length)
        {
            if (!isAchievementMission4Completed)
            {
                imageFillAchievementMission4.fillAmount = (float)achievementMission4Progress / achievementMission4Target[achievementMission4CurrentLevel];
                Debug.Log("Achievement Mission 4 Progress: " + imageFillAchievementMission4.fillAmount);
                textAchievementMission4.text = achievementMission4Progress + "/" + achievementMission4Target[achievementMission4CurrentLevel];

                if (achievementMission4Progress >= achievementMission4Target[achievementMission4CurrentLevel])
                {
                    isAchievementMission4Completed = true;
                    buttonAchievementMission4.interactable = true;
                    textAchievementMission4.text = "Mission Completed";
                    imageAchievementMission4.color = Color.yellow;
                    buttonAchievementMission4.image.color = Color.yellow;
                }
            }
        }
        else
        {
            textAchievementMission4.text = "MAX LEVEL";
            imageAchievementMission4.color = Color.gray;
            buttonAchievementMission4.image.color = Color.gray;
            buttonAchievementMission4.interactable = false;
            imageFillAchievementMission4.fillAmount = 1f;
        }

        // Achievement Mission 5
        if (achievementMission5CurrentLevel < achievementMission5Target.Length)
        {
            if (!isAchievementMission5Completed)
            {
                imageFillAchievementMission5.fillAmount = (float)achievementMission5Progress / achievementMission5Target[achievementMission5CurrentLevel];
                Debug.Log("Achievement Mission 5 Progress: " + imageFillAchievementMission5.fillAmount);
                textAchievementMission5.text = achievementMission5Progress + "/" + achievementMission5Target[achievementMission5CurrentLevel];

                if (achievementMission5Progress >= achievementMission5Target[achievementMission5CurrentLevel])
                {
                    isAchievementMission5Completed = true;
                    buttonAchievementMission5.interactable = true;
                    textAchievementMission5.text = "Mission Completed";
                    imageAchievementMission5.color = Color.yellow;
                    buttonAchievementMission5.image.color = Color.yellow;
                }
            }
        }
        else
        {
            textAchievementMission5.text = "MAX LEVEL";
            imageAchievementMission5.color = Color.gray;
            buttonAchievementMission5.image.color = Color.gray;
            buttonAchievementMission5.interactable = false;
            imageFillAchievementMission5.fillAmount = 1f;
        }
        
        Save();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("AchievementMission1Progress", achievementMission1Progress);
        PlayerPrefs.SetInt("AchievementMission2Progress", achievementMission2Progress);
        PlayerPrefs.SetInt("AchievementMission3Progress", achievementMission3Progress);
        PlayerPrefs.SetInt("AchievementMission4Progress", achievementMission4Progress);
        PlayerPrefs.SetInt("AchievementMission5Progress", achievementMission5Progress);
        
        PlayerPrefs.SetInt("AchievementMission1Completed", isAchievementMission1Completed ? 1 : 0);
        PlayerPrefs.SetInt("AchievementMission2Completed", isAchievementMission2Completed ? 1 : 0);
        PlayerPrefs.SetInt("AchievementMission3Completed", isAchievementMission3Completed ? 1 : 0);
        PlayerPrefs.SetInt("AchievementMission4Completed", isAchievementMission4Completed ? 1 : 0);
        PlayerPrefs.SetInt("AchievementMission5Completed", isAchievementMission5Completed ? 1 : 0);
        
        PlayerPrefs.SetInt("AchievementMission1CurrentLevel", achievementMission1CurrentLevel);
        PlayerPrefs.SetInt("AchievementMission2CurrentLevel", achievementMission2CurrentLevel);
        PlayerPrefs.SetInt("AchievementMission3CurrentLevel", achievementMission3CurrentLevel);
        PlayerPrefs.SetInt("AchievementMission4CurrentLevel", achievementMission4CurrentLevel);
        PlayerPrefs.SetInt("AchievementMission5CurrentLevel", achievementMission5CurrentLevel);
    }

    public void Load()
    {
        achievementMission1Progress = PlayerPrefs.GetInt("AchievementMission1Progress", 0);
        achievementMission2Progress = PlayerPrefs.GetInt("AchievementMission2Progress", 0);
        achievementMission3Progress = PlayerPrefs.GetInt("AchievementMission3Progress", 0);
        achievementMission4Progress = PlayerPrefs.GetInt("AchievementMission4Progress", 0);
        achievementMission5Progress = PlayerPrefs.GetInt("AchievementMission5Progress", 0);
        
        isAchievementMission1Completed = PlayerPrefs.GetInt("AchievementMission1Completed", 0) == 1;
        isAchievementMission2Completed = PlayerPrefs.GetInt("AchievementMission2Completed", 0) == 1;
        isAchievementMission3Completed = PlayerPrefs.GetInt("AchievementMission3Completed", 0) == 1;
        isAchievementMission4Completed = PlayerPrefs.GetInt("AchievementMission4Completed", 0) == 1;
        isAchievementMission5Completed = PlayerPrefs.GetInt("AchievementMission5Completed", 0) == 1;
        
        achievementMission1CurrentLevel = PlayerPrefs.GetInt("AchievementMission1CurrentLevel", 0);
        achievementMission2CurrentLevel = PlayerPrefs.GetInt("AchievementMission2CurrentLevel", 0);
        achievementMission3CurrentLevel = PlayerPrefs.GetInt("AchievementMission3CurrentLevel", 0);
        achievementMission4CurrentLevel = PlayerPrefs.GetInt("AchievementMission4CurrentLevel", 0);
        achievementMission5CurrentLevel = PlayerPrefs.GetInt("AchievementMission5CurrentLevel", 0);
    }

    public void ResetAchievementMissions()
    {
        achievementMission1Progress = 0;
        achievementMission2Progress = 0;
        achievementMission3Progress = 0;
        achievementMission4Progress = 0;
        achievementMission5Progress = 0;

        isAchievementMission1Completed = false;
        isAchievementMission2Completed = false;
        isAchievementMission3Completed = false;
        isAchievementMission4Completed = false;
        isAchievementMission5Completed = false;
        
        achievementMission1CurrentLevel = 0;
        achievementMission2CurrentLevel = 0;
        achievementMission3CurrentLevel = 0;
        achievementMission4CurrentLevel = 0;
        achievementMission5CurrentLevel = 0;

        buttonAchievementMission1.interactable = false;
        buttonAchievementMission2.interactable = false;
        buttonAchievementMission3.interactable = false;
        buttonAchievementMission4.interactable = false;
        buttonAchievementMission5.interactable = false;

        imageAchievementMission1.color = Color.gray;
        imageAchievementMission2.color = Color.gray;
        imageAchievementMission3.color = Color.gray;
        imageAchievementMission4.color = Color.gray;
        imageAchievementMission5.color = Color.gray;

        buttonAchievementMission1.image.color = Color.white;
        buttonAchievementMission2.image.color = Color.white;
        buttonAchievementMission3.image.color = Color.white;
        buttonAchievementMission4.image.color = Color.white;
        buttonAchievementMission5.image.color = Color.white;

        UpdateAchievementMission();
    }

    public void buttonMission1ClaimReward()
    {
        if (isAchievementMission1Completed && achievementMission1CurrentLevel < achievementMission1Target.Length)
        {
            Debug.Log("Achievement Mission 1 Reward Claimed! Level: " + (achievementMission1CurrentLevel + 1));
            
            // Tăng level (index) lên 1
            achievementMission1CurrentLevel++;
            
            // Reset cho level mới
            isAchievementMission1Completed = false;
            achievementMission1Progress = 0;
            buttonAchievementMission1.interactable = false;
            imageAchievementMission1.color = Color.gray;
            buttonAchievementMission1.image.color = Color.gray;
            
            UpdateAchievementMission();
        }
    }
    public void buttonMission2ClaimReward()
    {
        if (isAchievementMission2Completed && achievementMission2CurrentLevel < achievementMission2Target.Length)
        {
            Debug.Log("Achievement Mission 2 Reward Claimed! Level: " + (achievementMission2CurrentLevel + 1));
            
            achievementMission2CurrentLevel++;
            isAchievementMission2Completed = false;
            achievementMission2Progress = 0;
            buttonAchievementMission2.interactable = false;
            imageAchievementMission2.color = Color.gray;
            buttonAchievementMission2.image.color = Color.gray;
            
            UpdateAchievementMission();
        }
    }
    public void buttonMission3ClaimReward()
    {
        if (isAchievementMission3Completed && achievementMission3CurrentLevel < achievementMission3Target.Length)
        {
            Debug.Log("Achievement Mission 3 Reward Claimed! Level: " + (achievementMission3CurrentLevel + 1));
            
            achievementMission3CurrentLevel++;
            isAchievementMission3Completed = false;
            achievementMission3Progress = 0;
            buttonAchievementMission3.interactable = false;
            imageAchievementMission3.color = Color.gray;
            buttonAchievementMission3.image.color = Color.gray;
            
            UpdateAchievementMission();
        }
    }
    public void buttonMission4ClaimReward()
    {
        if (isAchievementMission4Completed && achievementMission4CurrentLevel < achievementMission4Target.Length)
        {
            Debug.Log("Achievement Mission 4 Reward Claimed! Level: " + (achievementMission4CurrentLevel + 1));
            
            achievementMission4CurrentLevel++;
            isAchievementMission4Completed = false;
            achievementMission4Progress = 0;
            buttonAchievementMission4.interactable = false;
            imageAchievementMission4.color = Color.gray;
            buttonAchievementMission4.image.color = Color.gray;
            
            UpdateAchievementMission();
        }
    }
    public void buttonMission5ClaimReward()
    {
        if (isAchievementMission5Completed && achievementMission5CurrentLevel < achievementMission5Target.Length)
        {
            Debug.Log("Achievement Mission 5 Reward Claimed! Level: " + (achievementMission5CurrentLevel + 1));
            
            achievementMission5CurrentLevel++;
            isAchievementMission5Completed = false;
            achievementMission5Progress = 0;
            buttonAchievementMission5.interactable = false;
            imageAchievementMission5.color = Color.gray;
            buttonAchievementMission5.image.color = Color.gray;
            
            UpdateAchievementMission();
        }
    }

}
