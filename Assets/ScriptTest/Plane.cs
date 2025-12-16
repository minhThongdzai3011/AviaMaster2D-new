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
            AnimCoin anim = other.GetComponent<AnimCoin>();
            AudioManager.instance.PlaySound(AudioManager.instance.collectMoneySoundClip);
            if (anim != null)
            {
                // tăng tiền ngay (hoặc bạn có thể tăng tiền khi animation hoàn tất trong AnimCoin)
                moneyCollect += 2;

                anim.collected = true;
                anim.Collect();
            }
            else
            {
                moneyCollect += 1;
                Destroy(other.gameObject);
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
        if (other.CompareTag("MapStartCity"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapCityUnlocked = true;
            PlayerPrefs.SetInt("IsMapCityUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapCity;
            PlayerPrefs.Save();
            if(MapSpawner.instance.isMapCityUnlocked){
                Settings.instance.NotificationNewMapText.text = "Welcome to City Map!";
            }
            else{
                Settings.instance.NotificationNewMapText.text = "You have unlocked the City Map!";
            }
            Settings.instance.NotificationNewMap();
        }
        
        if (other.CompareTag("MapStartBeach"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapBeachUnlocked = true;
            PlayerPrefs.SetInt("IsMapBeachUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapBeach;
            PlayerPrefs.Save();
            if(MapSpawner.instance.isMapBeachUnlocked){
                Settings.instance.NotificationNewMapText.text = "Welcome to Beach Map!";
            }
            else{
                Settings.instance.NotificationNewMapText.text = "You have unlocked the Beach Map!";
            }
            Settings.instance.NotificationNewMap();
        }
        
        if (other.CompareTag("MapStartDesert"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapDesertUnlocked = true;
            PlayerPrefs.SetInt("IsMapDesertUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapDesert;
            PlayerPrefs.Save();
            if(MapSpawner.instance.isMapDesertUnlocked){
                Settings.instance.NotificationNewMapText.text = "Welcome to Desert Map!";
            }
            else{
                Settings.instance.NotificationNewMapText.text = "You have unlocked the Desert Map!";
            }
            Settings.instance.NotificationNewMap();
        }
        
        if (other.CompareTag("MapStartField"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapFieldUnlocked = true;
            PlayerPrefs.SetInt("IsMapFieldUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapField;
            PlayerPrefs.Save();
            if(MapSpawner.instance.isMapFieldUnlocked){
                Settings.instance.NotificationNewMapText.text = "Welcome to Field Map!";
            }
            else{
                Settings.instance.NotificationNewMapText.text = "You have unlocked the Field Map!";
            }
            Settings.instance.NotificationNewMap();
        }
        
        if (other.CompareTag("MapStartIce"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapIceUnlocked = true;
            PlayerPrefs.SetInt("IsMapIceUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapIce;
            PlayerPrefs.Save();
            if(MapSpawner.instance.isMapIceUnlocked){
                Settings.instance.NotificationNewMapText.text = "Welcome to Ice Map!";
            }
            else{
                Settings.instance.NotificationNewMapText.text = "You have unlocked the Ice Map!";
            }
            Settings.instance.NotificationNewMap();
        }
        
        if (other.CompareTag("MapStartLava"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapLavaUnlocked = true;
            PlayerPrefs.SetInt("IsMapLavaUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapLava;
            PlayerPrefs.Save();
            if(MapSpawner.instance.isMapLavaUnlocked){
                Settings.instance.NotificationNewMapText.text = "Welcome to Lava Map!";
            }
            else{
                Settings.instance.NotificationNewMapText.text = "You have unlocked the Lava Map!";
            }
            Settings.instance.NotificationNewMap();
        }
        if (other.CompareTag("NofitiCity"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapCity;
            
            Settings.instance.NotificationNewMapText.text = "It’s so noisy, is this Map City?";
            
            Settings.instance.NotificationNewMap();
        }
        if (other.CompareTag("NofitiBeach"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapBeach;
            
            Settings.instance.NotificationNewMapText.text = "Wow, what a beautiful beach!";
            
            Settings.instance.NotificationNewMap();
        }
        if (other.CompareTag("NofitiDesert"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapDesert;
            
            Settings.instance.NotificationNewMapText.text = "I want to become a cowboy when I come to this Desert map!";
            
            Settings.instance.NotificationNewMap();
        }
        if (other.CompareTag("NofitiField"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapField;
            
            Settings.instance.NotificationNewMapText.text = "This Field map is beautiful, the air is fresh!";
            
            Settings.instance.NotificationNewMap();
        }
        if (other.CompareTag("NofitiIce"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapIce;
            
            Settings.instance.NotificationNewMapText.text = "Oh, so cold! Did we just reach the Ice Map?";
            
            Settings.instance.NotificationNewMap();
        }
        if (other.CompareTag("NofitiLava"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapLava;
            
            Settings.instance.NotificationNewMapText.text = "Hey, lava ahead!";
            
            Settings.instance.NotificationNewMap();
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
            GManager.instance.durationFuel += 5f;
            
            Debug.Log("Rocket collected - +5s fuel" + GManager.instance.durationFuel);
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
}
