using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Import DOTween

public class Shop : MonoBehaviour
{
    public static Shop instance;
    public Image imageShop;
    public Button Image;
    public Image imagePlane1;
    public Image imagePlane2;
    public Image imagePlane3;
    public Image imageBackgroundPlane1;
    public Image imageBackgroundPlane2;
    public Image imageBackgroundPlane3;
    public Sprite[] spritePlanes;
    public Sprite[] spriteBackgrounds; // Thêm array cho background sprites

    public Button button0;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    [Header("Thay đổi nhân vật")]
    public GameObject[] gameObjectsPlanes;

    [Header("Nhân vật ban đầu")]
    public GameObject defaultPlane;

    [Header ("Rigidbody2D của máy bay")]
    public Rigidbody2D[] airplanesRigidbody2D;
    
    private int currentIndex = 1;
    
    [Header("DOTween Settings")]
    public float transitionDuration = 0.5f;
    public Ease transitionEase = Ease.OutQuad;
    public float scaleEffect = 1.2f;
    
    [Header("Background Effects")]
    public float backgroundFadeSpeed = 0.3f;
    public float backgroundScaleEffect = 1.1f;
    public Color backgroundTintColor = new Color(1f, 1f, 1f, 0.8f);
    
    private bool isTransitioning = false;
    
    // LƯU VỊ TRÍ GỐC CỦA CÁC IMAGE
    private Vector3 originalPosPlane1;
    private Vector3 originalPosPlane2;
    private Vector3 originalPosPlane3;
    private Vector3 originalPosBg1;
    private Vector3 originalPosBg2;
    private Vector3 originalPosBg3;
    
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
        // Lưu vị trí Plane
        originalPosPlane1 = imagePlane1.transform.localPosition;
        originalPosPlane2 = imagePlane2.transform.localPosition;
        originalPosPlane3 = imagePlane3.transform.localPosition;
        
        // Lưu vị trí Background
        originalPosBg1 = imageBackgroundPlane1.transform.localPosition;
        originalPosBg2 = imageBackgroundPlane2.transform.localPosition;
        originalPosBg3 = imageBackgroundPlane3.transform.localPosition;
        
