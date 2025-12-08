using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Plane : MonoBehaviour
{
    public static Plane instance;

    public Material blackMaterial;
    public ParticleSystem explosionEffect;


    public ParticleSystem smokeEffect;
    public TrailRenderer trailEffect;
    public TrailRenderer trailRenderer;

    public bool isGrounded = false;

    public int moneyDistance = 0;
    public int moneyCollect = 0;
    public int moneyTotal = 0;
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

            Debug.Log("*** MÁY BAY CHẠM ĐẤT - BẮT ĐẦU HỆ THỐNG DỪNG ***");
            
            // THÊM: Thông báo cho GManager biết máy bay đã chạm đất
            if (GManager.instance != null)
            {
                GManager.instance.OnPlaneGroundCollision();
                // Tắt các hệ thống có thể can thiệp velocity
                GManager.instance.isControllable = false;
                GManager.instance.isBoosterActive = false;
            }

            // Dừng các hiệu ứng
            if (RotaryFront.instance != null)
                RotaryFront.instance.StopWithDeceleration(3.0f);
            
            smokeEffect.Stop();
            
            // *** QUAN TRỌNG: Gọi UpMass() để giảm tốc độ ***
            StartCoroutine(UpMass());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            AnimCoin anim = other.GetComponent<AnimCoin>();
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
            AudioManager.instance.PlaySound(AudioManager.instance.bonusCollisionSoundClip);
           RandomPrizeBird();
           Destroy(other.gameObject);
        }
        if (other.CompareTag("Bonus4"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.bonusCollisionSoundClip);
            Destroy(other.gameObject);
            StartCoroutine(FadeBlackScreen());
            
        }
        if (other.CompareTag("Bonus2"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.bonusCollisionSoundClip);
            Destroy(other.gameObject);
            GManager.instance.durationFuel = 0f;
            StartCoroutine(DelaytoEndGame());
            Debug.Log("Bonus2 collected - Ending game soon");
            GManager.instance.newMapText.text = "Fastest balloon pop ever… and you lost!";
            StartCoroutine(FadeInText(1f));
            
        }
        if (other.CompareTag("Bonus3"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.bonusCollisionSoundClip);
            Destroy(other.gameObject);
            GManager.instance.durationFuel += 5f;
            Debug.Log("Rocket collected - +5s fuel" + GManager.instance.durationFuel);
            GManager.instance.newMapText.text = "Bonus - +5s fuel";
            StartCoroutine(FadeInText(1f));
        }
        if (other.CompareTag("MapStartCity"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapCityUnlocked = true;
            PlayerPrefs.SetInt("IsMapCityUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapCity;
            PlayerPrefs.Save();
            Settings.instance.NotificationNewMapText.text = "You have unlocked the City Map!";
            Settings.instance.NotificationNewMap();
        }
        
        if (other.CompareTag("MapStartBeach"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapBeachUnlocked = true;
            PlayerPrefs.SetInt("IsMapBeachUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapBeach;
            PlayerPrefs.Save();
            Settings.instance.NotificationNewMapText.text = "You have unlocked the Beach Map!";
            Settings.instance.NotificationNewMap();
        }
        
        if (other.CompareTag("MapStartDesert"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapDesertUnlocked = true;
            PlayerPrefs.SetInt("IsMapDesertUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapDesert;
            PlayerPrefs.Save();
            Settings.instance.NotificationNewMapText.text = "You have unlocked the Desert Map!";
            Settings.instance.NotificationNewMap();
        }
        
        if (other.CompareTag("MapStartField"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapFieldUnlocked = true;
            PlayerPrefs.SetInt("IsMapFieldUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapField;
            PlayerPrefs.Save();
            Settings.instance.NotificationNewMapText.text = "You have unlocked the Field Map!";
            Settings.instance.NotificationNewMap();
        }
        
        if (other.CompareTag("MapStartIce"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapIceUnlocked = true;
            PlayerPrefs.SetInt("IsMapIceUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapIce;
            PlayerPrefs.Save();
            Settings.instance.NotificationNewMapText.text = "You have unlocked the Ice Map!";
            Settings.instance.NotificationNewMap();
        }
        
        if (other.CompareTag("MapStartLava"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.unlockMapSoundClip);
            MapSpawner.instance.isMapLavaUnlocked = true;
            PlayerPrefs.SetInt("IsMapLavaUnlocked", 1); // Lưu trạng thái mở khóa
            BonusSpawner.instance.rocketPrefabs = ChangeBonusMap.instance.bonusMapLava;
            PlayerPrefs.Save();
            Settings.instance.NotificationNewMapText.text = "You have unlocked the Lava Map!";
            Settings.instance.NotificationNewMap();
        }
        
    
        
    }
    
    
    
    IEnumerator UpMass()
    {
        // Lấy Rigidbody2D từ GManager
        Rigidbody2D airplaneRb = GManager.instance.airplaneRigidbody2D;
        
        if (airplaneRb == null)
        {
            Debug.LogWarning("Airplane Rigidbody2D not found!");
            yield break;
        }
        
        // Lấy góc ban đầu khi máy bay chạm đất
        float initialAngleZ = GetCurrentRotationZ(airplaneRb);
        float initialAngleX = airplaneRb.transform.eulerAngles.x;
        if (initialAngleZ < -15f || initialAngleZ > 15f){
            AudioManager.instance.PlaySound(AudioManager.instance.explosionSoundClip);
            AudioManager.instance.StopPlayerSound();
            MakePlaneBlackAndExplode();
            Debug.Log("Airplane crashed due to excessive tilt angle: " + initialAngleZ);
            TextIntro.instance.GetRandomTextWinMessage(0); // bad
        }
        else{
            AudioManager.instance.PlaySound(AudioManager.instance.perfectLandingSoundClip);
            AudioManager.instance.StopPlayerSound();
            TextIntro.instance.GetRandomTextWinMessage(1); // good
            Settings.instance.ImageErrorAngleZ.gameObject.SetActive(false);
            if (initialAngleX > 180f) initialAngleX -= 360f; // Chuẩn hóa về [-180, 180]
            
            // Lấy velocity ban đầu
            Vector2 initialVelocity = airplaneRb.velocity;
            
            // Thời gian để giảm góc và velocity về 0 (có thể điều chỉnh)
            float duration = 3f;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                
                // Tính tỷ lệ giảm dần (từ 1 về 0)
                float t = elapsedTime / duration;
                float smoothT = 1f - t; // Giảm từ 1 về 0
                
                // Giảm từ từ rotation về 0
                float targetAngleZ = initialAngleZ * smoothT;
                float targetAngleX = initialAngleX * smoothT;
                Vector3 currentRotation = airplaneRb.transform.eulerAngles;
                currentRotation.z = targetAngleZ;
                currentRotation.x = targetAngleX;
                airplaneRb.transform.eulerAngles = currentRotation;
                
                // Giảm từ từ velocity về 0
                Vector2 targetVelocity = initialVelocity * smoothT;
                airplaneRb.velocity = targetVelocity;
                
                yield return null;
            }
            
            // Đảm bảo rotation và velocity về 0 hoàn toàn
            Vector3 finalRotation = airplaneRb.transform.eulerAngles;
            finalRotation.z = 0f;
            finalRotation.x = 0f;
            airplaneRb.transform.eulerAngles = finalRotation;
            airplaneRb.velocity = Vector2.zero;
            
            // Có thể thêm logic khi máy bay đã dừng hoàn toàn
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

        // Đổi vật liệu nếu có
        var planeRenderer = GetComponent<Renderer>();
        if (planeRenderer != null && blackMaterial != null)
            planeRenderer.material = blackMaterial;

        // RESET / TWEEN ROTATION
        if (resetRotationOnExplode && GManager.instance != null && GManager.instance.airplaneRigidbody2D != null)
        {
            var rb = GManager.instance.airplaneRigidbody2D;
            rb.angularVelocity = 0f;

            // Tween về góc thẳng (Z = 0)
            Vector3 targetEuler = rb.transform.eulerAngles;
            targetEuler.z = 0f;

            // Nếu có DOTween: tween mượt
            rb.transform.DORotate(targetEuler, explodeRotationResetTime)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    // Khóa luôn rotation tránh lệch lại
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                });
        }

        // Nếu không dùng DOTween thì có thể làm tức thì:
        // var e = transform.eulerAngles; e.z = 0; transform.eulerAngles = e;

        EffectRotaryFront.ExplodeAll();
        if (DestroyWheels.instance != null) DestroyWheels.instance.Explode();
        if (ExplosionScale.instance != null) ExplosionScale.instance.Explosion();
        if (EffectAirplane.instance != null) EffectAirplane.instance.MakePlaneBlack();

        if (explosionEffect != null) explosionEffect.Play();

        StartCoroutine(DelayTwoSeconds(2f));
    }

    // Hàm helper lấy rotation Z chuẩn hóa về [-180, 180]
    float GetCurrentRotationZ(Rigidbody2D rb)
    {
        float z = rb.transform.eulerAngles.z;
        if (z > 180f) z -= 360f;
        return z;
    }
    private bool isAddMoneyDone = false;
    IEnumerator OpenImageWIn()
    {
        yield return new WaitForSeconds(2f);
        if (!isAddMoneyDone)
        {
            // THÊM: Lưu GameObject máy bay hiện tại trước khi kết thúc game
            SaveCurrentAirplane();
            AudioManager.instance.PlaySound(AudioManager.instance.victorySoundClip);
            GManager.instance.coinEffect.Stop();
            isAddMoneyDone = true;
            moneyDistance = (int)(GManager.instance.distanceTraveled / 6.34f);
            Debug.Log("Money from Distance: " + moneyDistance + " | Current Money: " + moneyCollect + " | Total Money: " + moneyTotal);
            
            Debug.Log("Updated Total Money: " + GManager.instance.totalMoney);
            // Settings.instance.totalMoneyPlayText.text = "" + moneyTotal;

            // Cập nhật UI
            GManager.instance.totalMoneyText.text = "" + GManager.instance.totalMoney;
            GManager.instance.moneyText.text = "" + moneyCollect;


            // Lưu TotalMoney và đảm bảo lưu ngay
            
            Settings.instance.OpenWinImage();
        }
    }
    
    // THÊM: Hàm lưu tên GameObject máy bay hiện tại
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
        if (RotaryFront.instance != null)
            RotaryFront.instance.StopWithDeceleration(3.0f);
        
        smokeEffect.Stop();
        StartCoroutine(OpenImageWIn());
    }

    IEnumerator FadeInText(float duration)
    {
        Color c = GManager.instance.newMapText.color;
        c.a = 0;
        GManager.instance.newMapText.color = c;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / duration);
            GManager.instance.newMapText.color = c;
            yield return null;
        }
    }

    IEnumerator FadeBlackScreen()
    {
        // Fade in
        float elapsed = 0f;
        Settings.instance.iamgeBlackScreen.gameObject.SetActive(true);
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            Color c = Settings.instance.iamgeBlackScreen.color;
            c.a = Mathf.Clamp01(elapsed / 1f); // 1 giây fade in
            Settings.instance.iamgeBlackScreen.color = c;
            yield return null;
        }
        GManager.instance.newMapText.text = "Grounded by an umbrella—enjoy the darkness!";
        StartCoroutine(FadeInText(1f));
        // Giữ nguyên đen trong 3 giây
        yield return new WaitForSeconds(4f);

        // Fade out
        elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            Color c = Settings.instance.iamgeBlackScreen.color;
            c.a = 1f - Mathf.Clamp01(elapsed / 1f); // 1 giây fade out
            Settings.instance.iamgeBlackScreen.color = c;
            yield return null;
        }
        Settings.instance.iamgeBlackScreen.gameObject.SetActive(false);
    }

    public void RandomPrizeBird(){
        int[] coinPrize = {1000, 2000, 5000};
        int[] diamondPrize = {100, 300, 1000};
        int count = Random.Range(0, 3); //random 0,1,2
        if(count == 0){
            int randomIndex = Random.Range(0, coinPrize.Length);
            int prize = coinPrize[randomIndex];
            GManager.instance.totalMoney += prize;
            GManager.instance.newMapText.text = $"You grabbed {prize} shiny coins!";
            StartCoroutine(FadeInText(1f));
        }
        else if(count == 1){
            int randomIndex = Random.Range(0, diamondPrize.Length);
            int prize = diamondPrize[randomIndex];
            GManager.instance.totalDiamond += prize;
            GManager.instance.newMapText.text = $"You grabbed {prize} shiny diamonds!";
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
