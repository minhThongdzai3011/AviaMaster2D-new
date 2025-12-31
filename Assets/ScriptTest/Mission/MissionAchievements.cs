using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


public class MissionAchievements : MonoBehaviour
{
    public static MissionAchievements instance;

    [Header("Mission Guide Achievements Text")]
    public TextMeshProUGUI textMissionGuideAchievement1;
    public TextMeshProUGUI textMissionGuideAchievement2;
    public TextMeshProUGUI textMissionGuideAchievement3;
    public TextMeshProUGUI textMissionGuideAchievement4;
    public TextMeshProUGUI textMissionGuideAchievement5;

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
    public Sprite spriteMissionIncomplete;
    public Sprite spriteMissionClaimed;

    [Header("Mission Achievement Image Fill")]
    public Image imageFillAchievementMission1;
    public Image imageFillAchievementMission2;
    public Image imageFillAchievementMission3;
    public Image imageFillAchievementMission4;
    public Image imageFillAchievementMission5;
    public Sprite spriteFillMissionIncomplete;
    public Sprite spriteFillMissionCompleted;

    [Header("Mission Plane Image Background Fill")]
    public Image imageBackGroundFillPlaneMission1;
    public Image imageBackGroundFillPlaneMission2;
    public Image imageBackGroundFillPlaneMission3;
    public Image imageBackGroundFillPlaneMission4;
    public Image imageBackGroundFillPlaneMission5;
    public Sprite spriteBackGroundFillMissionIncomplete;
    public Sprite spriteBackGroundFillMissionCompleted;

    [Header("Mission Achievement Button")]
    public Button buttonAchievementMission1;
    public Button buttonAchievementMission2;
    public Button buttonAchievementMission3;
    public Button buttonAchievementMission4;
    public Button buttonAchievementMission5;
    public Sprite spriteButtonIncomplete;
    public Sprite spriteButtonCompleted;
    public Sprite spriteButtonClaimed;

    [Header("Mission Achievement Bool")]
    public bool isAchievementMission1Completed = false;
    public bool isAchievementMission2Completed = false;
    public bool isAchievementMission3Completed = false;
    public bool isAchievementMission4Completed = false;
    public bool isAchievementMission5Completed = false;

    [Header("Prize Rewards Achievements")]
    public int[] prizeRewardAchievement1 = {1000, 2000, 3000, 4000, 5000};
    public int[] prizeRewardAchievement2 = {500, 1000, 2000, 3500, 5000};
    public int[] prizeRewardAchievement3 = {10000, 30000, 50000, 100000, 300000};
    public int[] prizeRewardAchievement4 = {1000, 2000, 3000, 4000, 5000};
    public int[] prizeRewardAchievement5 = {1000, 2000, 3000, 4000, 5000};
    public TextMeshProUGUI textPrizeRewardAchievement1;
    public TextMeshProUGUI textPrizeRewardAchievement2;
    public TextMeshProUGUI textPrizeRewardAchievement3;
    public TextMeshProUGUI textPrizeRewardAchievement4;
    public TextMeshProUGUI textPrizeRewardAchievement5;

    
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
    public int achievementMission5Progress = 1;
    public int[] achievementMission5Target = {2, 3, 4, 5, 6};

    public bool isReceivedAchievement1Reward = false;
    public bool isReceivedAchievement2Reward = false;
    public bool isReceivedAchievement3Reward = false;
    public bool isReceivedAchievement4Reward = false;
    public bool isReceivedAchievement5Reward = false;
    public bool isFalseButton1ClickedAchie = false;
    public bool isFalseButton2ClickedAchie = false;
    public bool isFalseButton3ClickedAchie = false;
    public bool isFalseButton4ClickedAchie = false;
    public bool isFalseButton5ClickedAchie = false;
    
    [Header("Flight Time Tracking")]
    public float flightStartTime;
    public float flightEndTime;
    public bool isFlightTimeActive = false;
    public float cumulativeFlightTimeSeconds = 0f; // Tích lũy thời gian bay tổng
    
    // Start is called before the first frame update
    void Awake()
    {
        // Ensure instance is set early to prevent null reference errors
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        Load();
        completeRewardCollection();
        UpdateAchievementMission();
    }

