using UnityEngine;
using DG.Tweening;

public class ScaleChestLuckyWheel : MonoBehaviour
{
    public static ScaleChestLuckyWheel instance;
    [Header("Scale Settings")]
    public float minScale = 0.1f;   
    public float maxScale = 1.06f;    
    public float duration = 0.5f;   
    public bool isScaling = false;

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
       
    }

    public void ScaleLuckyWheel()
    {
        if (Settings.instance.isSpinning) 
        {
            isScaling = true;
            transform.localScale = Vector3.one * minScale;

            transform.DOScale(maxScale, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Linear);
        }
        else  
        {
            isScaling = false;
            transform.DOKill();
            transform.localScale = Vector3.one * 1;
        }
    }
}