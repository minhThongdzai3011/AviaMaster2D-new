using UnityEngine;
using DG.Tweening;

public class ScaleMarkError : MonoBehaviour
{
    [Header("Scale Settings")]
    public float minScale = 0.5f;   // scale nhỏ nhất
    public float maxScale = 0.7f;   // scale lớn nhất
    public float duration = 0.5f;   // thời gian cho mỗi nửa chu kỳ (0.5s lên, 0.5s xuống)

    void Start()
    {
        // Đặt scale ban đầu
        transform.localScale = Vector3.one * minScale;

        // Tween scale lên maxScale rồi quay lại, lặp vô hạn
        transform.DOScale(maxScale, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }
}