    // Helper method to format numbers (1000 -> 1k, 1000000 -> 1M)
    private string FormatNumber(int number)
    {
        if (number >= 1000000)
        {
            return (number / 1000000f).ToString("0.#") + "M";
        }
        else if (number >= 1000)
        {
            return (number / 1000f).ToString("0.#") + "k";
        }
        return number.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ResetAchievementMissions();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            achievementMission1Progress = achievementMission1Target[achievementMission1CurrentLevel];
            achievementMission2Progress = achievementMission2Target[achievementMission2CurrentLevel];
            achievementMission3Progress = achievementMission3Target[achievementMission3CurrentLevel];
            achievementMission4Progress = achievementMission4Target[achievementMission4CurrentLevel];
            achievementMission5Progress = achievementMission5Target[achievementMission5CurrentLevel];
            UpdateAchievementMission();
        }
    }

    public void UpdateAchievementMission()
    {
        // Update Mission Guide Texts
        if (achievementMission1CurrentLevel < achievementMission1Target.Length)
        {
            textMissionGuideAchievement1.text = "Spend " + FormatNumber(achievementMission1Target[achievementMission1CurrentLevel]) + " coins";
            textPrizeRewardAchievement1.text = FormatNumber(prizeRewardAchievement1[achievementMission1CurrentLevel]);
        }
        
        if (achievementMission2CurrentLevel < achievementMission2Target.Length)
        {
            textMissionGuideAchievement2.text = "Own " + achievementMission2Target[achievementMission2CurrentLevel] + " airplanes";
            textPrizeRewardAchievement2.text = FormatNumber(prizeRewardAchievement2[achievementMission2CurrentLevel]);
        }
        
        if (achievementMission3CurrentLevel < achievementMission3Target.Length)
        {
            textMissionGuideAchievement3.text = "Land at airport " + achievementMission3Target[achievementMission3CurrentLevel] + " times";
            textPrizeRewardAchievement3.text = FormatNumber(prizeRewardAchievement3[achievementMission3CurrentLevel]);
        }
        
        if (achievementMission4CurrentLevel < achievementMission4Target.Length)
        {
            textMissionGuideAchievement4.text = "Play for " + achievementMission4Target[achievementMission4CurrentLevel] + " minutes";
            textPrizeRewardAchievement4.text = FormatNumber(prizeRewardAchievement4[achievementMission4CurrentLevel]);
        }
        
        if (achievementMission5CurrentLevel < achievementMission5Target.Length)
        {
            textMissionGuideAchievement5.text = "Unlock " + achievementMission5Target[achievementMission5CurrentLevel] + " maps";
            textPrizeRewardAchievement5.text = FormatNumber(prizeRewardAchievement5[achievementMission5CurrentLevel]);
        }
        
        // Achievement Mission 1
        if (achievementMission1CurrentLevel < achievementMission1Target.Length)
        {
            if (!isAchievementMission1Completed && !isReceivedAchievement1Reward)
            {
                imageFillAchievementMission1.fillAmount = (float)achievementMission1Progress / achievementMission1Target[achievementMission1CurrentLevel];
                Debug.Log("Achievement Mission 1 Progress: " + imageFillAchievementMission1.fillAmount);
                textAchievementMission1.text = achievementMission1Progress + "/" + achievementMission1Target[achievementMission1CurrentLevel];

                if (achievementMission1Progress >= achievementMission1Target[achievementMission1CurrentLevel])
                {
                    isAchievementMission1Completed = true;
                    buttonAchievementMission1.interactable = true;
                    textAchievementMission1.text = "Mission Completed";
                    buttonAchievementMission1.image.sprite = spriteButtonCompleted;
                    
                    MissionManager.instance.textQuantityRewardValue++;
                    MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                    MissionManager.instance.notificationImage.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            // Max level reached - set gray
            textAchievementMission1.text = "MAX LEVEL";
            imageAchievementMission1.sprite = spriteMissionClaimed;
            buttonAchievementMission1.image.sprite = spriteButtonClaimed;
            buttonAchievementMission1.interactable = false;
            imageFillAchievementMission1.fillAmount = 1f;
        }

        // Achievement Mission 2
        if (achievementMission2CurrentLevel < achievementMission2Target.Length)
        {
            if (!isAchievementMission2Completed && !isReceivedAchievement2Reward)
            {
                imageFillAchievementMission2.fillAmount = (float)achievementMission2Progress / achievementMission2Target[achievementMission2CurrentLevel];
                Debug.Log("Achievement Mission 2 Progress: " + imageFillAchievementMission2.fillAmount);
                textAchievementMission2.text = achievementMission2Progress + "/" + achievementMission2Target[achievementMission2CurrentLevel];

                if (achievementMission2Progress >= achievementMission2Target[achievementMission2CurrentLevel])
                {
                    isAchievementMission2Completed = true;
                    buttonAchievementMission2.interactable = true;
                    textAchievementMission2.text = "Mission Completed";
                    buttonAchievementMission2.image.sprite = spriteButtonCompleted;
                    
                    MissionManager.instance.textQuantityRewardValue++;
                    MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                    MissionManager.instance.notificationImage.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            textAchievementMission2.text = "MAX LEVEL";
            imageAchievementMission2.sprite = spriteMissionClaimed;
            buttonAchievementMission2.image.sprite = spriteButtonClaimed;
            buttonAchievementMission2.interactable = false;
            imageFillAchievementMission2.fillAmount = 1f;
        }

        // Achievement Mission 3
        if (achievementMission3CurrentLevel < achievementMission3Target.Length)
        {
            if (!isAchievementMission3Completed && !isReceivedAchievement3Reward)
            {
                imageFillAchievementMission3.fillAmount = (float)achievementMission3Progress / achievementMission3Target[achievementMission3CurrentLevel];
                Debug.Log("Achievement Mission 3 Progress: " + imageFillAchievementMission3.fillAmount);
                textAchievementMission3.text = achievementMission3Progress + "/" + achievementMission3Target[achievementMission3CurrentLevel];

                if (achievementMission3Progress >= achievementMission3Target[achievementMission3CurrentLevel])
                {
                    isAchievementMission3Completed = true;
                    buttonAchievementMission3.interactable = true;
                    textAchievementMission3.text = "Mission Completed";
                    buttonAchievementMission3.image.sprite = spriteButtonCompleted;
                    
                    MissionManager.instance.textQuantityRewardValue++;
                    MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                    MissionManager.instance.notificationImage.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            textAchievementMission3.text = "MAX LEVEL";
            imageAchievementMission3.sprite = spriteMissionClaimed;
            buttonAchievementMission3.image.sprite = spriteButtonClaimed;
            buttonAchievementMission3.interactable = false;
            imageFillAchievementMission3.fillAmount = 1f;
        }

        // Achievement Mission 4
        if (achievementMission4CurrentLevel < achievementMission4Target.Length)
        {
            if (!isAchievementMission4Completed && !isReceivedAchievement4Reward)
            {
                // Convert progress back to decimal minutes for display
                float currentMinutes = achievementMission4Progress / 100f;
                int targetMinutes = achievementMission4Target[achievementMission4CurrentLevel];
                
                imageFillAchievementMission4.fillAmount = currentMinutes / targetMinutes;
                Debug.Log("Achievement Mission 4 Progress: " + imageFillAchievementMission4.fillAmount);
                textAchievementMission4.text = currentMinutes.ToString("F2") + "/" + targetMinutes;

                if (currentMinutes >= targetMinutes)
                {
                    isAchievementMission4Completed = true;
                    buttonAchievementMission4.interactable = true;
                    textAchievementMission4.text = "Mission Completed";
                    buttonAchievementMission4.image.sprite = spriteButtonCompleted;
                    
                    MissionManager.instance.textQuantityRewardValue++;
                    MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                    MissionManager.instance.notificationImage.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            textAchievementMission4.text = "MAX LEVEL";
            imageAchievementMission4.sprite = spriteMissionClaimed;
            buttonAchievementMission4.image.sprite = spriteButtonClaimed;
            buttonAchievementMission4.interactable = false;
            imageFillAchievementMission4.fillAmount = 1f;
        }

        // Achievement Mission 5
        if (achievementMission5CurrentLevel < achievementMission5Target.Length)
        {
            if (!isAchievementMission5Completed && !isReceivedAchievement5Reward)
            {
                imageFillAchievementMission5.fillAmount = (float)achievementMission5Progress / achievementMission5Target[achievementMission5CurrentLevel];
                Debug.Log("Achievement Mission 5 Progress: " + imageFillAchievementMission5.fillAmount);
                textAchievementMission5.text = achievementMission5Progress + "/" + achievementMission5Target[achievementMission5CurrentLevel];

                if (achievementMission5Progress >= achievementMission5Target[achievementMission5CurrentLevel])
                {
                    isAchievementMission5Completed = true;
                    buttonAchievementMission5.interactable = true;
                    textAchievementMission5.text = "Mission Completed";
                    buttonAchievementMission5.image.sprite = spriteButtonCompleted;
                    
                    MissionManager.instance.textQuantityRewardValue++;
                    MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                    MissionManager.instance.notificationImage.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            textAchievementMission5.text = "MAX LEVEL";
            imageAchievementMission5.sprite = spriteMissionClaimed;
            buttonAchievementMission5.image.sprite = spriteButtonClaimed;
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
        
        // Save cumulative flight time
        PlayerPrefs.SetFloat("CumulativeFlightTimeSeconds", cumulativeFlightTimeSeconds);
        
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
        
        PlayerPrefs.SetInt("IsReceivedAchievement1Reward", isReceivedAchievement1Reward ? 1 : 0);
        PlayerPrefs.SetInt("IsReceivedAchievement2Reward", isReceivedAchievement2Reward ? 1 : 0);
        PlayerPrefs.SetInt("IsReceivedAchievement3Reward", isReceivedAchievement3Reward ? 1 : 0);
        PlayerPrefs.SetInt("IsReceivedAchievement4Reward", isReceivedAchievement4Reward ? 1 : 0);
        PlayerPrefs.SetInt("IsReceivedAchievement5Reward", isReceivedAchievement5Reward ? 1 : 0);
    }

    public void Load()
    {
        achievementMission1Progress = PlayerPrefs.GetInt("AchievementMission1Progress", 0);
        achievementMission2Progress = PlayerPrefs.GetInt("AchievementMission2Progress", 0);
        achievementMission3Progress = PlayerPrefs.GetInt("AchievementMission3Progress", 0);
        achievementMission4Progress = PlayerPrefs.GetInt("AchievementMission4Progress", 0);
        achievementMission5Progress = PlayerPrefs.GetInt("AchievementMission5Progress", 1); // Default is 1 because City map is unlocked by default
        
        // Load cumulative flight time
        cumulativeFlightTimeSeconds = PlayerPrefs.GetFloat("CumulativeFlightTimeSeconds", 0f);
        
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
        
        isReceivedAchievement1Reward = PlayerPrefs.GetInt("IsReceivedAchievement1Reward", 0) == 1;
        isReceivedAchievement2Reward = PlayerPrefs.GetInt("IsReceivedAchievement2Reward", 0) == 1;
        isReceivedAchievement3Reward = PlayerPrefs.GetInt("IsReceivedAchievement3Reward", 0) == 1;
        isReceivedAchievement4Reward = PlayerPrefs.GetInt("IsReceivedAchievement4Reward", 0) == 1;
        isReceivedAchievement5Reward = PlayerPrefs.GetInt("IsReceivedAchievement5Reward", 0) == 1;
    }

    public void ResetAchievementMissions()
    {
        achievementMission1Progress = 0;
        achievementMission2Progress = 0;
        achievementMission3Progress = 0;
        achievementMission4Progress = 0;
        achievementMission5Progress = 0;
        
        // Reset cumulative flight time
        cumulativeFlightTimeSeconds = 0f;

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

        isFalseButton1ClickedAchie = false;
        isFalseButton2ClickedAchie = false;
        isFalseButton3ClickedAchie = false;
        isFalseButton4ClickedAchie = false;
        isFalseButton5ClickedAchie = false;

        isReceivedAchievement1Reward = false;
        isReceivedAchievement2Reward = false;
        isReceivedAchievement3Reward = false;
        isReceivedAchievement4Reward = false;
        isReceivedAchievement5Reward = false;

        imageAchievementMission1.sprite = spriteMissionIncomplete;
        imageAchievementMission2.sprite = spriteMissionIncomplete;
        imageAchievementMission3.sprite = spriteMissionIncomplete;
        imageAchievementMission4.sprite = spriteMissionIncomplete;
        imageAchievementMission5.sprite = spriteMissionIncomplete;

        imageFillAchievementMission1.sprite = spriteFillMissionIncomplete;
        imageFillAchievementMission2.sprite = spriteFillMissionIncomplete;
        imageFillAchievementMission3.sprite = spriteFillMissionIncomplete;
        imageFillAchievementMission4.sprite = spriteFillMissionIncomplete;
        imageFillAchievementMission5.sprite = spriteFillMissionIncomplete;

        buttonAchievementMission1.image.sprite = spriteButtonIncomplete;
        buttonAchievementMission2.image.sprite = spriteButtonIncomplete;
        buttonAchievementMission3.image.sprite = spriteButtonIncomplete;
        buttonAchievementMission4.image.sprite = spriteButtonIncomplete;
        buttonAchievementMission5.image.sprite = spriteButtonIncomplete;

        imageBackGroundFillPlaneMission1.sprite = spriteBackGroundFillMissionIncomplete;
        imageBackGroundFillPlaneMission2.sprite = spriteBackGroundFillMissionIncomplete;
        imageBackGroundFillPlaneMission3.sprite = spriteBackGroundFillMissionIncomplete;
        imageBackGroundFillPlaneMission4.sprite = spriteBackGroundFillMissionIncomplete;
        imageBackGroundFillPlaneMission5.sprite = spriteBackGroundFillMissionIncomplete;

        // Reset fillAmount
        imageFillAchievementMission1.fillAmount = 0f;
        imageFillAchievementMission2.fillAmount = 0f;
        imageFillAchievementMission3.fillAmount = 0f;
        imageFillAchievementMission4.fillAmount = 0f;
        imageFillAchievementMission5.fillAmount = 0f;

        UpdateAchievementMission();
        Save();
    }

    public void buttonMission1ClaimReward()
    {
        if (isAchievementMission1Completed && achievementMission1CurrentLevel < achievementMission1Target.Length && !isFalseButton1ClickedAchie)
        {
            Debug.Log("Achievement Mission 1 Reward Claimed! Level: " + (achievementMission1CurrentLevel + 1));
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            // Trao thưởng kim cương cho người chơi
            GManager.instance.totalDiamond += prizeRewardAchievement1[achievementMission1CurrentLevel];
            GManager.instance.totalDiamondText.text = GManager.instance.totalDiamond.ToString();
            PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
            PlayerPrefs.Save();
            GManager.instance.SaveTotalDiamond();
            
            // Giảm notification counter
            MissionManager.instance.textQuantityRewardValue--;
            MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
            if (MissionManager.instance.textQuantityRewardValue <= 0)
            {
                MissionManager.instance.notificationImage.gameObject.SetActive(false);
            }
            
            // Tăng level (index) lên 1
            achievementMission1CurrentLevel++;
            MissionPlane.instance.planeMission3Progress++;
            MissionPlane.instance.UpdatePlaneMission();
            
            // Reset cho level mới
            isAchievementMission1Completed = false;
            achievementMission1Progress = 0;
            isReceivedAchievement1Reward = false;
            isFalseButton1ClickedAchie = false;
            
            // Chỉ đổi sprite sang completed nếu đã hoàn thành TẤT CẢ level
            if (achievementMission1CurrentLevel >= achievementMission1Target.Length)
            {
                imageAchievementMission1.sprite = spriteMissionClaimed;
                imageFillAchievementMission1.sprite = spriteFillMissionCompleted;
                buttonAchievementMission1.image.sprite = spriteButtonClaimed;
                imageBackGroundFillPlaneMission1.sprite = spriteBackGroundFillMissionCompleted;
                textAchievementMission1.text = "MAX LEVEL";
                buttonAchievementMission1.interactable = false;
            }
            else
            {
                // Reset lại sprite về trạng thái incomplete cho level mới
                imageAchievementMission1.sprite = spriteMissionIncomplete;
                imageFillAchievementMission1.sprite = spriteFillMissionIncomplete;
                buttonAchievementMission1.image.sprite = spriteButtonIncomplete;
                imageBackGroundFillPlaneMission1.sprite = spriteBackGroundFillMissionIncomplete;
                imageFillAchievementMission1.fillAmount = 0f;
            }
            
            Save();
            UpdateAchievementMission(); // Cập nhật UI với mốc mới
        }
    }
    public void buttonMission2ClaimReward()
    {
        if (isAchievementMission2Completed && achievementMission2CurrentLevel < achievementMission2Target.Length && !isFalseButton2ClickedAchie)
        {
            Debug.Log("Achievement Mission 2 Reward Claimed! Level: " + (achievementMission2CurrentLevel + 1));
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            // Trao thưởng kim cương
            GManager.instance.totalDiamond += prizeRewardAchievement2[achievementMission2CurrentLevel];
            PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
            GManager.instance.totalDiamondText.text = GManager.instance.totalDiamond.ToString();
            PlayerPrefs.Save();
            GManager.instance.SaveTotalDiamond();
            
            MissionManager.instance.textQuantityRewardValue--;
            MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
            if (MissionManager.instance.textQuantityRewardValue <= 0)
            {
                MissionManager.instance.notificationImage.gameObject.SetActive(false);
            }
            
            achievementMission2CurrentLevel++;
            MissionPlane.instance.planeMission3Progress++;
            MissionPlane.instance.UpdatePlaneMission();
            
            isAchievementMission2Completed = false;
            achievementMission2Progress = 0;
            isReceivedAchievement2Reward = false;
            isFalseButton2ClickedAchie = false;
            
            if (achievementMission2CurrentLevel >= achievementMission2Target.Length)
            {
                imageAchievementMission2.sprite = spriteMissionClaimed;
                imageFillAchievementMission2.sprite = spriteFillMissionCompleted;
                buttonAchievementMission2.image.sprite = spriteButtonClaimed;
                imageBackGroundFillPlaneMission2.sprite = spriteBackGroundFillMissionCompleted;
                textAchievementMission2.text = "MAX LEVEL";
                buttonAchievementMission2.interactable = false;
            }
            else
            {
                imageAchievementMission2.sprite = spriteMissionIncomplete;
                imageFillAchievementMission2.sprite = spriteFillMissionIncomplete;
                buttonAchievementMission2.image.sprite = spriteButtonIncomplete;
                imageBackGroundFillPlaneMission2.sprite = spriteBackGroundFillMissionIncomplete;
                imageFillAchievementMission2.fillAmount = 0f;
            }
            
            Save();
            UpdateAchievementMission();
        }
    }
    public void buttonMission3ClaimReward()
    {
        if (isAchievementMission3Completed && achievementMission3CurrentLevel < achievementMission3Target.Length && !isFalseButton3ClickedAchie)
        {
            Debug.Log("Achievement Mission 3 Reward Claimed! Level: " + (achievementMission3CurrentLevel + 1));
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            // Trao thưởng tiền
            GManager.instance.totalMoney += prizeRewardAchievement3[achievementMission3CurrentLevel];
            PlayerPrefs.SetFloat("TotalMoney", GManager.instance.totalMoney);
            PlayerPrefs.Save();
            GManager.instance.SaveTotalMoney();
            
            MissionManager.instance.textQuantityRewardValue--;
            MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
            if (MissionManager.instance.textQuantityRewardValue <= 0)
            {
                MissionManager.instance.notificationImage.gameObject.SetActive(false);
            }
            
            achievementMission3CurrentLevel++;
            MissionPlane.instance.planeMission3Progress++;
            MissionPlane.instance.UpdatePlaneMission();
            
            isAchievementMission3Completed = false;
            achievementMission3Progress = 0;
            isReceivedAchievement3Reward = false;
            isFalseButton3ClickedAchie = false;
            
            if (achievementMission3CurrentLevel >= achievementMission3Target.Length)
            {
                imageAchievementMission3.sprite = spriteMissionClaimed;
                imageFillAchievementMission3.sprite = spriteFillMissionCompleted;
                buttonAchievementMission3.image.sprite = spriteButtonClaimed;
                imageBackGroundFillPlaneMission3.sprite = spriteBackGroundFillMissionCompleted;
                textAchievementMission3.text = "MAX LEVEL";
                buttonAchievementMission3.interactable = false;
            }
            else
            {
                imageAchievementMission3.sprite = spriteMissionIncomplete;
                imageFillAchievementMission3.sprite = spriteFillMissionIncomplete;
                buttonAchievementMission3.image.sprite = spriteButtonIncomplete;
                imageBackGroundFillPlaneMission3.sprite = spriteBackGroundFillMissionIncomplete;
                imageFillAchievementMission3.fillAmount = 0f;
            }
            
            Save();
            UpdateAchievementMission();
        }
    }
    public void buttonMission4ClaimReward()
    {
        if (isAchievementMission4Completed && achievementMission4CurrentLevel < achievementMission4Target.Length && !isFalseButton4ClickedAchie)
        {
            Debug.Log("Achievement Mission 4 Reward Claimed! Level: " + (achievementMission4CurrentLevel + 1));
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            // Trao thưởng tiền
            GManager.instance.totalMoney += prizeRewardAchievement4[achievementMission4CurrentLevel];
            PlayerPrefs.SetFloat("TotalMoney", GManager.instance.totalMoney);
            PlayerPrefs.Save();
            GManager.instance.SaveTotalMoney();
            
            MissionManager.instance.textQuantityRewardValue--;
            MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
            if (MissionManager.instance.textQuantityRewardValue <= 0)
            {
                MissionManager.instance.notificationImage.gameObject.SetActive(false);
            }
            
            achievementMission4CurrentLevel++;
            MissionPlane.instance.planeMission3Progress++;
            MissionPlane.instance.UpdatePlaneMission();
            
            isAchievementMission4Completed = false;
            achievementMission4Progress = 0;
            isReceivedAchievement4Reward = false;
            isFalseButton4ClickedAchie = false;
            
            if (achievementMission4CurrentLevel >= achievementMission4Target.Length)
            {
                imageAchievementMission4.sprite = spriteMissionClaimed;
                imageFillAchievementMission4.sprite = spriteFillMissionCompleted;
                buttonAchievementMission4.image.sprite = spriteButtonClaimed;
                imageBackGroundFillPlaneMission4.sprite = spriteBackGroundFillMissionCompleted;
                textAchievementMission4.text = "MAX LEVEL";
                buttonAchievementMission4.interactable = false;
            }
            else
            {
                imageAchievementMission4.sprite = spriteMissionIncomplete;
                imageFillAchievementMission4.sprite = spriteFillMissionIncomplete;
                buttonAchievementMission4.image.sprite = spriteButtonIncomplete;
                imageBackGroundFillPlaneMission4.sprite = spriteBackGroundFillMissionIncomplete;
                imageFillAchievementMission4.fillAmount = 0f;
            }
            
            Save();
            UpdateAchievementMission();
        }
    }
    public void buttonMission5ClaimReward()
    {
        if (isAchievementMission5Completed && achievementMission5CurrentLevel < achievementMission5Target.Length && !isFalseButton5ClickedAchie)
        {
            Debug.Log("Achievement Mission 5 Reward Claimed! Level: " + (achievementMission5CurrentLevel + 1));
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            // Trao thưởng tiền
            GManager.instance.totalMoney += prizeRewardAchievement5[achievementMission5CurrentLevel];
            PlayerPrefs.SetFloat("TotalMoney", GManager.instance.totalMoney);
            PlayerPrefs.Save();
            GManager.instance.SaveTotalMoney();
            
            MissionManager.instance.textQuantityRewardValue--;
            MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
            if (MissionManager.instance.textQuantityRewardValue <= 0)
            {
                MissionManager.instance.notificationImage.gameObject.SetActive(false);
            }
            
            achievementMission5CurrentLevel++;
            MissionPlane.instance.planeMission3Progress++;
            MissionPlane.instance.UpdatePlaneMission();
            
            isAchievementMission5Completed = false;
            achievementMission5Progress = 0;
            isReceivedAchievement5Reward = false;
            isFalseButton5ClickedAchie = false;
            
            if (achievementMission5CurrentLevel >= achievementMission5Target.Length)
            {
                imageAchievementMission5.sprite = spriteMissionClaimed;
                imageFillAchievementMission5.sprite = spriteFillMissionCompleted;
                buttonAchievementMission5.image.sprite = spriteButtonClaimed;
                imageBackGroundFillPlaneMission5.sprite = spriteBackGroundFillMissionCompleted;
                textAchievementMission5.text = "MAX LEVEL";
                buttonAchievementMission5.interactable = false;
            }
            else
            {
                imageAchievementMission5.sprite = spriteMissionIncomplete;
                imageFillAchievementMission5.sprite = spriteFillMissionIncomplete;
                buttonAchievementMission5.image.sprite = spriteButtonIncomplete;
                imageBackGroundFillPlaneMission5.sprite = spriteBackGroundFillMissionIncomplete;
                imageFillAchievementMission5.fillAmount = 0f;
            }
            
            Save();
            UpdateAchievementMission();
        }
    }

    public void completeRewardCollection(){
        // Mission 1 - Only show claimed status when ALL 5 levels are completed
        if(achievementMission1CurrentLevel >= achievementMission1Target.Length){
            buttonAchievementMission1.interactable = false;
            imageAchievementMission1.sprite = spriteMissionClaimed;
            imageFillAchievementMission1.sprite = spriteFillMissionCompleted;
            buttonAchievementMission1.image.sprite = spriteButtonClaimed;
            imageBackGroundFillPlaneMission1.sprite = spriteBackGroundFillMissionCompleted;
            textAchievementMission1.text = "MAX LEVEL";
            imageFillAchievementMission1.fillAmount = 1f;
        }
        else if(isAchievementMission1Completed){
            buttonAchievementMission1.interactable = true;
            buttonAchievementMission1.image.sprite = spriteButtonCompleted;
            textAchievementMission1.text = "Mission Completed";
        }
        
        // Mission 2
        if(achievementMission2CurrentLevel >= achievementMission2Target.Length){
            buttonAchievementMission2.interactable = false;
            imageAchievementMission2.sprite = spriteMissionClaimed;
            imageFillAchievementMission2.sprite = spriteFillMissionCompleted;
            buttonAchievementMission2.image.sprite = spriteButtonClaimed;
            imageBackGroundFillPlaneMission2.sprite = spriteBackGroundFillMissionCompleted;
            textAchievementMission2.text = "MAX LEVEL";
            imageFillAchievementMission2.fillAmount = 1f;
        }
        else if(isAchievementMission2Completed){
            buttonAchievementMission2.interactable = true;
            buttonAchievementMission2.image.sprite = spriteButtonCompleted;
            textAchievementMission2.text = "Mission Completed";
        }
        
        // Mission 3
        if(achievementMission3CurrentLevel >= achievementMission3Target.Length){
            buttonAchievementMission3.interactable = false;
            imageAchievementMission3.sprite = spriteMissionClaimed;
            imageFillAchievementMission3.sprite = spriteFillMissionCompleted;
            buttonAchievementMission3.image.sprite = spriteButtonClaimed;
            imageBackGroundFillPlaneMission3.sprite = spriteBackGroundFillMissionCompleted;
            textAchievementMission3.text = "MAX LEVEL";
            imageFillAchievementMission3.fillAmount = 1f;
        }
        else if(isAchievementMission3Completed){
            buttonAchievementMission3.interactable = true;
            buttonAchievementMission3.image.sprite = spriteButtonCompleted;
            textAchievementMission3.text = "Mission Completed";
        }
        
        // Mission 4
        if(achievementMission4CurrentLevel >= achievementMission4Target.Length){
            buttonAchievementMission4.interactable = false;
            imageAchievementMission4.sprite = spriteMissionClaimed;
            imageFillAchievementMission4.sprite = spriteFillMissionCompleted;
            buttonAchievementMission4.image.sprite = spriteButtonClaimed;
            imageBackGroundFillPlaneMission4.sprite = spriteBackGroundFillMissionCompleted;
            textAchievementMission4.text = "MAX LEVEL";
            imageFillAchievementMission4.fillAmount = 1f;
        }
        else if(isAchievementMission4Completed){
            buttonAchievementMission4.interactable = true;
            buttonAchievementMission4.image.sprite = spriteButtonCompleted;
            textAchievementMission4.text = "Mission Completed";
        }
        
        // Mission 5
        if(achievementMission5CurrentLevel >= achievementMission5Target.Length){
            buttonAchievementMission5.interactable = false;
            imageAchievementMission5.sprite = spriteMissionClaimed;
            imageFillAchievementMission5.sprite = spriteFillMissionCompleted;
            buttonAchievementMission5.image.sprite = spriteButtonClaimed;
            imageBackGroundFillPlaneMission5.sprite = spriteBackGroundFillMissionCompleted;
            textAchievementMission5.text = "MAX LEVEL";
            imageFillAchievementMission5.fillAmount = 1f;
        }
        else if(isAchievementMission5Completed){
            buttonAchievementMission5.interactable = true;
            buttonAchievementMission5.image.sprite = spriteButtonCompleted;
            textAchievementMission5.text = "Mission Completed";
        }
    }

    // Flight Time Tracking Methods
    public void StartFlightTimer()
    {
        flightStartTime = Time.time;
        isFlightTimeActive = true;
        Debug.Log("[MISSION] Flight timer started at: " + flightStartTime);
    }
    
    public void StopFlightTimerAndUpdateMission()
    {
        if (isFlightTimeActive)
        {
            flightEndTime = Time.time;
            float thisFlightTime = flightEndTime - flightStartTime;
            isFlightTimeActive = false;
            
            // Cộng dồn thời gian bay tổng (theo giây)
            cumulativeFlightTimeSeconds += thisFlightTime;
            
            // Convert tổng thời gian sang phút với 2 chữ số thập phân
            float totalMinutes = cumulativeFlightTimeSeconds / 60f;
            
            // Cập nhật progress với số phút có thập phân (nhân 100 để làm việc với int)
            int progressValue = Mathf.FloorToInt(totalMinutes * 100f);
            achievementMission4Progress = progressValue;
            
            Debug.Log($"[MISSION] Flight completed! This flight: {thisFlightTime:F1}s");
            Debug.Log($"[MISSION] Total cumulative time: {cumulativeFlightTimeSeconds:F1}s ({totalMinutes:F2} minutes)");
            Debug.Log($"[MISSION] Achievement Mission 4 Progress: {(progressValue/100f):F2}/{(achievementMission4CurrentLevel < achievementMission4Target.Length ? achievementMission4Target[achievementMission4CurrentLevel] : "MAX")}");
            
            // Update mission UI
            UpdateAchievementMission();
            Save();
        }
        else
        {
            Debug.LogWarning("[MISSION] StopFlightTimer called but flight timer was not active");
        }
    }
    
    public string FormatFlightTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        
        if (minutes > 0)
            return string.Format("{0}:{1:00}", minutes, seconds);
        else
            return string.Format("{0}s", seconds);
    }
    
    public float GetCurrentFlightTime()
    {
        if (isFlightTimeActive)
            return Time.time - flightStartTime;
        else
            return 0f;
    }

}