        Debug.Log($"Saved original positions - Planes & Backgrounds");
    }
    
    void ResetToOriginalPositions()
    {
        // Reset Plane positions
        imagePlane1.transform.localPosition = originalPosPlane1;
        imagePlane2.transform.localPosition = originalPosPlane2;
        imagePlane3.transform.localPosition = originalPosPlane3;
        
        // Reset Background positions
        imageBackgroundPlane1.transform.localPosition = originalPosBg1;
        imageBackgroundPlane2.transform.localPosition = originalPosBg2;
        imageBackgroundPlane3.transform.localPosition = originalPosBg3;
    }
    
    void InitializePlanes()
    {
        if (spritePlanes != null && spritePlanes.Length >= 4)
        {
            // Khởi tạo Plane sprites
            imagePlane1.sprite = spritePlanes[1];
            imagePlane2.sprite = spritePlanes[2];
            imagePlane3.sprite = spritePlanes[3];
            
            // Khởi tạo Background sprites (nếu có)
            if (spriteBackgrounds != null && spriteBackgrounds.Length >= 4)
            {
                imageBackgroundPlane1.sprite = spriteBackgrounds[1];
                imageBackgroundPlane2.sprite = spriteBackgrounds[2];
                imageBackgroundPlane3.sprite = spriteBackgrounds[3];
            }
            
            currentIndex = 1;
            
            // Reset về vị trí gốc
            ResetToOriginalPositions();
            
            // Đặt scale và alpha ban đầu cho Planes
            ResetPlaneStates();
            
            // Đặt scale và alpha ban đầu cho Backgrounds
            ResetBackgroundStates();
            
            Debug.Log("Khởi tạo máy bay và background: Index 1, 2, 3");
        }
    }
    
    void ResetPlaneStates()
    {
        imagePlane1.transform.localScale = Vector3.one;
        imagePlane2.transform.localScale = Vector3.one;
        imagePlane3.transform.localScale = Vector3.one;
        imagePlane1.color = Color.white;
        imagePlane2.color = Color.white;
        imagePlane3.color = Color.white;
    }
    
    void ResetBackgroundStates()
    {
        imageBackgroundPlane1.transform.localScale = Vector3.one;
        imageBackgroundPlane2.transform.localScale = Vector3.one;
        imageBackgroundPlane3.transform.localScale = Vector3.one;
        imageBackgroundPlane1.color = Color.white;
        imageBackgroundPlane2.color = Color.white;
        imageBackgroundPlane3.color = Color.white;
    }

    public void OpenShop()
    {
        Debug.Log("Mở cửa hàng!");
        
        // Dừng tất cả animation đang chạy
        DOTween.Kill(this);
        isTransitioning = false;

        Settings.instance.lastDistanceText.gameObject.SetActive(false);
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
            .OnComplete(() =>
            {
                imageShop.gameObject.SetActive(false);
                Settings.instance.lastDistanceText.gameObject.SetActive(true);
            });
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
        
        // DỪNG TẤT CẢ ANIMATION ĐANG CHẠY
        KillAllAnimations();
        
        // RESET VỀ VỊ TRÍ GỐC TRƯỚC KHI BẮT ĐẦU ANIMATION MỚI
        ResetToOriginalPositions();
        
        // Đảm bảo alpha = 1
        ResetPlaneStates();
        ResetBackgroundStates();
        
        Debug.Log($"Bắt đầu transition từ index {currentIndex} → {newIndex}, Direction: {(isMovingRight ? "Right" : "Left")}");
        
        // Hiệu ứng slide out (trượt ra)
        float slideDistance = 200f;
        Vector3 slideOutDirection = isMovingRight ? Vector3.left : Vector3.right;
        slideOutDirection *= slideDistance;
        
        // Phase 1: Background Animation Out (chạy trước, nhanh hơn)
        yield return StartCoroutine(AnimateBackgroundOut(slideOutDirection));
        
        // Phase 2: Plane Animation Out
        yield return StartCoroutine(AnimatePlaneOut(slideOutDirection));
        
        // Cập nhật sprite mới
        UpdateSprites(newIndex);
        
        // Đặt vị trí ban đầu cho slide in (từ phía ngược lại)
        Vector3 slideInDirection = isMovingRight ? Vector3.right : Vector3.left;
        slideInDirection *= slideDistance;
        SetSlideInPositions(slideInDirection);
        
        // Phase 3: Background Animation In (chạy trước)
        yield return StartCoroutine(AnimateBackgroundIn(slideInDirection));
        
        // Phase 4: Plane Animation In (chạy sau, có hiệu ứng đẹp hơn)
        yield return StartCoroutine(AnimatePlaneIn(slideInDirection));
        
        // ĐẢM BẢO RESET VỀ TRẠNG THÁI BAN ĐẦU
        FinalizeTransition();
        
        isTransitioning = false;
        Debug.Log($"Transition hoàn thành - Current Index: {currentIndex}");
    }
    
    void KillAllAnimations()
    {
        // Kill Plane animations
        DOTween.Kill(imagePlane1.transform);
        DOTween.Kill(imagePlane2.transform);
        DOTween.Kill(imagePlane3.transform);
        DOTween.Kill(imagePlane1);
        DOTween.Kill(imagePlane2);
        DOTween.Kill(imagePlane3);
        
        // Kill Background animations
        DOTween.Kill(imageBackgroundPlane1.transform);
        DOTween.Kill(imageBackgroundPlane2.transform);
        DOTween.Kill(imageBackgroundPlane3.transform);
        DOTween.Kill(imageBackgroundPlane1);
        DOTween.Kill(imageBackgroundPlane2);
        DOTween.Kill(imageBackgroundPlane3);
    }
    
    IEnumerator AnimateBackgroundOut(Vector3 slideDirection)
    {
        Sequence bgOutSequence = DOTween.Sequence();
        
        // Background slide out với rotation và scale
        bgOutSequence.Append(imageBackgroundPlane1.transform.DOLocalMove(
            originalPosBg1 + slideDirection, backgroundFadeSpeed)
            .SetEase(Ease.InCubic));
        bgOutSequence.Join(imageBackgroundPlane2.transform.DOLocalMove(
            originalPosBg2 + slideDirection, backgroundFadeSpeed)
            .SetEase(Ease.InCubic));
        bgOutSequence.Join(imageBackgroundPlane3.transform.DOLocalMove(
            originalPosBg3 + slideDirection, backgroundFadeSpeed)
            .SetEase(Ease.InCubic));
        
        // Background fade và scale out
        bgOutSequence.Join(imageBackgroundPlane1.DOFade(0f, backgroundFadeSpeed));
        bgOutSequence.Join(imageBackgroundPlane2.DOFade(0f, backgroundFadeSpeed));
        bgOutSequence.Join(imageBackgroundPlane3.DOFade(0f, backgroundFadeSpeed));
        
        bgOutSequence.Join(imageBackgroundPlane1.transform.DOScale(0.8f, backgroundFadeSpeed));
        bgOutSequence.Join(imageBackgroundPlane2.transform.DOScale(0.8f, backgroundFadeSpeed));
        bgOutSequence.Join(imageBackgroundPlane3.transform.DOScale(0.8f, backgroundFadeSpeed));
        
        yield return bgOutSequence.WaitForCompletion();
    }
    
    IEnumerator AnimatePlaneOut(Vector3 slideDirection)
    {
        Sequence planeOutSequence = DOTween.Sequence();
        
        // Plane slide out
        planeOutSequence.Append(imagePlane1.transform.DOLocalMove(
            originalPosPlane1 + slideDirection, transitionDuration * 0.5f)
            .SetEase(transitionEase));
        planeOutSequence.Join(imagePlane2.transform.DOLocalMove(
            originalPosPlane2 + slideDirection, transitionDuration * 0.5f)
            .SetEase(transitionEase));
        planeOutSequence.Join(imagePlane3.transform.DOLocalMove(
            originalPosPlane3 + slideDirection, transitionDuration * 0.5f)
            .SetEase(transitionEase));
        
        // Plane fade out
        planeOutSequence.Join(imagePlane1.DOFade(0f, transitionDuration * 0.5f));
        planeOutSequence.Join(imagePlane2.DOFade(0f, transitionDuration * 0.5f));
        planeOutSequence.Join(imagePlane3.DOFade(0f, transitionDuration * 0.5f));
        
        yield return planeOutSequence.WaitForCompletion();
    }
    
    void UpdateSprites(int newIndex)
    {
        currentIndex = newIndex;
        
        // Update Plane sprites
        imagePlane1.sprite = spritePlanes[currentIndex];
        imagePlane2.sprite = spritePlanes[currentIndex + 1];
        imagePlane3.sprite = spritePlanes[currentIndex + 2];
        
        // Update Background sprites (nếu có)
        if (spriteBackgrounds != null && spriteBackgrounds.Length > currentIndex + 2)
        {
            imageBackgroundPlane1.sprite = spriteBackgrounds[currentIndex];
            imageBackgroundPlane2.sprite = spriteBackgrounds[currentIndex + 1];
            imageBackgroundPlane3.sprite = spriteBackgrounds[currentIndex + 2];
        }
    }
    
    void SetSlideInPositions(Vector3 slideDirection)
    {
        // Set Plane positions
        imagePlane1.transform.localPosition = originalPosPlane1 + slideDirection;
        imagePlane2.transform.localPosition = originalPosPlane2 + slideDirection;
        imagePlane3.transform.localPosition = originalPosPlane3 + slideDirection;
        
        // Set Background positions
        imageBackgroundPlane1.transform.localPosition = originalPosBg1 + slideDirection;
        imageBackgroundPlane2.transform.localPosition = originalPosBg2 + slideDirection;
        imageBackgroundPlane3.transform.localPosition = originalPosBg3 + slideDirection;
    }
    
    IEnumerator AnimateBackgroundIn(Vector3 slideDirection)
    {
        Sequence bgInSequence = DOTween.Sequence();
        
        // Background slide in với hiệu ứng đẹp
        bgInSequence.Append(imageBackgroundPlane1.transform.DOLocalMove(
            originalPosBg1, backgroundFadeSpeed)
            .SetEase(Ease.OutBack));
        bgInSequence.Join(imageBackgroundPlane2.transform.DOLocalMove(
            originalPosBg2, backgroundFadeSpeed)
            .SetEase(Ease.OutBack));
        bgInSequence.Join(imageBackgroundPlane3.transform.DOLocalMove(
            originalPosBg3, backgroundFadeSpeed)
            .SetEase(Ease.OutBack));
        
        // Background fade in với color tint
        bgInSequence.Join(imageBackgroundPlane1.DOFade(1f, backgroundFadeSpeed));
        bgInSequence.Join(imageBackgroundPlane2.DOFade(1f, backgroundFadeSpeed));
        bgInSequence.Join(imageBackgroundPlane3.DOFade(1f, backgroundFadeSpeed));
        
        // Background scale effect
        bgInSequence.Join(imageBackgroundPlane1.transform.DOScale(backgroundScaleEffect, backgroundFadeSpeed * 0.5f)
            .SetLoops(2, LoopType.Yoyo));
        bgInSequence.Join(imageBackgroundPlane2.transform.DOScale(backgroundScaleEffect, backgroundFadeSpeed * 0.5f)
            .SetLoops(2, LoopType.Yoyo));
        bgInSequence.Join(imageBackgroundPlane3.transform.DOScale(backgroundScaleEffect, backgroundFadeSpeed * 0.5f)
            .SetLoops(2, LoopType.Yoyo));
        
        yield return bgInSequence.WaitForCompletion();
    }
    
    IEnumerator AnimatePlaneIn(Vector3 slideDirection)
    {
        Sequence planeInSequence = DOTween.Sequence();
        
        // Plane slide in về vị trí gốc
        planeInSequence.Append(imagePlane1.transform.DOLocalMove(
            originalPosPlane1, transitionDuration * 0.5f)
            .SetEase(transitionEase));
        planeInSequence.Join(imagePlane2.transform.DOLocalMove(
            originalPosPlane2, transitionDuration * 0.5f)
            .SetEase(transitionEase));
        planeInSequence.Join(imagePlane3.transform.DOLocalMove(
            originalPosPlane3, transitionDuration * 0.5f)
            .SetEase(transitionEase));
        
        // Plane fade in
        planeInSequence.Join(imagePlane1.DOFade(1f, transitionDuration * 0.5f));
        planeInSequence.Join(imagePlane2.DOFade(1f, transitionDuration * 0.5f));
        planeInSequence.Join(imagePlane3.DOFade(1f, transitionDuration * 0.5f));
        
        // Hiệu ứng scale bounce cho Plane
        planeInSequence.Join(imagePlane1.transform.DOScale(scaleEffect, transitionDuration * 0.25f)
            .SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutBounce));
        planeInSequence.Join(imagePlane2.transform.DOScale(scaleEffect, transitionDuration * 0.25f)
            .SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutBounce));
        planeInSequence.Join(imagePlane3.transform.DOScale(scaleEffect, transitionDuration * 0.25f)
            .SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutBounce));
        
        yield return planeInSequence.WaitForCompletion();
    }
    
    void FinalizeTransition()
    {
        ResetToOriginalPositions();
        ResetPlaneStates();
        ResetBackgroundStates();
    }

    void OnDestroy()
    {
        // Cleanup DOTween để tránh memory leak
        DOTween.Kill(this);
    }

    public bool isCurrentIndex = false;

    public void btn1BuyCurrentPlane()
    {
        int selectedIndex = currentIndex;
        Debug.Log($"btn1BuyCurrentPlane: Mua máy bay tại index {selectedIndex} - {gameObjectsPlanes[selectedIndex].name}");
        
        defaultPlane = gameObjectsPlanes[selectedIndex];
        GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[selectedIndex];
        if (CameraManager.instance == null)
        {
            Debug.LogError("CameraManager instance is NULL!");
            return;
        }
        else if (airplanesRigidbody2D[selectedIndex] == null)
        {
            Debug.LogError($"Airplane Rigidbody2D at index {selectedIndex} is NULL!");
            return;
        }
        CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[selectedIndex].transform;
        CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[selectedIndex].transform;
        airplanesRigidbody2D[selectedIndex].velocity = Vector2.zero; // Đặt vận tốc về 0 khi chuyển máy bay
        
        Debug.Log($"Calling UpdateAircraftForCamera with: {airplanesRigidbody2D[selectedIndex].name}");

        for (int i = 0; i < gameObjectsPlanes.Length; i++)
        {
            if (i == selectedIndex)
            {
                gameObjectsPlanes[i].SetActive(true);
                Debug.Log($"Activated aircraft: {gameObjectsPlanes[i].name}");
            }
            else
            {
                gameObjectsPlanes[i].SetActive(false);
            }
        }
        if (currentIndex == 12){
            isCurrentIndex = true;
        }
    }

    public void btn2BuyCurrentPlane()
    {
        Debug.Log($"vị trí máy bay tại index {currentIndex + 1}");
        defaultPlane = gameObjectsPlanes[currentIndex + 1];
        GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[currentIndex + 1];
        CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[currentIndex + 1].transform;
        CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[currentIndex + 1].transform;
        airplanesRigidbody2D[currentIndex + 1].velocity = Vector2.zero; // Đặt vận tốc về 0 khi chuyển máy bay



        for (int i = 0; i < gameObjectsPlanes.Length; i++)
        {
            if (i == currentIndex + 1)
            {
                gameObjectsPlanes[i].SetActive(true);
            }
            else
            {
                gameObjectsPlanes[i].SetActive(false);
            }
        }
        
        if (currentIndex+1 == 12){
            isCurrentIndex = true;
        }
    }
    public void btn3BuyCurrentPlane()
    {
        Debug.Log($"vị trí máy bay tại index {currentIndex + 2}");
        defaultPlane = gameObjectsPlanes[currentIndex + 2];
        GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[currentIndex + 2];
        CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[currentIndex + 2].transform;
        CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[currentIndex + 2].transform;
        airplanesRigidbody2D[currentIndex + 2].velocity = Vector2.zero; // Đặt vận tốc về 0 khi chuyển máy bay



        for (int i = 0; i < gameObjectsPlanes.Length; i++)
        {
            if (i == currentIndex + 2)
            {
                gameObjectsPlanes[i].SetActive(true);
            }
            else
            {
                gameObjectsPlanes[i].SetActive(false);
            }
        }
        if (currentIndex+2 == 12){
            isCurrentIndex = true;
        }
    }
}