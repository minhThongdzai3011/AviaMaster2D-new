using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Import DOTween


public class Settings : MonoBehaviour
{
    public static Settings instance;

    [Header("Text Settings")]
    public TextMeshProUGUI musicText;
    public TextMeshProUGUI soundText;
    public TextMeshProUGUI distanceMoneyText;
    public TextMeshProUGUI collectMoneyText;
    public TextMeshProUGUI bonusText;
    public TextMeshProUGUI totalMoneyPlayText;
    public TextMeshProUGUI lastDistanceText;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI prizeText;
    public TextMeshProUGUI altitudeText;
    public TextMeshProUGUI NotificationNewMapText;
    public TextMeshProUGUI x2CoinText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI prizeChestText;
    public TextMeshProUGUI prizeNextChestText;
    public TextMeshProUGUI value1Text;
    public TextMeshProUGUI value2Text;
    public TextMeshProUGUI value3Text;
    public TextMeshProUGUI value4Text;
    public TextMeshProUGUI superBonusText;
    
    [Header("Image Settings")]
    public Image musicImageOn;
    public Image musicImageOff;
    public Image soundImageOn;
    public Image soundImageOff;
    public Image settingsImage;
    public Image winImage;
    public Image luckyWheelImage;
    public Image jackpotImage;
    public Image pannel;
    public Image prizeImage;
    public Image prizeChestImage;
    public Image altitudeImage;
    public Image iamgeBlackScreen;
    public RectTransform NotificationNewMapImage;
    public Image settingImage;
    public Image ImageErrorAngleZ;
    public Image pannelGray;
    public Image imagex2Fuel;
    public Image imagex2Power;
    public Image imageFuelPlay;
    public Image imageFill;
    public Image imageDiamondCoinText;
    public Image imageHighScore;
    public Image imageGuide;

    [Header("Button Settings")]
    public Button AdsButton;
    public Button AdsLuckyWheelButton;
    public Button butttonExitImageWin;
    
    [Header("Bool Settings")]
    public bool isMusicOn = true;
    public bool isSoundOn = true;
    
    [Header("DOTween Settings")]
    public float openDuration = 0.5f;
    public float closeDuration = 0.3f;
    public Ease openEase = Ease.OutBack;
    public Ease closeEase = Ease.InBack;
    public float targetValue = 0f;
    private bool isAnimating = false;
    public bool isSpinning = true;
    public bool isButtonChestClickeds = false;
    
    [Header("Slider Settings")]
    public Image prizeSlider;

    [Header("Input Settings")]
    public TMP_InputField inputField;

    [Header ("Countdown Settings")]
    public int currentTime = 0;
    [Header("Sprite Settings")]
    public Sprite spriteCoin;
    public Sprite spriteDiamond;
    public Sprite spriteUIMask;

