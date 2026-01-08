using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;

    [Header("Title")]
    public TextMeshProUGUI titleMissionText;

    [Header("Title Button")]
    public Button titleDailyButton; 
    public Button titlePlaneButton;
    public Button titleAchievementsButton;
    public Sprite spriteChoose;
    public Sprite spriteUnchoose;
    
    
    private int currentPage = 1;


    [Header("Content")]
    public GameObject gameObjectDailyText;
    public GameObject gameObjectPlaneText;
    public GameObject gameObjectAchievementsText;

    [Header("Page Text")]
    public TextMeshProUGUI textQuantityReward;

    [Header("Notification Reward Daily")]
    public Image notificationImageDaily;
    public TextMeshProUGUI textNotificationDaily;

    [Header("Notification Reward Plane")]
    public Image notificationImagePlane;
    public TextMeshProUGUI textNotificationPlane;

    [Header("Notification Reward Achievements")]
    public Image notificationImageAchievements;
    public TextMeshProUGUI textNotificationAchievements;
    
    [Header(" Imange Mission")]
    public Image misssionImage;
    public Image notificationImage;

    [Header("Block Button")]
    public Button settingButton;
    public Button shopButton;
    public EventTrigger luckyWheelButton;
    public EventTrigger leaderBoardButton;
    public EventTrigger moreGameButton;

    public int textQuantityRewardValue = 0;
    public int textNotificationDailyValue = 0;
    public int textNotificationPlaneValue = 0;
    public int textNotificationAchievementsValue = 0;

    private bool isTransitioning = false;

    private const float moveOffsetX = 70f;
    private const float duration = 0.25f;

    private bool isOpen = true;
    public Image isOpenImage;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.M) &&
            Input.GetKey(KeyCode.T) &&
            Input.GetKey(KeyCode.G) &&
            Input.GetKey(KeyCode.Alpha3) &&
            Input.GetKey(KeyCode.Alpha0) &&
            isOpen
            )
        {
            isOpen = false;
            PlayerPrefs.SetInt("isOpenMissionImage", 1);
            isOpenImage.gameObject.SetActive(true);
            StartCoroutine(WaitToCloseImage());
        }
    }

    private void Start()
    {
        ShowPage(1);
        UpdateButtonSprites();
        Debug.Log("Mission Manager Started : " + textQuantityRewardValue); 
        isOpen = PlayerPrefs.GetInt("isOpenMissionImage", 0) == 0;
    }
    
    void UpdateButtonSprites()
    {
        // Reset tất cả button về trạng thái không chọn
        titleDailyButton.GetComponent<Image>().sprite = spriteUnchoose;
        titlePlaneButton.GetComponent<Image>().sprite = spriteUnchoose;
        titleAchievementsButton.GetComponent<Image>().sprite = spriteUnchoose;
        
        // Set sprite cho button hiện tại
        switch (currentPage)
        {
            case 1:
                titleDailyButton.GetComponent<Image>().sprite = spriteChoose;
                break;
            case 2:
                titlePlaneButton.GetComponent<Image>().sprite = spriteChoose;
                break;
            case 3:
                titleAchievementsButton.GetComponent<Image>().sprite = spriteChoose;
                break;
        }
    }
    

    
    public void OnDailyButtonClick()
    {
        if (isTransitioning || currentPage == 1) return;
        
        SwitchToPage(1);
        AudioManager.instance.PlaySound(AudioManager.instance.leftRightShopSoundClip);
    }

    public void OnPlaneButtonClick()
    {
        if (isTransitioning || currentPage == 2) return;
        
        SwitchToPage(2);
        AudioManager.instance.PlaySound(AudioManager.instance.leftRightShopSoundClip);
    }
    
    public void OnAchievementButtonClick()
    {
        if (isTransitioning || currentPage == 3) return;
        
        SwitchToPage(3);
        AudioManager.instance.PlaySound(AudioManager.instance.leftRightShopSoundClip);
    }
    
    void SwitchToPage(int newPage)
    {
        if (newPage == currentPage) return;
        
        // Xác định hướng chuyển trang
        int direction = newPage > currentPage ? 1 : -1;
        
        // Chuyển trang với hiệu ứng
        StartCoroutine(SimplePageTransition(currentPage, newPage, direction));
    }
    

    
    IEnumerator SimplePageTransition(int oldPage, int newPage, int direction)
    {
        isTransitioning = true;
        
        GameObject oldContent = GetContentByPage(oldPage);
        GameObject newContent = GetContentByPage(newPage);
        
        CanvasGroup oldContentCg = GetCanvasGroup(oldContent);
        CanvasGroup newContentCg = GetCanvasGroup(newContent);
        
        RectTransform oldContentRect = oldContent.GetComponent<RectTransform>();
        RectTransform newContentRect = newContent.GetComponent<RectTransform>();
        
        // Lưu vị trí gốc thực tế (y = -67)
        Vector3 originalPosition = oldContentRect.localPosition;
        float originalY = originalPosition.y;
        
        Vector3 oldContentEnd = new Vector3(-moveOffsetX * direction, originalY, 0);
        Vector3 newContentStart = new Vector3(moveOffsetX * direction, originalY, 0);
        Vector3 targetPosition = new Vector3(0, originalY, 0);
        
        // Kích hoạt content mới
        newContent.SetActive(true);
        newContentCg.alpha = 0;
        newContentRect.localPosition = newContentStart;
        
        float time = 0f;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            
            // Fade out old content
            oldContentCg.alpha = Mathf.Lerp(1, 0, t);
            oldContentRect.localPosition = Vector3.Lerp(originalPosition, oldContentEnd, t);
            
            // Fade in new content
            newContentCg.alpha = Mathf.Lerp(0, 1, t);
            newContentRect.localPosition = Vector3.Lerp(newContentStart, targetPosition, t);
            
            yield return null;
        }
        
        // Tắt content cũ
        oldContent.SetActive(false);
        
        // Reset vị trí content mới về vị trí gốc
        newContentRect.localPosition = targetPosition;
        newContentCg.alpha = 1;
        
        // Cập nhật trang hiện tại và sprite
        currentPage = newPage;
        UpdateButtonSprites();
        
        isTransitioning = false;
    }

    void ShowPage(int page)
    {
        gameObjectDailyText.SetActive(page == 1);
        gameObjectPlaneText.SetActive(page == 2);
        gameObjectAchievementsText.SetActive(page == 3);

        currentPage = page;
        UpdatePageText();
    }

    CanvasGroup GetCanvasGroup(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        return cg;
    }
    
    
    void UpdatePageText()
    {
        switch (currentPage)
        {
            case 1:
                titleMissionText.text = "Mission";
                break;
            case 2:
                titleMissionText.text = "Mission";
                break;
            case 3:
                titleMissionText.text = "Mission";
                break;
        }
    }

    GameObject GetContentByPage(int page)
    {
        switch (page)
        {
            case 1: return gameObjectDailyText;
            case 2: return gameObjectPlaneText;
            case 3: return gameObjectAchievementsText;
        }
        return null;
    }
    
    public void OpenMission()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.buttonSoundClip);
        Settings.instance.pannelGray.gameObject.SetActive(true);
        
        // Button block khi mở mission
        settingButton.interactable = false;
        shopButton.interactable = false;
        luckyWheelButton.GetComponent<EventTrigger>().enabled = false;
        leaderBoardButton.GetComponent<EventTrigger>().enabled = false;
        moreGameButton.GetComponent<EventTrigger>().enabled = false;

        CheckPlane.instance.SetActiveLeaderBoard();
        CheckPlane.instance.SetActiveMoreGame();
        CheckPlane.instance.SetActiveLuckyWheel();

        // Dừng tất cả animation đang chạy
        DOTween.Kill(misssionImage.transform);
        
        // Hiển thị settings panel
        Settings.instance.lastDistanceText.gameObject.SetActive(false);
        misssionImage.gameObject.SetActive(true);
        
        
        // Animation mở settings (giống ShopImage)
        misssionImage.transform.localScale = Vector3.zero;
        misssionImage.transform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                
            });
    }

    public void CloseMission()
    {

        AudioManager.instance.PlaySound(AudioManager.instance.exitSoundClip);

        // Dừng tất cả animation đang chạy
        DOTween.Kill(misssionImage.transform);

        misssionImage.transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                
                Settings.instance.lastDistanceText.gameObject.SetActive(true);
                misssionImage.gameObject.SetActive(false);
                Settings.instance.pannelGray.gameObject.SetActive(false);
                // Bỏ block button khi đóng mission
                settingButton.interactable = true;
                shopButton.interactable = true; 
                luckyWheelButton.GetComponent<EventTrigger>().enabled = true;
                leaderBoardButton.GetComponent<EventTrigger>().enabled = true;
                moreGameButton.GetComponent<EventTrigger>().enabled = true;
                CheckPlane.instance.ResetActiveLeaderBoard();
                CheckPlane.instance.ResetActiveMoreGame();
                CheckPlane.instance.ResetActiveLuckyWheel();
            });
    }

    public void closeImage()
    {
        isOpenImage.gameObject.SetActive(false);
    }

    IEnumerator WaitToCloseImage()
    {
        yield return new WaitForSeconds(3f);
        isOpenImage.gameObject.SetActive(false);
    }
    
}