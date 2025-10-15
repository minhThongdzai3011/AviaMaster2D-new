using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Setting : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Setting Panel")]
    public Image imageCircle;
    public Image settingsPanel;
    public Image playPanel;
    public Image loopPanel;
    public Image imageHighQuality;
    public Image imageHoverHighQuality;

    [Header("Text UI")]
    public TextMeshProUGUI textLoop;

    [Header("Sprite UI")]
    public Sprite spriteNoClick;
    public Sprite spriteClick;
    public Sprite spriteHoverButton;

    [Header("Save Rigidbody2D")]
    public Vector2 saveVelocity;
    public float saveAngularVelocity;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AnimDoscaleCircleBigSetting()
    {
        saveVelocity = GameManager.instance.airplaneRigidbody2D.velocity;
        saveAngularVelocity = GameManager.instance.airplaneRigidbody2D.angularVelocity;
        GameManager.instance.airplaneRigidbody2D.velocity = Vector2.zero;
        GameManager.instance.airplaneRigidbody2D.angularVelocity = 0f;
        GameManager.instance.airplaneRigidbody2D.isKinematic = true;
        imageCircle.gameObject.SetActive(true);
        imageCircle.rectTransform.DOScale(25f, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            settingsPanel.gameObject.SetActive(true);
        });
    }

    public void AnimDoscaleCircleSmallSetting()
    {

        settingsPanel.gameObject.SetActive(false);
        imageCircle.rectTransform.DOScale(1f, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            imageCircle.gameObject.SetActive(false);
            GameManager.instance.airplaneRigidbody2D.isKinematic = false;
            GameManager.instance.airplaneRigidbody2D.velocity = saveVelocity;
            GameManager.instance.airplaneRigidbody2D.angularVelocity = saveAngularVelocity;
        });
    }

    public void AnimDoscaleCircleBigLoop()
    {
        saveVelocity = GameManager.instance.airplaneRigidbody2D.velocity;
        saveAngularVelocity = GameManager.instance.airplaneRigidbody2D.angularVelocity;
        GameManager.instance.airplaneRigidbody2D.velocity = Vector2.zero;
        GameManager.instance.airplaneRigidbody2D.angularVelocity = 0f;
        GameManager.instance.airplaneRigidbody2D.isKinematic = true;
        imageCircle.gameObject.SetActive(true);
        imageCircle.rectTransform.DOScale(25f, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            loopPanel.gameObject.SetActive(true);
        });
    }

    public void AnimDoscaleCircleSmallLoop()
    {

        loopPanel.gameObject.SetActive(false);
        imageCircle.rectTransform.DOScale(1f, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            imageCircle.gameObject.SetActive(false);
            GameManager.instance.airplaneRigidbody2D.isKinematic = false;
            GameManager.instance.airplaneRigidbody2D.velocity = saveVelocity;
            GameManager.instance.airplaneRigidbody2D.angularVelocity = saveAngularVelocity;
        });
    }


    public void EnterBtnMenuSetting(Image image)
    {
        image.color = Color.gray;
    }
    public void ExitBtnMenuSetting(Image image)
    {
        Color color = image.color;
        color.a = 0;
        image.color = color;
    }


    // High Quality
    public bool checkHighQuality = true;
    public void highQuality()
    {
        if (checkHighQuality)
        {
            imageHighQuality.sprite = spriteClick;
            checkHighQuality = false;
            QualitySettings.SetQualityLevel(5);
        }
        else
        {
            imageHighQuality.sprite = spriteNoClick;
            checkHighQuality = true;
            QualitySettings.SetQualityLevel(0);
        }
    }
    public void EnterBtnHighQuality()
    {
        imageHoverHighQuality.gameObject.SetActive(true);
    }
    public void ExitBtnHighQuality()
    {
        imageHoverHighQuality.gameObject.SetActive(false);
    }


    
}
