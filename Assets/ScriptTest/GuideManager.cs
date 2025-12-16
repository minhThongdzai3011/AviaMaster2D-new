using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GuideManager : MonoBehaviour
{
    public static GuideManager instance;
    public bool isShowGuide = false;
    public TextMeshProUGUI guideText;
    public CanvasGroup canvasGroup;
    public CanvasGroup canvasGroupButton;
    // Start is called before the first frame update
    public void Awake()
    {

        instance = this;
        int showGuidePref = PlayerPrefs.GetInt("IsShowGuide", 0);
        if (showGuidePref == 1)
        {
            isShowGuide = true;
        }
        else
        {
            isShowGuide = false;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowGuide()
    {
        isShowGuide = true;
        
        PlayerPrefs.SetInt("IsShowGuide", 1);
        PlayerPrefs.Save();
        // AnimButton.instance.PlayScaleAnim();
        Time.timeScale = 0f;
        Settings.instance.lastDistanceText.gameObject.SetActive(false);
        OpenGuide();
        
    }
    public void HideGuide()
    {
        Time.timeScale = 1f;
        Settings.instance.lastDistanceText.gameObject.SetActive(true);
        Settings.instance.imageGuide.gameObject.SetActive(false);
        // AnimButton.instance.StopAnim();
    }
    public void OpenGuide()
    {
        Settings.instance.imageGuide.gameObject.SetActive(true);
        canvasGroup.DOFade(1f, 1f).SetUpdate(true);
        GManager.instance.upAircraftImage.gameObject.SetActive(true);
        canvasGroupButton.DOFade(1f, 1f).SetUpdate(true);
    }
    
}
