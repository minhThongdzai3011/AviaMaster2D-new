using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
using TMPro;

public class StartGame : MonoBehaviour
{
    public Image blackPanel; 
    // private Canvas fadeCanvas;
    // private Image fadePanel;

    [Header("Text Settings")]
    public TextMeshProUGUI loadingText1;
    public TextMeshProUGUI loadingText2;
    public TextMeshProUGUI loadingText3;
    public TextMeshProUGUI nameGameText;

    [Header("Image Settings")]
    public Image imageLeft;
    public Image imageLeft1;
    public Image imageLeft2;
    public Image imageRight;
    public Image imageRight1;
    public Image imageRight2;
    public Image imageUp;
    public Image imageUp1;
    public Image imageUp2;
    public Image imageRotationZ;
    public float distance = 100f;
    public float duration = 1f;

    void Start()
    {
        // TẠO blackPanel RIÊNG để DontDestroyOnLoad
        // CreateFadePanel();
        
        // Đảm bảo panel đen hoàn toàn từ đầu
        // fadePanel.color = new Color(0, 0, 0, 1f);
        
        // Giữ script này qua scene
        DontDestroyOnLoad(gameObject);
        
        // Subscribe vào event scene loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Sau 1.5s thì chuyển sang scene tesfly
        Invoke(nameof(LoadNextScene), 2.5f);
        // Bắt đầu hiệu ứng chấm loading
        StartCoroutine(PlayLoadingDots(0.3f));
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "tesfly")
        {
            Debug.Log("Scene tesfly đã load xong!");
            // Bắt đầu fade sau khi scene load
            // StartCoroutine(FadeInAfterSceneLoad());
            isLoading = false;
            
            
            // Unsubscribe để tránh gọi nhiều lần
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    // void CreateFadePanel()
    // {
    //     // Tạo Canvas mới
    //     GameObject canvasObj = new GameObject("FadeCanvas");
    //     fadeCanvas = canvasObj.AddComponent<Canvas>();
    //     fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
    //     CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
    //     scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    //     scaler.referenceResolution = new Vector2(1920, 1080);
        
    //     canvasObj.AddComponent<GraphicRaycaster>();
        
    //     // Tạo Image Panel đen
    //     GameObject panelObj = new GameObject("BlackPanel");
    //     panelObj.transform.SetParent(canvasObj.transform, false);
        
    //     fadePanel = panelObj.AddComponent<Image>();
    //     fadePanel.color = new Color(0, 0, 0, 1f);
        
    //     // Phủ toàn màn hình
    //     RectTransform rectTransform = panelObj.GetComponent<RectTransform>();
    //     rectTransform.anchorMin = Vector2.zero;
    //     rectTransform.anchorMax = Vector2.one;
    //     rectTransform.sizeDelta = Vector2.zero;
    //     rectTransform.anchoredPosition = Vector2.zero;
        
    //     // Giữ Canvas qua scene
    //     DontDestroyOnLoad(canvasObj);
    // }

    void LoadNextScene()
    {
        Debug.Log("Đang load scene tesfly...");
        SceneManager.LoadScene("tesfly");
    }

    // IEnumerator FadeInAfterSceneLoad()
    // {
    //     // Đợi scene load hoàn toàn
    //     yield return new WaitForSeconds(0.3f);
        
    //     Debug.Log("Bắt đầu fade in từ đen sang trong suốt...");
        
    //     // Fade alpha từ 1 -> 0 trong 2s (tăng thời gian để rõ hơn)
    //     fadePanel.DOFade(0f, 2f).SetEase(Ease.InOutQuad).OnComplete(() =>
    //     {
    //         Debug.Log("Fade in hoàn tất!");
            
    //         // Xóa canvas sau khi fade xong
    //         if (fadeCanvas != null)
    //         {
    //             Destroy(fadeCanvas.gameObject, 0.5f);
    //         }
            
    //         // Xóa script này
    //         Destroy(gameObject, 0.5f);
    //     });
    // }

    private int state = 0;
    public bool isLoading = true;

    public void UpdateDots()
    {
        loadingText1.gameObject.SetActive(false);
        loadingText2.gameObject.SetActive(false);
        loadingText3.gameObject.SetActive(false);

        // Bật theo state
        if (state >= 1) loadingText1.gameObject.SetActive(true);
        if (state >= 2) loadingText2.gameObject.SetActive(true);
        if (state >= 3) loadingText3.gameObject.SetActive(true);

        state = (state + 1) % 4; 
    }

    IEnumerator PlayLoadingDots(float interval)
    {
        while (isLoading)
        {
            UpdateDots();
            yield return new WaitForSeconds(interval);
        }
    }


    void Update()
    {
        if (isLoading)
        {
            MoveLeftToRight();
            MoveRightToLeft();
            MoveUptoDown();
            RotateZ();
        }
    }
    public void MoveLeftToRight()
    {
        RectTransform rt = imageLeft.GetComponent<RectTransform>();
        Vector3 targetPos = rt.anchoredPosition + new Vector2(90, 0);
        rt.DOAnchorPos(targetPos, duration).SetEase(Ease.OutQuad);
        RectTransform rt1 = imageLeft1.GetComponent<RectTransform>();
        Vector3 targetPos1 = rt1.anchoredPosition + new Vector2(distance, 0);
        rt1.DOAnchorPos(targetPos1, duration).SetEase(Ease.OutQuad);
        RectTransform rt2 = imageLeft2.GetComponent<RectTransform>();
        Vector3 targetPos2 = rt2.anchoredPosition + new Vector2(120, 0);
        rt2.DOAnchorPos(targetPos2, duration).SetEase(Ease.OutQuad);
    }

    public void MoveRightToLeft()
    {
        RectTransform rt = imageRight.GetComponent<RectTransform>();
        Vector3 targetPos = rt.anchoredPosition - new Vector2(80, 0);
        rt.DOAnchorPos(targetPos, duration).SetEase(Ease.OutQuad);
        RectTransform rt1 = imageRight1.GetComponent<RectTransform>();
        Vector3 targetPos1 = rt1.anchoredPosition - new Vector2(70, 0);
        rt1.DOAnchorPos(targetPos1, duration).SetEase(Ease.OutQuad);
        RectTransform rt2 = imageRight2.GetComponent<RectTransform>();
        Vector3 targetPos2 = rt2.anchoredPosition - new Vector2(150, 0);
        rt2.DOAnchorPos(targetPos2, duration).SetEase(Ease.OutQuad);
    }

    public void MoveUptoDown()
    {
        RectTransform rt = imageUp.GetComponent<RectTransform>();
        Vector2 targetPos = rt.anchoredPosition + new Vector2(0, 10);
        rt.DOAnchorPos(targetPos, duration).SetEase(Ease.OutQuad);
        RectTransform rt1 = imageUp1.GetComponent<RectTransform>();
        Vector2 targetPos1 = rt1.anchoredPosition - new Vector2(0, 10);
        rt1.DOAnchorPos(targetPos1, duration).SetEase(Ease.OutQuad);
        RectTransform rt2 = imageUp2.GetComponent<RectTransform>();
        Vector2 targetPos2 = rt2.anchoredPosition + new Vector2(0, 10);
        rt2.DOAnchorPos(targetPos2, duration).SetEase(Ease.OutQuad);
    }

    public void RotateZ()
    {
        RectTransform rt = imageRotationZ.GetComponent<RectTransform>();
        rt.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Restart);
    }




}