using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class StartGame : MonoBehaviour
{
    public Image blackPanel; // Panel trong scene StartGame - KHÔNG CẦN NỮA
    private Canvas fadeCanvas;
    private Image fadePanel;

    void Start()
    {
        // TẠO blackPanel RIÊNG để DontDestroyOnLoad
        CreateFadePanel();
        
        // Đảm bảo panel đen hoàn toàn từ đầu
        fadePanel.color = new Color(0, 0, 0, 1f);
        
        // Giữ script này qua scene
        DontDestroyOnLoad(gameObject);
        
        // Subscribe vào event scene loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Sau 1.5s thì chuyển sang scene tesfly
        Invoke(nameof(LoadNextScene), 1.5f);
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "tesfly")
        {
            Debug.Log("Scene tesfly đã load xong!");
            // Bắt đầu fade sau khi scene load
            StartCoroutine(FadeInAfterSceneLoad());
            
            // Unsubscribe để tránh gọi nhiều lần
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void CreateFadePanel()
    {
        // Tạo Canvas mới
        GameObject canvasObj = new GameObject("FadeCanvas");
        fadeCanvas = canvasObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 9999; // Đảm bảo nằm trên cùng
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Tạo Image Panel đen
        GameObject panelObj = new GameObject("BlackPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        
        fadePanel = panelObj.AddComponent<Image>();
        fadePanel.color = new Color(0, 0, 0, 1f);
        
        // Phủ toàn màn hình
        RectTransform rectTransform = panelObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
        
        // Giữ Canvas qua scene
        DontDestroyOnLoad(canvasObj);
    }

    void LoadNextScene()
    {
        Debug.Log("Đang load scene tesfly...");
        SceneManager.LoadScene("tesfly");
    }

    IEnumerator FadeInAfterSceneLoad()
    {
        // Đợi scene load hoàn toàn
        yield return new WaitForSeconds(0.3f);
        
        Debug.Log("Bắt đầu fade in từ đen sang trong suốt...");
        
        // Fade alpha từ 1 -> 0 trong 2s (tăng thời gian để rõ hơn)
        fadePanel.DOFade(0f, 2f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            Debug.Log("✅ Fade in hoàn tất!");
            
            // Xóa canvas sau khi fade xong
            if (fadeCanvas != null)
            {
                Destroy(fadeCanvas.gameObject, 0.5f);
            }
            
            // Xóa script này
            Destroy(gameObject, 0.5f);
        });
    }
}