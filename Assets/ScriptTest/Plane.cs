using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Leaderboard;
using System.Linq;



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

    [Header("Kiểm tra máy bay khi dừng lại có ở airport không")]
    public bool isInAirPort = false;

    [Header("Rotation on Plane settings")]
    public int rotationOnPlane = 0;
    public float maxUpAngle = 35f;    
    public float maxDownAngle = -25f; 
    
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
        
        // Load SuperPlane status when game starts
        LoadSuperPlaneStatus();
    }
    public bool isStopSmokeEffect = true;
    public bool isStopExplosionEffect = true;
    public bool isStabilizeFuel = true;

    // Update is called once per frame
    void Update()
    {
        if(isStopSmokeEffect){
            smokeEffect.Stop();
        }
        if(isStopExplosionEffect){
            explosionEffect.Stop();
        }


        Vector3 planeRotation = GManager.instance.airplaneRigidbody2D.transform.eulerAngles;

        // Lấy góc Z
        float angleZ = planeRotation.z;

        // Nếu > 180 thì chuyển về giá trị âm tương ứng
        if (angleZ > 180f)
        {
            angleZ -= 360f;
            // angleZ += angleZ;
        }


        // Gán rotation cho trailEffect
         trailEffect.transform.localRotation = Quaternion.Euler(0f, 0f, angleZ);

        // Hiển thị text với định dạng 2 chữ số thập phân
        GManager.instance.rotationTrailText.text = angleZ.ToString("F2") + "°";

    }



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && !isGrounded)
        {
            AudioManager.instance.StopFallingSound();
            isStopExplosionEffect = false;
            isGrounded = true;

            Debug.Log("Máy bay đã chạm đất.");
            
            // Stop flight timer and update mission achievement 4
            if (MissionAchievements.instance != null)
            {
                MissionAchievements.instance.StopFlightTimerAndUpdateMission();
            }
            else
            {
                Debug.LogWarning("[LANDING] MissionAchievements.instance is null - cannot stop flight timer");
            }
            
            // *** CHỈ KIỂM TRA ANGLE, KHÔNG SET SUPER BONUS Ở ĐÂY ***
            float initialAngleZ = GetCurrentRotationZ(GManager.instance.airplaneRigidbody2D);
            bool isSafeLanding = (initialAngleZ >= -15f && initialAngleZ <= 15f);
            
            if (!isSafeLanding)
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
            if (Shop.instance != null && Shop.instance.isCheckedPlaneIndex == 14)
            {
                SuperPlaneManager.instance.skillEffectSuperPlane2.SetActive(false);
            }
            AudioManager.instance.StopFallingSound();
            StartCoroutine(UpMass());
        }
    }

    public bool isExplodedbyBoom = false;
        
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Reset"))
        {
            GManager.instance.AgainGame();
        }
        if (other.CompareTag("Coin") || other.CompareTag("Respawn") || other.CompareTag("green1"))
        {
            // AnimCoin anim = other.GetComponent<AnimCoin>();
            AudioManager.instance.PlaySound(AudioManager.instance.collectMoneySoundClip);
            if (PositionX.instance.isMaxPower)
            {
                // tăng tiền ngay (hoặc bạn có thể tăng tiền khi animation hoàn tất trong AnimCoin)
                moneyCollect += 20;
                MissionDaily.instance.dailyMission2Progress += 20;
                MissionDaily.instance.UpdateDailyMission();
                MissionPlane.instance.planeMission2Progress += 20;
                MissionPlane.instance.UpdatePlaneMission();

                // anim.collected = true;
                // anim.Collect();
                Destroy(other.gameObject);
            }
            else
            {
                moneyCollect += 10;
                MissionDaily.instance.dailyMission2Progress += 10;
                MissionDaily.instance.UpdateDailyMission();
                MissionPlane.instance.planeMission2Progress += 10;
                MissionPlane.instance.UpdatePlaneMission();
                Destroy(other.gameObject);
            }
        }
        if (other.CompareTag("Diamond"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.collectDiamondSoundClip);
            GManager.instance.totalDiamond += 10;
            GManager.instance.totalDiamondText.text = GManager.instance.totalDiamond.ToString("F0");
            Debug.Log("Diamond collected +10" + GManager.instance.totalDiamond);
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
                MissionPlane.instance.planeMission5Progress++;
                MissionPlane.instance.UpdatePlaneMission();
                //EffectExplosionBonus.instance.ExplosionEffect();
                // EffectExplosionBonus.instance.ExplosionEffect1();
                // Destroy(other.gameObject);
                other.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                if (SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane5 && SuperPlaneManager.instance.skillPlane5)
                {
                    SuperPlaneManager.instance.skillPlane5 = false;
                    SuperPlaneManager.instance.imageSkillSuperPlane5.gameObject.SetActive(false);
                    AudioManager.instance.PlaySound(AudioManager.instance.obstacleCollisionSoundClip);
                    EffectExplosionBonus ef = other.GetComponent<EffectExplosionBonus>();
                    ef.ExplosionEffect();
                    ef.ExplosionEffect1();
                    SuperPlaneManager.instance.skillEffectSuperPlane2.SetActive(false);
                    //EffectExplosionBonus.instance.ExplosionEffect();
                    // EffectExplosionBonus.instance.ExplosionEffect1();
                    // Destroy(other.gameObject);
                    other.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    // Đảm bảo tắt falling sound trước khi phát âm thanh khác
                    AudioManager.instance.StopFallingSound();
                    // AudioManager.instance.PlaySound(AudioManager.instance.landingSoundClip);
                    AudioManager.instance.StopPlayerSound();
                    AudioManager.instance.PlaySound(AudioManager.instance.explosionSoundClip);
                    isExplodedbyBoom = true;
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
                    isStabilizeFuel = false;

                    GManager.instance.durationFuel = 0f;
                    GManager.instance.currentControlDuration = 0f;
                    GManager.instance.currentControlTimer = GManager.instance.currentControlDuration;
                    AudioManager.instance.StopFallingSound();
                }
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
            // Kiểm tra xem hiệu ứng có đang chạy không
            if (isFogEffectPlaying)
            {
                Debug.Log("FadeFogImage đang chạy, bỏ qua va chạm Bonus4 này");
                Destroy(other.gameObject); // Vẫn phá hủy object
                MissionPlane.instance.planeMission5Progress++;
                MissionPlane.instance.UpdatePlaneMission();
                return;
            }
            
            AudioManager.instance.PlaySound(AudioManager.instance.obstacleCollisionSoundClip);
            Destroy(other.gameObject);
            StartCoroutine(FadeFogImage());
            MissionPlane.instance.planeMission5Progress++;
            MissionPlane.instance.UpdatePlaneMission();
        }
        if (other.CompareTag("Bonus2"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.obstacleCollisionSoundClip);
            EffectExplosionBonus ef = other.GetComponent<EffectExplosionBonus>();
            ef.ExplosionEffect();
            ef.ExplosionEffect1();
            MissionPlane.instance.planeMission5Progress++;
            MissionPlane.instance.UpdatePlaneMission();
            //EffectExplosionBonus.instance.ExplosionEffect();
           // EffectExplosionBonus.instance.ExplosionEffect1();
            // Destroy(other.gameObject);
            other.gameObject.GetComponent<SpriteRenderer>().enabled = false;

            Debug.Log("Bonus2 collected - Fuel to 0" + PositionX.instance.isMaxPower);
            if (!PositionX.instance.isMaxPower)
            {
                if (SuperPlaneManager.instance != null && SuperPlaneManager.instance.isSuperPlane5 && SuperPlaneManager.instance.skillPlane5)
                {
                    SuperPlaneManager.instance.skillPlane5 = false;
                    Debug.Log("Bonus2 collected - Super Plane 5 Active - No Fuel Lost");
                    SuperPlaneManager.instance.imageSkillSuperPlane5.gameObject.SetActive(false);
                    SuperPlaneManager.instance.skillEffectSuperPlane2.SetActive(false);
                }
                else
                {
                    GManager.instance.durationFuel = 0f;
                    GManager.instance.currentControlDuration = 0f;
                    Debug.Log("Bonus2 collected - Ending game soon" + GManager.instance.durationFuel);
                    GManager.instance.newMapText.text = "Fastest balloon pop ever… and you lost!";
                    isStabilizeFuel = false;
                    StartCoroutine(FadeInText(1f));
                    if(TrailRendererRight.instance != null ) TrailRendererRight.instance.StopTrail();
                    if(TrailRendererLeft.instance != null ) TrailRendererLeft.instance.StopTrail();
                }
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
            
            // Check if this is a new unlock
            bool wasAlreadyUnlocked = MapSpawner.instance.isMapCityUnlocked;
            MapSpawner.instance.isMapCityUnlocked = true;
            PlayerPrefs.SetInt("IsMapCityUnlocked", 1);
            
            // City map is default, don't count it for achievement progress
            // Only count the 5 additional maps: Beach, Desert, Field, Ice, Lava
            int unlockedMapsCount = 0;
            if (MapSpawner.instance.isMapBeachUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapDesertUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapFieldUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapIceUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapLavaUnlocked) unlockedMapsCount++;
            
            // Only update if progress increased
            if (unlockedMapsCount > MissionAchievements.instance.achievementMission5Progress)
            {
                MissionAchievements.instance.achievementMission5Progress = unlockedMapsCount;
                MissionAchievements.instance.UpdateAchievementMission();
                Debug.Log($"[MAP UNLOCK] City unlocked. Total progress: {unlockedMapsCount}/5");
            }

            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapCity;
            PlayerPrefs.Save();
            
            // Track airport hiện tại
            currentAirportTag = "MapStartCity";
            Debug.Log("[AIRPORT] Entered City Airport zone");
        }
        
        if (other.CompareTag("MapStartBeach") && !isBoom)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            
            // Check if this is a new unlock
            bool wasAlreadyUnlocked = MapSpawner.instance.isMapBeachUnlocked;

            bool check = PlayerPrefs.GetInt("IsMapBeachUnlocked", 0) == 1;
            if (check)
            {
                Debug.Log("Map Beach was already unlocked before.");
            }
            else
            {
                GMAnalytics.LogEvent("unlock_map_beach", 2);
            }


            MapSpawner.instance.isMapBeachUnlocked = true;
            PlayerPrefs.SetInt("IsMapBeachUnlocked", 1);

            // Only count the 5 additional maps: Beach, Desert, Field, Ice, Lava (not City)
            int unlockedMapsCount = 0;
            if (MapSpawner.instance.isMapBeachUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapDesertUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapFieldUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapIceUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapLavaUnlocked) unlockedMapsCount++;
            
            // Only update if progress increased
            if (unlockedMapsCount > MissionAchievements.instance.achievementMission5Progress)
            {
                MissionAchievements.instance.achievementMission5Progress = unlockedMapsCount;
                MissionAchievements.instance.UpdateAchievementMission();
                Debug.Log($"[MAP UNLOCK] Beach unlocked. Total progress: {unlockedMapsCount}/5");
            }

            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapBeach;
            PlayerPrefs.Save();
            
            // Track airport hiện tại
            currentAirportTag = "MapStartBeach";
            Debug.Log("[AIRPORT] Entered Beach Airport zone");
        }
        
        if (other.CompareTag("MapStartDesert") && !isBoom)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);



            bool check = PlayerPrefs.GetInt("IsMapDesertUnlocked", 0) == 1;
            if (check)
            {
                Debug.Log("Map Desert was already unlocked before.");
            }
            else
            {
                GMAnalytics.LogEvent("unlock_map_desert", 2);
            }



            
            // Check if this is a new unlock
            bool wasAlreadyUnlocked = MapSpawner.instance.isMapDesertUnlocked;
            MapSpawner.instance.isMapDesertUnlocked = true;
            PlayerPrefs.SetInt("IsMapDesertUnlocked", 1);

            // Only count the 5 additional maps: Beach, Desert, Field, Ice, Lava (not City)
            int unlockedMapsCount = 0;
            if (MapSpawner.instance.isMapBeachUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapDesertUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapFieldUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapIceUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapLavaUnlocked) unlockedMapsCount++;
            
            // Only update if progress increased
            if (unlockedMapsCount > MissionAchievements.instance.achievementMission5Progress)
            {
                MissionAchievements.instance.achievementMission5Progress = unlockedMapsCount;
                MissionAchievements.instance.UpdateAchievementMission();
                Debug.Log($"[MAP UNLOCK] Desert unlocked. Total progress: {unlockedMapsCount}/5");
            }

            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapDesert;
            PlayerPrefs.Save();
            
            // Track airport hiện tại
            currentAirportTag = "MapStartDesert";
            Debug.Log("[AIRPORT] Entered Desert Airport zone");
        }
        
        if (other.CompareTag("MapStartField") && !isBoom)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);


            bool check = PlayerPrefs.GetInt("IsMapFieldUnlocked", 0) == 1;
            if (check)
            {
                Debug.Log("Map Field was already unlocked before.");
            }
            else
            {
                GMAnalytics.LogEvent("unlock_map_field", 2);
            }
            
            // Check if this is a new unlock
            bool wasAlreadyUnlocked = MapSpawner.instance.isMapFieldUnlocked;
            MapSpawner.instance.isMapFieldUnlocked = true;
            PlayerPrefs.SetInt("IsMapFieldUnlocked", 1);

            // Only count the 5 additional maps: Beach, Desert, Field, Ice, Lava (not City)
            int unlockedMapsCount = 0;
            if (MapSpawner.instance.isMapBeachUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapDesertUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapFieldUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapIceUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapLavaUnlocked) unlockedMapsCount++;
            
            // Only update if progress increased
            if (unlockedMapsCount > MissionAchievements.instance.achievementMission5Progress)
            {
                MissionAchievements.instance.achievementMission5Progress = unlockedMapsCount;
                MissionAchievements.instance.UpdateAchievementMission();
                Debug.Log($"[MAP UNLOCK] Field unlocked. Total progress: {unlockedMapsCount}/5");
            }

            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapField;
            PlayerPrefs.Save();
            
            // Track airport hiện tại
            currentAirportTag = "MapStartField";
            Debug.Log("[AIRPORT] Entered Field Airport zone");
        }
        
        if (other.CompareTag("MapStartIce") && !isBoom)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);


            bool check = PlayerPrefs.GetInt("IsMapIceUnlocked", 0) == 1;
            if (check)
            {
                Debug.Log("Map Ice was already unlocked before.");
            }
            else
            {
                GMAnalytics.LogEvent("unlock_map_ice", 2);
            }
            
            // Check if this is a new unlock
            bool wasAlreadyUnlocked = MapSpawner.instance.isMapIceUnlocked;
            MapSpawner.instance.isMapIceUnlocked = true;
            PlayerPrefs.SetInt("IsMapIceUnlocked", 1);

            // Only count the 5 additional maps: Beach, Desert, Field, Ice, Lava (not City)
            int unlockedMapsCount = 0;
            if (MapSpawner.instance.isMapBeachUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapDesertUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapFieldUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapIceUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapLavaUnlocked) unlockedMapsCount++;
            
            // Only update if progress increased
            if (unlockedMapsCount > MissionAchievements.instance.achievementMission5Progress)
            {
                MissionAchievements.instance.achievementMission5Progress = unlockedMapsCount;
                MissionAchievements.instance.UpdateAchievementMission();
                Debug.Log($"[MAP UNLOCK] Ice unlocked. Total progress: {unlockedMapsCount}/5");
            }

            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapIce;
            PlayerPrefs.Save();
            
            // Track airport hiện tại
            currentAirportTag = "MapStartIce";
            Debug.Log("[AIRPORT] Entered Ice Airport zone");
        }
        
        if (other.CompareTag("MapStartLava") && !isBoom)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);

            bool check = PlayerPrefs.GetInt("IsMapLavaUnlocked", 0) == 1;
            if (check)
            {
                Debug.Log("Map Lava was already unlocked before.");
            }
            else
            {
                GMAnalytics.LogEvent("unlock_map_lava", 2);
            }
            
            // Check if this is a new unlock
            bool wasAlreadyUnlocked = MapSpawner.instance.isMapLavaUnlocked;
            MapSpawner.instance.isMapLavaUnlocked = true;
            PlayerPrefs.SetInt("IsMapLavaUnlocked", 1);
            
            // Only count the 5 additional maps: Beach, Desert, Field, Ice, Lava (not City)
            int unlockedMapsCount = 0;
            if (MapSpawner.instance.isMapBeachUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapDesertUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapFieldUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapIceUnlocked) unlockedMapsCount++;
            if (MapSpawner.instance.isMapLavaUnlocked) unlockedMapsCount++;
            
            // Only update if progress increased
            if (unlockedMapsCount > MissionAchievements.instance.achievementMission5Progress)
            {
                MissionAchievements.instance.achievementMission5Progress = unlockedMapsCount;
                MissionAchievements.instance.UpdateAchievementMission();
                Debug.Log($"[MAP UNLOCK] Lava unlocked. Total progress: {unlockedMapsCount}/5");
            }
            
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
    
    void OnTriggerExit2D(Collider2D other)
    {
        // Clear currentAirportTag khi máy bay rời khỏi airport zone
        if (other.CompareTag("MapStartCity") || 
            other.CompareTag("MapStartBeach") || 
            other.CompareTag("MapStartDesert") || 
            other.CompareTag("MapStartField") || 
            other.CompareTag("MapStartIce") || 
            other.CompareTag("MapStartLava"))
        {
            Debug.Log($"[AIRPORT] Left {other.tag} zone - clearing currentAirportTag");
            currentAirportTag = "";
        }
    }
    
    
    
    IEnumerator UpMass()
    {
        // Đảm bảo tắt falling sound trước khi phát âm thanh khác
        AudioManager.instance.StopFallingSound();
        // AudioManager.instance.PlaySound(AudioManager.instance.landingSoundClip);
        AudioManager.instance.StopPlayerSound();
        Rigidbody2D airplaneRb = GManager.instance.airplaneRigidbody2D;
        
        if (airplaneRb == null)
        {
            Debug.LogWarning("Airplane Rigidbody2D not found!");
            yield break;
        }
        if(SuperPlaneManager.instance != null)
        {
            SuperPlaneManager.instance.ResetBuffBoostandFuelSuperPlane1();
            SuperPlaneManager.instance.ResetBuffBoostandFuelSuperPlane2();
            SuperPlaneManager.instance.ResetBuffBoostandFuelSuperPlane3();
            SuperPlaneManager.instance.ResetBuffBoostandFuelSuperPlane4();
            SuperPlaneManager.instance.ResetBuffBoostandFuelSuperPlane5();
        }
        GManager.instance.downFuelImage.gameObject.SetActive(false);
        GManager.instance.upAircraftImage.gameObject.SetActive(false);
        MissionDaily.instance.dailyMission3Progress++;
        MissionDaily.instance.UpdateDailyMission();
        float initialAngleZ = GetCurrentRotationZ(airplaneRb);
        float initialAngleX = airplaneRb.transform.eulerAngles.x;
        if (initialAngleZ < -15f || initialAngleZ > 15f){
            AudioManager.instance.StopFallingSound();
            AudioManager.instance.PlaySound(AudioManager.instance.explosionSoundClip);
            AudioManager.instance.StopPlayerSound();
            MakePlaneBlackAndExplode();
            Debug.Log("Airplane crashed due to excessive tilt angle: " + initialAngleZ);
            TextIntro.instance.GetRandomTextWinMessage(0); 
            isBoom = true;
            GManager.instance.isBonus = false;   
        }
        else{
            AudioManager.instance.StopFallingSound();
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
            isInAirPort = true;
            
            GManager.instance.isBonus = true;   
            
            // *** KIỂM TRA VÀ SET SUPER BONUS SAU KHI MÁY BAY DỪNG HOÀN TOÀN ***
            if (!string.IsNullOrEmpty(currentAirportTag))
            {
                Debug.Log($"[SAFE LANDING] Landed safely at {currentAirportTag} after stopping!");
                
                // Set airport flag tương ứng và super bonus
                switch (currentAirportTag)
                {
                    case "MapStartCity":
                        isAirPortCity = true;
                        GManager.instance.isSuperBonus = true;   
                        GManager.instance.isBonus = false; 
                        PlayerPrefs.SetInt("LastSafeLandingAirport", 0);
                        Settings.instance.superBonusText.gameObject.SetActive(true);
                        Debug.Log("[SAFE LANDING] Next game will start at City map - SuperBonus activated!");
                        MissionDaily.instance.dailyMission4Progress++;
                        MissionDaily.instance.UpdateDailyMission();
                        break;
                    case "MapStartBeach":
                        isAirPortBeach = true;
                        GManager.instance.isSuperBonus = true; 
                        GManager.instance.isBonus = false;  
                        PlayerPrefs.SetInt("LastSafeLandingAirport", 1);
                        Settings.instance.superBonusText.gameObject.SetActive(true);
                        Debug.Log("[SAFE LANDING] Next game will start at Beach map - SuperBonus activated!");
                        MissionDaily.instance.dailyMission4Progress++;
                        MissionDaily.instance.UpdateDailyMission();
                        break;
                    case "MapStartDesert":
                        isAirPortDesert = true;
                        GManager.instance.isSuperBonus = true;
                        GManager.instance.isBonus = false;  
                        PlayerPrefs.SetInt("LastSafeLandingAirport", 2);
                        Settings.instance.superBonusText.gameObject.SetActive(true);
                        Debug.Log("[SAFE LANDING] Next game will start at Desert map - SuperBonus activated!");
                        MissionDaily.instance.dailyMission4Progress++;
                        MissionDaily.instance.UpdateDailyMission();
                        break;
                    case "MapStartField":
                        isAirPortField = true;
                        GManager.instance.isSuperBonus = true; 
                        GManager.instance.isBonus = false; 
                        PlayerPrefs.SetInt("LastSafeLandingAirport", 3);
                        Settings.instance.superBonusText.gameObject.SetActive(true);
                        Debug.Log("[SAFE LANDING] Next game will start at Field map - SuperBonus activated!");
                        MissionDaily.instance.dailyMission4Progress++;
                        MissionDaily.instance.UpdateDailyMission();
                        break;
                    case "MapStartIce":
                        isAirPortIce = true;
                        GManager.instance.isSuperBonus = true; 
                        GManager.instance.isBonus = false; 
                        PlayerPrefs.SetInt("LastSafeLandingAirport", 4);
                        Settings.instance.superBonusText.gameObject.SetActive(true);
                        Debug.Log("[SAFE LANDING] Next game will start at Ice map - SuperBonus activated!");
                        MissionDaily.instance.dailyMission4Progress++;
                        MissionDaily.instance.UpdateDailyMission();
                        break;
                    case "MapStartLava":
                        isAirPortLava = true;
                        GManager.instance.isSuperBonus = true; 
                        GManager.instance.isBonus = false; 
                        PlayerPrefs.SetInt("LastSafeLandingAirport", 5);
                        Settings.instance.superBonusText.gameObject.SetActive(true);
                        Debug.Log("[SAFE LANDING] Next game will start at Lava map - SuperBonus activated!");
                        MissionDaily.instance.dailyMission4Progress++;
                        MissionDaily.instance.UpdateDailyMission();
                        break;
                }
                PlayerPrefs.Save();
            }
            
            Debug.Log("Airplane stopped sliding");
            StartCoroutine(OpenImageWIn());
            
            }
        }

    public bool resetRotationOnExplode = true;
    public float explodeRotationResetTime = 0.4f;
    private bool hasExploded = false;
    public bool isImageWinOpen = false;

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
    
    // Helper method để format số tiền thành dạng rút gọn
    string FormatMoney(float money)
    {
        if (money >= 1000000000)
            return (money / 1000000000f).ToString("F1") + "B";
        else if (money >= 1000000)
            return (money / 1000000f).ToString("F1") + "M";
        else if (money >= 1000)
            return (money / 1000f).ToString("F1") + "k";
        else
            return money.ToString("F0");
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
            GManager.instance.totalMoneyText.text = FormatMoney(GManager.instance.totalMoney);
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
        // Đánh dấu hiệu ứng bắt đầu
        isFogEffectPlaying = true;
        Debug.Log("FadeFogImage started");
        
        float elapsed = 0f;
        Settings.instance.imageWhiteScreen.gameObject.SetActive(true);
        // Settings.instance.imageWhite1Screen.gameObject.SetActive(true);
        Settings.instance.iamgeBlackScreen.gameObject.SetActive(true);
        Color startColor = Settings.instance.iamgeBlackScreen.color;
        startColor.a = 0f;
        Settings.instance.iamgeBlackScreen.color = startColor;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            Color c = Settings.instance.iamgeBlackScreen.color;
            Color b = Settings.instance.imageWhiteScreen.color;
            c.a = Mathf.Clamp01(elapsed / 1f);
            b.a = Mathf.Clamp01(elapsed / 1f);
            // Settings.instance.imageWhiteScreen.color = b;
            Settings.instance.iamgeBlackScreen.color = c;
            yield return null;
        }

        float moveDuration = 5f;
        float moveElapsed = 0f;
        Vector3 startPos = Settings.instance.iamgeBlackScreen.transform.position;
        Vector3 startPosWhite = Settings.instance.imageWhiteScreen.transform.position;  
        Vector3 targetPos = startPos + new Vector3(-800f, 0f, 0f);
        Vector3 targetPosWhite = startPosWhite + new Vector3(-800f, 0f, 0f);

        while (moveElapsed < moveDuration)
        {
            moveElapsed += Time.deltaTime;
            float t = moveElapsed / moveDuration;
            Settings.instance.iamgeBlackScreen.transform.position = Vector3.Lerp(startPos, targetPos, t);
            Settings.instance.imageWhiteScreen.transform.position = Vector3.Lerp(startPosWhite, targetPosWhite, t);
            yield return null;
        }


        elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            Color c = Settings.instance.iamgeBlackScreen.color;
            Color b = Settings.instance.imageWhiteScreen.color;
            b.a = 1f - Mathf.Clamp01(elapsed / 1f);
            c.a = 1f - Mathf.Clamp01(elapsed / 1f); 
            Settings.instance.iamgeBlackScreen.color = c;
            // Settings.instance.imageWhiteScreen.color = b;
            yield return null;
        }

        // Quay trở lại vị trí ban đầu
        float returnDuration = 2f;
        float returnElapsed = 0f;
        Vector3 currentPos = Settings.instance.iamgeBlackScreen.transform.position;
        
        while (returnElapsed < returnDuration)
        {
            returnElapsed += Time.deltaTime;
            float t = returnElapsed / returnDuration;
            Settings.instance.iamgeBlackScreen.transform.position = Vector3.Lerp(currentPos, startPos, t);
            Settings.instance.imageWhiteScreen.transform.position = Vector3.Lerp(currentPos, startPosWhite, t);
            yield return null;
        }
        
        // Đảm bảo vị trí chính xác
        Settings.instance.iamgeBlackScreen.transform.position = startPos;
        Settings.instance.imageWhiteScreen.transform.position = startPosWhite;
        
        Settings.instance.imageWhiteScreen.gameObject.SetActive(false);
        Settings.instance.iamgeBlackScreen.gameObject.SetActive(false);
        
        // Đánh dấu hiệu ứng kết thúc
        isFogEffectPlaying = false;
        Debug.Log("FadeFogImage completed");
        // Settings.instance.imageWhiteScreen.gameObject.SetActive(false);
    }

    public bool isAddFuel = false;
    public float addedFuelAmount = 0f; // Lượng fuel được thêm vào
    private bool isFogEffectPlaying = false; // Flag kiểm tra hiệu ứng FadeFogImage
    
    public void RandomPrizeBird(){
        int[] coinPrize1 = {100, 200, 500};
        int[] coinPrize2 = {1000, 2000, 5000};
        int[] coinPrize3 = {5000, 10000, 20000};
        int count = Random.Range(0, 3); 
        // int count = Random.Range(1, 1); // Chỉ chọn phần thưởng nhiên liệu để kiểm tra hiệu ứng 
        if(GManager.instance.isFallingInSequence && count == 1){
            count = 2; // Chọn phần thưởng "không có gì" nếu đang trong chuỗi rơi 
        }
        if(count == 0){
            int prize = 0;
            if ( GManager.instance.moneyPower > 10000 || GManager.instance.moneyFuel > 10000 || GManager.instance.moneyBoost > 10000)
            {
                int randomIndex = Random.Range(0, coinPrize3.Length);
                prize = coinPrize3[randomIndex];
            }
            else if ( GManager.instance.moneyPower > 3000 || GManager.instance.moneyFuel > 3000 || GManager.instance.moneyBoost > 3000)
            {
                int randomIndex = Random.Range(0, coinPrize2.Length);
                prize = coinPrize2[randomIndex];
            }
            else
            {
                int randomIndex = Random.Range(0, coinPrize1.Length);
                prize = coinPrize1[randomIndex];
            }
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
            float addedFuelTime = GManager.instance.durationFuel * 0.15f;
            GManager.instance.durationFuel += addedFuelTime;
            
            // Set flag kèm theo lượng fuel thêm vào
            isAddFuel = true;
            addedFuelAmount = addedFuelTime;
            
            Debug.Log($"Bird collected - +{addedFuelTime}s fuel, new durationFuel: {GManager.instance.durationFuel}s");
            GManager.instance.newMapText.text = "Bonus +15% fuel";
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

    // Load SuperPlane status when game starts
    void LoadSuperPlaneStatus()
    {
        if (SuperPlaneManager.instance == null)
        {
            Debug.LogWarning("[PLANE] SuperPlaneManager instance is null");
            return;
        }

        int currentPlaneIndex = PlayerPrefs.GetInt("isCheckedPlaneIndex", 13);
        Debug.Log($"[PLANE] Loading SuperPlane status for plane index: {currentPlaneIndex}");
        
        CheckSuperPlaneByIndex(currentPlaneIndex);
    }

    // Check and set SuperPlane status by plane index  
    void CheckSuperPlaneByIndex(int planeIndex)
    {
        if (SuperPlaneManager.instance == null) 
        {
            Debug.LogWarning("[PLANE] SuperPlaneManager instance is null in CheckSuperPlaneByIndex");
            return;
        }
        
        // Reset all SuperPlane flags first
        SuperPlaneManager.instance.isSuperPlane1 = false;
        SuperPlaneManager.instance.isSuperPlane2 = false;
        SuperPlaneManager.instance.isSuperPlane3 = false;
        SuperPlaneManager.instance.isSuperPlane4 = false;
        SuperPlaneManager.instance.isSuperPlane5 = false;
        
        // Set SuperPlane based on plane index
        switch (planeIndex)
        {
            case 2: // Plane 3 = SuperPlane1 
                SuperPlaneManager.instance.isSuperPlane1 = true;
                Debug.Log("[PLANE] SuperPlane1 activated for Plane 3");
                break;
            case 5: // Plane 6 = SuperPlane2
                SuperPlaneManager.instance.isSuperPlane2 = true;
                Debug.Log("[PLANE] SuperPlane2 activated for Plane 6");
                break;
            case 7: // Plane 8 = SuperPlane3
                SuperPlaneManager.instance.isSuperPlane3 = true;
                Debug.Log("[PLANE] SuperPlane3 activated for Plane 8");
                break;
            case 11: // Plane 12 = SuperPlane4
                SuperPlaneManager.instance.isSuperPlane4 = true;
                Debug.Log("[PLANE] SuperPlane4 activated for Plane 12");
                break;
            case 14: // Plane 15 = SuperPlane5
                SuperPlaneManager.instance.isSuperPlane5 = true;
                Debug.Log("[PLANE] SuperPlane5 activated for Plane 15");
                break;
            default:
                Debug.Log($"[PLANE] No SuperPlane for plane index: {planeIndex}");
                break;
        }
        
        // Verify the SuperPlane status was set correctly
        Debug.Log($"[PLANE] SuperPlane Status - SP1: {SuperPlaneManager.instance.isSuperPlane1}, SP2: {SuperPlaneManager.instance.isSuperPlane2}, SP3: {SuperPlaneManager.instance.isSuperPlane3}, SP4: {SuperPlaneManager.instance.isSuperPlane4}, SP5: {SuperPlaneManager.instance.isSuperPlane5}");
    }
}
