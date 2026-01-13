using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;


public class MissionPlane : MonoBehaviour
{
    public static MissionPlane instance;
    [Header("Mission Plane Text")]
    public TextMeshProUGUI textPlaneMission1;
    public TextMeshProUGUI textPlaneMission2;
    public TextMeshProUGUI textPlaneMission3;
    public TextMeshProUGUI textPlaneMission4;
    public TextMeshProUGUI textPlaneMission5;

    [Header("Mission Plane Image")]
    public Image imagePlaneMission1;
    public Image imagePlaneMission2;
    public Image imagePlaneMission3;
    public Image imagePlaneMission4;
    public Image imagePlaneMission5;
    public Sprite spriteMissionIncomplete;
    public Sprite spriteMissionClaimed;


    [Header("Mission Plane Image Fill")]
    public Image imageFillPlaneMission1;
    public Image imageFillPlaneMission2;
    public Image imageFillPlaneMission3;
    public Image imageFillPlaneMission4;
    public Image imageFillPlaneMission5;
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

    [Header("Mission Plane Button")]
    public Button buttonPlaneMission1;
    public Button buttonPlaneMission2;
    public Button buttonPlaneMission3;
    public Button buttonPlaneMission4;
    public Button buttonPlaneMission5;
    public Sprite spriteButtonIncomplete;
    public Sprite spriteButtonCompleted;
    public Sprite spriteButtonClaimed;

    [Header("Mission Plane")]
    public bool isPlaneMission1Completed = false;
    public bool isPlaneMission2Completed = false;
    public bool isPlaneMission3Completed = false;
    public bool isPlaneMission4Completed = false;
    public bool isPlaneMission5Completed = false;
    public int planeMission1Progress = 0;
    public int planeMission1Target = 100;
    public int planeMission2Progress = 0;
    public int planeMission2Target = 100;
    public int planeMission3Progress = 0;
    public int planeMission3Target = 100;
    public int planeMission4Progress = 0;
    public int planeMission4Target = 100;
    public int planeMission5Progress = 0;
    public int planeMission5Target = 100;

    public bool isReceivedPlane1Reward = false;
    public bool isReceivedPlane2Reward = false;
    public bool isReceivedPlane3Reward = false;
    public bool isReceivedPlane4Reward = false;
    public bool isReceivedPlane5Reward = false;
    public bool isFalseButton1Clicked = false;
    public bool isFalseButton2Clicked = false;
    public bool isFalseButton3Clicked = false;
    public bool isFalseButton4Clicked = false;
    public bool isFalseButton5Clicked = false;

    public bool isUnlockSuperPlane1 = false;
    public bool isUnlockSuperPlane2 = false;
    public bool isUnlockSuperPlane3 = false;
    public bool isUnlockSuperPlane4 = false;
    public bool isUnlockSuperPlane5 = false;

    [Header("Information Super Plane")]
    public Image superPlaneInfoImage;
    public GameObject superPlaneInfoPlane1; 
    public GameObject superPlaneInfoPlane2;
    public GameObject superPlaneInfoPlane3;
    public GameObject superPlaneInfoPlane4;
    public GameObject superPlaneInfoPlane5;

