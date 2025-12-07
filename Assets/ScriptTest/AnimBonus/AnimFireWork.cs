using UnityEngine;
using DG.Tweening; 

public class AnimFireWork : MonoBehaviour
{
    [SerializeField] private float moveDistance = 200f; // khoảng cách di chuyển (px)
    [SerializeField] private float moveDuration = 4f;   // thời gian di chuyển (giây)

    void Start()
    {
        // Lấy vị trí hiện tại
        Vector3 currentPos = transform.position;

        // Tạo vị trí mới dịch sang trái 200 px
        Vector3 targetPos = currentPos + new Vector3(-moveDistance, 0f, 0f);

        // Di chuyển bằng DOTween
        transform.DOMove(targetPos, moveDuration).SetEase(Ease.Linear);
    }
}