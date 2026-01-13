using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ScaleChestLuckyWheel : MonoBehaviour
{
    public static ScaleChestLuckyWheel instance;
    [Header("Scale Settings")]
    public float minScale = 1f;   
    public float maxScale = 1.06f;    
    public float duration = 0.5f;   
    public bool isScaling = false;
    public Image imageNotify;

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        transform.localScale = Vector3.one * minScale;
    }

    public void StartScaling()
    {
        imageNotify.gameObject.SetActive(true);
        Debug.Log("Attempting to start scaling...");
        if (!isScaling) return;
        
        isScaling = true;
        transform.DOKill();
        transform.localScale = Vector3.one * minScale;

        transform.DOScale(maxScale, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        
        Debug.Log("Chest Lucky Wheel started scaling!");
    }

    public void StopScaling()
    {
        // if (isScaling) return;
        
        // isScaling = false;
        imageNotify.gameObject.SetActive(false);
        transform.DOKill();
        transform.DOScale(minScale, 0.2f).SetEase(Ease.OutQuad);
        
        Debug.Log("Chest Lucky Wheel stopped scaling!");
    }
}