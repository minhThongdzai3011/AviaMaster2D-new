using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Import DOTween


public class Shop : MonoBehaviour
{
    public static Shop instance;
    public Image imageShop;
    public Image imagePlane1;
    public Image imagePlane2;
    public Image imagePlane3;
    public Sprite[] spritePlanes;
    
    private int currentIndex = 1;
    
    [Header("DOTween Settings")]
    public float transitionDuration = 0.5f;
    public Ease transitionEase = Ease.OutQuad;
    public float scaleEffect = 1.2f;
    
    private bool isTransitioning = false;
    
    // LƯU VỊ TRÍ GỐC CỦA CÁC IMAGE
    private Vector3 originalPosPlane1;
    private Vector3 originalPosPlane2;
    private Vector3 originalPosPlane3;
    
    void Start()
    {
        instance = this;
        
        // Lưu vị trí gốc của các image
        SaveOriginalPositions();
        
        // Khởi tạo ban đầu với spritePlanes[1,2,3]
        InitializePlanes();
    }

    void SaveOriginalPositions()
    {
        originalPosPlane1 = imagePlane1.transform.localPosition;
        originalPosPlane2 = imagePlane2.transform.localPosition;
        originalPosPlane3 = imagePlane3.transform.localPosition;
        
        Debug.Log($"Saved original positions - Plane1: {originalPosPlane1}, Plane2: {originalPosPlane2}, Plane3: {originalPosPlane3}");
    }
    
    void ResetToOriginalPositions()
    {
        imagePlane1.transform.localPosition = originalPosPlane1;
        imagePlane2.transform.localPosition = originalPosPlane2;
        imagePlane3.transform.localPosition = originalPosPlane3;
    }
    
    void InitializePlanes()
    {
        if (spritePlanes != null && spritePlanes.Length >= 4)
        {
            imagePlane1.sprite = spritePlanes[1];
            imagePlane2.sprite = spritePlanes[2];
            imagePlane3.sprite = spritePlanes[3];
            currentIndex = 1;
            
            // Reset về vị trí gốc
            ResetToOriginalPositions();
            
            // Đặt scale và alpha ban đầu
            imagePlane1.transform.localScale = Vector3.one;
            imagePlane2.transform.localScale = Vector3.one;
            imagePlane3.transform.localScale = Vector3.one;
            imagePlane1.color = Color.white;
            imagePlane2.color = Color.white;
            imagePlane3.color = Color.white;
            
            Debug.Log("Khởi tạo máy bay: Plane1=1, Plane2=2, Plane3=3");
        }
    }

    public void OpenShop()
    {
        Debug.Log("Mở cửa hàng!");
        
        // Dừng tất cả animation đang chạy
        DOTween.Kill(this);
        isTransitioning = false;
        
        imageShop.gameObject.SetActive(true);
        
        // Đảm bảo hiển thị đúng khi mở shop
        InitializePlanes();
        
        // Animation mở shop
        imageShop.transform.localScale = Vector3.zero;
        imageShop.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void CloseShop()
    {
        Debug.Log("Đóng cửa hàng!");
        
        // Dừng tất cả animation
        DOTween.Kill(this);
        isTransitioning = false;
        
        // Animation đóng shop
        imageShop.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() => imageShop.gameObject.SetActive(false));
    }

    public void buttonRight()
    {
        if (spritePlanes == null || spritePlanes.Length == 0 || isTransitioning) 
        {
            Debug.Log("Button Right bị block - isTransitioning: " + isTransitioning);
            return;
        }
        
        // Tính index mới
        int newIndex = currentIndex + 1;
        if (newIndex >= spritePlanes.Length - 2)
        {
            newIndex = 0;
        }
        
        // Chạy animation chuyển đổi
        StartCoroutine(TransitionSprites(newIndex, true));
    }
    
    public void buttonLeft()
    {
        if (spritePlanes == null || spritePlanes.Length == 0 || isTransitioning) 
        {
            Debug.Log("Button Left bị block - isTransitioning: " + isTransitioning);
            return;
        }
        
        // Tính index mới
        int newIndex = currentIndex - 1;
        if (newIndex < 0)
        {
            newIndex = spritePlanes.Length - 3;
        }
        
        // Chạy animation chuyển đổi
        StartCoroutine(TransitionSprites(newIndex, false));
    }
    
