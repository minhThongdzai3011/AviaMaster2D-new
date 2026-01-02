using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;


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
    public Sprite spriteMissionIncomplete;
    public Sprite spriteMissionClaimed;

    [Header("Mission Daily Image Fill")]
    public Image imageFillDailyMission1;
    public Image imageFillDailyMission2;
    public Image imageFillDailyMission3;
    public Image imageFillDailyMission4;
    public Image imageFillDailyMission5;
    public Sprite spriteFillMissionIncomplete;
    public Sprite spriteFillMissionCompleted;

    [Header("Mission Daily Image Background Fill")]
    public Image imageBackGroundFillDailyMission1;
    public Image imageBackGroundFillDailyMission2;
    public Image imageBackGroundFillDailyMission3;
    public Image imageBackGroundFillDailyMission4;
    public Image imageBackGroundFillDailyMission5;
    public Sprite spriteBackGroundFillMissionIncomplete;
    public Sprite spriteBackGroundFillMissionCompleted;

    [Header("Mission Daily Button")]
    public Button buttonDailyMission1;
    public Button buttonDailyMission2;
    public Button buttonDailyMission3;
    public Button buttonDailyMission4;
    public Button buttonDailyMission5;
    public Sprite spriteButtonIncomplete;
    public Sprite spriteButtonCompleted;
    public Sprite spriteButtonClaimed;

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
    public bool isReceivedDaily1Reward = false;
    public bool isReceivedDaily2Reward = false;
    public bool isReceivedDaily3Reward = false;
    public bool isReceivedDaily4Reward = false;
    public bool isReceivedDaily5Reward = false;
    public bool isFasleButton1Clicked = false;
    public bool isFasleButton2Clicked = false;
    public bool isFasleButton3Clicked = false;
    public bool isFasleButton4Clicked = false;
    public bool isFasleButton5Clicked = false;



    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);    
        Load();
        UpdateDailyMission();
        completeRewardCollection();

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

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetDailyMissions();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            dailyMission1Progress = dailyMission1Target;
            dailyMission2Progress = dailyMission2Target;
            dailyMission3Progress = dailyMission3Target;
            dailyMission4Progress = dailyMission4Target;
            dailyMission5Progress = dailyMission5Target;
            UpdateDailyMission();
        }

    }

    public void UpdateDailyMission()
    {
        if (!isDailyMission1Completed && !isReceivedDaily1Reward)
        {
            bool[] bools = { isDailyMission1Completed,
                             isDailyMission2Completed,
                             isDailyMission3Completed,
                             isDailyMission4Completed,
                             isDailyMission5Completed};
            dailyMission1Progress = bools.Count(b => b);

            imageFillDailyMission1.fillAmount = (float)dailyMission1Progress / dailyMission1Target;
            
            Debug.Log("Daily Mission 1 Progress: " + imageFillDailyMission1.fillAmount);
            
            textDailyMission1.text =dailyMission1Progress + "/" + dailyMission1Target;

            if (dailyMission1Progress >= dailyMission1Target)
            {
                isDailyMission1Completed = true;
                buttonDailyMission1.interactable = true;
                textDailyMission1.text = "Mission Completed"; 
                buttonDailyMission1.image.sprite = spriteButtonCompleted;
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);

                MissionManager.instance.textNotificationDailyValue++;
                MissionManager.instance.textNotificationDaily.text = MissionManager.instance.textNotificationDailyValue.ToString();
                MissionManager.instance.notificationImageDaily.gameObject.SetActive(true);
            }
        }
        if (!isDailyMission2Completed && !isReceivedDaily2Reward)
        {
            imageFillDailyMission2.fillAmount = (float)dailyMission2Progress / dailyMission2Target;
            Debug.Log("Daily Mission 2 Progress: " + imageFillDailyMission2.fillAmount);
            textDailyMission2.text = dailyMission2Progress + "/" + dailyMission2Target;

            if (dailyMission2Progress >= dailyMission2Target)
            {
                isDailyMission2Completed = true;
                buttonDailyMission2.interactable = true;
                textDailyMission2.text = "Mission Completed";
                buttonDailyMission2.image.sprite = spriteButtonCompleted;
                UpdateDailyMission();
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);

                MissionManager.instance.textNotificationDailyValue++;
                MissionManager.instance.textNotificationDaily.text = MissionManager.instance.textNotificationDailyValue.ToString();
                MissionManager.instance.notificationImageDaily.gameObject.SetActive(true);
            }
        }
        if (!isDailyMission3Completed && !isReceivedDaily3Reward)
        {
            Debug.Log("dailyMission3Progress: " + dailyMission3Progress + " / dailyMission3Target: " + dailyMission3Target);
            imageFillDailyMission3.fillAmount = (float)dailyMission3Progress / dailyMission3Target;
           // Debug.Break(); tạm dừng bên Unity để kiểm tra giá trị
            Debug.Log("Daily Mission 3 Progress: " + imageFillDailyMission3.fillAmount);
            textDailyMission3.text = dailyMission3Progress + "/" + dailyMission3Target;

            if (dailyMission3Progress >= dailyMission3Target)
            {
                isDailyMission3Completed = true;
                buttonDailyMission3.interactable = true;
                textDailyMission3.text = "Mission Completed";
                buttonDailyMission3.image.sprite = spriteButtonCompleted;
                UpdateDailyMission();
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);

                MissionManager.instance.textNotificationDailyValue++;
                MissionManager.instance.textNotificationDaily.text = MissionManager.instance.textNotificationDailyValue.ToString();
                MissionManager.instance.notificationImageDaily.gameObject.SetActive(true);
            }
        }
        if (!isDailyMission4Completed && !isReceivedDaily4Reward)
        {
            imageFillDailyMission4.fillAmount = (float)dailyMission4Progress / dailyMission4Target;
            Debug.Log("Daily Mission 4 Progress: " + imageFillDailyMission4.fillAmount);
            textDailyMission4.text = dailyMission4Progress + "/" + dailyMission4Target;

            if (dailyMission4Progress >= dailyMission4Target)
            {
                isDailyMission4Completed = true;
                buttonDailyMission4.interactable = true;
                textDailyMission4.text = "Mission Completed";
                buttonDailyMission4.image.sprite = spriteButtonCompleted;
                UpdateDailyMission();
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);

                MissionManager.instance.textNotificationDailyValue++;
                MissionManager.instance.textNotificationDaily.text = MissionManager.instance.textNotificationDailyValue.ToString();
                MissionManager.instance.notificationImageDaily.gameObject.SetActive(true);
            }
        }
        if (!isDailyMission5Completed && !isReceivedDaily5Reward)
        {
            imageFillDailyMission5.fillAmount = (float)dailyMission5Progress / dailyMission5Target;
            Debug.Log("Daily Mission 5 Progress: " + imageFillDailyMission5.fillAmount);
            textDailyMission5.text = dailyMission5Progress + "/" + dailyMission5Target;

            if (dailyMission5Progress >= dailyMission5Target)
            {
                isDailyMission5Completed = true;
                buttonDailyMission5.interactable = true;
                textDailyMission5.text = "Mission Completed";
                buttonDailyMission5.image.sprite = spriteButtonCompleted;
                UpdateDailyMission();
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);

                MissionManager.instance.textNotificationDailyValue++;
                MissionManager.instance.textNotificationDaily.text = MissionManager.instance.textNotificationDailyValue.ToString();
                MissionManager.instance.notificationImageDaily.gameObject.SetActive(true);
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

        PlayerPrefs.SetInt("IsReceivedDaily1Reward", isReceivedDaily1Reward ? 1 : 0);
        PlayerPrefs.SetInt("IsReceivedDaily2Reward", isReceivedDaily2Reward ? 1 : 0);
        PlayerPrefs.SetInt("IsReceivedDaily3Reward", isReceivedDaily3Reward ? 1 : 0);
        PlayerPrefs.SetInt("IsReceivedDaily4Reward", isReceivedDaily4Reward ? 1 : 0);
        PlayerPrefs.SetInt("IsReceivedDaily5Reward", isReceivedDaily5Reward ? 1 : 0);
    }

    public void Load()
    {
        dailyMission1Progress = PlayerPrefs.GetInt("DailyMission1Progress", 0);
        dailyMission2Progress = PlayerPrefs.GetInt("DailyMission2Progress", 0);
        dailyMission3Progress = PlayerPrefs.GetInt("DailyMission3Progress", 0);
        dailyMission4Progress = PlayerPrefs.GetInt("DailyMission4Progress", 0);
        dailyMission5Progress = PlayerPrefs.GetInt("DailyMission5Progress", 0);

        isReceivedDaily1Reward = PlayerPrefs.GetInt("IsReceivedDaily1Reward", 0) == 1;
        isReceivedDaily2Reward = PlayerPrefs.GetInt("IsReceivedDaily2Reward", 0) == 1;
        isReceivedDaily3Reward = PlayerPrefs.GetInt("IsReceivedDaily3Reward", 0) == 1;
        isReceivedDaily4Reward = PlayerPrefs.GetInt("IsReceivedDaily4Reward", 0) == 1;
        isReceivedDaily5Reward = PlayerPrefs.GetInt("IsReceivedDaily5Reward", 0) == 1;
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

        isFasleButton1Clicked = false;
        isFasleButton2Clicked = false;
        isFasleButton3Clicked = false;
        isFasleButton4Clicked = false;
        isFasleButton5Clicked = false;

        isReceivedDaily1Reward = false;
        isReceivedDaily2Reward = false;
        isReceivedDaily3Reward = false;
        isReceivedDaily4Reward = false;
        isReceivedDaily5Reward = false;

        imageDailyMission1.sprite = spriteMissionIncomplete;
        imageDailyMission2.sprite = spriteMissionIncomplete;
        imageDailyMission3.sprite = spriteMissionIncomplete;
        imageDailyMission4.sprite = spriteMissionIncomplete;
        imageDailyMission5.sprite = spriteMissionIncomplete;

        imageFillDailyMission1.sprite = spriteFillMissionIncomplete;
        imageFillDailyMission2.sprite = spriteFillMissionIncomplete;
        imageFillDailyMission3.sprite = spriteFillMissionIncomplete;
        imageFillDailyMission4.sprite = spriteFillMissionIncomplete;
        imageFillDailyMission5.sprite = spriteFillMissionIncomplete;

        buttonDailyMission1.image.sprite = spriteButtonIncomplete;
        buttonDailyMission2.image.sprite = spriteButtonIncomplete;
        buttonDailyMission3.image.sprite = spriteButtonIncomplete;
        buttonDailyMission4.image.sprite = spriteButtonIncomplete;
        buttonDailyMission5.image.sprite = spriteButtonIncomplete;

        imageBackGroundFillDailyMission1.sprite = spriteBackGroundFillMissionIncomplete;
        imageBackGroundFillDailyMission2.sprite = spriteBackGroundFillMissionIncomplete;
        imageBackGroundFillDailyMission3.sprite = spriteBackGroundFillMissionIncomplete;
        imageBackGroundFillDailyMission4.sprite = spriteBackGroundFillMissionIncomplete;
        imageBackGroundFillDailyMission5.sprite = spriteBackGroundFillMissionIncomplete;

        UpdateDailyMission();
    }

    public void buttonMission1ClaimReward()
    {
        if (isDailyMission1Completed && !isFasleButton1Clicked)
        {
            Debug.Log("Daily Mission 1 Reward Claimed!");
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            Debug.Log("Setting button sprite to claimed...");
            buttonDailyMission1.image.sprite = spriteButtonClaimed;
            Debug.Log("Button sprite set successfully: " + (buttonDailyMission1.image.sprite == spriteButtonClaimed));
            isDailyMission1Completed = false;
            isFasleButton1Clicked = true;
            imageDailyMission1.sprite = spriteMissionClaimed;
            imageFillDailyMission1.sprite = spriteFillMissionCompleted;
            isReceivedDaily1Reward = true;
            textDailyMission1.text = "Claimed";
            imageBackGroundFillDailyMission1.sprite = spriteBackGroundFillMissionCompleted;
            MissionPlane.instance.planeMission4Progress++;
            MissionPlane.instance.UpdatePlaneMission();
            MissionPlane.instance.Save();
            // thưởng kim cương
            GManager.instance.totalDiamond += 200;
            PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
            PlayerPrefs.Save();
            
            if(isReceivedDaily1Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }

                MissionManager.instance.textNotificationDailyValue--;
                MissionManager.instance.textNotificationDaily.text = MissionManager.instance.textNotificationDailyValue.ToString();
                if (MissionManager.instance.textNotificationDailyValue <= 0)
                {
                    MissionManager.instance.notificationImageDaily.gameObject.SetActive(false);
                }
            }
            Save();
        }
    }
    public void buttonMission2ClaimReward()
    {
        if (isDailyMission2Completed && !isFasleButton2Clicked)
        {
            Debug.Log("Daily Mission 2 Reward Claimed!");
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            
            buttonDailyMission2.image.sprite = spriteButtonClaimed;
            isDailyMission2Completed = false;
            isFasleButton2Clicked = true;
            imageDailyMission2.sprite = spriteMissionClaimed;
            imageFillDailyMission2.sprite = spriteFillMissionCompleted;
            isReceivedDaily2Reward = true;
            textDailyMission2.text = "Claimed";
            imageBackGroundFillDailyMission2.sprite = spriteBackGroundFillMissionCompleted; 

            GManager.instance.totalDiamond += 50;
            PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
            PlayerPrefs.Save();
            GManager.instance.SaveTotalDiamond();
            if(isReceivedDaily2Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }

                MissionManager.instance.textNotificationDailyValue--;
                MissionManager.instance.textNotificationDaily.text = MissionManager.instance.textNotificationDailyValue.ToString();
                if (MissionManager.instance.textNotificationDailyValue <= 0)
                {
                    MissionManager.instance.notificationImageDaily.gameObject.SetActive(false);
                }
            }
            Save();
        }
    }
    public void buttonMission3ClaimReward()
    {
        if (isDailyMission3Completed && !isFasleButton3Clicked)
        {
            Debug.Log("Daily Mission 3 Reward Claimed!");
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            buttonDailyMission3.image.sprite = spriteButtonClaimed;
            isDailyMission3Completed = false;
            isFasleButton3Clicked = true;
            imageDailyMission3.sprite = spriteMissionClaimed;
            imageFillDailyMission3.sprite = spriteFillMissionCompleted;
            isReceivedDaily3Reward = true;
            textDailyMission3.text = "Claimed";
            imageBackGroundFillDailyMission3.sprite = spriteBackGroundFillMissionCompleted;
            GManager.instance.totalMoney += 1500f;
            PlayerPrefs.SetFloat("TotalMoney", GManager.instance.totalMoney);
            PlayerPrefs.Save();
            GManager.instance.SaveTotalMoney();

            if(isReceivedDaily3Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }

                MissionManager.instance.textNotificationDailyValue--;
                MissionManager.instance.textNotificationDaily.text = MissionManager.instance.textNotificationDailyValue.ToString();
                if (MissionManager.instance.textNotificationDailyValue <= 0)
                {
                    MissionManager.instance.notificationImageDaily.gameObject.SetActive(false);
                }
            }
            Save();
        }
    }
    public void buttonMission4ClaimReward()
    {
        if (isDailyMission4Completed && !isFasleButton4Clicked)
        {
            Debug.Log("Daily Mission 4 Reward Claimed!");
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            buttonDailyMission4.image.sprite = spriteButtonClaimed;
            isDailyMission4Completed = false;
            isFasleButton4Clicked = true;
            imageDailyMission4.sprite = spriteMissionClaimed;
            imageFillDailyMission4.sprite = spriteFillMissionCompleted;
            isReceivedDaily4Reward = true;
            imageBackGroundFillDailyMission4.sprite = spriteBackGroundFillMissionCompleted;
            textDailyMission4.text = "Claimed";

            GManager.instance.totalDiamond += 50;
            PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
            PlayerPrefs.Save();
            GManager.instance.SaveTotalDiamond();

            if(isReceivedDaily4Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }

                MissionManager.instance.textNotificationDailyValue--;
                MissionManager.instance.textNotificationDaily.text = MissionManager.instance.textNotificationDailyValue.ToString();
                if (MissionManager.instance.textNotificationDailyValue <= 0)
                {
                    MissionManager.instance.notificationImageDaily.gameObject.SetActive(false);
                }
            }
            Save();
        }
    }
    public void buttonMission5ClaimReward()
    {
        if (isDailyMission5Completed && !isFasleButton5Clicked)
        {
            Debug.Log("Daily Mission 5 Reward Claimed!");
            AudioManager.instance.PlaySound(AudioManager.instance.rewardMissionSoundClip);
            buttonDailyMission5.image.sprite = spriteButtonClaimed;
            Debug.Log("Button sprite set successfully: " + (buttonDailyMission5.image.sprite == spriteButtonClaimed));
            isDailyMission5Completed = false;
            // buttonDailyMission5.interactable = false;
            isFasleButton5Clicked = true;
            imageDailyMission5.sprite = spriteMissionClaimed;
            imageFillDailyMission5.sprite = spriteFillMissionCompleted;
            isReceivedDaily5Reward = true;
            textDailyMission5.text = "Claimed";
            imageBackGroundFillDailyMission5 .sprite = spriteBackGroundFillMissionCompleted;
            GManager.instance.totalMoney += 2000f;
            PlayerPrefs.SetFloat("TotalMoney", GManager.instance.totalMoney);
            PlayerPrefs.Save();
            GManager.instance.SaveTotalMoney();
            if(isReceivedDaily5Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }

                MissionManager.instance.textNotificationDailyValue--;
                MissionManager.instance.textNotificationDaily.text = MissionManager.instance.textNotificationDailyValue.ToString();
                if (MissionManager.instance.textNotificationDailyValue <= 0)
                {
                    MissionManager.instance.notificationImageDaily.gameObject.SetActive(false);
                }
            }
            UpdateDailyMission();
            Save();
        }
    }

    public void completeRewardCollection(){
        if(isReceivedDaily1Reward){
            // buttonDailyMission1.interactable = false;
            isFasleButton1Clicked = true;
            imageDailyMission1.sprite = spriteMissionClaimed;
            imageFillDailyMission1.sprite = spriteFillMissionCompleted;
            buttonDailyMission1.image.sprite = spriteButtonClaimed;
            imageBackGroundFillDailyMission1.sprite = spriteBackGroundFillMissionCompleted;
            textDailyMission1.text = "Claimed";
        }
        if(isReceivedDaily2Reward){
            // buttonDailyMission2.interactable = false;
            isFasleButton2Clicked = true;
            imageDailyMission2.sprite = spriteMissionClaimed;
            imageFillDailyMission2.sprite = spriteFillMissionCompleted;
            buttonDailyMission2.image.sprite = spriteButtonClaimed;
            imageBackGroundFillDailyMission2.sprite = spriteBackGroundFillMissionCompleted;
            textDailyMission2.text = "Claimed";
        }
        if(isReceivedDaily3Reward){
            // buttonDailyMission3.interactable = false;
            isFasleButton3Clicked = true;
            imageDailyMission3.sprite = spriteMissionClaimed;
            imageFillDailyMission3.sprite = spriteFillMissionCompleted;
            buttonDailyMission3.image.sprite = spriteButtonClaimed;
            imageBackGroundFillDailyMission3.sprite = spriteBackGroundFillMissionCompleted;
            textDailyMission3.text = "Claimed";
        }
        if(isReceivedDaily4Reward){
            // buttonDailyMission4.interactable = false;
            isFasleButton4Clicked = true;
            imageDailyMission4.sprite = spriteMissionClaimed;
            imageFillDailyMission4.sprite = spriteFillMissionCompleted;
            buttonDailyMission4.image.sprite = spriteButtonClaimed;
            imageBackGroundFillDailyMission4.sprite = spriteBackGroundFillMissionCompleted;
            textDailyMission4.text = "Claimed";
        }
        if(isReceivedDaily5Reward){
            // buttonDailyMission5.interactable = false;
            isFasleButton5Clicked = true;
            imageDailyMission5.sprite = spriteMissionClaimed;
            imageFillDailyMission5.sprite = spriteFillMissionCompleted;
            buttonDailyMission5.image.sprite = spriteButtonClaimed;
            imageBackGroundFillDailyMission5.sprite = spriteBackGroundFillMissionCompleted;
            textDailyMission5.text = "Claimed";
        }
    }
}