    void Start()
    {
        instance = this;
       // playerName = PlayerPrefs.GetString("player-name", "");
        Debug.Log("Loaded HighScore: " + (int)PlayerPrefs.GetInt("HighScore", 0));
        highScoreText.text = " " + (int)PlayerPrefs.GetInt("HighScore", 0);
        
        //inputField.text = playerName;
        isColdDownTimeLuckyWheel = PlayerPrefs.GetInt("LuckyWheelColdDownAds", 0) == 1;
        if (isColdDownTimeLuckyWheel)
        {
            AdsLuckyWheelButton.gameObject.SetActive(false);
        }
        
        currentTime = PlayerPrefs.GetInt("SaveTime", 10);
        
        // Load chest tier (giới hạn tối đa là 4)
        chestTier = PlayerPrefs.GetInt("ChestTier", 1);
        if (chestTier > 4) chestTier = 4;
        Debug.Log($"Loaded ChestTier: {chestTier} - Requirement: {GetCurrentRequirement()}coin -> Reward: {GetCurrentReward()}coin");

        if (currentTime < 10 && currentTime > 0) 
        {
            resultText.text = "Waiting...";
            isSpinning = false;
            StartCountdown(); 
        }
        else if (currentTime <= 0)
        {
            resultText.text = "Spin";
            isSpinning = true;
            AdsLuckyWheelButton.gameObject.SetActive(false);
        }
        else
        {
            resultText.text = "Spin";
            isSpinning = true;
        }
        // if (currentTime == 0)
        // {
        //     AdsLuckyWheelButton.gameObject.SetActive(false);
        // }

        // Load settings từ PlayerPrefs
        LoadSettings();

        // Cập nhật UI ban đầu
        UpdateUI();

        Debug.Log("Settings initialized. Music: " + isMusicOn + ", Sound: " + isSoundOn);
        targetValue = PlayerPrefs.GetFloat("PrizeSliderValue", 0f);
        prizeSlider.fillAmount = targetValue;
        StartCountdown();
        GManager.instance.coinEffect.Stop();
    }
    public bool isAltitudeImageActive = true;
    public void Update()
    {
        if (isPrizeCoin)
        {
            UpdateSliderPrizeCoin();
        }
        altitudeText.text = "" + (int)GManager.instance.currentAltitude + " m";
        if (GManager.instance.currentAltitude >= 30 && isAltitudeImageActive)
        {
            altitudeImage.gameObject.SetActive(true);
        }
        else
        {
            altitudeImage.gameObject.SetActive(false);
        }
        saveNameInputField();
    }

    void LoadSettings()
    {
        // Load settings từ PlayerPrefs (mặc định là true)
        isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        isSoundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
    }
    
