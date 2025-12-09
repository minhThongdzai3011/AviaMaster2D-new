using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

public class Shop : MonoBehaviour
{
    public static Shop instance;
    public Image imageShop;
    [Header("Image UI")]
    public Image[] imagePlayPlanes;
    [Header("Text UI")]
    public TextMeshProUGUI[] planePriceText;
    public TextMeshProUGUI[] planeBuyText;
    public TextMeshProUGUI pageText;
    private int page = 1;

    [Header("Enhanced Carousel System")]
    public Image[] planeImages; 
    
    [Header("Visual Settings")]
    public float moveDuration = 0.5f;
    public Ease moveEase = Ease.OutQuart;
    
    [Header("Advanced Effects")]
    public float rotationAngle = 20f;
    public float parallaxIntensity = 50f;
    public Color highlightColor = new Color(1f, 0.9f, 0.4f, 1f);
    public float glowPulseSpeed = 2f;
    
    [Header("Purchase Effects")]
    public ParticleSystem purchaseParticles;
    public AudioClip purchaseSound;
    
    private Vector2[] originalPositions;
    private int[] currentOrder;
    private bool isAnimating = false;
    
    // Tính toán centerIndex động dựa trên số lượng máy bay
    private int CenterIndex => planeImages.Length / 2;
    
    // Game Objects reference
    [Header("Aircraft GameObjects")]
    public GameObject[] gameObjectsPlanes;
    public Rigidbody2D[] airplanesRigidbody2D;
    public GameObject defaultPlane;
    public int isCheckedPlaneIndex = 13; // Mặc định chọn máy bay thứ 15

    public void Awake()
    {
        isCheckedPlaneIndex = PlayerPrefs.GetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
    
        // StartCoroutine(LoadTextPlaySelectDelayed());
    }
    void Start()
    {
        instance = this;
        Application.targetFrameRate = 60;
        InitializeCarousel();
        
        // *** SỬA: Load trạng thái bought trước ***
        LoadBoughtStatus();
        
        // *** SỬA: Setup plane theo index được lưu ***
        SetupPlaneByIndex(isCheckedPlaneIndex);
        
        // *** SỬA: Update UI dựa vào trạng thái bought và selected ***
        UpdateAllButtonsDisplay();

    }
    void Update()
    {
        foreach (var text in planeBuyText)
        {
            if(text.text == "Play"){
                text.color = highlightColor;
            }
            else {
                text.color = Color.white;
            }
        }
    }

    // THÊM: Hàm load trạng thái mua từ PlayerPrefs
    void LoadBoughtStatus()
    {
        isBuyPlane1Done = PlayerPrefs.GetInt("isBuyPlane1Done", 0) == 1;
        isBuyPlane2Done = PlayerPrefs.GetInt("isBuyPlane2Done", 0) == 1;
        isBuyPlane3Done = PlayerPrefs.GetInt("isBuyPlane3Done", 0) == 1;
        isBuyPlane4Done = PlayerPrefs.GetInt("isBuyPlane4Done", 0) == 1;
        isBuyPlane5Done = PlayerPrefs.GetInt("isBuyPlane5Done", 0) == 1;
        isBuyPlane6Done = PlayerPrefs.GetInt("isBuyPlane6Done", 0) == 1;
        isBuyPlane7Done = PlayerPrefs.GetInt("isBuyPlane7Done", 0) == 1;
        isBuyPlane8Done = PlayerPrefs.GetInt("isBuyPlane8Done", 0) == 1;
        isBuyPlane9Done = PlayerPrefs.GetInt("isBuyPlane9Done", 0) == 1;
        isBuyPlane10Done = PlayerPrefs.GetInt("isBuyPlane10Done", 0) == 1;
        isBuyPlane11Done = PlayerPrefs.GetInt("isBuyPlane11Done", 0) == 1;
        isBuyPlane12Done = PlayerPrefs.GetInt("isBuyPlane12Done", 0) == 1;
        isBuyPlane13Done = PlayerPrefs.GetInt("isBuyPlane13Done", 0) == 1;
        isBuyPlane14Done = PlayerPrefs.GetInt("isBuyPlane14Done", 1) == 1;
        isRotaryFrontZDone = PlayerPrefs.GetInt("isRotaryFrontZDone", 0) == 1;

        if (!isBuyPlane14Done)
        {
            isBuyPlane14Done = true;
            PlayerPrefs.SetInt("isBuyPlane14Done", 1);
            PlayerPrefs.Save();
        }
    }

    // THÊM: Hàm cập nhật hiển thị tất cả buttons
    void UpdateAllButtonsDisplay()
    {
        for (int i = 0; i < planeBuyText.Length && i < 15; i++)
        {
            UpdateButtonDisplay(i);
        }
    }

    // THÊM: Hàm cập nhật hiển thị cho 1 button
    void UpdateButtonDisplay(int index)
    {
        if (index < 0 || index >= planeBuyText.Length) return;
        
        bool isBought = GetPlaneBoughtStatus(index);
        bool isSelected = (index == isCheckedPlaneIndex);
        
        if (!isBought)
        {
            planePriceText[index].gameObject.SetActive(true);
            planeBuyText[index].gameObject.SetActive(false);
        }
        else
        {
            planePriceText[index].gameObject.SetActive(false);
            planeBuyText[index].gameObject.SetActive(true);
            
            if (isSelected)
            {
                planeBuyText[index].text = "Play";
            }
            else
            {
                planeBuyText[index].text = "Select";
            }
        }
    }

    // THÊM: Hàm lấy trạng thái bought của plane
    bool GetPlaneBoughtStatus(int index)
    {
        switch (index)
        {
            case 0: return isBuyPlane1Done;
            case 1: return isBuyPlane2Done;
            case 2: return isBuyPlane3Done;
            case 3: return isBuyPlane4Done;
            case 4: return isBuyPlane5Done;
            case 5: return isBuyPlane6Done;
            case 6: return isBuyPlane7Done;
            case 7: return isBuyPlane8Done;
            case 8: return isBuyPlane9Done;
            case 9: return isBuyPlane10Done;
            case 10: return isBuyPlane11Done;
            case 11: return isBuyPlane12Done;
            case 12: return isBuyPlane13Done;
            case 13: return isBuyPlane14Done;
            default: return false;
        }
    }

