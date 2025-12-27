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
    public TextMeshProUGUI titleDailyText;
    public TextMeshProUGUI titlePlaneText;
    public TextMeshProUGUI titleAchievementsText;

    [Header("Content")]
    public GameObject gameObjectDailyText;
    public GameObject gameObjectPlaneText;
    public GameObject gameObjectAchievementsText;

    [Header("Page Text")]
    public TextMeshProUGUI textPage;
    public TextMeshProUGUI textQuantityReward;

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

    private int currentPage = 1;
    private const int maxPage = 3;
    private bool isTransitioning = false;

    private const float moveOffsetX = 70f;
    private const float duration = 0.25f;

    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ShowPage(1);
    }
    

    
    public void OnLeftButtonClick()
    {
        if (isTransitioning) return;

        int nextPage = currentPage - 1;
        if (nextPage < 1) nextPage = maxPage;

        ChangePage(nextPage, -1);
        AudioManager.instance.PlaySound(AudioManager.instance.leftRightShopSoundClip);
    }

    public void OnRightButtonClick()
    {
        if (isTransitioning) return;

        int nextPage = currentPage + 1;
        if (nextPage > maxPage) nextPage = 1;

        ChangePage(nextPage, 1);
        AudioManager.instance.PlaySound(AudioManager.instance.leftRightShopSoundClip);
    }
    

    
    void ChangePage(int newPage, int direction)
    {
        StartCoroutine(PageTransition(
            GetTitleByPage(currentPage),
            GetTitleByPage(newPage),
            GetContentByPage(currentPage),
            GetContentByPage(newPage),
            direction
        ));

        currentPage = newPage;
        UpdatePageText();
    }

    void ShowPage(int page)
    {
        titleDailyText.gameObject.SetActive(page == 1);
        titlePlaneText.gameObject.SetActive(page == 2);
        titleAchievementsText.gameObject.SetActive(page == 3);

        gameObjectDailyText.SetActive(page == 1);
        gameObjectPlaneText.SetActive(page == 2);
        gameObjectAchievementsText.SetActive(page == 3);

        currentPage = page;
        UpdatePageText();
    }
    

    
    IEnumerator PageTransition(
        TextMeshProUGUI oldTitle,
        TextMeshProUGUI newTitle,
        GameObject oldContent,
        GameObject newContent,
        int direction)
    {
        isTransitioning = true;

        CanvasGroup oldTitleCg = GetCanvasGroup(oldTitle.gameObject);
        CanvasGroup newTitleCg = GetCanvasGroup(newTitle.gameObject);
        CanvasGroup oldContentCg = GetCanvasGroup(oldContent);
        CanvasGroup newContentCg = GetCanvasGroup(newContent);

        RectTransform oldTitleRect = oldTitle.GetComponent<RectTransform>();
        RectTransform newTitleRect = newTitle.GetComponent<RectTransform>();
        RectTransform oldContentRect = oldContent.GetComponent<RectTransform>();
        RectTransform newContentRect = newContent.GetComponent<RectTransform>();

        float titleY = oldTitleRect.localPosition.y;

        Vector3 oldTitleStart = new Vector3(0, titleY, 0);
        Vector3 oldTitleEnd   = new Vector3(-moveOffsetX * direction, titleY, 0);
        Vector3 newTitleStart = new Vector3(moveOffsetX * direction, titleY, 0);
        Vector3 newTitleEnd   = new Vector3(0, titleY, 0);

        Vector3 oldContentEnd = new Vector3(-moveOffsetX * direction, 0, 0);
        Vector3 newContentStart = new Vector3(moveOffsetX * direction, 0, 0);

        newTitle.gameObject.SetActive(true);
        newContent.gameObject.SetActive(true);

        newTitleCg.alpha = 0;
        newContentCg.alpha = 0;

        newTitleRect.localPosition = newTitleStart;
        newContentRect.localPosition = newContentStart;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // OLD
            oldTitleCg.alpha = Mathf.Lerp(1, 0, t);
            oldContentCg.alpha = Mathf.Lerp(1, 0, t);

            oldTitleRect.localPosition = Vector3.Lerp(oldTitleStart, oldTitleEnd, t);
            oldContentRect.localPosition = Vector3.Lerp(Vector3.zero, oldContentEnd, t);

            // NEW
            newTitleCg.alpha = Mathf.Lerp(0, 1, t);
            newContentCg.alpha = Mathf.Lerp(0, 1, t);

            newTitleRect.localPosition = Vector3.Lerp(newTitleStart, newTitleEnd, t);
            newContentRect.localPosition = Vector3.Lerp(newContentStart, Vector3.zero, t);

            yield return null;
        }

        oldTitle.gameObject.SetActive(false);
        oldContent.gameObject.SetActive(false);

        // Reset trạng thái an toàn
        newTitleRect.localPosition = newTitleEnd;
        newContentRect.localPosition = Vector3.zero;
        newTitleCg.alpha = 1;
        newContentCg.alpha = 1;

        isTransitioning = false;
    }

    CanvasGroup GetCanvasGroup(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        return cg;
    }
    
    
    void UpdatePageText()
    {
        textPage.text = currentPage.ToString();
    }

    TextMeshProUGUI GetTitleByPage(int page)
    {
        switch (page)
        {
            case 1: return titleDailyText;
            case 2: return titlePlaneText;
            case 3: return titleAchievementsText;
        }
        return null;
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
}
