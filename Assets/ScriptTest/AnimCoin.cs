using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimCoin : MonoBehaviour
{
    public bool collected = false;
    private SpriteRenderer sr;
    private Vector3 initialScale;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale;
    }

    public void Collect()
    {
        if (collected == false) collected = true; // an toàn nếu gọi nhiều lần

        // ví dụ animation: bay chéo + fade rồi destroy ở OnComplete
        Vector3 targetPos = transform.position + new Vector3(-100f, 50f, 0f);
        Sequence seq = DOTween.Sequence();
        // seq.Append(transform.DOScale(initialScale * 1.2f, 1f).SetEase(Ease.OutBack));
        // seq.Append(transform.DOScale(initialScale, 1f).SetEase(Ease.InBack));
        seq.Join(transform.DOMove(targetPos, 4f).SetEase(Ease.OutExpo));
        if (sr != null) seq.Join(sr.DOFade(0f, 0.45f));
        seq.OnComplete(() => Destroy(gameObject));
    }
}