    void InitializeCarousel()
    {
        int length = planeImages.Length;
        originalPositions = new Vector2[length];
        currentOrder = new int[length];
        
        // Lưu vị trí ban đầu và khởi tạo thứ tự
        for (int i = 0; i < length; i++)
        {
            originalPositions[i] = planeImages[i].rectTransform.anchoredPosition;
            currentOrder[i] = i;
        }
        
        // Thiết lập trạng thái ban đầu với hiệu ứng
        StartCoroutine(InitialShowcase());
    }
    
    // Method để reinitialize carousel khi cần
    public void ReinitializeCarousel()
    {
        // Reset về trạng thái ban đầu
        for (int i = 0; i < planeImages.Length; i++)
        {
            currentOrder[i] = i;
        }
        UpdateCarouselDisplay();
    }
    
    IEnumerator InitialShowcase()
    {
        // Hiệu ứng xuất hiện đơn giản
        foreach (var img in planeImages)
        {
            img.transform.localScale = Vector3.one;
            img.color = Color.white;
        }
        
        yield return new WaitForSeconds(0.1f);
        
        // Áp dụng trạng thái carousel
        UpdateCarouselDisplay();
    }
    
    IEnumerator ShowPlaneWithEffect(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Image plane = planeImages[index];
        
        // Scale animation với bounce - giữ nguyên scale = 1
        plane.transform.DOScale(1f, 0.5f)
            .SetEase(Ease.OutBounce);
            
        // Fade in
        plane.DOFade(1f, 0.3f);
        
        // Rotation effect
        plane.transform.DORotate(Vector3.forward * Random.Range(-5f, 5f), 0.2f)
            .OnComplete(() => plane.transform.DORotate(Vector3.zero, 0.3f));
    }

    public void OnClickRightArrow()
    {
        if (!isAnimating) StartCoroutine(SlideToDirection(-3));
        page++;
        pageText.text = page > 5 ? "1" : "" + page;
    }

    public void OnClickLeftArrow()
    {
        if (!isAnimating) StartCoroutine(SlideToDirection(3));
        page--;
        pageText.text = page < 1 ? "5" : "" + page;
    }
    
    IEnumerator SlideToDirection(int direction)
    {
        isAnimating = true;
        
        // Phase 1: Slide out với hiệu ứng 3D
        yield return StartCoroutine(SlideOutEffect(direction));
        
        // Cập nhật thứ tự
        ShiftOrder(direction);
        
        // Phase 2: Slide in với hiệu ứng mới
        yield return StartCoroutine(SlideInEffect(-direction));
        
        // Phase 3: Cập nhật trạng thái carousel
        UpdateCarouselDisplay();
        
        isAnimating = false;
    }
    
    IEnumerator SlideOutEffect(int direction)
    {
        Sequence slideOut = DOTween.Sequence();
        
        for (int i = 0; i < planeImages.Length; i++)
        {
            Image plane = planeImages[i];
            Vector3 targetPos = plane.rectTransform.anchoredPosition + 
                               new Vector2(direction * 50f, 0f);
            
            slideOut.Join(plane.rectTransform.DOAnchorPos(targetPos, moveDuration)
                .SetEase(Ease.OutQuart));
        }
        
        yield return slideOut.WaitForCompletion();
    }
    
    IEnumerator SlideInEffect(int direction)
    {
        // Reset positions for slide in
        for (int i = 0; i < planeImages.Length; i++)
        {
            Vector3 startPos = originalPositions[currentOrder[i]] + 
                              new Vector2(direction * 100f, 0f);
            planeImages[i].rectTransform.anchoredPosition = startPos;
        }
        
        Sequence slideIn = DOTween.Sequence();
        
        for (int i = 0; i < planeImages.Length; i++)
        {
            Image plane = planeImages[i];
            Vector2 targetPos = originalPositions[currentOrder[i]];
            
            slideIn.Join(plane.rectTransform.DOAnchorPos(targetPos, moveDuration)
                .SetEase(moveEase));
        }
        
        yield return slideIn.WaitForCompletion();
    }
    
    void UpdateCarouselDisplay()
    {
        for (int i = 0; i < planeImages.Length; i++)
        {
            Image plane = planeImages[i];
            
            // Giữ scale = 1 cho tất cả máy bay
            plane.transform.localScale = Vector3.one;
            
            // Giữ màu trắng cho tất cả máy bay
            plane.color = Color.white;
        }
    }
    
    IEnumerator PulseGlowEffect(Image centerPlane)
    {
        float timer = 0f;
        Color originalColor = Color.white;
        
        while (timer < 1f)
        {
            timer += Time.deltaTime * glowPulseSpeed;
            float pulse = (Mathf.Sin(timer * Mathf.PI * 2f) + 1f) * 0.5f;
            centerPlane.color = Color.Lerp(originalColor, highlightColor, pulse * 0.3f);
            yield return null;
        }
        
        centerPlane.color = originalColor;
    }
    void ShiftOrder(int direction)
    {
        int length = currentOrder.Length;
        int[] newOrder = new int[length];
        
        for (int i = 0; i < length; i++)
        {
            int newIndex = (i - direction + length) % length;
            newOrder[newIndex] = currentOrder[i];
        }
        
        currentOrder = newOrder;
    }
    
