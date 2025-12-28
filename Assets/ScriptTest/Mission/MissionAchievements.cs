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
    public int achievementMission5Progress = 0;
    public int[] achievementMission5Target = {2, 3, 4, 5, 6};

    public bool isReceivedAchievement1Reward = false;
    public bool isReceivedAchievement2Reward = false;
    public bool isReceivedAchievement3Reward = false;
    public bool isReceivedAchievement4Reward = false;
    public bool isReceivedAchievement5Reward = false;

    // Start is called before the first frame update
    void Start()
    {
        Load();
        UpdateAchievementMission();
        completeRewardCollection();
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
        if(Input.GetKeyDown(KeyCode.A))
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
            if (!isAchievementMission1Completed)
            {
                imageFillAchievementMission1.fillAmount = (float)achievementMission1Progress / achievementMission1Target[achievementMission1CurrentLevel];
                Debug.Log("Achievement Mission 1 Progress: " + imageFillAchievementMission1.fillAmount);
                if(!isReceivedAchievement1Reward){
                    textAchievementMission1.text = achievementMission1Progress + "/" + achievementMission1Target[achievementMission1CurrentLevel];
                }
                else{
                    textAchievementMission1.text = "Claimed";
                }

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
                if(!isReceivedAchievement2Reward){
                    textAchievementMission2.text = achievementMission2Progress + "/" + achievementMission2Target[achievementMission2CurrentLevel];
                }
                else{
                    textAchievementMission2.text = "Claimed";
                }

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
                if(!isReceivedAchievement3Reward){
                    textAchievementMission3.text = achievementMission3Progress + "/" + achievementMission3Target[achievementMission3CurrentLevel];
                }
                else{
                    textAchievementMission3.text = "Claimed";
                }

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
                if(!isReceivedAchievement4Reward){
                    textAchievementMission4.text = achievementMission4Progress + "/" + achievementMission4Target[achievementMission4CurrentLevel];
                }
                else{
                    textAchievementMission4.text = "Claimed";
                }

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
                if(!isReceivedAchievement5Reward){
                    textAchievementMission5.text = achievementMission5Progress + "/" + achievementMission5Target[achievementMission5CurrentLevel];
                }
                else{
                    textAchievementMission5.text = "Claimed";
                }

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

        isReceivedAchievement1Reward = false;
        isReceivedAchievement2Reward = false;
        isReceivedAchievement3Reward = false;
        isReceivedAchievement4Reward = false;
        isReceivedAchievement5Reward = false;

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
            isReceivedAchievement1Reward = true;
            textAchievementMission1.text = "Claimed";
            
            if(isReceivedAchievement1Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }
            }
            
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
            isReceivedAchievement2Reward = true;
            textAchievementMission2.text = "Claimed";
            
            if(isReceivedAchievement2Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }
            }
            
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
            isReceivedAchievement3Reward = true;
            textAchievementMission3.text = "Claimed";
            
            if(isReceivedAchievement3Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }
            }
            
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
            isReceivedAchievement4Reward = true;
            textAchievementMission4.text = "Claimed";
            
            if(isReceivedAchievement4Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }
            }
            
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
            isReceivedAchievement5Reward = true;
            textAchievementMission5.text = "Claimed";
            
            if(isReceivedAchievement5Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }
            }
            
            UpdateAchievementMission();
        }
    }

    public void completeRewardCollection(){
        if(isReceivedAchievement1Reward){
            buttonAchievementMission1.interactable = false;
            imageAchievementMission1.color = Color.gray;
            buttonAchievementMission1.image.color = Color.gray;
            textAchievementMission1.text = "Claimed";
        }
        if(isReceivedAchievement2Reward){
            buttonAchievementMission2.interactable = false;
            imageAchievementMission2.color = Color.gray;
            buttonAchievementMission2.image.color = Color.gray;
            textAchievementMission2.text = "Claimed";
        }
        if(isReceivedAchievement3Reward){
            buttonAchievementMission3.interactable = false;
            imageAchievementMission3.color = Color.gray;
            buttonAchievementMission3.image.color = Color.gray;
            textAchievementMission3.text = "Claimed";
        }
        if(isReceivedAchievement4Reward){
            buttonAchievementMission4.interactable = false;
            imageAchievementMission4.color = Color.gray;
            buttonAchievementMission4.image.color = Color.gray;
            textAchievementMission4.text = "Claimed";
        }
        if(isReceivedAchievement5Reward){
            buttonAchievementMission5.interactable = false;
            imageAchievementMission5.color = Color.gray;
            buttonAchievementMission5.image.color = Color.gray;
            textAchievementMission5.text = "Claimed";
        }
    }

}
