using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Leaderboard;


public class Plane : MonoBehaviour
{
    public static Plane instance;

    public Material blackMaterial;
    public Material GoldMaterial;
    public ParticleSystem explosionEffect;


    public ParticleSystem smokeEffect;
    public TrailRenderer trailEffect;
    public TrailRenderer trailRenderer;
    public TrailRenderer trailRendererPerfect;

    public bool isGrounded = false;

    public int moneyDistance = 0;
    public int moneyCollect = 0;
    public int moneyTotal1 = 0;
    public bool isUseCoinDiamond = false;

    [Header ("Kiểm tra đáp cánh an toàn airport")]
    public bool isBoom = false;
    public bool isAirPortCity = false;
    public bool isAirPortBeach = false;
    public bool isAirPortDesert = false;
    public bool isAirPortField = false;
    public bool isAirPortIce = false;
    public bool isAirPortLava = false;
    
    // Track airport hiện tại player đang ở trong
    private string currentAirportTag = "";

    // Start is called before the first frame update

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        smokeEffect.Stop();
        explosionEffect.Stop();
    }
    public bool isStopSmokeEffect = true;
    public bool isStopExplosionEffect = true;

    // Update is called once per frame
    void Update()
    {
        if(isStopSmokeEffect){
            smokeEffect.Stop();
        }
        if(isStopExplosionEffect){
            explosionEffect.Stop();
        }


    }



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && !isGrounded)
        {
            isStopExplosionEffect = false;
            isGrounded = true;

            Debug.Log("Máy bay đã chạm đất.");
            
            // *** KIỂM TRA HẠ CÁNH AN TOÀN TẠI AIRPORT ***
            float initialAngleZ = GetCurrentRotationZ(GManager.instance.airplaneRigidbody2D);
            bool isSafeLanding = (initialAngleZ >= -15f && initialAngleZ <= 15f);
            
            if (isSafeLanding && !string.IsNullOrEmpty(currentAirportTag))
            {
                Debug.Log($"[SAFE LANDING] Landed safely at {currentAirportTag}!");
                
                // Set airport flag tương ứng
                switch (currentAirportTag)
                {
                    case "MapStartCity":
                        isAirPortCity = true;
                        GManager.instance.isSuperBonus = true;   
                        GManager.instance.isBonus = false; 
                        PlayerPrefs.SetInt("LastSafeLandingAirport", 0);
                        Debug.Log("[SAFE LANDING] Next game will start at City map");
                        break;
                    case "MapStartBeach":
                        isAirPortBeach = true;
                        GManager.instance.isSuperBonus = true; 
                        GManager.instance.isBonus = false; 
                        PlayerPrefs.SetInt("LastSafeLandingAirport", 1);
                        Debug.Log("[SAFE LANDING] Next game will start at Beach map");
                        break;
                    case "MapStartDesert":
                        isAirPortDesert = true;
                        GManager.instance.isSuperBonus = true;
                        GManager.instance.isBonus = false;  
                        PlayerPrefs.SetInt("LastSafeLandingAirport", 2);
                        Debug.Log("[SAFE LANDING] Next game will start at Desert map");
                        break;
                    case "MapStartField":
                        isAirPortField = true;
                        GManager.instance.isSuperBonus = true; 
                        GManager.instance.isBonus = false; 
                        PlayerPrefs.SetInt("LastSafeLandingAirport", 3);
                        Debug.Log("[SAFE LANDING] Next game will start at Field map");
                        break;
                    case "MapStartIce":
                        isAirPortIce = true;
                        GManager.instance.isSuperBonus = true; 
                        GManager.instance.isBonus = false; 
                        PlayerPrefs.SetInt("LastSafeLandingAirport", 4);
                        Debug.Log("[SAFE LANDING] Next game will start at Ice map");
                        break;
                    case "MapStartLava":
                        isAirPortLava = true;
                        GManager.instance.isSuperBonus = true; 
                        GManager.instance.isBonus = false;
                        PlayerPrefs.SetInt("LastSafeLandingAirport", 5);
                        Debug.Log("[SAFE LANDING] Next game will start at Lava map");
                        break;
                }
                PlayerPrefs.Save();
            }
            else if (!isSafeLanding)
            {
                Debug.Log("[UNSAFE LANDING] Angle too steep, not counted as safe landing");
                isBoom = true;
            }
            
            if (GManager.instance != null)
            {
                GManager.instance.OnPlaneGroundCollision();
                GManager.instance.isControllable = false;
                GManager.instance.isBoosterActive = false;
            }

            // Dừng tất cả cánh quạt
            foreach (var propeller in RotaryFront.instances)
            {
                if (propeller != null)
                    propeller.StopWithDeceleration(3.0f);
            }
            foreach (var propeller in RotaryFrontZ.instances)
            {
                if (propeller != null)
                    propeller.StopWithDeceleration(3.0f);
            }
            
            smokeEffect.Stop();
            
            StartCoroutine(UpMass());
        }
    }
        
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin") || other.CompareTag("Respawn"))
        {
            // AnimCoin anim = other.GetComponent<AnimCoin>();
            AudioManager.instance.PlaySound(AudioManager.instance.collectMoneySoundClip);
            if (PositionX.instance.isMaxPower)
            {
                // tăng tiền ngay (hoặc bạn có thể tăng tiền khi animation hoàn tất trong AnimCoin)
                moneyCollect += 20;

                // anim.collected = true;
                // anim.Collect();
                Destroy(other.gameObject);
            }
            else
            {
                moneyCollect += 10;
                Destroy(other.gameObject);
            }
        }
        if (other.CompareTag("Diamond"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.collectDiamondSoundClip);
            GManager.instance.totalDiamond += 100;
            GManager.instance.totalDiamondText.text = GManager.instance.totalDiamond.ToString("F0");
            Debug.Log("Diamond collected +100" + GManager.instance.totalDiamond);
            PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
            PlayerPrefs.Save();
            GManager.instance.SaveTotalDiamond();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Boom"))
        {
            if (PositionX.instance.isMaxPower)
            {
                AudioManager.instance.PlaySound(AudioManager.instance.obstacleCollisionSoundClip);
                EffectExplosionBonus ef = other.GetComponent<EffectExplosionBonus>();
                ef.ExplosionEffect();
                ef.ExplosionEffect1();

                //EffectExplosionBonus.instance.ExplosionEffect();
                // EffectExplosionBonus.instance.ExplosionEffect1();
                // Destroy(other.gameObject);
                other.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                AudioManager.instance.PlaySound(AudioManager.instance.explosionSoundClip);
            
                if (CameraManager.instance != null)
                {
                    CameraManager.instance.FreezeCamera();
                }
                GManager.instance.stopDisplayDistance = true;
                string tempDistant = GManager.instance.distanceTraveled.ToString("F0");
                GManager.instance.distanceText.text = tempDistant + " ft";
                GManager.instance.upAircraftImage.gameObject.SetActive(false);
                GManager.instance.downFuelImage.gameObject.SetActive(false);
                MakePlaneBlackAndExplode();
                TextIntro.instance.GetRandomTextWinMessage(0); 
                GManager.instance.newMapText.text = "Space isn’t empty enough for this pilot!";
                GManager.instance.airplaneRigidbody2D.velocity = Vector2.zero;
                Settings.instance.isAltitudeImageActive = false;
                Settings.instance. altitudeImage.gameObject.SetActive(false);
                // StartCoroutine(DelayOneSecond(2f));
            }
            
        }
        if (other.CompareTag("bird"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.crashBonusSoundClip);
           RandomPrizeBird();
           Destroy(other.gameObject);
        }
        if (other.CompareTag("Bonus4"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.obstacleCollisionSoundClip);
            Destroy(other.gameObject);
            StartCoroutine(FadeFogImage());
            
        }
        if (other.CompareTag("Bonus2"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.obstacleCollisionSoundClip);
            EffectExplosionBonus ef = other.GetComponent<EffectExplosionBonus>();
            ef.ExplosionEffect();
            ef.ExplosionEffect1();

            //EffectExplosionBonus.instance.ExplosionEffect();
           // EffectExplosionBonus.instance.ExplosionEffect1();
            // Destroy(other.gameObject);
            other.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            if(TrailRendererRight.instance != null ) TrailRendererRight.instance.StopTrail();
            if(TrailRendererLeft.instance != null ) TrailRendererLeft.instance.StopTrail();
            Debug.Log("Bonus2 collected - Fuel to 0" + PositionX.instance.isMaxPower);
            if (!PositionX.instance.isMaxPower)
            {
                GManager.instance.durationFuel = 0f;
                Debug.Log("Bonus2 collected - Ending game soon");
                GManager.instance.newMapText.text = "Fastest balloon pop ever… and you lost!";
                StartCoroutine(FadeInText(1f));
            }
        }
        if (other.CompareTag("MaxPower"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.obstacleCollisionSoundClip);
            Destroy(other.gameObject);
            
        }
        if (other.CompareTag("bule1"))
        {
            Destroy(other.gameObject);
            AudioManager.instance.PlaySound(AudioManager.instance.crashBonusSoundClip);
            GManager.instance.totalDiamond += 200;
            GManager.instance.totalDiamondText.text = GManager.instance.totalDiamond.ToString("F0");
            Debug.Log("Diamond collected +200" + GManager.instance.totalDiamond);
            PlayerPrefs.SetInt("TotalDiamond", GManager.instance.totalDiamond);
            PlayerPrefs.Save();
            GManager.instance.SaveTotalDiamond();
            GManager.instance.newMapText.text = "You grabbed a 200";
            isUseCoinDiamond = true;
            Settings.instance.imageDiamondCoinText.gameObject.SetActive(true);
            Settings.instance.imageDiamondCoinText.sprite = Settings.instance.spriteDiamond;
            StartCoroutine(FadeInText(1f));
        }
        if (other.CompareTag("MapStartCity") && !isBoom)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapCityUnlocked = true;
            PlayerPrefs.SetInt("IsMapCityUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapCity;
            PlayerPrefs.Save();
            
            // Track airport hiện tại
            currentAirportTag = "MapStartCity";
            Debug.Log("[AIRPORT] Entered City Airport zone");
        }
        
        if (other.CompareTag("MapStartBeach") && !isBoom)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapBeachUnlocked = true;
            PlayerPrefs.SetInt("IsMapBeachUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapBeach;
            PlayerPrefs.Save();
            
            // Track airport hiện tại
            currentAirportTag = "MapStartBeach";
            Debug.Log("[AIRPORT] Entered Beach Airport zone");
        }
        
        if (other.CompareTag("MapStartDesert") && !isBoom)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapDesertUnlocked = true;
            PlayerPrefs.SetInt("IsMapDesertUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapDesert;
            PlayerPrefs.Save();
            
            // Track airport hiện tại
            currentAirportTag = "MapStartDesert";
            Debug.Log("[AIRPORT] Entered Desert Airport zone");
        }
        
        if (other.CompareTag("MapStartField") && !isBoom)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapFieldUnlocked = true;
            PlayerPrefs.SetInt("IsMapFieldUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapField;
            PlayerPrefs.Save();
            
            // Track airport hiện tại
            currentAirportTag = "MapStartField";
            Debug.Log("[AIRPORT] Entered Field Airport zone");
        }
        
        if (other.CompareTag("MapStartIce") && !isBoom)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapIceUnlocked = true;
            PlayerPrefs.SetInt("IsMapIceUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapIce;
            PlayerPrefs.Save();
            
            // Track airport hiện tại
            currentAirportTag = "MapStartIce";
            Debug.Log("[AIRPORT] Entered Ice Airport zone");
        }
        
        if (other.CompareTag("MapStartLava") && !isBoom)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapLavaUnlocked = true;
            PlayerPrefs.SetInt("IsMapLavaUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapLava;
            PlayerPrefs.Save();
            
            // Track airport hiện tại
            currentAirportTag = "MapStartLava";
            Debug.Log("[AIRPORT] Entered Lava Airport zone");
        }
        if (other.CompareTag("NofitiCity"))
        {
            
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapCity;
            if (GManager.instance.distanceTraveled >= 100f)
            {
                Settings.instance.NotificationNewMapText.text = "Beautiful sky above… Welcome to City Map!";
                Settings.instance.NotificationNewMap();
            }
            else
            {
                Settings.instance.NotificationNewMapText.text = "It’s so noisy, is this Map City?";
                Settings.instance.NotificationNewMap();
            }
        }
        if (other.CompareTag("NofitiBeach"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapBeach;

            if (GManager.instance.distanceTraveled >= 100f)
            {
                Settings.instance.NotificationNewMapText.text = "Under this vast sky, everything feels possible.";
                Settings.instance.NotificationNewMap();
            }
            else
            {
                Settings.instance.NotificationNewMapText.text = "The sound of waves is so relaxing!";
                Settings.instance.NotificationNewMap();
            }
        }
        if (other.CompareTag("NofitiDesert"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapDesert;
            if (GManager.instance.distanceTraveled >= 100f)
            {
                Settings.instance.NotificationNewMapText.text = "The desert stretches endlessly under the blazing sun.";
                Settings.instance.NotificationNewMap();
            }
            else
            {
                Settings.instance.NotificationNewMapText.text = "The heat is intense, but the view is worth it!";
                Settings.instance.NotificationNewMap();
            }
        }
        if (other.CompareTag("NofitiField"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapField;
            if (GManager.instance.distanceTraveled >= 100f)
            {
                Settings.instance.NotificationNewMapText.text = "Flying over endless fields, freedom feels real.";
                Settings.instance.NotificationNewMap();
            }
            else
            {
                Settings.instance.NotificationNewMapText.text = "This Field map is beautiful, the air is fresh!";
                Settings.instance.NotificationNewMap();
            }
        }
        if (other.CompareTag("NofitiIce"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapIce;
            if (GManager.instance.distanceTraveled >= 100f)
            {
                Settings.instance.NotificationNewMapText.text = "The icy expanse sparkles under the sun, a frozen wonderland.";
                Settings.instance.NotificationNewMap();
            }
            else
            {
                Settings.instance.NotificationNewMapText.text = "Brrr... it's chilly up here, but the view is stunning!";
                Settings.instance.NotificationNewMap();
            }
        }
        if (other.CompareTag("NofitiLava"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapLava;
            if (GManager.instance.distanceTraveled >= 100f)
            {
                Settings.instance.NotificationNewMapText.text = "The fiery landscape below is both terrifying and mesmerizing.";
                Settings.instance.NotificationNewMap();
            }
            else
            {
                Settings.instance.NotificationNewMapText.text = "Hey, lava ahead! Stay safe up here!";
                Settings.instance.NotificationNewMap();
            }
        }
        
    
        
    }
    
    
    
    IEnumerator UpMass()
    {
        Rigidbody2D airplaneRb = GManager.instance.airplaneRigidbody2D;
        
        if (airplaneRb == null)
        {
            Debug.LogWarning("Airplane Rigidbody2D not found!");
            yield break;
        }
        GManager.instance.downFuelImage.gameObject.SetActive(false);
        GManager.instance.upAircraftImage.gameObject.SetActive(false);
        
        float initialAngleZ = GetCurrentRotationZ(airplaneRb);
        float initialAngleX = airplaneRb.transform.eulerAngles.x;
        if (initialAngleZ < -15f || initialAngleZ > 15f){
            AudioManager.instance.PlaySound(AudioManager.instance.explosionSoundClip);
            AudioManager.instance.StopPlayerSound();
            MakePlaneBlackAndExplode();
            Debug.Log("Airplane crashed due to excessive tilt angle: " + initialAngleZ);
            TextIntro.instance.GetRandomTextWinMessage(0); 
            isBoom = true;
            GManager.instance.isBonus = false;   
        }
        else{
            AudioManager.instance.PlaySound(AudioManager.instance.perfectLandingSoundClip);
            AudioManager.instance.StopPlayerSound();
            TextIntro.instance.GetRandomTextWinMessage(1); 
            Settings.instance.ImageErrorAngleZ.gameObject.SetActive(false);


            if (initialAngleX > 180f) initialAngleX -= 360f; 
            
            Vector2 initialVelocity = airplaneRb.velocity;
            
            float duration = 3f;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                
                float t = elapsedTime / duration;
                float smoothT = 1f - t; 
                
                float targetAngleZ = initialAngleZ * smoothT;
                float targetAngleX = initialAngleX * smoothT;
                Vector3 currentRotation = airplaneRb.transform.eulerAngles;
                currentRotation.z = targetAngleZ;
                currentRotation.x = targetAngleX;
                airplaneRb.transform.eulerAngles = currentRotation;
                
                Vector2 targetVelocity = initialVelocity * smoothT;
                airplaneRb.velocity = targetVelocity;
                
                yield return null;
            }
            
            Vector3 finalRotation = airplaneRb.transform.eulerAngles;
            finalRotation.z = 0f;
            finalRotation.x = 0f;
            airplaneRb.transform.eulerAngles = finalRotation;
            airplaneRb.velocity = Vector2.zero;
            GManager.instance.isBonus = true;   
            Debug.Log("Airplane stopped sliding");
            StartCoroutine(OpenImageWIn());
            }
        }

    public bool resetRotationOnExplode = true;
    public float explodeRotationResetTime = 0.4f;
    private bool hasExploded = false;

    public void MakePlaneBlackAndExplode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Debug.Log("Making plane black and explode");

        var planeRenderer = GetComponent<Renderer>();
        if (planeRenderer != null && blackMaterial != null)
            planeRenderer.material = blackMaterial;

        if (resetRotationOnExplode && GManager.instance != null && GManager.instance.airplaneRigidbody2D != null)
        {
            var rb = GManager.instance.airplaneRigidbody2D;
            rb.angularVelocity = 0f;

            Vector3 targetEuler = rb.transform.eulerAngles;
            targetEuler.z = 0f;

            rb.transform.DORotate(targetEuler, explodeRotationResetTime)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                });
        }

        EffectRotaryFront.ExplodeAll();
        if (DestroyWheels.instance != null) DestroyWheels.instance.Explode();
        if (ExplosionScale.instance != null) ExplosionScale.instance.Explosion();
        if (EffectAirplane.instance != null) EffectAirplane.instance.MakePlaneBlack();

        if (explosionEffect != null) explosionEffect.Play();

        StartCoroutine(DelayTwoSeconds(2f));
    }
    float GetCurrentRotationZ(Rigidbody2D rb)
    {
        float z = rb.transform.eulerAngles.z;
        if (z > 180f) z -= 360f;
        return z;
    }
    private bool isAddMoneyDone = false;
    public int hightScore;
    IEnumerator OpenImageWIn()
    {
        GManager.instance.LoadMoneyUpgrade();
        float  finalDistance = GManager.instance.distanceTraveled;
        Debug.Log("Opening Win Image... 1" + finalDistance);
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.PostScore((int)finalDistance);
        }
        yield return new WaitForSeconds(2f);
        if (!isAddMoneyDone)
        {
            SaveCurrentAirplane();
            AudioManager.instance.PlaySound(AudioManager.instance.victorySoundClip);
            GManager.instance.coinEffect.Stop();
            isAddMoneyDone = true;
            
            moneyDistance = (int)(GManager.instance.distanceTraveled / 6.34f);

            Debug.Log("Money from Distance: " + moneyDistance + " | Current Money: " + moneyCollect + " | Total Money: " + moneyTotal1);
            
            Debug.Log("Updated Total Money: " + GManager.instance.totalMoney);
            GManager.instance.totalMoneyText.text = "" + GManager.instance.totalMoney;
            GManager.instance.moneyText.text = "" + moneyCollect;

            hightScore = PlayerPrefs.GetInt("HighScore", 0);
            Debug.Log("Opening Win Image... 2 " + hightScore);
            if (finalDistance > hightScore)
            {
                PlayerPrefs.SetInt("HighScore", (int)finalDistance);
                Debug.Log("Opening Win Image... 3" + (int)finalDistance);
                PlayerPrefs.Save();
            }
            
            Settings.instance.OpenWinImage();
        }
    }
    
    void SaveCurrentAirplane()
    {
        if (GManager.instance != null && GManager.instance.airplaneRigidbody2D != null)
        {
            string currentAirplaneName = GManager.instance.airplaneRigidbody2D.gameObject.name;
            PlayerPrefs.SetString("LastUsedAirplane", currentAirplaneName);
            PlayerPrefs.Save();
            Debug.Log($"Lưu máy bay hiện tại: {currentAirplaneName}");
        }
    }

    IEnumerator DelaytoEndGame()
    {
        yield return new WaitForSeconds(2f);
        if (GManager.instance != null)
        {
            GManager.instance.isControllable = false;
            GManager.instance.isBoosterActive = false;
        }
        // Dừng tất cả cánh quạt
        foreach (var propeller in RotaryFront.instances)
        {
            if (propeller != null)
                propeller.StopWithDeceleration(3.0f);
        }
        foreach (var propeller in RotaryFrontZ.instances)
        {
            if (propeller != null)
                propeller.StopWithDeceleration(3.0f);
        }
        
        smokeEffect.Stop();
        StartCoroutine(OpenImageWIn());
    }
    public bool isFadeDone = false;
    IEnumerator FadeInText(float duration)
    {
        // Fade Text
        Color c = GManager.instance.newMapText.color;
        c.a = 0;
        GManager.instance.newMapText.color = c;

        // ✅ Fade Image cùng lúc (nếu đang active)
        Color imgColor = Color.white;
        bool hasImage = false;
        if (Settings.instance.imageDiamondCoinText.gameObject.activeSelf)
        {
            hasImage = true;
            imgColor = Settings.instance.imageDiamondCoinText.color;
            imgColor.a = 0;
            Settings.instance.imageDiamondCoinText.color = imgColor;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            
            // Fade text
            c.a = alpha;
            GManager.instance.newMapText.color = c;
            
            // Fade image
            if (hasImage)
            {
                imgColor.a = alpha;
                Settings.instance.imageDiamondCoinText.color = imgColor;
            }
            
            yield return null;
        }
        isFadeDone = true;
        StartCoroutine(FadeOutText(2f));
    }
    

    IEnumerator FadeOutText(float duration)
    {
        if (!isFadeDone) yield break;
        
        Color c = GManager.instance.newMapText.color;
        Color imgColor = Settings.instance.imageDiamondCoinText.color;
        bool hasImage = Settings.instance.imageDiamondCoinText.gameObject.activeSelf;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / duration);
            
            // Fade text
            c.a = alpha;
            GManager.instance.newMapText.color = c;
            
            // ✅ Fade image cùng lúc
            if (hasImage)
            {
                imgColor.a = alpha;
                Settings.instance.imageDiamondCoinText.color = imgColor;
            }
            
            yield return null;
        }
        
        // Đặt alpha = 0 cuối cùng
        c.a = 0;
        GManager.instance.newMapText.color = c;
        
        // ✅ Ẩn image và reset sprite
        if (isUseCoinDiamond)
        {
            imgColor.a = 0;
            Settings.instance.imageDiamondCoinText.color = imgColor;
            Settings.instance.imageDiamondCoinText.sprite = Settings.instance.spriteUIMask;
            Settings.instance.imageDiamondCoinText.gameObject.SetActive(false);
            isUseCoinDiamond = false;
        }
    }
    private bool isImageFadeDone = false;
    // IEnumerator FadeInImage(Image img, float duration)
    // {
    //     if (img == null) yield break;

    //     Color c = img.color;
    //     c.a = 0f;
    //     img.color = c;

    //     float elapsed = 0f;
    //     while (elapsed < duration)
    //     {
    //         elapsed += Time.deltaTime;
    //         c.a = Mathf.Clamp01(elapsed / duration);
    //         img.color = c;
    //         yield return null;
    //     }

    //     isImageFadeDone = true;

    //     // Nếu muốn fade out giống Text
    //     StartCoroutine(FadeOutImage(img, 2f));
    // }
    // IEnumerator FadeOutImage(Image img, float duration)
    // {
    //     if (!isImageFadeDone) yield break;
    //     if (img == null) yield break;

    //     Color c = img.color;

    //     float elapsed = 0f;
    //     while (elapsed < duration)
    //     {
    //         elapsed += Time.deltaTime;
    //         c.a = 1f - Mathf.Clamp01(elapsed / duration);
    //         img.color = c;
    //         yield return null;
    //     }
    //     c.a = 0f;
    //     img.color = c;
    // }


    IEnumerator FadeFogImage()
    {
        float elapsed = 0f;
        Settings.instance.iamgeBlackScreen.gameObject.SetActive(true);
        Color startColor = Settings.instance.iamgeBlackScreen.color;
        startColor.a = 0f;
        Settings.instance.iamgeBlackScreen.color = startColor;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            Color c = Settings.instance.iamgeBlackScreen.color;
            c.a = Mathf.Clamp01(elapsed / 1f);
            Settings.instance.iamgeBlackScreen.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(6f);

        elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            Color c = Settings.instance.iamgeBlackScreen.color;
            c.a = 1f - Mathf.Clamp01(elapsed / 1f); 
            Settings.instance.iamgeBlackScreen.color = c;
            yield return null;
        }

        Settings.instance.iamgeBlackScreen.gameObject.SetActive(false);
    }

    public bool isAddFuel = false;
    public void RandomPrizeBird(){
        int[] coinPrize = {1000, 2000, 5000};
        int count = Random.Range(0, 3); 
        if(count == 0){
            int randomIndex = Random.Range(0, coinPrize.Length);
            int prize = coinPrize[randomIndex];
            Debug.Log("Money collected +" + prize + " | Total Money: " + GManager.instance.totalMoney);
            GManager.instance.totalMoney += prize;
            Debug.Log("Money collected +" + prize + " | Total Money: " + GManager.instance.totalMoney);
            GManager.instance.totalMoneyText.text = GManager.instance.totalMoney.ToString("F0");
            Debug.Log("Money collected +" + prize + " | Total Money: " + GManager.instance.totalMoney);
            PlayerPrefs.SetFloat("TotalMoney", GManager.instance.totalMoney);
            PlayerPrefs.Save();
            GManager.instance.SaveTotalMoney();
            GManager.instance.newMapText.text = $"You grabbed a {prize}";
            isUseCoinDiamond = true;
            Settings.instance.imageDiamondCoinText.sprite = Settings.instance.spriteCoin;
            Settings.instance.imageDiamondCoinText.gameObject.SetActive(true);
            StartCoroutine(FadeInText(1f));
            
        }
        else if(count == 1){
            float addedFuelTime = 5f;
            GManager.instance.durationFuel += addedFuelTime;
            float fuelRatio = addedFuelTime / GManager.instance.durationFuel;
            GManager.instance.sliderFuel.value = Mathf.Min(1f, GManager.instance.sliderFuel.value + fuelRatio);
            
            Debug.Log($"Bird collected - +{addedFuelTime}s fuel, new durationFuel: {GManager.instance.durationFuel}s, slider value: {GManager.instance.sliderFuel.value}");
            isAddFuel = true;
            GManager.instance.newMapText.text = "Bonus +5s fuel";
            StartCoroutine(FadeInText(1f));
        }
        else{
            GManager.instance.newMapText.text = "Congrats… you just unlocked absolutely nothing!";
            StartCoroutine(FadeInText(1f));

        }
    }

    IEnumerator DelayTwoSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        StartCoroutine(DelaytoEndGame());
    }

    IEnumerator DelayOneSecond(float delay)
    {
        yield return new WaitForSeconds(delay);
        Rigidbody2D airplaneRb = GManager.instance.airplaneRigidbody2D;
        airplaneRb.gameObject.SetActive(false);
        Debug.Log("Airplane deactivated after delay");
    }
}