    // Hiệu ứng đẹp cho button máy bay
    IEnumerator PlayButtonEffect(int planeIndex)
    {
        if (planeIndex < 0 || planeIndex >= planeImages.Length) yield break;
        
        Image targetPlane = planeImages[planeIndex];
        
        // Hiệu ứng scale: 0.9 -> 1.2 -> 1
        Sequence scaleSeq = DOTween.Sequence();
        scaleSeq.Append(targetPlane.transform.DOScale(0.9f, 0.1f).SetEase(Ease.OutQuart));
        scaleSeq.Append(targetPlane.transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack));
        scaleSeq.Append(targetPlane.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce));
        
        yield return scaleSeq.WaitForCompletion();
    }
    
    // Buy functions với enhanced effects
    public void BuyCenterPlane()
    {
        if (isAnimating) return;
        StartCoroutine(PurchaseEffect());
    }
    
    // Helper method để lấy index của máy bay đang ở giữa
    int GetCenterPlaneIndex()
    {
        return currentOrder[CenterIndex];
    }
    
    IEnumerator PurchaseEffect()
    {
        Image centerPlane = planeImages[CenterIndex];
        
        // Purchase animation sequence
        Sequence purchaseSeq = DOTween.Sequence();
        
        // Pulse effect - giữ scale từ 1 đến 1.2 rồi về 1
        purchaseSeq.Append(centerPlane.transform.DOScale(1.2f, 0.2f)
            .SetEase(Ease.OutQuart));
        purchaseSeq.Append(centerPlane.transform.DOScale(1f, 0.3f)
            .SetEase(Ease.OutBounce));
            
        // Color flash
        purchaseSeq.Join(centerPlane.DOColor(Color.green, 0.1f)
            .SetLoops(6, LoopType.Yoyo));
            
        // Rotation celebration
        purchaseSeq.Join(centerPlane.transform.DORotate(
            Vector3.forward * 360f, 0.8f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuart));
        
        // Particle effect
        if (purchaseParticles != null)
        {
            purchaseParticles.transform.position = centerPlane.transform.position;
            purchaseParticles.Play();
        }
        
        yield return purchaseSeq.WaitForCompletion();
        
        // Apply purchase logic - sử dụng helper method
        int planeIndex = GetCenterPlaneIndex();
        if (gameObjectsPlanes != null && planeIndex < gameObjectsPlanes.Length)
        {
            BuyPlane(planeIndex);
        }
    }
    
    void BuyPlane(int planeIndex)
    {
        if (gameObjectsPlanes == null || planeIndex >= gameObjectsPlanes.Length) return;
        
        defaultPlane = gameObjectsPlanes[planeIndex];
        GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[planeIndex];
        
        // Activate selected plane, deactivate others
        for (int i = 0; i < gameObjectsPlanes.Length; i++)
        {
            gameObjectsPlanes[i].SetActive(i == planeIndex);
        }
        
        // Camera update
        CameraManager cameraManager = FindObjectOfType<CameraManager>();
        if (cameraManager != null)
        {
            cameraManager.UpdateAircraftTarget(airplanesRigidbody2D[planeIndex].transform);
            cameraManager.UpdateCinemachineFollow(airplanesRigidbody2D[planeIndex].transform);
        }
        
        Debug.Log($"Purchased plane: {gameObjectsPlanes[planeIndex].name}");
    }
    
    // THÊM: Hàm thiết lập máy bay theo tên (cho hệ thống lưu/load)
    public void SetAirplaneByName(string airplaneName)
    {
        if (gameObjectsPlanes == null) 
        {
            Debug.LogWarning("gameObjectsPlanes is null!");
            return;
        }
        
        // Tìm index của máy bay theo tên
        int targetIndex = -1;
        for (int i = 0; i < gameObjectsPlanes.Length; i++)
        {
            if (gameObjectsPlanes[i].name.Equals(airplaneName, System.StringComparison.OrdinalIgnoreCase))
            {
                targetIndex = i;
                break;
            }
        }
        
        if (targetIndex == -1)
        {
            Debug.LogWarning($"Không tìm thấy máy bay với tên: {airplaneName}");
            return;
        }
        
        // Thiết lập máy bay được tìm thấy
        defaultPlane = gameObjectsPlanes[targetIndex];
        
        // Cập nhật GManager để sử dụng Rigidbody2D của máy bay này
        if (GManager.instance != null && airplanesRigidbody2D != null && targetIndex < airplanesRigidbody2D.Length)
        {
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[targetIndex];
        }
        
        Debug.Log($"Đã khôi phục máy bay: {airplaneName} (Index: {targetIndex})");
    }
    
    // Additional visual effects
    public void AddSparkleEffect(Transform target)
    {
        StartCoroutine(SparkleAnimation(target));
    }
    
    IEnumerator SparkleAnimation(Transform target)
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject sparkle = new GameObject("Sparkle");
            Image sparkleImg = sparkle.AddComponent<Image>();
            sparkleImg.color = highlightColor;
            
            sparkle.transform.SetParent(target);
            sparkle.transform.localPosition = Vector3.zero;
            sparkle.transform.localScale = Vector3.zero;
            
            // Animate sparkle - giữ scale ổn định
            Vector3 randomDir = Random.insideUnitCircle * 100f;
            sparkle.transform.DOLocalMove(randomDir, 0.5f);
            sparkle.transform.DOScale(0.3f, 0.2f).OnComplete(() => 
                sparkle.transform.DOScale(0f, 0.3f).OnComplete(() => 
                    Destroy(sparkle)));
            sparkleImg.DOFade(0f, 0.5f);
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    
    // Shop open/close effects
    public void OpenShop()
    {
        Debug.Log("Mở cửa hàng!");
        AudioManager.instance.PlaySound(AudioManager.instance.buttonSoundClip);
        Settings.instance.pannelGray.gameObject.SetActive(true);
        // Dừng tất cả animation đang chạy
        DOTween.Kill(this);

        Settings.instance.lastDistanceText.gameObject.SetActive(false);
        imageShop.gameObject.SetActive(true); 
        
        // Animation mở shop
        imageShop.transform.localScale = Vector3.zero;
        imageShop.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        CheckPlane.instance.SetActiveShopPlane();
    }

    public void CloseShop()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.buttonSoundClip);
        Debug.Log("Đóng cửa hàng!");
        
        // Dừng tất cả animation
        DOTween.Kill(this);
        
        // ✅ THÊM: Gắn lại máy bay vào camera trước khi đóng shop
        if (CameraManager.instance != null && defaultPlane != null)
        {
            // Tìm Rigidbody2D của máy bay hiện tại
            Rigidbody2D currentPlaneRb = defaultPlane.GetComponent<Rigidbody2D>();
            
            if (currentPlaneRb != null)
            {
                // Cập nhật GManager
                GManager.instance.airplaneRigidbody2D = currentPlaneRb;
                
                // Cập nhật Camera
                CameraManager.instance.UpdateAircraftTarget(currentPlaneRb.transform);
                CameraManager.instance.UpdateCinemachineFollow(currentPlaneRb.transform);
                
                Debug.Log($"Camera đã được gắn lại với máy bay: {defaultPlane.name}");
            }
            else
            {
                Debug.LogError("Không tìm thấy Rigidbody2D trên máy bay!");
            }
        }
        else
        {
            Debug.LogError("CameraManager hoặc defaultPlane bị null!");
        }
        
        // Animation đóng shop
        imageShop.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                imageShop.gameObject.SetActive(false);
                Settings.instance.lastDistanceText.gameObject.SetActive(true);
                CheckPlane.instance.ResetActiveShopPlane();
                Settings.instance.pannelGray.gameObject.SetActive(false);
                // ✅ THÊM: Đảm bảo camera follow đúng sau khi đóng shop
                if (CameraManager.instance != null && GManager.instance != null && GManager.instance.airplaneRigidbody2D != null)
                {
                    CameraManager.instance.virtualCamera.Follow = GManager.instance.airplaneRigidbody2D.transform;
                    CameraManager.instance.virtualCamera.LookAt = GManager.instance.airplaneRigidbody2D.transform;
                    Debug.Log("✅ Camera Follow/LookAt đã được set lại trong OnComplete");
                }
            });
            
        if (Plane.instance != null && Plane.instance.smokeEffect != null)
        {
            Plane.instance.smokeEffect.Stop();
        }
    }


    public bool isBuyPlane1Done = false;
    public bool isBuyPlane2Done = false;
    public bool isBuyPlane3Done = false;
    public bool isBuyPlane4Done = false;
    public bool isBuyPlane5Done = false;
    public bool isBuyPlane6Done = false;
    public bool isBuyPlane7Done = false;
    public bool isBuyPlane8Done = false;
    public bool isBuyPlane9Done = false;
    public bool isBuyPlane10Done = false;
    public bool isBuyPlane11Done = false;
    public bool isBuyPlane12Done = false;
    public bool isBuyPlane13Done = false;
    public bool isBuyPlane14Done = false;
    public bool isRotaryFrontZDone = false;
    public void buyPlane1(){
        StartCoroutine(PlayButtonEffect(0));
        if(!isBuyPlane1Done){
            if(GManager.instance.totalDiamond >= 2500)
            {
                GManager.instance.totalDiamond -= 2500;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
                PlayerPrefs.Save();
                GManager.instance.SaveTotalDiamond();
                planeBuyText[0].gameObject.SetActive(true);
                planeBuyText[0].text = "Play";
                imagePlayPlanes[0].gameObject.SetActive(true);
                isCheckedPlaneIndex = 0;
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 0){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
                planePriceText[0].gameObject.SetActive(false);
                isBuyPlane1Done = true;
            }
            else {
                return;
            }
            
        }
        else {
            if(planeBuyText[0].text == "Play"){
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                return;
            } 
            else{
                planeBuyText[0].text = "Play";
                isCheckedPlaneIndex = 0;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                imagePlayPlanes[0].gameObject.SetActive(true);
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 0){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        SaveTextPlane();
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 0){
            defaultPlane = gameObjectsPlanes[0];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[0];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[0].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[0].transform;
            gameObjectsPlanes[0].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 0){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
            EnsureCameraSetup();
        }
        PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }
    public void buyPlane2(){
        StartCoroutine(PlayButtonEffect(1));
        if(!isBuyPlane2Done){
            if(GManager.instance.totalDiamond >= 2500)
            {
                GManager.instance.totalDiamond -= 2500;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
                PlayerPrefs.Save();
                GManager.instance.SaveTotalDiamond();
                planeBuyText[1].gameObject.SetActive(true);
                planeBuyText[1].text = "Play";
                imagePlayPlanes[1].gameObject.SetActive(true);
                isCheckedPlaneIndex = 1;
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 1){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
                planePriceText[1].gameObject.SetActive(false);
                isBuyPlane2Done = true;
            }
            else {
                return;
            }

        }
        else {
            if(planeBuyText[1].text == "Play"){
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                return;
            }
            else{
                planeBuyText[1].text = "Play";
                isCheckedPlaneIndex = 1;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                imagePlayPlanes[1].gameObject.SetActive(true);
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 1){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        SaveTextPlane();
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 1){
            defaultPlane = gameObjectsPlanes[1];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[1].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[1].transform;
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[1];
            gameObjectsPlanes[1].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 1){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
            EnsureCameraSetup();
        }
        PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }
    public void buyPlane3(){
        StartCoroutine(PlayButtonEffect(2));
        if(!isBuyPlane3Done){
            if(GManager.instance.totalDiamond >= 2500)
            {
                GManager.instance.totalDiamond -= 2500;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
                PlayerPrefs.Save();
                GManager.instance.SaveTotalDiamond();
                planeBuyText[2].gameObject.SetActive(true);
                planeBuyText[2].text = "Play";
                imagePlayPlanes[2].gameObject.SetActive(true);
                isCheckedPlaneIndex = 2;
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 2){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
                planePriceText[2].gameObject.SetActive(false);
                isBuyPlane3Done = true;
            }
            else {
                return;
            }

        }
        else {
            if(planeBuyText[2].text == "Play"){
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                return;
            }
            else{
                planeBuyText[2].text = "Play";
                isCheckedPlaneIndex = 2;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                imagePlayPlanes[2].gameObject.SetActive(true);
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 2){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        SaveTextPlane();
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 2){
            defaultPlane = gameObjectsPlanes[2];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[2];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[2].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[2].transform;
            gameObjectsPlanes[2].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 2){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
            EnsureCameraSetup();
        }
        PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }
    public void buyPlane4(){
        StartCoroutine(PlayButtonEffect(3));
        if(!isBuyPlane4Done){
            if(GManager.instance.totalDiamond >= 2500)
            {
                GManager.instance.totalDiamond -= 2500;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
                PlayerPrefs.Save();
                GManager.instance.SaveTotalDiamond();
                planeBuyText[3].gameObject.SetActive(true);
                planeBuyText[3].text = "Play";
                isCheckedPlaneIndex = 3;
                imagePlayPlanes[3].gameObject.SetActive(true);
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 3){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
                planePriceText[3].gameObject.SetActive(false);
                isBuyPlane4Done = true;
            }
            else {
                return;
            }
        }
        else {
            if(planeBuyText[3].text == "Play"){
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                return;
            }
            else{
                planeBuyText[3].text = "Play";
                isCheckedPlaneIndex = 3;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                imagePlayPlanes[3].gameObject.SetActive(true);
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 3){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        SaveTextPlane();
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 3){
            defaultPlane = gameObjectsPlanes[3];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[3];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[3].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[3].transform;
            gameObjectsPlanes[3].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 3){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
            EnsureCameraSetup();
        }
        PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }
    public void buyPlane5(){
        StartCoroutine(PlayButtonEffect(4));
        if(!isBuyPlane5Done){
            if(GManager.instance.totalDiamond >= 2500)
            {
                GManager.instance.totalDiamond -= 2500;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
                PlayerPrefs.Save();
                GManager.instance.SaveTotalDiamond();
                planeBuyText[4].gameObject.SetActive(true);
                planeBuyText[4].text = "Play";
                imagePlayPlanes[4].gameObject.SetActive(true);
                isCheckedPlaneIndex = 4;
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 4){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
                planePriceText[4].gameObject.SetActive(false);
                isBuyPlane5Done = true;
            }
            else {
                return;
            }
        }
        else {
            if(planeBuyText[4].text == "Play"){
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                return;
            }
            else{
                planeBuyText[4].text = "Play";
                isCheckedPlaneIndex = 4;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                imagePlayPlanes[4].gameObject.SetActive(true);
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 4){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        SaveTextPlane();
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 4){
            defaultPlane = gameObjectsPlanes[4];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[4];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[4].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[4].transform;
            gameObjectsPlanes[4].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 4){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
            EnsureCameraSetup();
        }
        PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }
    public void buyPlane6(){
        StartCoroutine(PlayButtonEffect(5));
        if(!isBuyPlane6Done){
            if(GManager.instance.totalDiamond >= 2500)
            {
                GManager.instance.totalDiamond -= 2500;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
                PlayerPrefs.Save();
                GManager.instance.SaveTotalDiamond();
                planeBuyText[5].gameObject.SetActive(true);
                planeBuyText[5].text = "Play";
                imagePlayPlanes[5].gameObject.SetActive(true);
                isCheckedPlaneIndex = 5;
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 5){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
                planePriceText[5].gameObject.SetActive(false);
                isBuyPlane6Done = true;
            }
            else {
                return;
            }
        }
        else {
            if(planeBuyText[5].text == "Play")  {
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                return;
            }
            else{
                planeBuyText[5].text = "Play";
                isCheckedPlaneIndex = 5;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                imagePlayPlanes[5].gameObject.SetActive(true);
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 5){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        SaveTextPlane();
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 5){
            defaultPlane = gameObjectsPlanes[5];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[5];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[5].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[5].transform;
            gameObjectsPlanes[5].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 5){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
            EnsureCameraSetup();
        }
        PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }
    public void buyPlane7(){
        StartCoroutine(PlayButtonEffect(6));
        if(!isBuyPlane7Done){
            if(GManager.instance.totalDiamond >= 2500)
            {
                GManager.instance.totalDiamond -= 2500;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
                PlayerPrefs.Save();
                GManager.instance.SaveTotalDiamond();
                planeBuyText[6].gameObject.SetActive(true);
                planeBuyText[6].text = "Play";
                imagePlayPlanes[6].gameObject.SetActive(true);
                isCheckedPlaneIndex = 6;
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 6){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
                planePriceText[6].gameObject.SetActive(false);
                isBuyPlane7Done = true;
            }
            else {
                return;
            }
        }
        else {
            if(planeBuyText[6].text == "Play"){
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                return;
            }
            else{
                planeBuyText[6].text = "Play";
                isCheckedPlaneIndex = 6;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                imagePlayPlanes[6].gameObject.SetActive(true);
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 6){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        
        SaveTextPlane();
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 6){
            defaultPlane = gameObjectsPlanes[6];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[6];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[6].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[6].transform;
            isRotaryFrontZDone = true;
            gameObjectsPlanes[6].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 6){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
            EnsureCameraSetup();
        }
        PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }
    public void buyPlane8(){
        StartCoroutine(PlayButtonEffect(7));
        if(!isBuyPlane8Done){
            if(GManager.instance.totalDiamond >= 2500)
            {
                GManager.instance.totalDiamond -= 2500;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
                PlayerPrefs.Save();
                GManager.instance.SaveTotalDiamond();
                planeBuyText[7].gameObject.SetActive(true);
                planeBuyText[7].text = "Play";
                imagePlayPlanes[7].gameObject.SetActive(true);
                isCheckedPlaneIndex = 7;
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 7){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
                planePriceText[7].gameObject.SetActive(false);
                isBuyPlane8Done = true;
            }
            else {
                return;
            }
        }
        else {
            if(planeBuyText[7].text == "Play"){
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                return;
            }
            else{
                planeBuyText[7].text = "Play";
                isCheckedPlaneIndex = 7;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                imagePlayPlanes[7].gameObject.SetActive(true);
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 7){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        SaveTextPlane();
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 7){
            defaultPlane = gameObjectsPlanes[7];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[7];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[7].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[7].transform;
            gameObjectsPlanes[7].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 7){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
            EnsureCameraSetup();
        }
        PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }
    public void buyPlane9(){
        StartCoroutine(PlayButtonEffect(8));
        if(!isBuyPlane9Done){
            if(GManager.instance.totalDiamond >= 2500)
            {
                GManager.instance.totalDiamond -= 2500;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
                PlayerPrefs.Save();
                GManager.instance.SaveTotalDiamond();
                planeBuyText[8].gameObject.SetActive(true);
                planeBuyText[8].text = "Play";
                imagePlayPlanes[8].gameObject.SetActive(true);
                isCheckedPlaneIndex = 8;
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 8){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
                planePriceText[8].gameObject.SetActive(false);
                isBuyPlane9Done = true;
            }
            else {
                return;
            }
        }
        else {
            if(planeBuyText[8].text == "Play"){
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                return;
            }
            else{
                planeBuyText[8].text = "Play";
                isCheckedPlaneIndex = 8;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                imagePlayPlanes[8].gameObject.SetActive(true);
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 8){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        SaveTextPlane();
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 8){
            defaultPlane = gameObjectsPlanes[8];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[8];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[8].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[8].transform;
            gameObjectsPlanes[8].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 8){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
            EnsureCameraSetup();
        }
        PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }
    public void buyPlane10(){
        StartCoroutine(PlayButtonEffect(9));
        if(!isBuyPlane10Done){
            if(GManager.instance.totalDiamond >= 2500)
            {
                GManager.instance.totalDiamond -= 2500;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
                PlayerPrefs.Save();
                GManager.instance.SaveTotalDiamond();
                planeBuyText[9].gameObject.SetActive(true);
                planeBuyText[9].text = "Play";
                imagePlayPlanes[9].gameObject.SetActive(true);
                isCheckedPlaneIndex = 9;
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 9){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
                planePriceText[9].gameObject.SetActive(false);
                isBuyPlane10Done = true;
            }
            else {
                return;
            }
        }
        else {
            if(planeBuyText[9].text == "Play"){
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                return;
            }
            else{
                planeBuyText[9].text = "Play";
                isCheckedPlaneIndex = 9;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                imagePlayPlanes[9].gameObject.SetActive(true);
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 9){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        SaveTextPlane();
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 9){
            defaultPlane = gameObjectsPlanes[9];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[9];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[9].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[9].transform;
            gameObjectsPlanes[9].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 9){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
            EnsureCameraSetup();
        }
        PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }
    public void buyPlane11(){
        StartCoroutine(PlayButtonEffect(10));
        if(!isBuyPlane11Done){
            if(GManager.instance.totalDiamond >= 2500)
            {
                GManager.instance.totalDiamond -= 2500;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
                PlayerPrefs.Save();
                GManager.instance.SaveTotalDiamond();
                planeBuyText[10].gameObject.SetActive(true);
                planeBuyText[10].text = "Play";
                imagePlayPlanes[10].gameObject.SetActive(true);
                isCheckedPlaneIndex = 10;
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 10){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
                planePriceText[10].gameObject.SetActive(false);
                isBuyPlane11Done = true;
            }
            else {
                return;
            }
        }
        else {
            if(planeBuyText[10].text == "Play"){
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                return;
            }
            else{
                planeBuyText[10].text = "Play";
                isCheckedPlaneIndex = 10;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                imagePlayPlanes[10].gameObject.SetActive(true);
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 10){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        
        SaveTextPlane();
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 10){
            defaultPlane = gameObjectsPlanes[10];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[10];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[10].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[10].transform;
            gameObjectsPlanes[10].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 10){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
            EnsureCameraSetup();
        }
        PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }
    public void buyPlane12(){
        StartCoroutine(PlayButtonEffect(11));
        if(!isBuyPlane12Done){
            if(GManager.instance.totalDiamond >= 2500)
            {
                GManager.instance.totalDiamond -= 2500;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
                PlayerPrefs.Save();
                GManager.instance.SaveTotalDiamond();
                planeBuyText[11].gameObject.SetActive(true);
                planeBuyText[11].text = "Play";
                imagePlayPlanes[11].gameObject.SetActive(true);
                isCheckedPlaneIndex = 11;
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 11){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
                planePriceText[11].gameObject.SetActive(false);
                isBuyPlane12Done = true;
            }
            else {
                return;
            }
        }
        else {
            if(planeBuyText[11].text == "Play"){
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                return;
            }
            else{
                planeBuyText[11].text = "Play";
                isCheckedPlaneIndex = 11;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                imagePlayPlanes[11].gameObject.SetActive(true);
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 11){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        SaveTextPlane();
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 11){
            defaultPlane = gameObjectsPlanes[11];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[11];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[11].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[11].transform;
            gameObjectsPlanes[11].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 11){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
            EnsureCameraSetup();
        }
        PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }
    public void buyPlane13(){
        StartCoroutine(PlayButtonEffect(12));
        if(!isBuyPlane13Done){
            if(GManager.instance.totalDiamond >= 2500)
            {
                GManager.instance.totalDiamond -= 2500;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
                PlayerPrefs.Save();
                GManager.instance.SaveTotalDiamond();
                planeBuyText[12].gameObject.SetActive(true);
                planeBuyText[12].text = "Play";
                imagePlayPlanes[12].gameObject.SetActive(true);
                isCheckedPlaneIndex = 12;
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 12){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
                planePriceText[12].gameObject.SetActive(false);
                isBuyPlane13Done = true;
            }
            else {
                return;
            }
        }
        else {
            if(planeBuyText[12].text == "Play"){
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                return;
            }
            else{
                planeBuyText[12].text = "Play";
                isCheckedPlaneIndex = 12;
                AudioManager.instance.PlaySound(AudioManager.instance.unlockPlaneSoundClip);
                imagePlayPlanes[12].gameObject.SetActive(true);
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 12){
                        planeBuyText[i].text = "Select";
                        imagePlayPlanes[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        SaveTextPlane();
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 12){
            defaultPlane = gameObjectsPlanes[12];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[12];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[12].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[12].transform;
            gameObjectsPlanes[12].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 12){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
            EnsureCameraSetup();
        }
        PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }
    public void buyPlane14(){
        StartCoroutine(PlayButtonEffect(13));
        isBuyPlane14Done = true;
        isRotaryFrontZDone = false;
        if (planePriceText != null && planePriceText.Length > 13)
            planePriceText[13].gameObject.SetActive(false);

        planeBuyText[13].text = "Play";
        imagePlayPlanes[13].gameObject.SetActive(true);
        isCheckedPlaneIndex = 13;
        for(int i=0; i<planeBuyText.Length; i++){
            if(i != 13){
                planeBuyText[i].text = "Select";
                imagePlayPlanes[i].gameObject.SetActive(false);
            }
        }
        SaveTextPlane();

        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 13){
            defaultPlane = gameObjectsPlanes[13];
            // GManager.instance.airplaneRigidbody2D.velocity = Vector2.zero;
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[13];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[13].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[13].transform;
            gameObjectsPlanes[13].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 13){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
            EnsureCameraSetup();
            // var rb = GManager.instance.airplaneRigidbody2D;
            // if (rb != null){
            //     rb.constraints = RigidbodyConstraints2D.None;
            //     rb.velocity = Vector2.zero;
            //     Debug.Log("Reset Rigidbody2D constraints and velocity for the new plane."); 
            // }
        }
        
        PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }
    
    public void saveIndexPlane()
    {
        PlayerPrefs.SetInt("SavedPlaneIndex", isCheckedPlaneIndex);
        PlayerPrefs.Save();
    }

    void SetupPlaneByIndex(int planeIndex)
    {
        if (gameObjectsPlanes == null || airplanesRigidbody2D == null) 
        {
            Debug.LogError("GameObjects hoặc Rigidbody2D arrays chưa được setup!");
            return;
        }
        
        if (planeIndex < 0 || planeIndex >= gameObjectsPlanes.Length)
        {
            Debug.LogError($"Plane index {planeIndex} ngoài phạm vi!");
            planeIndex = 0; // Fallback về máy bay đầu tiên
        }
        
        // Activate máy bay được chọn, deactivate các máy bay khác
        for (int i = 0; i < gameObjectsPlanes.Length; i++)
        {
            gameObjectsPlanes[i].SetActive(i == planeIndex);
        }
        
        // QUAN TRỌNG: Set defaultPlane và GManager.airplaneRigidbody2D
        defaultPlane = gameObjectsPlanes[planeIndex];
        
        if (GManager.instance != null)
        {
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[planeIndex];
            Debug.Log($"Setup máy bay: {gameObjectsPlanes[planeIndex].name} - Rigidbody2D: {airplanesRigidbody2D[planeIndex].name}");
            // Plane.instance.smokeEffect.Stop();
            StartCoroutine(SetupCameraDelayed(planeIndex));
        }
        else
        {
            Debug.LogWarning("GManager.instance is null khi setup máy bay!");
        }
    }

    IEnumerator SetupCameraDelayed(int planeIndex)
    {
        // Đợi CameraManager sẵn sàng
        while (CameraManager.instance == null || CameraManager.instance.virtualCamera == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // Setup camera khi đã sẵn sàng
        CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[planeIndex].transform;
        CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[planeIndex].transform;
        Debug.Log($"Camera đã được setup cho: {airplanesRigidbody2D[planeIndex].name}");
    }

    public void saveTextPlaySelect(int indexPlane)
    {
        // Lưu index máy bay hiện tại
        PlayerPrefs.SetInt("isCheckedPlaneIndex", indexPlane);
        
        // KHÔNG CẦN lưu text nữa vì sẽ tính toán lại khi load
        // Chỉ cần lưu trạng thái mua và index được chọn
        
        // Lưu trạng thái mua của từng máy bay
        PlayerPrefs.SetInt("isBuyPlane1Done", isBuyPlane1Done ? 1 : 0);
        PlayerPrefs.SetInt("isBuyPlane2Done", isBuyPlane2Done ? 1 : 0);
        PlayerPrefs.SetInt("isBuyPlane3Done", isBuyPlane3Done ? 1 : 0);
        PlayerPrefs.SetInt("isBuyPlane4Done", isBuyPlane4Done ? 1 : 0);
        PlayerPrefs.SetInt("isBuyPlane5Done", isBuyPlane5Done ? 1 : 0);
        PlayerPrefs.SetInt("isBuyPlane6Done", isBuyPlane6Done ? 1 : 0);
        PlayerPrefs.SetInt("isBuyPlane7Done", isBuyPlane7Done ? 1 : 0);
        PlayerPrefs.SetInt("isBuyPlane8Done", isBuyPlane8Done ? 1 : 0);
        PlayerPrefs.SetInt("isBuyPlane9Done", isBuyPlane9Done ? 1 : 0);
        PlayerPrefs.SetInt("isBuyPlane10Done", isBuyPlane10Done ? 1 : 0);
        PlayerPrefs.SetInt("isBuyPlane11Done", isBuyPlane11Done ? 1 : 0);
        PlayerPrefs.SetInt("isBuyPlane12Done", isBuyPlane12Done ? 1 : 0);
        PlayerPrefs.SetInt("isBuyPlane13Done", isBuyPlane13Done ? 1 : 0);
        PlayerPrefs.SetInt("isBuyPlane14Done", isBuyPlane14Done ? 1 : 0);
        PlayerPrefs.SetInt("isRotaryFrontZDone", isRotaryFrontZDone ? 1 : 0);
        
        PlayerPrefs.Save();
        Debug.Log($"Đã lưu trạng thái: máy bay {indexPlane} được chọn");
    }
    // THÊM: Hàm khôi phục text và trạng thái button
    
    // SỬA: Hàm LoadTextPlaySelect() với logic đúng
    public void LoadTextPlaySelect()
    {
        // Khôi phục trạng thái mua của từng máy bay
        isBuyPlane1Done = PlayerPrefs.GetInt("isBuyPlane1Done", 0) == 1;
        isBuyPlane2Done = PlayerPrefs.GetInt("isBuyPlane2Done", 0) == 1;
        isBuyPlane3Done = PlayerPrefs.GetInt("isBuyPlane3Done", 0) == 1;
        isBuyPlane4Done = PlayerPrefs.GetInt("isBuyPlane4Done", 0) == 1;
        isBuyPlane5Done = PlayerPrefs.GetInt("isBuyPlane5Done", 0) == 1;
        isBuyPlane6Done = PlayerPrefs.GetInt("isBuyPlane6Done", 0) == 1;
        isBuyPlane7Done = PlayerPrefs.GetInt("isBuyPlane7Done", 0) == 1;
        isBuyPlane8Done = PlayerPrefs.GetInt("isBuyPlane8Done", 0) == 1;
        isBuyPlane9Done = PlayerPrefs.GetInt("isBuyPlane9Done", 0) == 1;
        isBuyPlane10Done = PlayerPrefs.GetInt("isBuyPlane10Done", 0) == 1;
        isBuyPlane11Done = PlayerPrefs.GetInt("isBuyPlane11Done", 0) == 1;
        isBuyPlane12Done = PlayerPrefs.GetInt("isBuyPlane12Done", 0) == 1;
        isBuyPlane13Done = PlayerPrefs.GetInt("isBuyPlane13Done", 0) == 1;
        isBuyPlane14Done = PlayerPrefs.GetInt("isBuyPlane14Done", 0) == 1;
        isRotaryFrontZDone = PlayerPrefs.GetInt("isRotaryFrontZDone", 0) == 1;
        
        // Lấy index máy bay hiện tại đang được chọn (có text "Play")
        int currentSelectedIndex = PlayerPrefs.GetInt("isCheckedPlaneIndex", 14);
        
        // Khôi phục text của từng button với logic đúng
        for (int i = 0; i < planeBuyText.Length && i < 15; i++)
        {
            if (planeBuyText[i] != null)
            {
                bool isPlaneBought = GetPlaneBoughtStatus(i);
                string correctText;
                
                if (!isPlaneBought)
                {
                    // Máy bay chưa mua → text = "Select"
                    correctText = "Select";
                }
                else if (i == currentSelectedIndex)
                {
                    // Máy bay đã mua VÀ đang được chọn → text = "Play"  
                    correctText = "Play";
                }
                else
                {
                    // Máy bay đã mua NHƯNG không được chọn → text = "Select"
                    correctText = "Select";
                }
                
                // Set text đúng
                planeBuyText[i].text = correctText;
                
                //Hiển thị/ẩn price text tùy theo trạng thái mua
                if (planePriceText[i] != null)
                {
                    planePriceText[i].gameObject.SetActive(!isPlaneBought);
                }
                
                Debug.Log($"Khôi phục button {i}: text='{correctText}', bought={isPlaneBought}, isSelected={i == currentSelectedIndex}");
            }
        }
        
        Debug.Log($"Đã khôi phục tất cả trạng thái. Máy bay được chọn: {currentSelectedIndex}");
    }


    // THÊM: Coroutine để delay load text
    IEnumerator LoadTextPlaySelectDelayed()
    {
        yield return new WaitForSeconds(0.1f); // Đợi UI sẵn sàng
        LoadTextPlaySelect();
    }

    // ...existing code...

// THÊM: Hàm reset trạng thái game khi chọn máy bay mới
   
   public void SaveTextPlane(){
       // *** LƯU TRẠNG THÁI MUA, KHÔNG LƯU TEXT CONTENT ***
       PlayerPrefs.SetInt("isBuyPlane1Done", isBuyPlane1Done ? 1 : 0);
       PlayerPrefs.SetInt("isBuyPlane2Done", isBuyPlane2Done ? 1 : 0);
       PlayerPrefs.SetInt("isBuyPlane3Done", isBuyPlane3Done ? 1 : 0);
       PlayerPrefs.SetInt("isBuyPlane4Done", isBuyPlane4Done ? 1 : 0);
       PlayerPrefs.SetInt("isBuyPlane5Done", isBuyPlane5Done ? 1 : 0);
       PlayerPrefs.SetInt("isBuyPlane6Done", isBuyPlane6Done ? 1 : 0);
       PlayerPrefs.SetInt("isBuyPlane7Done", isBuyPlane7Done ? 1 : 0);
       PlayerPrefs.SetInt("isBuyPlane8Done", isBuyPlane8Done ? 1 : 0);
       PlayerPrefs.SetInt("isBuyPlane9Done", isBuyPlane9Done ? 1 : 0);
       PlayerPrefs.SetInt("isBuyPlane10Done", isBuyPlane10Done ? 1 : 0);
       PlayerPrefs.SetInt("isBuyPlane11Done", isBuyPlane11Done ? 1 : 0);
       PlayerPrefs.SetInt("isBuyPlane12Done", isBuyPlane12Done ? 1 : 0);
       PlayerPrefs.SetInt("isBuyPlane13Done", isBuyPlane13Done ? 1 : 0);
       PlayerPrefs.SetInt("isBuyPlane14Done", isBuyPlane14Done ? 1 : 0);
       PlayerPrefs.SetInt("isRotaryFrontZDone", isRotaryFrontZDone ? 1 : 0);
       
       PlayerPrefs.SetInt("isCheckedPlaneIndex", isCheckedPlaneIndex);
       PlayerPrefs.Save();
   }

   // THÊM: Method để đảm bảo camera luôn được gắn đúng
    public void EnsureCameraSetup()
    {
        if (defaultPlane == null)
        {
            Debug.LogError("❌ defaultPlane is null!");
            return;
        }
        
        // Tìm Rigidbody2D
        Rigidbody2D rb = defaultPlane.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError($"❌ Không tìm thấy Rigidbody2D trên {defaultPlane.name}!");
            return;
        }
        
        // Cập nhật GManager
        if (GManager.instance != null)
        {
            GManager.instance.airplaneRigidbody2D = rb;
            Debug.Log($"✅ GManager.airplaneRigidbody2D = {rb.name}");
        }
        
        // Cập nhật Camera
        if (CameraManager.instance != null)
        {
            CameraManager.instance.UpdateAircraftTarget(rb.transform);
            CameraManager.instance.UpdateCinemachineFollow(rb.transform);
            CameraManager.instance.virtualCamera.Follow = rb.transform;
            CameraManager.instance.virtualCamera.LookAt = rb.transform;
            Debug.Log($"✅ Camera setup hoàn tất cho {rb.name}");
        }
    }
}