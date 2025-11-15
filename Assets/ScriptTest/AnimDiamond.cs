using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimDiamond : MonoBehaviour
{
    public static AnimDiamond instance;
    [Header("Move settings")]
    public float moveAmount = 20f;
    public float moveDuration = 0.5f;
    public bool useLocalPosition = false; 
    public Ease ease = Ease.OutQuad;

    private Transform cachedTransform;

    void Awake()
    {
        instance = this;
        cachedTransform = transform;
    }

    public void MoveUp()
    {
        if (cachedTransform == null) return;

        if (useLocalPosition)
        {
            Vector3 target = cachedTransform.localPosition + new Vector3(0f, moveAmount, 0f);
            cachedTransform.DOLocalMoveY(target.y, moveDuration).SetEase(ease);
        }
        else
        {
            Vector3 target = cachedTransform.position + new Vector3(0f, moveAmount, 0f);
            cachedTransform.DOMoveY(target.y, moveDuration).SetEase(ease);
        }
    }
}