using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Setting : MonoBehaviour
{
    public Image[] planeImages; 
    private Vector2[] positions; 
    private int[] currentIndexOfImage;    
    public float moveDuration = 0.5f;

    void Start()
    {
        int len = planeImages.Length;
        positions = new Vector2[len];
        currentIndexOfImage = new int[len];

        for (int i = 0; i < len; i++)
        {
            positions[i] = planeImages[i].rectTransform.anchoredPosition;
            currentIndexOfImage[i] = i; // ban đầu image i nằm ở vị trí i
        }
    }

    public void OnClickRightArrow()
    {
        Shift(-1); // shift right nghĩa là mỗi ảnh giảm index 1 (mod len)
    }

    public void OnClickLeftArrow()
    {
        Shift(+1); // shift left nghĩa là mỗi ảnh tăng index 1 (mod len)
    }

    // dir = +1 (left), dir = -1 (right)
    private void Shift(int dir)
    {
        int len = planeImages.Length;

        for (int i = 0; i < len; i++)
        {
            int cur = currentIndexOfImage[i];
            int target = (cur + dir + len) % len;

            // detect wrap: nếu di chuyển từ đầu sang cuối hoặc ngược lại
            bool wrapToEnd = (cur == 0 && target == len - 1);
            bool wrapToStart = (cur == len - 1 && target == 0);
            Image img = planeImages[i];

            if (wrapToEnd || wrapToStart)
            {
                // ẩn hình trước khi tween (không dùng SetActive)
                img.enabled = false;

                // tween vị trí vẫn chạy, khi hoàn tất bật hiển thị
                img.rectTransform.DOAnchorPos(positions[target], moveDuration).OnComplete(() =>
                {
                    img.enabled = true;
                });
            }
            else
            {
                // bình thường: đảm bảo hiển thị và tween
                img.enabled = true;
                img.rectTransform.DOAnchorPos(positions[target], moveDuration);
            }

            // cập nhật index cho ảnh này (quan trọng để lần sau tính đúng)
            currentIndexOfImage[i] = target;
        }
    }
}