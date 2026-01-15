using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class AnimTextUI : MonoBehaviour
{
    public TextMeshProUGUI uiText;            
    public float moveDistance = 20f; 
    public float duration = 0.5f;   
    public Ease easeType = Ease.OutQuad;

    private Vector3 originalPos;

    void Start()
    {
        if (uiText == null) uiText = GetComponent<TextMeshProUGUI>();
        originalPos = uiText.rectTransform.anchoredPosition;

        MoveUpDownLoop();
    }

    public void MoveUpDownOnce()
    {
        uiText.rectTransform.DOAnchorPosY(originalPos.y + moveDistance, duration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                uiText.rectTransform.DOAnchorPosY(originalPos.y, duration).SetEase(easeType);
            });
    }

    public void MoveUpDownLoop()
    {
        uiText.rectTransform.DOAnchorPosY(originalPos.y + moveDistance, duration)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Yoyo);
    }
}