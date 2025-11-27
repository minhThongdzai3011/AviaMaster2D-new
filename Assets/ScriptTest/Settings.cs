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
    public Slider prizeSlider;

    [Header("Input Settings")]
    public TMP_InputField inputField;

    [Header ("Countdown Settings")]
    public int currentTime = 0;

    void Start()
    {
        instance = this;
        currentTime = PlayerPrefs.GetInt("SaveTime", 900);
        if (currentTime < 900)
        {
            countdownText.text = currentTime.ToString();
            StartCoroutine(CountdownCoroutine());
            resultText.text = "Waiting...";
            isSpinning = false;

        }

        // Load settings từ PlayerPrefs
        LoadSettings();

        // Cập nhật UI ban đầu
        UpdateUI();

        Debug.Log("Settings initialized. Music: " + isMusicOn + ", Sound: " + isSoundOn);
        targetValue = PlayerPrefs.GetFloat("PrizeSliderValue", 0f);
        inputField.text = PlayerPrefs.GetString("PlayerName", "");
        prizeSlider.value = targetValue;
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
        
        // TODO: Bật/tắt nhạc nền thực tế ở đây
        // AudioManager.instance.ToggleMusic(isMusicOn);
    }

    public void ToggleSound()
    {
        if (isAnimating) return;
        
        Debug.Log("Toggling Sound. Current state: " + isSoundOn);
        
        // Đổi trạng thái
        isSoundOn = !isSoundOn;
        
        // Animation cho button sound
        StartCoroutine(AnimateToggle(isSoundOn, soundImageOn, soundImageOff, soundText, "Sound"));
        
        // TODO: Bật/tắt âm thanh thực tế ở đây
        // AudioManager.instance.ToggleSound(isSoundOn);
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

    public void CloseWinImage()
    {
        if (isAnimating) return;
        if (Settings.instance == null)
        {
            Debug.LogError("Settings.instance is null!");
            return;
        }

        Debug.Log("Đóng Win Image!");

        // Dừng tất cả animation đang chạy
        DOTween.Kill(winImage.transform);
        isAnimating = true;

        // Animation đóng win image
        winImage.transform.DOScale(Vector3.zero, closeDuration)
            .SetEase(closeEase)
            .OnComplete(() =>
            {
                lastDistanceText.gameObject.SetActive(true);
                winImage.gameObject.SetActive(false);
                GManager.instance.AgainGame();
                isAnimating = false;
                Debug.Log("Win Image animation đóng hoàn thành!");
                targetValue = 0f;
                Plane.instance.moneyCollect = 0;
                Plane.instance.moneyDistance = 0;
                Plane.instance.moneyTotal = 0;
            });
    }


    
    public void openWheel()
    {
        if (isAnimating) return;

        Debug.Log("Mở Win Image!");

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
                Debug.Log("Lucky Wheel Image animation đóng hoàn thành!");
            });
    }

    public void leaderBoard()
    {
        
        if (isAnimating) return;

        Debug.Log("Mở Win Image!");

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
            float maxDistanceCoin = 1000f; // Khoảng cách tối đa để đạt 100%

            if (Plane.instance == null)
            {
                Debug.LogError("Plane.instance is null!");
                return;
            }
            float temp = targetValue;

            if (isCheckAddTargetValue)
            {
                targetValue = temp + Mathf.Clamp01(Plane.instance.moneyTotal / maxDistanceCoin);
                isCheckAddTargetValue = false;
            }

            PlayerPrefs.SetFloat("PrizeSliderValue", targetValue);
            PlayerPrefs.Save();
            // Cập nhật slider với hiệu ứng mượt mà
            float smoothSpeed = 2f; // Tốc độ cập nhật (càng cao càng nhanh)
            prizeSlider.value = Mathf.Lerp(prizeSlider.value, targetValue, smoothSpeed * Time.deltaTime);
            if (prizeSlider. value >= 1f)
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


    public void StartCountingCoin()
    {
        if (Plane.instance == null)
        {
            Debug.LogWarning("Plane.instance is null");
            return;
        }

        float distanceValue = Plane.instance.moneyDistance;
        float collectValue  = Plane.instance.moneyCollect;
        float totalValue    = Plane.instance.moneyTotal;

        Debug.Log("Tiền khoảng cách: " + distanceValue + " | Tiền thu thập: " + collectValue + " | Tổng tiền: " + totalValue + "isBonus: " + GManager.instance.isBonus);

        // Sequence để chạy lần lượt: distance -> collect -> (x2 bonus) -> total
        Sequence seq = DOTween.Sequence();

        // 1) Đếm distance
        seq.Append(DOVirtual.Float(0f, distanceValue, 2f, value =>
        {
            if (distanceMoneyText != null) distanceMoneyText.text = Mathf.FloorToInt(value).ToString();
        }).SetEase(Ease.OutCubic));

        // 2) Đếm collect
        seq.Append(DOVirtual.Float(0f, collectValue, 2f, value =>
        {
            if (collectMoneyText != null) collectMoneyText.text = Mathf.FloorToInt(value).ToString();
        }).SetEase(Ease.OutCubic));
        float duration = 2f;
        // 3) Xử lý bonus trước khi đếm total, sau đó đếm total
        seq.AppendCallback(() =>
        {
            if (GManager.instance != null && GManager.instance.isBonus)
            {
                Debug.Log("Áp dụng bonus x2!");
                if (bonusText != null) bonusText.text = "Bonus : x2";
                Debug.Log("Tổng tiền trước bonus: " + totalValue);
                totalValue *= 2f;
                duration = 4f;
                Debug.Log("Tổng tiền sau bonus: " + totalValue);
                GManager.instance.isBonus = false;
            }
            else
            {
                Debug.Log("Không áp dụng bonus");
                if (bonusText != null) bonusText.text = "No Bonus";
                duration = 2f;
            }

            // Cập nhật Plane.instance.moneyTotal nếu bạn muốn ghi lại giá trị mới
            if (Plane.instance != null) Plane.instance.moneyTotal = (int)totalValue;
            Debug.Log("Final Total Value to count: " + totalValue);

            // 4) Đếm total
            Debug.Log("Bắt đầu đếm tổng tiền: " + totalValue);
            seq.Append(DOVirtual.Float(0f, totalValue, duration, value =>
            {
                if (totalMoneyPlayText != null) totalMoneyPlayText.text = Mathf.FloorToInt(value).ToString();
            }).SetEase(Ease.OutCubic)).OnComplete(() =>
            {
                GManager.instance.coinEffect.Play();
                
                isPrizeCoin = true;
            });
        });

        

        seq.Play();
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
        pannel.gameObject.SetActive(false);
    }

    public void buttonChestClicked()
    {
        if (isButtonChestClickeds)
        {
            if (isAnimating) return;

            Debug.Log("Mở Win Image!");

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
                    prizeSlider.value = 0f;
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
        GManager.instance.totalMoney += 1000;
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

    public void saveNameInputField()
    {
        string playerName = inputField.text;
        PlayerPrefs.SetString("PlayerName", playerName);
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


    
}