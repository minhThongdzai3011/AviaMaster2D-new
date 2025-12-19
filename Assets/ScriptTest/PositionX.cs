using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PositionX : MonoBehaviour
{
    public static PositionX instance;
    public Vector2 positionX;
    public Vector2 newPositionX;
    public float timePerfect = 0f;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    
    {
        Debug.Log("Time Perfect: " + timePerfect);


        positionX = transform.position;
        
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMoveX(transform.position.x + 1.3f, 0.4f).SetEase(Ease.Linear));
        seq.Append(transform.DOMoveX(transform.position.x + 1.5f, 0.05f).SetEase(Ease.Linear)); 
        seq.Append(transform.DOMoveX(transform.position.x + 3.2f, 0.3f).SetEase(Ease.Linear));

        seq.SetLoops(-1, LoopType.Yoyo);     
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool isMaxPower = false;

    public void checkPlay()
    {
            DOTween.PauseAll();
            newPositionX = transform.position;
            float deltaX = newPositionX.x - positionX.x;
            Debug.Log($"Delta X: {deltaX:F2}");
            
            int temp = Mathf.RoundToInt((deltaX / 3.2f) * 100);
            temp = Mathf.Clamp(temp, 0, 100);
            Debug.Log($"Temp Value: {temp}%");
            
            
            if ( 41f <= temp && temp <= 58f)
            {
                isMaxPower = true;

                // Plane.instance.trailRendererPerfect.gameObject.SetActive(true);
                // Plane.instance.trailEffect.gameObject.SetActive(false);

                GManager.instance.isBonus = true;    
                Debug.Log(" Kích hoạt Max Power!");
                AudioManager.instance.PlaySound(AudioManager.instance.perfectAngleSoundClip);
                Settings.instance.imageDiamondCoinText.gameObject.SetActive(false);
                GManager.instance.newMapText.text = "Max Power Activated!";
                StartCoroutine(FadeOutText(2f));
                if(ExplosionScale.instance != null) ExplosionScale.instance.Explosion();
                if (EffectAirplane.instance != null) EffectAirplane.instance.MakePlaneGold();
                if (DestroyWheels.instance != null) DestroyWheels.instance.Golden();
                foreach (var propeller in EffectRotaryFront.instances)
                {
                    propeller.gameObject.GetComponent<Renderer>().material = Plane.instance.GoldMaterial;
                }
                if (Plane.instance.explosionEffect != null) Plane.instance.explosionEffect.Play();

                StartCoroutine(DelayTwoSeconds(1f));
                TrailRendererRight.instance.ChangeColor();
                if( TrailRendererLeft.instance != null ){TrailRendererLeft.instance.ChangeColor();}
            }
            else
            {
                isMaxPower = false;

                // Plane.instance.trailRendererPerfect.gameObject.SetActive(false);
                // Plane.instance.trailEffect.gameObject.SetActive(true);

                Debug.Log(" Không kích hoạt Max Power.");
                AudioManager.instance.PlaySound(AudioManager.instance.notPerfectAngleSoundClip);
                GManager.instance.isBonus = false;
                Settings.instance.imageDiamondCoinText.gameObject.SetActive(false);
                TrailRendererRight.instance.ChangeColor();
                if( TrailRendererLeft.instance != null ){TrailRendererLeft.instance.ChangeColor();}
            }
            
        }
    public void RandomTextMaxPower(){
        string[] texts = {
            "Exceed the limit = MAX POWER",
            "Push beyond to unlock MAX POWER",
            "Cross the threshold → MAX POWER",
            "High performance unlocks MAX POWER",
            "Go further to power up"
        };
        int randomIndex = Random.Range(0, texts.Length);
        GManager.instance.newMapText.text = texts[randomIndex];
        StartCoroutine(FadeOutText(2f));
    }

    IEnumerator DelayTwoSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        
    }

    IEnumerator FadeOutText(float duration)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("Bắt đầu Fade Out Text");
        
        GManager.instance.newMapText.DOFade(0f, 1f).OnComplete(() =>
        {
            Debug.Log("Text đã biến mất hoàn toàn!");
        });
    }
}
