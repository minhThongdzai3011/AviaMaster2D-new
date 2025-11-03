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
    public TextMeshProUGUI totalMoneyPlayText;

    [Header("Image Settings")]
    public Image musicImageOn;
    public Image musicImageOff;
    public Image soundImageOn;
    public Image soundImageOff;
    public Image settingsImage;
    public Image winImage;

    [Header("Bool Settings")]
    public bool isMusicOn = true;
    public bool isSoundOn = true;
    
    [Header("DOTween Settings")]
    public float openDuration = 0.5f;
    public float closeDuration = 0.3f;
    public Ease openEase = Ease.OutBack;
    public Ease closeEase = Ease.InBack;
    
    private bool isAnimating = false;

    void Start()
    {
        instance = this;
        
        // Load settings từ PlayerPrefs
        LoadSettings();
        
        // Cập nhật UI ban đầu
        UpdateUI();
        
        Debug.Log("Settings initialized. Music: " + isMusicOn + ", Sound: " + isSoundOn);
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
        Debug.Log("Settings saved. Music: " + isMusicOn + ", Sound: " + isSoundOn);
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

        // Dừng tất cả animation đang chạy
        DOTween.Kill(winImage.transform);
        isAnimating = true;

        // Hiển thị win image
        winImage.gameObject.SetActive(true);

        // Cập nhật UI trước khi hiển thị
        UpdateUI();

        // Animation mở win image
        winImage.transform.localScale = Vector3.zero;
        winImage.transform.DOScale(Vector3.one, openDuration)
            .SetEase(openEase)
            .OnComplete(() => {
                isAnimating = false;
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
                winImage.gameObject.SetActive(false);
                GManager.instance.AgainGame();
                isAnimating = false;
                Debug.Log("Win Image animation đóng hoàn thành!");
            });
    }
        
}