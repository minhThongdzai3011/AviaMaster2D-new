using UnityEngine;
using DG.Tweening;

public class EffectCloud : MonoBehaviour
{
    private Tween moveTween;

    void Start()
    {
        // Random chọn hướng: 0 = trái, 1 = phải
        int dir = Random.Range(0, 2);

        float targetX = (dir == 0) 
            ? transform.position.x - 5f   // sang trái 5px
            : transform.position.x + 5f;  // sang phải 5px

        moveTween = transform.DOMoveX(targetX, 10f)
            .SetLoops(-1, LoopType.Yoyo) // lặp vô hạn, đi rồi quay lại
            .SetEase(Ease.Linear);
    }


}