    [Header("Guide Plane")]
    public Image guidePlane1Image;
    public Image guidePlane2Image;
    public Image guidePlane3Image;
    public Image guidePlane4Image;
    public Image guidePlane5Image;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);    
        Load();
        UpdatePlaneMission();
        completeRewardCollection();
        if(isUnlockSuperPlane1)
        {
            guidePlane1Image.gameObject.SetActive(false);
            Shop.instance.listTextPriceSuperlane[0].text = "Select";
        }
        if(isUnlockSuperPlane2)
        {
            guidePlane2Image.gameObject.SetActive(false);
            Shop.instance.listTextPriceSuperlane[1].text = "Select";
        }
        if(isUnlockSuperPlane3)
        {
            guidePlane3Image.gameObject.SetActive(false);
            Shop.instance.listTextPriceSuperlane[2].text = "Select";
        }
        if(isUnlockSuperPlane4)
        {
            guidePlane4Image.gameObject.SetActive(false);
            Shop.instance.listTextPriceSuperlane[3].text = "Select";
        }
        if(isUnlockSuperPlane5)
        {
            guidePlane5Image.gameObject.SetActive(false);
            Shop.instance.listTextPriceSuperlane[4].text = "Select";
        }


        


    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Z))
        // {
        //     ResetPlaneMissions();
        // }
        // if (Input.GetKeyDown(KeyCode.X))
        // {
        //     planeMission1Progress = planeMission1Target;
        //     planeMission2Progress = planeMission2Target;
        //     planeMission3Progress = planeMission3Target;
        //     planeMission4Progress = planeMission4Target;
        //     planeMission5Progress = planeMission5Target;
        //     UpdatePlaneMission();
        // }

    }

    public void UpdatePlaneMission()
    {
        if (!isPlaneMission1Completed && !isReceivedPlane1Reward)
        {

            imageFillPlaneMission1.fillAmount = (float)planeMission1Progress / planeMission1Target;
            Debug.Log("Plane Mission 1 Progress: " + imageFillPlaneMission1.fillAmount);
            textPlaneMission1.text = planeMission1Progress + "/" + planeMission1Target;

            if (planeMission1Progress >= planeMission1Target)
            {
                isPlaneMission1Completed = true;
                buttonPlaneMission1.interactable = true;
                textPlaneMission1.text = "Mission Completed";
                buttonPlaneMission1.image.sprite = spriteButtonCompleted;
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);

                MissionManager.instance.textNotificationPlaneValue++;
                MissionManager.instance.textNotificationPlane.text = MissionManager.instance.textNotificationPlaneValue.ToString();
                MissionManager.instance.notificationImagePlane.gameObject.SetActive(true);
            }
        }
        if (!isPlaneMission2Completed && !isReceivedPlane2Reward)
        {
            imageFillPlaneMission2.fillAmount = (float)planeMission2Progress / planeMission2Target;
            Debug.Log("Plane Mission 2 Progress: " + imageFillPlaneMission2.fillAmount);
            textPlaneMission2.text = planeMission2Progress + "/" + planeMission2Target;

            if (planeMission2Progress >= planeMission2Target)
            {
                isPlaneMission2Completed = true;
                buttonPlaneMission2.interactable = true;
                textPlaneMission2.text = "Mission Completed";
                buttonPlaneMission2.image.sprite = spriteButtonCompleted;
                UpdatePlaneMission();
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);

                MissionManager.instance.textNotificationPlaneValue++;
                MissionManager.instance.textNotificationPlane.text = MissionManager.instance.textNotificationPlaneValue.ToString();
                MissionManager.instance.notificationImagePlane.gameObject.SetActive(true);
            }
        }
        if (!isPlaneMission3Completed && !isReceivedPlane3Reward)
        {
            imageFillPlaneMission3.fillAmount = (float)planeMission3Progress / planeMission3Target;
           // Debug.Break(); tạm dừng bên Unity để kiểm tra giá trị
            Debug.Log("Plane Mission 3 Progress: " + imageFillPlaneMission3.fillAmount);
            textPlaneMission3.text = planeMission3Progress + "/" + planeMission3Target;

            if (planeMission3Progress >= planeMission3Target)
            {
                isPlaneMission3Completed = true;
                buttonPlaneMission3.interactable = true;
                textPlaneMission3.text = "Mission Completed";
                buttonPlaneMission3.image.sprite = spriteButtonCompleted;
                UpdatePlaneMission();
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);

                MissionManager.instance.textNotificationPlaneValue++;
                MissionManager.instance.textNotificationPlane.text = MissionManager.instance.textNotificationPlaneValue.ToString();
                MissionManager.instance.notificationImagePlane.gameObject.SetActive(true);
            }
        }
        if (!isPlaneMission4Completed && !isReceivedPlane4Reward)
        {
            imageFillPlaneMission4.fillAmount = (float)planeMission4Progress / planeMission4Target;
            Debug.Log("Plane Mission 4 Progress: " + imageFillPlaneMission4.fillAmount);
            textPlaneMission4.text = planeMission4Progress + "/" + planeMission4Target;

            if (planeMission4Progress >= planeMission4Target)
            {
                isPlaneMission4Completed = true;
                buttonPlaneMission4.interactable = true;
                textPlaneMission4.text = "Mission Completed";
                buttonPlaneMission4.image.sprite = spriteButtonCompleted;
                UpdatePlaneMission();
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);
                MissionManager.instance.textNotificationPlaneValue++;
                MissionManager.instance.textNotificationPlane.text = MissionManager.instance.textNotificationPlaneValue.ToString();
                MissionManager.instance.notificationImagePlane.gameObject.SetActive(true);

            }
        }
        if (!isPlaneMission5Completed && !isReceivedPlane5Reward)
        {
            imageFillPlaneMission5.fillAmount = (float)planeMission5Progress / planeMission5Target;
            Debug.Log("Plane Mission 5 Progress: " + imageFillPlaneMission5.fillAmount);
            textPlaneMission5.text = planeMission5Progress + "/" + planeMission5Target;

            if (planeMission5Progress >= planeMission5Target)
            {
                isPlaneMission5Completed = true;
                buttonPlaneMission5.interactable = true;
                textPlaneMission5.text = "Mission Completed";
                buttonPlaneMission5.image.sprite = spriteButtonCompleted;
                UpdatePlaneMission();
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);

                MissionManager.instance.textNotificationPlaneValue++;
                MissionManager.instance.textNotificationPlane.text = MissionManager.instance.textNotificationPlaneValue.ToString();
                MissionManager.instance.notificationImagePlane.gameObject.SetActive(true);
            }
        }
        Save();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("PlaneMission1Progress", planeMission1Progress);
        PlayerPrefs.SetInt("PlaneMission2Progress", planeMission2Progress);
        PlayerPrefs.SetInt("PlaneMission3Progress", planeMission3Progress);
        PlayerPrefs.SetInt("PlaneMission4Progress", planeMission4Progress);
        PlayerPrefs.SetInt("PlaneMission5Progress", planeMission5Progress);

        PlayerPrefs.SetInt("IsReceivedPlane1Reward", isReceivedPlane1Reward ? 1 : 0);
        PlayerPrefs.SetInt("IsReceivedPlane2Reward", isReceivedPlane2Reward ? 1 : 0);
        PlayerPrefs.SetInt("IsReceivedPlane3Reward", isReceivedPlane3Reward ? 1 : 0);
        PlayerPrefs.SetInt("IsReceivedPlane4Reward", isReceivedPlane4Reward ? 1 : 0);
        PlayerPrefs.SetInt("IsReceivedPlane5Reward", isReceivedPlane5Reward ? 1 : 0);
    }

    public void Load()
    {
        planeMission1Progress = PlayerPrefs.GetInt("PlaneMission1Progress", 0);
        planeMission2Progress = PlayerPrefs.GetInt("PlaneMission2Progress", 0);
        planeMission3Progress = PlayerPrefs.GetInt("PlaneMission3Progress", 0);
        planeMission4Progress = PlayerPrefs.GetInt("PlaneMission4Progress", 0);
        planeMission5Progress = PlayerPrefs.GetInt("PlaneMission5Progress", 0);

        isReceivedPlane1Reward = PlayerPrefs.GetInt("IsReceivedPlane1Reward", 0) == 1;
        isReceivedPlane2Reward = PlayerPrefs.GetInt("IsReceivedPlane2Reward", 0) == 1;
        isReceivedPlane3Reward = PlayerPrefs.GetInt("IsReceivedPlane3Reward", 0) == 1;
        isReceivedPlane4Reward = PlayerPrefs.GetInt("IsReceivedPlane4Reward", 0) == 1;
        isReceivedPlane5Reward = PlayerPrefs.GetInt("IsReceivedPlane5Reward", 0) == 1;

        isUnlockSuperPlane1 = PlayerPrefs.GetInt("IsUnlockSuperPlane1", 0) == 1;
        isUnlockSuperPlane2 = PlayerPrefs.GetInt("IsUnlockSuperPlane2", 0) == 1;
        isUnlockSuperPlane3 = PlayerPrefs.GetInt("IsUnlockSuperPlane3", 0) == 1;
        isUnlockSuperPlane4 = PlayerPrefs.GetInt("IsUnlockSuperPlane4", 0) == 1;
        isUnlockSuperPlane5 = PlayerPrefs.GetInt("IsUnlockSuperPlane5", 0) == 1;

    }

    public void ResetPlaneMissions()
    {
        planeMission1Progress = 0;
        planeMission2Progress = 0;
        planeMission3Progress = 0;
        planeMission4Progress = 0;
        planeMission5Progress = 0;

        isPlaneMission1Completed = false;
        isPlaneMission2Completed = false;
        isPlaneMission3Completed = false;
        isPlaneMission4Completed = false;
        isPlaneMission5Completed = false;

        isFalseButton1Clicked = false;
        isFalseButton2Clicked = false;
        isFalseButton3Clicked = false;
        isFalseButton4Clicked = false;
        isFalseButton5Clicked = false;

        isReceivedPlane1Reward = false;
        isReceivedPlane2Reward = false;
        isReceivedPlane3Reward = false;
        isReceivedPlane4Reward = false;
        isReceivedPlane5Reward = false;

        imagePlaneMission1.sprite = spriteMissionIncomplete;
        imagePlaneMission2.sprite = spriteMissionIncomplete;
        imagePlaneMission3.sprite = spriteMissionIncomplete;
        imagePlaneMission4.sprite = spriteMissionIncomplete;
        imagePlaneMission5.sprite = spriteMissionIncomplete;

        imageFillPlaneMission1.sprite = spriteFillMissionIncomplete;
        imageFillPlaneMission2.sprite = spriteFillMissionIncomplete;
        imageFillPlaneMission3.sprite = spriteFillMissionIncomplete;
        imageFillPlaneMission4.sprite = spriteFillMissionIncomplete;
        imageFillPlaneMission5.sprite = spriteFillMissionIncomplete;

        buttonPlaneMission1.image.sprite = spriteButtonIncomplete;
        buttonPlaneMission2.image.sprite = spriteButtonIncomplete;
        buttonPlaneMission3.image.sprite = spriteButtonIncomplete;
        buttonPlaneMission4.image.sprite = spriteButtonIncomplete;
        buttonPlaneMission5.image.sprite = spriteButtonIncomplete;

        imageBackGroundFillPlaneMission1.sprite = spriteBackGroundFillMissionIncomplete;
        imageBackGroundFillPlaneMission2.sprite = spriteBackGroundFillMissionIncomplete;
        imageBackGroundFillPlaneMission3.sprite = spriteBackGroundFillMissionIncomplete;
        imageBackGroundFillPlaneMission4.sprite = spriteBackGroundFillMissionIncomplete;
        imageBackGroundFillPlaneMission5.sprite = spriteBackGroundFillMissionIncomplete;

        // Reset fillAmount của tất cả progress bars
        imageFillPlaneMission1.fillAmount = 0f;
        imageFillPlaneMission2.fillAmount = 0f;
        imageFillPlaneMission3.fillAmount = 0f;
        imageFillPlaneMission4.fillAmount = 0f;
        imageFillPlaneMission5.fillAmount = 0f;

        UpdatePlaneMission();
        Save();
    }

    public void buttonMission1ClaimReward()
    {
        if (isPlaneMission1Completed && !isFalseButton1Clicked)
        {
            Debug.Log("Plane Mission 1 Reward Claimed!");
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            isUnlockSuperPlane1 = true;
            PlayerPrefs.SetInt("IsUnlockSuperPlane1", 1);
            buttonPlaneMission1.image.sprite = spriteButtonClaimed;
            isPlaneMission1Completed = false;
            isFalseButton1Clicked = true;
            imagePlaneMission1.sprite = spriteMissionClaimed;
            imageFillPlaneMission1.sprite = spriteFillMissionCompleted;
            isReceivedPlane1Reward = true;
            textPlaneMission1.text = "Claimed";
            imageBackGroundFillPlaneMission1.sprite = spriteBackGroundFillMissionCompleted;
            Shop.instance.listTextPriceSuperlane[0].text = "Select";
            guidePlane1Image.gameObject.SetActive(false);

            MissionAchievements.instance.achievementMission2Progress++;
            MissionAchievements.instance.UpdateAchievementMission();
            if(isReceivedPlane1Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }

                MissionManager.instance.textNotificationPlaneValue--;
                MissionManager.instance.textNotificationPlane.text = MissionManager.instance.textNotificationPlaneValue.ToString();
                if (MissionManager.instance.textNotificationPlaneValue <= 0)
                {
                    MissionManager.instance.notificationImagePlane.gameObject.SetActive(false);
                }



                GMAnalytics.LogEvent("unlock_super_plane", 2);
            
            }
            Save();
        }
    }
    public void buttonMission2ClaimReward()
    {
        if (isPlaneMission2Completed && !isFalseButton2Clicked)
        {
            Debug.Log("Plane Mission 2 Reward Claimed!");
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            isUnlockSuperPlane2 = true;
            PlayerPrefs.SetInt("IsUnlockSuperPlane2", 1);   
            buttonPlaneMission2.image.sprite = spriteButtonClaimed;
            isPlaneMission2Completed = false;
            isFalseButton2Clicked = true;
            imagePlaneMission2.sprite = spriteMissionClaimed;
            imageFillPlaneMission2.sprite = spriteFillMissionCompleted;
            isReceivedPlane2Reward = true;
            textPlaneMission2.text = "Claimed";
            imageBackGroundFillPlaneMission2.sprite = spriteBackGroundFillMissionCompleted;
            Shop.instance.listTextPriceSuperlane[1].text = "Select";
            guidePlane2Image.gameObject.SetActive(false);
            MissionAchievements.instance.achievementMission2Progress++;
            MissionAchievements.instance.UpdateAchievementMission();
            if(isReceivedPlane2Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }

                MissionManager.instance.textNotificationPlaneValue--;
                MissionManager.instance.textNotificationPlane.text = MissionManager.instance.textNotificationPlaneValue.ToString();
                if (MissionManager.instance.textNotificationPlaneValue <= 0)
                {
                    MissionManager.instance.notificationImagePlane.gameObject.SetActive(false);
                }

                GMAnalytics.LogEvent("unlock_super_plane", 2);
            }
            Save();
        }
    }
    public void buttonMission3ClaimReward()
    {
        if (isPlaneMission3Completed && !isFalseButton3Clicked)
        {
            Debug.Log("Plane Mission 3 Reward Claimed!");
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            isUnlockSuperPlane3 = true;
            PlayerPrefs.SetInt("IsUnlockSuperPlane3", 1);
            buttonPlaneMission3.image.sprite = spriteButtonClaimed;
            isPlaneMission3Completed = false;
            isFalseButton3Clicked = true;
            imagePlaneMission3.sprite = spriteMissionClaimed;
            imageFillPlaneMission3.sprite = spriteFillMissionCompleted;
            isReceivedPlane3Reward = true;
            textPlaneMission3.text = "Claimed";
            imageBackGroundFillPlaneMission3.sprite = spriteBackGroundFillMissionCompleted;
            Shop.instance.listTextPriceSuperlane[2].text = "Select";
            guidePlane3Image.gameObject.SetActive(false);
            MissionAchievements.instance.achievementMission2Progress++;
            MissionAchievements.instance.UpdateAchievementMission();
            if(isReceivedPlane3Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }

                MissionManager.instance.textNotificationPlaneValue--;
                MissionManager.instance.textNotificationPlane.text = MissionManager.instance.textNotificationPlaneValue.ToString();
                if (MissionManager.instance.textNotificationPlaneValue <= 0)
                {
                    MissionManager.instance.notificationImagePlane.gameObject.SetActive(false);
                }

                GMAnalytics.LogEvent("unlock_super_plane", 2);
            }
            Save();
        }
    }
    public void buttonMission4ClaimReward()
    {
        if (isPlaneMission4Completed && !isFalseButton4Clicked)
        {
            Debug.Log("Plane Mission 4 Reward Claimed!");
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            isUnlockSuperPlane4 = true;
            PlayerPrefs.SetInt("IsUnlockSuperPlane4", 1);
            buttonPlaneMission4.image.sprite = spriteButtonClaimed;
            isPlaneMission4Completed = false;
            isFalseButton4Clicked = true;
            imagePlaneMission4.sprite = spriteMissionClaimed;
            imageFillPlaneMission4.sprite = spriteFillMissionCompleted;
            isReceivedPlane4Reward = true;
            textPlaneMission4.text = "Claimed";
            imageBackGroundFillPlaneMission4.sprite = spriteBackGroundFillMissionCompleted;
            Shop.instance.listTextPriceSuperlane[3].text = "Select";
            guidePlane4Image.gameObject.SetActive(false);
            MissionAchievements.instance.achievementMission2Progress++;
            MissionAchievements.instance.UpdateAchievementMission();
            if(isReceivedPlane4Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }

                MissionManager.instance.textNotificationPlaneValue--;
                MissionManager.instance.textNotificationPlane.text = MissionManager.instance.textNotificationPlaneValue.ToString();
                if (MissionManager.instance.textNotificationPlaneValue <= 0)
                {
                    MissionManager.instance.notificationImagePlane.gameObject.SetActive(false);
                }

                GMAnalytics.LogEvent("unlock_super_plane", 2);
            }
            Save();
        }
    }
    public void buttonMission5ClaimReward()
    {
        if (isPlaneMission5Completed && !isFalseButton5Clicked)
        {
            Debug.Log("Plane Mission 5 Reward Claimed!");
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            isUnlockSuperPlane5 = true;
            PlayerPrefs.SetInt("IsUnlockSuperPlane5", 1);
            buttonPlaneMission5.image.sprite = spriteButtonClaimed;
            isPlaneMission5Completed = false;
            isFalseButton5Clicked = true;
            imagePlaneMission5.sprite = spriteMissionClaimed;
            imageFillPlaneMission5.sprite = spriteFillMissionCompleted;
            isReceivedPlane5Reward = true;
            textPlaneMission5.text = "Claimed";
            imageBackGroundFillPlaneMission5.sprite = spriteBackGroundFillMissionCompleted;
            Shop.instance.listTextPriceSuperlane[4].text = "Select";
            guidePlane5Image.gameObject.SetActive(false);
            MissionAchievements.instance.achievementMission2Progress++;
            MissionAchievements.instance.UpdateAchievementMission();
            if(isReceivedPlane5Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }

                MissionManager.instance.textNotificationPlaneValue--;
                MissionManager.instance.textNotificationPlane.text = MissionManager.instance.textNotificationPlaneValue.ToString();
                if (MissionManager.instance.textNotificationPlaneValue <= 0)
                {
                    MissionManager.instance.notificationImagePlane.gameObject.SetActive(false);
                }

                GMAnalytics.LogEvent("unlock_super_plane", 2);
            }
            Save();
        }
    }

    public void completeRewardCollection(){
        if(isReceivedPlane1Reward){
            // buttonPlaneMission1.interactable = false;
            isFalseButton1Clicked = true;
            imagePlaneMission1.sprite = spriteMissionClaimed;
            imageFillPlaneMission1.sprite = spriteFillMissionCompleted;
            buttonPlaneMission1.image.sprite = spriteButtonClaimed;
            imageBackGroundFillPlaneMission1.sprite = spriteBackGroundFillMissionCompleted;
            textPlaneMission1.text = "Claimed";
        }
        if(isReceivedPlane2Reward){
            // buttonPlaneMission2.interactable = false;
            isFalseButton2Clicked = true;
            imagePlaneMission2.sprite = spriteMissionClaimed;
            imageFillPlaneMission2.sprite = spriteFillMissionCompleted;
            buttonPlaneMission2.image.sprite = spriteButtonClaimed;
            imageBackGroundFillPlaneMission2.sprite = spriteBackGroundFillMissionCompleted;
            textPlaneMission2.text = "Claimed";
        }
        if(isReceivedPlane3Reward){
            // buttonPlaneMission3.interactable = false;
            isFalseButton3Clicked = true;
            imagePlaneMission3.sprite = spriteMissionClaimed;
            imageFillPlaneMission3.sprite = spriteFillMissionCompleted;
            buttonPlaneMission3.image.sprite = spriteButtonClaimed;
            imageBackGroundFillPlaneMission3.sprite = spriteBackGroundFillMissionCompleted;
            textPlaneMission3.text = "Claimed";
        }
        if(isReceivedPlane4Reward){
            // buttonPlaneMission4.interactable = false;
            isFalseButton4Clicked = true;
            imagePlaneMission4.sprite = spriteMissionClaimed;
            imageFillPlaneMission4.sprite = spriteFillMissionCompleted;
            buttonPlaneMission4.image.sprite = spriteButtonClaimed;
            imageBackGroundFillPlaneMission4.sprite = spriteBackGroundFillMissionCompleted;
            textPlaneMission4.text = "Claimed";
        }
        if(isReceivedPlane5Reward){
            // buttonPlaneMission5.interactable = false;
            isFalseButton5Clicked = true;
            imagePlaneMission5.sprite = spriteMissionClaimed;
            imageFillPlaneMission5.sprite = spriteFillMissionCompleted;
            buttonPlaneMission5.image.sprite = spriteButtonClaimed;
            imageBackGroundFillPlaneMission5.sprite = spriteBackGroundFillMissionCompleted;
            textPlaneMission5.text = "Claimed";
        }
    }

    public void OpenInfoSuperPlane(int planeIndex)
    {
        AudioManager.instance.PlaySound(AudioManager.instance.buttonSoundClip);

        switch (planeIndex)
        {
            case 1:
                superPlaneInfoPlane1.SetActive(true);
                superPlaneInfoPlane2.SetActive(false);
                superPlaneInfoPlane3.SetActive(false);
                superPlaneInfoPlane4.SetActive(false);
                superPlaneInfoPlane5.SetActive(false);
                break;
            case 2:
                superPlaneInfoPlane1.SetActive(false);
                superPlaneInfoPlane2.SetActive(true);
                superPlaneInfoPlane3.SetActive(false);
                superPlaneInfoPlane4.SetActive(false);
                superPlaneInfoPlane5.SetActive(false);
                break;
            case 3:
                superPlaneInfoPlane1.SetActive(false);
                superPlaneInfoPlane2.SetActive(false);
                superPlaneInfoPlane3.SetActive(true);
                superPlaneInfoPlane4.SetActive(false);
                superPlaneInfoPlane5.SetActive(false);
                break;
            case 4:
                superPlaneInfoPlane1.SetActive(false);
                superPlaneInfoPlane2.SetActive(false);
                superPlaneInfoPlane3.SetActive(false);
                superPlaneInfoPlane4.SetActive(true);
                superPlaneInfoPlane5.SetActive(false);
                break;
            case 5:
                superPlaneInfoPlane1.SetActive(false);
                superPlaneInfoPlane2.SetActive(false);
                superPlaneInfoPlane3.SetActive(false);
                superPlaneInfoPlane4.SetActive(false);
                superPlaneInfoPlane5.SetActive(true);
                break;
            default:
                break;
        }

        // Dừng tất cả animation đang chạy
        DOTween.Kill(superPlaneInfoImage.transform);
        
        // Hiển thị settings panel
        superPlaneInfoImage.gameObject.SetActive(true);
        
        
        // Animation mở settings (giống ShopImage)
        superPlaneInfoImage.transform.localScale = Vector3.zero;
        superPlaneInfoImage.transform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                
            });
    }

    public void CloseInfoSuperPlane()
    {

        AudioManager.instance.PlaySound(AudioManager.instance.exitSoundClip);

        DOTween.Kill(superPlaneInfoImage.transform);

        superPlaneInfoImage.transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                superPlaneInfoImage.gameObject.SetActive(false);
            });
    }


}