    void SaveSettings()
    {
        // Lưu settings vào PlayerPrefs
        PlayerPrefs.SetInt("MusicOn", isMusicOn ? 1 : 0);
        PlayerPrefs.SetInt("SoundOn", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    void UpdateUI()
    {
        // Cập nhật UI Music
        if (isMusicOn)
        {
            musicText.text = "Music on";
            musicImageOn.gameObject.SetActive(true);
            musicImageOff.gameObject.SetActive(false);
        }
        else
        {
            musicText.text = "Music off";
            musicImageOn.gameObject.SetActive(false);
            musicImageOff.gameObject.SetActive(true);
        }
        
        // Cập nhật UI Sound
        if (isSoundOn)
        {
            soundText.text = "Sound on";
            soundImageOn.gameObject.SetActive(true);
            soundImageOff.gameObject.SetActive(false);
        }
        else
        {
            soundText.text = "Sound off";
            soundImageOn.gameObject.SetActive(false);
            soundImageOff.gameObject.SetActive(true);
        }
    }

    public void OpenSettings()
    {
        if (isAnimating) return;
        AudioManager.instance.PlaySound(AudioManager.instance.buttonSoundClip);
        pannelGray.gameObject.SetActive(true);
        Debug.Log("Mở Settings!");
        
        // Dừng tất cả animation đang chạy
        DOTween.Kill(settingsImage.transform);
        isAnimating = true;
        
        // Hiển thị settings panel
        lastDistanceText.gameObject.SetActive(false);
        settingsImage.gameObject.SetActive(true);
        
        // Cập nhật UI trước khi hiển thị
        UpdateUI();
        
        // Animation mở settings (giống ShopImage)
        settingsImage.transform.localScale = Vector3.zero;
        settingsImage.transform.DOScale(Vector3.one, openDuration)
            .SetEase(openEase)
            .OnComplete(() => {
                isAnimating = false;
                Debug.Log("Settings animation mở hoàn thành!");
            });
    }

    public void CloseSettings()
    {
        if (isAnimating) return;
        if (Settings.instance == null)
        {
            Debug.LogError("Settings.instance is null!");
            return;
        }

        AudioManager.instance.PlaySound(AudioManager.instance.exitSoundClip);

        Debug.Log("Đóng Settings!");

        // Dừng tất cả animation đang chạy
        DOTween.Kill(settingsImage.transform);
        isAnimating = true;

        // Lưu settings trước khi đóng
        SaveSettings();

        // Animation đóng settings (giống ShopImage)
        settingsImage.transform.DOScale(Vector3.zero, closeDuration)
            .SetEase(closeEase)
            .OnComplete(() =>
            {
                
                lastDistanceText.gameObject.SetActive(true);
                settingsImage.gameObject.SetActive(false);
                isAnimating = false;
                pannelGray.gameObject.SetActive(false);
                Debug.Log("Settings animation đóng hoàn thành!");

            });
    }


    public void ToggleMusic()
    {
        if (isAnimating) return;
        
        Debug.Log("Toggling Music. Current state: " + isMusicOn);
        
        // Đổi trạng thái
        isMusicOn = !isMusicOn;
        
        // Animation cho button music
        StartCoroutine(AnimateToggle(isMusicOn, musicImageOn, musicImageOff, musicText, "Music"));
        
        AudioManager.instance.SetMusicVolume();
    }

    public void ToggleSound()
    {
        if (isAnimating) return;
        
        Debug.Log("Toggling Sound. Current state: " + isSoundOn);
        
        // Đổi trạng thái
        isSoundOn = !isSoundOn;
        
        // Animation cho button sound
        StartCoroutine(AnimateToggle(isSoundOn, soundImageOn, soundImageOff, soundText, "Sound"));
        
        AudioManager.instance.SetSoundVolume();
    }
    
    IEnumerator AnimateToggle(bool isOn, Image imageOn, Image imageOff, TextMeshProUGUI text, string settingName)
    {
        // Phase 1: Scale down hiện tại
        Transform currentTransform = isOn ? imageOff.transform : imageOn.transform;
        currentTransform.DOScale(0f, 0.15f).SetEase(Ease.InCubic);
        
        yield return new WaitForSeconds(0.15f);
        
        // Phase 2: Thay đổi UI
        if (isOn)
        {
            text.text = settingName + " on";
            imageOn.gameObject.SetActive(true);
            imageOff.gameObject.SetActive(false);
            
            // Animation scale up cho imageOn
            imageOn.transform.localScale = Vector3.zero;
            imageOn.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
        else
        {
            text.text = settingName + " off";
            imageOn.gameObject.SetActive(false);
            imageOff.gameObject.SetActive(true);
            
            // Animation scale up cho imageOff
            imageOff.transform.localScale = Vector3.zero;
            imageOff.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
        
        // Phase 3: Text animation
        text.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 2, 0.5f);
        
        Debug.Log(settingName + " toggled to: " + (isOn ? "ON" : "OFF"));
    }
    
    // Hàm để gọi từ UI Button
    public void OnSettingsButtonClick()
    {
        if (settingsImage.gameObject.activeInHierarchy)
        {
            CloseSettings();
            Debug.Log("Settings button clicked - closing settings.");
        }
        else
        {
            OpenSettings();
            Debug.Log("Settings button clicked - opening settings.");
        }
    }
    
    void OnDestroy()
    {
        // Cleanup DOTween để tránh memory leak
        DOTween.Kill(this);
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveSettings();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveSettings();
        }
    }

    public void OpenWinImage()
    {
        if (isAnimating) return;

        Debug.Log("Mở Win Image!");
        GManager.instance.coinEffect.Stop();
        // Dừng tất cả animation đang chạy
        DOTween.Kill(winImage.transform);
        isAnimating = true;

        // Hiển thị win image
        lastDistanceText.gameObject.SetActive(false);
        Debug.Log("lastDistanceText set to inactive");
        winImage.gameObject.SetActive(true);

        // Cập nhật UI trước khi hiển thị
        UpdateUI();

        // Animation mở win image
        winImage.transform.localScale = Vector3.zero;
        winImage.transform.DOScale(Vector3.one, openDuration)
            .SetEase(openEase)
            .OnComplete(() =>
            {
                isAnimating = false;
                StartCountingCoin();
                Debug.Log("Win Image animation mở hoàn thành!");
                
            });
    }
    
    public bool isCloseWinImage = false;

    public void CloseWinImage()
    {
        if (isAnimating) return;

        if (!isCloseWinImage)
        {
            Debug.Log("Không được đóng WinImage vì chưa tính tiền xong!");
            return;
        }
        AudioManager.instance.PlaySound(AudioManager.instance.exitSoundClip);

        isAnimating = true;

        winImage.transform.DOScale(Vector3.zero, closeDuration)
            .SetEase(closeEase)
            .OnComplete(() =>
            {
                GManager.instance.totalMoney += (int)totalValue;
                PlayerPrefs.SetFloat("TotalMoney", GManager.instance.totalMoney);
                PlayerPrefs.Save();

                lastDistanceText.gameObject.SetActive(true);
                winImage.gameObject.SetActive(false);
                isAnimating = false;
                
                Plane.instance.moneyCollect = 0;
                Plane.instance.moneyDistance = 0;
                Plane.instance.moneyTotal1 = 0;
                GManager.instance.AgainGame();

                Debug.Log("WinImage đã đóng!");
            });
    }

    public bool isOpenLuckyWheel = false;
    public void openWheel()
    {
        if (isAnimating) return;
        pannelGray.gameObject.SetActive(true);
        isOpenLuckyWheel = true;
        AudioManager.instance.PlaySound(AudioManager.instance.buttonSoundClip);
        Debug.Log("Mở Win Image!");
        CheckPlane.instance.SetActiveLuckyWheel();
        // Dừng tất cả animation đang chạy
        DOTween.Kill(luckyWheelImage.transform);
        isAnimating = true;

        // Hiển thị lucky wheel image
        lastDistanceText.gameObject.SetActive(false);
        luckyWheelImage.gameObject.SetActive(true);

        // Cập nhật UI trước khi hiển thị
        UpdateUI();

        // Animation mở lucky wheel image
        luckyWheelImage.transform.localScale = Vector3.zero;
        luckyWheelImage.transform.DOScale(Vector3.one, openDuration)
            .SetEase(openEase)
            .OnComplete(() => {
                isAnimating = false;
                Debug.Log("Lucky Wheel Image animation mở hoàn thành!");
            });
    }

    public void closeWheel()
    {
        if (isAnimating) return;
        AudioManager.instance.PlaySound(AudioManager.instance.buttonSoundClip);
        if (Settings.instance == null)
        {
            Debug.LogError("Settings.instance is null!");
            return;
        }
        
        Debug.Log("Đóng Lucky Wheel Image!");

        // Dừng tất cả animation đang chạy
        DOTween.Kill(luckyWheelImage.transform);
        isAnimating = true;

        // Animation đóng lucky wheel image
        luckyWheelImage.transform.DOScale(Vector3.zero, closeDuration)
            .SetEase(closeEase)
            .OnComplete(() =>
            {
                lastDistanceText.gameObject.SetActive(true);
                luckyWheelImage.gameObject.SetActive(false);
                isAnimating = false;
                pannelGray.gameObject.SetActive(false);
                CheckPlane.instance.ResetActiveLuckyWheel();
                Debug.Log("Lucky Wheel Image animation đóng hoàn thành!");
                isOpenLuckyWheel = false;
            });
    }

    public void leaderBoard()
    {
        
        if (isAnimating) return;
        if (!isOpenLuckyWheel)
        {
            pannelGray.gameObject.SetActive(true);
            AudioManager.instance.PlaySound(AudioManager.instance.buttonSoundClip);
            Debug.Log("Mở Win Image!");

            CheckPlane.instance.SetActiveLeaderBoard();
            // Dừng tất cả animation đang chạy
            DOTween.Kill(GManager.instance.leaderBoardImage.transform);
            isAnimating = true;

            // Hiển thị lucky wheel image
            lastDistanceText.gameObject.SetActive(false);
            GManager.instance.leaderBoardImage.gameObject.SetActive(true);

            // Cập nhật UI trước khi hiển thị
            UpdateUI();

            // Animation mở lucky wheel image
            GManager.instance.leaderBoardImage.transform.localScale = Vector3.zero;
            GManager.instance.leaderBoardImage.transform.DOScale(Vector3.one, openDuration)
                .SetEase(openEase)
                .OnComplete(() => {
                    isAnimating = false;
                    Debug.Log("Lucky Wheel Image animation mở hoàn thành!");
                });
        }
    }

    public void exitLeaderBoard()
    {
        if (isAnimating) return;
        AudioManager.instance.PlaySound(AudioManager.instance.exitSoundClip);
        if (Settings.instance == null)
        {
            Debug.LogError("Settings.instance is null!");
            return;
        }

        Debug.Log("Đóng Lucky Wheel Image!");

        // Dừng tất cả animation đang chạy
        DOTween.Kill(GManager.instance.leaderBoardImage.transform);
        isAnimating = true;

        // Animation đóng lucky wheel image
        GManager.instance.leaderBoardImage.transform.DOScale(Vector3.zero, closeDuration)
            .SetEase(closeEase)
            .OnComplete(() =>
            {
                lastDistanceText.gameObject.SetActive(true);
                GManager.instance.leaderBoardImage.gameObject.SetActive(false);
                isAnimating = false;
                CheckPlane.instance.ResetActiveLeaderBoard();
                pannelGray.gameObject.SetActive(false);
                Debug.Log("Lucky Wheel Image animation đóng hoàn thành!");
            });
    }


    public bool isPrizeCoin = false;
    bool isCheckAddTargetValue = true;
    public bool isPrizeCoinDone = true;
    
    [Header("Chest Tier System")]
    public int chestTier = 1; 
    public float[] chestRequirements = { 500f, 1000f, 2000f, 2000f };
    public int[] chestRewards = { 2000, 5000, 12000, 15000 };     
    void UpdateSliderPrizeCoin()
    {
        if (prizeSlider != null)
        {
            float maxDistanceCoin = GetCurrentRequirement();

            if (Plane.instance == null)
            {
                Debug.LogError("Plane.instance is null!");
                return;
            }
            float temp = targetValue;

            if (isCheckAddTargetValue)
            {
                targetValue = temp + Mathf.Clamp01(totalValue / maxDistanceCoin);
                isCheckAddTargetValue = false;
            }

            PlayerPrefs.SetFloat("PrizeSliderValue", targetValue);
            PlayerPrefs.Save();
            // Cập nhật slider với hiệu ứng mượt mà
            float smoothSpeed = 2f; 
            prizeSlider.fillAmount = Mathf.Lerp(prizeSlider.fillAmount, targetValue, smoothSpeed * Time.deltaTime);
            if (prizeSlider. fillAmount >= 1f)
            {
                isShakeZ = true;
                ShakeZ();
                if (isPrizeCoinDone)
                {
                    isButtonChestClickeds = true;
                    isPrizeCoinDone = false;
                }
            }
        }
    }
    
    public Image chestImage;
    public float duration = 5f;
    public float strength = 15f;
    public int vibrato = 10;
    public bool isShakeZ = false;
    public bool isShake = true;


    private Tween shakeTween;

    public void ShakeZ()
    {
        if (!isShakeZ || !isShake) return;

        RectTransform rect = chestImage.rectTransform;

        // Kill tween cũ nếu có
        shakeTween?.Kill();

        // Lắc LIÊN TỤC trong 2 giây
        Tween shake2s = rect.DOShakeRotation(
                0.2f,                    
                new Vector3(0, 0, 5),    
                10,
                90,
                false
            )
            .SetEase(Ease.OutQuad)
            .SetLoops(10, LoopType.Restart); 

        shakeTween = DOTween.Sequence()
            .Append(shake2s)
            .AppendInterval(1f)          
            .SetLoops(-1, LoopType.Restart);

        isShakeZ = false;
        isShake = false;
        isButtonChestClickeds = true;
    }




    // IEnumerator StopShakeZ()
    // {
    //     while (isShakeZ)
    //     {
    //         yield return new WaitForSeconds(1f);
    //         isShakeZ = false;
    //         yield return new WaitForSeconds(1f);
    //         isShakeZ = true;
    //     }
    // }

    public float totalValue = 0f;

    public void StartCountingCoin()
    {
        float distanceValue = Plane.instance.moneyDistance;
        float collectValue  = Plane.instance.moneyCollect;
        MissionPlane.instance.planeMission1Progress += ((int)distanceValue);
        MissionPlane.instance.UpdatePlaneMission();

        if(SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane3 )
        {
            distanceValue *= 1.5f;
            collectValue *= 1.5f;
            Debug.Log("Kích hoạt Super Plane 5: Tiền thưởng gấp đôi!");
        }

        float bonusDuration = 1f;

        totalValue = distanceValue + collectValue;

        if (GManager.instance.isBonus && !GManager.instance.isSuperBonus)
        {
            totalValue *= 2;
            bonusText.text = "Bonus : x2";
            GManager.instance.isBonus = false;
            GManager.instance.isSuperBonus = false;
        }
        else if (GManager.instance.isSuperBonus)
        {
            totalValue *= 5;
            bonusText.color = new Color32(156, 147, 0, 255);
            bonusText.text = "Super Bonus : x5";
            GManager.instance.isSuperBonus = false;
            GManager.instance.isBonus = false;
            MissionAchievements.instance.achievementMission3Progress++;
            MissionAchievements.instance.UpdateAchievementMission();
        }

        else bonusText.text = "No Bonus";

        Sequence seq = DOTween.Sequence();
        AudioManager.instance.PlaySound(AudioManager.instance.countMoneySoundClip);

        if (SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane3)
        {
            distanceMoneyText.color = new Color32(156, 147, 0, 255);
            collectMoneyText.color = new Color32(156, 147, 0, 255);
            totalMoneyPlayText.color = new Color32(156, 147, 0, 255);
            // 1. count distance
            seq.Append(DOVirtual.Float(0, distanceValue, 2f, v =>
            {
                distanceMoneyText.text = ((int)v).ToString();
            }));

            // 2. count collect
            seq.Join(DOVirtual.Float(0, collectValue, 2f, v =>
            {
                collectMoneyText.text = ((int)v).ToString();
            }));

            AudioManager.instance.StopPlayerSound();
            AudioManager.instance.PlaySound(AudioManager.instance.countMoneySoundClip);
            // 3. count total
            seq.Join(DOVirtual.Float(0, totalValue, bonusDuration, v =>
            {
                totalMoneyPlayText.text = ((int)v).ToString();
            }));
        }
        else
        {
            // 1. count distance
            seq.Append(DOVirtual.Float(0, distanceValue, 2f, v =>
            {
                distanceMoneyText.text = ((int)v).ToString();
            }));

            // 2. count collect
            seq.Join(DOVirtual.Float(0, collectValue, 2f, v =>
            {
                collectMoneyText.text = ((int)v).ToString();
            }));

            AudioManager.instance.StopPlayerSound();
            AudioManager.instance.PlaySound(AudioManager.instance.countMoneySoundClip);
            // 3. count total
            seq.Join(DOVirtual.Float(0, totalValue, bonusDuration, v =>
            {
                totalMoneyPlayText.text = ((int)v).ToString();
            }));
        }


        Debug.Log("Total Value: " + totalValue);
        isPrizeCoin = true;

        seq.OnComplete(() =>
        {
            isCloseWinImage = true;
            butttonExitImageWin.gameObject.SetActive(true);
            

            Debug.Log("Cho phép đóng WinImage!");
        });
    }


    public bool isCountingDown = false;
    public void StartCountdown()
    {
        if (!isCountingDown) 
        {
            isCountingDown = true;
            StartCoroutine(CountdownCoroutine());
            Debug.Log("Countdown coroutine started!");
        }
    }

    public IEnumerator CountdownCoroutine()
    {
        while (currentTime > 0)
        {
            int minutes = currentTime / 60;
            int seconds = currentTime % 60;

            resultText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            
            PlayerPrefs.SetInt("SaveTime", currentTime);
            PlayerPrefs.Save();

            yield return new WaitForSeconds(1f);
            currentTime--;
        }

        Debug.Log("Countdown finished!");
        
        resultText.text = "Spin";
        isSpinning = true;
        isCountingDown = false;
        
        if (AdsLuckyWheelButton != null)
        {
            AdsLuckyWheelButton.gameObject.SetActive(false);
        }
        
        isColdDownTimeLuckyWheel = false;
        PlayerPrefs.SetInt("LuckyWheelColdDownAds", 0);
        PlayerPrefs.Save();
    }
    // public void Exitpannel()
    // {
    //     AudioManager.instance.PlaySound(AudioManager.instance.exitSoundClip);
    //     pannel.gameObject.SetActive(false);
    // }
    public void buttonChestClicked()
    {
        if (isButtonChestClickeds)
        {
            if (isAnimating) return;

            Debug.Log("Mở Chest Image!");
            int currentReward = GetCurrentReward();
            prizeChestText.text = "+ " + currentReward.ToString();
            MissionDaily.instance.dailyMission5Progress++;
            MissionDaily.instance.UpdateDailyMission();
            // Hiển thị phần thưởng tiếp theo
            if (chestTier < 4)
            {
                int nextRewardIndex = chestTier; 
                if (nextRewardIndex < chestRewards.Length)
                {
                    int nextReward = chestRewards[nextRewardIndex];
                    prizeNextChestText.text = "Next : " + nextReward.ToString();
                    Debug.Log($"ChestTier: {chestTier}, Next reward index: {nextRewardIndex}, Next reward: {nextReward}");
                }
                else
                {
                    Debug.LogError($"Next reward index {nextRewardIndex} out of bounds! Array length: {chestRewards.Length}");
                }
            }
            else
            {
                prizeNextChestText.text = "Max : " + chestRewards[3].ToString();
            }
            AudioManager.instance.PlaySound(AudioManager.instance.buttonSoundClip);

            // Dừng tất cả animation đang chạy
            DOTween.Kill(chestImage.transform);
            isAnimating = true;

            // Hiển thị prize chest image
            prizeChestImage.gameObject.SetActive(true);

            // Cập nhật UI trước khi hiển thị
            UpdateUI();

            // Animation mở prize chest image
            prizeChestImage.transform.localScale = Vector3.zero;
            prizeChestImage.transform.DOScale(Vector3.one, openDuration)
                .SetEase(openEase)
                .OnComplete(() =>
                {
                    isAnimating = false;
                    Debug.Log("Lucky Wheel Image animation mở hoàn thành!");
                    // prizeSlider.value = 0f;
                });
            isButtonChestClickeds = false;
            shakeTween?.Kill();
        }
    }
    
    public void buttonExitChestClicked()
    {
        if (isAnimating) return;
        if (Settings.instance == null)
        {
            Debug.LogError("Settings.instance is null!");
            return;
        }
        AudioManager.instance.PlaySound(AudioManager.instance.buttonSoundClip);
        
        int currentReward = GetCurrentReward();
        Debug.Log($"Chest Tier {chestTier} claimed! Reward: {currentReward} coins");
        
        // GManager.instance.totalMoney += currentReward;
        
        Sequence seq = DOTween.Sequence();
        seq.Append(DOVirtual.Float(totalValue, totalValue + currentReward, 2f, v =>
        {
            totalMoneyPlayText.text = ((int)v).ToString();
        }));
        totalValue += currentReward;
        
        if (chestTier < 4)
        {
            chestTier++;
            PlayerPrefs.SetInt("ChestTier", chestTier);
            Debug.Log($"Chest tier increased to {chestTier}! Next requirement: {GetCurrentRequirement()}coin -> Reward: {GetCurrentReward()}coin");
        }
        else
        {
            Debug.Log($"Max chest tier reached! Stays at tier 4: {GetCurrentRequirement()}coin -> Reward: {GetCurrentReward()}coin");
        }
        PlayerPrefs.SetFloat("TotalMoney", GManager.instance.totalMoney);
        PlayerPrefs.Save();
        
        targetValue = 0;
        prizeSlider.fillAmount = 0;
        PlayerPrefs.SetFloat("PrizeSliderValue", 0f);
        PlayerPrefs.Save();
        isCheckAddTargetValue = true;
        isPrizeCoin = false; 
        isShake = true; 
        isPrizeCoinDone = true; 
        
        Debug.Log("Đóng Prize Chest Image!");

        DOTween.Kill(prizeChestImage.transform);
        isAnimating = true;

        prizeChestImage.transform.DOScale(Vector3.zero, closeDuration)
            .SetEase(closeEase)
            .OnComplete(() =>
            {
                prizeChestImage.gameObject.SetActive(false);
                isAnimating = false;
                Debug.Log("Prize Chest Image animation đóng hoàn thành!");
            });
    }
    public string playerName = "";
    public void saveNameInputField()
    {
        playerName = inputField.text;
        //PlayerPrefs.SetString("player-name", playerName);
       // PlayerPrefs.Save();
    }

    public void NotificationNewMap()
    {
        if (NotificationNewMapImage == null) return;

        Debug.Log("NotificationNewMapImage is not null.");
        Vector2 originalPos = NotificationNewMapImage.anchoredPosition;

        Sequence seq = DOTween.Sequence();

        // 1. Di chuyển xuống y = -114f
        seq.Append(NotificationNewMapImage.DOAnchorPosY(-114f, 1f).SetEase(Ease.OutQuad));

        // 2. Reset slider ngay sau khi hạ xuống
        seq.AppendCallback(() =>
        {
            GManager.instance.ResetAchievementSlider();
            GManager.instance.tempDistanceTraveled -= 300f;
            GManager.instance.sliderAchievement.fillAmount = 0f;
            Debug.Log("Slider reset xong.");
            GManager.instance.milestoneDistance = GManager.instance.milestoneDistance2;
        });

        // 3. Giữ nguyên vị trí trong 1 giây
        seq.AppendInterval(1f);

        // 4. Trở lại vị trí ban đầu
        seq.Append(NotificationNewMapImage.DOAnchorPosY(originalPos.y, 1f).SetEase(Ease.OutQuad));

        // 5. Log khi hoàn tất
        seq.OnComplete(() =>
        {
            Debug.Log("Thông báo bản đồ mới đã ẩn.");
        });
    }


    public void Adsx2Coin()
    {
        x2CoinText.gameObject.SetActive(true);
        Sequence seq = DOTween.Sequence();
        int temp = (int)totalValue;
        totalValue = temp * 2;
        Debug.Log("Total value after doubling: " + totalValue);
        AdsButton.gameObject.SetActive(false);
        seq.Append(DOVirtual.Float(temp, totalValue, 2, value =>
            {
                if (totalMoneyPlayText != null) totalMoneyPlayText.text = Mathf.FloorToInt(value).ToString();
            }).SetEase(Ease.OutCubic)).OnComplete(() =>
            {
                
            });
    }

    public bool isColdDownTimeLuckyWheel = false;

    public void AdsLuckyWheelColdDownCoin()
    {
        currentTime -= 600;
        PlayerPrefs.SetInt("SaveTime", currentTime);
        PlayerPrefs.Save();
        if (!isCountingDown)
        {
            StartCoroutine(CountdownCoroutine());
        }
        if (currentTime < 0)
        {
            currentTime = 0;
        }
        AdsLuckyWheelButton.gameObject.SetActive(false);
        isColdDownTimeLuckyWheel = true;
        PlayerPrefs.SetInt("LuckyWheelColdDownAds", 1);
        PlayerPrefs.Save();
    }
     

    public void HineGuide()
    {
        GuideManager.instance.HideGuide();
    }
    
    // Helper methods cho Chest Tier System
    float GetCurrentRequirement()
    {
        int index = Mathf.Clamp(chestTier - 1, 0, chestRequirements.Length - 1);
        return chestRequirements[index];
    }
    
    int GetCurrentReward()
    {
        int index = Mathf.Clamp(chestTier - 1, 0, chestRewards.Length - 1);
        return chestRewards[index];
    }

    



    
}