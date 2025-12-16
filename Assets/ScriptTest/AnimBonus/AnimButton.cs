using UnityEngine;
using DG.Tweening; 

public class AnimButton : MonoBehaviour
{
    public static AnimButton instance;
    public float duration = 0.5f;

    public Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f);

    public void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        PlayScaleAnim();
    }

    public void PlayScaleAnim()
    {
        transform.localScale = Vector3.one;

        transform.DOScale(targetScale, duration)
                 .SetEase(Ease.OutBack)
                 .OnComplete(() =>
                 {
                     transform.DOScale(Vector3.one, duration)
                              .SetEase(Ease.InBack)
                              .SetLoops(-1, LoopType.Yoyo);
                 });
    }
    public void StopAnim()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
    }

}