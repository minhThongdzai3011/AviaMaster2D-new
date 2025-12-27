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

    [Header("Mission Plane Image Fill")]
    public Image imageFillPlaneMission1;
    public Image imageFillPlaneMission2;
    public Image imageFillPlaneMission3;
    public Image imageFillPlaneMission4;
    public Image imageFillPlaneMission5;

    [Header("Mission Plane Button")]
    public Button buttonPlaneMission1;
    public Button buttonPlaneMission2;
    public Button buttonPlaneMission3;
    public Button buttonPlaneMission4;
    public Button buttonPlaneMission5;
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

    [Header("Information Super Plane")]
    public Image superPlaneInfoImage;
    public GameObject superPlaneInfoPlane1; 
    public GameObject superPlaneInfoPlane2;
    public GameObject superPlaneInfoPlane3;
    public GameObject superPlaneInfoPlane4;
    public GameObject superPlaneInfoPlane5;

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

        


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            planeMission1Progress = 100;
            planeMission2Progress = 100;
            planeMission3Progress = 100;
            planeMission4Progress = 100;
            planeMission5Progress = 100;
            UpdatePlaneMission();
        }

    }

    public void UpdatePlaneMission()
    {
        if (!isPlaneMission1Completed)
        {

            imageFillPlaneMission1.fillAmount = (float)planeMission1Progress / planeMission1Target;
            Debug.Log("Plane Mission 1 Progress: " + imageFillPlaneMission1.fillAmount);
            if(!isReceivedPlane1Reward){
                textPlaneMission1.text = planeMission1Progress + "/" + planeMission1Target;
            }
            else{
                textPlaneMission1.text = "Claimed";
            }

            if (planeMission1Progress >= planeMission1Target)
            {
                isPlaneMission1Completed = true;
                buttonPlaneMission1.interactable = true;
                textPlaneMission1.text = "Mission Completed";
                imagePlaneMission1.color = Color.yellow;
                buttonPlaneMission1.image.color = Color.yellow;
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);
            }
        }
        if (!isPlaneMission2Completed)
        {
            imageFillPlaneMission2.fillAmount = (float)planeMission2Progress / planeMission2Target;
            Debug.Log("Plane Mission 2 Progress: " + imageFillPlaneMission2.fillAmount);
            if(!isReceivedPlane2Reward){
                textPlaneMission2.text = planeMission2Progress + "/" + planeMission2Target;
            }
            else{
                textPlaneMission2.text = "Claimed";
            }

            if (planeMission2Progress >= planeMission2Target)
            {
                isPlaneMission2Completed = true;
                buttonPlaneMission2.interactable = true;
                textPlaneMission2.text = "Mission Completed";
                imagePlaneMission2.color = Color.yellow;
                buttonPlaneMission2.image.color = Color.yellow;
                UpdatePlaneMission();
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);
            }
        }
        if (!isPlaneMission3Completed)
        {
            imageFillPlaneMission3.fillAmount = (float)planeMission3Progress / planeMission3Target;
           // Debug.Break(); tạm dừng bên Unity để kiểm tra giá trị
            Debug.Log("Plane Mission 3 Progress: " + imageFillPlaneMission3.fillAmount);
            if(!isReceivedPlane3Reward){
                textPlaneMission3.text = planeMission3Progress + "/" + planeMission3Target;
            }
            else{
                textPlaneMission3.text = "Claimed";
            }

            if (planeMission3Progress >= planeMission3Target)
            {
                isPlaneMission3Completed = true;
                buttonPlaneMission3.interactable = true;
                textPlaneMission3.text = "Mission Completed";
                imagePlaneMission3.color = Color.yellow;
                buttonPlaneMission3.image.color = Color.yellow;
                UpdatePlaneMission();
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);
            }
        }
        if (!isPlaneMission4Completed)
        {
            imageFillPlaneMission4.fillAmount = (float)planeMission4Progress / planeMission4Target;
            Debug.Log("Plane Mission 4 Progress: " + imageFillPlaneMission4.fillAmount);
            if(!isReceivedPlane4Reward){
                textPlaneMission4.text = planeMission4Progress + "/" + planeMission4Target;
            }
            else{
                textPlaneMission4.text = "Claimed";
            }

            if (planeMission4Progress >= planeMission4Target)
            {
                isPlaneMission4Completed = true;
                buttonPlaneMission4.interactable = true;
                textPlaneMission4.text = "Mission Completed";
                imagePlaneMission4.color = Color.yellow;
                buttonPlaneMission4.image.color = Color.yellow;
                UpdatePlaneMission();
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);
            }
        }
        if (!isPlaneMission5Completed)
        {
            imageFillPlaneMission5.fillAmount = (float)planeMission5Progress / planeMission5Target;
            Debug.Log("Plane Mission 5 Progress: " + imageFillPlaneMission5.fillAmount);
            if(!isReceivedPlane5Reward){
                textPlaneMission5.text = planeMission5Progress + "/" + planeMission5Target;
            }
            else{
                textPlaneMission5.text = "Claimed";
            }

            if (planeMission5Progress >= planeMission5Target)
            {
                isPlaneMission5Completed = true;
                buttonPlaneMission5.interactable = true;
                textPlaneMission5.text = "Mission Completed";
                imagePlaneMission5.color = Color.yellow;
                buttonPlaneMission5.image.color = Color.yellow;
                UpdatePlaneMission();
                
                MissionManager.instance.textQuantityRewardValue++;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                MissionManager.instance.notificationImage.gameObject.SetActive(true);
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

        buttonPlaneMission1.interactable = false;
        buttonPlaneMission2.interactable = false;
        buttonPlaneMission3.interactable = false;
        buttonPlaneMission4.interactable = false;
        buttonPlaneMission5.interactable = false;

        isReceivedPlane1Reward = false;
        isReceivedPlane2Reward = false;
        isReceivedPlane3Reward = false;
        isReceivedPlane4Reward = false;
        isReceivedPlane5Reward = false;

        imagePlaneMission1.color = Color.gray;
        imagePlaneMission2.color = Color.gray;
        imagePlaneMission3.color = Color.gray;
        imagePlaneMission4.color = Color.gray;
        imagePlaneMission5.color = Color.gray;

        buttonPlaneMission1.image.color = Color.white;
        buttonPlaneMission2.image.color = Color.white;
        buttonPlaneMission3.image.color = Color.white;
        buttonPlaneMission4.image.color = Color.white;
        buttonPlaneMission5.image.color = Color.white;

        UpdatePlaneMission();
    }

    public void buttonMission1ClaimReward()
    {
        if (isPlaneMission1Completed)
        {
            Debug.Log("Plane Mission 1 Reward Claimed!");
            isPlaneMission1Completed = false;
            planeMission1Progress = 0;
            buttonPlaneMission1.interactable = false;
            imagePlaneMission1.color = Color.gray;
            buttonPlaneMission1.image.color = Color.gray;
            isReceivedPlane1Reward = true;
            textPlaneMission1.text = "Claimed";

            if(isReceivedPlane1Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }
            }
            UpdatePlaneMission();
            Save();
        }
    }
    public void buttonMission2ClaimReward()
    {
        if (isPlaneMission2Completed)
        {
            Debug.Log("Plane Mission 2 Reward Claimed!");
            isPlaneMission2Completed = false;
            planeMission2Progress = 0;
            buttonPlaneMission2.interactable = false;
            imagePlaneMission2.color = Color.gray;
            buttonPlaneMission2.image.color = Color.gray;
            isReceivedPlane2Reward = true;
            textPlaneMission2.text = "Claimed";

            if(isReceivedPlane2Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }
            }
            UpdatePlaneMission();
            Save();
        }
    }
    public void buttonMission3ClaimReward()
    {
        if (isPlaneMission3Completed)
        {
            Debug.Log("Plane Mission 3 Reward Claimed!");
            isPlaneMission3Completed = false;
            planeMission3Progress = 0;
            buttonPlaneMission3.interactable = false;
            imagePlaneMission3.color = Color.gray;
            buttonPlaneMission3.image.color = Color.gray;
            isReceivedPlane3Reward = true;
            textPlaneMission3.text = "Claimed";

            if(isReceivedPlane3Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }
            }
            UpdatePlaneMission();
            Save();
        }
    }
    public void buttonMission4ClaimReward()
    {
        if (isPlaneMission4Completed)
        {
            Debug.Log("Plane Mission 4 Reward Claimed!");
            isPlaneMission4Completed = false;
            planeMission4Progress = 0;
            buttonPlaneMission4.interactable = false;
            imagePlaneMission4.color = Color.gray;
            buttonPlaneMission4.image.color = Color.gray;
            isReceivedPlane4Reward = true;
            textPlaneMission4.text = "Claimed";

            if(isReceivedPlane4Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }
            }
            UpdatePlaneMission();
            Save();
        }
    }
    public void buttonMission5ClaimReward()
    {
        if (isPlaneMission5Completed)
        {
            Debug.Log("Plane Mission 5 Reward Claimed!");
            isPlaneMission5Completed = false;
            planeMission5Progress = 0;
            buttonPlaneMission5.interactable = false;
            imagePlaneMission5.color = Color.gray;
            buttonPlaneMission5.image.color = Color.gray;
            isReceivedPlane5Reward = true;
            textPlaneMission5.text = "Claimed";

            if(isReceivedPlane5Reward)
            {
                MissionManager.instance.textQuantityRewardValue--;
                MissionManager.instance.textQuantityReward.text = MissionManager.instance.textQuantityRewardValue.ToString();
                if (MissionManager.instance.textQuantityRewardValue <= 0)
                {
                    MissionManager.instance.notificationImage.gameObject.SetActive(false);
                }
            }
            UpdatePlaneMission();
            Save();
        }
    }

    public void completeRewardCollection(){
        if(isReceivedPlane1Reward){
            buttonPlaneMission1.interactable = false;
            imagePlaneMission1.color = Color.gray;
            buttonPlaneMission1.image.color = Color.gray;
            textPlaneMission1.text = "Claimed";
        }
        if(isReceivedPlane2Reward){
            buttonPlaneMission2.interactable = false;
            imagePlaneMission2.color = Color.gray;
            buttonPlaneMission2.image.color = Color.gray;
            textPlaneMission2.text = "Claimed";
        }
        if(isReceivedPlane3Reward){
            buttonPlaneMission3.interactable = false;
            imagePlaneMission3.color = Color.gray;
            buttonPlaneMission3.image.color = Color.gray;
            textPlaneMission3.text = "Claimed";
        }
        if(isReceivedPlane4Reward){
            buttonPlaneMission4.interactable = false;
            imagePlaneMission4.color = Color.gray;
            buttonPlaneMission4.image.color = Color.gray;
            textPlaneMission4.text = "Claimed";
        }
        if(isReceivedPlane5Reward){
            buttonPlaneMission5.interactable = false;
            imagePlaneMission5.color = Color.gray;
            buttonPlaneMission5.image.color = Color.gray;
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