    IEnumerator TransitionSprites(int newIndex, bool isMovingRight)
    {
        isTransitioning = true;
        
        // DỪNG TẤT CẢ ANIMATION ĐANG CHẠY TRÊN CÁC IMAGE
        DOTween.Kill(imagePlane1.transform);
        DOTween.Kill(imagePlane2.transform);
        DOTween.Kill(imagePlane3.transform);
        DOTween.Kill(imagePlane1);
        DOTween.Kill(imagePlane2);
        DOTween.Kill(imagePlane3);
        
        // RESET VỀ VỊ TRÍ GỐC TRƯỚC KHI BẮT ĐẦU ANIMATION MỚI
        ResetToOriginalPositions();
        
        // Đảm bảo alpha = 1
        imagePlane1.color = Color.white;
        imagePlane2.color = Color.white;
        imagePlane3.color = Color.white;
        
        Debug.Log($"Bắt đầu transition từ index {currentIndex} → {newIndex}, Direction: {(isMovingRight ? "Right" : "Left")}");
        
        // Hiệu ứng slide out (trượt ra)
        float slideDistance = 200f;
        Vector3 slideOutDirection = isMovingRight ? Vector3.left : Vector3.right;
        slideOutDirection *= slideDistance;
        
        // Phase 1: Slide out và fade out các image hiện tại
        Sequence slideOutSequence = DOTween.Sequence();
        
        slideOutSequence.Append(imagePlane1.transform.DOLocalMove(
            originalPosPlane1 + slideOutDirection, transitionDuration * 0.5f)
            .SetEase(transitionEase));
        slideOutSequence.Join(imagePlane2.transform.DOLocalMove(
            originalPosPlane2 + slideOutDirection, transitionDuration * 0.5f)
            .SetEase(transitionEase));
        slideOutSequence.Join(imagePlane3.transform.DOLocalMove(
            originalPosPlane3 + slideOutDirection, transitionDuration * 0.5f)
            .SetEase(transitionEase));
        
        // Fade out
        slideOutSequence.Join(imagePlane1.DOFade(0f, transitionDuration * 0.5f));
        slideOutSequence.Join(imagePlane2.DOFade(0f, transitionDuration * 0.5f));
        slideOutSequence.Join(imagePlane3.DOFade(0f, transitionDuration * 0.5f));
        
        // Chờ animation slide out hoàn thành
        yield return slideOutSequence.WaitForCompletion();
        
        // Cập nhật sprite mới
        currentIndex = newIndex;
        imagePlane1.sprite = spritePlanes[currentIndex];
        imagePlane2.sprite = spritePlanes[currentIndex + 1];
        imagePlane3.sprite = spritePlanes[currentIndex + 2];
        
        // Đặt vị trí ban đầu cho slide in (từ phía ngược lại)
        Vector3 slideInDirection = isMovingRight ? Vector3.right : Vector3.left;
        slideInDirection *= slideDistance;
        
        imagePlane1.transform.localPosition = originalPosPlane1 + slideInDirection;
        imagePlane2.transform.localPosition = originalPosPlane2 + slideInDirection;
        imagePlane3.transform.localPosition = originalPosPlane3 + slideInDirection;
        
        // Phase 2: Slide in và fade in các image mới
        Sequence slideInSequence = DOTween.Sequence();
        
        // Slide in về vị trí gốc
        slideInSequence.Append(imagePlane1.transform.DOLocalMove(
            originalPosPlane1, transitionDuration * 0.5f)
            .SetEase(transitionEase));
        slideInSequence.Join(imagePlane2.transform.DOLocalMove(
            originalPosPlane2, transitionDuration * 0.5f)
            .SetEase(transitionEase));
        slideInSequence.Join(imagePlane3.transform.DOLocalMove(
            originalPosPlane3, transitionDuration * 0.5f)
            .SetEase(transitionEase));
        
        // Fade in
        slideInSequence.Join(imagePlane1.DOFade(1f, transitionDuration * 0.5f));
        slideInSequence.Join(imagePlane2.DOFade(1f, transitionDuration * 0.5f));
        slideInSequence.Join(imagePlane3.DOFade(1f, transitionDuration * 0.5f));
        
        // Hiệu ứng scale nhẹ
        slideInSequence.Join(imagePlane1.transform.DOScale(scaleEffect, transitionDuration * 0.25f)
            .SetLoops(2, LoopType.Yoyo));
        slideInSequence.Join(imagePlane2.transform.DOScale(scaleEffect, transitionDuration * 0.25f)
            .SetLoops(2, LoopType.Yoyo));
        slideInSequence.Join(imagePlane3.transform.DOScale(scaleEffect, transitionDuration * 0.25f)
            .SetLoops(2, LoopType.Yoyo));
        
        yield return slideInSequence.WaitForCompletion();
        
        // ĐẢM BẢO RESET VỀ TRẠNG THÁI BAN ĐẦU
        ResetToOriginalPositions();
        imagePlane1.transform.localScale = Vector3.one;
        imagePlane2.transform.localScale = Vector3.one;
        imagePlane3.transform.localScale = Vector3.one;
        
        isTransitioning = false;
        Debug.Log($"Transition hoàn thành - Current Index: {currentIndex}, Planes: [{currentIndex}, {currentIndex + 1}, {currentIndex + 2}]");
    }
    
    void OnDestroy()
    {
        // Cleanup DOTween để tránh memory leak
        DOTween.Kill(this);
    }
}
