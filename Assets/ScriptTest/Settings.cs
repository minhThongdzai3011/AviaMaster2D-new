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
        playerName = PlayerPrefs.GetString("player-name", "");
        Debug.Log("Loaded HighScore: " + (int)PlayerPrefs.GetInt("HighScore", 0));
        highScoreText.text = " " + (int)PlayerPrefs.GetInt("HighScore", 0);
        
        inputField.text = playerName;
        isColdDownTimeLuckyWheel = PlayerPrefs.GetInt("LuckyWheelColdDownAds", 0) == 1;
        if (isColdDownTimeLuckyWheel)
        {
            AdsLuckyWheelButton.gameObject.SetActive(false);
        }
        
        currentTime = PlayerPrefs.GetInt("SaveTime", 900);
        if (currentTime < 900)
        {
            countdownText.text = currentTime.ToString();
            StartCoroutine(CountdownCoroutine());
            resultText.text = "Waiting...";
            isSpinning = false;

        }
        if (currentTime == 0)
        {
            AdsLuckyWheelButton.gameObject.SetActive(false);
        }

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

    public void Update()
    {
        if (isPrizeCoin)
        {
            UpdateSliderPrizeCoin();
        }
        altitudeText.text = "" + (int)GManager.instance.currentAltitude + " m";
        if (GManager.instance.currentAltitude >= 30)
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

                // reset
                targetValue = 0;
                Plane.instance.moneyCollect = 0;
                Plane.instance.moneyDistance = 0;
                Plane.instance.moneyTotal1 = 0;
                GManager.instance.AgainGame();

                Debug.Log("WinImage đã đóng!");
            });
    }

    
    public void openWheel()
    {
        if (isAnimating) return;
        pannelGray.gameObject.SetActive(true);
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
            });
    }

    public void leaderBoard()
    {
        
        if (isAnimating) return;
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
    void UpdateSliderPrizeCoin()
    {
        if (prizeSlider != null)
        {
            // Tính toán giá trị mục tiêu dựa trên khoảng cách bay được
            float maxDistanceCoin = 500f; // Khoảng cách tối đa để đạt 100%

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
            float smoothSpeed = 2f; // Tốc độ cập nhật (càng cao càng nhanh)
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

    public void ShakeZ()
    {
        if (isShakeZ && isShake)
        {
            RectTransform rect = chestImage.rectTransform;

            rect.DOShakeRotation(0.2f, new Vector3(0, 0, 20), 10, 90, false)
                .SetEase(Ease.OutQuad)
                .SetLoops(4, LoopType.Restart);

            isShakeZ = false;
            isShake = false;
            isButtonChestClickeds = true;
        }
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

        float bonusDuration = 1f;

        totalValue = distanceValue + collectValue;

        if (GManager.instance.isBonus)
        {
            totalValue *= 2;
            bonusText.text = "Bonus : x2";
            GManager.instance.isBonus = false;
        }
        else bonusText.text = "No Bonus";

        Sequence seq = DOTween.Sequence();
        AudioManager.instance.PlaySound(AudioManager.instance.countMoneySoundClip);
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
        Debug.Log("Total Value: " + totalValue);
        isPrizeCoin = true;

        // 4. finish → set allow close
        seq.OnComplete(() =>
        {
            isCloseWinImage = true;
            butttonExitImageWin.gameObject.SetActive(true);
            // AdsButton.gameObject.SetActive(true);
            

            Debug.Log("Cho phép đóng WinImage!");
        });
    }


    public bool isCountingDown = false;
    public void StartCountdown()
    {
        if (isCountingDown)
        {
            StartCoroutine(CountdownCoroutine());
        }
    }

    
    public IEnumerator CountdownCoroutine()
    {
        if (isSpinning)
        {

            while (currentTime > 0)
            {
                int minutes = currentTime / 60;
                int seconds = currentTime % 60;

                countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

                yield return new WaitForSeconds(1f);
                currentTime--;
            }

            countdownText.text = " ";
            Debug.Log("Countdown finished!");
            resultText.text = "Spin";
            isSpinning = true;
            isCountingDown = false;
        }
    }

    public void Exitpannel()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.exitSoundClip);
        pannel.gameObject.SetActive(false);
    }

    public void buttonChestClicked()
    {
        if (isButtonChestClickeds)
        {
            if (isAnimating) return;

            Debug.Log("Mở Win Image!");
            AudioManager.instance.PlaySound(AudioManager.instance.buttonSoundClip);

            // Dừng tất cả animation đang chạy
            DOTween.Kill(prizeChestImage.transform);
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
        
        Sequence seq = DOTween.Sequence();
        seq.Append(DOVirtual.Float(totalValue, totalValue+2000, 2f, v =>
        {
            totalMoneyPlayText.text = ((int)v).ToString();
        }));
        totalValue += 2000;
        PlayerPrefs.SetFloat("TotalMoney", GManager.instance.totalMoney);
        PlayerPrefs.Save();
        Debug.Log("Đóng Prize Chest Image!");

        // Dừng tất cả animation đang chạy
        DOTween.Kill(prizeChestImage.transform);
        isAnimating = true;

        // Animation đóng prize chest image
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
        PlayerPrefs.SetString("player-name", playerName);
        PlayerPrefs.Save();
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
            GManager.instance.sliderAchievement.value = 0f;
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
     



    



    
}