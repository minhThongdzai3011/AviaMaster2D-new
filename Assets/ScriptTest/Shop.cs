using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

public class Shop : MonoBehaviour
{
    public static Shop instance;
    public Image imageShop;
    [Header("Text UI")]
    public TextMeshProUGUI[] planePriceText;
    public TextMeshProUGUI[] planeBuyText;

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

    void Start()
    {
        instance = this;
        Application.targetFrameRate = 60;
        InitializeCarousel();
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
        if (!isAnimating) StartCoroutine(SlideToDirection(-1));
    }

    public void OnClickLeftArrow()
    {
        if (!isAnimating) StartCoroutine(SlideToDirection(1));
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
                               new Vector2(direction * 200f, 0f);
            
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
                              new Vector2(direction * 300f, 0f);
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
        
        // Dừng tất cả animation đang chạy
        DOTween.Kill(this);

        Settings.instance.lastDistanceText.gameObject.SetActive(false);
        imageShop.gameObject.SetActive(true);
        
        // Animation mở shop
        imageShop.transform.localScale = Vector3.zero;
        imageShop.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void CloseShop()
    {
        Debug.Log("Đóng cửa hàng!");
        
        // Dừng tất cả animation
        DOTween.Kill(this);
        
        // Animation đóng shop
        imageShop.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                imageShop.gameObject.SetActive(false);
                Settings.instance.lastDistanceText.gameObject.SetActive(true);
            });
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
    public bool isBuyPlane15Done = false;
    public bool isRotaryFrontZDone = false;

