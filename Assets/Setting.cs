using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    // Start is called before the first frame update
    public Image imageCircle;
    public Image settingsPanel;
    public Image playPanel;
    public Vector2 saveVelocity;
    public float saveAngularVelocity;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AnimDoscaleCircleBig()
    {
        saveVelocity = GameManager.instance.airplaneRigidbody2D.velocity;
        saveAngularVelocity = GameManager.instance.airplaneRigidbody2D.angularVelocity;
        GameManager.instance.airplaneRigidbody2D.velocity = Vector2.zero;
        GameManager.instance.airplaneRigidbody2D.angularVelocity = 0f;
        GameManager.instance.airplaneRigidbody2D.isKinematic = true;
        imageCircle.gameObject.SetActive(true);
        imageCircle.rectTransform.DOScale(25f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            settingsPanel.gameObject.SetActive(true);
        });
    }

    public void AnimDoscaleCircleSmall()
    {
        
        settingsPanel.gameObject.SetActive(false);
        imageCircle.rectTransform.DOScale(1f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            imageCircle.gameObject.SetActive(false);
            GameManager.instance.airplaneRigidbody2D.isKinematic = false;
            GameManager.instance.airplaneRigidbody2D.velocity = saveVelocity;
            GameManager.instance.airplaneRigidbody2D.angularVelocity = saveAngularVelocity;
        });
    }


    public void EnterBtn(Image image)
    {
        image.color = Color.gray;
    }
    public void ExitBtn(Image image)
    {
        Color color = image.color;
        color.a = 0;
        image.color = color;
    }
    
    
}