    public void buyPlane1(){
        StartCoroutine(PlayButtonEffect(0));
        if(!isBuyPlane1Done){
            planeBuyText[0].gameObject.SetActive(true);
            planeBuyText[0].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 0){
                    planeBuyText[i].text = "Select";
                    
                }
            }
            planePriceText[0].gameObject.SetActive(false);
            isBuyPlane1Done = true;
        }
        else {
            if(planeBuyText[0].text == "Play") return;
            else{
                planeBuyText[0].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 0){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 0){
            defaultPlane = gameObjectsPlanes[0];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[0].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[0].transform;
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[0];
            gameObjectsPlanes[0].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 0){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
        }
    }
    public void buyPlane2(){
        StartCoroutine(PlayButtonEffect(1));
        if(!isBuyPlane2Done){
            planeBuyText[1].gameObject.SetActive(true);
            planeBuyText[1].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 1){
                    planeBuyText[i].text = "Select";
                }
            }
            planePriceText[1].gameObject.SetActive(false);
            isBuyPlane2Done = true;
        }
        else {
            if(planeBuyText[1].text == "Play") return;
            else{
                planeBuyText[1].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 1){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
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
        }
    }
    public void buyPlane3(){
        StartCoroutine(PlayButtonEffect(2));
        if(!isBuyPlane3Done){
            planeBuyText[2].gameObject.SetActive(true);
            planeBuyText[2].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 2){
                    planeBuyText[i].text = "Select";
                }
            }
            planePriceText[2].gameObject.SetActive(false);
            isBuyPlane3Done = true;
        }
        else {
            if(planeBuyText[2].text == "Play") return;
            else{
                planeBuyText[2].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 2){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 2){
            defaultPlane = gameObjectsPlanes[2];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[2].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[2].transform;
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[2];
            gameObjectsPlanes[2].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 2){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
        }
    }
    public void buyPlane4(){
        StartCoroutine(PlayButtonEffect(3));
        if(!isBuyPlane4Done){
            planeBuyText[3].gameObject.SetActive(true);
            planeBuyText[3].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 3){
                    planeBuyText[i].text = "Select";
                }
            }
            planePriceText[3].gameObject.SetActive(false);
            isBuyPlane4Done = true;
        }
        else {
            if(planeBuyText[3].text == "Play") return;
            else{
                planeBuyText[3].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 3){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
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
        }
    }
    public void buyPlane5(){
        StartCoroutine(PlayButtonEffect(4));
        if(!isBuyPlane5Done){
            planeBuyText[4].gameObject.SetActive(true);
            planeBuyText[4].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 4){
                    planeBuyText[i].text = "Select";
                }
            }
            planePriceText[4].gameObject.SetActive(false);
            isBuyPlane5Done = true;
        }
        else {
            if(planeBuyText[4].text == "Play") return;
            else{
                planeBuyText[4].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 4){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
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
        }
    }
    public void buyPlane6(){
        StartCoroutine(PlayButtonEffect(5));
        if(!isBuyPlane6Done){
            planeBuyText[5].gameObject.SetActive(true);
            planeBuyText[5].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 5){
                    planeBuyText[i].text = "Select";
                }
            }
            planePriceText[5].gameObject.SetActive(false);
            isBuyPlane6Done = true;
        }
        else {
            if(planeBuyText[5].text == "Play") return;
            else{
                planeBuyText[5].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 5){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
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
        }
    }
    public void buyPlane7(){
        StartCoroutine(PlayButtonEffect(6));
        if(!isBuyPlane7Done){
            planeBuyText[6].gameObject.SetActive(true);
            planeBuyText[6].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 6){
                    planeBuyText[i].text = "Select";
                }
            }
            planePriceText[6].gameObject.SetActive(false);
            isBuyPlane7Done = true;
        }
        else {
            if(planeBuyText[6].text == "Play") return;
            else{
                planeBuyText[6].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 6){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 6){
            defaultPlane = gameObjectsPlanes[6];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[6];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[6].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[6].transform;
            gameObjectsPlanes[6].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 6){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
        }
    }
    public void buyPlane8(){
        StartCoroutine(PlayButtonEffect(7));
        if(!isBuyPlane8Done){
            planeBuyText[7].gameObject.SetActive(true);
            planeBuyText[7].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 7){
                    planeBuyText[i].text = "Select";
                }
            }
            planePriceText[7].gameObject.SetActive(false);
            isBuyPlane8Done = true;
        }
        else {
            if(planeBuyText[7].text == "Play") return;
            else{
                planeBuyText[7].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 7){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 7){
            defaultPlane = gameObjectsPlanes[7];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[7];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[7].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[7].transform;
            isRotaryFrontZDone = true;
            gameObjectsPlanes[7].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 7){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
        }
    }
    public void buyPlane9(){
        StartCoroutine(PlayButtonEffect(8));
        if(!isBuyPlane9Done){
            planeBuyText[8].gameObject.SetActive(true);
            planeBuyText[8].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 8){
                    planeBuyText[i].text = "Select";
                }
            }
            planePriceText[8].gameObject.SetActive(false);
            isBuyPlane9Done = true;
        }
        else {
            if(planeBuyText[8].text == "Play") return;
            else{
                planeBuyText[8].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 8){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
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
        }
    }
    public void buyPlane10(){
        StartCoroutine(PlayButtonEffect(9));
        if(!isBuyPlane10Done){
            planeBuyText[9].gameObject.SetActive(true);
            planeBuyText[9].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 9){
                    planeBuyText[i].text = "Select";
                }
            }
            planePriceText[9].gameObject.SetActive(false);
            isBuyPlane10Done = true;
        }
        else {
            if(planeBuyText[9].text == "Play") return;
            else{
                planeBuyText[9].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 9){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
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
        }
    }
    public void buyPlane11(){
        StartCoroutine(PlayButtonEffect(10));
        if(!isBuyPlane11Done){
            planeBuyText[10].gameObject.SetActive(true);
            planeBuyText[10].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 10){
                    planeBuyText[i].text = "Select";
                }
            }
            planePriceText[10].gameObject.SetActive(false);
            isBuyPlane11Done = true;
        }
        else {
            if(planeBuyText[10].text == "Play") return;
            else{
                planeBuyText[10].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 10){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
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
        }
    }
    public void buyPlane12(){
        StartCoroutine(PlayButtonEffect(11));
        if(!isBuyPlane12Done){
            planeBuyText[11].gameObject.SetActive(true);
            planeBuyText[11].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 11){
                    planeBuyText[i].text = "Select";
                }
            }
            planePriceText[11].gameObject.SetActive(false);
            isBuyPlane12Done = true;
        }
        else {
            if(planeBuyText[11].text == "Play") return;
            else{
                planeBuyText[11].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 11){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
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
        }
    }
    public void buyPlane13(){
        StartCoroutine(PlayButtonEffect(12));
        if(!isBuyPlane13Done){
            planeBuyText[12].gameObject.SetActive(true);
            planeBuyText[12].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 12){
                    planeBuyText[i].text = "Select";
                }
            }
            planePriceText[12].gameObject.SetActive(false);
            isBuyPlane13Done = true;
        }
        else {
            if(planeBuyText[12].text == "Play") return;
            else{
                planeBuyText[12].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 12){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
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
        }
    }
    public void buyPlane14(){
        StartCoroutine(PlayButtonEffect(13));
        if(!isBuyPlane14Done){
            planeBuyText[13].gameObject.SetActive(true);
            planeBuyText[13].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 13){
                    planeBuyText[i].text = "Select";
                }
            }
            planePriceText[13].gameObject.SetActive(false);
            isBuyPlane14Done = true;
        }
        else {
            if(planeBuyText[13].text == "Play") return;
            else{
                planeBuyText[13].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 13){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 13){
            defaultPlane = gameObjectsPlanes[13];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[13];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[13].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[13].transform;
            gameObjectsPlanes[13].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 13){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
        }
    }
    public void buyPlane15(){
        StartCoroutine(PlayButtonEffect(14));
        if(!isBuyPlane15Done){
            planeBuyText[14].gameObject.SetActive(true);
            planeBuyText[14].text = "Play";
            for(int i=0; i<planeBuyText.Length; i++){
                if(i != 14){
                    planeBuyText[i].text = "Select";
                }
            }
            planePriceText[14].gameObject.SetActive(false);
            isBuyPlane15Done = true;
        }
        else {
            if(planeBuyText[14].text == "Play") return;
            else{
                planeBuyText[14].text = "Play";
                for(int i=0; i<planeBuyText.Length; i++){
                    if(i != 14){
                        planeBuyText[i].text = "Select";
                    }
                }
            }
        }
        
        // Thay đổi defaultPlane
        if(gameObjectsPlanes != null && gameObjectsPlanes.Length > 14){
            defaultPlane = gameObjectsPlanes[14];
            GManager.instance.airplaneRigidbody2D = airplanesRigidbody2D[14];
            CameraManager.instance.virtualCamera.Follow = airplanesRigidbody2D[14].transform;
            CameraManager.instance.virtualCamera.LookAt = airplanesRigidbody2D[14].transform;
            gameObjectsPlanes[14].SetActive(true);
            for (int i=0; i<gameObjectsPlanes.Length; i++){
                if(i != 14){
                    gameObjectsPlanes[i].SetActive(false);
                }
            }
        }
    }

    public void savePlaneSelection(){
        
        PlayerPrefs.Save();
    }
    


}